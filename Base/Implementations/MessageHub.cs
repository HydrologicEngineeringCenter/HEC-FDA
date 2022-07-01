using System.Collections.Generic;
using System.Net.Sockets;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Interfaces;

namespace HEC.MVVMFramework.Base.Implementations
{
    public sealed class MessageHub : IReportMessage
    {
        public event MessageReportedEventHandler MessageReport;
        public static event ReporterAddedEventHandler ReporterAdded;
        public static event ReporterRemovedEventHandler ReporterRemoved;
        private static List<IRecieveMessages> _subscribers;
        private static List<IReportMessage> _reporters;
        private static TcpClient _reporter;
        private static TcpListener _listener;
        private static System.Net.IPAddress _ip;
        //private static int _listeningPort;
        //private static int _reportingPort;
        private static bool _connected;
        public static List<IReportMessage> Reporters
        {
            get { return _reporters; }
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        public static System.Net.IPAddress IPAddress
        {
            get { return _ip; }
            set { _ip = value; }
        }
        public static bool Connected
        {
            get { return _connected; }
        }
        public static readonly MessageHub Instance = new MessageHub();
        private MessageHub()
        {
            _subscribers = new List<IRecieveMessages>();
            _reporters = new List<IReportMessage>();
        }
        public static void Subscribe(IRecieveMessages listener)
        {
            _subscribers.Add(listener);
        }
        public static void Register(IReportMessage messanger)
        {
            messanger.MessageReport += Broadcast;
            _reporters.Add(messanger);
            ReporterAdded?.Invoke(null, new ReporterAddedEventArgs(messanger));
        }
        public static void Unregister(IReportMessage messanger)
        {
            messanger.MessageReport -= Broadcast;
            _reporters.Remove(messanger);
            ReporterRemoved?.Invoke(null, new ReporterRemovedEventArgs(messanger));
        }
        public static void InitalizeListener(int port)
        {
            _listener = new TcpListener(_ip, port);
            _listener.Start();

            System.Threading.Timer _t = new System.Threading.Timer(Listen, null, 0, 100);

        }
        private static void Listen(object state)
        {
            if (_listener.Pending())
            {
                Broadcast(_listener, new MessageEventArgs(new Message("Connection Found.")));
                TcpClient client = (TcpClient)_listener.AcceptTcpClientAsync().AsyncState;
                if (client != null)
                {
                    NetworkStream ns = client.GetStream();
                    byte[] bytes = new byte[256];
                    string data;
                    int i;
                    while ((i = ns.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Broadcast(_listener, new MessageEventArgs(new Message(data)));
                    }
                    client.Dispose();
                }
            }
        }
        public static async void AttemptConnectionToListner(int port)
        {
            if (_reporter == null) { _reporter = new TcpClient(); }
            try
            {
                await _reporter.ConnectAsync(_ip, port);
                _connected = true;
            }
            catch
            {
                Broadcast(_reporter, new MessageEventArgs(new Message("Connection attempt failed.")));
            }

        }
        private static void Broadcast(object sender, MessageEventArgs e)
        {
            foreach (IRecieveMessages s in _subscribers)
            {
                if (s is IRecieveInstanceMessages)
                {
                    IRecieveInstanceMessages sinstance = s as IRecieveInstanceMessages;
                    int senderHash = sender.GetHashCode();
                    if (sinstance.InstanceHash != senderHash) 
                    {
                        break; 
                    }
                    if (e.Message is IErrorMessage)
                    {
                        if (s.MessageTypeFilter == null || s.MessageTypeFilter == e.Message.GetType())
                        {
                            //error filter
                            IErrorMessage emess = e.Message as IErrorMessage;
                            if (emess.ErrorLevel >= s.FilterLevel)
                            {
                                s.RecieveMessage(sender, e);
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (s.MessageTypeFilter == null || s.MessageTypeFilter == e.Message.GetType())
                        {
                            s.RecieveMessage(sender, e);
                            break;
                        }
                    }
                }
                if (e.Message is IErrorMessage)
                {
                    if (s.SenderTypeFilter == null || s.SenderTypeFilter == sender.GetType())
                    {
                        if (s.MessageTypeFilter == null || s.MessageTypeFilter == e.Message.GetType())
                        {
                            //error filter
                            IErrorMessage emess = e.Message as IErrorMessage;
                            if (emess.ErrorLevel >= s.FilterLevel)
                            {
                                s.RecieveMessage(sender, e);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (s.SenderTypeFilter == null || s.SenderTypeFilter == sender.GetType())
                    {
                        if (s.MessageTypeFilter == null || s.MessageTypeFilter == e.Message.GetType())
                        {
                            s.RecieveMessage(sender, e);
                            break;
                        }
                    }
                }
            }
            if (_connected)
            {
                byte[] stream = System.Text.Encoding.ASCII.GetBytes(e.Message.ToString());
                _reporter.GetStream().Write(stream, 0, stream.Length);
                _reporter.GetStream().Flush();
            }
        }
        public static void UnsubscribeAll(IRecieveMessages listener)
        {
            foreach (IRecieveMessages s in _subscribers)
            {
                if (s == listener) _subscribers.Remove(s);
            }
        }
        public static void Unsubscribe(IRecieveMessages listener, ErrorLevel filterLevel, System.Type senderFilterType, System.Type messageFilterType)
        {
            foreach (IRecieveMessages s in _subscribers)
            {
                if (s == listener && s.FilterLevel == filterLevel && s.SenderTypeFilter == senderFilterType && s.MessageTypeFilter == messageFilterType) _subscribers.Remove(s);
            }
        }
    }
}
