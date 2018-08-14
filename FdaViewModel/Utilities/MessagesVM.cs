using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
{
    public class MessagesVM: BaseViewModel
    {
        #region Fields
        public List<MessageItem> _messages = new List<MessageItem>();
        #endregion
        #region Properties
        public List<MessageItem> MessageList
        {
            get { return _messages; }
            set { _messages = value;NotifyPropertyChanged(); }
        }
        #endregion
        public MessagesVM(List<FdaModel.Utilities.Messager.ErrorMessage> messages)
        {
            List<MessageItem>_mess = new List<MessageItem>();
            foreach(FdaModel.Utilities.Messager.ErrorMessage m in messages)
            {
                _mess.Add(new MessageItem(m.Message, m.ErrorLevel.ToString(), m.ReportedFrom, m.Date, m.User));
            }
            MessageList = _mess;
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
            //AddRule(nameof(MessageList), () => MessageList.Count < 0, "Message List must have 0 elements!");
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }
    }
}
