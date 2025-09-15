using HEC.CS.Collections;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HEC.FDA.ViewModel.Watershed;
using HEC.FDA.Model.structures;
using Geospatial.GDALAssist;
using HEC.FDA.Model.Spatial;
using Geospatial.Features;
using System.Windows.Documents;
using Geospatial.IO;
using Utility.Logging;

namespace HEC.FDA.ViewModel.Inventory
{
    public class InventoryColumnSelectionsVM : BaseViewModel
    {
        private string _Path;
        private bool _FirstFloorElevationIsSelected = true;
        private bool _FromTerrainFileIsSelected;
        private readonly List<float> _StructureElevations = new List<float>();

        //required rows
        private InventoryColumnSelectionsRowItem _StructureIDRow = new InventoryColumnSelectionsRowItem(StructureSelectionMapping.STRUCTURE_ID, "Structure ID");
        private InventoryColumnSelectionsRowItem _OccupancyTypeRow = new InventoryColumnSelectionsRowItem(StructureSelectionMapping.OCCUPANCY_TYPE, "Occupancy Type");
        private InventoryColumnSelectionsRowItem _FirstFloorElevRow = new InventoryColumnSelectionsRowItem(StructureSelectionMapping.FIRST_FLOOR_ELEV, "First Floor Elevation Value");
        private InventoryColumnSelectionsRowItem _StructureValueRow = new InventoryColumnSelectionsRowItem(StructureSelectionMapping.STRUCTURE_VALUE, "Structure Value");
        private InventoryColumnSelectionsRowItem _FoundationHeightRow = new InventoryColumnSelectionsRowItem(StructureSelectionMapping.FOUNDATION_HEIGHT, "Foundation Height");
        private InventoryColumnSelectionsRowItem _GroundElevRow = new InventoryColumnSelectionsRowItem(StructureSelectionMapping.GROUND_ELEV, "Ground Elevation Value");

        //optional rows
        private InventoryColumnSelectionsRowItem _ContentValueRow = new InventoryColumnSelectionsRowItem(StructureSelectionMapping.CONTENT_VALUE, "Content Value");
        private InventoryColumnSelectionsRowItem _OtherValueRow = new InventoryColumnSelectionsRowItem(StructureSelectionMapping.OTHER_VALUE, "Other Value");
        private InventoryColumnSelectionsRowItem _VehicleValueRow = new InventoryColumnSelectionsRowItem(StructureSelectionMapping.VEHICLE_VALUE, "Vehicle Value");
        private InventoryColumnSelectionsRowItem _BegDamDepthRow = new InventoryColumnSelectionsRowItem(StructureSelectionMapping.BEG_DAMAGE_DEPTH, "Beginning Damage Depth");
        private InventoryColumnSelectionsRowItem _YearInConstructionRow = new InventoryColumnSelectionsRowItem(StructureSelectionMapping.YEAR_IN_CONSTRUCTION, "Year In Construction");
        private InventoryColumnSelectionsRowItem _NotesRow = new InventoryColumnSelectionsRowItem(StructureSelectionMapping.NOTES, "Notes");
        private InventoryColumnSelectionsRowItem _DescriptionRow = new InventoryColumnSelectionsRowItem(StructureSelectionMapping.DESCRIPTION, "Description");
        private InventoryColumnSelectionsRowItem _NumberOfStructuresRow = new InventoryColumnSelectionsRowItem(StructureSelectionMapping.NUMBER_OF_STRUCTURES, "Number Of Structures");

        public InventoryColumnSelectionsRowItem OccupancyTypeRow
        {
            get { return _OccupancyTypeRow; }
        }
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

        public CustomObservableCollection<InventoryColumnSelectionsRowItem> RequiredRows { get; } = new CustomObservableCollection<InventoryColumnSelectionsRowItem>();
        public CustomObservableCollection<InventoryColumnSelectionsRowItem> OptionalRows { get; } = new CustomObservableCollection<InventoryColumnSelectionsRowItem>();

        public List<InventoryColumnSelectionsRowItem> FirstFloorElevationRows { get; } = new List<InventoryColumnSelectionsRowItem>();
        public List<InventoryColumnSelectionsRowItem> GroundElevationRows { get; } = new List<InventoryColumnSelectionsRowItem>();
        public List<InventoryColumnSelectionsRowItem> TerrainElevationRows { get; } = new List<InventoryColumnSelectionsRowItem>();

        public PointFeatureCollection PointShapefile { get; set; }

        public InventoryColumnSelectionsVM()
        {
            LoadRows();
            RequiredRows.AddRange(FirstFloorElevationRows);
            FirstFloorElevationIsSelected = true;
        }

        public InventoryColumnSelectionsVM(StructureSelectionMapping mappings, string inventoryShpPath)
        {
            Path = inventoryShpPath;
            LoadRows();
            RequiredRows.AddRange(FirstFloorElevationRows);

            FromTerrainFileIsSelected = mappings.IsUsingTerrainFile;
            FirstFloorElevationIsSelected = mappings.IsUsingFirstFloorElevation;

            _StructureIDRow.SelectedItem = mappings.StructureIDCol;
            _OccupancyTypeRow.SelectedItem = mappings.OccTypeCol;
            _FirstFloorElevRow.SelectedItem = mappings.FirstFloorElevCol;
            _StructureValueRow.SelectedItem = mappings.StructureValueCol;
            _FoundationHeightRow.SelectedItem = mappings.FoundationHeightCol;
            _GroundElevRow.SelectedItem = mappings.GroundElevCol;

            _ContentValueRow.SelectedItem = mappings.ContentValueCol;
            _OtherValueRow.SelectedItem = mappings.OtherValueCol;
            _VehicleValueRow.SelectedItem = mappings.VehicleValueCol;
            _BegDamDepthRow.SelectedItem = mappings.BeginningDamageDepthCol;
            _YearInConstructionRow.SelectedItem = mappings.YearInConstructionCol;
            _NotesRow.SelectedItem = mappings.NotesCol;
            //todo:

            _NumberOfStructuresRow.SelectedItem = mappings.NumberOfStructuresCol;
        }

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
            if (_FirstFloorElevationIsSelected)
            {
                RequiredRows.AddRange(FirstFloorElevationRows);
            }
            else
            {
                FromTerrainFileSelectionChanged();
            }
        }

        /// <summary>
        /// These are the rows that correspond to the different radio button options.
        /// </summary>
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
            OptionalRows.Add(_BegDamDepthRow);
            OptionalRows.Add(_YearInConstructionRow);
            OptionalRows.Add(_NotesRow);
            OptionalRows.Add(_DescriptionRow);
            OptionalRows.Add(_NumberOfStructuresRow);
        }

        private void PathChanged()
        {
            OperationResult res = ShapefileIO.TryRead(Path, out PointFeatureCollection points);
            if (!res)
            {
                throw new Exception(res.GetConcatenatedMessages()); 
            }
            PointShapefile = points;
            UpdateRows();
        }

        private void UpdateRows()
        {
            string[] allColumnNames = GetColumnNames();
            string[] optionalColumnNames = GetColumnNamesWithClearOption();

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

            //optional rows - use the list with clear option
            _ContentValueRow.Items.Clear();
            _ContentValueRow.Items.AddRange(optionalColumnNames);
            _OtherValueRow.Items.Clear();
            _OtherValueRow.Items.AddRange(optionalColumnNames);
            _VehicleValueRow.Items.Clear();
            _VehicleValueRow.Items.AddRange(optionalColumnNames);
            _BegDamDepthRow.Items.Clear();
            _BegDamDepthRow.Items.AddRange(optionalColumnNames);
            _YearInConstructionRow.Items.Clear();
            _YearInConstructionRow.Items.AddRange(optionalColumnNames);
            _NotesRow.Items.Clear();
            _NotesRow.Items.AddRange(optionalColumnNames);
            _DescriptionRow.Items.Clear();
            _DescriptionRow.Items.AddRange(optionalColumnNames);
            _NumberOfStructuresRow.Items.Clear();
            _NumberOfStructuresRow.Items.AddRange(optionalColumnNames);
        }

        private string[] GetColumnNamesWithClearOption()
        {
            var columnNames = GetColumnNames().ToList();
            columnNames.Insert(0, ""); // Add empty string as first option for clearing
            return columnNames.ToArray();
        }

        private string[] GetColumnNames()
        {
            return PointShapefile.AttributeTable.Columns.Select((c) => c.Name).ToArray();
        }

        private object[] GetStructureNames()
        {
            string selectedColumn = _StructureIDRow.SelectedItem;
            return PointShapefile.AttributeTable.Rows
                .Select(row => StructureFactory.GetFID(selectedColumn, row))
                .ToArray();
        }

        public static string getTerrainFile()
        {
            string filePath = "";
            List<TerrainElement> terrainElements = StudyCache.GetChildElementsOfType<TerrainElement>();
            if (terrainElements.Count > 0)
            {
                //there can only be one terrain in the study
                TerrainElement elem = terrainElements[0];
                filePath = Storage.Connection.Instance.TerrainDirectory + "\\" + elem.Name + "\\" + elem.FileName;
            }
            return filePath;
        }

        /// <summary>
        /// Uses the terrain file and the structures shapefile to get elevations for each structure.
        /// It returns a list of the structures that are missing elevations.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private List<StructureMissingDataRowItem> GetMissingTerrainElevations()
        {
            List<StructureMissingDataRowItem> missingDataRows = new();
            int badElevationNumber = -9999;
            _StructureElevations.Clear();
            Projection projection;
            try
            {
                projection = RASHelper.GetProjectionFromTerrain(getTerrainFile());
            }
            catch (Exception)
            {
                throw;
            }
            _StructureElevations.AddRange(RASHelper.SamplePointsFromTerrain(Path, getTerrainFile(), projection));
            List<int> idsWithNoElevation = new List<int>();
            for (int i = 0; i < _StructureElevations.Count(); i++)
            {
                if (_StructureElevations[i] == badElevationNumber)
                {
                    idsWithNoElevation.Add(i);
                }
            }
            object[] structureNames = GetStructureNames();
            //get list of structure names that don't have elevs
            foreach (int i in idsWithNoElevation)
            {
                string uniqueName = structureNames[i].ToString();
                StructureMissingDataRowItem missingRow = new (uniqueName, GetRequiredRowValues(i), MissingDataType.TerrainElevation);
                missingDataRows.Add(missingRow);
            }
            return missingDataRows;
        }

        /// <summary>
        /// Validates that the user has made a selection for all the required attributes.
        /// It also validates that there is a structure id for each structure.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public FdaValidationResult ValidateSelectionsMade()
        {
            FdaValidationResult vr = ValidateSIAttributes();
            if (vr.IsValid)
            {
                List<StructureMissingDataRowItem> missingDataRows = AreAllStructureValuesDefinedForRow(_StructureIDRow, MissingDataType.ID);
                if (missingDataRows.Count > 0)
                {
                    vr.AddErrorMessage("There are missing values in the selected structure id column.");
                }
                else
                {
                    FdaValidationResult validationResult = AreStructureIdsUnique();
                    if (!validationResult.IsValid)
                    {
                        vr.AddErrorMessage(validationResult.ErrorMessage);
                    }
                }
            }
            return vr;
        }

        private FdaValidationResult AreStructureIdsUnique()
        {
            FdaValidationResult vr = new();
            if(!StructureDataValidator.AllRowsHaveUniqueValueForColumn<string>(PointShapefile, _StructureIDRow.SelectedItem, out _))
            {
                vr.AddErrorMessage("Duplicate structure ID's were found. This is not allowed.");
            }
            return vr;
        }

        public new StructuresMissingDataManager Validate()
        {
            StructuresMissingDataManager missingDataManager = new StructuresMissingDataManager();

            if (!FirstFloorElevationIsSelected)
            {
                missingDataManager.AddStructuresWithMissingData(AreAllStructureValuesDefinedForRow(_FoundationHeightRow, MissingDataType.FoundationHt));
                if (FromTerrainFileIsSelected)
                {
                    List<StructureMissingDataRowItem> missingTerrainElevRows = GetMissingTerrainElevations();
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
        private List<StructureMissingDataRowItem> AreAllStructureValuesDefinedForRow(InventoryColumnSelectionsRowItem row, MissingDataType missingType)
        {
            List<StructureMissingDataRowItem> missingDataRows = new();
            StructureDataValidator.RowsHaveValueForColumn(PointShapefile, row.SelectedItem, out List<int> rowsWithMissingData);
            string[] structureNames = GetStructureNames().Select((name) => name.ToString()).ToArray();
            foreach(int i in rowsWithMissingData)
            {
                object[] rowValues = RowToObjects(PointShapefile.AttributeTable.Rows[i]);
                missingDataRows.Add(new(structureNames[i], rowValues, missingType));
            }
            return missingDataRows;
        }
        /// <summary>
        /// Transforms a TableRow into an object array. Would like to redesign this whole interface to not need this. 
        /// </summary>
        private static object[] RowToObjects(Utility.Memory.TableRow row) => Enumerable.Range(0, row.Table.Columns.Count).Select(i => row[i]).ToArray();


        /// <summary>
        /// Validates that a selection has been made for each required attribute.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private FdaValidationResult ValidateSIAttributes()
        {
            FdaValidationResult vr = new FdaValidationResult();
            //these are the shared required rows
            if (!_StructureIDRow.IsValid())
            {
                vr.AddErrorMessage("A structure ID selection is required.");
            }
            if (!_OccupancyTypeRow.IsValid())
            {
                vr.AddErrorMessage("An occupancy type selection is required.");
            }

            if (FirstFloorElevationIsSelected)
            {
                //first floor elevation
                if (!_FirstFloorElevRow.IsValid())
                {
                    vr.AddErrorMessage("A first floor elevation selection is required.");
                }
            }
            else
            {
                //found height
                if (!_FoundationHeightRow.IsValid())
                {
                    vr.AddErrorMessage("A foundation height selection is required.");
                }
                if (!FromTerrainFileIsSelected)
                {
                    if (!_GroundElevRow.IsValid())
                    {
                        vr.AddErrorMessage("A ground elevation selection is required.");
                    }
                }
            }

            if (!_StructureValueRow.IsValid())
            {
                vr.AddErrorMessage("A structure value selection is required.");
            }

            return vr;
        }

        public StructureSelectionMapping CreateSelectionMapping()
        {
            return new StructureSelectionMapping(FirstFloorElevationIsSelected, FromTerrainFileIsSelected,
                _StructureIDRow.SelectedItem, _OccupancyTypeRow.SelectedItem, _FirstFloorElevRow.SelectedItem,
                _StructureValueRow.SelectedItem, _FoundationHeightRow.SelectedItem, _GroundElevRow.SelectedItem,
                _ContentValueRow.SelectedItem, _OtherValueRow.SelectedItem, _VehicleValueRow.SelectedItem,
                _BegDamDepthRow.SelectedItem, _YearInConstructionRow.SelectedItem, _NotesRow.SelectedItem,
                _DescriptionRow.SelectedItem, _NumberOfStructuresRow.SelectedItem);
        }

        /// <summary>
        /// Returns an array of objects that map to the required columns for the row at the given index. 
        /// </summary>
        public object[] GetRequiredRowValues(int index)
        {
            //Get a list of the columns we're actually going to pull data from
            string[] requiredColumns = RequiredRows.Select((c) => c.SelectedItem).ToArray();

            //Grab the row we're interested in
            var row = PointShapefile.AttributeTable.Rows[index];

            return requiredColumns.Select((columnName) => row.Value(columnName)).ToArray();
        }
        }
}
