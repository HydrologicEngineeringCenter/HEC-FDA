using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaModel.Messaging
{
    public class Message : IMessage
    {
        private readonly string _message;
        string IMessage.Message
        {
            get
            {
                return _message;
            }
        }
        public Message(string message)
        {
            _message = message;
        }
        public override string ToString()
        {
            return _message;
        }
    }
}
