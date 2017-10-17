using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Interfaces
{
    public interface IReportMessage
    {
        event Events.MessageReportedEventHandler MessageReport;
        void ReportMessage(object sender, Events.MessageEventArgs e);
    }
}
