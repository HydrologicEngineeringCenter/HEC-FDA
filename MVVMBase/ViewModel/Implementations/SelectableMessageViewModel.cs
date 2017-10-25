using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.Enumerations;
using Base.Events;
using System.Reflection;

namespace ViewModel.Implementations
{
    public class SelectableMessageViewModel : Base.Interfaces.IRecieveMessages, System.ComponentModel.INotifyPropertyChanged
    {
        private Base.Interfaces.IMessage _message;
        public event PropertyChangedEventHandler PropertyChanged;
        private Base.Enumerations.ErrorLevel _filterLevel = ErrorLevel.Unassigned;
        private Type _messageType = null;
        private int _messageCount = 10;
        private Type _senderType = null;
        private TypeRepresentation _sender = null;
        private List<TypeRepresentation> _reporters;
        public List<TypeRepresentation> Reporters
        {
            get { return _reporters; }
        }
        public Base.Interfaces.IMessage IMessage
        {
            get { return _message; }
            set { _message = value; NotifyPropertyChanged(); }
        }
        public int MessageCounter
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
        public TypeRepresentation Sender
        {
            get
            {
                return _sender;
            }
            set
            {
                _sender = value;
                if (_sender == null)
                {
                    SenderTypeFilter = null;
                }
                else
                {
                    SenderTypeFilter = _sender.Type();
                }
                NotifyPropertyChanged();
            }
        }
        public SelectableMessageViewModel()
        {
            Base.Implementations.MessageHub.ReporterAdded += MessageHub_ReporterAdded;
            MessageHub_ReporterAdded(null, null);

        }
        private void MessageHub_ReporterAdded(object sender, ReporterAddedEventArgs e)
        {
            _reporters = new List<TypeRepresentation>();
            TypeRepresentation nullType = new TypeRepresentation();
            nullType.Count = Base.Implementations.MessageHub.Reporters.Count();
            _reporters.Add(nullType);
            foreach (Base.Interfaces.IReportMessage r in Base.Implementations.MessageHub.Reporters)
            {
                TypeRepresentation newTr = new TypeRepresentation(r);
                bool hasMatch = false;
                foreach (TypeRepresentation tr in _reporters)
                {
                    if (tr.Assembly == newTr.Assembly)
                    {
                        hasMatch = true;
                        tr.Count++;
                    }
                }
                if (!hasMatch) { _reporters.Add(new TypeRepresentation(r)); }
            }
            if (_reporters.Count() > 0) { Sender = _reporters.First(); }
            NotifyPropertyChanged(nameof(Reporters));
        }

        protected virtual void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void RecieveMessage(object sender, MessageEventArgs e)
        {
            IMessage = e.Message;
        }
        public class TypeRepresentation
        {
            public string Name;
            public int Count;
            public string Assembly;
            private Type _type;
            protected int Hash;
            public TypeRepresentation(object o)
            {
                Count = 1;
                Type t = o.GetType();
                System.Reflection.TypeInfo ti = t.GetTypeInfo();
                Base.Attributes.ReporterDisplayNameAttribute rdma = (Base.Attributes.ReporterDisplayNameAttribute)ti.GetCustomAttribute(typeof(Base.Attributes.ReporterDisplayNameAttribute));
                if (rdma != null)
                {
                    Name = rdma.DisplayName;
                }
                else
                {
                    Name = o.GetType().Name;
                }
                _type = t;
                Hash = o.GetHashCode();
                Assembly = o.GetType().AssemblyQualifiedName;
            }
            public TypeRepresentation()
            {
                Count = 1;
                _type = null;
                Name = "All Reports";

            }
            public Type Type()
            {
                return _type;
            }
            public override string ToString()
            {
                return Name + " [" + Count + "]";
            }
        }
    }
}
