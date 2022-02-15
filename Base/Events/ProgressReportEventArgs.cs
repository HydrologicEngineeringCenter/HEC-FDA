using System;

namespace HEC.MVVMFramework.Base.Events
{
    public delegate void ProgressReportedEventHandler(object sender, ProgressReportEventArgs progress);
    public class ProgressReportEventArgs : EventArgs
    {
        private readonly int _progress;
        public int Progress { get { return _progress; } }
        public ProgressReportEventArgs(int progress)
        {
            _progress = progress;
        }
    }
}
