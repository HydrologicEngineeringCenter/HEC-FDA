using HEC.MVVMFramework.Base.Events;

namespace HEC.MVVMFramework.Base.Interfaces
{
    public interface IReportMessage
    {
        event MessageReportedEventHandler MessageReport;
        void ReportMessage(object sender, MessageEventArgs e);
    }
}
