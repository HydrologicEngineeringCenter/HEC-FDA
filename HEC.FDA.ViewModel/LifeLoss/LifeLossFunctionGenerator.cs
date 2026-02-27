using Amazon.Runtime;
using Amazon.Runtime.SharedInterfaces;
using Geospatial.Features;
using Geospatial.GDALAssist;
using Geospatial.IO;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.Model.LifeLoss;
using HEC.FDA.Model.LifeLoss.Saving;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.Spatial;
using HEC.FDA.Model.utilities;
using HEC.FDA.ViewModel.Storage;
using RasMapperLib;
using Statistics.Histograms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Utility.Logging;
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
    private readonly HydraulicDataSource _hydraulicDataSource;
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
        _hydraulicDataSource = simulation.HydraulicsDataSource;
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

        List<LifeLossFunction> lifeLossFunctions = [];

        var prjFile = Connection.Instance.ProjectionFile;
        Projection studyPrj = Projection.FromFile(prjFile);
        // reproject polygons
        OperationResult polygonResult = ShapefileIO.TryRead(summarySetPath, out PolygonFeatureCollection polygons, studyPrj);
        if (!polygonResult.Result)
        {
            System.Windows.MessageBox.Show(polygonResult.GetConcatenatedMessages());
            return lifeLossFunctions;
        }
        // reproject points
        OperationResult pointsResult = ShapefileIO.TryRead(indexPointsPath, out PointFeatureCollection points, studyPrj);
        if (!pointsResult.Result)
        {
            System.Windows.MessageBox.Show(pointsResult.GetConcatenatedMessages());
            return lifeLossFunctions;
        }

        // create the map of summary zone names to their corresponding index points
        if (!RASHelper.TryMapPolygonsToPoints(polygons, points, summarySetUniqueName, out _indexPointBySummaryZone))
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
        var stageTask = pr.SubTask("Get Stages", 0, 0.5f);
        var computeTask = pr.SubTask("Stage Life Loss Compute", 0.5f, 0.5f);
        computeTask.ReportMessage("Beginning Aggregated Stage Life Loss Function Compute");
        Stopwatch sw = new();
        sw.Start();
        int i = 0;
        int count = _impactAreaIDByName.Count;
        var altStagePairsByIA = GetAllStages(_indexPointBySummaryZone, stageTask, sw);

        foreach (var (name, id) in _impactAreaIDByName.OrderBy(kvp => kvp.Value))
        {
            PointMs indexPoint = [_indexPointBySummaryZone[name]];

            var iaTask = computeTask.SubTask(name, (float)i / count, 1f / count);
            iaTask.ReportTimestampedMessage(sw?.Elapsed, 1, $"Computing Impact Area: {name}...");
            string[] alternatives = altStagePairsByIA[name].Item1;
            double[] stages = altStagePairsByIA[name].Item2;
            List<LifeLossFunction> functions = CreateLifeLossFunctionsForSummaryZone(name, alternatives, stages, iaTask, sw);
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
                // this is how .tif grids are saved -- a folder at the root level with a name matching hydraulic profile, formatted [name]/
                string tifFolderPath = Path.Combine(_topLevelHydraulicsFolder, hydraulicsName);
                // this is how .hdf files are saved -- a .hdf file at the root level with the format [name].hdf
                string hdfPath = Path.Combine(_topLevelHydraulicsFolder, $"{hydraulicsName}.hdf");
                // if we either have a folder with the proper name or a .hdf the the proper name, our hydraulics exist
                if (!Directory.Exists(tifFolderPath) && !File.Exists(hdfPath))
                    missingHydraulics.Add(hydraulicsName);
            }
        }
        return missingHydraulics;
    }

    // returns an dictionary of impact areas to their associated alternative + stage pairs
    public Dictionary<string, (string[], double[])> GetAllStages(Dictionary<string, PointM> indexPointsByIA, ProgressReporter pr = null, Stopwatch sw = null)
    {
        pr ??= ProgressReporter.None();
        pr.ReportTimestampedMessage(sw?.Elapsed, 1, $"Querying stages off of hydraulic profiles...");

        var ias = _indexPointBySummaryZone.Keys;
        var pts = _indexPointBySummaryZone.Values;
        PointMs pointMs = new(pts);
        Dictionary<string, (string[], double[])> altStagePairsByIA = [];
        foreach (string ia in ias)
            altStagePairsByIA[ia] = new(new string[_alternativeNames.Length], new double[_alternativeNames.Length]);

        int altIdx = 0;
        foreach (string alternativeName in _alternativeNames)
        {
            var altTask = pr.SubTask(alternativeName, (float)altIdx / _alternativeNames.Length, 1f / _alternativeNames.Length);
            string associatedHydraulics = _hydraulicsFolderByAlternative[alternativeName];
            altTask.ReportTimestampedMessage(sw?.Elapsed, 2, $"Reading {associatedHydraulics}...");

            string hydraulicsPath;
            if (_hydraulicDataSource == HydraulicDataSource.UnsteadyHDF)
            {
                hydraulicsPath = Directory.EnumerateFiles(_topLevelHydraulicsFolder, $"{associatedHydraulics}.hdf", SearchOption.TopDirectoryOnly).FirstOrDefault();
            }
            else
            {
                string subDirPath = Path.Combine(_topLevelHydraulicsFolder, associatedHydraulics);
                hydraulicsPath = Directory.EnumerateFiles(subDirPath, "*.tif", SearchOption.TopDirectoryOnly)
                                           .FirstOrDefault();
            }
            if (hydraulicsPath == null)
                throw new Exception(); // no .hdf or .tif found, we shouldn't reach this unless the user tampered with the FDA project folder structure

            float[] computedStages = RASHelper.GetStageFromHydraulics(pointMs, _hydraulicDataSource, hydraulicsPath); // costly compute
            if (computedStages == null)
                throw new Exception(); // I think the only way to get here is steady HDF, but that is handled long before this is called

            int stageIdx = 0;
            foreach (string ia in ias)
            {
                var (altArr, stageArr) = altStagePairsByIA[ia];
                altArr[altIdx] = alternativeName;
                stageArr[altIdx] = computedStages[stageIdx];
                stageIdx++;
            }
            altIdx++;
            altTask.ReportProgress(100);
        }

        pr.ReportProgress(100);
        return altStagePairsByIA;
    }
    private List<LifeLossFunction> CreateLifeLossFunctionsForSummaryZone(string summaryZone, string[] alternatives, double[] stages, ProgressReporter pr = null, Stopwatch sw = null)
    {
        pr ??= ProgressReporter.None();
        List<LifeLossFunction> functions = [];

        // sort ascending stage order which is what we need for UPD
        // cannot guarantee they come in sorted
        Array.Sort(stages, alternatives); 

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