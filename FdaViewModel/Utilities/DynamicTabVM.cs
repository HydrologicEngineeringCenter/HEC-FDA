using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
{
    public class DynamicTabVM:BaseViewModel
    {
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

        public override void AddValidationRules()
        {
           // throw new NotImplementedException();
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }
    }
}
