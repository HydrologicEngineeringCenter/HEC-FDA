using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using System.Collections.Generic;

namespace HEC.MVVMFramework.Model.Messaging
{
    public class ValidationErrorLogger : Validation, IReportMessage
    {
        public event MessageReportedEventHandler MessageReport;

        public ValidationErrorLogger()
        {
            MessageHub.Register(this);
        }

        public void LogErrors()
        {
            LogErrors(null);
        }

        public void LogErrors(string introMessage)
        {
            Validate();
            if (HasErrors)
            {
                //each class can have multiple string errors but it will only have one error level
                List<IErrorMessage> errorMessages = GetErrorMessages();
                if(errorMessages.Count>0 && introMessage != null)
                {
                    ReportMessage(this, new MessageEventArgs(new Message(introMessage)));
                }
                foreach(IErrorMessage errorMessage in errorMessages)
                {
                    ReportMessage(this, new MessageEventArgs(errorMessage));
                }
            }
        }

        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }

    }
}
