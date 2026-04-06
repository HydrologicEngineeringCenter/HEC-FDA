# HEC.FDA.TestingUtility

A command-line regression testing tool for HEC-FDA studies. It loads FDA study databases, runs computations (stage damage, scenarios, alternatives, and alternative comparisons), and produces comprehensive CSV reports of the results.

## Purpose

This utility runs FDA computations outside the GUI so that results can be validated programmatically. Typical use cases include:

- **Regression testing** - Run computations on known studies and compare the CSV output against baseline results to detect unintended changes.
- **Batch computation** - Execute all computations in one or more studies without manual interaction.
- **CI/CD integration** - Automate computation verification as part of a build pipeline.

## Quick Start

### 1. Build the project

```bash
dotnet build HEC.FDA.TestingUtility/HEC.FDA.TestingUtility.csproj
```

### 2a. Run directly against a study file (simplest)

If you just want to run all computations on a single study, point directly at its `.sqlite` file:

```bash
dotnet run --project HEC.FDA.TestingUtility -- compute -sp "C:/path/to/my/study/MyStudy.sqlite" -o results
```

This auto-discovers and runs all stage damage, scenario, alternative, and alternative comparison elements in the study. Output defaults to the study's directory if `-o` is not specified.

### 2b. Run with a configuration file (multiple studies or selective runs)

Copy `example-config.json` from this project directory and update `networkSourcePath` to point to your FDA study folder (the folder containing the `.sqlite` or `.db` file):

```json
{
  "testSuiteId": "quick-start",
  "globalSettings": {
    "localTempDirectory": "C:/temp/FDATests",
    "timeoutMinutes": 60
  },
  "studies": [
    {
      "studyId": "my-study",
      "studyName": "My Study",
      "networkSourcePath": "C:/path/to/my/study/folder",
      "runAllStageDamage": true,
      "runAllScenarios": true,
      "runAllAlternatives": true,
      "runAllAlternativeComparisons": true,
      "computations": []
    }
  ]
}
```

Setting the `runAll*` flags to `true` auto-discovers every element of that type in the study, so you don't need to list them individually. If you only want to run specific elements, set the flags to `false` and use `computations` instead:

```json
{
  "testSuiteId": "targeted-run",
  "globalSettings": {},
  "studies": [
    {
      "studyId": "my-study",
      "studyName": "My Study",
      "networkSourcePath": "C:/path/to/my/study/folder",
      "runAllStageDamage": false,
      "runAllScenarios": false,
      "runAllAlternatives": false,
      "runAllAlternativeComparisons": false,
      "computations": [
        { "type": "stagedamage", "elementName": "My Stage Damage" },
        { "type": "scenario",    "elementName": "Existing Conditions" },
        { "type": "alternative", "elementName": "Proposed Levee" }
      ]
    }
  ]
}
```

You can list computations in any order -- the utility automatically sorts them by dependency (stage damage first, then scenarios, then alternatives, then alternative comparisons).

### 3. Run the utility

```bash
# Run all studies in the config, write results to a "results" folder
dotnet run --project HEC.FDA.TestingUtility -- compute -c my-test.json -o results

# Run only a single study by its studyId
dotnet run --project HEC.FDA.TestingUtility -- compute -c my-test.json -o results -s "my-study"

# Filter to multiple studies
dotnet run --project HEC.FDA.TestingUtility -- compute -c my-test.json -s "study-a" -s "study-b"

# Run directly against a single .sqlite file
dotnet run --project HEC.FDA.TestingUtility -- compute -sp "C:/Studies/Muncie/Muncie.sqlite" -o results
```

### 4. Check the output

After the run completes, the output directory will contain:

```
results/
  results_report.csv                          # Global summary across all studies
  muncie/
    results_report.csv                        # Per-study report for Muncie
    StructureDetails/
      My Stage Damage_StructureStageDamageDetails.csv
      My Stage Damage_DamagedElementCountsByStage.csv
  greenbrook/
    results_report.csv                        # Per-study report for Greenbrook
    StructureDetails/
      ...
```

- **`results_report.csv`** (root) - Combined report across all studies with sections for scenario EAD, damage by category, performance metrics, alternative EqAD, stage damage summaries, and alternative comparison results.
- **`{studyId}/results_report.csv`** - Same report format, but scoped to a single study.
- **`{studyId}/StructureDetails/`** - Generated for computed (non-manual) stage damage elements. Contains per-structure stage-damage details and damaged element counts by stage.

The console output will also show a summary:

```
=== Summary ===
Completed: 5
Errors:    0
Duration:  2m 34.1s

CSV report saved to: results/results_report.csv
```

An exit code of `0` means all computations passed. An exit code of `1` means at least one failed -- scroll up in the console output to find lines marked `ERROR`.

## Prerequisites

- .NET 9.0 SDK (Windows, targets `net9.0-windows`)
- Access to FDA study databases (`.sqlite` or `.db` files) located on a network share or local directory
- NuGet sources configured as described in the root `CLAUDE.md`

## Building

```bash
dotnet build HEC.FDA.TestingUtility/HEC.FDA.TestingUtility.csproj
```

## Usage

The utility uses the `System.CommandLine` library and exposes a `compute` subcommand:

```bash
dotnet run --project HEC.FDA.TestingUtility -- compute [options]
```

You must provide either `--config` or `--study-path` (but not both).

### Options

| Option | Alias | Description |
|---|---|---|
| `--config` | `-c` | Path to a JSON configuration file defining the studies and computations to run. Required unless `--study-path` is used. |
| `--study-path` | `-sp` | Path to a `.sqlite` study file. Runs all computations (stage damage, scenarios, alternatives, alternative comparisons) automatically. Required unless `--config` is used. |
| `--output` | `-o` | Output directory for generated files. Defaults to the current working directory (or the study directory when using `--study-path`). |
| `--study` | `-s` | Filter to one or more specific study IDs. Can be specified multiple times. Only applies when using `--config`. |

### Examples

Run all studies defined in a config file:

```bash
dotnet run --project HEC.FDA.TestingUtility -- compute -c tests/regression.json -o results/
```

Run only a specific study:

```bash
dotnet run --project HEC.FDA.TestingUtility -- compute -c tests/regression.json -s "muncie"
```

Run a single study directly without a config file:

```bash
dotnet run --project HEC.FDA.TestingUtility -- compute -sp "E:/Studies/Muncie/Muncie.sqlite" -o results/
```

Run all case studies from a directory (see `example-directory.json`):

```bash
dotnet run --project HEC.FDA.TestingUtility -- compute -c example-directory.json -o results/
```

## Configuration File

The configuration is a JSON file with the following structure:

```json
{
  "testSuiteId": "regression-v1",
  "globalSettings": {
    "localTempDirectory": "C:/temp/FDATests",
    "timeoutMinutes": 120
  },
  "studies": [
    {
      "studyId": "muncie",
      "studyName": "Muncie Indiana",
      "networkSourcePath": "\\\\server\\share\\studies\\Muncie",
      "runAllStageDamage": true,
      "runAllScenarios": true,
      "runAllAlternatives": true,
      "runAllAlternativeComparisons": true,
      "computations": []
    }
  ]
}
```

### Global Settings

| Field | Default | Description |
|---|---|---|
| `localTempDirectory` | System temp + `FDATests` | Directory where studies are copied locally before computation. |
| `timeoutMinutes` | `120` | Maximum wall-clock time for all computations before cancellation. Set to `0` to disable the timeout. |

### Study Configuration

| Field | Description |
|---|---|
| `studyId` | Short identifier used in the CSV report and for the `--study` filter. |
| `studyName` | Human-readable name printed during execution. |
| `networkSourcePath` | Path to the study folder containing the `.sqlite` or `.db` database file. |
| `runAllStageDamage` | Auto-discover and run all stage damage elements in the study. |
| `runAllScenarios` | Auto-discover and run all scenario elements. |
| `runAllAlternatives` | Auto-discover and run all alternative elements. |
| `runAllAlternativeComparisons` | Auto-discover and run all alternative comparison report elements. |
| `computations` | Explicit list of computations (see below). Combined with auto-discovered elements. |

### Computation Entry

Each entry in the `computations` array targets a specific element:

```json
{
  "type": "scenario",
  "elementName": "Existing Conditions"
}
```

Valid `type` values (case-insensitive):

| Type | Description |
|---|---|
| `stagedamage` | Computes stage-damage curves from hydraulics and inventory data. |
| `scenario` | Runs a Monte Carlo scenario simulation using convergence criteria from study properties. |
| `alternative` | Computes annualized damages for an alternative using its base and future scenario results. |
| `alternativecomparison` | Compares with-project alternatives against a without-project alternative. |

Computations are automatically sorted in dependency order: stage damage -> scenario -> alternative -> alternative comparison. This means you can list them in any order and the utility will execute them correctly.

## Example Configuration Files

| File | Description |
|---|---|
| `example-config.json` | Minimal template for a single study with all `runAll*` flags enabled. |
| `example-directory.json` | Template for running all studies found as subfolders under a common parent directory. |
| `all_case_studies.json` | Real-world config targeting 9 case studies with all computations. |
| `sunnyvale_alt_only.json` | Example of a selective run (alternatives only). |

## How It Works

1. **Study Loading** - The study folder is copied from `networkSourcePath` to a local temp directory to avoid locking network files. The SQLite database is opened and all element types are loaded into the `StudyCache` in dependency order (terrains, impact areas, hydraulics, frequencies, inventories, stage damage, scenarios, alternatives, etc.).

2. **Computation** - Each computation is dispatched to the appropriate runner:
   - `StageDamageRunner` - Builds stage-damage configuration from hydraulics and inventory, then calls `ScenarioStageDamage.Compute()`.
   - `ScenarioRunner` - Creates `ImpactAreaScenarioSimulation` objects and runs `Scenario.Compute()` with the study's convergence criteria.
   - `AlternativeRunner` - Calls `Alternative.AnnualizationCompute()` using the base/future scenario results and study discount rate / period of analysis.
   - `AlternativeComparisonRunner` - Computes alternatives and then runs `AlternativeComparisonReport.ComputeAlternativeComparisonReport()`.

3. **Result Saving** - Computed results are saved back to the temp study database so that downstream computations (e.g., alternatives depending on scenario results) can access them.

4. **CSV Report** - A `results_report.csv` file is written both globally (to the output directory) and per-study (to `{outputDir}/{studyId}/`). Each report contains sections for:
   - Scenario results (mean and percentile EAD by impact area)
   - Scenario damage by category
   - Scenario performance (long-term risk, AEP, assurance)
   - Scenario assurance of AEP
   - Alternative results (mean and percentile EqAD)
   - Alternative damage by category
   - Stage damage summary (point counts, stage ranges, median integrals)
   - Alternative comparison summary (EqAD reduced, base/future EAD reduced)
   - Alternative comparison by damage category

5. **Structure Details** - For computed (non-manual) stage damage elements, two additional CSVs are written to `{outputDir}/{studyId}/StructureDetails/`:
   - `{elementName}_StructureStageDamageDetails.csv` - Per-structure stage-damage detail curves.
   - `{elementName}_DamagedElementCountsByStage.csv` - Number of damaged elements at each stage.

6. **Cleanup** - When a study finishes, the temp copy is deleted automatically.

## Project Structure

```
HEC.FDA.TestingUtility/
  Program.cs                              # CLI entry point (System.CommandLine)
  ComputeRunner.cs                        # Orchestrates study loading, computation, and reporting
  Configuration/
    TestConfiguration.cs                  # JSON config deserialization models
  Services/
    StudyLoader.cs                        # Copies study to temp, opens DB, loads all elements
    StageDamageRunner.cs                  # Stage damage computation
    ScenarioRunner.cs                     # Scenario computation + element lookup helpers
    AlternativeRunner.cs                  # Alternative annualization computation
    AlternativeComparisonRunner.cs        # Alternative comparison report computation
  Reporting/
    CsvReportFactory.cs                   # Builds the multi-section CSV report
```

## Exit Codes

| Code | Meaning |
|---|---|
| `0` | All computations completed successfully. |
| `1` | One or more computations failed, or the configuration was invalid. |
