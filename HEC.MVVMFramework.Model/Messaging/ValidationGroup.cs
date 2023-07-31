using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HEC.MVVMFramework.Model.Messaging
{
    public class ValidationGroup : Validation, IReportMessage
    {
        private const string GROUP_INDENTATION = "    ";
        private List<ValidationErrorLogger> _ValidationObjects = new List<ValidationErrorLogger>();
        private List<string> _IntroMessages = new List<string>();
        private string _IntroMessage;

        public event MessageReportedEventHandler MessageReport;

        public List<ValidationGroup> ChildGroups { get; set; } = new List<ValidationGroup>();

        public ValidationGroup(string intoMessage)
        {
            MessageHub.Register(this);
            _IntroMessage = intoMessage;
        }

        public ValidationGroup(List<ValidationErrorLogger> validationObjects, List<string> introMessages, string introMessage)
        {
            MessageHub.Register(this);
            _ValidationObjects = validationObjects;
            _IntroMessages = introMessages;
            _IntroMessage = introMessage;
        }

        public new string GetErrorMessages()
        {
            string errorMsg = null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _ValidationObjects.Count; i++)
            {
                string msg = _ValidationObjects[i].GetErrorMessages(_IntroMessages[i]);
                if (msg != null)
                {
                    //add indent to each line
                    msg = GROUP_INDENTATION + msg.Replace(Environment.NewLine, Environment.NewLine + GROUP_INDENTATION);
                    sb.AppendLine(msg);
                }
            }

            //now handle sub groups:
            foreach (ValidationGroup vg in ChildGroups)
            {
                string msg = vg.GetErrorMessages();
                if (msg != null)
                {
                    //add indent to each line
                    msg = GROUP_INDENTATION + msg.Replace(Environment.NewLine, Environment.NewLine + GROUP_INDENTATION);
                    sb.AppendLine(msg);
                }
            }

            if (sb.Length != 0)
            {
                errorMsg = _IntroMessage + Environment.NewLine + Environment.NewLine + sb.ToString();
            }
            return errorMsg;

        }

        //can i override the has errors and get errors and error level etc?

        private ErrorLevel GetHighestErrorLevel(List<ValidationErrorLogger> validationObjects)
        {
            ErrorLevel highestErrorLevel = ErrorLevel.Unassigned;
            foreach (ValidationErrorLogger validationObject in validationObjects)
            {
                if (validationObject.ErrorLevel > highestErrorLevel)
                {
                    highestErrorLevel = validationObject.ErrorLevel;
                }
            }
            return highestErrorLevel;
        }

        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
    }
}
