using System;
using ViewModel.Inventory.OccupancyTypes;

namespace ViewModel.Inventory
{
    public class OcctypeGroupRowItem:BaseViewModel
    {
        public event EventHandler OcctypeGroupSelectionChanged;

        private bool _IsSelected;
        public OccupancyTypesElement GroupElement { get; set; }
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; OcctypeGroupSelectionChanged?.Invoke(this, new EventArgs()); NotifyPropertyChanged(); }
        }

        public OcctypeGroupRowItem(OccupancyTypesElement groupElement)
        {
            GroupElement = groupElement;
        }

    }
}
