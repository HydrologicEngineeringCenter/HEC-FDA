using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Events;
using System;

namespace HEC.MVVMFramework.Base.Interfaces
{
    public interface IRecieveMessages
    {
        ErrorLevel FilterLevel { get; }
        Type SenderTypeFilter { get; }
        Type MessageTypeFilter { get; }
        void RecieveMessage(object sender, MessageEventArgs e);
    }
}
