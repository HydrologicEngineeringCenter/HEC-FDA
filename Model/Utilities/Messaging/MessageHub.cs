using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaModel.Messaging
{
    public sealed class MessageHub
    {
        private static List<IRecieveMessages> _subscribers;
        private static List<IReportMessage> _reporters;

        private MessageHub()
        {
            _subscribers = new List<IRecieveMessages>();
            _reporters = new List<IReportMessage>();
        }

        public static readonly MessageHub Instance = new MessageHub();

        public static void Subscribe(IRecieveMessages listener)
        {
            _subscribers.Add(listener);
        }
        public static void Register(IReportMessage messanger)
        {
            messanger.MessageReport += Broadcast;
            _reporters.Add(messanger);
            //ReporterAdded?.Invoke(null, new ReporterAddedEventArgs(messanger));
        }

        private static void Broadcast(object sender, MessageEventArgs e)
        {
            foreach (IRecieveMessages s in _subscribers)
            {
                s.RecieveMessage(sender, e);
            //    if (s is Base.Interfaces.IRecieveInstanceMessages)
            //    {
            //        Base.Interfaces.IRecieveInstanceMessages sinstance = s as Base.Interfaces.IRecieveInstanceMessages;
            //        if (sinstance.InstanceHash != sender.GetHashCode()) { continue; }
            //        if (e.Message is Base.Interfaces.IErrorMessage)
            //        {
            //            if (s.MessageTypeFilter == null || s.MessageTypeFilter == e.Message.GetType())
            //            {
            //                //error filter
            //                Base.Interfaces.IErrorMessage emess = e.Message as Base.Interfaces.IErrorMessage;
            //                if (emess.ErrorLevel >= s.FilterLevel)
            //                {
            //                    s.RecieveMessage(sender, e);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            if (s.MessageTypeFilter == null || s.MessageTypeFilter == e.Message.GetType())
            //            {
            //                s.RecieveMessage(sender, e);
            //            }
            //        }
            //    }
            //    if (e.Message is Base.Interfaces.IErrorMessage)
            //    {
            //        if (s.SenderTypeFilter == null || s.SenderTypeFilter == sender.GetType())
            //        {
            //            if (s.MessageTypeFilter == null || s.MessageTypeFilter == e.Message.GetType())
            //            {
            //                //error filter
            //                Base.Interfaces.IErrorMessage emess = e.Message as Base.Interfaces.IErrorMessage;
            //                if (emess.ErrorLevel >= s.FilterLevel)
            //                {
            //                    s.RecieveMessage(sender, e);
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (s.SenderTypeFilter == null || s.SenderTypeFilter == sender.GetType())
            //        {
            //            if (s.MessageTypeFilter == null || s.MessageTypeFilter == e.Message.GetType())
            //            {
            //                s.RecieveMessage(sender, e);
            //            }
            //        }
            //    }
            //}
            //if (_connected)
            //{
            //    byte[] stream = System.Text.Encoding.ASCII.GetBytes(e.Message.ToString());
            //    _reporter.GetStream().Write(stream, 0, stream.Length);
            //    _reporter.GetStream().Flush();
            }
        }

    }
}
