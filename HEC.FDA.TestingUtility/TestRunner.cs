using System.Diagnostics;
using System.Xml.Linq;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;
using HEC.FDA.TestingUtility.Comparison;
using HEC.FDA.TestingUtility.Configuration;
using HEC.FDA.TestingUtility.Reporting;
using HEC.FDA.TestingUtility.Services;
using HEC.FDA.ViewModel;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.AlternativeComparisonReport;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.TestingUtility;

public class TestRunner
{
    private readonly TestConfiguration _config;
    private readonly string _outputDir;
    private readonly bool _verbose;
    private readonly string[]? _studyFilter;
    private readonly CancellationTokenSource _cts;

    private readonly XmlResultComparer _comparer = new();
    private readonly CsvReportFactory _csvReportFactory = new();

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
        List<(string StudyId, TimeSpan Duration, List<(string Type, string Name, TimeSpan Duration)> Computations)> studyTimings = new();

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
            List<(string Type, string Name, TimeSpan Duration)> computationTimings = new();

            try
            {
                using StudyLoader loader = new();
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
                var computedBaseline = StudyBaselineWriter.CreateStudyBaseline(study.StudyId, study.StudyName);

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
                            case "stagedamage":
                                List<UncertainPairedData> sdCurves = StageDamageRunner.RunStageDamage(compute.ElementName);
                                SaveStageDamageResults(compute.ElementName, sdCurves);
                                StudyBaselineWriter.AddStageDamage(computedBaseline, compute.ElementName, sdCurves);
                                _csvReportFactory.AddStageDamageSummary(study.StudyId, compute.ElementName, sdCurves);
                                result = _comparer.CompareStageDamage(compute.ElementName, sdCurves);
                                break;

                            case "scenario":
                                ScenarioResults scenarioResults = ScenarioRunner.RunScenario(compute.ElementName, _cts.Token);
                                SaveScenarioResults(compute.ElementName, scenarioResults);
                                StudyBaselineWriter.AddScenarioResults(computedBaseline, compute.ElementName, scenarioResults);
                                _csvReportFactory.AddScenarioResults(study.StudyId, compute.ElementName, scenarioResults);
                                result = _comparer.CompareScenarioResults(compute.ElementName, scenarioResults);
                                break;

                            case "alternative":
                                AlternativeResults altResults = AlternativeRunner.RunAlternative(compute.ElementName, _cts.Token);
                                SaveAlternativeResults(compute.ElementName, altResults);
                                StudyBaselineWriter.AddAlternativeResults(computedBaseline, compute.ElementName, altResults);
                                _csvReportFactory.AddAlternativeResults(study.StudyId, compute.ElementName, altResults);
                                result = _comparer.CompareAlternativeResults(compute.ElementName, altResults);
                                break;

                            case "alternativecomparison":
                                (AlternativeComparisonReportResults compResults, List<(int altId, string altName)> withProjAlts) = RunAlternativeComparisonWithMetadata(compute.ElementName, _cts.Token);
                                StudyBaselineWriter.AddAlternativeComparisonResults(computedBaseline, compute.ElementName, compResults, withProjAlts);
                                _csvReportFactory.AddAlternativeComparisonResults(study.StudyId, compute.ElementName, compResults, withProjAlts);
                                result = _comparer.CompareAlternativeComparisonResults(compute.ElementName, compResults, withProjAlts);
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

        // Save CSV report
        Console.WriteLine();
        string csvPath = Path.Combine(_outputDir, "results_report.csv");
        _csvReportFactory.SaveReport(csvPath);

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

    private static List<ComputeConfiguration> BuildComputationList(StudyConfiguration study)
    {
        List<ComputeConfiguration> computations = new(study.Computations);

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

        // Auto-discover alternative comparison reports (depend on alternatives, so run last)
        if (study.RunAllAlternativeComparisons)
        {
            List<AlternativeComparisonReportElement> altCompReports = BaseViewModel.StudyCache.GetChildElementsOfType<AlternativeComparisonReportElement>();
            foreach (AlternativeComparisonReportElement report in altCompReports)
            {
                if (!computations.Any(c => c.Type.Equals("alternativecomparison", StringComparison.OrdinalIgnoreCase)
                    && c.ElementName.Equals(report.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    computations.Add(new ComputeConfiguration
                    {
                        Type = "alternativecomparison",
                        ElementName = report.Name
                    });
                    Console.WriteLine($"    Auto-discovered alternative comparison: {report.Name}");
                }
            }
        }

        // Sort by dependency order: stagedamage → scenario → alternative → alternativecomparison
        return SortByDependencyOrder(computations);
    }

    private static List<ComputeConfiguration> SortByDependencyOrder(List<ComputeConfiguration> computations)
    {
        int GetOrder(string type) => type.ToLowerInvariant() switch
        {
            "stagedamage" => 0,
            "scenario" => 1,
            "alternative" => 2,
            "alternativecomparison" => 3,
            _ => 99
        };

        return computations.OrderBy(c => GetOrder(c.Type)).ToList();
    }

    private static (AlternativeComparisonReportResults results, List<(int altId, string altName)> withProjectAlternatives) RunAlternativeComparisonWithMetadata(string elementName, CancellationToken cancellationToken)
    {
        // Get the element to extract the with-project alternative IDs
        var element = ScenarioRunner.FindElement<AlternativeComparisonReportElement>(elementName);

        // Build the list of with-project alternatives with names
        List<(int altId, string altName)> withProjectAlternatives = new();
        var allAlternatives = BaseViewModel.StudyCache.GetChildElementsOfType<AlternativeElement>();

        foreach (int altId in element.WithProjAltIDs)
        {
            var alt = allAlternatives.FirstOrDefault(a => a.ID == altId);
            string altName = alt?.Name ?? $"Alternative_{altId}";
            withProjectAlternatives.Add((altId, altName));
        }

        // Run the computation
        var results = AlternativeComparisonRunner.RunAlternativeComparison(elementName, cancellationToken);

        return (results, withProjectAlternatives);
    }

    private static string GetBaselinePath(StudyConfiguration study)
    {
        // Single baseline file per study
        return Path.Combine(study.BaselineDirectory, $"{study.StudyId}_baseline.xml");
    }

    private void SaveComputedResults(XElement computedBaseline, StudyConfiguration study)
    {
        // Save computed results in same format as baseline for easy comparison
        string outputPath = Path.Combine(_outputDir, $"{study.StudyId}_computed.xml");
        StudyBaselineWriter.Save(computedBaseline, outputPath);
        Console.WriteLine($"  Computed results saved to: {outputPath}");
    }

    private void PrintDifferences(ComparisonResult result)
    {
        if (!_verbose || result.Differences.Count == 0)
        {
            return;
        }

        Console.WriteLine("          Differences:");
        foreach (Difference diff in result.Differences.Take(10))
        {
            Console.WriteLine($"            - {diff}");
        }

        if (result.Differences.Count > 10)
        {
            Console.WriteLine($"            ... and {result.Differences.Count - 10} more");
        }
    }

    /// <summary>
    /// Saves scenario results to the temp database so downstream computations can use them.
    /// </summary>
    private static void SaveScenarioResults(string elementName, ScenarioResults results)
    {
        IASElement element = ScenarioRunner.FindElement<IASElement>(elementName);
        element.Results = results;
        PersistenceFactory.GetIASManager().SaveExisting(element);
        Console.WriteLine($"      Saved scenario results to temp database.");
    }

    /// <summary>
    /// Saves alternative results to the temp database so downstream computations can use them.
    /// </summary>
    private static void SaveAlternativeResults(string elementName, AlternativeResults results)
    {
        AlternativeElement element = ScenarioRunner.FindElement<AlternativeElement>(elementName);
        element.Results = results;
        PersistenceFactory.GetElementManager<AlternativeElement>().SaveExisting(element);
        Console.WriteLine($"      Saved alternative results to temp database.");
    }

    /// <summary>
    /// Saves stage damage curves to the temp database so downstream computations can use them.
    /// </summary>
    private static void SaveStageDamageResults(string elementName, List<UncertainPairedData> curves)
    {
        AggregatedStageDamageElement element = ScenarioRunner.FindElement<AggregatedStageDamageElement>(elementName);

        // Get impact area element to look up names
        List<ImpactAreaElement> impactAreaElements = BaseViewModel.StudyCache.GetChildElementsOfType<ImpactAreaElement>();
        ImpactAreaElement? impactAreaElement = impactAreaElements.Count > 0 ? impactAreaElements[0] : null;

        // Convert UncertainPairedData curves to StageDamageCurves and update the element
        List<StageDamageCurve> stageDamageCurves = new();
        foreach (UncertainPairedData upd in curves)
        {
            // Create CurveComponentVM and set the paired data
            CurveComponentVM curveComponent = new(StringConstants.STAGE_DAMAGE, StringConstants.STAGE, StringConstants.DAMAGE, DistributionOptions.HISTOGRAM_ONLY);
            curveComponent.SetPairedData(upd);

            // Get the impact area row item
            ImpactAreaRowItem impactAreaRowItem = impactAreaElement?.GetImpactAreaRow(upd.ImpactAreaID)
                ?? new ImpactAreaRowItem(upd.ImpactAreaID, "");

            StageDamageCurve sdCurve = new(impactAreaRowItem, upd.DamageCategory, curveComponent, upd.AssetCategory, StageDamageConstructionType.COMPUTED);
            stageDamageCurves.Add(sdCurve);
        }

        // Update the element's curves - this modifies the in-memory element
        element.Curves.Clear();
        element.Curves.AddRange(stageDamageCurves);

        PersistenceFactory.GetElementManager<AggregatedStageDamageElement>().SaveExisting(element);
        Console.WriteLine($"      Saved {curves.Count} stage damage curves to temp database.");
    }
}
