using System.Collections.Generic;
using Base.Events;
using System.Net.Sockets;

namespace Base.Implementations
{
    public sealed class MessageHub : Base.Interfaces.IReportMessage
    {
        public event MessageReportedEventHandler MessageReport;
        public static event ReporterAddedEventHandler ReporterAdded;
        public static event ReporterRemovedEventHandler ReporterRemoved;
        private static List<Base.Interfaces.IRecieveMessages> _subscribers;
        private static List<Base.Interfaces.IReportMessage> _reporters;
        private static System.Net.Sockets.TcpClient _reporter;
        private static System.Net.Sockets.TcpListener _listener;
        private static System.Net.IPAddress _ip;
        //private static int _listeningPort;
        //private static int _reportingPort;
        private static bool _connected;
        public static List<Base.Interfaces.IReportMessage> Reporters
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
            _subscribers = new List<Base.Interfaces.IRecieveMessages>();
            _reporters = new List<Interfaces.IReportMessage>();
        }
        public static void Subscribe(Base.Interfaces.IRecieveMessages listener)
        {
            _subscribers.Add(listener);
        }
        public static void Register(Base.Interfaces.IReportMessage messanger)
        {
            messanger.MessageReport += Broadcast;
            _reporters.Add(messanger);
            ReporterAdded?.Invoke(null, new ReporterAddedEventArgs(messanger));
        }
        public static void Unregister(Base.Interfaces.IReportMessage messanger)
        {
            messanger.MessageReport -= Broadcast;
            _reporters.Remove(messanger);
            ReporterRemoved?.Invoke(null, new ReporterRemovedEventArgs(messanger));
        }
        public static void InitalizeListener(int port)
        {
            _listener = new System.Net.Sockets.TcpListener(_ip, port);
            _listener.Start();
            
            System.Threading.Timer _t = new System.Threading.Timer(Listen,null,0,100);

        }
        private static void Listen(object state)
        {
            if (_listener.Pending())
            {
                Broadcast(_listener, new MessageEventArgs(new Message("Connection Found.")));
                System.Net.Sockets.TcpClient client = (System.Net.Sockets.TcpClient)_listener.AcceptTcpClientAsync().AsyncState;
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
            foreach (Base.Interfaces.IRecieveMessages s in _subscribers)
            {
                if (s is Base.Interfaces.IRecieveInstanceMessages)
                {
                    Base.Interfaces.IRecieveInstanceMessages sinstance = s as Base.Interfaces.IRecieveInstanceMessages;
                    if (sinstance.InstanceHash != sender.GetHashCode()) { continue; }
                    if (e.Message is Base.Interfaces.IErrorMessage)
                    {
                        if (s.MessageTypeFilter == null || s.MessageTypeFilter == e.Message.GetType())
                        {
                            //error filter
                            Base.Interfaces.IErrorMessage emess = e.Message as Base.Interfaces.IErrorMessage;
                            if (emess.ErrorLevel >= s.FilterLevel)
                            {
                                s.RecieveMessage(sender, e);
                            }
                        }
                    }
                    else
                    {
                        if (s.MessageTypeFilter == null || s.MessageTypeFilter == e.Message.GetType())
                        {
                            s.RecieveMessage(sender, e);
                        }
                    }
                }
                if (e.Message is Base.Interfaces.IErrorMessage)
                {
                    if (s.SenderTypeFilter == null || s.SenderTypeFilter == sender.GetType())
                    {
                        if (s.MessageTypeFilter == null || s.MessageTypeFilter == e.Message.GetType())
                        {
                            //error filter
                            Base.Interfaces.IErrorMessage emess = e.Message as Base.Interfaces.IErrorMessage;
                            if (emess.ErrorLevel >= s.FilterLevel)
                            {
                                s.RecieveMessage(sender, e);
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
        public static void UnsubscribeAll(Base.Interfaces.IRecieveMessages listener)
        {
            foreach (Base.Interfaces.IRecieveMessages s in _subscribers)
            {
                if (s == listener) _subscribers.Remove(s);
            }
        }
        public static void Unsubscribe(Base.Interfaces.IRecieveMessages listener, Base.Enumerations.ErrorLevel filterLevel, System.Type senderFilterType, System.Type messageFilterType)
        {
            foreach (Base.Interfaces.IRecieveMessages s in _subscribers)
            {
                if (s == listener && s.FilterLevel == filterLevel && s.SenderTypeFilter == senderFilterType && s.MessageTypeFilter == messageFilterType) _subscribers.Remove(s);
            }
        }
    }
}
