using System.Xml.Linq;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;

namespace HEC.FDA.TestingUtility.Comparison;

public class XmlResultComparer
{
    private const double RelativeTolerance = 0.01; // 1% relative tolerance
    private const double MinimumAbsoluteDifference = 1.0; // $1 minimum to consider a difference

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

                if (!ValuesAreEqual(baselineMean, actualMean))
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

    private static bool ValuesAreEqual(double expected, double actual)
    {
        double absoluteDiff = Math.Abs(expected - actual);
        if (absoluteDiff <= MinimumAbsoluteDifference) return true;

        double relativeDiff = expected != 0 ? absoluteDiff / Math.Abs(expected) : absoluteDiff;
        return relativeDiff <= RelativeTolerance;
    }

    private static void GenerateScenarioDiff(ScenarioResults baseline, ScenarioResults actual, ComparisonResult result)
    {
        foreach (int iaId in baseline.GetImpactAreaIDs())
        {
            foreach (string damCat in baseline.GetDamageCategories())
            {
                foreach (string assetCat in baseline.GetAssetCategories())
                {
                    double baselineMean = baseline.SampleMeanExpectedAnnualConsequences(iaId, damCat, assetCat);
                    double actualMean = actual.SampleMeanExpectedAnnualConsequences(iaId, damCat, assetCat);

                    if (!ValuesAreEqual(baselineMean, actualMean))
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

    public ComparisonResult CompareAlternativeComparisonResults(string elementName, AlternativeComparisonReportResults actual, List<(int altId, string altName)> withProjectAlternatives)
    {
        var result = new ComparisonResult { ElementName = elementName, ElementType = "AlternativeComparisonReport" };

        if (_baselineDoc == null)
        {
            result.Passed = false;
            result.ErrorMessage = "Baseline not loaded";
            return result;
        }

        var baselineElement = _baselineDoc.Elements("AlternativeComparisonReport")
            .FirstOrDefault(e => e.Attribute("name")?.Value == elementName);

        if (baselineElement == null)
        {
            result.Passed = false;
            result.ErrorMessage = $"Baseline not found for AlternativeComparisonReport '{elementName}'";
            return result;
        }

        result.Passed = true;

        foreach (var (altId, altName) in withProjectAlternatives)
        {
            var baselineAltElement = baselineElement.Elements("WithProjectAlternative")
                .FirstOrDefault(e => e.Attribute("id")?.Value == altId.ToString());

            if (baselineAltElement == null)
            {
                result.Passed = false;
                result.Differences.Add(new Difference
                {
                    Metric = $"WithProjectAlternative[{altName}]",
                    ExpectedDescription = "present in baseline",
                    ActualDescription = "missing"
                });
                continue;
            }

            foreach (var baselineIaElement in baselineAltElement.Elements("ImpactArea"))
            {
                int impactAreaId = int.Parse(baselineIaElement.Attribute("id")?.Value ?? "0");

                // Compare EqAD Reduced
                double baselineEqadReduced = double.Parse(baselineIaElement.Attribute("eqadReduced")?.Value ?? "0");
                double actualEqadReduced = actual.SampleMeanEqadReduced(altId, impactAreaId);

                if (!ValuesAreEqual(baselineEqadReduced, actualEqadReduced))
                {
                    result.Passed = false;
                    result.Differences.Add(new Difference
                    {
                        Metric = $"EqadReduced[Alt={altName},IA={impactAreaId}]",
                        Expected = baselineEqadReduced,
                        Actual = actualEqadReduced
                    });
                }

                // Compare Base EAD Reduced
                double baselineBaseEadReduced = double.Parse(baselineIaElement.Attribute("baseEadReduced")?.Value ?? "0");
                double actualBaseEadReduced = actual.SampleMeanBaseYearEADReduced(altId, impactAreaId);

                if (!ValuesAreEqual(baselineBaseEadReduced, actualBaseEadReduced))
                {
                    result.Passed = false;
                    result.Differences.Add(new Difference
                    {
                        Metric = $"BaseEadReduced[Alt={altName},IA={impactAreaId}]",
                        Expected = baselineBaseEadReduced,
                        Actual = actualBaseEadReduced
                    });
                }

                // Compare Future EAD Reduced
                double baselineFutureEadReduced = double.Parse(baselineIaElement.Attribute("futureEadReduced")?.Value ?? "0");
                double actualFutureEadReduced = actual.SampleMeanFutureYearEADReduced(altId, impactAreaId);

                if (!ValuesAreEqual(baselineFutureEadReduced, actualFutureEadReduced))
                {
                    result.Passed = false;
                    result.Differences.Add(new Difference
                    {
                        Metric = $"FutureEadReduced[Alt={altName},IA={impactAreaId}]",
                        Expected = baselineFutureEadReduced,
                        Actual = actualFutureEadReduced
                    });
                }
            }
        }

        return result;
    }
}
