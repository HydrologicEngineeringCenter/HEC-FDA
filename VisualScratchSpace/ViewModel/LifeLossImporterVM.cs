using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace VisualScratchSpace.ViewModel
{
    public partial class LifeLossImporterVM : ObservableObject
    {
        [ObservableProperty]
        private string _selectedPath;

        [RelayCommand]
        public void OpenDB(string path)
        {
            SelectedPath += "_opened";
        }
    }
}
