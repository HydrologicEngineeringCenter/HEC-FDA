using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Enumerations;
using System;
using System.Collections.Generic;
using HEC.MVVMFramework.Base.Interfaces;

namespace BaseTest.MessageHubTesting
{
    internal class InstanceMessageReciever : IRecieveInstanceMessages
    {
        private List<int> _instanceHash = new List<int>();
        private List<string> _messagesRecieved = new List<string>();

        public List<string> MessagesRecieved
        {
            get { return _messagesRecieved; }
        }

        public ErrorLevel FilterLevel => ErrorLevel.Severe;

        public Type SenderTypeFilter => null;

        public Type MessageTypeFilter => null;

        List<int> IRecieveInstanceMessages.InstanceHash { get { return _instanceHash; } set { _instanceHash = value; } }

        public void RecieveMessage(object sender, MessageEventArgs e)
        {
            int hash = sender.GetHashCode();
            _messagesRecieved.Add("I was poked by " + hash + " " + e.Message.Message);
        }
        public InstanceMessageReciever(int instanceHash)
        {
            _instanceHash.Add(instanceHash);
        }
    }
}
