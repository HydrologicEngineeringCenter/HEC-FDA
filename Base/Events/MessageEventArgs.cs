using System;
using Base.Interfaces;

namespace Base.Events
{
    public delegate void MessageReportedEventHandler(object sender, MessageEventArgs e);
    public class MessageEventArgs: EventArgs
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
