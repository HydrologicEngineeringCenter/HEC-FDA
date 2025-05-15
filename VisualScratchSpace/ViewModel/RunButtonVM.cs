using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace VisualScratchSpace.ViewModel

{
    public partial class RunButtonVM : ObservableObject
    {
        private bool _running = false;

        [ObservableProperty]
        private string _btnText = "Run";

        [ObservableProperty]
        private string _status = "Stopped";

        [RelayCommand]
        private void Run()
        {
            _running = !_running;
            BtnText = _running ? "Stop" : "Run";
            Status = _running ? "Running" : "Stopped";
        }
    }
}

