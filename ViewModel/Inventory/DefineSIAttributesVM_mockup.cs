using DatabaseManager;
using HEC.CS.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                if (File.Exists(System.IO.Path.ChangeExtension(_Path, "dbf")))
                {
                    DbfReader dbf = new DbfReader(System.IO.Path.ChangeExtension(Path, ".dbf"));
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

        public bool Validate(ref string errorMessage)
        {
            AreAllFirstFloorElevationsDefined();
            bool isValid = ValidateSIAttributes(ref errorMessage);

            //are all elev values filled in?

            return isValid;
        }

        private void AreAllFirstFloorElevationsDefined()
        {
            if (File.Exists(System.IO.Path.ChangeExtension(_Path, "dbf")))
            {
                DbfReader dbf = new DbfReader(System.IO.Path.ChangeExtension(Path, ".dbf"));
                DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);

                object[] rows = dtv.GetColumn(_FirstFloorElevRow.SelectedValue);
                foreach (object row in rows)
                {
                    if("".Equals(row.ToString()))
                    { 
                        //blank entry
                        //todo;
                    }
                }
            }
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
                errorMessage = "A Structure ID selection or default value is required.";
            }
            else if(!_OccupancyTypeRow.IsValid())
            {
                isValid = false;
                errorMessage = "An Occupancy Type selection or default value is required.";
            }
            else if (!_OccupancyTypeRow.IsValid())
            {
                isValid = false;
                errorMessage = "An Occupancy Type selection or default value is required.";
            }


            if (FirstFloorElevationIsChecked == true)
            {
                //first floor elevation
                if (!_FirstFloorElevRow.IsValid())
                {
                    isValid = false;
                    errorMessage = "A First Floor Elevation selection or default value is required.";
                }
            }
            else
            {
                //found height
                if (!_FoundationHeightRow.IsValid())
                {
                    isValid = false;
                    errorMessage = "A Foundation Height selection or default value is required.";
                }
                else if (!_GroundElevRow.IsValid())
                {
                    isValid = false;
                    errorMessage = "A Ground Elevation selection or default value is required.";
                }

            }

            return isValid;
        }

        #endregion

    }
}
