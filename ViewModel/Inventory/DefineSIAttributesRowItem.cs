using HEC.CS.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Inventory
{
    public class DefineSIAttributesRowItem
    {

        public string Name { get; set; }
        public bool UseDefault { get; set; } = false;
        /// <summary>
        /// The user defined default value
        /// </summary>
        public string DefaultValue { get; set; }
        public CustomObservableCollection<string> Items { get;} = new CustomObservableCollection<string>();
        /// <summary>
        /// The selected value from the combobox
        /// </summary>
        public string SelectedItem { get; set; }

        /// <summary>
        /// Returns the default value if the user selected "Missing", else returns
        /// the selected item from the combobox.
        /// </summary>
        public string SelectedValue
        {
            get
            {
                if (UseDefault)
                {
                    return DefaultValue;
                }
                else
                {
                    return SelectedItem;
                }
            }
        }

        public DefineSIAttributesRowItem(string name)
        {
            Name = name;
        }

        public bool IsValid()
        {
            bool isValid = true;
            if(UseDefault && DefaultValue == "")
            {
                isValid = false;
            }
            else if(SelectedItem == null || SelectedItem == "")
            {
                isValid = false;
            }
            return isValid;
        }

    }
}
