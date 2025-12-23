using System.Diagnostics;
using System.Xml.Linq;
using HEC.FDA.Model.metrics;
using HEC.FDA.TestingUtility.Comparison;
using HEC.FDA.TestingUtility.Configuration;
using HEC.FDA.TestingUtility.Services;
using HEC.FDA.ViewModel;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.ImpactAreaScenario;

namespace HEC.FDA.TestingUtility;

public class TestRunner
{
    private readonly TestConfiguration _config;
    private readonly string _outputDir;
    private readonly bool _verbose;
    private readonly string[]? _studyFilter;
    private readonly CancellationTokenSource _cts;

    private readonly XmlResultComparer _comparer = new();
    private readonly StudyBaselineWriter _baselineWriter = new();
    private readonly ScenarioRunner _scenarioRunner = new();
    private readonly AlternativeRunner _alternativeRunner = new();
    private readonly StageDamageRunner _stageDamageRunner = new();

    public TestRunner(TestConfiguration config, string outputDir, bool verbose, string[]? studyFilter)
    {
        _config = config;
        _outputDir = outputDir;
        _verbose = verbose;
        _studyFilter = studyFilter;
        _cts = new CancellationTokenSource();

        // Set timeout
        if (_config.GlobalSettings.TimeoutMinutes > 0)
        {
            _cts.CancelAfter(TimeSpan.FromMinutes(_config.GlobalSettings.TimeoutMinutes));
        }
    }

    public async Task<int> RunAsync()
    {
        int failures = 0;
        int passed = 0;
        var totalStopwatch = Stopwatch.StartNew();
        var studyTimings = new List<(string StudyId, TimeSpan Duration, List<(string Type, string Name, TimeSpan Duration)> Computations)>();

        Console.WriteLine($"Starting test suite: {_config.TestSuiteId}");
        Console.WriteLine($"Output directory: {_outputDir}");
        Console.WriteLine();

        var studiesToRun = _config.Studies;
        if (_studyFilter != null && _studyFilter.Length > 0)
        {
            studiesToRun = studiesToRun
                .Where(s => _studyFilter.Contains(s.StudyId, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (studiesToRun.Count == 0)
            {
                Console.WriteLine($"No studies match the filter: {string.Join(", ", _studyFilter)}");
                return 1;
            }
        }

        foreach (var study in studiesToRun)
        {
            Console.WriteLine($"=== Testing study: {study.StudyName} ({study.StudyId}) ===");
            var studyStopwatch = Stopwatch.StartNew();
            var computationTimings = new List<(string Type, string Name, TimeSpan Duration)>();

            try
            {
                using var loader = new StudyLoader();
                loader.LoadStudy(study.NetworkSourcePath, _config.GlobalSettings.LocalTempDirectory);

                // Load the single baseline file for this study
                string baselinePath = GetBaselinePath(study);
                Console.WriteLine($"  Loading baseline: {baselinePath}");

                if (!File.Exists(baselinePath))
                {
                    Console.WriteLine($"  WARNING: Baseline file not found. Tests will fail.");
                }
                else
                {
                    _comparer.LoadBaseline(baselinePath);
                }

                // Create computed results document for debugging
                var computedBaseline = _baselineWriter.CreateStudyBaseline(study.StudyId, study.StudyName);

                // Build computation list (from config or auto-discover)
                var computations = BuildComputationList(study);
                Console.WriteLine($"  Found {computations.Count} computations to run.");

                foreach (var compute in computations)
                {
                    _cts.Token.ThrowIfCancellationRequested();
                    var computeStopwatch = Stopwatch.StartNew();

                    try
                    {
                        ComparisonResult? result = null;

                        switch (compute.Type.ToLowerInvariant())
                        {
                            case "scenario":
                                var scenarioResults = _scenarioRunner.RunScenario(compute.ElementName, _cts.Token);
                                _baselineWriter.AddScenarioResults(computedBaseline, compute.ElementName, scenarioResults);
                                result = _comparer.CompareScenarioResults(compute.ElementName, scenarioResults);
                                break;

                            case "alternative":
                                var altResults = _alternativeRunner.RunAlternative(compute.ElementName, _cts.Token);
                                _baselineWriter.AddAlternativeResults(computedBaseline, compute.ElementName, altResults);
                                result = _comparer.CompareAlternativeResults(compute.ElementName, altResults);
                                break;

                            case "stagedamage":
                                var sdElement = _stageDamageRunner.GetStageDamageElement(compute.ElementName);
                                _baselineWriter.AddStageDamage(computedBaseline, compute.ElementName, sdElement);
                                result = _comparer.CompareStageDamage(compute.ElementName, sdElement);
                                break;

                            default:
                                Console.WriteLine($"    SKIP: Unknown compute type '{compute.Type}'");
                                continue;
                        }

                        computeStopwatch.Stop();
                        computationTimings.Add((compute.Type, compute.ElementName, computeStopwatch.Elapsed));

                        if (result != null)
                        {
                            if (!result.Passed)
                            {
                                failures++;
                                Console.WriteLine($"    FAIL: {compute.Type} '{compute.ElementName}' [{FormatDuration(computeStopwatch.Elapsed)}]");
                                Console.WriteLine($"          {result.Summary}");
                                PrintDifferences(result);
                            }
                            else
                            {
                                passed++;
                                Console.WriteLine($"    PASS: {compute.Type} '{compute.ElementName}' [{FormatDuration(computeStopwatch.Elapsed)}]");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        computeStopwatch.Stop();
                        computationTimings.Add((compute.Type, compute.ElementName, computeStopwatch.Elapsed));
                        failures++;
                        Console.WriteLine($"    ERROR: {compute.Type} '{compute.ElementName}' [{FormatDuration(computeStopwatch.Elapsed)}] - {ex.Message}");
                        if (_verbose)
                        {
                            Console.WriteLine($"           {ex.StackTrace}");
                        }
                    }
                }

                // Save computed results for debugging
                SaveComputedResults(computedBaseline, study);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("  TIMEOUT: Test run exceeded time limit.");
                failures++;
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ERROR loading study: {ex.Message}");
                if (_verbose)
                {
                    Console.WriteLine($"         {ex.StackTrace}");
                }
                failures++;
            }

            studyStopwatch.Stop();
            studyTimings.Add((study.StudyId, studyStopwatch.Elapsed, computationTimings));
            Console.WriteLine($"  Study completed in {FormatDuration(studyStopwatch.Elapsed)}");
            Console.WriteLine();
        }

        totalStopwatch.Stop();

        // Summary
        Console.WriteLine("=== Summary ===");
        Console.WriteLine($"Passed: {passed}");
        Console.WriteLine($"Failed: {failures}");
        Console.WriteLine($"Total:  {passed + failures}");
        Console.WriteLine();

        // Timing Summary
        Console.WriteLine("=== Timing Summary ===");
        Console.WriteLine($"Total Duration: {FormatDuration(totalStopwatch.Elapsed)}");
        Console.WriteLine();

        foreach (var (studyId, duration, computations) in studyTimings)
        {
            Console.WriteLine($"  {studyId}: {FormatDuration(duration)}");
            foreach (var (type, name, compDuration) in computations)
            {
                Console.WriteLine($"    - {type} '{name}': {FormatDuration(compDuration)}");
            }
        }

        return failures > 0 ? 1 : 0;
    }

    private static string FormatDuration(TimeSpan duration)
    {
        if (duration.TotalHours >= 1)
        {
            return $"{duration.Hours}h {duration.Minutes}m {duration.Seconds}s";
        }
        else if (duration.TotalMinutes >= 1)
        {
            return $"{duration.Minutes}m {duration.Seconds}.{duration.Milliseconds / 100}s";
        }
        else
        {
            return $"{duration.Seconds}.{duration.Milliseconds:D3}s";
        }
    }

    private List<ComputeConfiguration> BuildComputationList(StudyConfiguration study)
    {
        var computations = new List<ComputeConfiguration>(study.Computations);

        // Auto-discover scenarios
        if (study.RunAllScenarios)
        {
            var scenarios = BaseViewModel.StudyCache.GetChildElementsOfType<IASElement>();
            foreach (var scenario in scenarios)
            {
                if (!computations.Any(c => c.Type.Equals("scenario", StringComparison.OrdinalIgnoreCase)
                    && c.ElementName.Equals(scenario.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    computations.Add(new ComputeConfiguration
                    {
                        Type = "scenario",
                        ElementName = scenario.Name
                    });
                    Console.WriteLine($"    Auto-discovered scenario: {scenario.Name}");
                }
            }
        }

        // Auto-discover stage damage elements
        if (study.RunAllStageDamage)
        {
            var stageDamages = BaseViewModel.StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();
            foreach (var sd in stageDamages)
            {
                if (!computations.Any(c => c.Type.Equals("stagedamage", StringComparison.OrdinalIgnoreCase)
                    && c.ElementName.Equals(sd.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    computations.Add(new ComputeConfiguration
                    {
                        Type = "stagedamage",
                        ElementName = sd.Name
                    });
                    Console.WriteLine($"    Auto-discovered stage damage: {sd.Name}");
                }
            }
        }

        // Auto-discover alternatives (these depend on scenario results, so run last)
        if (study.RunAllAlternatives)
        {
            var alternatives = BaseViewModel.StudyCache.GetChildElementsOfType<AlternativeElement>();
            foreach (var alt in alternatives)
            {
                if (!computations.Any(c => c.Type.Equals("alternative", StringComparison.OrdinalIgnoreCase)
                    && c.ElementName.Equals(alt.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    computations.Add(new ComputeConfiguration
                    {
                        Type = "alternative",
                        ElementName = alt.Name
                    });
                    Console.WriteLine($"    Auto-discovered alternative: {alt.Name}");
                }
            }
        }

        return computations;
    }

    private string GetBaselinePath(StudyConfiguration study)
    {
        // Single baseline file per study
        return Path.Combine(study.BaselineDirectory, $"{study.StudyId}_baseline.xml");
    }

    private void SaveComputedResults(XElement computedBaseline, StudyConfiguration study)
    {
        // Save computed results in same format as baseline for easy comparison
        string outputPath = Path.Combine(_outputDir, $"{study.StudyId}_computed.xml");
        _baselineWriter.Save(computedBaseline, outputPath);
        Console.WriteLine($"  Computed results saved to: {outputPath}");
    }

    private void PrintDifferences(ComparisonResult result)
    {
        if (!_verbose || result.Differences.Count == 0)
        {
            return;
        }

        Console.WriteLine("          Differences:");
        foreach (var diff in result.Differences.Take(10)) // Limit to first 10
        {
            Console.WriteLine($"            - {diff}");
        }

        if (result.Differences.Count > 10)
        {
            Console.WriteLine($"            ... and {result.Differences.Count - 10} more");
        }
    }
}
