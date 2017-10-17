using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Events
{
    public delegate void ProgressReportedEventHandler(object sender, ProgressReportEventArgs progress);
    public class ProgressReportEventArgs: EventArgs 
    {
        private readonly int _progress;
        public int Progress { get { return _progress; } }
        public ProgressReportEventArgs(int progress)
        {
            _progress = progress;
        }
    }
}
