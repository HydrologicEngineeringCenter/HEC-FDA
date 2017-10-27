using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.Enumerations;
using Base.Events;

namespace Base.Implementations
{
    public sealed class TextFileMessageSubscriber : Base.Interfaces.IRecieveMessages
    {
        private ErrorLevel _filterLevel;
        private Type _messageTypeFilter;
        private Type _senderTypeFilter;
        
        public ErrorLevel FilterLevel
        {
            get
            {
                return _filterLevel;
            }
        }

        public Type MessageTypeFilter
        {
            get
            {
                return _messageTypeFilter;
            }
        }

        public Type SenderTypeFilter
        {
            get
            {
                return _senderTypeFilter;
            }
        }

        public void RecieveMessage(object sender, MessageEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
