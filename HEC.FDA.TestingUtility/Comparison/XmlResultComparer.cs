using System.Xml.Linq;
using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.AggregatedStageDamage;

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

    public ComparisonResult CompareStageDamage(string elementName, AggregatedStageDamageElement actual)
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

        // Use element's Equals method for comparison
        var innerXml = baselineElement.Elements().FirstOrDefault();
        if (innerXml == null)
        {
            result.Passed = false;
            result.ErrorMessage = $"Invalid baseline format for StageDamage '{elementName}'";
            return result;
        }

        // Note: AggregatedStageDamageElement.Equals needs to be called on the actual element
        // The baseline needs to be reconstructed from XML - this may require additional work
        // For now, we'll compare the XML directly
        var actualXml = actual.ToXML();
        result.Passed = XmlCompare(innerXml, actualXml);

        if (!result.Passed)
        {
            result.ErrorMessage = "Stage damage elements do not match";
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
