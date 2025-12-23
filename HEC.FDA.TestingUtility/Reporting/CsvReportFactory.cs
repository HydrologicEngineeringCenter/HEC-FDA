using System.Text;
using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.AggregatedStageDamage;

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

                // Assurance values (probability of not exceeding standard event)
                double assurance10 = iaResult.AssuranceOfEvent(thresholdId, 0.10);
                double assurance04 = iaResult.AssuranceOfEvent(thresholdId, 0.04);
                double assurance02 = iaResult.AssuranceOfEvent(thresholdId, 0.02);
                double assurance01 = iaResult.AssuranceOfEvent(thresholdId, 0.01);

                // Long-term risk
                double ltRisk10 = iaResult.LongTermExceedanceProbability(thresholdId, 10);
                double ltRisk30 = iaResult.LongTermExceedanceProbability(thresholdId, 30);
                double ltRisk50 = iaResult.LongTermExceedanceProbability(thresholdId, 50);

                _scenarioPerformance.AppendLine($"{EscapeCsv(studyId)},{EscapeCsv(scenarioName)},{impactAreaId},{thresholdId},{meanAEP:F6},{medianAEP:F6},{assurance10:F4},{assurance04:F4},{assurance02:F4},{assurance01:F4},{ltRisk10:F4},{ltRisk30:F4},{ltRisk50:F4}");
            }
            catch
            {
                // Performance metrics may not be available for this threshold
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
    /// Adds stage damage summary from an element to the report.
    /// </summary>
    public void AddStageDamageSummary(string studyId, AggregatedStageDamageElement element)
    {
        if (element == null) return;

        try
        {
            foreach (var curve in element.Curves)
            {
                int impactAreaId = curve.ImpArea?.ID ?? -1;
                string impactAreaName = curve.ImpArea?.Name ?? "";
                string damCat = curve.DamCat ?? "";
                string assetCat = curve.AssetCategory ?? "";

                // Get curve data points
                int pointCount = 0;
                double minStage = 0;
                double maxStage = 0;

                try
                {
                    var pairedData = curve.ComputeComponent?.SelectedItemToPairedData();
                    if (pairedData != null && pairedData.Xvals != null && pairedData.Xvals.Length > 0)
                    {
                        pointCount = pairedData.Xvals.Length;
                        minStage = pairedData.Xvals.Min();
                        maxStage = pairedData.Xvals.Max();
                    }
                }
                catch
                {
                    // Curve data may not be available
                }

                _stageDamageSummary.AppendLine($"{EscapeCsv(studyId)},{EscapeCsv(element.Name)},{impactAreaId},{EscapeCsv(impactAreaName)},{EscapeCsv(damCat)},{EscapeCsv(assetCat)},{pointCount},{minStage:F2},{maxStage:F2}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    Warning: Error extracting stage damage for CSV: {ex.Message}");
        }
    }

    /// <summary>
    /// Saves the comprehensive report to a CSV file.
    /// </summary>
    public void SaveReport(string outputPath)
    {
        var report = new StringBuilder();

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
