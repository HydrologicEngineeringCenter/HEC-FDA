using HEC.CS.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.Utilities;
using System.Windows;
using Geospatial.IO;
using Geospatial.Features;
using Utility.Logging;
using Utility.Memory;

namespace HEC.FDA.ViewModel.Inventory
{
    public class InventoryOcctypeLinkingVM : BaseViewModel
    {
        #region Fields
        private string _Path;
        private string _SelectedOcctypeName;
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

        public InventoryOcctypeLinkingVM(string filePath, string selectedOcctypeName)
        {
            //get the list of occtype names yourself, don't pass them in.
            _Path = filePath;
            _SelectedOcctypeName = selectedOcctypeName;
            OccupancyTypesInFile = GetUniqueOccupancyTypes();
            LoadOcctypeGroups();
            LoadRows();

            //select the first group
            if(OccTypeGroups.Count>0)
            {
                OccTypeGroups[0].IsSelected = true;
                UpdateRowsWithSelectedGroups();
            }
        }

        /// <summary>
        /// Loading this from an existing inventory element. We are in edit mode.
        /// </summary>
        /// <param name="occtypeNames">The occtypes that are in the inventory shapefile.</param>
        /// <param name="occtypeMappings">The saved occtype mappings from fda occtypes to the inventory shapefile occtypes stored on the inventory element.</param>
        public InventoryOcctypeLinkingVM(string filePath, string selectedOcctypeName, Dictionary<string, OcctypeReference> occtypeMappings)
        {
            _Path = filePath;
            _SelectedOcctypeName = selectedOcctypeName;
            OccupancyTypesInFile = GetUniqueOccupancyTypes();

            LoadOcctypeGroups();
            LoadRows();
            SelectRequiredOcctypeGroups(occtypeMappings);
            SelectOcctypesInRows(occtypeMappings);
        }

        private void SelectOcctypesInRows(Dictionary<string, OcctypeReference> occtypeMappings)
        {
            foreach(OccTypeSelectionRowItem row in Rows)
            {
                string shapefileOcctypeName = row.OccTypeName;
                if(occtypeMappings.ContainsKey(shapefileOcctypeName))
                {
                    SelectOcctypeInRow(row, occtypeMappings[shapefileOcctypeName]);
                }
                else
                {
                    //we couldn't find the occtype. I guess we don't need to do anything. The row will probably just have a blank combobox.
                }
            }
        }

        private void SelectOcctypeInRow(OccTypeSelectionRowItem row, OcctypeReference otRef)
        {
            foreach(OccTypeDisplayName ot in row.PossibleOccTypes)
            {
                if(ot.GroupID == otRef.GroupID && ot.OccType.ID == otRef.ID)
                {
                    row.SelectedOccType = ot;
                    break;
                }
            }
        }

        /// <summary>
        /// Loop over all our stored occtypes and select any groups that those occtypes belong to.
        /// </summary>
        private void SelectRequiredOcctypeGroups(Dictionary<string, OcctypeReference> occtypeMappings)
        {
            HashSet<int> uniqueGroupIDs = new HashSet<int>();
            foreach(KeyValuePair<string, OcctypeReference> kvp in occtypeMappings)
            {
                OcctypeReference otRef = kvp.Value;
                uniqueGroupIDs.Add(otRef.GroupID);
            }

            foreach (int i in uniqueGroupIDs)
            {
                bool groupSelected = SelectGroupWithID(i);
                if(!groupSelected)
                {
                    MessageBox.Show("Saved occtypes belonged to a group that no longer exists.", "Failed To Find Occupancy Type Group", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

        }
        private bool SelectGroupWithID(int id)
        {
            bool foundGroup = false;
            foreach (OcctypeGroupRowItem groupRow in OccTypeGroups)
            {
                if(id == groupRow.GroupElement.ID)
                {
                    groupRow.IsSelected = true;
                    foundGroup = true;
                    break;
                }
            }
            return foundGroup;
        }

        private void LoadOcctypeGroups()
        {
            List<OccupancyTypesElement> occupancyTypesElements = StudyCache.GetChildElementsOfType<OccupancyTypesElement>();

            foreach (OccupancyTypesElement elem in occupancyTypesElements)
            {
                OcctypeGroupRowItem row = new OcctypeGroupRowItem(elem);
                row.OcctypeGroupSelectionChanged += OcctypeGroupSelectionChanged;
                OccTypeGroups.Add(row);
            } 
        }

        /// <summary>
        /// Add a row item for each unique occtype in the structure inventory.
        /// </summary>
        private void LoadRows()
        {
            Rows.Clear();
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
                OcctypeReference otRef = new OcctypeReference(row.SelectedOccType.GroupID, row.SelectedOccType.OccType.ID);
                dict.Add(row.OccTypeName, otRef);
            }
            return dict;
        }

        /// <summary>
        /// Reads the dbf file and loops over all the structures and creates a list of unique occtypes.
        /// This is reading the column in the dbf file that corresponds to the user selected occtype header.
        /// </summary>
        /// <returns></returns>
        public List<string> GetUniqueOccupancyTypes()
        {
            HashSet<string> uniqueList = new();
            PointFeatureCollection collection;
            OperationResult res = ShapefileWriter.TryReadShapefile(_Path, out collection);
            if (res.Result)
            {
                TableColumn occTypeColumn = collection.AttributeTable.GetColumn(_SelectedOcctypeName);
                for(int i = 0; i < collection.AttributeTable.Rows.Count; i++)
                {
                    var row = collection.AttributeTable.Rows[i];
                    string occtype = row.ValueAs(_SelectedOcctypeName, string.Empty);
                    if (occTypeColumn != null)
                    {
                        uniqueList.Add(occtype);
                    }
                }
            }
            return uniqueList.ToList();
        }

        public void UpdateOcctypeColumnSelectionName(string occtypeColName)
        {
            if(!_SelectedOcctypeName.Equals(occtypeColName))
            {
                _SelectedOcctypeName = occtypeColName;
                OccupancyTypesInFile = GetUniqueOccupancyTypes();
                LoadRows();
                UpdateRowsWithSelectedGroups();
            }
        }

        #endregion
    }
}
