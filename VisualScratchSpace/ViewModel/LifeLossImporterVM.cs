using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace VisualScratchSpace.ViewModel
{
    public partial class LifeLossImporterVM : ObservableObject
    {
        [ObservableProperty]
        private string _selectedPath;

        [RelayCommand]
        public void OpenDB()
        {
            // this can be a call to a model method opening the DB
            // TODO: get something from that DB and update something on the UI as proof of concept
            // also want to use the hazard time and alternative name at some point
            SelectedPath += "_opened";
        }
    }
}
