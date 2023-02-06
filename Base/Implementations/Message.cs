using HEC.MVVMFramework.Base.Interfaces;
using System;

namespace HEC.MVVMFramework.Base.Implementations
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
            _message = DateTime.Now + " " + message;
        }
        public override string ToString()
        {
            return _message;
        }
    }
}
