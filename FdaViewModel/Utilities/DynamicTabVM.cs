using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
{
    public class DynamicTabVM:BaseViewModel, IDynamicTab
    {

        public event EventHandler RemoveEvent;
        public event EventHandler PopTabOutEvent;

        private BaseViewModel _BaseVM;
        private string _Header;
        

        public string Header
        {
            get { return _Header; }
            set { _Header = value; NotifyPropertyChanged(); }
        }
        public bool CanDelete { get; set; }
        public BaseViewModel BaseVM
        {
            get { return _BaseVM;  }
            set { _BaseVM = value; NotifyPropertyChanged(); }
        }
      

        public DynamicTabVM(string header, BaseViewModel baseVM, bool canDelete = true)
        {
            CanDelete = canDelete;
            Header = header;
            BaseVM = baseVM;
        }
        
        public void PopTabOut()
        {
            PopTabOutEvent?.Invoke(this, new EventArgs());
        }
        public void RemoveTab()
        {
            RemoveEvent?.Invoke(this, new EventArgs());
        }

        //public void DisableEditContextMenuItem()
        //{
        //    foreach (Utilities.NamedAction a in ((BaseFdaElement)BaseVM).Actions)
        //    {
        //        if (a.Header.Equals(EditString))
        //        {
        //            a.IsEnabled = false;
        //        }
        //    }
        //}

        //public void EnableEditContextMenuItem()
        //{
        //    foreach (Utilities.NamedAction a in ((BaseFdaElement)BaseVM).Actions)
        //    {
        //        if (a.Header.Equals(EditString))
        //        {
        //            a.IsEnabled = true;
        //        }
        //    }
        //}
    }
}
