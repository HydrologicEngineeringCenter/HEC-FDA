import jetbrains.buildServer.configs.kotlin.*
import jetbrains.buildServer.configs.kotlin.buildSteps.dotnetPublish
import jetbrains.buildServer.configs.kotlin.buildSteps.dotnetTest
import jetbrains.buildServer.configs.kotlin.buildSteps.powerShell
import jetbrains.buildServer.configs.kotlin.buildSteps.script
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

    buildType(SignExecutables)
    buildType(SetVersion)

    subProject(Deploy)
    subProject(Endpoints)
    subProject(Build)
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
        root(AbsoluteId("Consequences_HecFda"))

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
        root(AbsoluteId("Consequences_HecFda"))
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
    description = "Publishes the self-contained HEC.FDA.View win-x64 distribution. Mirrors the publish step of CI.yaml/Release.yml."

    artifactRules = "%PUBLISH_OUT_DIR% => HEC-FDA-%Version%"
    buildNumberPattern = "%Version%"

    params {
        param("env.RAS_GDAL", "%teamcity.build.checkoutDir%/%PUBLISH_OUT_DIR%/GDAL/")
        param("Version", "${SetVersion.depParamRefs["Version"]}")
        param("VersionShort", "${SetVersion.depParamRefs["VersionShort"]}")
        param("PUBLISH_OUT_DIR", "Distribution")
    }

    vcs {
        root(AbsoluteId("Consequences_HecFda"))
    }

    steps {
        script {
            name = "Configure NuGet private feed credentials"
            scriptContent = """dotnet nuget update source ras-nuget-private --username "%env.NEXUS_USER%" --password "%env.NEXUS_PASSWORD%" --store-password-in-clear-text --configfile nuget.config"""
        }
        powerShell {
            name = "Download and unzip GDAL"
            scriptMode = script {
                content = """
                    ${'$'}ErrorActionPreference = 'Stop'
                    
                    ${'$'}zipUrl  = "https://s3.hecdev.net/ras-public-data/ras-GDAL-3.9.1.zip"
                    ${'$'}zipPath = Join-Path "%teamcity.build.checkoutDir%" "downloaded.zip"
                    ${'$'}dest    = Join-Path "%teamcity.build.checkoutDir%" "%PUBLISH_OUT_DIR%"
                    
                    Write-Host "Downloading ${'$'}zipUrl"
                    Invoke-WebRequest -Uri ${'$'}zipUrl -OutFile ${'$'}zipPath
                    
                    Write-Host "Expanding to ${'$'}dest"
                    Expand-Archive -Path ${'$'}zipPath -DestinationPath ${'$'}dest -Force
                    
                    Remove-Item ${'$'}zipPath -Force
                """.trimIndent()
            }
        }
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
    description = "Runs the RunsOn=Remote test suite. Mirrors the test step of CI.yaml."

    buildNumberPattern = "%Version%"

    params {
        param("env.RAS_GDAL", "%teamcity.build.checkoutDir%/%PUBLISH_OUT_DIR%/GDAL/")
        param("Version", "${SetVersion.depParamRefs["Version"]}")
        param("PUBLISH_OUT_DIR", "Distribution")
    }

    vcs {
        root(AbsoluteId("Consequences_HecFda"))
    }

    steps {
        script {
            name = "Configure NuGet private feed credentials"
            scriptContent = """dotnet nuget update source ras-nuget-private --username "%env.NEXUS_USER%" --password "%env.NEXUS_PASSWORD%" --store-password-in-clear-text --configfile nuget.config"""
        }
        powerShell {
            name = "Download and unzip GDAL"
            scriptMode = script {
                content = """
                    ${'$'}ErrorActionPreference = 'Stop'
                    
                    ${'$'}zipUrl  = "https://s3.hecdev.net/ras-public-data/ras-GDAL-3.9.1.zip"
                    ${'$'}zipPath = Join-Path "%teamcity.build.checkoutDir%" "downloaded.zip"
                    ${'$'}dest    = Join-Path "%teamcity.build.checkoutDir%" "%PUBLISH_OUT_DIR%"
                    
                    Write-Host "Downloading ${'$'}zipUrl"
                    Invoke-WebRequest -Uri ${'$'}zipUrl -OutFile ${'$'}zipPath
                    
                    Write-Host "Expanding to ${'$'}dest"
                    Expand-Archive -Path ${'$'}zipPath -DestinationPath ${'$'}dest -Force
                    
                    Remove-Item ${'$'}zipPath -Force
                """.trimIndent()
            }
        }
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

    steps {
        script {
            name = "Upload distribution to Nexus"
            scriptContent = """
                #!/bin/bash
                set -euo pipefail
                
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
                HTTP=${'$'}(curl --silent --output /dev/null --write-out '%%{http_code}' --head \
                  -u "%env.NEXUS_USER%:%env.NEXUS_PASSWORD%" "${'$'}TARGET" || true)
                
                if [ "${'$'}HTTP" = "200" ] && [ "%ALLOW_OVERWRITE%" != "true" ]; then
                  echo "Refusing to overwrite an already-published release artifact:" >&2
                  echo "  ${'$'}TARGET" >&2
                  echo "Set ALLOW_OVERWRITE=true to replace it deliberately." >&2
                  exit 1
                fi
                
                echo "Uploading ${'$'}{ZIP} (${'$'}(du -h "${'$'}ZIP" | cut -f1)) to ${'$'}{TARGET}"
                
                # --fail-with-body makes curl exit non-zero on 4xx/5xx; without it a 401 would
                # still exit 0 and the build would go green on a failed upload.
                curl --fail-with-body --show-error --silent \
                  -u "%env.NEXUS_USER%:%env.NEXUS_PASSWORD%" \
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
    description = "Triggers the HEC-FDA build chain on pushes to main and pull requests. Mirrors CI.yaml."

    type = BuildTypeSettings.Type.DEPLOYMENT
    buildNumberPattern = "%Version%"

    params {
        param("Version", "${Build_Publish.depParamRefs["Version"]}")
    }

    vcs {
        root(AbsoluteId("Consequences_HecFda"))
    }

    triggers {
        vcs {
            triggerRules = "-:.teamcity/**"
            branchFilter = """
                +:main
                +:*/merge
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
    description = "Triggers the signed release chain on v*.*.* tags. Mirrors Release.yml."

    type = BuildTypeSettings.Type.DEPLOYMENT
    buildNumberPattern = "%Version%"

    params {
        param("Version", "${Deploy_PushToNexus.depParamRefs["Version"]}")
    }

    vcs {
        root(AbsoluteId("Consequences_HecFda"))
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
