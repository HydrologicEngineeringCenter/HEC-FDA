namespace ViewModel.Interfaces
{
    public interface IClose
    {
        event Events.CloseEventHandler CloseEvent;
        void CloseProgram(object sender, Events.CloseEventArgs e);
    }
}
