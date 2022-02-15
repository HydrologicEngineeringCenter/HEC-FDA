using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor
{
    /// <summary>
    /// This class is used for the items in the combos in the IASEditor. The main point of
    /// this class is so that i can easily add an empty row item in each combo by passing
    /// in a null element here and the binding to Name will still work.
    /// </summary>
    public class ChildElementComboItem:BaseViewModel
    {
        private const int INVALID_ID = -1;
        private string _Name;
        private ChildElement _ChildElement;
        public int ID { get; set; } = INVALID_ID;
        public ChildElement ChildElement
        {
            get { return _ChildElement; }
            set 
            { 
                _ChildElement = value; 
                Name = _ChildElement.Name; 
            }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }
        public ChildElementComboItem(ChildElement element)
        {
            if(element == null)
            {
                _Name = "";
            }
            else
            {
                _Name = element.Name;
                ID = element.GetElementID();
            }
            _ChildElement = element;
        }
    }
}
