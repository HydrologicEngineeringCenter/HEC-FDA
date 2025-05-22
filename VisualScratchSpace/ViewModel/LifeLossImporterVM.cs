using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SciChart.Core.Extensions;
using System.Collections.ObjectModel;
using VisualScratchSpace.Model;

namespace VisualScratchSpace.ViewModel
{
    public partial class LifeLossImporterVM : ObservableObject
    {

        public ObservableCollection<Simulation> Simulations { get; set; }
        public ObservableCollection<string> Alternatives { get; set; }
        public ObservableCollection<string> HazardTimes { get; set; }        

        private LifeLossDB _lifeLossDB;
        [ObservableProperty]
        private string _selectedPath;
        [ObservableProperty]
        private string _selectedSimulation;
        [ObservableProperty]
        private string _selectedAlternative;
        [ObservableProperty]
        private string _selectedHazardTime;

        public LifeLossImporterVM()
        {
            SelectedPath = "";
            Simulations = new ObservableCollection<Simulation>();
            Alternatives = new ObservableCollection<string>();
            HazardTimes = new ObservableCollection<string>();
        }

        /// <summary>
        /// Open a LifeSim database and populate combo boxes appropriately
        /// </summary>
        [RelayCommand]
        public void OpenDB()
        {
            if (!SelectedPath.IsNullOrWhiteSpace())
            {
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
                        Alternatives.Add(a);
                    foreach (string h in s.HazardTimes) 
                        HazardTimes.Add(h);
                }
            }
        }
    }
}
