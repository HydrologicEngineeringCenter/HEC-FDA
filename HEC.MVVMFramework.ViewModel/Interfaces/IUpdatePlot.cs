namespace HEC.MVVMFramework.ViewModel.Interfaces
{
    public interface IUpdatePlot
    {
        event Events.UpdatePlotEventHandler UpdatePlotEvent;
        void InvalidatePlotModel(bool updateData);
        void UpdatePlot(object sender, Events.UpdatePlotEventArgs e);
    }
}
