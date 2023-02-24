using System.ComponentModel;

namespace HEC.MVVMFramework.ViewModel.Implementations
{
    public class BaseViewModel : Interfaces.IBaseViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
