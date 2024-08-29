using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// Provides a method for constructing new <see cref="IMessageBoard"/> objects.
    /// </summary>
    public static class IMessageBoardFactory
    {
        /// <summary>
        /// Generates a new <see cref="IMessageBoard"/> with no subscriptions. <see cref="IMessagePublisher"/>s can be added later through the thread-safe <see cref="IMessageBoard.Subscribe(IMessagePublisher)"/> method.
        /// </summary>
        /// <returns> An <see cref="IMessageBoard"/>. </returns>
        public static IMessageBoard Factory() => new Messages.MessageBoard();
        /// <summary>
        /// Generates a new <see cref="IMessageBoard"/> with a single subscription. Additional <see cref="IMessagePublisher"/>s can be added later through the thread-safe <see cref="IMessageBoard.Subscribe(IMessagePublisher)"/> method.
        /// </summary>
        /// <returns> An <see cref="IMessageBoard"/>. </returns>
        public static IMessageBoard Factory(IMessagePublisher publisher) => new Messages.MessageBoard(publisher);
        /// <summary>
        /// Generates a new <see cref="IMessageBoard"/>. Additional <see cref="IMessagePublisher"/>s can be added later through the thread-safe <see cref="IMessageBoard.Subscribe(IMessagePublisher)"/> method.
        /// </summary>
        /// <returns> An <see cref="IMessageBoard"/>. </returns>
        public static IMessageBoard Factory(IEnumerable<IMessagePublisher> publishers) => new Messages.MessageBoard(publishers);
        public static IMessageBoard Factory(IEnumerable<IMessage> messages) => new Messages.MessageBoard(messages);
        public static IMessageBoard Factory(IMessage message) => new Messages.MessageBoard(message);
    }
}
