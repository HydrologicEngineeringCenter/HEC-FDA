using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Model.Messaging;

namespace ViewTest.Model
{
    public class Counter: IReportMessage
    {
        private int _count;

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }
        private int _interval;

        public event MessageReportedEventHandler MessageReport;

        public int Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }
        public Counter( int initialCount, int countingInterval)
        {
            Count = initialCount;
            Interval = countingInterval;
            MessageHub.Register(this);
        }
        public void DoCounting()
        {
            Count = Count + Interval;
            IMessage message = new ErrorMessage("New Count is " + Count, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
            MessageEventArgs args = new MessageEventArgs(message);
            ReportMessage(this, args);
        }

        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
    }
}
