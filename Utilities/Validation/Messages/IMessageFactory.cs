using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// Provides external access for the construction of IMessage implementations.
    /// </summary>
    public static class IMessageFactory
    {
        /// <summary>
        /// Constructs an IMessage.
        /// </summary>
        /// <param name="level"> The severity of type of message being created. </param>
        /// <param name="notice"> The notice that should be posted with the message. </param>
        /// <returns></returns>
        public static IMessage Factory(IMessageLevels level, string notice) => new Validation.Messages.Message(level, notice);
    }
}
