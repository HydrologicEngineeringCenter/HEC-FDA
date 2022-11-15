using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Interfaces;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ModelTest.unittests.MessagingTests
{
    internal class Listener : IRecieveMessages
    {
        public List<IMessage> MessageLog { get; set; } = new List<IMessage> { };
        public ErrorLevel FilterLevel => ErrorLevel.Fatal;

        public Type SenderTypeFilter => null;

        public Type MessageTypeFilter => null;

        public void RecieveMessage(object sender, MessageEventArgs e)
        {
            MessageLog.Add(e.Message);
        }
        public string GetMessageLogAsString()
        {
            //Listener can still be recieving messages while this loop is iterating, which changes the loop and throws an exception. This copy prevents that issue
            List<IMessage> messageLogCopy = new(MessageLog);

            List<string> messages = new List<string>();
            foreach (var message in messageLogCopy)
            {
                messages.Add(message.ToString());
            }
            string messagesAsOneString = string.Join(Environment.NewLine, messages.ToArray());
            return messagesAsOneString;
        }
    }
}
