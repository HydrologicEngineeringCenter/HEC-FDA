using System;
using System.Text;
using System.IO;
using System.Threading;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Base.Implementations;

namespace HEC.MVVMFramework.View.Implementations
{
    public sealed class TextFileMessageSubscriber : IRecieveMessages, IDisposable
    {
        private ErrorLevel _filterLevel = ErrorLevel.Unassigned;
        private Type _messageTypeFilter = null;
        private Type _senderTypeFilter = null;
        private int _maxMessageCount = 100;
        private object _lock = new object();
        private object _bwListLock = new object();
        private static int _enqueue;
        private static int _dequeue;
        private string _filePath = Path.GetTempFileName();
        StreamWriter sw;
        //private System.Collections.Generic.List<System.ComponentModel.BackgroundWorker> _bwList;
        private System.ComponentModel.BackgroundWorker _bw;
        private System.Collections.Concurrent.ConcurrentQueue<IMessage> _messages;
        public static TextFileMessageSubscriber Instance = new TextFileMessageSubscriber();
        public ErrorLevel FilterLevel
        {
            get
            {
                return _filterLevel;
            }
            set
            {
                _filterLevel = value;
            }
        }

        public Type MessageTypeFilter
        {
            get
            {
                return _messageTypeFilter;
            }
            set
            {
                _messageTypeFilter = value;
            }
        }
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                if (!File.Exists(_filePath))
                {
                    sw = new StreamWriter(new FileStream(_filePath, FileMode.Create, FileAccess.Write));
                }
                else
                {
                    sw = new StreamWriter(new FileStream(_filePath, FileMode.Append, FileAccess.Write));
                }
            }
        }
        public Type SenderTypeFilter
        {
            get
            {
                return _senderTypeFilter;
            }
            set
            {
                _senderTypeFilter = value;
            }
        }
        private TextFileMessageSubscriber()
        {
            _messages = new System.Collections.Concurrent.ConcurrentQueue<IMessage>();
            //register
            MessageHub.Subscribe(this);
            if (!File.Exists(_filePath)) { File.Create(_filePath); }
            sw = new StreamWriter(new FileStream(_filePath, FileMode.Create, FileAccess.Write));
            sw.AutoFlush = true;
            _bw = new System.ComponentModel.BackgroundWorker();
            _bw.DoWork += _bw_DoWork;
        }
        public void RecieveMessage(object sender, MessageEventArgs e)
        {
            _messages.Enqueue(e.Message);
            Interlocked.Increment(ref _enqueue);
            if (_messages.Count > _maxMessageCount && !_bw.IsBusy)
            {
                //dequeue
                lock (_bwListLock)
                {
                    if (!_bw.IsBusy) _bw.RunWorkerAsync();
                }


            }
        }
        private void _bw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DeQueue();
        }
        private void DeQueue()
        {
            StringBuilder s = new StringBuilder();
            IMessage imess;
            while (_messages.TryDequeue(out imess))
            {
                s.AppendLine(imess.ToString());
                Interlocked.Increment(ref _dequeue);
            }
            try
            {
                var str = s.ToString();
                if (!string.IsNullOrEmpty(str)) sw.Write(str);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
            }

        }
        public void Dispose()
        {
            //int busyCount = 0;
            //for (int i = 0; i < _bwList.Count(); i++)
            //{
            //    if (!_bwList[i].IsBusy)
            //    {
            //        busyCount++;
            //    }
            //}
            sw.Dispose();
            sw.Close();
        }
    }
}
