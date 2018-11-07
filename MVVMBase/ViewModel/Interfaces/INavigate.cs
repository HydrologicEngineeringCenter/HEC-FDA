namespace ViewModel.Interfaces
{
    public interface INavigate
    {
        event Events.NavigationEventHandler NavigationEvent;
        void Navigate(object sender, Events.NavigationEventArgs e);
    }
}
