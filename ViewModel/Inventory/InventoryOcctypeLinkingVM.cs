using HEC.CS.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Inventory
{
    public class InventoryOcctypeLinkingVM : BaseViewModel
    {
        #region Fields
        private List<string> _OccupancyTypesInFile;
        #endregion
        #region Properties
        public List<OcctypeGroupRowItem> OccTypeGroups { get; } = new List<OcctypeGroupRowItem>();
        public CustomObservableCollection<OccTypeSelectionRowItem> Rows { get; } = new CustomObservableCollection<OccTypeSelectionRowItem>();

        public List<string> OccupancyTypesInFile
        {
            get { return _OccupancyTypesInFile; }
            set { _OccupancyTypesInFile = value; NotifyPropertyChanged(); }
        }

        #endregion
        #region Constructors

        public InventoryOcctypeLinkingVM(List<string> occtypeNames)
        {
            OccupancyTypesInFile = occtypeNames;
            List<OccupancyTypesElement> occupancyTypesElements = StudyCache.GetChildElementsOfType<OccupancyTypesElement>();

            foreach (OccupancyTypesElement elem in occupancyTypesElements)
            {
                OcctypeGroupRowItem row = new OcctypeGroupRowItem(elem);
                row.OcctypeGroupSelectionChanged += OcctypeGroupSelectionChanged;
                OccTypeGroups.Add(row);
            }

            LoadRows();

            //select the first group
            if(OccTypeGroups.Count>0)
            {
                OccTypeGroups[0].IsSelected = true;
                UpdateRowsWithSelectedGroups();
            }

        }

        /// <summary>
        /// Add a row item for each unique occtype in the structure inventory.
        /// </summary>
        private void LoadRows()
        {
            foreach(string occtypeName in OccupancyTypesInFile)
            {
                Rows.Add(new OccTypeSelectionRowItem(occtypeName));
            }
        }

        /// <summary>
        /// When an occtype group gets selected or unselected then we need to update all the possible occtypes
        /// to choose from in the rows.
        /// </summary>
        private void UpdateRowsWithSelectedGroups()
        {
            List<OcctypeGroupRowItem> selectedGroups = GetSelectedGroups();
            foreach(OccTypeSelectionRowItem row in Rows)
            {
                row.UpdateSelectedGroups(selectedGroups);
            }
        }

        /// <summary>
        /// Gets the selected occtype groups.
        /// </summary>
        /// <returns></returns>
        private List<OcctypeGroupRowItem> GetSelectedGroups()
        {
            List<OcctypeGroupRowItem> selectedGroups = new List<OcctypeGroupRowItem>();
            foreach (OcctypeGroupRowItem row in OccTypeGroups)
            {
                if(row.IsSelected)
                {
                    selectedGroups.Add(row);
                }
            }
            return selectedGroups;
        }

        #endregion
        #region Voids

        private void OcctypeGroupSelectionChanged(object sender, EventArgs e)
        {
            UpdateRowsWithSelectedGroups();
        }

 

        #endregion
        #region Functions

        /// <summary>
        /// Checks that all the selections have been made
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public FdaValidationResult AreRowsValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            List<OccTypeSelectionRowItem> rowsWithSelections = Rows.Where(row => row.SelectionMade()).ToList();
            if(rowsWithSelections.Count != Rows.Count)
            {
                vr.AddErrorMessage("A selection must be made for each occupancy type.");
            }
            return vr;
        }

        /// <summary>
        /// The key is the occtype name in the structures shapefile. The value is the selected occtype
        /// from this study's occtypes.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, OcctypeReference> CreateOcctypeMapping()
        {
            Dictionary<string, OcctypeReference> dict = new Dictionary<string, OcctypeReference>();
            foreach (OccTypeSelectionRowItem row in Rows)
            {
                OcctypeReference otRef = new OcctypeReference(row.SelectedOccType.OccType.GroupID, row.SelectedOccType.OccType.ID);
                dict.Add(row.OccTypeName, otRef);
            }
            return dict;
        }

        #endregion
    }
}
