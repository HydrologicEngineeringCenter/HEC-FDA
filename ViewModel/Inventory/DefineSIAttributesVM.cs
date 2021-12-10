using DatabaseManager;
using HEC.CS.Collections;
using LifeSimGIS;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using ViewModel.Saving.PersistenceManagers;

namespace ViewModel.Inventory
{
    public class DefineSIAttributesVM : BaseViewModel
    {
        #region Fields
        private string _Path;
        private bool _FirstFloorElevationIsSelected = true;
        private bool _FromTerrainFileIsSelected;

        //required rows
        private DefineSIAttributesRowItem _StructureIDRow = new DefineSIAttributesRowItem("Structure ID:");
        private DefineSIAttributesRowItem _OccupancyTypeRow = new DefineSIAttributesRowItem("Occupancy Type:");
        private DefineSIAttributesRowItem _FirstFloorElevRow = new DefineSIAttributesRowItem("First Floor Elevation Value:");
        private DefineSIAttributesRowItem _StructureValueRow = new DefineSIAttributesRowItem("Structure Value:");
        private DefineSIAttributesRowItem _FoundationHeightRow = new DefineSIAttributesRowItem("Foundation Height:");
        private DefineSIAttributesRowItem _GroundElevRow = new DefineSIAttributesRowItem("Ground Elevation Value:");

        //optional rows
        private DefineSIAttributesRowItem _ContentValueRow = new DefineSIAttributesRowItem("Content Value:");
        private DefineSIAttributesRowItem _OtherValueRow = new DefineSIAttributesRowItem("Other Value:");
        private DefineSIAttributesRowItem _VehicleValueRow = new DefineSIAttributesRowItem("Vehicle Value:");
        private DefineSIAttributesRowItem _YearRow = new DefineSIAttributesRowItem("Year:");
        private DefineSIAttributesRowItem _ModuleRow = new DefineSIAttributesRowItem("Module:");
        private DefineSIAttributesRowItem _BegDamDepthRow = new DefineSIAttributesRowItem("Beginning Damage Depth:");
        private DefineSIAttributesRowItem _YearInConstructionRow = new DefineSIAttributesRowItem("Year In Construction:");
        private DefineSIAttributesRowItem _NotesRow = new DefineSIAttributesRowItem("Notes/Metadata:");
        private DefineSIAttributesRowItem _OtherRow = new DefineSIAttributesRowItem("Other:");
        #endregion
        #region Properties
        public string Path
        {
            get { return _Path; }
            set { _Path = value; PathChanged(); }
        }
        public bool FirstFloorElevationIsSelected
        {
            get { return _FirstFloorElevationIsSelected; }
            set { _FirstFloorElevationIsSelected = value; ElevationRadioChanged(); NotifyPropertyChanged(); }
        }

        public bool FromTerrainFileIsSelected
        {
            get { return _FromTerrainFileIsSelected; }
            set { _FromTerrainFileIsSelected = value; FromTerrainFileSelectionChanged(); }
        }

        public CustomObservableCollection<DefineSIAttributesRowItem> RequiredRows { get; } = new CustomObservableCollection<DefineSIAttributesRowItem>();
        public CustomObservableCollection<DefineSIAttributesRowItem> OptionalRows { get; } = new CustomObservableCollection<DefineSIAttributesRowItem>();

        public List<DefineSIAttributesRowItem> FirstFloorElevationRows { get; } = new List<DefineSIAttributesRowItem>();
        public List<DefineSIAttributesRowItem> GroundElevationRows { get; } = new List<DefineSIAttributesRowItem>();
        public List<DefineSIAttributesRowItem> TerrainElevationRows { get; } = new List<DefineSIAttributesRowItem>();

        #endregion
        #region Constructors
        public DefineSIAttributesVM() : base()
        {
            LoadRows();
            RequiredRows.AddRange(FirstFloorElevationRows);
            FirstFloorElevationIsSelected = true;
        }

        #endregion
        #region Voids
        private void FromTerrainFileSelectionChanged()
        {
            RequiredRows.Clear();
            if (FromTerrainFileIsSelected)
            {
                RequiredRows.AddRange(TerrainElevationRows);
            }
            else
            {
                RequiredRows.AddRange(GroundElevationRows);
            }
        }
        private void ElevationRadioChanged()
        {
            RequiredRows.Clear();
            if(_FirstFloorElevationIsSelected)
            {
                RequiredRows.AddRange(FirstFloorElevationRows);
            }
            else
            {
                FromTerrainFileSelectionChanged();
            }
        }

        private void LoadRows()
        {
            FirstFloorElevationRows.Add(_StructureIDRow);
            FirstFloorElevationRows.Add(_OccupancyTypeRow);
            FirstFloorElevationRows.Add(_FirstFloorElevRow);
            FirstFloorElevationRows.Add(_StructureValueRow);

            GroundElevationRows.Add(_StructureIDRow);
            GroundElevationRows.Add(_OccupancyTypeRow);
            GroundElevationRows.Add(_FoundationHeightRow);
            GroundElevationRows.Add(_GroundElevRow);
            GroundElevationRows.Add(_StructureValueRow);

            TerrainElevationRows.Add(_StructureIDRow);
            TerrainElevationRows.Add(_OccupancyTypeRow);
            TerrainElevationRows.Add(_FoundationHeightRow);
            TerrainElevationRows.Add(_StructureValueRow);

            OptionalRows.Add(_ContentValueRow);
            OptionalRows.Add(_OtherValueRow);
            OptionalRows.Add(_VehicleValueRow);
            OptionalRows.Add(_YearRow);
            OptionalRows.Add(_ModuleRow);
            OptionalRows.Add(_BegDamDepthRow);
            OptionalRows.Add(_YearInConstructionRow);
            OptionalRows.Add(_NotesRow);
            OptionalRows.Add(_OtherRow);
        }

        private void PathChanged()
        {
            UpdateRows();
        }

        private void UpdateRows()
        {
            List<string> allColumnNames = GetColumnNames();      

            //required rows
            _StructureIDRow.Items.Clear();
            _StructureIDRow.Items.AddRange(allColumnNames);
            _OccupancyTypeRow.Items.Clear();
            _OccupancyTypeRow.Items.AddRange(allColumnNames);
            _FirstFloorElevRow.Items.Clear();
            _FirstFloorElevRow.Items.AddRange(allColumnNames);
            _StructureValueRow.Items.Clear();
            _StructureValueRow.Items.AddRange(allColumnNames);
            _FoundationHeightRow.Items.Clear();
            _FoundationHeightRow.Items.AddRange(allColumnNames);
            _GroundElevRow.Items.Clear();
            _GroundElevRow.Items.AddRange(allColumnNames);

            //optional rows
            _ContentValueRow.Items.Clear();
            _ContentValueRow.Items.AddRange(allColumnNames);
            _OtherValueRow.Items.Clear();
            _OtherValueRow.Items.AddRange(allColumnNames);
            _VehicleValueRow.Items.Clear();
            _VehicleValueRow.Items.AddRange(allColumnNames);
            _YearRow.Items.Clear();
            _YearRow.Items.AddRange(allColumnNames);
            _ModuleRow.Items.Clear();
            _ModuleRow.Items.AddRange(allColumnNames);
        }

        private List<string> GetColumnNames()
        {
            DbfReader dbf = new DbfReader(System.IO.Path.ChangeExtension(_Path, ".dbf"));
            DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);
            return dtv.ColumnNames.ToList();
        }

        #endregion
        #region Functions
        /// <summary>
        /// Reads the dbf file and loops over all the structures and creates a list of unique occtypes.
        /// This is reading the column in the dbf file that corresponds to the user selected occtype header.
        /// </summary>
        /// <returns></returns>
        public List<string> GetUniqueOccupancyTypes()
        {
            List<string> uniqueList = new List<string>();
            DataTableView dtv = GetStructureInventoryTable();
            if (dtv != null)
            {
                object[] occtypesFromFile = dtv.GetColumn(_OccupancyTypeRow.SelectedItem);
                foreach (object o in occtypesFromFile)
                {
                    uniqueList.Add((string)o);
                }
            }
            return uniqueList.Distinct().ToList();
        }

        private object[] GetStructureNames()
        {
            object[] structureNames = null;
            DataTableView dtv = GetStructureInventoryTable();
            if(dtv != null)
            {
                structureNames = dtv.GetColumn(_StructureIDRow.SelectedItem);
            }
            return structureNames;
        }

        private DataTableView GetStructureInventoryTable()
        {
            DataTableView dtv = null;
            string dbfPath = System.IO.Path.ChangeExtension(_Path, "dbf");
            if (File.Exists(dbfPath))
            {
                DbfReader dbf = new DbfReader(dbfPath);
                dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);
            }
            return dtv;
        }

        /// <summary>
        /// Uses the terrain file and the structures shapefile to get elevations for each structure.
        /// It returns a list of the structures that are missing elevations.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private List<StructureMissingDataRowItem> GetMissingTerrainElevations(ref string errorMessage)
        {
            List<StructureMissingDataRowItem> missingDataRows = new List<StructureMissingDataRowItem>();
            int badElevationNumber = -9999;

            StructureElevationsFromTerrainFile elevsFromTerrainHelper = new StructureElevationsFromTerrainFile(_Path);
            float[] elevs = elevsFromTerrainHelper.GetStructureElevationsFromTerrainFile(ref errorMessage);
            if (errorMessage != null && errorMessage.Length > 0)
            {
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK);
            }
            if (elevs != null)
            {
                List<int> idsWithNoElevation = new List<int>();
                for (int i = 0; i < elevs.Count(); i++)
                {
                    if (elevs[i] == badElevationNumber)
                    {
                        idsWithNoElevation.Add(i);
                    }
                }
                object[] structureNames = GetStructureNames();
                //get list of structure names that don't have elevs
                foreach (int i in idsWithNoElevation)
                {
                    string uniqueName = structureNames[i].ToString();
                    StructureMissingDataRowItem missingRow = new StructureMissingDataRowItem(uniqueName, MissingDataType.TerrainElevation);
                    missingDataRows.Add(missingRow);
                }
            }
            return missingDataRows;
        }

        /// <summary>
        /// Validates that the user has made a selection for all the required attributes.
        /// It also validates that there is a structure id for each structure.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool ValidateSelectionsMade(ref string errorMessage)
        {
            bool isValid = true;
            isValid = ValidateSIAttributes(ref errorMessage);
            if (isValid)
            {
                List<StructureMissingDataRowItem> missingDataRows = AreAllStructureValuesDefinedForRow(_StructureIDRow, MissingDataType.ID);
                if (missingDataRows.Count > 0)
                {
                    isValid = false;
                    errorMessage = "There are missing values in the selected structure id column.";
                }
            }
            return isValid;
        }

        public StructuresMissingDataManager Validate(ref string errorMessage)
        {
            StructuresMissingDataManager missingDataManager = new StructuresMissingDataManager();

            if (!FirstFloorElevationIsSelected)
            {
                missingDataManager.AddStructuresWithMissingData(AreAllStructureValuesDefinedForRow(_FoundationHeightRow, MissingDataType.FoundationHt));
                if (FromTerrainFileIsSelected)
                {
                    List<StructureMissingDataRowItem> missingTerrainElevRows = GetMissingTerrainElevations(ref errorMessage);
                    missingDataManager.AddStructuresWithMissingData(missingTerrainElevRows);
                }
                else
                {
                    //check foundation height and ground elevation
                    missingDataManager.AddStructuresWithMissingData(AreAllStructureValuesDefinedForRow(_GroundElevRow, MissingDataType.GroundElevation));
                }
            }
            else
            {
                missingDataManager.AddStructuresWithMissingData(AreAllStructureValuesDefinedForRow(_FirstFloorElevRow, MissingDataType.FirstFloorElevation));
            }

            //check occupancy type
            missingDataManager.AddStructuresWithMissingData(AreAllStructureValuesDefinedForRow(_OccupancyTypeRow, MissingDataType.Occtype));

            //check structure value
            missingDataManager.AddStructuresWithMissingData(AreAllStructureValuesDefinedForRow(_StructureValueRow, MissingDataType.StructureValue));

            return missingDataManager;
        }

        /// <summary>
        /// Loops over all the rows in the dbf file for a specific column and looks for any missing values.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="missingType"></param>
        /// <returns></returns>
        private List<StructureMissingDataRowItem> AreAllStructureValuesDefinedForRow(DefineSIAttributesRowItem row, MissingDataType missingType)
        {
            List<StructureMissingDataRowItem> missingDataRows = new List<StructureMissingDataRowItem>();

            if (File.Exists(System.IO.Path.ChangeExtension(_Path, "dbf")))
            {
                DbfReader dbf = new DbfReader(System.IO.Path.ChangeExtension(Path, ".dbf"));
                DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);

                object[] rows = dtv.GetColumn(row.SelectedItem);
                for (int i = 0; i < rows.Length; i++)
                {
                    if (rows[i] == DBNull.Value || rows[i].ToString() == "")
                    {
                        string structId = dtv.GetCell(_StructureIDRow.SelectedItem, i).ToString();
                        StructureMissingDataRowItem missingDataRow = new StructureMissingDataRowItem(structId, missingType);
                        missingDataRows.Add(missingDataRow);
                    }
                }
            }

            return missingDataRows;
        }

        /// <summary>
        /// Validates that a selection has been made for each required attribute.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private bool ValidateSIAttributes(ref string errorMessage)
        {
            bool isValid = true;

            //these are the shared required rows
            if(!_StructureIDRow.IsValid())
            {
                isValid = false;
                errorMessage = "A structure id selection is required.";
            }
            else if(!_OccupancyTypeRow.IsValid())
            {
                isValid = false;
                errorMessage = "An occupancy type selection is required.";
            }
            else if (!_StructureValueRow.IsValid())
            {
                isValid = false;
                errorMessage = "A structure value selection is required.";
            }

            if (FirstFloorElevationIsSelected)
            {
                //first floor elevation
                if (!_FirstFloorElevRow.IsValid())
                {
                    isValid = false;
                    errorMessage = "A first floor elevation selection is required.";
                }
            }
            else
            {
                //found height
                if (!_FoundationHeightRow.IsValid())
                {
                    isValid = false;
                    errorMessage = "A foundation height selection is required.";
                }
                if(!FromTerrainFileIsSelected)
                {
                    if (!_GroundElevRow.IsValid())
                    {
                        isValid = false;
                        errorMessage = "A ground elevation selection is required.";
                    }
                }
            }

            return isValid;
        }

        /// <summary>
        /// Creates the data table that will be saved to the database.
        /// </summary>
        /// <param name="shapefilePath"></param>
        /// <param name="occtypeSelectionRows"></param>
        /// <returns></returns>
        public DataTable CreateStructureTable(string shapefilePath, CustomObservableCollection<OccTypeSelectionRowItem> occtypeSelectionRows)
        {
            StructureInventoryPersistenceManager manager = Saving.PersistenceFactory.GetStructureInventoryManager();
            DataTable table = manager.CreateEmptyStructuresTable();

            ShapefileReader myReader = new ShapefileReader(shapefilePath);
            DataTableView attributeTableFromFile = myReader.GetAttributeTable();

            //todo: what is this? is this necessary? 
            if (attributeTableFromFile.ParentDatabase.DataBaseOpen == false)
            {
                attributeTableFromFile.ParentDatabase.Open();
            }

            //loop over all structures and grab the values that we want to store in our database from the 
            //structure inventory table.
            for (int i = 0; i < attributeTableFromFile.NumberOfRows; i++)
            {
                DataRow row = table.NewRow();
                string structureOcctypeName = GetValueForRow(attributeTableFromFile, i, _OccupancyTypeRow);
                OccTypeDisplayName occTypeDisplayName = GetOccTypeDisplayObject(structureOcctypeName, occtypeSelectionRows);

                AssignValuesToRow(row, attributeTableFromFile, i, occTypeDisplayName);
                table.Rows.Add(row);
            }

            return table;
        }

        /// <summary>
        /// This grabs the "existing" occtype object that the user has assigned to the "structure" occtype.
        /// </summary>
        /// <param name="occTypeName"></param>
        /// <param name="occtypeSelectionRows"></param>
        /// <returns></returns>
        private OccTypeDisplayName GetOccTypeDisplayObject(string occTypeName, CustomObservableCollection<OccTypeSelectionRowItem> occtypeSelectionRows)
        {
            OccTypeDisplayName selectedOccTypeObject = null;
            OccTypeSelectionRowItem rowForThisOcctype = occtypeSelectionRows.Where(row => row.OccTypeName.Equals(occTypeName)).FirstOrDefault();
            if(rowForThisOcctype != null)
            {
                selectedOccTypeObject = rowForThisOcctype.SelectedOccType;
            }
            return selectedOccTypeObject;
        }

        private void AssignValuesToRow(DataRow row,  DataTableView dataTableView, int i, OccTypeDisplayName selectedOcctype)
        {
            //id
            row[StructureInventoryPersistenceManager.STRUCTURE_ID] = GetValueForRow(dataTableView, i, _StructureIDRow);

            //occtypes and damcats
            row[StructureInventoryBaseElement.OccupancyTypeField] = selectedOcctype.OccType.Name;
            row[StructureInventoryBaseElement.OccupancyTypeGroupName] = selectedOcctype.OccType.GroupID;
            row[StructureInventoryBaseElement.damCatField] = selectedOcctype.OccType.DamageCategory.Name;

            //foundation and elevation
            row[StructureInventoryBaseElement.FoundationHeightField] = GetValueForRow(dataTableView, i, _FoundationHeightRow);
            row[StructureInventoryBaseElement.FirstFloorElevationField] = GetValueForRow(dataTableView, i, _FirstFloorElevRow);
            row[StructureInventoryBaseElement.GroundElevationField] = GetValueForRow(dataTableView, i, _GroundElevRow);

            //asset values
            row[StructureInventoryBaseElement.StructureValueField] = GetValueForRow(dataTableView, i, _StructureValueRow);
            row[StructureInventoryBaseElement.ContentValueField] = GetValueForRow(dataTableView, i, _ContentValueRow);
            row[StructureInventoryBaseElement.OtherValueField] = GetValueForRow(dataTableView, i, _OtherValueRow);
            row[StructureInventoryBaseElement.VehicleValueField] = GetValueForRow(dataTableView, i, _VehicleValueRow);

            //optional fields
            row[StructureInventoryBaseElement.YearField] = GetValueForRow(dataTableView, i, _YearRow);
            row[StructureInventoryBaseElement.ModuleField] = GetValueForRow(dataTableView, i, _ModuleRow);
            row[StructureInventoryPersistenceManager.BEG_DAM_DEPTH] = GetValueForRow(dataTableView, i, _BegDamDepthRow);
            row[StructureInventoryPersistenceManager.YEAR_IN_CONSTRUCTION] = GetValueForRow(dataTableView, i, _YearInConstructionRow);
            row[StructureInventoryPersistenceManager.NOTES] = GetValueForRow(dataTableView, i, _NotesRow);
            row[StructureInventoryPersistenceManager.OTHER] = GetValueForRow(dataTableView, i, _OtherRow);
        }

        /// <summary>
        /// This will either use the default value the user defined or will grab the correct value from the attribute table.
        /// </summary>
        /// <param name="attributeTableFromFile"></param>
        /// <param name="i"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private string GetValueForRow(DataTableView attributeTableFromFile, int i, DefineSIAttributesRowItem row)
        {
            string value = null;
            if(row.SelectedItem != null)
            {
                value = attributeTableFromFile.GetCell(row.SelectedItem, i).ToString();
            }
            return value;
        }

        #endregion
    }
}
