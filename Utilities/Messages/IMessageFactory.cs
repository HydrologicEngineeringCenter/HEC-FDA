using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// Provides methods for the creation of objects implementing the <see cref="IMessage"/> interface.
    /// </summary>
    public static class IMessageFactory
    {
        /// <summary>
        /// Constructs an IMessage.
        /// </summary>
        /// <param name="level"> The severity of type of message being created. </param>
        /// <param name="notice"> A short notice that should be posted with the message. </param>
        /// <param name="details"> An optional more detailed message to be posted with the message. </param>
        /// <returns> An <see cref="IMessage"/> consisting of a string <paramref name="notice"/> or message and its associated level of severity, provide by the <paramref name="level"/> parameter. </returns>
        public static IMessage Factory(IMessageLevels level, string notice, string details = "") => new Messages.Message(level, notice, details);
    }
}
