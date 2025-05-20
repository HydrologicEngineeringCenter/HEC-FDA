using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SciChart.Core.Extensions;
using System.Collections.ObjectModel;
using VisualScratchSpace.Model;

namespace VisualScratchSpace.ViewModel
{
    public partial class LifeLossImporterVM : ObservableObject
    {

        public ObservableCollection<string> Simulations { get; set; }
        public ObservableCollection<string> Alternatives { get; set; }
        public ObservableCollection<string> HazardTimes { get; set; }        

        private List<Simulation> _simulations;
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
            Simulations = new ObservableCollection<string>();
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
        }
    }
}
