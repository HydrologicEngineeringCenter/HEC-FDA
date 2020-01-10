using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Messages
{
    internal class MessageSubscriber : IMessageSubscriber
    {
        private System.Collections.Concurrent.ConcurrentQueue<IMessagePublisher> _Publishers;
        public IEnumerable<IMessagePublisher> Publishers
        {
            get { return _Publishers; }
        }

        internal MessageSubscriber()
        {
            _Publishers = new System.Collections.Concurrent.ConcurrentQueue<IMessagePublisher>();
        }
        internal MessageSubscriber(IMessagePublisher publisher)
        {
            _Publishers = new System.Collections.Concurrent.ConcurrentQueue<IMessagePublisher>();
            if (!publisher.IsNull()) Subscribe(publisher);
        }
        internal MessageSubscriber(IEnumerable<IMessagePublisher> publishers)
        {
            _Publishers = new System.Collections.Concurrent.ConcurrentQueue<IMessagePublisher>(publishers);
        }

        public IEnumerable<IMessage> ReadMessages()
        {
            List<IMessage> msgs = new List<IMessage>();
            foreach (var publisher in Publishers) msgs.AddRange(publisher.Messages);
            return msgs;
        }
        public void Subscribe(IMessagePublisher publisherToRegister)
        {
            _Publishers.Enqueue(publisherToRegister);
        }
    }
}
