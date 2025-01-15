using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;

namespace HEC.FDA.ViewModel.Utilities
{
    public sealed class TextFileMessageSubscriber : IRecieveMessages, IDisposable
    {
        private ErrorLevel _filterLevel = ErrorLevel.Unassigned;
        private Type _messageTypeFilter = null;
        private Type _senderTypeFilter = null;
        private object _bwListLock = new object();
        private static int _enqueue;
        private static int _dequeue;
        private string _filePath = Path.GetTempFileName();
        StreamWriter sw;
        private BackgroundWorker _bw;
        private ConcurrentQueue<IMessage> _messages;
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
        //TODO: What am I?
        private TextFileMessageSubscriber()
        {
            //start timer, action every 5 seconds call the flush method on SW. check if bwlistlock is locked an not isbusy. could
            //also look at if deque is >0.
            //_timer = new System.Timers.Timer();
            //_timer.Interval = 5000;
            //_timer.Elapsed += flushStringWriter;
            //_timer.Start();

            FilterLevel = ErrorLevel.Unassigned;
            _messages = new ConcurrentQueue<IMessage>();
            //register
            MessageHub.Subscribe(this);
            if (!File.Exists(_filePath))
            {
                File.Create(_filePath);
            }
            sw = new StreamWriter(new FileStream(_filePath, FileMode.Create, FileAccess.Write));
            sw.AutoFlush = true;
            _bw = new BackgroundWorker();
            _bw.DoWork += _bw_DoWork;
        }

        //private void flushStringWriter(object sender, EventArgs e)
        //{
        //    sw.Flush();
        //}
        //Phenomenally Fragile. Could crush memory if not careful.
        public void RecieveMessage(object sender, MessageEventArgs e)
        {
            _messages.Enqueue(e.Message);
            Interlocked.Increment(ref _enqueue);
            if (!_bw.IsBusy)
            {
                //dequeue
                lock (_bwListLock)
                {
                    if (!_bw.IsBusy)
                    {
                        _bw.RunWorkerAsync();
                    }
                }
            }
        }

        private void _bw_DoWork(object sender, DoWorkEventArgs e)
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
                if (!string.IsNullOrEmpty(str))
                {
                    sw.Write(str);
                }
            }
            catch (Exception ex)
            {
                string msg = "Exception occured trying to write messages to the log file: " + ex.ToString();
                sw.Write(msg);
            }
        }
        public void Dispose()
        {
            DeQueue();
            sw.Dispose();
            sw.Close();
        }
    }
}
