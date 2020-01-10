using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Messages
{
    internal class Message: IMessage
    {
        public string Notice { get; }
        public IMessageLevels Level { get; }

        internal Message(IMessageLevels level, string message)
        {
            Level = level;
            Notice = message;
        }
    }
}
