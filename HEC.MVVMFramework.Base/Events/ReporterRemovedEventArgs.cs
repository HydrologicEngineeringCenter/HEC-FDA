using HEC.MVVMFramework.Base.Interfaces;
using System;


namespace HEC.MVVMFramework.Base.Events
{
    public delegate void ReporterRemovedEventHandler(object sender, ReporterRemovedEventArgs e);
    public class ReporterRemovedEventArgs : EventArgs
    {
        private readonly IReportMessage _reporter;
        public IReportMessage Reporter
        {
            get { return _reporter; }
        }
        public ReporterRemovedEventArgs(IReportMessage reporter)
        {
            _reporter = reporter;
        }
    }
}
