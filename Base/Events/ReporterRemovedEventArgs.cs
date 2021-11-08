using System;


namespace Base.Events
{
    public delegate void ReporterRemovedEventHandler(object sender, ReporterRemovedEventArgs e);
    public class ReporterRemovedEventArgs : EventArgs
    {
        private readonly Base.Interfaces.IReportMessage _reporter;
        public Base.Interfaces.IReportMessage Reporter
        {
            get { return _reporter; }
        }
        public ReporterRemovedEventArgs(Base.Interfaces.IReportMessage reporter)
        {
            _reporter = reporter;
        }
    }
}
