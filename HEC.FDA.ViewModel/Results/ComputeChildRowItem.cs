using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Results
{
    public class ComputeChildRowItem
    {
        public ChildElement ChildElement { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        //{
        //    get { return _IsSelected; }
        //    set { _IsSelected = value; NotifyPropertyChanged(); }
        //}

        public ComputeChildRowItem(ChildElement childElement)
        {
            Name = childElement.Name;
            ChildElement = childElement;
        }

        public void ValidateElement()
        {

        }
    }
}
