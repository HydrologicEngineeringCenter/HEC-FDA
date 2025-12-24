using System.Diagnostics;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;
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

/// <summary>
/// Runs FDA computations and generates CSV result reports.
/// </summary>
public class ComputeRunner
{
    private readonly TestConfiguration _config;
    private readonly string _outputDir;
    private readonly string[]? _studyFilter;
    private readonly CancellationTokenSource _cts;
    private readonly CsvReportFactory _csvReportFactory = new();

    public ComputeRunner(TestConfiguration config, string outputDir, string[]? studyFilter)
    {
        _config = config;
        _outputDir = outputDir;
        _studyFilter = studyFilter;
        _cts = new CancellationTokenSource();

        if (_config.GlobalSettings.TimeoutMinutes > 0)
        {
            _cts.CancelAfter(TimeSpan.FromMinutes(_config.GlobalSettings.TimeoutMinutes));
        }
    }

    public async Task<int> RunAsync()
    {
        int errors = 0;
        int completed = 0;
        Stopwatch totalStopwatch = Stopwatch.StartNew();
        List<(string StudyId, TimeSpan Duration, int ComputeCount, int ErrorCount)> studyTimings = new();

        Console.WriteLine($"Configuration: {_config.TestSuiteId}");
        Console.WriteLine($"Output directory: {_outputDir}");
        Console.WriteLine();

        List<StudyConfiguration> studiesToRun = _config.Studies;
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

        foreach (StudyConfiguration study in studiesToRun)
        {
            Console.WriteLine($"=== Computing: {study.StudyName} ({study.StudyId}) ===");
            Stopwatch studyStopwatch = Stopwatch.StartNew();
            int studyErrors = 0;
            int studyCompleted = 0;

            try
            {
                using StudyLoader loader = new();
                loader.LoadStudy(study.NetworkSourcePath, _config.GlobalSettings.LocalTempDirectory);

                List<ComputeConfiguration> computations = BuildComputationList(study);
                Console.WriteLine($"  Found {computations.Count} computations to run.");

                foreach (ComputeConfiguration compute in computations)
                {
                    _cts.Token.ThrowIfCancellationRequested();
                    Stopwatch computeStopwatch = Stopwatch.StartNew();

                    try
                    {
                        switch (compute.Type.ToLowerInvariant())
                        {
                            case "stagedamage":
                                List<UncertainPairedData> sdCurves = StageDamageRunner.RunStageDamage(compute.ElementName);
                                SaveStageDamageResults(compute.ElementName, sdCurves);
                                _csvReportFactory.AddStageDamageSummary(study.StudyId, compute.ElementName, sdCurves);
                                break;

                            case "scenario":
                                ScenarioResults scenarioResults = ScenarioRunner.RunScenario(compute.ElementName, _cts.Token);
                                IASElement scenarioElement = SaveScenarioResults(compute.ElementName, scenarioResults);
                                _csvReportFactory.AddScenarioResults(study.StudyId, scenarioElement);
                                break;

                            case "alternative":
                                AlternativeResults altResults = AlternativeRunner.RunAlternative(compute.ElementName, _cts.Token);
                                AlternativeElement altElement = SaveAlternativeResults(compute.ElementName, altResults);
                                _csvReportFactory.AddAlternativeResults(study.StudyId, altElement);
                                break;

                            case "alternativecomparison":
                                (AlternativeComparisonReportResults compResults, List<(int altId, string altName)> withProjAlts) = RunAlternativeComparisonWithMetadata(compute.ElementName, _cts.Token);
                                _csvReportFactory.AddAlternativeComparisonResults(study.StudyId, compute.ElementName, compResults, withProjAlts);
                                break;

                            default:
                                Console.WriteLine($"    SKIP: Unknown compute type '{compute.Type}'");
                                continue;
                        }

                        computeStopwatch.Stop();
                        studyCompleted++;
                        Console.WriteLine($"    OK: {compute.Type} '{compute.ElementName}' [{FormatDuration(computeStopwatch.Elapsed)}]");
                    }
                    catch (Exception ex)
                    {
                        computeStopwatch.Stop();
                        studyErrors++;
                        Console.WriteLine($"    ERROR: {compute.Type} '{compute.ElementName}' [{FormatDuration(computeStopwatch.Elapsed)}]");
                        Console.WriteLine($"           {ex.Message}");
                        Console.WriteLine($"           {ex.StackTrace}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("  TIMEOUT: Computation exceeded time limit.");
                studyErrors++;
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ERROR loading study: {ex.Message}");
                Console.WriteLine($"         {ex.StackTrace}");
                studyErrors++;
            }

            studyStopwatch.Stop();
            studyTimings.Add((study.StudyId, studyStopwatch.Elapsed, studyCompleted, studyErrors));
            completed += studyCompleted;
            errors += studyErrors;
            Console.WriteLine($"  Completed in {FormatDuration(studyStopwatch.Elapsed)} ({studyCompleted} succeeded, {studyErrors} failed)");
            Console.WriteLine();
        }

        totalStopwatch.Stop();

        // Summary
        Console.WriteLine("=== Summary ===");
        Console.WriteLine($"Completed: {completed}");
        Console.WriteLine($"Errors:    {errors}");
        Console.WriteLine($"Duration:  {FormatDuration(totalStopwatch.Elapsed)}");
        Console.WriteLine();

        // Save CSV report
        string csvPath = Path.Combine(_outputDir, "results_report.csv");
        _csvReportFactory.SaveReport(csvPath);

        return errors > 0 ? 1 : 0;
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

        if (study.RunAllStageDamage)
        {
            List<AggregatedStageDamageElement> stageDamages = BaseViewModel.StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();
            foreach (AggregatedStageDamageElement sd in stageDamages)
            {
                if (!computations.Any(c => c.Type.Equals("stagedamage", StringComparison.OrdinalIgnoreCase)
                    && c.ElementName.Equals(sd.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    computations.Add(new ComputeConfiguration { Type = "stagedamage", ElementName = sd.Name });
                    Console.WriteLine($"    Auto-discovered stage damage: {sd.Name}");
                }
            }
        }

        if (study.RunAllScenarios)
        {
            List<IASElement> scenarios = BaseViewModel.StudyCache.GetChildElementsOfType<IASElement>();
            foreach (IASElement scenario in scenarios)
            {
                if (!computations.Any(c => c.Type.Equals("scenario", StringComparison.OrdinalIgnoreCase)
                    && c.ElementName.Equals(scenario.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    computations.Add(new ComputeConfiguration { Type = "scenario", ElementName = scenario.Name });
                    Console.WriteLine($"    Auto-discovered scenario: {scenario.Name}");
                }
            }
        }

        if (study.RunAllAlternatives)
        {
            List<AlternativeElement> alternatives = BaseViewModel.StudyCache.GetChildElementsOfType<AlternativeElement>();
            foreach (AlternativeElement alt in alternatives)
            {
                if (!computations.Any(c => c.Type.Equals("alternative", StringComparison.OrdinalIgnoreCase)
                    && c.ElementName.Equals(alt.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    computations.Add(new ComputeConfiguration { Type = "alternative", ElementName = alt.Name });
                    Console.WriteLine($"    Auto-discovered alternative: {alt.Name}");
                }
            }
        }

        if (study.RunAllAlternativeComparisons)
        {
            List<AlternativeComparisonReportElement> altCompReports = BaseViewModel.StudyCache.GetChildElementsOfType<AlternativeComparisonReportElement>();
            foreach (AlternativeComparisonReportElement report in altCompReports)
            {
                if (!computations.Any(c => c.Type.Equals("alternativecomparison", StringComparison.OrdinalIgnoreCase)
                    && c.ElementName.Equals(report.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    computations.Add(new ComputeConfiguration { Type = "alternativecomparison", ElementName = report.Name });
                    Console.WriteLine($"    Auto-discovered alternative comparison: {report.Name}");
                }
            }
        }

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
        AlternativeComparisonReportElement element = ScenarioRunner.FindElement<AlternativeComparisonReportElement>(elementName);

        List<(int altId, string altName)> withProjectAlternatives = new();
        List<AlternativeElement> allAlternatives = BaseViewModel.StudyCache.GetChildElementsOfType<AlternativeElement>();

        foreach (int altId in element.WithProjAltIDs)
        {
            AlternativeElement? alt = allAlternatives.FirstOrDefault(a => a.ID == altId);
            string altName = alt?.Name ?? $"Alternative_{altId}";
            withProjectAlternatives.Add((altId, altName));
        }

        AlternativeComparisonReportResults results = AlternativeComparisonRunner.RunAlternativeComparison(elementName, cancellationToken);
        return (results, withProjectAlternatives);
    }

    private static IASElement SaveScenarioResults(string elementName, ScenarioResults results)
    {
        IASElement element = ScenarioRunner.FindElement<IASElement>(elementName);
        element.Results = results;
        PersistenceFactory.GetIASManager().SaveExisting(element);
        Console.WriteLine($"      Saved to temp database.");
        return element;
    }

    private static AlternativeElement SaveAlternativeResults(string elementName, AlternativeResults results)
    {
        AlternativeElement element = ScenarioRunner.FindElement<AlternativeElement>(elementName);
        element.Results = results;
        PersistenceFactory.GetElementManager<AlternativeElement>().SaveExisting(element);
        Console.WriteLine($"      Saved to temp database.");
        return element;
    }

    private static void SaveStageDamageResults(string elementName, List<UncertainPairedData> curves)
    {
        AggregatedStageDamageElement element = ScenarioRunner.FindElement<AggregatedStageDamageElement>(elementName);

        List<ImpactAreaElement> impactAreaElements = BaseViewModel.StudyCache.GetChildElementsOfType<ImpactAreaElement>();
        ImpactAreaElement? impactAreaElement = impactAreaElements.Count > 0 ? impactAreaElements[0] : null;

        List<StageDamageCurve> stageDamageCurves = new();
        foreach (UncertainPairedData upd in curves)
        {
            CurveComponentVM curveComponent = new(StringConstants.STAGE_DAMAGE, StringConstants.STAGE, StringConstants.DAMAGE, DistributionOptions.HISTOGRAM_ONLY);
            curveComponent.SetPairedData(upd);

            ImpactAreaRowItem impactAreaRowItem = impactAreaElement?.GetImpactAreaRow(upd.ImpactAreaID)
                ?? new ImpactAreaRowItem(upd.ImpactAreaID, "");

            StageDamageCurve sdCurve = new(impactAreaRowItem, upd.DamageCategory, curveComponent, upd.AssetCategory, StageDamageConstructionType.COMPUTED);
            stageDamageCurves.Add(sdCurve);
        }

        element.Curves.Clear();
        element.Curves.AddRange(stageDamageCurves);

        PersistenceFactory.GetElementManager<AggregatedStageDamageElement>().SaveExisting(element);
        Console.WriteLine($"      Saved {curves.Count} curves to temp database.");
    }
}
