using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaModel.Messaging
{
    public interface IReportMessage
    {
        event MessageReportedEventHandler MessageReport;
        void ReportMessage(object sender, MessageEventArgs e);
    }
}
