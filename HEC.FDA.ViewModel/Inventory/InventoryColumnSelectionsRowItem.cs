using HEC.CS.Collections;

namespace HEC.FDA.ViewModel.Inventory
{
    public class InventoryColumnSelectionsRowItem
    {
        private string _DisplayName;
        /// <summary>
        /// This is the label in the Structures import UI
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The user defined default value
        /// </summary>
        public CustomObservableCollection<string> Items { get;} = new CustomObservableCollection<string>();
        /// <summary>
        /// The selected value from the combobox
        /// </summary>
        public string SelectedItem { get; set; } = "";

        public string MissingValueColumnHeader
        {
            get { return _DisplayName + " (" + SelectedItem + ")"; }
        }

        public InventoryColumnSelectionsRowItem(string name, string displayName = "")
        {
            Name = name;
            _DisplayName = displayName;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(SelectedItem);
        }

    }
}
