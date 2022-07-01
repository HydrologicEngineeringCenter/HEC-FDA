using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Enumerations;
using System;
using System.Collections.Generic;

namespace BaseTest.MessageHubTesting
{
    internal class InstanceMessageReciever : HEC.MVVMFramework.Base.Interfaces.IRecieveInstanceMessages
    {
        private int _instanceHash;
        private List<string> _messagesRecieved = new List<string>();

        public List<string> MessagesRecieved
        {
            get { return _messagesRecieved; }
        }

        public int InstanceHash
        {
            get
            {
                return _instanceHash;
            }
        }

        public ErrorLevel FilterLevel => ErrorLevel.Severe;

        public Type SenderTypeFilter => null;

        public Type MessageTypeFilter => null;

        public void RecieveMessage(object sender, MessageEventArgs e)
        {
            int hash = sender.GetHashCode();
            _messagesRecieved.Add("I was poked by " + hash + " " + e.Message.Message);
        }
        public InstanceMessageReciever(int instanceHash)
        {
            _instanceHash = instanceHash;
        }
    }
}
