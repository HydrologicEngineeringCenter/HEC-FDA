using System;
using System.ComponentModel;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Interfaces;

namespace HEC.MVVMFramework.ViewModel.Implementations
{
    public class SubscriberMessageViewModel : IRecieveMessages, INotifyPropertyChanged
    {
        private IMessage _message;
        public event PropertyChangedEventHandler PropertyChanged;
        private ErrorLevel _filterLevel = ErrorLevel.Unassigned;
        private Type _senderType = null;
        private Type _messageType = null;
        private int _messageCount = 100;

        private int _instanceFilter;

        public int InstanceFilter
        {
            get { return _instanceFilter; }
            set { _instanceFilter = value; NotifyPropertyChanged(); }
        }

        public IMessage IMessage
        {
            get { return _message; }
            set { _message = value; NotifyPropertyChanged(); }
        }
        public ErrorLevel FilterLevel
        {
            get
            {
                return _filterLevel;
            }
            set
            {
                _filterLevel = value; NotifyPropertyChanged();
            }
        }

        public Type SenderTypeFilter
        {
            get
            {
                return _senderType;
            }
            set
            {
                System.Diagnostics.Debugger.Break();
                _senderType = value; NotifyPropertyChanged();
            }
        }

        public Type MessageTypeFilter
        {
            get
            {
                return _messageType;
            }
            set
            {
                _messageType = value; NotifyPropertyChanged();
            }
        }
        public int MessageCount
        {
            get
            {
                return _messageCount;
            }
            set
            {
                _messageCount = value; NotifyPropertyChanged();
            }
        }
        protected virtual void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void RecieveMessage(object sender, MessageEventArgs e)
        {
            IMessage = e.Message;
        }
    }
}
