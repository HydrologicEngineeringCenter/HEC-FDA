using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// Provides a method for constructing new <see cref="IMessageSubscriber"/> objects.
    /// </summary>
    public static class IMessageSubscriberFactory
    {
        /// <summary>
        /// Generates a new <see cref="IMessageSubscriber"/> with no subscriptions. <see cref="IMessagePublisher"/>s can be added later through the thread-safe <see cref="IMessageSubscriber.Subscribe(IMessagePublisher)"/> method.
        /// </summary>
        /// <returns> An <see cref="IMessageSubscriber"/>. </returns>
        public static IMessageSubscriber Factory() => new Messages.MessageSubscriber();
        /// <summary>
        /// Generates a new <see cref="IMessageSubscriber"/> with a single subscription. Additional <see cref="IMessagePublisher"/>s can be added later through the thread-safe <see cref="IMessageSubscriber.Subscribe(IMessagePublisher)"/> method.
        /// </summary>
        /// <returns> An <see cref="IMessageSubscriber"/>. </returns>
        public static IMessageSubscriber Factory(IMessagePublisher publisher) => new Messages.MessageSubscriber(publisher);
        /// <summary>
        /// Generates a new <see cref="IMessageSubscriber"/>. Additional <see cref="IMessagePublisher"/>s can be added later through the thread-safe <see cref="IMessageSubscriber.Subscribe(IMessagePublisher)"/> method.
        /// </summary>
        /// <returns> An <see cref="IMessageSubscriber"/>. </returns>
        public static IMessageSubscriber Factory(IEnumerable<IMessagePublisher> publishers) => new Messages.MessageSubscriber(publishers);
    }
}
