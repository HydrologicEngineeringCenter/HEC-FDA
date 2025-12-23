using System.Text;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;

namespace HEC.FDA.TestingUtility.Reporting;

/// <summary>
/// Factory class that produces a comprehensive CSV report containing all tabularly
/// visualized results from FDA computations across all studies.
/// </summary>
public class CsvReportFactory
{
    private readonly StringBuilder _scenarioResults = new();
    private readonly StringBuilder _scenarioDamageByCategory = new();
    private readonly StringBuilder _scenarioPerformance = new();
    private readonly StringBuilder _alternativeResults = new();
    private readonly StringBuilder _alternativeDamageByCategory = new();
    private readonly StringBuilder _stageDamageSummary = new();
    private readonly StringBuilder _altComparisonSummary = new();
    private readonly StringBuilder _altComparisonByCategory = new();

    public CsvReportFactory()
    {
        WriteHeaders();
    }

    private void WriteHeaders()
    {
        // Scenario Results header
        _scenarioResults.AppendLine("Study ID,Scenario Name,Impact Area ID,Mean EAD,EAD 25th Pct,EAD 50th Pct,EAD 75th Pct");

        // Scenario Damage by Category header
        _scenarioDamageByCategory.AppendLine("Study ID,Scenario Name,Impact Area ID,Damage Category,Asset Category,Mean EAD");

        // Scenario Performance header
        _scenarioPerformance.AppendLine("Study ID,Scenario Name,Impact Area ID,Threshold ID,Mean AEP,Median AEP,Assurance 0.10,Assurance 0.04,Assurance 0.02,Assurance 0.01,LT Risk 10yr,LT Risk 30yr,LT Risk 50yr");

        // Alternative Results header
        _alternativeResults.AppendLine("Study ID,Alternative Name,Impact Area ID,Base Year,Future Year,Period of Analysis,Mean Base EAD,Mean Future EAD,Mean EqAD,EqAD 25th Pct,EqAD 50th Pct,EqAD 75th Pct");

        // Alternative Damage by Category header
        _alternativeDamageByCategory.AppendLine("Study ID,Alternative Name,Impact Area ID,Damage Category,Asset Category,Mean EqAD");

        // Stage Damage Summary header
        _stageDamageSummary.AppendLine("Study ID,Element Name,Impact Area ID,Impact Area Name,Damage Category,Asset Category,Point Count,Min Stage,Max Stage");

        // Alternative Comparison Summary header
        _altComparisonSummary.AppendLine("Study ID,Report Name,With Project Alt,Impact Area ID,Without Proj EqAD,With Proj EqAD,EqAD Reduced,EqAD Reduced 25th Pct,EqAD Reduced 50th Pct,EqAD Reduced 75th Pct,Without Proj Base EAD,With Proj Base EAD,Base EAD Reduced,Without Proj Future EAD,With Proj Future EAD,Future EAD Reduced");

        // Alternative Comparison by Category header
        _altComparisonByCategory.AppendLine("Study ID,Report Name,With Project Alt,Impact Area ID,Damage Category,Asset Category,EqAD Reduced,Base Year EAD Reduced,Future Year EAD Reduced");
    }

    /// <summary>
    /// Adds scenario results from a computation to the report.
    /// Iterates over ResultsList for direct access to each ImpactAreaScenarioResults.
    /// </summary>
    public void AddScenarioResults(string studyId, string scenarioName, ScenarioResults results)
    {
        if (results == null) return;

        try
        {
            // Iterate over the ResultsList directly for better access to impact area data
            foreach (var iaResult in results.ResultsList)
            {
                int impactAreaId = iaResult.ImpactAreaID;

                // Write aggregate EAD results for this impact area
                double meanEAD = iaResult.MeanExpectedAnnualConsequences();
                double ead25 = iaResult.ConsequencesExceededWithProbabilityQ(0.75); // 25th percentile = exceeded by 75%
                double ead50 = iaResult.ConsequencesExceededWithProbabilityQ(0.50);
                double ead75 = iaResult.ConsequencesExceededWithProbabilityQ(0.25); // 75th percentile = exceeded by 25%

                _scenarioResults.AppendLine($"{EscapeCsv(studyId)},{EscapeCsv(scenarioName)},{impactAreaId},{meanEAD:F2},{ead25:F2},{ead50:F2},{ead75:F2}");

                // Write damage by category from ConsequenceResults
                foreach (var consequence in iaResult.ConsequenceResults.ConsequenceResultList)
                {
                    string damCat = consequence.DamageCategory;
                    string assetCat = consequence.AssetCategory;
                    double categoryMeanEAD = iaResult.MeanExpectedAnnualConsequences(impactAreaId, damCat, assetCat);

                    _scenarioDamageByCategory.AppendLine($"{EscapeCsv(studyId)},{EscapeCsv(scenarioName)},{impactAreaId},{EscapeCsv(damCat)},{EscapeCsv(assetCat)},{categoryMeanEAD:F2}");
                }

                // Write performance metrics for each threshold in this impact area
                WritePerformanceMetrics(studyId, scenarioName, iaResult);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    Warning: Error extracting scenario results for CSV: {ex.Message}");
        }
    }

    private void WritePerformanceMetrics(string studyId, string scenarioName, ImpactAreaScenarioResults iaResult)
    {
        int impactAreaId = iaResult.ImpactAreaID;

        // Iterate over all available thresholds for this impact area
        foreach (var threshold in iaResult.PerformanceByThresholds.ListOfThresholds)
        {
            try
            {
                int thresholdId = threshold.ThresholdID;

                double meanAEP = iaResult.MeanAEP(thresholdId);
                double medianAEP = iaResult.MedianAEP(thresholdId);

                double assurance10 = iaResult.AssuranceOfEvent(thresholdId, 0.10);
                double assurance04 = iaResult.AssuranceOfEvent(thresholdId, 0.04);
                double assurance02 = iaResult.AssuranceOfEvent(thresholdId, 0.02);
                double assurance01 = iaResult.AssuranceOfEvent(thresholdId, 0.01);

                double ltRisk10 = iaResult.LongTermExceedanceProbability(thresholdId, 10);
                double ltRisk30 = iaResult.LongTermExceedanceProbability(thresholdId, 30);
                double ltRisk50 = iaResult.LongTermExceedanceProbability(thresholdId, 50);

                _scenarioPerformance.AppendLine($"{EscapeCsv(studyId)},{EscapeCsv(scenarioName)},{impactAreaId},{thresholdId},{meanAEP:F6},{medianAEP:F6},{assurance10:F4},{assurance04:F4},{assurance02:F4},{assurance01:F4},{ltRisk10:F4},{ltRisk30:F4},{ltRisk50:F4}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Warning: Could not extract performance metrics for threshold {threshold.ThresholdID}: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Adds alternative results from a computation to the report.
    /// </summary>
    public void AddAlternativeResults(string studyId, string alternativeName, AlternativeResults results)
    {
        if (results == null || results.IsNull) return;

        try
        {
            var impactAreaIds = results.GetImpactAreaIDs();
            var damageCategories = results.GetDamageCategories();
            var assetCategories = results.GetAssetCategories();

            int baseYear = results.AnalysisYears.Count > 0 ? results.AnalysisYears[0] : 0;
            int futureYear = results.AnalysisYears.Count > 1 ? results.AnalysisYears[1] : 0;
            int periodOfAnalysis = results.PeriodOfAnalysis;

            // Write aggregate results per impact area
            foreach (int impactAreaId in impactAreaIds)
            {
                double meanBaseEAD = results.SampleMeanBaseYearEAD(impactAreaId);
                double meanFutureEAD = results.SampleMeanFutureYearEAD(impactAreaId);
                double meanEqAD = results.SampleMeanEqad(impactAreaId);
                double eqad25 = results.EqadExceededWithProbabilityQ(0.75, impactAreaId);
                double eqad50 = results.EqadExceededWithProbabilityQ(0.50, impactAreaId);
                double eqad75 = results.EqadExceededWithProbabilityQ(0.25, impactAreaId);

                _alternativeResults.AppendLine($"{EscapeCsv(studyId)},{EscapeCsv(alternativeName)},{impactAreaId},{baseYear},{futureYear},{periodOfAnalysis},{meanBaseEAD:F2},{meanFutureEAD:F2},{meanEqAD:F2},{eqad25:F2},{eqad50:F2},{eqad75:F2}");
            }

            // Write damage by category
            foreach (int impactAreaId in impactAreaIds)
            {
                foreach (string damCat in damageCategories)
                {
                    foreach (string assetCat in assetCategories)
                    {
                        double meanEqAD = results.SampleMeanEqad(impactAreaId, damCat, assetCat);
                        if (meanEqAD != 0) // Only write non-zero values
                        {
                            _alternativeDamageByCategory.AppendLine($"{EscapeCsv(studyId)},{EscapeCsv(alternativeName)},{impactAreaId},{EscapeCsv(damCat)},{EscapeCsv(assetCat)},{meanEqAD:F2}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    Warning: Error extracting alternative results for CSV: {ex.Message}");
        }
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

                // Get curve data points
                int pointCount = curve.Xvals?.Length ?? 0;
                double minStage = pointCount > 0 ? curve.Xvals!.Min() : 0;
                double maxStage = pointCount > 0 ? curve.Xvals!.Max() : 0;

                _stageDamageSummary.AppendLine($"{EscapeCsv(studyId)},{EscapeCsv(elementName)},{impactAreaId},,{EscapeCsv(damCat)},{EscapeCsv(assetCat)},{pointCount},{minStage:F2},{maxStage:F2}");
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
                // Write aggregated summary per impact area
                foreach (int impactAreaId in impactAreaIds)
                {
                    // EqAD values
                    double withoutProjEqad = results.SampleMeanWithoutProjectEqad(impactAreaId);
                    double withProjEqad = results.SampleMeanWithProjectEqad(altId, impactAreaId);
                    double eqadReduced = results.SampleMeanEqadReduced(altId, impactAreaId);
                    double eqadReduced25 = results.EqadReducedExceededWithProbabilityQ(0.75, altId, impactAreaId);
                    double eqadReduced50 = results.EqadReducedExceededWithProbabilityQ(0.50, altId, impactAreaId);
                    double eqadReduced75 = results.EqadReducedExceededWithProbabilityQ(0.25, altId, impactAreaId);

                    // Base year EAD values
                    double withoutProjBaseEad = results.SampleMeanWithoutProjectBaseYearEAD(impactAreaId);
                    double withProjBaseEad = results.SampleMeanWithProjectBaseYearEAD(altId, impactAreaId);
                    double baseEadReduced = results.SampleMeanBaseYearEADReduced(altId, impactAreaId);

                    // Future year EAD values
                    double withoutProjFutureEad = results.SampleMeanWithoutProjectFutureYearEAD(impactAreaId);
                    double withProjFutureEad = results.SampleMeanWithProjectFutureYearEAD(altId, impactAreaId);
                    double futureEadReduced = results.SampleMeanFutureYearEADReduced(altId, impactAreaId);

                    _altComparisonSummary.AppendLine($"{EscapeCsv(studyId)},{EscapeCsv(reportName)},{EscapeCsv(altName)},{impactAreaId},{withoutProjEqad:F2},{withProjEqad:F2},{eqadReduced:F2},{eqadReduced25:F2},{eqadReduced50:F2},{eqadReduced75:F2},{withoutProjBaseEad:F2},{withProjBaseEad:F2},{baseEadReduced:F2},{withoutProjFutureEad:F2},{withProjFutureEad:F2},{futureEadReduced:F2}");
                }

                // Write by category breakdown
                foreach (int impactAreaId in impactAreaIds)
                {
                    foreach (string damCat in damageCategories)
                    {
                        foreach (string assetCat in assetCategories)
                        {
                            double eqadReduced = results.SampleMeanEqadReduced(altId, impactAreaId, damCat, assetCat);
                            double baseEadReduced = results.SampleMeanBaseYearEADReduced(altId, impactAreaId, damCat, assetCat);
                            double futureEadReduced = results.SampleMeanFutureYearEADReduced(altId, impactAreaId, damCat, assetCat);

                            // Only write non-zero values
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
        report.Append(_scenarioResults);
        report.AppendLine();

        report.AppendLine("=== SCENARIO DAMAGE BY CATEGORY ===");
        report.Append(_scenarioDamageByCategory);
        report.AppendLine();

        report.AppendLine("=== SCENARIO PERFORMANCE ===");
        report.Append(_scenarioPerformance);
        report.AppendLine();

        report.AppendLine("=== ALTERNATIVE RESULTS ===");
        report.Append(_alternativeResults);
        report.AppendLine();

        report.AppendLine("=== ALTERNATIVE DAMAGE BY CATEGORY ===");
        report.Append(_alternativeDamageByCategory);
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
