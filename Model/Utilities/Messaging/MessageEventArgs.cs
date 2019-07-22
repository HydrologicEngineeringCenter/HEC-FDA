using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaModel.Messaging
{
    public delegate void MessageReportedEventHandler(object sender, MessageEventArgs e);

    public class MessageEventArgs : EventArgs
    {
        private readonly IMessage _message;
        public IMessage Message
        {
            get { return _message; }
        }
        public MessageEventArgs(IMessage message)
        {
            _message = message;
        }
    }
}
