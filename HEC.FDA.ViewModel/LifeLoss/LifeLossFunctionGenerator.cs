using Amazon.Runtime.SharedInterfaces;
using HEC.FDA.Model.LifeLoss;
using HEC.FDA.Model.LifeLoss.Saving;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.Spatial;
using HEC.FDA.Model.utilities;
using RasMapperLib;
using Statistics.Histograms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Utility.Progress;

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
    public async Task<List<LifeLossFunction>> CreateLifeLossFunctionsAsync(string summarySetPath, string indexPointsPath, string summarySetUniqueName, ProgressReporter pr = null)
    {
        pr ??= ProgressReporter.None();
        double weightSum = _hazardTimes.Values.Sum();
        if (Math.Abs(weightSum - 1.0) > 0.001)
        {
            System.Windows.MessageBox.Show($"Hazard time weights must sum to 1.00, but they sum to {weightSum:F2}.", "Invalid Weights", MessageBoxButton.OK, MessageBoxImage.Warning);
            return [];
        }

        List<LifeLossFunction> lifeLossFunctions = new();

        // create the map of summary zone names to their corresponding index points
        if (!RASHelper.TryQueryPolygons(summarySetPath, indexPointsPath, summarySetUniqueName, out _indexPointBySummaryZone))
        {
            System.Windows.MessageBox.Show($"Could not create a 1-1 mapping of Index Points '{Path.GetFileName(indexPointsPath)}' to Impact Areas '{Path.GetFileName(summarySetPath)}'.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return [];
        }

        lifeLossFunctions = await Task.Run(() => CreateLifeLossFunctions(pr));

        return lifeLossFunctions;
    }

    private List<LifeLossFunction> CreateLifeLossFunctions(ProgressReporter pr = null)
    {
        pr ??= ProgressReporter.None();
        int functionID = 1;
        List<LifeLossFunction> lifeLossFunctions = [];
        var computeTask = pr.SubTask("Stage Life Loss Compute", 0, 1);
        computeTask.ReportMessage("Beginning Aggregated Stage Life Loss Function Compute");
        Stopwatch sw = new();
        sw.Start();
        int i = 0;
        int count = _impactAreaIDByName.Count;
        foreach (var (name, id) in _impactAreaIDByName.OrderBy(kvp => kvp.Value))
        {
            PointMs indexPoint = [_indexPointBySummaryZone[name]];
            var iaTask = computeTask.SubTask(name, (float)i / count, 1f / count);
            iaTask.ReportTimestampedMessage(sw?.Elapsed, 1, $"Computing Impact Area: {name}...");
            List<LifeLossFunction> functions = CreateLifeLossFunctionsForSummaryZone(name, indexPoint, iaTask, sw);
            foreach (LifeLossFunction function in functions)
            {
                function.FunctionID = functionID;
                functionID++;
            }
            lifeLossFunctions.AddRange(functions); // AddRange because we are adding a list to another list
            i++;
            iaTask.ReportProgress(100);
        }
        computeTask.ReportProgress(100);
        computeTask.ReportTaskCompleted(sw.Elapsed);
        sw.Stop();
        return lifeLossFunctions;
    }

    /// <summary>
    /// Validates that all the associated hydraulics needed exist in the selected hydraulics element. Returns list of missing hydraulics if any are missing.
    /// </summary>
    public List<string> GetMissingHydraulics()
    {
        List<string> missingHydraulics = [];
        foreach (string alternativeName in _alternativeNames)
        {
            if (_hydraulicsFolderByAlternative.TryGetValue(alternativeName, out string hydraulicsName))
            {
                string filePath = Directory.EnumerateFiles(_topLevelHydraulicsFolder, $"{hydraulicsName}.hdf").FirstOrDefault();
                if (filePath == null)
                {
                    missingHydraulics.Add($"{hydraulicsName}.hdf");
                }
            }
        }
        return missingHydraulics;
    }


    private List<LifeLossFunction> CreateLifeLossFunctionsForSummaryZone(string summaryZone, PointMs indexPoint, ProgressReporter pr = null, Stopwatch sw = null)
    {
        pr ??= ProgressReporter.None();
        List<LifeLossFunction> functions = [];

        string[] alternatives = [.. _alternativeNames]; // create a copy of the alternative names because we are sorting, don't want to sort in place as we iterate through (although I think it would still work)
        double[] stages = new double[alternatives.Length];
        pr.ReportTimestampedMessage(sw?.Elapsed, 2, $"Computing stages...");
        for (int i = 0; i < alternatives.Length; i++)
        {
            string associatedHydraulics = _hydraulicsFolderByAlternative[alternatives[i]];
            string filePath = Directory.EnumerateFiles(_topLevelHydraulicsFolder, $"{associatedHydraulics}.hdf").FirstOrDefault()
                ?? throw new System.Exception($"'{associatedHydraulics}' hydraulics file not found in {_topLevelHydraulicsFolder}.");
            float[] computedStage = RASHelper.GetStageFromHDF(indexPoint, filePath); // costly compute
            double stage = computedStage[0];
            stages[i] = stage;
        }
        Array.Sort(stages, alternatives); // both arrays are now sorted in ascending stage order which is what we need for UPD

        Dictionary<UncertainPairedData, double> updweights = new();
        int idx = 0;
        pr.ReportTimestampedMessage(sw?.Elapsed, 2, $"Computing for each hazard time...");
        foreach (var kvp in _hazardTimes)
        {
            string hazardTime = kvp.Key;
            double weight = kvp.Value;
            var timeTask = pr.SubTask(hazardTime, (float)idx / _hazardTimes.Count, 1f / _hazardTimes.Count);
            
            DynamicHistogram[] histograms = new DynamicHistogram[alternatives.Length];
            for (int i = 0; i < alternatives.Length; i++)
            {
                var altTask = timeTask.SubTask(alternatives[i], (float)i / alternatives.Length, 1f / alternatives.Length);
                string tableName = $"{_simulationName}>Results_By_Iteration>{alternatives[i]}>{hazardTime}>{_summarySetName}>{summaryZone}";
                DynamicHistogram histogram = _db.QueryLifeLossTable(tableName);
                histograms[i] = histogram;
                altTask.ReportProgress(100);
            }
            UncertainPairedData upd = new(stages, histograms, new CurveMetaData("Stage", "Life Loss", $"{_simulationName}_{summaryZone}_{hazardTime}", "LifeLoss", _impactAreaIDByName[summaryZone], "LifeLoss"));
            updweights[upd] = weight;
            LifeLossFunction llf = new(-1, -1, upd, alternatives, _simulationName, summaryZone, hazardTime);
            functions.Add(llf);
            idx++;
            timeTask.ReportProgress(100);
        }
        pr.ReportTimestampedMessage(sw?.Elapsed, 2, $"Combining...");
        UncertainPairedData combined = UncertainPairedData.CombineWithWeights(updweights);
        string weightsDisplay = string.Join("/", _hazardTimes.Values.Select(w => w.ToString("F2")));
        string combinedHazardTime = $"{LifeLossStringConstants.COMBINED_MAGIC_STRING} ({weightsDisplay})";
        LifeLossFunction combinedFunc = new(-1, -1, combined, alternatives, _simulationName, summaryZone, combinedHazardTime);
        functions.Add(combinedFunc);
        return functions;
    }
}