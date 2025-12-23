using System.Xml.Linq;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;

namespace HEC.FDA.TestingUtility.Comparison;

public class StudyBaselineWriter
{
    public XElement CreateStudyBaseline(string studyId, string studyName)
    {
        return new XElement("StudyBaseline",
            new XAttribute("studyId", studyId),
            new XAttribute("studyName", studyName),
            new XAttribute("createdDate", DateTime.Now.ToString("yyyy-MM-dd")));
    }

    public void AddScenarioResults(XElement baseline, string name, ScenarioResults results)
    {
        var wrapper = new XElement("ScenarioResults",
            new XAttribute("name", name),
            results.WriteToXML());
        baseline.Add(wrapper);
    }

    public void AddAlternativeResults(XElement baseline, string name, AlternativeResults results)
    {
        var wrapper = new XElement("AlternativeResults",
            new XAttribute("name", name),
            new XElement("BaseYearResults", results.BaseYearScenarioResults.WriteToXML()),
            new XElement("FutureYearResults", results.FutureYearScenarioResults.WriteToXML()));
        baseline.Add(wrapper);
    }

    public void AddStageDamage(XElement baseline, string name, List<UncertainPairedData> curves)
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

    public void Save(XElement baseline, string path)
    {
        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        baseline.Save(path);
    }
}
