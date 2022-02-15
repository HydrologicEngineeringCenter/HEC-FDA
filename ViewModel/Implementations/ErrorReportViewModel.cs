using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace HEC.MVVMFramework.ViewModel.Implementations
{
    public class ErrorReportViewModel : BaseViewModel
    {
        private IMessage _message;
        private int _messageCount = 100;
        private System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
        public IMessage IMessage
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
        public void SetErrors(Dictionary<string, IPropertyRule> ruleMap)
        {
            IMessage imsg = null;
            int messageCount = ruleMap.Values.Count();
            MessageCounter = messageCount;

            foreach (IPropertyRule r in ruleMap.Values)
            {
                string msg = "";
                if (r.ErrorLevel > ErrorLevel.Unassigned)
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
