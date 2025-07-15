using HEC.FDA.Model.alternatives;
using HEC.FDA.Model.paireddata;
using RasMapperLib;
using SciChart.Core.Extensions;
using Statistics.Histograms;
using System.IO;
using VisualScratchSpace.Model.Saving;

namespace VisualScratchSpace.Model;

/// <summary>
/// Creates a list of life loss functions for each summary zone in simulation for a given set of alternatives and hazard times
/// </summary>
public class LifeLossFunctionGenerator
{
    private readonly LifeLossDB _db;
    private readonly LifeLossPlotSaver _saver = new(@"C:\FDA_Test_Data\WKS20230525\WKS20230525\save-test.db");
    private readonly Dictionary<string, string> _hydraulicsFolderByAlternative;
    private readonly string _summarySetName;
    private Dictionary<string, PointM> _indexPointBySummaryZone; // not readonly because we reassign its pointer in CreateLifeLossFunctions, too costly to do in constructor?
    private readonly string _simulationName;
    private readonly string _topLevelHydraulicsFolder;
    private readonly string[] _alternativeNames;
    private readonly string[] _hazardTimes;

    public LifeLossFunctionGenerator(string selectedPath, Simulation simulation)
    {
        _db = new LifeLossDB(selectedPath);
        _hydraulicsFolderByAlternative = _db.CreateAlternativeHydraulicsPairs();
        _summarySetName = _db.SummarySetName(simulation.Name);
        _indexPointBySummaryZone = simulation.SummarySet;
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
        _indexPointBySummaryZone = GeospatialHelpers.QueryPolygons(summarySetPath, indexPointsPath);

        // create life loss functions for each hazard time within each summary zone
        foreach (string summaryZone in _indexPointBySummaryZone.Keys)
        {
            // creating points array of size 1 because that RAS API needs an array
            PointMs indexPoint = [_indexPointBySummaryZone[summaryZone]]; 

            List<LifeLossFunction> functions = CreateLifeLossFunctionsForSummaryZone(summaryZone, indexPoint);
            lifeLossFunctions.AddRange(functions); // AddRange because we are adding a list to another list
        }
        return lifeLossFunctions;
    }

    /// <summary>
    /// Return a list of life loss functions for a given summary zone
    /// </summary>
    /// <param name="summaryZone"></param>
    /// <param name="indexPoint"></param>
    /// <returns></returns>
    private List<LifeLossFunction> CreateLifeLossFunctionsForSummaryZone(string summaryZone, PointMs indexPoint)
    {
        // if any of the functions we are asking for are already present in the database, grab them first
        PlotFilter allPF = new()
        {
            Simulation = [_simulationName],
            Summary_Zone = [summaryZone],
            Alternative = _alternativeNames,
            Hazard_Time = _hazardTimes,
        };
        List<LifeLossFunction> existingFunctions = _saver.ReadFromSQLite(allPF);
        // set up dictionaries for stages and histograms already in the DB, allows us to make O(1) lookups instead of recomputing
        var seenStages = new Dictionary<(string simulation, string summaryZone, string alternative), double>();
        var seenHistograms = new Dictionary<(string simulation, string summaryZone, string alternative, string hazardTime), DynamicHistogram>();
        foreach (LifeLossFunction llf  in existingFunctions)
        {
            for (int i = 0; i < llf.AlternativeNames.Length; i++)
            {
                seenStages[(llf.SimulationName, llf.SummaryZone, llf.AlternativeNames[i])] = llf.Data.Xvals[i];
                seenHistograms[(llf.SimulationName, llf.SummaryZone, llf.AlternativeNames[i], llf.HazardTime)] = (DynamicHistogram)llf.Data.Yvals[i];
            }
        }

        bool newEntries = false; // if this flag remains false, we can just return the existing functions because we only read functions already in the DB
        foreach (string hazardTime in _hazardTimes)
        {
            // build one function
            bool newEntry = false; // this flag signals whether the function being built has any new entries not already present in the DB
            List<double> stages = new();
            List<DynamicHistogram> histograms = new();
            foreach (string alternative in _alternativeNames)
            {
                if (!seenStages.TryGetValue((_simulationName, summaryZone, alternative), out double stage)) // checking if the stage already exists in the DB
                {
                    newEntry = true; newEntries = true;
                    string associatedHydraulicsFolder = _hydraulicsFolderByAlternative[alternative];
                    string hdf = Path.Combine(_topLevelHydraulicsFolder, associatedHydraulicsFolder, $"{associatedHydraulicsFolder}.hdf"); // asserting that the HDF file name is always in this format
                    float[] computedStage = GeospatialHelpers.GetStageFromHDF(indexPoint, hdf);
                    stage = (double)computedStage[0]; // GetStageFromHDF returns an array with one value in it (RAS API), so we get the first (and only) value
                    seenStages[(_simulationName, summaryZone, alternative)] = stage;
                }
                stages.Add(stage);

                if (!seenHistograms.TryGetValue((_simulationName, summaryZone, alternative, hazardTime), out DynamicHistogram? histogram)) // checking if the histogram already exists in the DB
                {
                    newEntry = true; newEntries = true;
                    string tableName = $"{_simulationName}>Results_By_Iteration>{alternative}>{hazardTime}>{_summarySetName}>{summaryZone}";
                    histogram = _db.QueryLifeLossTable(tableName);
                    seenHistograms[(_simulationName, summaryZone, alternative, hazardTime)] = histogram;
                }
                histograms.Add(histogram);
            }
            if (newEntry) // we had at least one new entry for the function which was just built, so save the function to SQLite
            {
                UncertainPairedData upd = new(stages.ToArray(), histograms.ToArray(), new CurveMetaData());
                LifeLossFunction llf = new(upd, _alternativeNames, _simulationName, summaryZone, hazardTime);
                _saver.SaveToSQLite(llf); 
            } 
        }
        if (!newEntries) return existingFunctions;
        return _saver.ReadFromSQLite(allPF); // this call is needed to order the functions (uses ORDER BY in the SELECT statement)
    }
}