using RasMapperLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualScratchSpace.Model;
public class LifeLossPlotter
{
    private Dictionary<string, string> alternativeHydraulicsPairs = new();
    private LifeLossDB db;

    public LifeLossPlotter(LifeLossDB lldb)
    {
        db = lldb;
        alternativeHydraulicsPairs = db.CreateAlternativeHydraulicsPairs();
    }
    public Dictionary<string, PointM>? CreateSummaryZonePointPairs(string summarySetPath, string indexPointsPath)
    {
        return GeospatialHelpers.QueryPolygons(summarySetPath, indexPointsPath);
    }

    public void CreatePairedData(string[] selectedAlternatives, PointMs points, string hydraulicsPath)
    {
        foreach (string alternative in selectedAlternatives)
        {
            string associatedHydraulicsFolder = alternativeHydraulicsPairs[alternative];
            string hydraulicFilePath = hydraulicsPath + "\\" + associatedHydraulicsFolder + "\\" + associatedHydraulicsFolder + ".hdf";
            float[] wse = GeospatialHelpers.GetStageFromHDF(points, hydraulicFilePath);
        }
    }
}
