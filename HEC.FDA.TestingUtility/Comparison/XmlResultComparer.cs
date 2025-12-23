using System.Xml.Linq;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;

namespace HEC.FDA.TestingUtility.Comparison;

public class XmlResultComparer
{
    private XElement? _baselineDoc;

    public void LoadBaseline(string baselinePath)
    {
        if (!File.Exists(baselinePath))
        {
            throw new FileNotFoundException($"Baseline file not found: {baselinePath}");
        }
        _baselineDoc = XElement.Load(baselinePath);
    }

    public ComparisonResult CompareScenarioResults(string elementName, ScenarioResults actual)
    {
        var result = new ComparisonResult { ElementName = elementName, ElementType = "Scenario" };

        if (_baselineDoc == null)
        {
            result.Passed = false;
            result.ErrorMessage = "Baseline not loaded";
            return result;
        }

        // Find the baseline element by name
        var baselineElement = _baselineDoc.Elements("ScenarioResults")
            .FirstOrDefault(e => e.Attribute("name")?.Value == elementName);

        if (baselineElement == null)
        {
            result.Passed = false;
            result.ErrorMessage = $"Baseline not found for Scenario '{elementName}'";
            return result;
        }

        // Get the inner ScenarioResults XML
        var innerXml = baselineElement.Element("ScenarioResults");
        if (innerXml == null)
        {
            result.Passed = false;
            result.ErrorMessage = $"Invalid baseline format for Scenario '{elementName}'";
            return result;
        }

        ScenarioResults baseline = ScenarioResults.ReadFromXML(innerXml);

        // Use built-in Equals method
        result.Passed = actual.Equals(baseline);

        if (!result.Passed)
        {
            GenerateScenarioDiff(baseline, actual, result);
        }

        return result;
    }

    public ComparisonResult CompareAlternativeResults(string elementName, AlternativeResults actual)
    {
        var result = new ComparisonResult { ElementName = elementName, ElementType = "Alternative" };

        if (_baselineDoc == null)
        {
            result.Passed = false;
            result.ErrorMessage = "Baseline not loaded";
            return result;
        }

        var baselineElement = _baselineDoc.Elements("AlternativeResults")
            .FirstOrDefault(e => e.Attribute("name")?.Value == elementName);

        if (baselineElement == null)
        {
            result.Passed = false;
            result.ErrorMessage = $"Baseline not found for Alternative '{elementName}'";
            return result;
        }

        result.Passed = true;

        // Compare base year scenario results
        var baseYearXml = baselineElement.Element("BaseYearResults")?.Element("ScenarioResults");
        if (baseYearXml != null)
        {
            var baselineBaseYear = ScenarioResults.ReadFromXML(baseYearXml);
            bool baseYearMatch = actual.BaseYearScenarioResults.Equals(baselineBaseYear);
            result.Passed &= baseYearMatch;

            if (!baseYearMatch)
            {
                result.Differences.Add(new Difference
                {
                    Metric = "BaseYearResults",
                    Expected = null,
                    Actual = null
                });
            }
        }

        // Compare future year scenario results
        var futureYearXml = baselineElement.Element("FutureYearResults")?.Element("ScenarioResults");
        if (futureYearXml != null)
        {
            var baselineFutureYear = ScenarioResults.ReadFromXML(futureYearXml);
            bool futureYearMatch = actual.FutureYearScenarioResults.Equals(baselineFutureYear);
            result.Passed &= futureYearMatch;

            if (!futureYearMatch)
            {
                result.Differences.Add(new Difference
                {
                    Metric = "FutureYearResults",
                    Expected = null,
                    Actual = null
                });
            }
        }

        return result;
    }

    public ComparisonResult CompareStageDamage(string elementName, List<UncertainPairedData> actualCurves)
    {
        var result = new ComparisonResult { ElementName = elementName, ElementType = "StageDamage" };

        if (_baselineDoc == null)
        {
            result.Passed = false;
            result.ErrorMessage = "Baseline not loaded";
            return result;
        }

        var baselineElement = _baselineDoc.Elements("StageDamage")
            .FirstOrDefault(e => e.Attribute("name")?.Value == elementName);

        if (baselineElement == null)
        {
            result.Passed = false;
            result.ErrorMessage = $"Baseline not found for StageDamage '{elementName}'";
            return result;
        }

        // Get baseline curves
        var curvesElement = baselineElement.Element("Curves");
        if (curvesElement == null)
        {
            result.Passed = false;
            result.ErrorMessage = $"Invalid baseline format for StageDamage '{elementName}' - no Curves element";
            return result;
        }

        var baselineCurveElements = curvesElement.Elements("UncertainPairedData").ToList();

        // Compare curve counts
        if (baselineCurveElements.Count != actualCurves.Count)
        {
            result.Passed = false;
            result.Differences.Add(new Difference
            {
                Metric = "CurveCount",
                Expected = baselineCurveElements.Count,
                Actual = actualCurves.Count
            });
            return result;
        }

        result.Passed = true;

        // Compare each curve
        for (int i = 0; i < actualCurves.Count; i++)
        {
            var baselineCurve = UncertainPairedData.ReadFromXML(baselineCurveElements[i]);
            var actualCurve = actualCurves[i];

            // Compare metadata
            if (baselineCurve.ImpactAreaID != actualCurve.ImpactAreaID ||
                baselineCurve.DamageCategory != actualCurve.DamageCategory ||
                baselineCurve.AssetCategory != actualCurve.AssetCategory)
            {
                result.Passed = false;
                result.Differences.Add(new Difference
                {
                    Metric = $"Curve[{i}].Metadata",
                    ExpectedDescription = $"IA={baselineCurve.ImpactAreaID}, DamCat={baselineCurve.DamageCategory}, Asset={baselineCurve.AssetCategory}",
                    ActualDescription = $"IA={actualCurve.ImpactAreaID}, DamCat={actualCurve.DamageCategory}, Asset={actualCurve.AssetCategory}"
                });
                continue;
            }

            // Compare X values (stages)
            if (baselineCurve.Xvals.Length != actualCurve.Xvals.Length)
            {
                result.Passed = false;
                result.Differences.Add(new Difference
                {
                    Metric = $"Curve[{i}].XValueCount",
                    Expected = baselineCurve.Xvals.Length,
                    Actual = actualCurve.Xvals.Length
                });
                continue;
            }

            // Compare mean damage values at each stage
            for (int j = 0; j < baselineCurve.Xvals.Length; j++)
            {
                double baselineMean = baselineCurve.Yvals[j].InverseCDF(0.5);
                double actualMean = actualCurve.Yvals[j].InverseCDF(0.5);

                double tolerance = 0.01;
                double absoluteDiff = Math.Abs(baselineMean - actualMean);
                double relativeDiff = baselineMean != 0 ? absoluteDiff / Math.Abs(baselineMean) : absoluteDiff;

                if (relativeDiff > tolerance && absoluteDiff > 1.0)
                {
                    result.Passed = false;
                    result.Differences.Add(new Difference
                    {
                        Metric = $"Curve[{i}].Stage[{baselineCurve.Xvals[j]:F2}].MedianDamage",
                        Expected = baselineMean,
                        Actual = actualMean
                    });
                }
            }
        }

        return result;
    }

    private static bool XmlCompare(XElement expected, XElement actual)
    {
        // Simple XML comparison - compare serialized strings
        // This is a basic comparison; a more sophisticated comparison could be added
        return XNode.DeepEquals(expected, actual);
    }

    private static void GenerateScenarioDiff(ScenarioResults baseline, ScenarioResults actual, ComparisonResult result)
    {
        // Compare mean EAD for each impact area / damage category
        foreach (int iaId in baseline.GetImpactAreaIDs())
        {
            foreach (string damCat in baseline.GetDamageCategories())
            {
                foreach (string assetCat in baseline.GetAssetCategories())
                {
                    double baselineMean = baseline.SampleMeanExpectedAnnualConsequences(iaId, damCat, assetCat);
                    double actualMean = actual.SampleMeanExpectedAnnualConsequences(iaId, damCat, assetCat);

                    double tolerance = 0.01; // 1% relative tolerance for small values
                    double absoluteDiff = Math.Abs(baselineMean - actualMean);
                    double relativeDiff = baselineMean != 0 ? absoluteDiff / Math.Abs(baselineMean) : absoluteDiff;

                    if (relativeDiff > tolerance && absoluteDiff > 1.0) // At least $1 difference
                    {
                        result.Differences.Add(new Difference
                        {
                            Metric = $"MeanEAD[IA={iaId},DamCat={damCat},Asset={assetCat}]",
                            Expected = baselineMean,
                            Actual = actualMean
                        });
                    }
                }
            }
        }
    }
}
