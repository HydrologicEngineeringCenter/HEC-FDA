using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Messages
{
    internal class Message: IMessage
    {
        public string Notice { get; }
        public string Details { get; }
        public IMessageLevels Level { get; }

        internal Message(IMessageLevels level, string notice, string details = "")
        {
            Level = level;
            Notice = notice;
            Details = details; 
        }

        public string Concatenate() => $"{Level.ToString().ToUpper()}: {Notice} {Details}";//
    }
}
