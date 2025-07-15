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

    private List<LifeLossFunction> CreateLifeLossFunctionsForSummaryZone(string summaryZone, PointMs indexPoint)
    {
        PlotFilter allPF = new()
        {
            Simulation = [_simulationName],
            Summary_Zone = [summaryZone],
            Alternative = _alternativeNames,
            Hazard_Time = _hazardTimes,
        };

        List<LifeLossFunction> existingFunctions = _saver.ReadFromSQLite(allPF);
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

        bool newEntries = false;
        foreach (string hazardTime in _hazardTimes)
        {
            bool newEntry = false;
            List<double> stages = new();
            List<DynamicHistogram> histograms = new();
            foreach (string alternative in _alternativeNames)
            {
                if (!seenStages.TryGetValue((_simulationName, summaryZone, alternative), out double stage))
                {
                    newEntry = true; newEntries = true;
                    string associatedHydraulicsFolder = _hydraulicsFolderByAlternative[alternative];
                    string hdf = Path.Combine(_topLevelHydraulicsFolder, associatedHydraulicsFolder, $"{associatedHydraulicsFolder}.hdf"); // asserting that the HDF file name is always in this format
                    float[] computedStage = GeospatialHelpers.GetStageFromHDF(indexPoint, hdf);
                    stage = (double)computedStage[0]; // GetStageFromHDF returns an array with one value in it (RAS API), so we get the first (and only) value
                    seenStages[(_simulationName, summaryZone, alternative)] = stage;
                }
                stages.Add(stage); // getting the stage that is already saved in the DB

                if (!seenHistograms.TryGetValue((_simulationName, summaryZone, alternative, hazardTime), out DynamicHistogram? histogram))
                {
                    newEntry = true; newEntries = true;
                    string tableName = $"{_simulationName}>Results_By_Iteration>{alternative}>{hazardTime}>{_summarySetName}>{summaryZone}";
                    histogram = _db.QueryLifeLossTable(tableName);
                    seenHistograms[(_simulationName, summaryZone, alternative, hazardTime)] = histogram;
                }
                histograms.Add(histogram);
            }
            if (newEntry) 
            {
                UncertainPairedData upd = new(stages.ToArray(), histograms.ToArray(), new CurveMetaData());
                LifeLossFunction llf = new(upd, _alternativeNames, _simulationName, summaryZone, hazardTime);
                _saver.SaveToSQLite(llf);
            } 
        }

        if (!newEntries) return existingFunctions;
        return _saver.ReadFromSQLite(allPF);
    }

    /// <summary>
    /// Create life loss functions for a given summary zone
    /// </summary>
    /// <param name="summaryZone">Name of the summary zone</param>
    /// <param name="points">Array of points to query for stage</param>
    /// <returns></returns>
    private List<LifeLossFunction> CreateLifeLossRelationships(string summaryZone, PointMs points)
    {
        Dictionary<(string, string), DynamicHistogram> histogramsByAlternativeTime = new(); // ((alternative name, hazard time), histogram)
        Dictionary<string, double> stageByAlternative = new(); // (alternative name, stage)
        foreach (string alternative in _alternativeNames)
        {
            string associatedHydraulicsFolder = _hydraulicsFolderByAlternative[alternative];
            // asserting that the HDF file name is always in this format
            string hydraulicFilePath = _topLevelHydraulicsFolder + "\\" + associatedHydraulicsFolder + "\\" + associatedHydraulicsFolder + ".hdf";
            float[] stage = GeospatialHelpers.GetStageFromHDF(points, hydraulicFilePath);
            stageByAlternative[alternative] = (double)stage[0]; // GetStageFromHDF returns an array with one value in it (RAS API), so we get the first (and only) value
            foreach (string hazardTime in _hazardTimes)
            {
                string tableName = $"{_simulationName}>Results_By_Iteration>{alternative}>{hazardTime}>{_summarySetName}>{summaryZone}";
                DynamicHistogram histogram = _db.QueryLifeLossTable(tableName);
                histogramsByAlternativeTime[(alternative, hazardTime)] = histogram;
            }
        }

        List<LifeLossFunction> lifeLossRelationships = new();
        // make the functions
        foreach (string hazardTime in _hazardTimes)
        {
            lifeLossRelationships.Add(BuildLifeLossFunctionForTime(hazardTime, summaryZone, stageByAlternative, histogramsByAlternativeTime));
        }
        return lifeLossRelationships;
    }

    private readonly record struct LifeLossFunctionEntry
    (
        double Stage,
        DynamicHistogram Histogram,
        string Alternative
    );

    private LifeLossFunction BuildLifeLossFunctionForTime(string hazardTime, string summaryZone, Dictionary<string, double> stageByAlt, Dictionary<(string alt, string time), DynamicHistogram> histByAltTime)
    {
        var entries = new List<LifeLossFunctionEntry>();

        foreach (string alt in _alternativeNames)
            if (histByAltTime.TryGetValue((alt, hazardTime), out var hist)) entries.Add(new LifeLossFunctionEntry(stageByAlt[alt], hist, alt));

        entries.Sort((a, b) => a.Stage.CompareTo(b.Stage));

        int n = entries.Count;
        var stages = new double[n];
        var histograms = new DynamicHistogram[n];
        var alternatives = new string[n];
        for (int i = 0; i < n; i++)
        {
            stages[i] = entries[i].Stage;
            histograms[i] = entries[i].Histogram;
            alternatives[i] = entries[i].Alternative;
        }

        var upd = new UncertainPairedData(stages, histograms, new CurveMetaData());
        return new LifeLossFunction(upd, alternatives, _simulationName, summaryZone, hazardTime);
    }
}