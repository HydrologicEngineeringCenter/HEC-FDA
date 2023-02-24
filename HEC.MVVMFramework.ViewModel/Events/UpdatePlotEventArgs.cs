using System;

namespace HEC.MVVMFramework.ViewModel.Events
{
    public delegate void UpdatePlotEventHandler(object sender, UpdatePlotEventArgs e);
    public class UpdatePlotEventArgs : EventArgs
    {
        public bool UpdateData { get; set; }
        public int MinMillisecondsBetweenUpdates { get; set; }
        public UpdatePlotEventArgs(bool updateData = true, int updateLag = 30)
        {
            UpdateData = updateData;
            MinMillisecondsBetweenUpdates = updateLag;
        }
    }
}
