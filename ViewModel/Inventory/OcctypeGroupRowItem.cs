using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            set { _IsSelected = value; SelectionChanged(); OcctypeGroupSelectionChanged?.Invoke(this, new EventArgs()); NotifyPropertyChanged(); }
        }

        public OcctypeGroupRowItem(OccupancyTypesElement groupElement)
        {
            GroupElement = groupElement;
        }

        private void SelectionChanged()
        {

        }

    }
}
