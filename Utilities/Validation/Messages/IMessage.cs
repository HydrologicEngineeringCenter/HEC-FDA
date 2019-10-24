using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// Provides an interface for message and error notifications.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// The severity or type of message.
        /// </summary>
        IMessageLevels Level { get; }
        /// <summary>
        /// The note to be posted with the message.
        /// </summary>
        string Notice { get; }
    }
}
