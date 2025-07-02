using RasMapperLib;
using Statistics.Histograms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualScratchSpace.Model;
public class LifeLossPlotter
{
    public string TopLevelHydraulicsFolder { get; set; }
    public string[] Alternatives {  get; set; }
    public string[] HazardTimes { get; set; }
    public string SimulationName { get; set; }

    private string _summarySetName;
    private Dictionary<string, string> _alternativeHydraulicsPairs = new();
    private LifeLossDB _db;

    public LifeLossPlotter(LifeLossDB lldb, string simulationName, string hydraulicsFolder, string[] alternatives, string[] hazardTimes)
    {
        _db = lldb;
        _alternativeHydraulicsPairs = _db.CreateAlternativeHydraulicsPairs();
        TopLevelHydraulicsFolder = hydraulicsFolder;
        SimulationName = simulationName;
        Alternatives = alternatives;
        HazardTimes = hazardTimes;
        _summarySetName = _db.SummarySetName(simulationName);
    }
    public static Dictionary<string, PointM>? CreateSummaryZonePointPairs(string summarySetPath, string indexPointsPath)
    {
        return GeospatialHelpers.QueryPolygons(summarySetPath, indexPointsPath);
    }

    public void CreatePairedData(string summaryZone, PointMs points)
    {
        Dictionary<(string, string), DynamicHistogram> histogramDict = new();
        Dictionary<string, float> alternativeStagePairs = new();
        foreach (string alternative in Alternatives)
        {
            string associatedHydraulicsFolder = _alternativeHydraulicsPairs[alternative];
            string hydraulicFilePath = TopLevelHydraulicsFolder + "\\" + associatedHydraulicsFolder + "\\" + associatedHydraulicsFolder + ".hdf";
            float[] stage = GeospatialHelpers.GetStageFromHDF(points, hydraulicFilePath);
            alternativeStagePairs[alternative] = stage[0];
            foreach (string hazardTime in HazardTimes)
            {
                string tableName = $"{SimulationName}>Results_By_Iteration>{alternative}>{hazardTime}>{_summarySetName}>{summaryZone}";
                DynamicHistogram histogram = _db.QueryLifeLossTable(tableName);
                histogramDict[(alternative, hazardTime)] = histogram;
            }   
        }
        // make the plots
    }
}
