using System.Xml.Linq;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;

namespace HEC.FDA.TestingUtility.Comparison;

public static class StudyBaselineWriter
{
    public static XElement CreateStudyBaseline(string studyId, string studyName)
    {
        return new XElement("StudyBaseline",
            new XAttribute("studyId", studyId),
            new XAttribute("studyName", studyName),
            new XAttribute("createdDate", DateTime.Now.ToString("yyyy-MM-dd")));
    }

    public static void AddScenarioResults(XElement baseline, string name, ScenarioResults results)
    {
        var wrapper = new XElement("ScenarioResults",
            new XAttribute("name", name),
            results.WriteToXML());
        baseline.Add(wrapper);
    }

    public static void AddAlternativeResults(XElement baseline, string name, AlternativeResults results)
    {
        var wrapper = new XElement("AlternativeResults",
            new XAttribute("name", name),
            new XElement("BaseYearResults", results.BaseYearScenarioResults.WriteToXML()),
            new XElement("FutureYearResults", results.FutureYearScenarioResults.WriteToXML()));
        baseline.Add(wrapper);
    }

    public static void AddStageDamage(XElement baseline, string name, List<UncertainPairedData> curves)
    {
        var curvesElement = new XElement("Curves");
        foreach (var curve in curves)
        {
            curvesElement.Add(curve.WriteToXML());
        }

        var wrapper = new XElement("StageDamage",
            new XAttribute("name", name),
            new XAttribute("curveCount", curves.Count),
            curvesElement);
        baseline.Add(wrapper);
    }

    public static void AddAlternativeComparisonResults(XElement baseline, string name, AlternativeComparisonReportResults results, List<(int altId, string altName)> withProjectAlternatives)
    {
        if (results == null) return;

        var wrapper = new XElement("AlternativeComparisonReport",
            new XAttribute("name", name));

        var impactAreaIds = results.GetImpactAreaIDs();
        var damageCategories = results.GetDamageCategories();
        var assetCategories = results.GetAssetCategories();

        foreach (var (altId, altName) in withProjectAlternatives)
        {
            var altElement = new XElement("WithProjectAlternative",
                new XAttribute("id", altId),
                new XAttribute("name", altName));

            foreach (int impactAreaId in impactAreaIds)
            {
                var iaElement = new XElement("ImpactArea",
                    new XAttribute("id", impactAreaId),
                    new XAttribute("eqadReduced", results.SampleMeanEqadReduced(altId, impactAreaId)),
                    new XAttribute("baseEadReduced", results.SampleMeanBaseYearEADReduced(altId, impactAreaId)),
                    new XAttribute("futureEadReduced", results.SampleMeanFutureYearEADReduced(altId, impactAreaId)));

                // Add category breakdowns
                foreach (string damCat in damageCategories)
                {
                    foreach (string assetCat in assetCategories)
                    {
                        double eqadReduced = results.SampleMeanEqadReduced(altId, impactAreaId, damCat, assetCat);
                        if (eqadReduced != 0)
                        {
                            iaElement.Add(new XElement("Category",
                                new XAttribute("damageCategory", damCat),
                                new XAttribute("assetCategory", assetCat),
                                new XAttribute("eqadReduced", eqadReduced),
                                new XAttribute("baseEadReduced", results.SampleMeanBaseYearEADReduced(altId, impactAreaId, damCat, assetCat)),
                                new XAttribute("futureEadReduced", results.SampleMeanFutureYearEADReduced(altId, impactAreaId, damCat, assetCat))));
                        }
                    }
                }

                altElement.Add(iaElement);
            }

            wrapper.Add(altElement);
        }

        baseline.Add(wrapper);
    }

    public static void Save(XElement baseline, string path)
    {
        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        baseline.Save(path);
    }
}
