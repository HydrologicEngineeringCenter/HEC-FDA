using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public String GetErrorMessages(string introMessage)
        {
            string errorsMsg = null;
            if (HasErrors)
            {
                //each class can have multiple string errors but it will only have one error level
                //List<IErrorMessage> errorMessages = GetErrors().Cast<IErrorMessage>().ToList();
                string errorMessages = GetErrorMessages();
                if (errorMessages.Length > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    if (introMessage != null)
                    {
                        sb.AppendLine(introMessage).AppendLine();
                    }
                    string[] lines = errorMessages.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    foreach (string line in lines)
                    {
                        if (!"".Equals(line))
                        {
                            sb.AppendLine("    * " + line).AppendLine();
                        }
                    }
                    errorsMsg = sb.ToString();
                }

            }
            return errorsMsg;
        }

        public void LogErrors(string introMessage)
        {
            string errorMsg = GetErrorMessages(introMessage);
            ReportMessage(this, new MessageEventArgs(new Message(errorMsg)));
        }

        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }

    }
}
