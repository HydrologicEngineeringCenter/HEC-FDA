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
        /// A note summarizing the message.
        /// </summary>
        string Notice { get; }
        /// <summary>
        /// A longer or more detailed message.
        /// </summary>
        string Details { get; }
    }
}
