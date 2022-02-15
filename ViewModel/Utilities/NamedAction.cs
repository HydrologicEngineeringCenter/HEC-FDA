using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Utilities
{
    public class NamedAction: System.ComponentModel.INotifyPropertyChanged
    {
        #region Notes
        #endregion
        #region Fields
        private string _Header;
        private readonly string _Path;
        private bool _IsEnabled = true;
        private bool _IsVisible = true;
        private Action<object, EventArgs> _Click;
        private string _ToolTip;

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region Properties
        public string ToolTip
        {
            get { return _ToolTip; }
            set { _ToolTip = value; NotifyPropertyChanged(nameof(ToolTip)); }
        }
        public string Header
        {
            get { return _Header; }
            set { _Header = value; NotifyPropertyChanged(nameof(Header)); }
        }
        public string Path { get { return _Path; } }
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { _IsEnabled = value; NotifyPropertyChanged(nameof(IsEnabled)); }
        }
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { _IsVisible = value; NotifyPropertyChanged(nameof(IsVisible)); }
        }
        public Action<object, EventArgs> Action
        {
            get { return _Click; }
            set { _Click = value; NotifyPropertyChanged(nameof(Action)); }
        }
        public Utilities.ChildElement Element { get; set; }

        #endregion
        public NamedAction()
        {
            Header = "Default Header Name";
            _Click = delegate (object sender, EventArgs e) { System.Console.WriteLine("To Do: Implement Me!"); };
        }
        public NamedAction(string path)
        {
            _Path = path;
            Header = "Default Header Name";
            _Click = delegate (object sender, EventArgs e) { System.Console.WriteLine("To Do: Implement Me!"); };
        }
        public NamedAction(Utilities.ChildElement element)
        {
            Header = "Default Header Name";
            Element = element;
            _Click = delegate (object sender, EventArgs e) { System.Console.WriteLine("To Do: Implement Me!"); };
        }
        protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
