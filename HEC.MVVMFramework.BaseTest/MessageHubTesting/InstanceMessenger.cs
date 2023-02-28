using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Events;


namespace BaseTest.MessageHubTesting
{
    public class InstanceMessenger : HEC.MVVMFramework.Base.Interfaces.IReportMessage
    {
        public event MessageReportedEventHandler MessageReport;

        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }

        public void Poke()
        {
            ReportMessage(this, new MessageEventArgs(new Message("poked ya")));
        }
    }
}
