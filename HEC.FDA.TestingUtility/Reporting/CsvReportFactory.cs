using System.Text;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.Alternatives.Results.BatchCompute;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Results;

namespace HEC.FDA.TestingUtility.Reporting;

/// <summary>
/// Factory class that produces a comprehensive CSV report containing all tabularly
/// visualized results from FDA computations across all studies.
/// Uses ViewModel row item classes for scenario and alternative results.
/// </summary>
public class CsvReportFactory
{
    private readonly List<(string StudyId, IASElement Element)> _scenarioElements = [];
    private readonly List<(string StudyId, AlternativeElement Element)> _alternativeElements = [];
    private readonly StringBuilder _stageDamageSummary = new();
    private readonly StringBuilder _altComparisonSummary = new();
    private readonly StringBuilder _altComparisonByCategory = new();

    public CsvReportFactory()
    {
        WriteStageDamageHeader();
        WriteAltComparisonHeaders();
    }

    private void WriteStageDamageHeader()
    {
        _stageDamageSummary.AppendLine("Study ID,Element Name,Impact Area ID,Impact Area Name,Damage Category,Asset Category,Point Count,Min Stage,Max Stage, Median Sample Integral");
    }

    private void WriteAltComparisonHeaders()
    {
        _altComparisonSummary.AppendLine("Study ID,Report Name,With Project Alt,Impact Area ID,Without Proj EqAD,With Proj EqAD,EqAD Reduced,EqAD Reduced 25th Pct,EqAD Reduced 50th Pct,EqAD Reduced 75th Pct,Without Proj Base EAD,With Proj Base EAD,Base EAD Reduced,Without Proj Future EAD,With Proj Future EAD,Future EAD Reduced");
        _altComparisonByCategory.AppendLine("Study ID,Report Name,With Project Alt,Impact Area ID,Damage Category,Asset Category,EqAD Reduced,Base Year EAD Reduced,Future Year EAD Reduced");
    }

    /// <summary>
    /// Adds a scenario element to be included in the report.
    /// Uses ScenarioDamageRowItem, ScenarioDamCatRowItem, and ScenarioPerformanceRowItem for data extraction.
    /// </summary>
    public void AddScenarioResults(string studyId, IASElement element)
    {
        if (element?.Results == null) return;
        _scenarioElements.Add((studyId, element));
    }

    /// <summary>
    /// Adds an alternative element to be included in the report.
    /// Uses AlternativeDamageRowItem and AlternativeDamCatRowItem for data extraction.
    /// </summary>
    public void AddAlternativeResults(string studyId, AlternativeElement element)
    {
        if (element?.Results == null || element.Results.IsNull) return;
        _alternativeElements.Add((studyId, element));
    }

    /// <summary>
    /// Adds stage damage summary from computed curves to the report.
    /// </summary>
    public void AddStageDamageSummary(string studyId, string elementName, List<UncertainPairedData> curves)
    {
        if (curves == null) return;

        try
        {
            foreach (var curve in curves)
            {
                int impactAreaId = curve.ImpactAreaID;
                string damCat = curve.DamageCategory ?? "";
                string assetCat = curve.AssetCategory ?? "";

                int pointCount = curve.Xvals?.Length ?? 0;
                double minStage = pointCount > 0 ? curve.Xvals!.Min() : 0;
                double maxStage = pointCount > 0 ? curve.Xvals!.Max() : 0;

                PairedData medianCurve = curve.SamplePairedData(.5);
                double integral = medianCurve.integrate();

                _stageDamageSummary.AppendLine($"{EscapeCsv(studyId)},{EscapeCsv(elementName)},{impactAreaId},{EscapeCsv(damCat)},{EscapeCsv(assetCat)},{pointCount},{minStage:F2},{maxStage:F2},{integral:F2}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    Warning: Error extracting stage damage for CSV: {ex.Message}");
        }
    }

    /// <summary>
    /// Adds alternative comparison report results to the report.
    /// </summary>
    public void AddAlternativeComparisonResults(string studyId, string reportName, AlternativeComparisonReportResults results, List<(int altId, string altName)> withProjectAlternatives)
    {
        if (results == null) return;

        try
        {
            var impactAreaIds = results.GetImpactAreaIDs();
            var damageCategories = results.GetDamageCategories();
            var assetCategories = results.GetAssetCategories();

            foreach (var (altId, altName) in withProjectAlternatives)
            {
                foreach (int impactAreaId in impactAreaIds)
                {
                    double withoutProjEqad = results.SampleMeanWithoutProjectEqad(impactAreaId);
                    double withProjEqad = results.SampleMeanWithProjectEqad(altId, impactAreaId);
                    double eqadReduced = results.SampleMeanEqadReduced(altId, impactAreaId);
                    double eqadReduced25 = results.EqadReducedExceededWithProbabilityQ(0.75, altId, impactAreaId);
                    double eqadReduced50 = results.EqadReducedExceededWithProbabilityQ(0.50, altId, impactAreaId);
                    double eqadReduced75 = results.EqadReducedExceededWithProbabilityQ(0.25, altId, impactAreaId);

                    double withoutProjBaseEad = results.SampleMeanWithoutProjectBaseYearEAD(impactAreaId);
                    double withProjBaseEad = results.SampleMeanWithProjectBaseYearEAD(altId, impactAreaId);
                    double baseEadReduced = results.SampleMeanBaseYearEADReduced(altId, impactAreaId);

                    double withoutProjFutureEad = results.SampleMeanWithoutProjectFutureYearEAD(impactAreaId);
                    double withProjFutureEad = results.SampleMeanWithProjectFutureYearEAD(altId, impactAreaId);
                    double futureEadReduced = results.SampleMeanFutureYearEADReduced(altId, impactAreaId);

                    _altComparisonSummary.AppendLine($"{EscapeCsv(studyId)},{EscapeCsv(reportName)},{EscapeCsv(altName)},{impactAreaId},{withoutProjEqad:F2},{withProjEqad:F2},{eqadReduced:F2},{eqadReduced25:F2},{eqadReduced50:F2},{eqadReduced75:F2},{withoutProjBaseEad:F2},{withProjBaseEad:F2},{baseEadReduced:F2},{withoutProjFutureEad:F2},{withProjFutureEad:F2},{futureEadReduced:F2}");
                }

                foreach (int impactAreaId in impactAreaIds)
                {
                    foreach (string damCat in damageCategories)
                    {
                        foreach (string assetCat in assetCategories)
                        {
                            double eqadReduced = results.SampleMeanEqadReduced(altId, impactAreaId, damCat, assetCat);
                            double baseEadReduced = results.SampleMeanBaseYearEADReduced(altId, impactAreaId, damCat, assetCat);
                            double futureEadReduced = results.SampleMeanFutureYearEADReduced(altId, impactAreaId, damCat, assetCat);

                            if (eqadReduced != 0 || baseEadReduced != 0 || futureEadReduced != 0)
                            {
                                _altComparisonByCategory.AppendLine($"{EscapeCsv(studyId)},{EscapeCsv(reportName)},{EscapeCsv(altName)},{impactAreaId},{EscapeCsv(damCat)},{EscapeCsv(assetCat)},{eqadReduced:F2},{baseEadReduced:F2},{futureEadReduced:F2}");
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    Warning: Error extracting alternative comparison results for CSV: {ex.Message}");
        }
    }

    /// <summary>
    /// Saves the comprehensive report to a CSV file.
    /// </summary>
    public void SaveReport(string outputPath)
    {
        StringBuilder report = new();

        report.AppendLine("=== FDA COMPUTATION RESULTS REPORT ===");
        report.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        report.AppendLine();

        report.AppendLine("=== SCENARIO RESULTS ===");
        report.Append(BuildScenarioResultsSection());
        report.AppendLine();

        report.AppendLine("=== SCENARIO DAMAGE BY CATEGORY ===");
        report.Append(BuildScenarioDamCatSection());
        report.AppendLine();

        report.AppendLine("=== SCENARIO PERFORMANCE ===");
        report.Append(BuildScenarioPerformanceSection());
        report.AppendLine();

        report.AppendLine("=== SCENARIO ASSURANCE OF AEP ===");
        report.Append(BuildScenarioAssuranceSection());
        report.AppendLine();

        report.AppendLine("=== ALTERNATIVE RESULTS ===");
        report.Append(BuildAlternativeResultsSection());
        report.AppendLine();

        report.AppendLine("=== ALTERNATIVE DAMAGE BY CATEGORY ===");
        report.Append(BuildAlternativeDamCatSection());
        report.AppendLine();

        report.AppendLine("=== STAGE DAMAGE SUMMARY ===");
        report.Append(_stageDamageSummary);
        report.AppendLine();

        report.AppendLine("=== ALTERNATIVE COMPARISON SUMMARY ===");
        report.Append(_altComparisonSummary);
        report.AppendLine();

        report.AppendLine("=== ALTERNATIVE COMPARISON BY CATEGORY ===");
        report.Append(_altComparisonByCategory);

        File.WriteAllText(outputPath, report.ToString());
        Console.WriteLine($"CSV report saved to: {outputPath}");
    }

    #region Scenario Section Builders

    private StringBuilder BuildScenarioResultsSection()
    {
        StringBuilder sb = new();
        sb.AppendLine("Study ID,Name,Analysis Year,Impact Area,Mean EAD,25th Percentile EAD,50th Percentile EAD,75th Percentile EAD");

        foreach (var (studyId, element) in _scenarioElements)
        {
            try
            {
                var rows = ScenarioDamageRowItem.CreateScenarioDamageRowItems(element);
                foreach (var row in rows)
                {
                    sb.AppendLine($"{EscapeCsv(studyId)},{EscapeCsv(row.Name)},{EscapeCsv(row.AnalysisYear)},{EscapeCsv(row.ImpactArea)},{row.Mean:F2},{row.Point25:F2},{row.Point5:F2},{row.Point75:F2}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Warning: Error extracting scenario results for {element.Name}: {ex.Message}");
            }
        }

        return sb;
    }

    private StringBuilder BuildScenarioDamCatSection()
    {
        StringBuilder sb = new();
        sb.AppendLine("Study ID,Scenario Name,Analysis Year,Impact Area,Damage Category,Asset Category,Mean EAD");

        foreach (var (studyId, element) in _scenarioElements)
        {
            try
            {
                var rows = ScenarioDamCatRowItem.CreateScenarioDamCatRowItems(element);
                foreach (var row in rows)
                {
                    sb.AppendLine($"{EscapeCsv(studyId)},{EscapeCsv(row.Name)},{EscapeCsv(row.AnalysisYear)},{EscapeCsv(row.ImpactAreaName)},{EscapeCsv(row.DamCat)},{EscapeCsv(row.AssetCat)},{row.MeanDamage:F2}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Warning: Error extracting scenario damage categories for {element.Name}: {ex.Message}");
            }
        }

        return sb;
    }

    private StringBuilder BuildScenarioPerformanceSection()
    {
        StringBuilder sb = new();
        sb.AppendLine("Study ID,Name,Analysis Year,Impact Area,Threshold Type,Threshold Value,LT Risk 10yr,LT Risk 30yr,LT Risk 50yr,Mean AEP,Median AEP,Assurance 0.10,Assurance 0.04,Assurance 0.02,Assurance 0.01,Assurance 0.004,Assurance 0.002");

        foreach (var (studyId, element) in _scenarioElements)
        {
            try
            {
                foreach (var iaResult in element.Results.ResultsList)
                {
                    int iasID = iaResult.ImpactAreaID;
                    SpecificIAS? ias = element.SpecificIASElements.FirstOrDefault(s => s.ImpactAreaID == iasID);
                    if (ias == null) continue;

                    foreach (var threshold in iaResult.PerformanceByThresholds.ListOfThresholds)
                    {
                        var row = new ScenarioPerformanceRowItem(element, ias, threshold);
                        sb.AppendLine($"{EscapeCsv(studyId)},{EscapeCsv(row.Name)},{EscapeCsv(row.AnalysisYear)},{EscapeCsv(row.ImpactArea)},{EscapeCsv(row.ThresholdType)},{row.ThresholdValue:F2},{row.LongTerm10:F4},{row.LongTerm30:F4},{row.LongTerm50:F4},{row.Mean:F6},{row.Median:F6},{row.Threshold1:F4},{row.Threshold04:F4},{row.Threshold02:F4},{row.Threshold01:F4},{row.Threshold004:F4},{row.Threshold002:F4}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Warning: Error extracting scenario performance for {element.Name}: {ex.Message}");
            }
        }

        return sb;
    }

    private StringBuilder BuildScenarioAssuranceSection()
    {
        StringBuilder sb = new();
        sb.AppendLine("Study ID,Name,Analysis Year,Impact Area,Threshold Type,Threshold Value,Mean AEP,Median AEP,90% Assurance AEP,Assurance of 0.10 AEP,Assurance of 0.04 AEP,Assurance of 0.02 AEP,Assurance of 0.01 AEP,Assurance of 0.004 AEP,Assurance of 0.002 AEP");

        foreach (var (studyId, element) in _scenarioElements)
        {
            try
            {
                foreach (var iaResult in element.Results.ResultsList)
                {
                    int iasID = iaResult.ImpactAreaID;
                    SpecificIAS? ias = element.SpecificIASElements.FirstOrDefault(s => s.ImpactAreaID == iasID);
                    if (ias == null) continue;

                    foreach (var threshold in iaResult.PerformanceByThresholds.ListOfThresholds)
                    {
                        var row = new AssuranceOfAEPRowItem(element, ias, threshold);
                        sb.AppendLine($"{EscapeCsv(studyId)},{EscapeCsv(row.Name)},{EscapeCsv(row.AnalysisYear)},{EscapeCsv(row.ImpactArea)},{EscapeCsv(row.ThresholdType)},{row.ThresholdValue:F2},{row.Mean:F6},{row.Median:F6},{row.NinetyPercentAssurance:F6},{row.AEP1:F4},{row.AEP04:F4},{row.AEP02:F4},{row.AEP01:F4},{row.AEP004:F4},{row.AEP002:F4}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Warning: Error extracting scenario assurance for {element.Name}: {ex.Message}");
            }
        }

        return sb;
    }

    #endregion

    #region Alternative Section Builders

    private StringBuilder BuildAlternativeResultsSection()
    {
        StringBuilder sb = new();
        sb.AppendLine("Study ID,Name,Impact Area,Base Year,Future Year,Discount Rate,Period of Analysis,Mean EqAD,25th Percentile EqAD,50th Percentile EqAD,75th Percentile EqAD");

        foreach (var (studyId, element) in _alternativeElements)
        {
            try
            {
                var rows = AlternativeDamageRowItem.CreateAlternativeDamageRowItems(element);
                foreach (var row in rows)
                {
                    sb.AppendLine($"{EscapeCsv(studyId)},{EscapeCsv(row.Name)},{EscapeCsv(row.ImpactArea)},{row.BaseYear},{row.FutureYear},{row.DiscountRate:F4},{row.PeriodOfAnalysis},{row.Mean:F2},{row.Point75:F2},{row.Point5:F2},{row.Point25:F2}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Warning: Error extracting alternative results for {element.Name}: {ex.Message}");
            }
        }

        return sb;
    }

    private StringBuilder BuildAlternativeDamCatSection()
    {
        StringBuilder sb = new();
        sb.AppendLine("Study ID,Scenario Name,Impact Area,Damage Category,Asset Category,Mean EqAD");

        foreach (var (studyId, element) in _alternativeElements)
        {
            try
            {
                var rows = AlternativeDamCatRowItem.CreateAlternativeDamCatRowItems(element);
                foreach (var row in rows)
                {
                    sb.AppendLine($"{EscapeCsv(studyId)},{EscapeCsv(row.Name)},{EscapeCsv(row.ImpactAreaName)},{EscapeCsv(row.DamCat)},{EscapeCsv(row.AssetCat)},{row.MeanDamage:F2}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Warning: Error extracting alternative damage categories for {element.Name}: {ex.Message}");
            }
        }

        return sb;
    }

    #endregion

    /// <summary>
    /// Escapes a value for CSV output (handles commas and quotes).
    /// </summary>
    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";

        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}
