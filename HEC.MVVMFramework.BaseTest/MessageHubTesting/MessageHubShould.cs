using System.Collections.Generic;
using Xunit;
using HEC.MVVMFramework.Base.Implementations;

namespace BaseTest.MessageHubTesting
{
    [Trait("RunsOn", "Remote")]
    public class MessageHubShould
    {
        [Fact]
        public void RelayMessages()
        {
            int iterations = 10;

            InstanceMessenger messenger1 = new InstanceMessenger();
            InstanceMessenger messenger2 = new InstanceMessenger();
            MessageHub.Register(messenger1);
            MessageHub.Register(messenger2);
            List<InstanceMessenger> messengerList = new List<InstanceMessenger>();
            messengerList.Add(messenger1);
            messengerList.Add(messenger2);

            int message1Hash = messenger1.GetHashCode();

            InstanceMessageReciever reciever = new InstanceMessageReciever(message1Hash);
            MessageHub.Subscribe(reciever); 

            for(int i = 0; i < iterations; i++)
            {
                foreach(InstanceMessenger me in messengerList)
                {
                    int hash = me.GetHashCode();
                    me.Poke();
                }
            }
            Assert.Equal(iterations, reciever.MessagesRecieved.Count);
        }
    }
}