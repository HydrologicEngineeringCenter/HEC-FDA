using System;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;

namespace HEC.FDA.ViewModel.Inventory
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
