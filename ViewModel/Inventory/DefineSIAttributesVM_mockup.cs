using DatabaseManager;
using HEC.CS.Collections;
using LifeSimGIS;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModel.Saving.PersistenceManagers;
using ViewModel.Utilities;
using ViewModel.Watershed;

namespace ViewModel.Inventory
{
    public class DefineSIAttributesVM_mockup : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/23/2017 8:53:46 AM
        #endregion
        #region Fields
        //private string _SelectedPath;
        //private ObservableCollection<string> _AvailablePaths = new ObservableCollection<string>();
        private string _Path;

        private bool _FirstFloorElevationIsChecked = true;


        #endregion
        #region Properties
        //public List<string> StringColumnNames
        //{
        //    get { return _StringColumnNames; }
        //    set { _StringColumnNames = value; NotifyPropertyChanged(); }
        //}
        //public List<string> NumericColumnNames
        //{
        //    get { return _NumericColumnNames; }
        //    set { _NumericColumnNames = value; NotifyPropertyChanged(); }
        //}
        public string Path
        {
            get { return _Path; }
            set { _Path = value; PathChanged(); }
        }
        public bool FirstFloorElevationIsChecked
        {
            get { return _FirstFloorElevationIsChecked; }
            set { _FirstFloorElevationIsChecked = value; ElevationRadioChanged(); NotifyPropertyChanged(); }
        }

        public bool FromTerrainFile { get; set; }


        public CustomObservableCollection<DefineSIAttributesRowItem> RequiredRows { get; } = new CustomObservableCollection<DefineSIAttributesRowItem>();
        public CustomObservableCollection<DefineSIAttributesRowItem> OptionalRows { get; } = new CustomObservableCollection<DefineSIAttributesRowItem>();

        public List<DefineSIAttributesRowItem> FirstFloorElevationRows { get; } = new List<DefineSIAttributesRowItem>();
        public List<DefineSIAttributesRowItem> GroundElevationRows { get; } = new List<DefineSIAttributesRowItem>();


        #endregion
        #region Constructors
        public DefineSIAttributesVM_mockup() : base()
        {
            LoadRows();
            RequiredRows.AddRange(FirstFloorElevationRows);
            //AvailablePaths = availablePointFiles;
        }


        //public DefineSIAttributesVM(List<string> stringColumnNames, List<string> numericColumnNames) : base()
        //{
        //    StringColumnNames = stringColumnNames;
        //    NumericColumnNames = numericColumnNames;
        //}

        #endregion
        #region Voids

        private void ElevationRadioChanged()
        {
                RequiredRows.Clear();
            if(_FirstFloorElevationIsChecked)
            {
                RequiredRows.AddRange(FirstFloorElevationRows);
            }
            else
            {
                RequiredRows.AddRange(GroundElevationRows);
            }

        }

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
            List<string> stringColumnNames = GetStringColumnNames();
            List<string> numericColumnNames = GetNumericColumnNames();
            List<string> allColumnNames = new List<string>(stringColumnNames);         
            allColumnNames.AddRange(numericColumnNames);

            //required rows
            _StructureIDRow.Items.Clear();
            _StructureIDRow.Items.AddRange(allColumnNames);
            _OccupancyTypeRow.Items.Clear();
            _OccupancyTypeRow.Items.AddRange(stringColumnNames);
            _FirstFloorElevRow.Items.Clear();
            _FirstFloorElevRow.Items.AddRange(numericColumnNames);
            _StructureValueRow.Items.Clear();
            _StructureValueRow.Items.AddRange(numericColumnNames);
            _FoundationHeightRow.Items.Clear();
            _FoundationHeightRow.Items.AddRange(numericColumnNames);
            _GroundElevRow.Items.Clear();
            _GroundElevRow.Items.AddRange(numericColumnNames);

            //optional rows
            _ContentValueRow.Items.Clear();
            _ContentValueRow.Items.AddRange(numericColumnNames);
            _OtherValueRow.Items.Clear();
            _OtherValueRow.Items.AddRange(numericColumnNames);
            _VehicleValueRow.Items.Clear();
            _VehicleValueRow.Items.AddRange(numericColumnNames);
            _YearRow.Items.Clear();
            _YearRow.Items.AddRange(numericColumnNames);
            _ModuleRow.Items.Clear();
            _ModuleRow.Items.AddRange(stringColumnNames);
        }

        private List<string> GetNumericColumnNames()
        {
            DbfReader dbf = new DbfReader(System.IO.Path.ChangeExtension(_Path, ".dbf"));
            DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);

            List<string> numericColumnNames = new List<string>();

            for (int i = 0; i < dtv.ColumnNames.Count(); i++)
            {
                if (dtv.ColumnTypes[i] != typeof(string))
                {
                    numericColumnNames.Add(dtv.ColumnNames[i]);
                }
            }
            return numericColumnNames;
        }

        private List<string> GetStringColumnNames()
        {
            DbfReader dbf = new DbfReader(System.IO.Path.ChangeExtension(_Path, ".dbf"));
            DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);

            List<string> stringColumnNames = new List<string>();

            for (int i = 0; i < dtv.ColumnNames.Count(); i++)
            {
                if (dtv.ColumnTypes[i] == typeof(string))
                {
                    stringColumnNames.Add(dtv.ColumnNames[i]);
                }
            }
            return stringColumnNames;
        }



        #endregion
        #region Functions
        public List<string> GetUniqueOccupancyTypes()
        {
            List<string> uniqueList = new List<string>();

            if (_OccupancyTypeRow.UseDefault)
            {
                uniqueList.Add(_OccupancyTypeRow.DefaultValue);
            }
            else
            {
                //todo: one change extension has a period the other doesn't?
                if (File.Exists(System.IO.Path.ChangeExtension(_Path, "dbf")))
                {
                    DbfReader dbf = new DbfReader(System.IO.Path.ChangeExtension(_Path, ".dbf"));
                    DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);

                    object[] occtypesFromFile = dtv.GetColumn(_OccupancyTypeRow.SelectedItem);
                    foreach (object o in occtypesFromFile)
                    {
                        uniqueList.Add((string)o);
                    }
                }
            }
            return uniqueList.Distinct().ToList();
        }

        private object[] GetStructureNames()
        {
            object[] structureNames = null;
            //todo: one change extension has a period the other doesn't?
            if (File.Exists(System.IO.Path.ChangeExtension(_Path, "dbf")))
            {
                DbfReader dbf = new DbfReader(System.IO.Path.ChangeExtension(_Path, ".dbf"));
                DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);

                structureNames = dtv.GetColumn(_StructureIDRow.SelectedValue);
            }
            return structureNames;
        }

        private List<StructureMissingDataRowItem> GetMissingTerrainElevations(ref string errorMessage)
        {
            List<StructureMissingDataRowItem> missingDataRows = new List<StructureMissingDataRowItem>();
            int badElevationNumber = -9999;

            StructureElevationsFromTerrainFile elevsFromTerrainHelper = new StructureElevationsFromTerrainFile();
            float[] elevs = elevsFromTerrainHelper.GetStructureElevationsFromTerrainFile(ref errorMessage);
            if (errorMessage != null && errorMessage.Length > 0)
            {
                //isValid = false;
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
                //StructureMissingElevationEditorVM vm = new StructureMissingElevationEditorVM(missingElevStructNames);
                //DynamicTabVM tab = new DynamicTabVM("Missing Elevations", vm, "missingElevations");
                //Navigate(tab);
            }
            return missingDataRows;

        }

        public bool ValidateSelectionsMade(ref string errorMessage)
        {
            bool isValid = true;
            isValid = ValidateSIAttributes(ref errorMessage); //todo: early exit?
            //todo: if we are still valid then check the id's?
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
            int badElevationNumber = -9999;

            StructuresMissingDataManager missingDataManager = new StructuresMissingDataManager();

            //check structure id? what to do if it isn't all there or unique? Early exit?
            missingDataManager.AddStructuresWithMissingData(AreAllStructureValuesDefinedForRow(_StructureIDRow, MissingDataType.ID));

            if (!FirstFloorElevationIsChecked)
            {
                if (FromTerrainFile)
                {
                    List<StructureMissingDataRowItem> missingTerrainElevRows = GetMissingTerrainElevations(ref errorMessage);
                    missingDataManager.AddStructuresWithMissingData(missingTerrainElevRows);
                }
                else
                {
                    //check foundation height and ground elevation
                    missingDataManager.AddStructuresWithMissingData( AreAllStructureValuesDefinedForRow(_FoundationHeightRow, MissingDataType.FoundationHt));
                    missingDataManager.AddStructuresWithMissingData(AreAllStructureValuesDefinedForRow(_GroundElevRow, MissingDataType.GroundElevation));
                }
            }
            else
            {
                missingDataManager.AddStructuresWithMissingData(AreAllStructureValuesDefinedForRow(_FirstFloorElevRow, MissingDataType.FirstFloorElevation));
            }

            //check occupancy type?
            missingDataManager.AddStructuresWithMissingData(AreAllStructureValuesDefinedForRow(_OccupancyTypeRow, MissingDataType.Occtype));

            //check structure value
            missingDataManager.AddStructuresWithMissingData(AreAllStructureValuesDefinedForRow(_StructureValueRow, MissingDataType.StructureValue));


            

            return missingDataManager;

        }

        private List<StructureMissingDataRowItem> AreAllStructureValuesDefinedForRow(DefineSIAttributesRowItem row, MissingDataType missingType)
        {
            List<StructureMissingDataRowItem> missingDataRows = new List<StructureMissingDataRowItem>();
            if (!row.UseDefault)
            {
                if (File.Exists(System.IO.Path.ChangeExtension(_Path, "dbf")))
                {
                    DbfReader dbf = new DbfReader(System.IO.Path.ChangeExtension(Path, ".dbf"));
                    DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);

                    object[] rows = dtv.GetColumn(row.SelectedValue);
                    for (int i = 0; i < rows.Length; i++)
                    {
                        if (rows[i] == DBNull.Value || rows[i].ToString() == "")
                        {
                            //todo: this will break if this isn't selected (default);
                            string structId = dtv.GetCell(_StructureIDRow.SelectedItem, i).ToString();
                            StructureMissingDataRowItem missingDataRow = new StructureMissingDataRowItem(structId, missingType);
                            missingDataRows.Add(missingDataRow);
                        }
                    }
                }
            }
            return missingDataRows;
        }

        private bool ValidateSIAttributes(ref string errorMessage)
        {
            //here i need to validate all the fields for valid values (no negatives etc)
            //occtype
            bool isValid = true;

            //these are the shared required rows
            if(!_StructureIDRow.IsValid())
            {
                isValid = false;
                errorMessage = "A structure id selection or default value is required.";
            }
            else if(!_OccupancyTypeRow.IsValid())
            {
                isValid = false;
                errorMessage = "An occupancy type selection or default value is required.";
            }
            else if (!_StructureValueRow.IsValid())
            {
                isValid = false;
                errorMessage = "A structure value selection or default value is required.";
            }


            if (FirstFloorElevationIsChecked)
            {
                //first floor elevation
                if (!_FirstFloorElevRow.IsValid())
                {
                    isValid = false;
                    errorMessage = "A first floor elevation selection or default value is required.";
                }
            }
            else
            {
                //found height
                if (!_FoundationHeightRow.IsValid())
                {
                    isValid = false;
                    errorMessage = "A foundation height selection or default value is required.";
                }
                else if (!_GroundElevRow.IsValid())
                {
                    isValid = false;
                    errorMessage = "A ground elevation selection or default value is required.";
                }

            }

            return isValid;
        }


        public DataTable CreateStructureTable(string shapefilePath)
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

            for (int i = 0; i < attributeTableFromFile.NumberOfRows; i++)
            {
                DataRow row = table.NewRow();
                AssignValuesToRow(row, attributeTableFromFile, i);
                table.Rows.Add(row);
            }

            return table;
        }

        private void AssignValuesToRow(DataRow row,  DataTableView dataTableView, int i)
        {
            //todo: group name?

            //id
            row[StructureInventoryPersistenceManager.STRUCTURE_ID] = GetValueForRow(dataTableView, i, _StructureIDRow);

            //occtypes and damcats
            row[StructureInventoryBaseElement.OccupancyTypeField] = GetValueForRow(dataTableView, i, _OccupancyTypeRow);
            row[StructureInventoryBaseElement.OccupancyTypeGroupName] = GetValueForRow(dataTableView, i, _OccupancyTypeRow);
            row[StructureInventoryBaseElement.damCatField] = GetValueForRow(dataTableView, i, _OccupancyTypeRow);

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
            row[StructureInventoryPersistenceManager.BEG_DAM_DEPTH] = GetValueForRow(dataTableView, i, _OccupancyTypeRow);
            row[StructureInventoryPersistenceManager.YEAR_IN_CONSTRUCTION] = GetValueForRow(dataTableView, i, _OccupancyTypeRow);
            row[StructureInventoryPersistenceManager.NOTES] = GetValueForRow(dataTableView, i, _OccupancyTypeRow);
            row[StructureInventoryPersistenceManager.OTHER] = GetValueForRow(dataTableView, i, _OccupancyTypeRow);

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
            string retval = null;
            if (row.UseDefault)
            {
                retval = row.DefaultValue;
            }
            else
            {
                retval = attributeTableFromFile.GetCell(row.SelectedItem, i).ToString();
            }
            return retval;
        }



        #endregion

    }
}
