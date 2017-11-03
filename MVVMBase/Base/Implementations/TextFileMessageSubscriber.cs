//using System;
//using System.Collections;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Base.Enumerations;
//using Base.Events;
//using System.IO;

//namespace Base.Implementations
//{
//    public sealed class TextFileMessageSubscriber : Base.Interfaces.IRecieveMessages
//    {
//        private ErrorLevel _filterLevel = ErrorLevel.Unassigned;
//        private Type _messageTypeFilter = null;
//        private Type _senderTypeFilter = null;
//        private int _maxMessageCount = 100;
//        private object _lock = new object();
//        private static int _enqueue;
//        private static int _dequeue;
//        private string _filePath = System.IO.Path.GetTempFileName();
//        private System.Collections.Concurrent.ConcurrentQueue<Base.Interfaces.IMessage> _messages;
//        public static TextFileMessageSubscriber Instance = new TextFileMessageSubscriber();
//        public ErrorLevel FilterLevel
//        {
//            get
//            {
//                return _filterLevel;
//            }
//            set
//            {
//                _filterLevel = value;
//            }
//        }

//        public Type MessageTypeFilter
//        {
//            get
//            {
//                return _messageTypeFilter;
//            }
//            set
//            {
//                _messageTypeFilter = value;
//            }
//        }
//        public string FilePath
//        {
//            get { return _filePath; }
//            set { _filePath = value; }
//        }
//        public Type SenderTypeFilter
//        {
//            get
//            {
//                return _senderTypeFilter;
//            }
//            set
//            {
//                _senderTypeFilter = value;
//            }
//        }
//        private TextFileMessageSubscriber()
//        {
//            _messages = new System.Collections.Concurrent.ConcurrentQueue<Interfaces.IMessage>();
//            //register
//            Base.Implementations.MessageHub.Subscribe(this);

//        }
//        public void RecieveMessage(object sender, MessageEventArgs e)
//        {
//            _messages.Enqueue(e.Message);
//            _enqueue++;
//            if (_messages.Count > _maxMessageCount)
//            {
//                //dequeue
//                DeQueue();

//            }
//        }
//        private void DeQueue()
//        {
//            System.Text.StringBuilder s = new StringBuilder();
//            Base.Interfaces.IMessage imess;
//            while (_messages.TryDequeue(out imess))
//            {
//                s.AppendLine(imess.ToString());
//                _dequeue++;
//            }
//            //if (!System.IO.File.Exists(_filePath)) { System.IO.File.Create(_filePath); }
//            //System.IO.FileStream sw = new FileStream(_filePath, FileMode.Append, FileAccess.Write);
//            lock (_lock)
//            {
//                //using (System.IO.StreamWriter bw = new System.IO.StreamWriter(sw))
//                //{
//                //    bw.Write(s.ToString());
//                //}
//                //string str = s.ToString();
//                System.Console.Write(s.ToString());
//            }
//        }
//    }
//}
