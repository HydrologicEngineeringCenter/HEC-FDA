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
        /// <returns> An <see cref="IMessage"/> consisting of a string <paramref name="notice"/> or message and its associated level of severity, provide by the <paramref name="level"/> parameter. </returns>
        public static IMessage Factory(IMessageLevels level, string notice) => new Messages.Message(level, notice);
    }
}
