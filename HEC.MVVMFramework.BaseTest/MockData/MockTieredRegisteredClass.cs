using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using System;
using System.Collections.Generic;

namespace HEC.MVVMFramework.BaseTest.MockData
{
    internal class MockTieredRegisteredClass : IReportMessage
    {
        public event MessageReportedEventHandler MessageReport;

        public IReportMessage MessageReporterProperty { get; set; }
        public IReportMessage[] ArrayOfMessageReporterProperty { get; set; }
        public List<IReportMessage> ListOfMessageReporterProperty { get; set; }
        public int StructProperty { get; set; }
        public Message ObjectProperty { get; set; }

        public MockTieredRegisteredClass()
        {
            
        }

        public void ReportMessage(object sender, MessageEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
