using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HEC.FDA.Model.paireddata;
using OxyPlot;
using OxyPlot.Series;
using SciChart.Core.Extensions;
using System.Collections.ObjectModel;
using VisualScratchSpace.Model;
using VisualScratchSpace.Model.Saving;

namespace VisualScratchSpace.ViewModel;
public partial class CheckBoxImporterVM : ObservableObject
{
    public PlotModel MyModel { get; set; }
    public ObservableCollection<Simulation> SimulationsComboBox { get; set; }
    public ObservableCollection<CheckableItem> AlternativesCheckBox { get; set; }
    public ObservableCollection<CheckableItem> HazardTimesCheckBox { get; set; }

    private int _plotIndex = 0;
    private List<LifeLossFunction> _lifeLossFunctions = new();

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
    /// Import the selected simulation parametes to create associated functions and plots
    /// </summary>
    [RelayCommand]
    public void Import()
    {
        if (SelectedPath.IsNullOrWhiteSpace() || SelectedSimulation.IsNullOrWhiteSpace() || SelectedHydraulicsFolder.IsNullOrWhiteSpace()) return;
        if (AlternativesCheckBox.IsEmpty() || HazardTimesCheckBox.IsEmpty()) return;

        // determine which parameters have been selected
        Simulation currentSimulation = new(SelectedSimulation, SelectedHydraulicsFolder);
        foreach (CheckableItem a in AlternativesCheckBox)
            if (a.IsChecked) currentSimulation.Alternatives.Add(a.Name);
        foreach (CheckableItem h in HazardTimesCheckBox)
            if (h.IsChecked) currentSimulation.HazardTimes.Add(h.Value);

        // send those parameters to the a generator
        LifeLossFunctionGenerator generator = new(SelectedPath, currentSimulation);

        if (SelectedSummarySetPath.IsNullOrWhiteSpace() || SelectedPointsPath.IsNullOrWhiteSpace()) return;

        // generate associated functions for the selected parameters and plot the first one
        _lifeLossFunctions.Clear();
        _lifeLossFunctions = generator.CreateLifeLossFunctions(SelectedSummarySetPath, SelectedPointsPath);
        _plotIndex = 0;
        ChangePlot(_plotIndex);
    }

    /// <summary>
    /// Cycle through all plots and loop back to start once the end is passed
    /// </summary>
    [RelayCommand]
    public void NextPlot()
    {
        if (_lifeLossFunctions.IsEmpty()) return;
        _plotIndex = (_plotIndex + 1) % _lifeLossFunctions.Count;
        ChangePlot(_plotIndex);
    }

    [RelayCommand]
    public void SavePlots()
    {
        string dbpath = @"C:\FDA_Test_Data\WKS20230525\WKS20230525\save-test.db";
        PlotSaver ps = new(dbpath, _lifeLossFunctions);
        ps.SaveToSQLite();
    }

    /// <summary>
    /// Update the combobox of simulations available to the user when a new .fia database is imported
    /// </summary>
    /// <param name="value"></param>
    partial void OnSelectedPathChanged(string value)
    {
        if (SelectedPath.IsNullOrWhiteSpace()) return;

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

    /// <summary>
    /// Update the alternative and hazard time checkboxes when a new simulation is selected
    /// </summary>
    /// <param name="value"></param>
    partial void OnSelectedSimulationChanged(string value)
    {
        // reset the other options
        AlternativesCheckBox.Clear();
        HazardTimesCheckBox.Clear();

        // update options to match the currently selected simulation
        foreach (Simulation simulation in SimulationsComboBox)
        {
            if (simulation.Name == value)
            {
                foreach (string alternative in simulation.Alternatives)
                    AlternativesCheckBox.Add(new CheckableItem { Name = alternative });
                foreach (string hazardTime in simulation.HazardTimes)
                {
                    int time = int.Parse(hazardTime);
                    // display name in military time format
                    HazardTimesCheckBox.Add(new CheckableItem { Name = time < 10 ? $"0{time}00" : $"{time}00", Value = time.ToString() });
                }
            }
        }
    }

    /// <summary>
    /// Update the plot displayed in the UI
    /// </summary>
    /// <param name="next">The index of the plot to be switched to</param>
    private void ChangePlot(int next)
    {
        LifeLossFunction relationship = _lifeLossFunctions[next];
        UncertainPairedData uncertainData = relationship.Data;
        var upper = uncertainData.SamplePairedData(0.975);
        var lower = uncertainData.SamplePairedData(0.025);
        var mid = uncertainData.SamplePairedData(0.5);

        MyModel.Series.Clear();
        AddLineSeriesToPlot(upper, true);
        AddLineSeriesToPlot(lower, true);
        AddLineSeriesToPlot(mid);

        MyModel.ResetAllAxes(); // recenter on the newly plotted lines
        int time = int.Parse(relationship.HazardTime);
        string formattedTime = time < 10 ? $" 0{time}00" : $" {time}00";
        MyModel.Title = relationship.SummaryZone + formattedTime;
        MyModel.InvalidatePlot(true);
    }

    /// <summary>
    /// Add a single line series to a plot
    /// </summary>
    /// <param name="function"></param>
    /// <param name="isConfidenceLimit"></param>
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