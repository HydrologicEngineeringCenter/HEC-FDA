using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Implementations
{
    public class ErrorReportViewModel : Implementations.BaseViewModel
    {
        private Base.Interfaces.IMessage _message;
        private int _messageCount = 100;
        private System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
        public Base.Interfaces.IMessage IMessage
        {
            get { return _message; }
            set { _message = value; NotifyPropertyChanged(); }
        }
        public int MessageCounter
        {
            get
            {
                return _messageCount;
            }
            set
            {
                _messageCount = value; NotifyPropertyChanged();
            }
        }
        public void SetErrors(Dictionary<string, Base.Interfaces.IPropertyRule> ruleMap)
        {
            Base.Interfaces.IMessage imsg = null;
            int messageCount = ruleMap.Values.Count();
            MessageCounter = messageCount;

            foreach (Base.Interfaces.IPropertyRule r in ruleMap.Values)
            {
                string msg = "";
                if (r.ErrorLevel > Base.Enumerations.ErrorLevel.Unassigned)
                {

                    foreach (string m in r.Errors)
                    {
                        msg += m;
                    }
                    imsg = new Model.Messaging.ErrorMessage(msg, r.ErrorLevel);
                    IMessage = imsg;
                }

            }
        }
        public ErrorReportViewModel()
        {

        }
    }
}
