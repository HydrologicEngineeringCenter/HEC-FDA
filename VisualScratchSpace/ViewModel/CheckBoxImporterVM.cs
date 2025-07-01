using CommunityToolkit.HighPerformance;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SciChart.Core.Extensions;
using System.Collections.ObjectModel;
using VisualScratchSpace.Model;

namespace VisualScratchSpace.ViewModel;
public partial class CheckBoxImporterVM : ObservableObject
{
    public ObservableCollection<Simulation> SimulationsComboBox { get; set; }
    public ObservableCollection<CheckableItem> AlternativesCheckBox { get; set; }
    public ObservableCollection<CheckableItem> HazardTimesCheckBox { get; set; }

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
        List<string> selectedAlernatives = [];
        List<string> selectedHazardTimes = [];
        foreach (CheckableItem a in AlternativesCheckBox)
            if (a.IsChecked) selectedAlernatives.Add(a.Name);
        foreach (CheckableItem h in HazardTimesCheckBox)
            if (h.IsChecked) selectedHazardTimes.Add(h.Value);

        List<string> prefixes = LifeLossDB.GetSimulationTablePrefixes(SelectedSimulation, selectedAlernatives.ToArray(), selectedHazardTimes.ToArray());
        LifeLossDB db = new(SelectedPath);
        //db.QueryMatchingTables(prefixes.ToArray());
        db.CreateSummaryZonePointPairs(SelectedSummarySetPath, SelectedPointsPath); 
    }

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
}
