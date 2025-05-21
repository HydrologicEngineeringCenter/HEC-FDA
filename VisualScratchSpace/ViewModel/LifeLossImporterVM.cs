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

        [RelayCommand]
        public void OpenDB()
        {
            // this can be a call to a model method opening the DB
            // TODO: get something from that DB and update something on the UI as proof of concept
            // also want to use the hazard time and alternative name at some point
            if (!SelectedPath.IsNullOrWhiteSpace())
            {
                Simulations.Clear();
                Simulation s = new Simulation();
                s.Name = "sim 1";
                s.Alternatives = new List<string> { "al11", "al12" };
                s.HazardTimes = new List<string> { "hz11", "hz12" };
                Simulation d = new Simulation();
                d.Name = "sim 2";
                d.Alternatives = new List<string> { "al21", "al22" };
                d.HazardTimes = new List<string> { "hz21", "hz22" };
                Simulations.Add(s);
                Simulations.Add(d);
                SelectedPath += "_opened";
            }
           
        }

        partial void OnSelectedSimulationChanged(string value)
        {
            UpdateSimulationFields(value);
        }

        private void UpdateSimulationFields(string value)
        {
            Alternatives.Clear();
            HazardTimes.Clear();
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
