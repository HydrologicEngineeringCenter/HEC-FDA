using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            if(messages == null) { return; }
            List<MessageItem>_mess = new List<MessageItem>();
            foreach(FdaModel.Utilities.Messager.ErrorMessage m in messages)
            {
                _mess.Add(new MessageItem(m.Message, m.ErrorLevel.ToString(), m.ReportedFrom, m.Date, m.User));
            }
            MessageList = _mess;
        }

        public MessagesVM()
        {
            MessageList = MessagesVM.GetMessages();
        }

        public static List<MessageItem> GetMessages()
        {
            List<MessageItem> allMessages = new List<MessageItem>();

            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                if (Storage.Connection.Instance.TableNames().Contains("Messages"))
                {
                    if (!Storage.Connection.Instance.IsOpen) { Storage.Connection.Instance.Open(); }
                    DatabaseManager.DataTableView dtv = Storage.Connection.Instance.GetTable("Messages");
                    object[] row = null;
                    for (int i = 0; i < dtv.NumberOfRows; i++)
                    {
                        row = dtv.GetRow(i);
                        allMessages.Add(new MessageItem((string)row[0],(string)row[1], (string)row[4], (string)row[3], (string)row[2]));
                    }
                }
            }

            return allMessages;
        }

        public static ObservableCollection<MessageRowItem> GetMessageRowsForElement(BaseFdaElement elem)
        {
            List<MessageItem> allMessages = MessagesVM.GetMessages();
            ObservableCollection<MessageRowItem> messages = new ObservableCollection<MessageRowItem>();
            foreach (MessageItem mes in allMessages)
            {
                if(mes.ReportedFrom == nameof(FlowTransforms.InflowOutflowElement))
                {
                    //messages.Add(new MessageRowItem(mes.Date,mes.Message,mes.User));
                }
            }
            return messages;

        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
            //AddRule(nameof(MessageList), () => MessageList.Count < 0, "Message List must have 0 elements!");
        }

       
    }
}
