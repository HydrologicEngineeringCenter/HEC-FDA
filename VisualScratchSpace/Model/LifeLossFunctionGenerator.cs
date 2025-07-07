using HEC.FDA.Model.paireddata;
using RasMapperLib;
using Statistics.Histograms;

namespace VisualScratchSpace.Model;

/// <summary>
/// Creates a list of life loss functions for each summary zone in simulation for a given set of alternatives and hazard times
/// </summary>
public class LifeLossFunctionGenerator
{
    private readonly LifeLossDB _db;
    private readonly Dictionary<string, string> _alternativeHydraulicsPairs;
    private readonly string _summarySetName;
    private Dictionary<string, PointM> _summaryZonePointPairs; // not readonly because we reassign its pointer in CreateLifeLossFunctions, too costly to do in constructor?
    private readonly string _simulationName;
    private readonly string _topLevelHydraulicsFolder;
    private readonly string[] _alternativeNames;
    private readonly string[] _hazardTimes;

    public LifeLossFunctionGenerator(string selectedPath, Simulation simulation)
    {
        _db = new LifeLossDB(selectedPath);
        _alternativeHydraulicsPairs = _db.CreateAlternativeHydraulicsPairs();
        _summarySetName = _db.SummarySetName(simulation.Name);
        _summaryZonePointPairs = simulation.SummarySet;
        _simulationName = simulation.Name;
        _topLevelHydraulicsFolder = simulation.HydraulicsFolder;
        _alternativeNames = simulation.Alternatives.ToArray();
        _hazardTimes = simulation.HazardTimes.ToArray();
    }

    /// <summary>
    /// Generate the functions using the summary set and index point shape files (currently required for user to import)
    /// </summary>
    /// <param name="summarySetPath">Path to the summary set shape file</param>
    /// <param name="indexPointsPath">Path to the index points shape file</param>
    /// <returns></returns>
    public List<LifeLossFunction> CreateLifeLossFunctions(string summarySetPath, string indexPointsPath)
    {
        List<LifeLossFunction> lifeLossFunctions = new();

        // create the map of summary zone names to their corresponding index points
        _summaryZonePointPairs = GeospatialHelpers.QueryPolygons(summarySetPath, indexPointsPath);

        // create life loss functions for each hazard time within each summary zone
        foreach (string summaryZone in _summaryZonePointPairs.Keys)
        {
            // creating points array of size 1 because that RAS API needs an array
            // this can be avoided and I can get all stages with one API call if I can guarantee that the order is preserved upon return from RAS
            // i.e. if I pass {pt1, pt2, pt3}, is the returned array from RAS {pt1_WSE, pt2_WSE, pt3_WSE}, or could the order have been changed
            PointMs indexPoint = [_summaryZonePointPairs[summaryZone]]; 

            List<LifeLossFunction> functions = CreateLifeLossRelationships(summaryZone, indexPoint);
            lifeLossFunctions.AddRange(functions); // AddRange because we are adding a list to another list
        }
        return lifeLossFunctions;
    }

    /// <summary>
    /// Create life loss functions for a given summary zone
    /// </summary>
    /// <param name="summaryZone">Name of the summary zone</param>
    /// <param name="points">Array of points to query for stage</param>
    /// <returns></returns>
    private List<LifeLossFunction> CreateLifeLossRelationships(string summaryZone, PointMs points)
    {
        Dictionary<(string, string), DynamicHistogram> alternativeTimePairToHistogram = new(); // ((alternative name, hazard time), histogram)
        Dictionary<string, float> alternativeStagePairs = new(); // (alternativeName, stage)
        foreach (string alternative in _alternativeNames)
        {
            string associatedHydraulicsFolder = _alternativeHydraulicsPairs[alternative];
            // asserting that the HDF file name is always in this format
            string hydraulicFilePath = _topLevelHydraulicsFolder + "\\" + associatedHydraulicsFolder + "\\" + associatedHydraulicsFolder + ".hdf";
            float[] stage = GeospatialHelpers.GetStageFromHDF(points, hydraulicFilePath);
            alternativeStagePairs[alternative] = stage[0]; // GetStageFromHDF returns an array with one value in it (RAS API), so we get the first (and only) value
            foreach (string hazardTime in _hazardTimes)
            {
                string tableName = $"{_simulationName}>Results_By_Iteration>{alternative}>{hazardTime}>{_summarySetName}>{summaryZone}";
                DynamicHistogram histogram = _db.QueryLifeLossTable(tableName);
                alternativeTimePairToHistogram[(alternative, hazardTime)] = histogram;
            }   
        }
        List<LifeLossFunction> lifeLossRelationships = new();
        // make the plots
        foreach (string hazardTime in _hazardTimes)
        {
            List<double> stages = new();
            List<DynamicHistogram> histograms = new();
            // this foreach loop exists to preserve the mapping of alternatives to histograms for different times, because the histograms (but not alternatives/stage) change with time
            foreach (var ((alternative, time), histogram) in alternativeTimePairToHistogram)
            {
                if (time == hazardTime)
                {
                    stages.Add(alternativeStagePairs[alternative]);
                    histograms.Add(histogram);
                }
            }
            var stageArray = stages.ToArray();
            var histogramArray = histograms.ToArray();
            Array.Sort(stageArray, histogramArray); // this overloaded sort sorts the stages while preserving their mappings to histograms
            UncertainPairedData data = new(stageArray, histogramArray, new CurveMetaData());
            LifeLossFunction function = new(data, summaryZone, hazardTime);
            lifeLossRelationships.Add(function);
        }
        return lifeLossRelationships;
    }   
}