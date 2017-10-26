using System.Collections.Generic;
using Base.Events;

namespace Base.Implementations
{
    public sealed class MessageHub : Base.Interfaces.IReportMessage
    {
        public event MessageReportedEventHandler MessageReport;
        public static event ReporterAddedEventHandler ReporterAdded;
        private static List<Base.Interfaces.IRecieveMessages> _subscribers;
        private static List<Base.Interfaces.IReportMessage> _reporters;
        public static List<Base.Interfaces.IReportMessage> Reporters
        {
            get { return _reporters; }
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
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
