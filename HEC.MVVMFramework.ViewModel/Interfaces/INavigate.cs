using HEC.MVVMFramework.ViewModel.Events;

namespace HEC.MVVMFramework.ViewModel.Interfaces
{
    public interface INavigate
    {
        event NavigationEventHandler NavigationEvent;
        void Navigate(object sender, NavigationEventArgs e);
    }
}
