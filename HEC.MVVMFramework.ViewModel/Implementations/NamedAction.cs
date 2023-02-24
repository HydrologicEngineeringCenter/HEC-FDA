using System;
using System.ComponentModel;

namespace HEC.MVVMFramework.ViewModel.Implementations
{
    public class NamedAction : Interfaces.IDisplayableNamedAction, INotifyPropertyChanged
    {
        #region Fields
        private string _Header;
        private bool _IsEnabled = true;
        private bool _IsVisible = true;
        private Action<object, EventArgs> _Action;

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region Properties
        public string Name
        {
            get { return _Header; }
            set { _Header = value; NotifyPropertyChanged(); }
        }
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { _IsEnabled = value; NotifyPropertyChanged(); }
        }
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { _IsVisible = value; NotifyPropertyChanged(); }
        }
        public Action<object, EventArgs> Action
        {
            get { return _Action; }
            set { _Action = value; NotifyPropertyChanged(); }
        }
        #endregion
        public NamedAction()
        {
            Name = "Default Header Name";
            _Action = (sender, e) => { throw new NotImplementedException("Implement an action on " + Name); };
        }
        protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
