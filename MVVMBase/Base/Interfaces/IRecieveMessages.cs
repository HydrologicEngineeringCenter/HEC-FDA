using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Interfaces
{
    public interface IRecieveMessages
    {
        Enumerations.ErrorLevel FilterLevel { get; }
        Type SenderTypeFilter { get; }
        Type MessageTypeFilter { get; }
        void RecieveMessage(object sender, Events.MessageEventArgs e);
    }
}
