using HEC.CS.Collections;

namespace HEC.FDA.ViewModel.Inventory
{
    public class DefineSIAttributesRowItem
    {
        public string Name { get; set; }

        /// <summary>
        /// The user defined default value
        /// </summary>
        public CustomObservableCollection<string> Items { get;} = new CustomObservableCollection<string>();
        /// <summary>
        /// The selected value from the combobox
        /// </summary>
        public string SelectedItem { get; set; }

        public DefineSIAttributesRowItem(string name)
        {
            Name = name;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(SelectedItem);
        }

    }
}
