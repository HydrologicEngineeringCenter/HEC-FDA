import jetbrains.buildServer.configs.kotlin.*
import jetbrains.buildServer.configs.kotlin.buildSteps.dotnetPublish
import jetbrains.buildServer.configs.kotlin.buildSteps.dotnetTest
import jetbrains.buildServer.configs.kotlin.buildSteps.powerShell
import jetbrains.buildServer.configs.kotlin.buildSteps.script
import jetbrains.buildServer.configs.kotlin.triggers.schedule
import jetbrains.buildServer.configs.kotlin.triggers.vcs

/*
The settings script is an entry point for defining a TeamCity
project hierarchy. The script should contain a single call to the
project() function with a Project instance or an init function as
an argument.

VcsRoots, BuildTypes, Templates, and subprojects can be
registered inside the project using the vcsRoot(), buildType(),
template(), and subProject() methods respectively.

To debug settings scripts in command-line, run the

    mvnDebug org.jetbrains.teamcity:teamcity-configs-maven-plugin:generate

command and attach your debugger to the port 8000.

To debug in IntelliJ Idea, open the 'Maven Projects' tool window (View
-> Tool Windows -> Maven Projects), find the generate task node
(Plugins -> teamcity-configs -> teamcity-configs:generate), the
'Debug' option is available in the context menu for the task.
*/

version = "2026.1"

project {

    // Secrets consumed by this project. Every one of these must be declared as a
    // Password-type parameter in TeamCity (on this project or the parent) so the value
    // is masked in build logs. The DSL cannot enforce the type of an inherited parameter,
    // so verify it in the UI whenever a secret is added or rotated.
    //   env.NEXUS_USER, env.NEXUS_PASSWORD  attached per build configuration by NexusCredentials
    //                                       below; Nexus NuGet feed (read) and fda-releases (write)
    //   env.FDA_READ_ONLY_PAT               Bitbucket repository-scoped read token (Mirror to GitHub)
    //   env.GITHUB_MIRROR_TOKEN             GitHub token for the public mirror (Mirror to GitHub)
    //
    // Versioned Settings for this project must stay in read-only "sync from VCS" mode with
    // "apply settings from the branch being built" left off. CI builds pull-request branches
    // with the Nexus credentials in the environment; applying DSL from those branches would
    // let a pull request rewrite this pipeline.

    params {
        // GDAL runtime bundle unpacked into the distribution. The SHA-256 is pinned so a
        // modified upload to the bucket fails the build instead of shipping in a signed release.
        // When updating GDAL, download the new zip and record its hash here in the same change.
        param("gdal.zip.url", "https://s3.hecdev.net/ras-public-data/ras-GDAL-3.9.1.zip")
        param("gdal.zip.sha256", "99df8bd72b76f59b2f3ddc0f2d2f04d5fe4bdfcf715addf4302525ef7a1ac5a1")

        // Password-type parameter added in the TeamCity UI; the credentialsJSON value is an
        // opaque reference to the secret stored on the server, not the secret itself.
        password("env.FDA_READ_ONLY_PAT", "credentialsJSON:3877294f-82ce-4df0-9e27-091e5b4cd1ff")
    }

    buildType(SignExecutables)
    buildType(SetVersion)
    buildType(MirrorToGitHub)

    subProject(Deploy)
    subProject(Endpoints)
    subProject(Build)
}

/**
 * Nexus credentials, attached only to the build configurations that talk to Nexus (feed
 * restore in Build/Test and Build/Publish, upload in Deploy/Push to Nexus). They are deliberately
 * not project-level parameters: a project-level env. parameter is injected into every build in
 * the project, including ones that never touch Nexus. The credentialsJSON value is an opaque
 * reference to the secret stored on the server for this project, not the secret itself.
 *
 * NuGet reads credentials for a package source from the environment variable
 * NuGetPackageSourceCredentials_<source name>, where <source name> is the key in nuget.config.
 * This replaces `dotnet nuget update source --store-password-in-clear-text`, which wrote the
 * password into nuget.config inside the checkout directory. Only the Windows agents consume it,
 * and '-' is a valid character in a Windows variable name.
 */
object NexusCredentials {
    fun attach(buildType: BuildType) = buildType.params {
        param("env.NEXUS_USER", "bbeam")
        password("env.NEXUS_PASSWORD", "credentialsJSON:93766cea-6722-458b-933b-d40045f7ff10")
        param("env.NuGetPackageSourceCredentials_ras-nuget-private", "Username=%env.NEXUS_USER%;Password=%env.NEXUS_PASSWORD%")
    }
}

/**
 * Downloads the pinned GDAL bundle, verifies it against gdal.zip.sha256, and unpacks it into
 * PUBLISH_OUT_DIR. Shared by Build/Test and Build/Publish so the check cannot drift between them.
 *
 * Lives in an object rather than as a top-level function: in a .kts script a top-level function
 * makes every object that calls it capture the script instance, which the TeamCity DSL runner
 * rejects ("Object Build_Publish captures the script class instance").
 */
object Gdal {
    fun downloadAndVerify(steps: BuildSteps) = steps.powerShell {
        name = "Download and verify GDAL"
        scriptMode = script {
            content = """
                ${'$'}ErrorActionPreference = 'Stop'

                ${'$'}zipUrl   = "%gdal.zip.url%"
                ${'$'}expected = "%gdal.zip.sha256%".ToLowerInvariant()
                ${'$'}zipPath  = Join-Path "%teamcity.build.checkoutDir%" "downloaded.zip"
                ${'$'}dest     = Join-Path "%teamcity.build.checkoutDir%" "%PUBLISH_OUT_DIR%"

                Write-Host "Downloading ${'$'}zipUrl"
                Invoke-WebRequest -Uri ${'$'}zipUrl -OutFile ${'$'}zipPath

                ${'$'}actual = (Get-FileHash -Path ${'$'}zipPath -Algorithm SHA256).Hash.ToLowerInvariant()
                if (${'$'}actual -ne ${'$'}expected) {
                    Remove-Item ${'$'}zipPath -Force
                    throw "GDAL bundle checksum mismatch. Expected ${'$'}expected but downloaded file is ${'$'}actual. Refusing to unpack."
                }
                Write-Host "SHA-256 verified: ${'$'}actual"

                Write-Host "Expanding to ${'$'}dest"
                Expand-Archive -Path ${'$'}zipPath -DestinationPath ${'$'}dest -Force

                Remove-Item ${'$'}zipPath -Force
            """.trimIndent()
        }
    }
}

object SetVersion : BuildType({
    name = "Set Version"
    description = "Computes Version from branch context: a v* tag yields the tag without its leading v, anything else yields 2.1.0.<counter>-Beta. No triggers - pulled into chains via snapshot dependency."

    buildNumberPattern = "%Version%"

    params {
        param("Version", "")
        param("VersionShort", "")
    }

    vcs {
        root(DslContext.settingsRoot)

        branchFilter = """
            +:<default>
            +:refs/tags/v*
            +:*
        """.trimIndent()
    }

    steps {
        powerShell {
            name = "Compute Version"
            scriptMode = script {
                content = """
                    ${'$'}branch = "%teamcity.build.branch%"
                    
                    if (${'$'}branch -match '^v') {
                        # Release tag: strip the leading 'v' (v2.3.1 -> 2.3.1)
                        ${'$'}version = ${'$'}branch -replace '^v', ''
                    } else {
                        ${'$'}version = "2.1.0.%build.counter%-Beta"
                    }
                    
                    # Abbreviated form used for the portable zip filename:
                    #   major+minor, patch appended only when non-zero, 4th component appended when present.
                    #   2.1.0 -> 21 | 2.0.2 -> 202 | 2.0.0 -> 20 | 2.1.0.1534 -> 21_1534 | 2.1.0.3-Beta -> 21_3-Beta
                    ${'$'}parts = ${'$'}version -split '\.'
                    ${'$'}short = "${'$'}(${'$'}parts[0])${'$'}(${'$'}parts[1])"
                    if (${'$'}parts.Count -ge 3 -and ${'$'}parts[2] -ne '0') { ${'$'}short += ${'$'}parts[2] }
                    if (${'$'}parts.Count -ge 4) { ${'$'}short += "_${'$'}(${'$'}parts[3])" }
                    
                    Write-Host "Branch:       ${'$'}branch"
                    Write-Host "Version:      ${'$'}version"
                    Write-Host "VersionShort: ${'$'}short"
                    
                    Write-Host "##teamcity[setParameter name='Version' value='${'$'}version']"
                    Write-Host "##teamcity[setParameter name='VersionShort' value='${'$'}short']"
                    Write-Host "##teamcity[buildNumber '${'$'}version']"
                """.trimIndent()
            }
        }
    }
})

object MirrorToGitHub : BuildType({
    name = "Mirror to GitHub"
    description = """
        Pushes every branch and tag from the canonical Bitbucket repository to the public GitHub mirror.
        Runs on every push (any branch or tag) and nightly, so anything pushed directly to GitHub is
        overwritten within a day. Pushes are forced and pruned with explicit refs/heads and refs/tags
        refspecs, so the mirror always matches Bitbucket exactly.
        Requires two project-level secure parameters: env.FDA_READ_ONLY_PAT (a repository-scoped
        Bitbucket HTTP access token with read access to hec-fda) and env.GITHUB_MIRROR_TOKEN (a token
        with Contents read/write on the mirror that is allowed to force-push and delete branches).
    """.trimIndent()

    maxRunningBuilds = 1

    params {
        param("github.mirror.url", "https://github.com/HydrologicEngineeringCenter/HEC-FDA.git")
        // Explicit rather than derived from the VCS root's URL so the mirror source cannot drift
        // if the shared VCS root is ever changed or repointed.
        param("bitbucket.source.url", "https://bitbucket.hecdev.net/scm/con/hec-fda.git")
    }

    vcs {
        root(DslContext.settingsRoot)

        // The VCS root is attached only so TeamCity detects changes and triggers the build.
        // The script maintains its own bare clone with every ref, which the normal checkout
        // (a single branch, no tags) cannot provide.
        checkoutMode = CheckoutMode.MANUAL

        branchFilter = """
            +:*
            +:refs/tags/*
        """.trimIndent()
    }

    steps {
        script {
            name = "Mirror all branches and tags to GitHub"
            scriptContent = """
                #!/bin/bash
                set -euo pipefail

                : "${'$'}{FDA_READ_ONLY_PAT:?Set project parameter env.FDA_READ_ONLY_PAT}"
                : "${'$'}{GITHUB_MIRROR_TOKEN:?Set project parameter env.GITHUB_MIRROR_TOKEN}"

                SOURCE_URL="%bitbucket.source.url%"
                MIRROR_URL="%github.mirror.url%"
                MIRROR_DIR="%teamcity.build.checkoutDir%/mirror.git"

                # Credentials are handed to git through inline credential helpers that read the
                # environment, so tokens never appear in remote URLs, git config, or the build log.
                # Bitbucket repository HTTP access tokens are sent as the basic-auth password and
                # Bitbucket ignores the username; GitHub expects x-access-token.
                SOURCE_CRED='!f() { echo "username=x-token-auth"; echo "password=${'$'}FDA_READ_ONLY_PAT"; }; f'
                MIRROR_CRED='!f() { echo "username=x-access-token"; echo "password=${'$'}GITHUB_MIRROR_TOKEN"; }; f'

                # Keep a persistent bare clone on the agent so each run only fetches what changed.
                if [ ! -d "${'$'}MIRROR_DIR" ]; then
                  echo "Creating bare clone of ${'$'}SOURCE_URL"
                  git -c credential.helper= -c "credential.helper=${'$'}SOURCE_CRED" \
                    clone --bare "${'$'}SOURCE_URL" "${'$'}MIRROR_DIR"
                fi

                cd "${'$'}MIRROR_DIR"
                git remote set-url origin "${'$'}SOURCE_URL"

                echo "Fetching all branches and tags from Bitbucket"
                git -c credential.helper= -c "credential.helper=${'$'}SOURCE_CRED" \
                  fetch --prune --prune-tags --force origin \
                    '+refs/heads/*:refs/heads/*' \
                    '+refs/tags/*:refs/tags/*'

                echo "Refs to mirror:"
                git for-each-ref --format='  %%(refname)' refs/heads refs/tags

                # Force-push with explicit refspecs and --prune so the GitHub mirror ends up
                # identical to Bitbucket: rewritten, added, or deleted branches and tags on
                # GitHub are all corrected, undoing any direct changes made there.
                echo "Pushing to ${'$'}MIRROR_URL"
                git -c credential.helper= -c "credential.helper=${'$'}MIRROR_CRED" \
                  push --prune --force "${'$'}MIRROR_URL" \
                    '+refs/heads/*:refs/heads/*' \
                    '+refs/tags/*:refs/tags/*'

                echo "Mirror is in sync with Bitbucket."
            """.trimIndent()
        }
    }

    triggers {
        // Every push to any branch or tag on Bitbucket.
        vcs {
            branchFilter = """
                +:*
                +:refs/tags/*
            """.trimIndent()
        }
        // Nightly full sync, even with no new commits, to undo anything pushed
        // directly to GitHub since the last run.
        schedule {
            schedulingPolicy = daily {
                hour = 2
                minute = 0
            }
            branchFilter = "+:<default>"
            triggerBuild = always()
            withPendingChangesOnly = false
        }
    }

    requirements {
        contains("teamcity.agent.name", "linux")
    }
})

object SignExecutables : BuildType({
    templates(AbsoluteId("SignExecutables"))
    name = "Sign Binaries"
    description = """Signs the HEC-FDA distribution produced by Build, then packages it as HEC-FDA-<version>.zip. Uses the shared Root-level "Sign Binaries" template."""

    artifactRules = "%TO_BE_SIGNED_DIR% => HEC-FDA_%VersionShort%_Portable.zip!/HEC-FDA-%Version%"
    buildNumberPattern = "%Version%"

    params {
        param("Version", "${Build_Publish.depParamRefs["Version"]}")
        param("sign.filePatterns", "'HEC.FDA.View.exe' 'HEC.*.dll' 'Hec.*.dll' 'HecCs.dll' 'hecdss.dll' 'Geospatial.*.dll' 'H5Assist.dll' 'PipeClient.dll' 'PlottingLibrary*.dll' 'Ras.*.dll' 'Tiff*.dll' 'Utility.*.dll' 'Visual.*.dll'")
        param("VersionShort", "${Build_Publish.depParamRefs["VersionShort"]}")
    }

    vcs {
        root(DslContext.settingsRoot)
    }

    dependencies {
        dependency(Build_Publish) {
            snapshot {
                reuseBuilds = ReuseBuilds.NO
                onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                id = "ARTIFACT_DEPENDENCY_20"
                artifactRules = "HEC-FDA-%Version%/** => %TO_BE_SIGNED_DIR%"
            }
        }
        snapshot(Build_Test) {
            reuseBuilds = ReuseBuilds.NO
            onDependencyFailure = FailureAction.FAIL_TO_START
        }
    }
})


object Build : Project({
    name = "Build"

    buildType(Build_Test)
    buildType(Build_Publish)
})

object Build_Publish : BuildType({
    name = "Publish"
    description = "Publishes the self-contained HEC.FDA.View win-x64 distribution. Replaces the publish step of the former CI.yaml/Release.yml GitHub Actions workflows."

    artifactRules = "%PUBLISH_OUT_DIR% => HEC-FDA-%Version%"
    buildNumberPattern = "%Version%"

    params {
        param("env.RAS_GDAL", "%teamcity.build.checkoutDir%/%PUBLISH_OUT_DIR%/GDAL/")
        param("Version", "${SetVersion.depParamRefs["Version"]}")
        param("VersionShort", "${SetVersion.depParamRefs["VersionShort"]}")
        param("PUBLISH_OUT_DIR", "Distribution")
    }

    NexusCredentials.attach(this)

    vcs {
        root(DslContext.settingsRoot)
    }

    steps {
        Gdal.downloadAndVerify(this)
        dotnetPublish {
            name = "Publish"
            projects = "HEC.FDA.View/HEC.FDA.View.csproj"
            configuration = "Release"
            runtime = "win-x64"
            outputDir = "%PUBLISH_OUT_DIR%"
            args = "-v quiet /p:Version=%Version% --self-contained true"
        }
    }

    dependencies {
        snapshot(SetVersion) {
            reuseBuilds = ReuseBuilds.NO
            onDependencyFailure = FailureAction.FAIL_TO_START
        }
    }

    requirements {
        contains("teamcity.agent.name", "windows")
    }
})

object Build_Test : BuildType({
    name = "Test"
    description = "Runs the RunsOn=Remote test suite. Replaces the test step of the former CI.yaml GitHub Actions workflow."

    buildNumberPattern = "%Version%"

    params {
        param("env.RAS_GDAL", "%teamcity.build.checkoutDir%/%PUBLISH_OUT_DIR%/GDAL/")
        param("Version", "${SetVersion.depParamRefs["Version"]}")
        param("PUBLISH_OUT_DIR", "Distribution")
    }

    NexusCredentials.attach(this)

    vcs {
        root(DslContext.settingsRoot)
    }

    steps {
        Gdal.downloadAndVerify(this)
        dotnetTest {
            name = "Test Solution"
            configuration = "Release"
            args = "--nologo --filter RunsOn=Remote"
        }
    }

    dependencies {
        snapshot(SetVersion) {
            reuseBuilds = ReuseBuilds.NO
            onDependencyFailure = FailureAction.FAIL_TO_START
        }
    }

    requirements {
        contains("teamcity.agent.name", "windows")
    }
})


object Deploy : Project({
    name = "Deploy"

    buildType(Deploy_PushToNexus)
})

object Deploy_PushToNexus : BuildType({
    name = "Push to Nexus"
    description = "Uploads the signed HEC-FDA distribution zip to the fda-releases raw Nexus repository. Tag builds only - reached via the Release endpoint."

    type = BuildTypeSettings.Type.DEPLOYMENT
    buildNumberPattern = "%Version%"

    params {
        param("nexus.raw.url", "https://www.hec.usace.army.mil/nexus/repository")
        param("ALLOW_OVERWRITE", "false")
        param("nexus.raw.repo", "fda-releases")
        param("Version", "${SignExecutables.depParamRefs["Version"]}")
        param("VersionShort", "${SignExecutables.depParamRefs["VersionShort"]}")
    }

    NexusCredentials.attach(this)

    steps {
        script {
            name = "Upload distribution to Nexus"
            scriptContent = """
                #!/bin/bash
                set -euo pipefail
                
                # Nexus credentials are env. parameters, read from the environment here rather than
                # substituted into the script text, and handed to curl through a config read from
                # stdin. They never appear on a command line, so they are not visible in the agent's
                # process table while curl runs.
                : "${'$'}{NEXUS_USER:?Set project parameter env.NEXUS_USER}"
                : "${'$'}{NEXUS_PASSWORD:?Set project parameter env.NEXUS_PASSWORD}"
                nexus_curl() {
                  curl --config - "${'$'}@" <<EOF
                user = "${'$'}{NEXUS_USER}:${'$'}{NEXUS_PASSWORD}"
                EOF
                }

                ZIP="HEC-FDA_%VersionShort%_Portable.zip"
                # Nexus path keeps the full, precise version so artifacts stay uniquely addressable.
                TARGET="%nexus.raw.url%/%nexus.raw.repo%/HEC-FDA/%Version%/${'$'}{ZIP}"
                
                if [ ! -f "${'$'}ZIP" ]; then
                  echo "Expected artifact not found: ${'$'}ZIP" >&2
                  exit 1
                fi
                
                # Releases are immutable. A VCS trigger can re-fire on an already-built tag, so
                # refuse to overwrite a published artifact rather than silently replacing a
                # signed release. Set ALLOW_OVERWRITE=true on the build to deliberately replace.
                HTTP=${'$'}(nexus_curl --silent --output /dev/null --write-out '%%{http_code}' \
                  --head "${'$'}TARGET" || true)
                
                if [ "${'$'}HTTP" = "200" ] && [ "%ALLOW_OVERWRITE%" != "true" ]; then
                  echo "Refusing to overwrite an already-published release artifact:" >&2
                  echo "  ${'$'}TARGET" >&2
                  echo "Set ALLOW_OVERWRITE=true to replace it deliberately." >&2
                  exit 1
                fi
                
                echo "Uploading ${'$'}{ZIP} (${'$'}(du -h "${'$'}ZIP" | cut -f1)) to ${'$'}{TARGET}"
                
                # --fail-with-body makes curl exit non-zero on 4xx/5xx; without it a 401 would
                # still exit 0 and the build would go green on a failed upload.
                nexus_curl --fail-with-body --show-error --silent \
                  --upload-file "${'$'}ZIP" \
                  "${'$'}TARGET"
                
                echo "Upload complete: ${'$'}{TARGET}"
            """.trimIndent()
        }
    }

    dependencies {
        dependency(SignExecutables) {
            snapshot {
                reuseBuilds = ReuseBuilds.NO
                onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                cleanDestination = true
                artifactRules = "HEC-FDA_%VersionShort%_Portable.zip"
            }
        }
    }

    requirements {
        contains("teamcity.agent.name", "linux")
    }
})


object Endpoints : Project({
    name = "Endpoints"

    buildType(Endpoints_CI)
    buildType(Endpoints_Release)
})

object Endpoints_CI : BuildType({
    name = "CI"
    description = "Triggers the HEC-FDA build chain on pushes to main and pull requests. Replaces the former CI.yaml GitHub Actions workflow."

    type = BuildTypeSettings.Type.DEPLOYMENT
    buildNumberPattern = "%Version%"

    params {
        param("Version", "${Build_Publish.depParamRefs["Version"]}")
    }

    vcs {
        root(DslContext.settingsRoot)
    }

    triggers {
        vcs {
            triggerRules = "-:.teamcity/**"
            branchFilter = """
                +:main
                +:*/from
            """.trimIndent()
        }
    }

    dependencies {
        snapshot(Build_Publish) {
            reuseBuilds = ReuseBuilds.NO
            onDependencyFailure = FailureAction.FAIL_TO_START
        }
        snapshot(Build_Test) {
            reuseBuilds = ReuseBuilds.NO
            onDependencyFailure = FailureAction.FAIL_TO_START
        }
    }
})

object Endpoints_Release : BuildType({
    name = "Release"
    description = "Triggers the signed release chain on v*.*.* tags. Replaces the former Release.yml GitHub Actions workflow."

    type = BuildTypeSettings.Type.DEPLOYMENT
    buildNumberPattern = "%Version%"

    params {
        param("Version", "${Deploy_PushToNexus.depParamRefs["Version"]}")
    }

    vcs {
        root(DslContext.settingsRoot)
    }

    triggers {
        vcs {
            triggerRules = "-:.teamcity/**"
            branchFilter = "+:v*"
        }
    }

    dependencies {
        snapshot(Deploy_PushToNexus) {
            reuseBuilds = ReuseBuilds.NO
            onDependencyFailure = FailureAction.FAIL_TO_START
        }
    }
})
