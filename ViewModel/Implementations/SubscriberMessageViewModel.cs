using System;
using System.Collections.Generic;
using System.ComponentModel;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;

namespace HEC.MVVMFramework.ViewModel.Implementations
{
    public class SubscriberMessageViewModel : IRecieveInstanceMessages, INotifyPropertyChanged
    {
        private IMessage _message;
        public event PropertyChangedEventHandler PropertyChanged;
        private ErrorLevel _filterLevel = ErrorLevel.Unassigned;
        private Type _senderType = null;
        private Type _messageType = null;
        private int _messageCount = 100;
        private List<int> _instanceHash;
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
        public List<int> InstanceHash
        {
            get { return _instanceHash; }
            set { _instanceHash = value; NotifyPropertyChanged(); }
        }

        public SubscriberMessageViewModel()
        {
            MessageHub.Subscribe(this);
            InstanceHash = new List<int>();
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
