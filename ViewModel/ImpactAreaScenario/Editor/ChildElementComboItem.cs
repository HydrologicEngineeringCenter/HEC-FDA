using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Utilities;

namespace ViewModel.ImpactAreaScenario.Editor
{
    /// <summary>
    /// This class is used for the items in the combos in the IASEditor. The main point of
    /// this class is so that i can easily add an empty row item in each combo by passing
    /// in a null element here and the binding to Name will still work.
    /// </summary>
    public class ChildElementComboItem:BaseViewModel
    {

        public ChildElement ChildElement { get; set; }
        public string Name { get; set; }

        public ChildElementComboItem(ChildElement element)
        {
            ChildElement = element;
            if(element == null)
            {
                Name = "";
            }
            else
            {
                Name = element.Name;
            }
        }

    }
}
