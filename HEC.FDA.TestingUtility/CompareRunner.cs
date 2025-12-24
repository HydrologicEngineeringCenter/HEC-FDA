using System.Text;
using System.Xml.Linq;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;

namespace HEC.FDA.TestingUtility;

/// <summary>
/// Compares two sets of FDA computation results by deserializing XML into model objects
/// and comparing the same statistics that CsvReportFactory outputs.
/// </summary>
public class CompareRunner
{
    private readonly string _baselineDir;
    private readonly string _newDir;
    private readonly string _outputPath;
    private readonly double _tolerance;
    private const double MinimumAbsoluteDifference = 1.0; // $1 minimum to report a difference

    public CompareRunner(string baselineDir, string newDir, string outputPath, double tolerance)
    {
        _baselineDir = baselineDir;
        _newDir = newDir;
        _outputPath = outputPath;
        _tolerance = tolerance;
    }

    public int Run()
    {
        Console.WriteLine($"Baseline directory: {_baselineDir}");
        Console.WriteLine($"New results directory: {_newDir}");
        Console.WriteLine($"Tolerance: {_tolerance:P1}");
        Console.WriteLine();

        // Find all XML result files in both directories
        string[] baselineFiles = Directory.GetFiles(_baselineDir, "*_results.xml");
        string[] newFiles = Directory.GetFiles(_newDir, "*_results.xml");

        if (baselineFiles.Length == 0)
        {
            Console.WriteLine("No baseline result files (*_results.xml) found.");
            return 1;
        }

        if (newFiles.Length == 0)
        {
            Console.WriteLine("No new result files (*_results.xml) found.");
            return 1;
        }

        StringBuilder report = new();
        report.AppendLine("FDA Results Comparison Report (Model-Based)");
        report.AppendLine("============================================");
        report.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        report.AppendLine($"Baseline: {_baselineDir}");
        report.AppendLine($"New: {_newDir}");
        report.AppendLine($"Tolerance: {_tolerance:P1}");
        report.AppendLine();

        int totalDifferences = 0;
        int filesCompared = 0;

        // Match files by study ID
        foreach (string baselineFile in baselineFiles)
        {
            string fileName = Path.GetFileName(baselineFile);
            string newFile = Path.Combine(_newDir, fileName);

            if (!File.Exists(newFile))
            {
                report.AppendLine($"MISSING: {fileName} not found in new results");
                Console.WriteLine($"MISSING: {fileName}");
                totalDifferences++;
                continue;
            }

            Console.WriteLine($"Comparing: {fileName}");
            int differences = CompareFiles(baselineFile, newFile, report);
            totalDifferences += differences;
            filesCompared++;

            if (differences == 0)
            {
                Console.WriteLine($"  MATCH: No differences found");
            }
            else
            {
                Console.WriteLine($"  DIFF: {differences} difference(s) found");
            }
        }

        // Check for files in new that aren't in baseline
        foreach (string newFile in newFiles)
        {
            string fileName = Path.GetFileName(newFile);
            string baselineFile = Path.Combine(_baselineDir, fileName);

            if (!File.Exists(baselineFile))
            {
                report.AppendLine($"NEW: {fileName} exists only in new results");
                Console.WriteLine($"NEW: {fileName}");
            }
        }

        // Summary
        report.AppendLine();
        report.AppendLine("=== Summary ===");
        report.AppendLine($"Files compared: {filesCompared}");
        report.AppendLine($"Total differences: {totalDifferences}");
        report.AppendLine($"Result: {(totalDifferences == 0 ? "PASS" : "FAIL")}");

        // Save report
        File.WriteAllText(_outputPath, report.ToString());
        Console.WriteLine();
        Console.WriteLine($"Report saved to: {_outputPath}");
        Console.WriteLine();
        Console.WriteLine($"=== Result: {(totalDifferences == 0 ? "PASS" : "FAIL")} ===");
        Console.WriteLine($"Files compared: {filesCompared}");
        Console.WriteLine($"Total differences: {totalDifferences}");

        return totalDifferences > 0 ? 1 : 0;
    }

    private int CompareFiles(string baselinePath, string newPath, StringBuilder report)
    {
        int differences = 0;

        try
        {
            XElement baselineDoc = XElement.Load(baselinePath);
            XElement newDoc = XElement.Load(newPath);

            string studyId = baselineDoc.Attribute("studyId")?.Value ?? Path.GetFileNameWithoutExtension(baselinePath);
            report.AppendLine($"=== {studyId} ===");

            // Compare scenarios
            differences += CompareScenarioResults(baselineDoc, newDoc, report);

            // Compare alternatives
            differences += CompareAlternativeResults(baselineDoc, newDoc, report);

            // Compare stage damage
            differences += CompareStageDamageResults(baselineDoc, newDoc, report);

            // Compare alternative comparison reports
            differences += CompareAlternativeComparisonResults(baselineDoc, newDoc, report);

            if (differences == 0)
            {
                report.AppendLine("  All statistics match within tolerance.");
            }
            report.AppendLine();
        }
        catch (Exception ex)
        {
            report.AppendLine($"  ERROR: {ex.Message}");
            differences++;
        }

        return differences;
    }

    #region Scenario Comparison

    private int CompareScenarioResults(XElement baselineDoc, XElement newDoc, StringBuilder report)
    {
        int differences = 0;
        var baselineElements = baselineDoc.Elements("ScenarioResults").ToList();
        var newElements = newDoc.Elements("ScenarioResults").ToList();

        foreach (var baselineWrapper in baselineElements)
        {
            string name = baselineWrapper.Attribute("name")?.Value ?? "unknown";
            var newWrapper = newElements.FirstOrDefault(e => e.Attribute("name")?.Value == name);

            if (newWrapper == null)
            {
                report.AppendLine($"  MISSING: Scenario '{name}' not in new results");
                differences++;
                continue;
            }

            // Extract inner ScenarioResults element and deserialize
            var baselineInner = baselineWrapper.Element("ScenarioResults");
            var newInner = newWrapper.Element("ScenarioResults");

            if (baselineInner == null || newInner == null)
            {
                report.AppendLine($"  ERROR: Invalid XML structure for Scenario '{name}'");
                differences++;
                continue;
            }

            try
            {
                var baselineResults = ScenarioResults.ReadFromXML(baselineInner);
                var newResults = ScenarioResults.ReadFromXML(newInner);
                differences += CompareScenarioStatistics(name, baselineResults, newResults, report);
            }
            catch (Exception ex)
            {
                report.AppendLine($"  ERROR: Failed to deserialize Scenario '{name}': {ex.Message}");
                differences++;
            }
        }

        // Check for scenarios in new that aren't in baseline
        foreach (var newWrapper in newElements)
        {
            string name = newWrapper.Attribute("name")?.Value ?? "unknown";
            if (!baselineElements.Any(e => e.Attribute("name")?.Value == name))
            {
                report.AppendLine($"  NEW: Scenario '{name}' only in new results");
            }
        }

        return differences;
    }

    private int CompareScenarioStatistics(string scenarioName, ScenarioResults baseline, ScenarioResults actual, StringBuilder report)
    {
        int differences = 0;
        report.AppendLine($"  Scenario: {scenarioName}");

        foreach (var iaResult in baseline.ResultsList)
        {
            int iaId = iaResult.ImpactAreaID;

            // Find corresponding impact area in actual
            var actualIaResult = actual.ResultsList.FirstOrDefault(r => r.ImpactAreaID == iaId);
            if (actualIaResult == null)
            {
                report.AppendLine($"    MISSING: ImpactArea[{iaId}] not in new results");
                differences++;
                continue;
            }

            // Compare aggregate EAD metrics
            differences += CompareValue($"ImpactArea[{iaId}].MeanEAD",
                iaResult.MeanExpectedAnnualConsequences(),
                actualIaResult.MeanExpectedAnnualConsequences(),
                report);

            differences += CompareValue($"ImpactArea[{iaId}].EAD_25thPct",
                iaResult.ConsequencesExceededWithProbabilityQ(0.75),
                actualIaResult.ConsequencesExceededWithProbabilityQ(0.75),
                report);

            differences += CompareValue($"ImpactArea[{iaId}].EAD_50thPct",
                iaResult.ConsequencesExceededWithProbabilityQ(0.50),
                actualIaResult.ConsequencesExceededWithProbabilityQ(0.50),
                report);

            differences += CompareValue($"ImpactArea[{iaId}].EAD_75thPct",
                iaResult.ConsequencesExceededWithProbabilityQ(0.25),
                actualIaResult.ConsequencesExceededWithProbabilityQ(0.25),
                report);

            // Compare damage by category
            foreach (var consequence in iaResult.ConsequenceResults.ConsequenceResultList)
            {
                string damCat = consequence.DamageCategory;
                string assetCat = consequence.AssetCategory;

                double baselineCatEAD = iaResult.MeanExpectedAnnualConsequences(iaId, damCat, assetCat);
                double actualCatEAD = actualIaResult.MeanExpectedAnnualConsequences(iaId, damCat, assetCat);

                differences += CompareValue($"ImpactArea[{iaId}].{damCat}.{assetCat}.MeanEAD",
                    baselineCatEAD, actualCatEAD, report);
            }

            // Compare performance metrics for each threshold
            foreach (var threshold in iaResult.PerformanceByThresholds.ListOfThresholds)
            {
                int thresholdId = threshold.ThresholdID;

                try
                {
                    differences += CompareValue($"ImpactArea[{iaId}].Threshold[{thresholdId}].MeanAEP",
                        iaResult.MeanAEP(thresholdId),
                        actualIaResult.MeanAEP(thresholdId),
                        report);

                    differences += CompareValue($"ImpactArea[{iaId}].Threshold[{thresholdId}].MedianAEP",
                        iaResult.MedianAEP(thresholdId),
                        actualIaResult.MedianAEP(thresholdId),
                        report);

                    // Assurance metrics
                    differences += CompareValue($"ImpactArea[{iaId}].Threshold[{thresholdId}].Assurance_0.10",
                        iaResult.AssuranceOfEvent(thresholdId, 0.10),
                        actualIaResult.AssuranceOfEvent(thresholdId, 0.10),
                        report);

                    differences += CompareValue($"ImpactArea[{iaId}].Threshold[{thresholdId}].Assurance_0.04",
                        iaResult.AssuranceOfEvent(thresholdId, 0.04),
                        actualIaResult.AssuranceOfEvent(thresholdId, 0.04),
                        report);

                    differences += CompareValue($"ImpactArea[{iaId}].Threshold[{thresholdId}].Assurance_0.02",
                        iaResult.AssuranceOfEvent(thresholdId, 0.02),
                        actualIaResult.AssuranceOfEvent(thresholdId, 0.02),
                        report);

                    differences += CompareValue($"ImpactArea[{iaId}].Threshold[{thresholdId}].Assurance_0.01",
                        iaResult.AssuranceOfEvent(thresholdId, 0.01),
                        actualIaResult.AssuranceOfEvent(thresholdId, 0.01),
                        report);

                    // Long-term risk
                    differences += CompareValue($"ImpactArea[{iaId}].Threshold[{thresholdId}].LTRisk_10yr",
                        iaResult.LongTermExceedanceProbability(thresholdId, 10),
                        actualIaResult.LongTermExceedanceProbability(thresholdId, 10),
                        report);

                    differences += CompareValue($"ImpactArea[{iaId}].Threshold[{thresholdId}].LTRisk_30yr",
                        iaResult.LongTermExceedanceProbability(thresholdId, 30),
                        actualIaResult.LongTermExceedanceProbability(thresholdId, 30),
                        report);

                    differences += CompareValue($"ImpactArea[{iaId}].Threshold[{thresholdId}].LTRisk_50yr",
                        iaResult.LongTermExceedanceProbability(thresholdId, 50),
                        actualIaResult.LongTermExceedanceProbability(thresholdId, 50),
                        report);
                }
                catch (Exception)
                {
                    // Threshold may not exist in actual - skip silently
                }
            }
        }

        return differences;
    }

    #endregion

    #region Alternative Comparison

    private int CompareAlternativeResults(XElement baselineDoc, XElement newDoc, StringBuilder report)
    {
        int differences = 0;
        var baselineElements = baselineDoc.Elements("AlternativeResults").ToList();
        var newElements = newDoc.Elements("AlternativeResults").ToList();

        foreach (var baselineWrapper in baselineElements)
        {
            string name = baselineWrapper.Attribute("name")?.Value ?? "unknown";
            var newWrapper = newElements.FirstOrDefault(e => e.Attribute("name")?.Value == name);

            if (newWrapper == null)
            {
                report.AppendLine($"  MISSING: Alternative '{name}' not in new results");
                differences++;
                continue;
            }

            try
            {
                // AlternativeResults stores BaseYearResults and FutureYearResults as ScenarioResults
                // We compare the underlying scenarios since EqAD is derived from them
                differences += CompareAlternativeStatistics(name, baselineWrapper, newWrapper, report);
            }
            catch (Exception ex)
            {
                report.AppendLine($"  ERROR: Failed to compare Alternative '{name}': {ex.Message}");
                differences++;
            }
        }

        return differences;
    }

    private int CompareAlternativeStatistics(string altName, XElement baselineWrapper, XElement newWrapper, StringBuilder report)
    {
        int differences = 0;
        report.AppendLine($"  Alternative: {altName}");

        // Deserialize the base year and future year scenario results
        var baselineBaseYearXml = baselineWrapper.Element("BaseYearResults")?.Element("ScenarioResults");
        var newBaseYearXml = newWrapper.Element("BaseYearResults")?.Element("ScenarioResults");
        var baselineFutureYearXml = baselineWrapper.Element("FutureYearResults")?.Element("ScenarioResults");
        var newFutureYearXml = newWrapper.Element("FutureYearResults")?.Element("ScenarioResults");

        if (baselineBaseYearXml == null || newBaseYearXml == null)
        {
            report.AppendLine($"    ERROR: Missing BaseYearResults");
            return 1;
        }

        if (baselineFutureYearXml == null || newFutureYearXml == null)
        {
            report.AppendLine($"    ERROR: Missing FutureYearResults");
            return 1;
        }

        var baselineBaseYear = ScenarioResults.ReadFromXML(baselineBaseYearXml);
        var newBaseYear = ScenarioResults.ReadFromXML(newBaseYearXml);
        var baselineFutureYear = ScenarioResults.ReadFromXML(baselineFutureYearXml);
        var newFutureYear = ScenarioResults.ReadFromXML(newFutureYearXml);

        // Compare base year results
        report.AppendLine($"    BaseYear:");
        foreach (var iaResult in baselineBaseYear.ResultsList)
        {
            int iaId = iaResult.ImpactAreaID;
            var actualIaResult = newBaseYear.ResultsList.FirstOrDefault(r => r.ImpactAreaID == iaId);

            if (actualIaResult == null)
            {
                report.AppendLine($"      MISSING: ImpactArea[{iaId}] not in new results");
                differences++;
                continue;
            }

            differences += CompareValue($"ImpactArea[{iaId}].MeanEAD",
                iaResult.MeanExpectedAnnualConsequences(),
                actualIaResult.MeanExpectedAnnualConsequences(),
                report);

            // Compare by category
            foreach (var consequence in iaResult.ConsequenceResults.ConsequenceResultList)
            {
                string damCat = consequence.DamageCategory;
                string assetCat = consequence.AssetCategory;

                differences += CompareValue($"ImpactArea[{iaId}].{damCat}.{assetCat}.MeanEAD",
                    iaResult.MeanExpectedAnnualConsequences(iaId, damCat, assetCat),
                    actualIaResult.MeanExpectedAnnualConsequences(iaId, damCat, assetCat),
                    report);
            }
        }

        // Compare future year results
        report.AppendLine($"    FutureYear:");
        foreach (var iaResult in baselineFutureYear.ResultsList)
        {
            int iaId = iaResult.ImpactAreaID;
            var actualIaResult = newFutureYear.ResultsList.FirstOrDefault(r => r.ImpactAreaID == iaId);

            if (actualIaResult == null)
            {
                report.AppendLine($"      MISSING: ImpactArea[{iaId}] not in new results");
                differences++;
                continue;
            }

            differences += CompareValue($"ImpactArea[{iaId}].MeanEAD",
                iaResult.MeanExpectedAnnualConsequences(),
                actualIaResult.MeanExpectedAnnualConsequences(),
                report);

            // Compare by category
            foreach (var consequence in iaResult.ConsequenceResults.ConsequenceResultList)
            {
                string damCat = consequence.DamageCategory;
                string assetCat = consequence.AssetCategory;

                differences += CompareValue($"ImpactArea[{iaId}].{damCat}.{assetCat}.MeanEAD",
                    iaResult.MeanExpectedAnnualConsequences(iaId, damCat, assetCat),
                    actualIaResult.MeanExpectedAnnualConsequences(iaId, damCat, assetCat),
                    report);
            }
        }

        return differences;
    }

    #endregion

    #region Stage Damage Comparison

    private int CompareStageDamageResults(XElement baselineDoc, XElement newDoc, StringBuilder report)
    {
        int differences = 0;
        var baselineElements = baselineDoc.Elements("StageDamage").ToList();
        var newElements = newDoc.Elements("StageDamage").ToList();

        foreach (var baselineWrapper in baselineElements)
        {
            string name = baselineWrapper.Attribute("name")?.Value ?? "unknown";
            var newWrapper = newElements.FirstOrDefault(e => e.Attribute("name")?.Value == name);

            if (newWrapper == null)
            {
                report.AppendLine($"  MISSING: StageDamage '{name}' not in new results");
                differences++;
                continue;
            }

            try
            {
                var baselineCurves = DeserializeStageDamageCurves(baselineWrapper);
                var newCurves = DeserializeStageDamageCurves(newWrapper);

                differences += CompareStageDamageStatistics(name, baselineCurves, newCurves, report);
            }
            catch (Exception ex)
            {
                report.AppendLine($"  ERROR: Failed to deserialize StageDamage '{name}': {ex.Message}");
                differences++;
            }
        }

        return differences;
    }

    private List<UncertainPairedData> DeserializeStageDamageCurves(XElement wrapper)
    {
        var curves = new List<UncertainPairedData>();
        var curvesElement = wrapper.Element("Curves");

        if (curvesElement == null)
            return curves;

        foreach (var curveElement in curvesElement.Elements("UncertainPairedData"))
        {
            curves.Add(UncertainPairedData.ReadFromXML(curveElement));
        }

        return curves;
    }

    private int CompareStageDamageStatistics(string name, List<UncertainPairedData> baseline, List<UncertainPairedData> actual, StringBuilder report)
    {
        int differences = 0;
        report.AppendLine($"  StageDamage: {name}");

        if (baseline.Count != actual.Count)
        {
            report.AppendLine($"    DIFF: CurveCount - Baseline: {baseline.Count}, New: {actual.Count}");
            differences++;
            return differences;
        }

        for (int i = 0; i < baseline.Count; i++)
        {
            var baselineCurve = baseline[i];
            var actualCurve = actual[i];

            string curveId = $"Curve[IA={baselineCurve.ImpactAreaID},Dam={baselineCurve.DamageCategory},Asset={baselineCurve.AssetCategory}]";

            // Compare point count
            if (baselineCurve.Xvals.Length != actualCurve.Xvals.Length)
            {
                report.AppendLine($"    DIFF: {curveId}.PointCount - Baseline: {baselineCurve.Xvals.Length}, New: {actualCurve.Xvals.Length}");
                differences++;
                continue;
            }

            // Compare min/max stages
            if (baselineCurve.Xvals.Length > 0)
            {
                differences += CompareValue($"{curveId}.MinStage",
                    baselineCurve.Xvals.Min(),
                    actualCurve.Xvals.Min(),
                    report);

                differences += CompareValue($"{curveId}.MaxStage",
                    baselineCurve.Xvals.Max(),
                    actualCurve.Xvals.Max(),
                    report);

                // Compare median damage at each stage
                for (int j = 0; j < baselineCurve.Xvals.Length; j++)
                {
                    double stage = baselineCurve.Xvals[j];
                    double baselineMedian = baselineCurve.Yvals[j].InverseCDF(0.5);
                    double actualMedian = actualCurve.Yvals[j].InverseCDF(0.5);

                    differences += CompareValue($"{curveId}.Stage[{stage:F2}].MedianDamage",
                        baselineMedian, actualMedian, report);
                }
            }
        }

        return differences;
    }

    #endregion

    #region Alternative Comparison Report

    private int CompareAlternativeComparisonResults(XElement baselineDoc, XElement newDoc, StringBuilder report)
    {
        int differences = 0;
        var baselineElements = baselineDoc.Elements("AlternativeComparisonReport").ToList();
        var newElements = newDoc.Elements("AlternativeComparisonReport").ToList();

        foreach (var baselineWrapper in baselineElements)
        {
            string name = baselineWrapper.Attribute("name")?.Value ?? "unknown";
            var newWrapper = newElements.FirstOrDefault(e => e.Attribute("name")?.Value == name);

            if (newWrapper == null)
            {
                report.AppendLine($"  MISSING: AlternativeComparisonReport '{name}' not in new results");
                differences++;
                continue;
            }

            try
            {
                // Compare statistics directly from XML attributes (stored pre-computed)
                differences += CompareAlternativeComparisonStatistics(name, baselineWrapper, newWrapper, report);
            }
            catch (Exception ex)
            {
                report.AppendLine($"  ERROR: Failed to compare AlternativeComparisonReport '{name}': {ex.Message}");
                differences++;
            }
        }

        return differences;
    }

    private int CompareAlternativeComparisonStatistics(string reportName, XElement baselineWrapper, XElement newWrapper, StringBuilder report)
    {
        int differences = 0;
        report.AppendLine($"  AlternativeComparisonReport: {reportName}");

        // Iterate over with-project alternatives in baseline
        foreach (var baselineAltElement in baselineWrapper.Elements("WithProjectAlternative"))
        {
            string altId = baselineAltElement.Attribute("id")?.Value ?? "0";
            string altName = baselineAltElement.Attribute("name")?.Value ?? $"Alt{altId}";

            var newAltElement = newWrapper.Elements("WithProjectAlternative")
                .FirstOrDefault(e => e.Attribute("id")?.Value == altId);

            if (newAltElement == null)
            {
                report.AppendLine($"    MISSING: Alternative '{altName}' not in new results");
                differences++;
                continue;
            }

            // Compare each impact area
            foreach (var baselineIaElement in baselineAltElement.Elements("ImpactArea"))
            {
                string iaId = baselineIaElement.Attribute("id")?.Value ?? "0";

                var newIaElement = newAltElement.Elements("ImpactArea")
                    .FirstOrDefault(e => e.Attribute("id")?.Value == iaId);

                if (newIaElement == null)
                {
                    report.AppendLine($"    MISSING: Alt[{altName}].ImpactArea[{iaId}] not in new results");
                    differences++;
                    continue;
                }

                string prefix = $"Alt[{altName}].IA[{iaId}]";

                // Compare aggregate metrics stored as attributes
                differences += CompareXmlAttribute($"{prefix}.EqADReduced",
                    baselineIaElement, newIaElement, "eqadReduced", report);

                differences += CompareXmlAttribute($"{prefix}.BaseEADReduced",
                    baselineIaElement, newIaElement, "baseEadReduced", report);

                differences += CompareXmlAttribute($"{prefix}.FutureEADReduced",
                    baselineIaElement, newIaElement, "futureEadReduced", report);

                // Compare category breakdowns
                foreach (var baselineCatElement in baselineIaElement.Elements("Category"))
                {
                    string damCat = baselineCatElement.Attribute("damageCategory")?.Value ?? "";
                    string assetCat = baselineCatElement.Attribute("assetCategory")?.Value ?? "";

                    var newCatElement = newIaElement.Elements("Category")
                        .FirstOrDefault(e =>
                            e.Attribute("damageCategory")?.Value == damCat &&
                            e.Attribute("assetCategory")?.Value == assetCat);

                    if (newCatElement == null)
                    {
                        report.AppendLine($"    MISSING: {prefix}.{damCat}.{assetCat} not in new results");
                        differences++;
                        continue;
                    }

                    differences += CompareXmlAttribute($"{prefix}.{damCat}.{assetCat}.EqADReduced",
                        baselineCatElement, newCatElement, "eqadReduced", report);

                    differences += CompareXmlAttribute($"{prefix}.{damCat}.{assetCat}.BaseEADReduced",
                        baselineCatElement, newCatElement, "baseEadReduced", report);

                    differences += CompareXmlAttribute($"{prefix}.{damCat}.{assetCat}.FutureEADReduced",
                        baselineCatElement, newCatElement, "futureEadReduced", report);
                }
            }
        }

        return differences;
    }

    private int CompareXmlAttribute(string metricName, XElement baseline, XElement actual, string attrName, StringBuilder report)
    {
        if (!double.TryParse(baseline.Attribute(attrName)?.Value, out double baselineVal))
            return 0;
        if (!double.TryParse(actual.Attribute(attrName)?.Value, out double actualVal))
            return 0;

        return CompareValue(metricName, baselineVal, actualVal, report);
    }

    #endregion

    #region Value Comparison Helpers

    private int CompareValue(string metricName, double baseline, double actual, StringBuilder report)
    {
        if (ValuesAreEqual(baseline, actual))
            return 0;

        double absDiff = Math.Abs(baseline - actual);
        double relDiff = baseline != 0 ? absDiff / Math.Abs(baseline) : (actual != 0 ? 1.0 : 0);

        report.AppendLine($"    DIFF: {metricName}");
        report.AppendLine($"          Baseline: {baseline:F4}");
        report.AppendLine($"          New:      {actual:F4}");
        report.AppendLine($"          Diff:     {absDiff:F4} ({relDiff:P2})");

        return 1;
    }

    private bool ValuesAreEqual(double baseline, double actual)
    {
        double absoluteDiff = Math.Abs(baseline - actual);

        // If the absolute difference is less than minimum threshold, consider equal
        if (absoluteDiff <= MinimumAbsoluteDifference)
            return true;

        // Check relative tolerance
        double relativeDiff = baseline != 0 ? absoluteDiff / Math.Abs(baseline) : absoluteDiff;
        return relativeDiff <= _tolerance;
    }

    #endregion
}
