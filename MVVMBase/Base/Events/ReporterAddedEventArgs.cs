using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Events
{
    public delegate void ReporterAddedEventHandler(object sender, ReporterAddedEventArgs e);
    public class ReporterAddedEventArgs: EventArgs
    {
        private readonly Base.Interfaces.IReportMessage _reporter;
        public Base.Interfaces.IReportMessage Reporter
        {
            get { return _reporter; }
        }
        public ReporterAddedEventArgs(Base.Interfaces.IReportMessage reporter)
        {
            _reporter = reporter;
        }
    }
}
