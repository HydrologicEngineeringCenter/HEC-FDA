using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.FrequencyRelationships;
using OxyPlot;
using OxyPlot.Series;
using RasMapperLib;
using SciChart.Core.Extensions;
using Statistics.Distributions;
using System.Collections.ObjectModel;
using VisualScratchSpace.Model;

namespace VisualScratchSpace.ViewModel;
public partial class CheckBoxImporterVM : ObservableObject
{
    public PlotModel MyModel { get; set; }
    public ObservableCollection<Simulation> SimulationsComboBox { get; set; }
    public ObservableCollection<CheckableItem> AlternativesCheckBox { get; set; }
    public ObservableCollection<CheckableItem> HazardTimesCheckBox { get; set; }

    private int plotIndex = 0;
    private List<LifeLossRelationship> data = new();

    [ObservableProperty]
    private string _selectedPath = "";
    [ObservableProperty]
    private string _selectedSimulation = "";
    [ObservableProperty]
    private string _selectedPointsPath = "";
    [ObservableProperty]
    private string _selectedSummarySetPath = "";
    [ObservableProperty]
    private string _selectedHydraulicsFolder = "";

    public CheckBoxImporterVM()
    {
        SimulationsComboBox = [];
        AlternativesCheckBox = [];
        HazardTimesCheckBox = [];
        MyModel = new PlotModel();
    }

    /// <summary>
    /// 
    /// </summary>
    [RelayCommand]
    public void CreateStageLLGraphs()
    {

    }

    /// <summary>
    /// Generate histograms for tables matching the selected alternatives and hazard times
    /// </summary>
    [RelayCommand]
    public void Import()
    {
        if (SelectedPath.IsNullOrWhiteSpace())
            return;

        List<string> selectedAlternatives = [];
        List<string> selectedHazardTimes = [];
        foreach (CheckableItem a in AlternativesCheckBox)
            if (a.IsChecked) selectedAlternatives.Add(a.Name);
        foreach (CheckableItem h in HazardTimesCheckBox)
            if (h.IsChecked) selectedHazardTimes.Add(h.Value);

        List<string> prefixes = LifeLossDB.GetSimulationTablePrefixes(SelectedSimulation, selectedAlternatives.ToArray(), selectedHazardTimes.ToArray());
        LifeLossDB db = new(SelectedPath);
        //db.QueryMatchingTables(prefixes.ToArray());
        Dictionary<string, PointM>? summaryZonePointPairs = LifeLossPlotter.CreateSummaryZonePointPairs(SelectedSummarySetPath, SelectedPointsPath);
        if (summaryZonePointPairs == null) return;
        LifeLossPlotter plotter = new(db, SelectedSimulation, SelectedHydraulicsFolder, selectedAlternatives.ToArray(), selectedHazardTimes.ToArray());

        data.Clear();
        foreach (string summaryZone in summaryZonePointPairs.Keys)
        {
            PointMs indexPoint = [summaryZonePointPairs[summaryZone]];
            var lifeLossPlots = plotter.CreatePairedData(summaryZone, indexPoint);
            foreach (LifeLossRelationship relationship in lifeLossPlots)
            {
                relationship.SummaryZone = summaryZone;
            }
            data.AddRange(lifeLossPlots);
        }
        plotIndex = 0;
        ChangePlot(plotIndex);
    }

    [RelayCommand]
    public void NextPlot()
    {
        plotIndex = (plotIndex + 1) % data.Count;
        ChangePlot(plotIndex);
    }

    /// <summary>
    /// Update the combobox of simulations available to the user when a new .fia database is imported
    /// </summary>
    /// <param name="value"></param>
    partial void OnSelectedPathChanged(string value)
    {
        if (SelectedPath.IsNullOrWhiteSpace())
            return;

        // reset the simulation options
        SimulationsComboBox.Clear();
        LifeLossDB db = new(SelectedPath);

        // add new simulations from the newly selected database
        List<Simulation> newSimulations = db.UpdateSimulations();
        foreach (Simulation simulation in newSimulations)
        {
            SimulationsComboBox.Add(simulation);
        }
    }

    partial void OnSelectedSimulationChanged(string value)
    {
        UpdateSimulationFields(value);
    }

    private void UpdateSimulationFields(string value)
    {
        // reset the other options
        AlternativesCheckBox.Clear();
        HazardTimesCheckBox.Clear();

        // update options to match the currently selected simulation
        foreach (Simulation s in SimulationsComboBox)
        {
            if (s.Name == value)
            {
                foreach (string a in s.Alternatives)
                    AlternativesCheckBox.Add(new CheckableItem { Name = a });
                foreach (string h in s.HazardTimes)
                {
                    int time = int.Parse(h);
                    // display name in military time format
                    HazardTimesCheckBox.Add(new CheckableItem { Name = time < 10 ? $"0{time}00" : $"{time}00", Value = time.ToString() });
                } 
            }
        }
    }

    private void ChangePlot(int next)
    {
        LifeLossRelationship relationship = data[next];
        UncertainPairedData d = relationship.Data;
        var upper = d.SamplePairedData(0.975);
        var lower = d.SamplePairedData(0.025);
        var mid = d.SamplePairedData(0.5);

        MyModel.Series.Clear();
        AddLineSeriesToPlot(upper, true);
        AddLineSeriesToPlot(lower, true);
        AddLineSeriesToPlot(mid);

        MyModel.ResetAllAxes();
        int time = int.Parse(relationship.HazardTime);
        string formattedTime = time < 10 ? $" 0{time}00" : $" {time}00";
        MyModel.Title = relationship.SummaryZone + formattedTime;
        MyModel.InvalidatePlot(true);
    }

    private void AddLineSeriesToPlot(PairedData function, bool isConfidenceLimit = false)
    {
        LineSeries lineSeries = new();

        DataPoint[] points = new DataPoint[function.Xvals.Length];
        for (int i = 0; i < function.Xvals.Length; i++)
        {
            double stage = function.Xvals[i];
            double lifeLoss = function.Yvals[i];
            points[i] = new DataPoint(stage, lifeLoss);
        }

        if (isConfidenceLimit) { lineSeries.Color = OxyColors.Blue; lineSeries.LineStyle = LineStyle.Dash; }
        else { lineSeries.Color = OxyColors.Black; }

        lineSeries.ItemsSource = points;
        //lineSeries.DataFieldX = "Stage";
        //lineSeries.DataFieldY = "Life Loss";
        MyModel.Series.Add(lineSeries);
    }
}
