

using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Interfaces;
using System;
using System.Collections.Generic;

namespace fda_model_test.unittests.MessagingTests
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
            List<string> messages = new List<string>();
            foreach(var message in MessageLog)
            {
                messages.Add(message.ToString());
            }
            string messagesAsOneString = String.Join(Environment.NewLine, messages.ToArray());
            return messagesAsOneString;
        }
    }
}
