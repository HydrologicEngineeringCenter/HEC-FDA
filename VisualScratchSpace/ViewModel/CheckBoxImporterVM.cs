using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SciChart.Core.Extensions;
using System.Collections.ObjectModel;
using VisualScratchSpace.Model;

namespace VisualScratchSpace.ViewModel;
public partial class CheckBoxImporterVM : ObservableObject
{
    public ObservableCollection<Simulation> Simulations { get; set; }
    public ObservableCollection<CheckableItem> Alternatives { get; set; }
    public ObservableCollection<CheckableItem> HazardTimes { get; set; }

    [ObservableProperty]
    private string _selectedPath;
    [ObservableProperty]
    private string _selectedSimulation;

    public CheckBoxImporterVM()
    {
        Simulations = new();
        Alternatives = new();
        HazardTimes = new();
    }

    [RelayCommand]
    public void OpenDB()
    {
        if (SelectedPath.IsNullOrWhiteSpace())
            return;

        // reset the simulation options
        Simulations.Clear();
        LifeLossDB db = new LifeLossDB(SelectedPath);

        // add new simulations from the newly selected database
        List<Simulation> newSimulations = db.UpdateSimulations();
        foreach (Simulation simulation in newSimulations)
        {
            Simulations.Add(simulation);
        }
    }

    partial void OnSelectedSimulationChanged(string value)
    {
        UpdateSimulationFields(value);
    }

    private void UpdateSimulationFields(string value)
    {
        // reset the other options
        Alternatives.Clear();
        HazardTimes.Clear();

        // update options to match the currently selected simulation
        foreach (Simulation s in Simulations)
        {
            if (s.Name == value)
            {
                foreach (string a in s.Alternatives)
                    Alternatives.Add(new CheckableItem { Name = a });
                foreach (string h in s.HazardTimes)
                {
                    int time = int.Parse(h);
                    // display name in military time format
                    HazardTimes.Add(new CheckableItem { Name = time < 10 ? $"0{time}00" : $"{time}00" });
                }
                    
            }
        }
    }
}
