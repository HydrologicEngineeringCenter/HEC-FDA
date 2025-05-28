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

    public CheckBoxImporterVM()
    {
        SimulationsComboBox = [];
        AlternativesCheckBox = [];
        HazardTimesCheckBox = [];
    }

    [RelayCommand]
    public void OpenDB()
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
                    HazardTimesCheckBox.Add(new CheckableItem { Name = time < 10 ? $"0{time}00" : $"{time}00" });
                } 
            }
        }
    }
}
