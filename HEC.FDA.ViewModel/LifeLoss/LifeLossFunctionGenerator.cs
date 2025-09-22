using HEC.FDA.Model.LifeLoss;
using HEC.FDA.Model.LifeLoss.Saving;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.Storage;
using RasMapperLib;
using Statistics.Histograms;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.LifeLoss;

/// <summary>
/// Creates a list of life loss functions for each summary zone in simulation for a given set of alternatives and hazard times
/// </summary>
public class LifeLossFunctionGenerator
{
    private readonly LifeSimDatabase _db;
    private readonly Dictionary<string, string> _hydraulicsFolderByAlternative;
    private readonly string _summarySetName;
    private Dictionary<string, PointM> _indexPointBySummaryZone; // not readonly because we reassign its pointer in CreateLifeLossFunctions, too costly to do in constructor?
    private readonly string _simulationName;
    private readonly string _topLevelHydraulicsFolder;
    private readonly string[] _alternativeNames;
    private readonly string[] _hazardTimes;

    public LifeLossFunctionGenerator(string selectedPath, LifeSimSimulation simulation)
    {
        _db = new LifeSimDatabase(selectedPath);
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
    public async Task<List<LifeLossFunction>> CreateLifeLossFunctionsAsync(string summarySetPath, string indexPointsPath, string summarySetUniqueName)
    {
        List<LifeLossFunction> lifeLossFunctions = new();

        // create the map of summary zone names to their corresponding index points
        _indexPointBySummaryZone = GeospatialHelpers.QueryPolygons(summarySetPath, indexPointsPath, summarySetUniqueName);

        lifeLossFunctions = await Task.Run(CreateLifeLossFunctions);

        return lifeLossFunctions;
    }

    private List<LifeLossFunction> CreateLifeLossFunctions()
    {
        int functionID = 1;
        List<LifeLossFunction> lifeLossFunctions = [];
        foreach (string summaryZone in _indexPointBySummaryZone.Keys)
        {
            // creating points array of size 1 because that RAS API needs an array
            PointMs indexPoint = [_indexPointBySummaryZone[summaryZone]];

            List<LifeLossFunction> functions = CreateLifeLossFunctionsForSummaryZone2(summaryZone, indexPoint);
            foreach (LifeLossFunction function in functions)
            {
                function.FunctionID = functionID;
                functionID++;
            }
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
        LifeLossFunctionFilter allPF = new()
        {
            Simulation = [_simulationName],
            Summary_Zone = [summaryZone],
            Alternative = _alternativeNames,
            Hazard_Time = _hazardTimes,
        };
        string projFile = Connection.Instance.ProjectFile;
        using LifeLossFunctionSaver saver = new(projFile);
        List<LifeLossFunction> existingFunctions = saver.ReadFromSQLite(allPF);
        // set up dictionaries for stages and histograms already in the DB, allows us to make O(1) lookups instead of recomputing
        var seenStages = new Dictionary<(string simulation, string summaryZone, string alternative), double>();
        var seenHistograms = new Dictionary<(string simulation, string summaryZone, string alternative, string hazardTime), DynamicHistogram>();
        foreach (LifeLossFunction llf in existingFunctions)
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
                    stage = computedStage[0]; // GetStageFromHDF returns an array with one value in it (RAS API), so we get the first (and only) value
                    seenStages[(_simulationName, summaryZone, alternative)] = stage;
                }
                stages.Add(stage);

                if (!seenHistograms.TryGetValue((_simulationName, summaryZone, alternative, hazardTime), out DynamicHistogram histogram)) // checking if the histogram already exists in the DB
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
                LifeLossFunction llf = new(-1, -1, upd, _alternativeNames, _simulationName, summaryZone, hazardTime);
                saver.SaveToSQLite(llf);
            }
        }
        if (!newEntries) return existingFunctions;
        return saver.ReadFromSQLite(allPF); // this call is needed to order the functions (uses ORDER BY in the SELECT statement)
    }



    private List<LifeLossFunction> CreateLifeLossFunctionsForSummaryZone2(string summaryZone, PointMs indexPoint)
    {
        Dictionary<string, double> stageByAlternative = [];
        List<LifeLossFunction> functions = [];
        foreach (string hazardTime in _hazardTimes)
        {
            List<double> stages = [];
            List<DynamicHistogram> histograms = [];
            string[] alternatives = [.. _alternativeNames]; // create a copy of the alternative names because we are sorting, don't want to sort in place as we iterate through (although I think it would still work)
            foreach (string alternative in alternatives)
            {
                if (!stageByAlternative.TryGetValue(alternative, out double stage))
                {
                    string associatedHydraulicsFolder = _hydraulicsFolderByAlternative[alternative];
                    string hdf = Path.Combine(_topLevelHydraulicsFolder, associatedHydraulicsFolder, $"{associatedHydraulicsFolder}.hdf");
                    float[] computedStage = GeospatialHelpers.GetStageFromHDF(indexPoint, hdf); // costly compute
                    stage = computedStage[0];
                    stageByAlternative[alternative] = stage; // cache the stages. same for any time of day (only associated with alternative, not time)
                }
                stages.Add(stage);
                string tableName = $"{_simulationName}>Results_By_Iteration>{alternative}>{hazardTime}>{_summarySetName}>{summaryZone}";
                DynamicHistogram histogram = _db.QueryLifeLossTable(tableName);
                histograms.Add(histogram);

            }
            List<Entry> entries = [];
            for (int i = 0; i < stages.Count; i++)
            {
                entries.Add(new Entry
                {
                    Alternative = alternatives[i],
                    Stage = stages[i],
                    Histogram = histograms[i]
                });
            }
            entries.Sort((e1, e2) => e1.Stage.CompareTo(e2.Stage)); // the stages will not necessarily be in order, but we need them to be for UPD
            for (int i = 0; i < entries.Count; i++)
            {
                alternatives[i] = entries[i].Alternative;
                stages[i] = entries[i].Stage;
                histograms[i] = entries[i].Histogram;
            }
            UncertainPairedData upd = new(stages.ToArray(), histograms.ToArray(), new CurveMetaData());
            LifeLossFunction llf = new(-1, -1, upd, alternatives, _simulationName, summaryZone, hazardTime);
            functions.Add(llf);
        }
        return functions;
    }

    private class Entry
    {
        public string Alternative;
        public double Stage;
        public DynamicHistogram Histogram;
    }
}