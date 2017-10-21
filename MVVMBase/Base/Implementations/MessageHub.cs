using System.Collections.Generic;
using Base.Events;

namespace Base.Implementations
{
    public sealed class MessageHub : Base.Interfaces.IReportMessage
    {
        public event MessageReportedEventHandler MessageReport;
        private static List<Base.Interfaces.IRecieveMessages> _subscribers;
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        public static readonly MessageHub Instance = new MessageHub();
        private MessageHub()
        {
            _subscribers = new List<Base.Interfaces.IRecieveMessages>();
        }
        public static void Subscribe(Base.Interfaces.IRecieveMessages listener)
        {
            _subscribers.Add(listener);
        }
        public static void Register(Base.Interfaces.IReportMessage messanger)
        {
            messanger.MessageReport += Broadcast;
        }
        public static void Unregister(Base.Interfaces.IReportMessage messanger)
        {
            messanger.MessageReport -= Broadcast;
        }
        private static void Broadcast(object sender, MessageEventArgs e)
        {
            foreach (Base.Interfaces.IRecieveMessages s in _subscribers)
            {
                if(s is Base.Interfaces.IRecieveInstanceMessages)
                {
                    Base.Interfaces.IRecieveInstanceMessages sinstance = s as Base.Interfaces.IRecieveInstanceMessages;
                    if(sinstance.InstanceHash != sender.GetHashCode()) { continue; }
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
