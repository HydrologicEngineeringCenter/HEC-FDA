# HEC.FDA.TestingUtility

A command-line regression testing tool for HEC-FDA studies. It loads FDA study databases, runs computations (stage damage, scenarios, alternatives, and alternative comparisons), and produces a comprehensive CSV report of the results.

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

### 2. Create a configuration file

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
```

### 4. Check the output

After the run completes, look for `results_report.csv` in the output directory. It contains separate sections for scenario EAD, damage by category, performance metrics, alternative EqAD, stage damage summaries, and alternative comparison results.

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
dotnet run --project HEC.FDA.TestingUtility -- compute --config <path-to-config.json> [options]
```

### Options

| Option | Alias | Required | Description |
|---|---|---|---|
| `--config` | `-c` | Yes | Path to a JSON configuration file defining the studies and computations to run. |
| `--output` | `-o` | No | Output directory for generated files. Defaults to the current working directory. |
| `--study` | `-s` | No | Filter to one or more specific study IDs. Can be specified multiple times. |

### Examples

Run all studies defined in a config file:

```bash
dotnet run --project HEC.FDA.TestingUtility -- compute -c tests/regression.json -o results/
```

Run only a specific study:

```bash
dotnet run --project HEC.FDA.TestingUtility -- compute -c tests/regression.json -s "muncie"
```

## Configuration File

The configuration is a JSON file with the following structure:

```json
{
  "testSuiteId": "regression-v1",
  "globalSettings": {
    "localTempDirectory": "C:/temp/FDATests",
    "timeoutMinutes": 30
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
| `timeoutMinutes` | `30` | Maximum wall-clock time for all computations before cancellation. |

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

## How It Works

1. **Study Loading** - The study folder is copied from `networkSourcePath` to a local temp directory to avoid locking network files. The SQLite database is opened and all element types are loaded into the `StudyCache` in dependency order (terrains, impact areas, hydraulics, frequencies, inventories, stage damage, scenarios, alternatives, etc.).

2. **Computation** - Each computation is dispatched to the appropriate runner:
   - `StageDamageRunner` - Builds stage-damage configuration from hydraulics and inventory, then calls `ScenarioStageDamage.Compute()`.
   - `ScenarioRunner` - Creates `ImpactAreaScenarioSimulation` objects and runs `Scenario.Compute()` with the study's convergence criteria.
   - `AlternativeRunner` - Calls `Alternative.AnnualizationCompute()` using the base/future scenario results and study discount rate / period of analysis.
   - `AlternativeComparisonRunner` - Computes alternatives and then runs `AlternativeComparisonReport.ComputeAlternativeComparisonReport()`.

3. **Result Saving** - Computed results are saved back to the temp study database so that downstream computations (e.g., alternatives depending on scenario results) can access them.

4. **CSV Report** - A single `results_report.csv` file is written to the output directory containing sections for:
   - Scenario results (mean and percentile EAD by impact area)
   - Scenario damage by category
   - Scenario performance (long-term risk, AEP, assurance)
   - Scenario assurance of AEP
   - Alternative results (mean and percentile EqAD)
   - Alternative damage by category
   - Stage damage summary (point counts, stage ranges, median integrals)
   - Alternative comparison summary (EqAD reduced, base/future EAD reduced)
   - Alternative comparison by damage category

5. **Cleanup** - When a study finishes, the temp copy is deleted automatically.

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
