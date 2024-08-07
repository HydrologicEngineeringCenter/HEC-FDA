﻿using HEC.CS.Collections;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;

namespace HEC.FDA.ViewModel.Inventory
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
            OccTypeDisplayName currentlySelectedItem = SelectedOccType;
            //add all the occtypes to the list of possible occtypes
            PossibleOccTypes.Clear();
            foreach(OcctypeGroupRowItem row in selectedGroups)
            {
                string groupName = row.GroupElement.Name;
                List<OccupancyType> occTypes = row.GroupElement.ListOfOccupancyTypes;
                foreach(OccupancyType ot in occTypes)
                {
                    PossibleOccTypes.Add(new OccTypeDisplayName(groupName,row.GroupElement.ID, ot));
                }
            }

            //if we have a selected occtype, then we want to check that it is still in one of our selected groups
            bool foundSelectedOccType = false;
            if(currentlySelectedItem != null)
            {
                foreach(OccTypeDisplayName ot in PossibleOccTypes)
                {
                    if(currentlySelectedItem.DisplayName.Equals(ot.DisplayName))
                    {
                        SelectedOccType = ot;
                        foundSelectedOccType = true;
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
