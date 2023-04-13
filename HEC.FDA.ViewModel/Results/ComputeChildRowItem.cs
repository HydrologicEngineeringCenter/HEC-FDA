using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Results
{
    public class ComputeChildRowItem:BaseViewModel
    {
        private bool _HasError;
        private string _ErrorMessage;
        private bool _IsSelected;
        public ChildElement ChildElement { get; set; }
        public string Name { get; set; }
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; NotifyPropertyChanged(); }
        }
        public bool HasError
        {
            get { return _HasError; }
            set { _HasError = value; NotifyPropertyChanged(); }
        }
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; NotifyPropertyChanged(); }
        }
        //{
        //    get { return _IsSelected; }
        //    set { _IsSelected = value; NotifyPropertyChanged(); }
        //}

        public ComputeChildRowItem(ChildElement childElement)
        {
            Name = childElement.Name;
            ChildElement = childElement;
        }

        public void MarkInError(string errorMessage)
        {
            HasError = true;
            ErrorMessage = errorMessage;
            IsSelected = false;
        }
    }
}
