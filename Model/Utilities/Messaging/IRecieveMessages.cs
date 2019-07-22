using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaModel.Messaging
{
    public interface IRecieveMessages
    {
        ErrorLevel FilterLevel { get; }
        Type SenderTypeFilter { get; }
        Type MessageTypeFilter { get; }
        void RecieveMessage(object sender, MessageEventArgs e);
    }
}
