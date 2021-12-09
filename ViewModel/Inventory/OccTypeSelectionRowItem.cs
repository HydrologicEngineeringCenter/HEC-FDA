using HEC.CS.Collections;
using System.Collections.Generic;
using ViewModel.Inventory.OccupancyTypes;

namespace ViewModel.Inventory
{
    public class OccTypeSelectionRowItem : BaseViewModel
    {
        private OccTypeDisplayName _SelectedOccType;
        private string _OccTypeName;
        public string OccTypeName
        {
            get { return _OccTypeName; }
            set { _OccTypeName = value; NotifyPropertyChanged(); }
        }
        public OccTypeDisplayName SelectedOccType
        {
            get { return _SelectedOccType; }
            set { _SelectedOccType = value; NotifyPropertyChanged(); }
        }

        public bool SelectionMade()
        {
            return SelectedOccType != null;
        }

        public CustomObservableCollection<OccTypeDisplayName> PossibleOccTypes { get; } = new CustomObservableCollection<OccTypeDisplayName>();

        public OccTypeSelectionRowItem(string occTypeName)
        {
            OccTypeName = occTypeName;
        }

        public void UpdateSelectedGroups(List<OcctypeGroupRowItem> selectedGroups)
        {
            //add all the occtypes to the list of possible occtypes
            PossibleOccTypes.Clear();
            foreach(OcctypeGroupRowItem row in selectedGroups)
            {
                string groupName = row.GroupElement.Name;
                List<IOccupancyType> occTypes = row.GroupElement.ListOfOccupancyTypes;
                foreach(IOccupancyType ot in occTypes)
                {
                    PossibleOccTypes.Add(new OccTypeDisplayName(groupName, ot));
                }
            }

            //if we have a selected occtype, then we want to check that it is still in one of our selected groups
            bool foundSelectedOccType = false;
            if(SelectedOccType != null)
            {
                foreach(OccTypeDisplayName ot in PossibleOccTypes)
                {
                    if(SelectedOccType.DisplayName.Equals(ot.DisplayName))
                    {
                        SelectedOccType = ot;
                        break;
                    }
                }
            }

            //if we did not find our currently selected occtype then we should see if we can auto select it.
            if (!foundSelectedOccType)
            {
                AutoSelectOcctype();
            }
        }

        private void AutoSelectOcctype()
        {
            SelectedOccType = null;
            foreach(OccTypeDisplayName ot in PossibleOccTypes)
            {
                if(ot.OccType.Name.Equals(OccTypeName) )
                {
                    SelectedOccType = ot;
                    break;
                }
            }
        }

    }
}
