using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Messages
{
    internal class MessageBoard : IMessageBoard
    {
        private System.Collections.Concurrent.ConcurrentQueue<IMessagePublisher> _Publishers;
        private System.Collections.Concurrent.ConcurrentQueue<IMessage> _OtherMessages;
        public IEnumerable<IMessagePublisher> Publishers
        {
            get { return _Publishers; }
        }

        internal MessageBoard()
        {
            _Publishers = new System.Collections.Concurrent.ConcurrentQueue<IMessagePublisher>();
            _OtherMessages = new System.Collections.Concurrent.ConcurrentQueue<IMessage>();
        }
        internal MessageBoard(IMessagePublisher publisher)
        {
            _Publishers = new System.Collections.Concurrent.ConcurrentQueue<IMessagePublisher>();
            _OtherMessages = new System.Collections.Concurrent.ConcurrentQueue<IMessage>();
            Subscribe(publisher);
        }
        internal MessageBoard(IEnumerable<IMessagePublisher> publishers)
        {
            _Publishers = new System.Collections.Concurrent.ConcurrentQueue<IMessagePublisher>(publishers);
            _OtherMessages = new System.Collections.Concurrent.ConcurrentQueue<IMessage>();
        }
        internal MessageBoard(IEnumerable<IMessage> messages)
        {
            _Publishers = new System.Collections.Concurrent.ConcurrentQueue<IMessagePublisher>();
            _OtherMessages = new System.Collections.Concurrent.ConcurrentQueue<IMessage>();
            PostMessages(messages);
        }
        internal MessageBoard(IMessage message)
        {
            _Publishers = new System.Collections.Concurrent.ConcurrentQueue<IMessagePublisher>();
            _OtherMessages = new System.Collections.Concurrent.ConcurrentQueue<IMessage>();
            PostMessage(message);
        }
        public IEnumerable<IMessage> ReadMessages()
        {
            List<IMessage> msgs = new List<IMessage>( _OtherMessages );
            foreach (var publisher in Publishers) msgs.AddRange(publisher.Messages);
            return msgs;
        }
        public void Subscribe(IMessagePublisher publisherToRegister)
        {
            if (!publisherToRegister.IsNull())_Publishers.Enqueue(publisherToRegister);
        }
        public void PostMessages(IEnumerable<IMessage> messages)
        {
            foreach (IMessage msg in messages) if(msg.Level > IMessageLevels.NoErrors && msg.Notice.Length > 0) _OtherMessages.Enqueue(msg);
        }
        public void PostMessage(IMessage message)
        {
            if (!message.IsNull() && message.Level > IMessageLevels.NoErrors && message.Notice.Length > 0) _OtherMessages.Enqueue(message);
        }
    }
}
