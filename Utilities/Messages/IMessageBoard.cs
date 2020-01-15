using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// Provides an interface for aggregating messages written by multiple <see cref="IMessagePublisher"/>s. The <see cref="Subscribe(IMessagePublisher)"/> method provides a thread safe method for adding new <see cref="IMessagePublisher"/>s.
    /// </summary>
    public interface IMessageBoard
    {
        /// <summary>
        /// Retrieves the current set of <see cref="IMessagePublisher"/>s.
        /// </summary>
        IEnumerable<IMessagePublisher> Publishers { get; }
        /// <summary>
        /// Gets the messages for the registered <see cref="IMessagePublisher"/>s.
        /// </summary>
        /// <returns> Collects all the messages published by the current list of <see cref="IMessagePublisher"/>s. </returns>
        IEnumerable<IMessage> ReadMessages();
        /// <summary>
        /// Registers a new <see cref="IMessagePublisher"/> in a thread-safe manner so that their messages can be recieved.
        /// </summary>
        void Subscribe(IMessagePublisher newPublisher);
        /// <summary>
        /// Publishes messages to the board in a thread-safe manner, for use when the <see cref="IMessage"/> publisher is not an <see cref="IMessagePublisher"/> object. If the <paramref name="messages"/> are null or empty they are ignored.
        /// </summary>
        /// <param name="messages"></param>
        void PostMessages(IEnumerable<IMessage> messages);
        /// <summary>
        /// Publishes a single message to the board in a thread-safe manner for use when the <see cref="IMessage"/> publisher is not an <see cref="IMessagePublisher"/> object. If the <paramref name="message"/> is null or empty it is ignored.
        /// </summary>
        /// <param name="message"></param>
        void PostMessage(IMessage message);
    }
}
