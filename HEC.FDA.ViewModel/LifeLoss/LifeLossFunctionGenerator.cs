using HEC.FDA.Model.LifeLoss;
using HEC.FDA.Model.LifeLoss.Saving;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.Spatial;
using RasMapperLib;
using Statistics.Histograms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    private Dictionary<string, int> _impactAreaIDByName;
    private readonly string _simulationName;
    private readonly string _topLevelHydraulicsFolder;
    private readonly string[] _alternativeNames;
    private readonly Dictionary<string, double> _hazardTimes;

    public LifeLossFunctionGenerator(string selectedPath, LifeSimSimulation simulation, Dictionary<string, int> impactAreaIDByName)
    {
        _db = new LifeSimDatabase(selectedPath);
        _hydraulicsFolderByAlternative = _db.CreateAlternativeHydraulicsPairs();
        _summarySetName = _db.SummarySetName(simulation.Name);
        _indexPointBySummaryZone = simulation.SummarySet;
        _impactAreaIDByName = impactAreaIDByName;
        _simulationName = simulation.Name;
        _topLevelHydraulicsFolder = simulation.HydraulicsFolder;
        _alternativeNames = simulation.Alternatives.ToArray();
        _hazardTimes = simulation.HazardTimes;
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
        _indexPointBySummaryZone = RASHelper.QueryPolygons(summarySetPath, indexPointsPath, summarySetUniqueName);

        lifeLossFunctions = await Task.Run(CreateLifeLossFunctions);

        return lifeLossFunctions;
    }

    private List<LifeLossFunction> CreateLifeLossFunctions()
    {
        int functionID = 1;
        List<LifeLossFunction> lifeLossFunctions = [];
        foreach (var (name, id) in _impactAreaIDByName.OrderBy(kvp => kvp.Value))
        {
            PointMs indexPoint = [_indexPointBySummaryZone[name]];
            List<LifeLossFunction> functions = CreateLifeLossFunctionsForSummaryZone(name, indexPoint);
            foreach (LifeLossFunction function in functions)
            {
                function.FunctionID = functionID;
                functionID++;
            }
            lifeLossFunctions.AddRange(functions); // AddRange because we are adding a list to another list
        }
        return lifeLossFunctions;
    }

    private List<LifeLossFunction> CreateLifeLossFunctionsForSummaryZone(string summaryZone, PointMs indexPoint)
    {
        List<LifeLossFunction> functions = [];

        string[] alternatives = [.. _alternativeNames]; // create a copy of the alternative names because we are sorting, don't want to sort in place as we iterate through (although I think it would still work)
        double[] stages = new double[alternatives.Length];
        for (int i = 0; i < alternatives.Length; i++)
        {
            string associatedHydraulics = _hydraulicsFolderByAlternative[alternatives[i]];
            // looking for a file matching the hydraulicsname.hdf
            // This is hardcoding LifeSim convention of .hdf extension for hydraulics files. Could be made more flexible in the future if needed.
            // Instead, load the whole hydraulics folder as RASResults and scrub the names from them, then use those to compare with the LifeSim database.
            string filePath = Directory.EnumerateFiles(_topLevelHydraulicsFolder, $"{associatedHydraulics}.hdf").FirstOrDefault()
                ?? throw new System.Exception($"'{associatedHydraulics}' hydraulics file not found in {_topLevelHydraulicsFolder}.");
            float[] computedStage = RASHelper.GetStageFromHDF(indexPoint, filePath); // costly compute
            double stage = computedStage[0];
            stages[i] = stage;
        }
        Array.Sort(stages, alternatives); // both arrays are now sorted in ascending stage order which is what we need for UPD

        Dictionary<UncertainPairedData, double> updweights = new();
        int idx = 0;
        foreach (var kvp in _hazardTimes)
        {
            string hazardTime = kvp.Key;
            double weight = kvp.Value;
            DynamicHistogram[] histograms = new DynamicHistogram[alternatives.Length];
            for (int i = 0; i < alternatives.Length; i++)
            {
                string tableName = $"{_simulationName}>Results_By_Iteration>{alternatives[i]}>{hazardTime}>{_summarySetName}>{summaryZone}";
                DynamicHistogram histogram = _db.QueryLifeLossTable(tableName);
                histograms[i] = histogram;
            }
            UncertainPairedData upd = new(stages, histograms, new CurveMetaData("Stage", "Life Loss", $"{_simulationName}_{summaryZone}_{hazardTime}", "LifeLoss", _impactAreaIDByName[summaryZone], "LifeLoss"));
            updweights[upd] = weight;
            LifeLossFunction llf = new(-1, -1, upd, alternatives, _simulationName, summaryZone, hazardTime);
            functions.Add(llf);
            idx++;
        }
        UncertainPairedData combined = UncertainPairedData.CombineWithWeights(updweights);
        LifeLossFunction combinedFunc = new(-1, -1, combined, alternatives, _simulationName, summaryZone, LifeLossStringConstants.COMBINED_MAGIC_STRING);
        //functions.Add(combinedFunc);
        return functions;
    }
}