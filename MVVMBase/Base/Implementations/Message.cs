using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.Interfaces;

namespace Base.Implementations
{
    public class Message : Interfaces.IMessage
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
    }
}
