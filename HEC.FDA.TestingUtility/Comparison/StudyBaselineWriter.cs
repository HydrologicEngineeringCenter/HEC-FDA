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
        XElement wrapper = new("ScenarioResults",
            new XAttribute("name", name),
            results.WriteToXML());
        baseline.Add(wrapper);
    }

    public static void AddAlternativeResults(XElement baseline, string name, AlternativeResults results)
    {
        XElement wrapper = new("AlternativeResults",
            new XAttribute("name", name),
            new XElement("BaseYearResults", results.BaseYearScenarioResults.WriteToXML()),
            new XElement("FutureYearResults", results.FutureYearScenarioResults.WriteToXML()));
        baseline.Add(wrapper);
    }

    public static void AddStageDamage(XElement baseline, string name, List<UncertainPairedData> curves)
    {
        XElement curvesElement = new("Curves");
        foreach (UncertainPairedData curve in curves)
        {
            curvesElement.Add(curve.WriteToXML());
        }

        XElement wrapper = new("StageDamage",
            new XAttribute("name", name),
            new XAttribute("curveCount", curves.Count),
            curvesElement);
        baseline.Add(wrapper);
    }

    public static void AddAlternativeComparisonResults(XElement baseline, string name, AlternativeComparisonReportResults results, List<(int altId, string altName)> withProjectAlternatives)
    {
        if (results == null) return;

        XElement wrapper = new("AlternativeComparisonReport",
            new XAttribute("name", name));

        List<int> impactAreaIds = results.GetImpactAreaIDs();
        List<string> damageCategories = results.GetDamageCategories();
        List<string> assetCategories = results.GetAssetCategories();

        foreach ((int altId, string altName) in withProjectAlternatives)
        {
            XElement altElement = new("WithProjectAlternative",
                new XAttribute("id", altId),
                new XAttribute("name", altName));

            foreach (int impactAreaId in impactAreaIds)
            {
                XElement iaElement = new("ImpactArea",
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
