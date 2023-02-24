using HEC.MVVMFramework.Base.Interfaces;
using System;

namespace HEC.MVVMFramework.Base.Events
{
    public delegate void ReporterAddedEventHandler(object sender, ReporterAddedEventArgs e);
    public class ReporterAddedEventArgs : EventArgs
    {
        private readonly IReportMessage _reporter;
        public IReportMessage Reporter
        {
            get { return _reporter; }
        }
        public ReporterAddedEventArgs(IReportMessage reporter)
        {
            _reporter = reporter;
        }
    }
}
