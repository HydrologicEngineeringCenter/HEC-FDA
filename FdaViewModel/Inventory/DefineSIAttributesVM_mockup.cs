using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory
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


        private List<string> _StringColumnNames;
        private List<string> _NumericColumnNames;

        private string _OccupancyTypeColumnName;
        private bool _OccupancyTypeIsUsingDefault = false;
        private string _OccupancyTypeDefaultValue;

        private string _FoundationHeightColumnName;
        private bool _FoundationHeightIsUsingDefault = false;
        private string _FoundationHeightDefaultValue;


        private string _GroundElevationColumnName;
        private bool _GroundElevationIsUsingDefault = false;
        private string _GroundElevationDefaultValue;

        private string _FirstFloorElevationColumnName;
        private bool _FirstFloorElevationIsUsingDefault = false;
        private string _FirstFloorElevationDefaultValue;


        private string _StructureValueColumnName;
        private bool _StructureValueIsUsingDefault = false;
        private string _StructureValueDefaultValue;

        private string _ContentValueColumnName;
        private bool _ContentValueIsUsingDefault = false;
        private string _ContentValueDefaultValue;

        private string _OtherValueColumnName;
        private bool _OtherValueIsUsingDefault = false;
        private string _OtherValueDefaultValue;

        private string _VehicleValueColumnName;
        private bool _VehicleValueIsUsingDefault = false;
        private string _VehicleValueDefaultValue;

        private bool _FirstFloorElevationIsChecked = true;
        private bool _GroundElevationIsChecked;

        private string _YearColumnName;
        private bool _YearIsUsingDefault = false;
        private string _YearDefaultValue;

        private string _ModuleColumnName;
        private bool _ModuleIsUsingDefault = false;
        private string _ModuleDefaultValue;
        #endregion
        #region Properties

        //public ObservableCollection<string> AvailablePaths
        //{
        //    get { return _AvailablePaths; }
        //    set { _AvailablePaths = value; NotifyPropertyChanged(); }
        //}
        //public string SelectedPath
        //{
        //    get { return _SelectedPath; }
        //    set { _SelectedPath = value; NotifyPropertyChanged(); }
        //}
        public string Path
        {
            get { return _Path; }
            set { _Path = value; NotifyPropertyChanged(); }
        }
        public string ModuleColumnName
        {
            get { return _ModuleColumnName; }
            set { _ModuleColumnName = value; NotifyPropertyChanged(); }
        }
        public bool ModuleIsUsingDefault
        {
            get { return _ModuleIsUsingDefault; }
            set { _ModuleIsUsingDefault = value; NotifyPropertyChanged(); }
        }
        public string ModuleDefaultValue
        {
            get { return _ModuleDefaultValue; }
            set { _ModuleDefaultValue = value; NotifyPropertyChanged(); }
        }

        public string YearColumnName
        {
            get { return _YearColumnName; }
            set { _YearColumnName = value; NotifyPropertyChanged(); }
        }
        public bool YearIsUsingDefault
        {
            get { return _YearIsUsingDefault; }
            set { _YearIsUsingDefault = value; NotifyPropertyChanged(); }
        }
        public string YearDefaultValue
        {
            get { return _YearDefaultValue; }
            set { _YearDefaultValue = value; NotifyPropertyChanged(); }
        }

        public bool FirstFloorElevationIsChecked
        {
            get { return _FirstFloorElevationIsChecked; }
            set { _FirstFloorElevationIsChecked = value; NotifyPropertyChanged(); }
        }
        public bool GroundElevationIsChecked
        {
            get { return _GroundElevationIsChecked; }
            set { _GroundElevationIsChecked = value; NotifyPropertyChanged(); }
        }
        public string VehicleValueColumnName
        {
            get { return _VehicleValueColumnName; }
            set { _VehicleValueColumnName = value; NotifyPropertyChanged(); }
        }
        public bool VehicleValueIsUsingDefault
        {
            get { return _VehicleValueIsUsingDefault; }
            set { _VehicleValueIsUsingDefault = value; NotifyPropertyChanged(); }
        }
        public string VehicleValueDefaultValue
        {
            get { return _VehicleValueDefaultValue; }
            set { _VehicleValueDefaultValue = value; NotifyPropertyChanged(); }
        }


        public string OtherValueColumnName
        {
            get { return _OtherValueColumnName; }
            set { _OtherValueColumnName = value; NotifyPropertyChanged(); }
        }
        public bool OtherValueIsUsingDefault
        {
            get { return _OtherValueIsUsingDefault; }
            set { _OtherValueIsUsingDefault = value; NotifyPropertyChanged(); }
        }
        public string OtherValueDefaultValue
        {
            get { return _OtherValueDefaultValue; }
            set { _OtherValueDefaultValue = value; NotifyPropertyChanged(); }
        }


        public string ContentValueColumnName
        {
            get { return _ContentValueColumnName; }
            set { _ContentValueColumnName = value; NotifyPropertyChanged(); }
        }
        public bool ContentValueIsUsingDefault
        {
            get { return _ContentValueIsUsingDefault; }
            set { _ContentValueIsUsingDefault = value; NotifyPropertyChanged(); }
        }
        public string ContentValueDefaultValue
        {
            get { return _ContentValueDefaultValue; }
            set { _ContentValueDefaultValue = value; NotifyPropertyChanged(); }
        }


        public string StructureValueDefaultValue
        {
            get { return _StructureValueDefaultValue; }
            set { _StructureValueDefaultValue = value; NotifyPropertyChanged(); }
        }
        public bool StructureValueIsUsingDefault
        {
            get { return _StructureValueIsUsingDefault; }
            set { _StructureValueIsUsingDefault = value; NotifyPropertyChanged(); }
        }
        public string StructureValueColumnName
        {
            get { return _StructureValueColumnName; }
            set { _StructureValueColumnName = value; NotifyPropertyChanged(); }
        }



        public string FirstFloorElevationDefaultValue
        {
            get { return _FirstFloorElevationDefaultValue; }
            set { _FirstFloorElevationDefaultValue = value; NotifyPropertyChanged(); }
        }
        public bool FirstFloorElevationIsUsingDefault
        {
            get { return _FirstFloorElevationIsUsingDefault; }
            set { _FirstFloorElevationIsUsingDefault = value; NotifyPropertyChanged(); }
        }
        public string FirstFloorElevationColumnName
        {
            get { return _FirstFloorElevationColumnName; }
            set { _FirstFloorElevationColumnName = value; NotifyPropertyChanged(); }
        }




        public string GroundElevationDefaultValue
        {
            get { return _GroundElevationDefaultValue; }
            set { _GroundElevationDefaultValue = value; NotifyPropertyChanged(); }
        }
        public bool GroundElevationIsUsingDefault
        {
            get { return _GroundElevationIsUsingDefault; }
            set { _GroundElevationIsUsingDefault = value; NotifyPropertyChanged(); }
        }
        public string GroundElevationColumnName
        {
            get { return _GroundElevationColumnName; }
            set { _GroundElevationColumnName = value; NotifyPropertyChanged(); }
        }



        public string FoundationHeightDefaultValue
        {
            get { return _FoundationHeightDefaultValue; }
            set { _FoundationHeightDefaultValue = value; NotifyPropertyChanged(); }
        }
        public bool FoundationHeightIsUsingDefault
        {
            get { return _FoundationHeightIsUsingDefault; }
            set { _FoundationHeightIsUsingDefault = value; NotifyPropertyChanged(); }
        }

        public string FoundationHeightColumnName
        {
            get { return _FoundationHeightColumnName; }
            set { _FoundationHeightColumnName = value; NotifyPropertyChanged(); }
        }



        public string OccupancyTypeDefaultValue
        {
            get { return _OccupancyTypeDefaultValue; }
            set { _OccupancyTypeDefaultValue = value; NotifyPropertyChanged(); }
        }
        public bool OccupancyTypeIsUsingDefault
        {
            get { return _OccupancyTypeIsUsingDefault; }
            set { _OccupancyTypeIsUsingDefault = value; NotifyPropertyChanged(); }
        }

        public string OccupancyTypeColumnName
        {
            get { return _OccupancyTypeColumnName; }
            set { _OccupancyTypeColumnName = value; NotifyPropertyChanged(); }
        }
        public List<string> StringColumnNames
        {
            get { return _StringColumnNames; }
            set { _StringColumnNames = value; NotifyPropertyChanged(); }
        }
        public List<string> NumericColumnNames
        {
            get { return _NumericColumnNames; }
            set { _NumericColumnNames = value; NotifyPropertyChanged(); }
        }

        #endregion
        #region Constructors
        public DefineSIAttributesVM_mockup(string path) : base()
        {
            Path = path;
            //AvailablePaths = availablePointFiles;
        }
        //public DefineSIAttributesVM(List<string> stringColumnNames, List<string> numericColumnNames) : base()
        //{
        //    StringColumnNames = stringColumnNames;
        //    NumericColumnNames = numericColumnNames;
        //}

        #endregion
        #region Voids
        //public void loadUniqueNames(string path)

        //{

        //    List<string> stringColumnNames = new List<string>();
        //    List<string> numericColumnNames = new List<string>();


        //    //CurrentViewIsEnabled = true;
        //    //SelectedPath = path; //isnt this bound?? yes but it is not working.
        //    DataBase_Reader.DbfReader dbf = new DataBase_Reader.DbfReader(System.IO.Path.ChangeExtension(path, ".dbf"));
        //    DataBase_Reader.DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);


        //    for (int i = 0; i < dtv.ColumnNames.Count(); i++)
        //    {
        //        if (dtv.ColumnTypes[i] == typeof(string))
        //        {
        //            stringColumnNames.Add(dtv.ColumnNames[i]);
        //        }
        //        else
        //        {
        //            numericColumnNames.Add(dtv.ColumnNames[i]);
        //        }
        //    }
        //    StringColumnNames = stringColumnNames;
        //    NumericColumnNames = numericColumnNames;

        //    //CurrentView = _DefineSIAttributes;

        //}



        #endregion
        #region Functions
        public List<string> GetUniqueOccupancyTypes(string path)
        {
            List<string> uniqueList = new List<string>();

            if (OccupancyTypeIsUsingDefault == true)
            {
                uniqueList.Add(OccupancyTypeDefaultValue);
            }
            else
            {
                string occTypeHeader = OccupancyTypeColumnName;

                if (!System.IO.File.Exists(System.IO.Path.ChangeExtension(path, "dbf")))
                {
                    //ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("This path has no associated *.dbf file.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel | FdaModel.Utilities.Messager.ErrorMessageEnum.Report));
                    return new List<string>();
                }
                DatabaseManager.DbfReader dbf = new DatabaseManager.DbfReader(System.IO.Path.ChangeExtension(path, ".dbf"));
                DatabaseManager.DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);

                object[] occtypesFromFile = dtv.GetColumn(occTypeHeader);
                foreach (object o in occtypesFromFile)
                {
                    uniqueList.Add((string)o);
                }
            }



            return uniqueList.Distinct().ToList();
        }
        private bool ValidateSIAttributes(ref string errorMessage)
        {
            //here i need to validate all the fields for valid values (no negatives etc)
            //occtype
            if (OccupancyTypeIsUsingDefault == true)
            {
                if (OccupancyTypeDefaultValue == null || OccupancyTypeDefaultValue == "") { errorMessage = "A default occupancy type must be entered"; return false; }
            }
            else
            {
                if (OccupancyTypeColumnName == null) { errorMessage = "An occupancy type must be selected"; return false; }

            }

            if (FirstFloorElevationIsChecked == true)
            {
                //first floor elevation
                if (FirstFloorElevationIsUsingDefault == true)
                {
                    if (FirstFloorElevationDefaultValue == null || FirstFloorElevationDefaultValue == "") { errorMessage = "A first floor elevation must be entered"; return false; }
                }
                else
                {
                    if (FirstFloorElevationColumnName == null) { errorMessage = "A first floor elevation must be entered"; return false; }

                }
            }
            else
            {
                //found height

                if (FoundationHeightIsUsingDefault == true)
                {
                    if (FoundationHeightDefaultValue == null || FoundationHeightDefaultValue == "") { errorMessage = "A foundation height must be entered"; return false; }
                }
                else
                {
                    if (FoundationHeightColumnName == null) { errorMessage = "A foundation height must be entered"; return false; }

                }
                //grnd elevation
                if (GroundElevationIsUsingDefault == true)
                {
                    if (GroundElevationDefaultValue == null || GroundElevationDefaultValue == "") { errorMessage = "A ground elevation must be entered"; return false; }
                }
                else
                {
                    if (GroundElevationColumnName == null) { errorMessage = "A ground elevation must be entered"; return false; }

                }
            }



            //struct value
            if (StructureValueIsUsingDefault == true)
            {
                if (StructureValueDefaultValue == null || StructureValueDefaultValue == "") { errorMessage = "A structure value must be entered"; return false; }
            }
            else
            {
                if (StructureValueColumnName == null) { errorMessage = "A structure value must be entered"; return false; }

            }
            //cont value
            if (ContentValueIsUsingDefault == true)
            {
                if (ContentValueDefaultValue == null || ContentValueDefaultValue == "") { errorMessage = "A content value must be entered, or the 'Missing' checkbox should be unchecked."; return false; }
            }
            else
            {
                //if (_DefineSIAttributes.ContentValueColumnName == null) { return false; }

            }
            //other value
            if (OtherValueIsUsingDefault == true)
            {
                if (OtherValueDefaultValue == null || OtherValueDefaultValue == "") { errorMessage = "An other value must be entered, or the 'Missing' checkbox should be unchecked."; return false; }
            }
            else
            {
                //if (_DefineSIAttributes.OtherValueColumnName == null) { return false; }

            }
            //vehicle value
            if (VehicleValueIsUsingDefault == true)
            {
                if (VehicleValueDefaultValue == null || VehicleValueDefaultValue == "") { errorMessage = "A vehicle value must be entered, or the 'Missing' checkbox should be unchecked."; return false; }
            }
            else
            {
                //if (_DefineSIAttributes.VehicleValueColumnName == null) { return false; }

            }
            //Year value
            if (YearIsUsingDefault == true)
            {
                if (YearDefaultValue == null || YearDefaultValue == "") { errorMessage = "A year must be entered, or the 'Missing' checkbox should be unchecked."; return false; }
            }
            else
            {
                //if (_DefineSIAttributes.VehicleValueColumnName == null) { return false; }

            }
            //Module value
            if (ModuleIsUsingDefault == true)
            {
                if (ModuleDefaultValue == null || ModuleDefaultValue == "") { errorMessage = "A module must be entered, or the 'Missing' checkbox should be unchecked."; return false; }
            }
            else
            {
                //if (_DefineSIAttributes.VehicleValueColumnName == null) { return false; }

            }


            return true;
        }

        #endregion
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }
    }
}
