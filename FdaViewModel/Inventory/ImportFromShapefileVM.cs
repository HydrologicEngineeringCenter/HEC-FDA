using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace FdaViewModel.Inventory
{
    //[Author("q0heccdm", "10 / 20 / 2016 8:26:40 AM")]
    public class ImportFromShapefileVM:BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/20/2016 8:26:40 AM
        #endregion
        #region Fields
        
        //These fields aid in loading the comboboxes
        private ObservableCollection<string> _Paths;
        private string[] _SIAttributes;
        private string[] _SINumericAttributes;
        private string[] _MonetaryUnits = new string[] { "$'s", "1,000$'s", "1,000,000$'s" };

        //these fields are the actual selected items that will be used to create the InventoryElement
        private string _Name;
        private string _Path;

        private bool _UseFirstFloorElevationChecked = false;
        private bool _UseFoundationHeightChecked = true;
        private bool _UseDbfForTerrainChecked = false;

        private string _StructureNameColumnName;
        private string _DamageCategoryColumnName;
        private string _OccupancyTypeColumnName;
        private string _GroundElevationColumnName;
        private string _FoundationHeightColumnName;
        private string _FirstFloorElevationColumnName;
        private string _StructureValueColumnName;
        private string _InputMonetaryUnitColumnName;

        private string _ContentValueColumnName;
        private string _OtherValueColumnName;
        private string _YearColumnName;
        private string _ModuleColumnName;

        //private StructureInventoryBaseElement _StructureInventory;

        //validation fields
        private int _StructNameIndex = -1;
        private int _DamCatIndex = -1;
        private int _OccTypeIndex =-1;
        private int _GroundElevationIndex = -1;
        private int _FoundHeightIndex = -1;
        private int _FirstFloorIndex = -1;
        private int _StructValueIndex = -1;
        private int _InputMonetaryIndex = -1;

        #endregion
        #region Properties
        public string ModuleColumnName
        {
            get { return _ModuleColumnName; }
            set { _ModuleColumnName = value; NotifyPropertyChanged(); }
        }
        public string YearColumnName
        {
            get { return _YearColumnName; }
            set { _YearColumnName = value; NotifyPropertyChanged(); }
        }
        public string OtherValueColumnName
        {
            get { return _OtherValueColumnName; }
            set { _OtherValueColumnName = value; NotifyPropertyChanged(); }
        }
        public string ContentValueColumnName
        {
            get { return _ContentValueColumnName; }
            set { _ContentValueColumnName = value; NotifyPropertyChanged(); }
        }
        public string InputMonetaryUnitColumnName
        {
            get { return _InputMonetaryUnitColumnName; }
            set { _InputMonetaryUnitColumnName = value; NotifyPropertyChanged(); }
        }
        public string StructureValueColumnName
        {
            get { return _StructureValueColumnName; }
            set { _StructureValueColumnName = value; NotifyPropertyChanged(); }
        }
        public string FirstFloorElevationColumnName
        {
            get { return _FirstFloorElevationColumnName; }
            set { _FirstFloorElevationColumnName = value; NotifyPropertyChanged(); }
        }
        public string FoundationHeightColumnName
        {
            get { return _FoundationHeightColumnName; }
            set { _FoundationHeightColumnName = value; NotifyPropertyChanged(); }
        }
        public string GroundElevationColumnName
        {
            get { return _GroundElevationColumnName; }
            set { _GroundElevationColumnName = value; NotifyPropertyChanged(); }
        }
        public string OccupancyTypeColumnName
        {
            get { return _OccupancyTypeColumnName; }
            set { _OccupancyTypeColumnName = value; NotifyPropertyChanged(); }
        }
        public string DamageCategoryColumnName
        {
            get { return _DamageCategoryColumnName; }
            set { _DamageCategoryColumnName = value; NotifyPropertyChanged(); }
        }
        public string StructureNameColumnName
        {
            get { return _StructureNameColumnName; }
            set { _StructureNameColumnName = value; NotifyPropertyChanged(); }
        }
        //public StructureInventoryBaseElement StructureInventory
        //{
        //    get { return _StructureInventory; }
        //    set { _StructureInventory = value; NotifyPropertyChanged(); }
        //}
        public ObservableCollection<string> Paths
        {
            get { return _Paths; }
            set { _Paths = value; NotifyPropertyChanged(); }
        }
        public string[] MonetaryUnits
        {
            get { return _MonetaryUnits; }
        }
        public bool UseFirstFloorElevationChecked
        {
            get { return _UseFirstFloorElevationChecked; }
            set { _UseFirstFloorElevationChecked = value; }
        }
        public bool UseFoundationHeightChecked
        {
            get { return _UseFoundationHeightChecked; }
            set { _UseFoundationHeightChecked = value; }
        }
        public bool UseDbfForTerrainChecked
        {
            get { return _UseDbfForTerrainChecked; }
            set { _UseDbfForTerrainChecked = value;}
        }
        public int StructNameIndex
        {
            get { return _StructNameIndex; }
            set { _StructNameIndex = value; }
        }
        public bool isStructNameValid
        {
            get { return validateStructureNamesAreUnique(StructNameIndex); }
           // set { _IsStructNameValid = value; }
        }
        

        public int DamCatIndex
        {
            get { return _DamCatIndex; }
            set { _DamCatIndex = value; }
        }
        public int OccTypeIndex
        {
            get { return _OccTypeIndex; }
            set { _OccTypeIndex = value; }
        }
        public int GroundElevationIndex
        {
            get { return _GroundElevationIndex; }
            set { _GroundElevationIndex = value; }
        }
        public int FoundHeightIndex
        {
            get { return _FoundHeightIndex; }
            set { _FoundHeightIndex = value; }
        }
        public int FirstFloorIndex
        {
            get { return _FirstFloorIndex; }
            set { _FirstFloorIndex = value; }
        }
        public int StructValueIndex
        {
            get { return _StructValueIndex; }
            set { _StructValueIndex = value; }
        }
        public int InputMonetaryIndex
        {
            get { return _InputMonetaryIndex; }
            set { _InputMonetaryIndex = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }
        public string Path
        {
            get { return _Path; }
            set { _Path = value; NotifyPropertyChanged(); loadDBFData();  }
        }

        

        public string[] SIAttributes
        {
            get { return _SIAttributes; }
            set { _SIAttributes = value; NotifyPropertyChanged(); }
        }
        public string[] SINumericAttributes
        {
            get { return _SINumericAttributes; }
            set { _SINumericAttributes = value; NotifyPropertyChanged(); }
        }

        #endregion
        #region Constructors
        public ImportFromShapefileVM():base()
        {
            _Name = "Example";
            Paths = new ObservableCollection<string>{ "cody", "bob" , "John"};
        }
        #endregion
        #region Voids
   
        private void loadDBFData()
        {
            //i am creating two lists here. One is all the column names and the other is just the numeric column names
            //this is to shorten the list for the user on the choices that have to be numeric.
            string dbfPath = System.IO.Path.ChangeExtension(Path, ".dbf");
            List<string> tempColumnNames = new List<string>();
            List<string> tempNumColumnNames = new List<string>();
            if (!System.IO.File.Exists(dbfPath))
            {
                //do nothing.
            }
            else
            {
                DataBase_Reader.DbfReader dbf = new DataBase_Reader.DbfReader(dbfPath);

                DataBase_Reader.DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);//this is assuming one table
                tempColumnNames = dtv.ColumnNames.ToList();
                tempNumColumnNames = dtv.GetNumericColumns();

            }
           // FdaViewModel.Inventory.ImportFromShapefileVM tempVM = (FdaViewModel.Inventory.ImportFromShapefileVM)Resources["vm"];
            SIAttributes = tempColumnNames.ToArray();
            SINumericAttributes = tempNumColumnNames.ToArray();
            //Resources["vm"] = tempVM;
        }
        private bool validateStructureNamesAreUnique(int index)
        {
            if (Path == null || Path == "") return false;
            string dbfPath = System.IO.Path.ChangeExtension(Path, ".dbf");
            DataBase_Reader.DbfReader dbf = new DataBase_Reader.DbfReader(dbfPath);

            DataBase_Reader.DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);
            object[] columnValues = dtv.GetColumn(index);
            string[] uniqueNames = new string[columnValues.Length];
                  
            for(int i = 0; i<columnValues.Length;i++)
            {
                if (uniqueNames.Contains(columnValues[i].ToString())) { return false; }
                else { uniqueNames[i] = columnValues[i].ToString(); }
            }
            return true;
        }
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
            AddRule(nameof(Name), () => !(Name == ""), "The Name cannot be blank.");
            AddRule(nameof(Name), () => !(Name == null), "The Name cannot be blank.");
            AddRule(nameof(StructNameIndex), () => !(StructNameIndex < 0), "You must select a structure name.");
            AddRule(nameof(isStructNameValid), () => (isStructNameValid), "Not all structure names are unique.");


            AddRule(nameof(DamCatIndex), () => !(DamCatIndex < 0), "You must select a damage category.");
            AddRule(nameof(OccTypeIndex), () => !(OccTypeIndex < 0), "You must select an occupancy type.");

            AddRule(nameof(GroundElevationIndex), () => !((UseDbfForTerrainChecked==true) && (GroundElevationIndex < 0)), "You must select a ground elevation.");

            AddRule(nameof(FoundHeightIndex), () => !((UseFoundationHeightChecked == true) && (FoundHeightIndex < 0)), "You must select a foundation height.");

            AddRule(nameof(FirstFloorIndex), () => !((UseFirstFloorElevationChecked == true) && (FirstFloorIndex < 0)), "You must select a first floor elevation.");
            AddRule(nameof(StructValueIndex), () => !(StructValueIndex < 0), "You must select a structure value.");
            AddRule(nameof(InputMonetaryIndex), () => !(InputMonetaryIndex < 0), "You must select an input monetary unit.");

        }

        public  void Save()
        {
            //this file will already be created somewhere else. It will be our main FDA study file
            DataBase_Reader.SqLiteReader.CreateSqLiteFile(System.IO.Path.GetDirectoryName(Path) + "\\codyTest.sqlite");
            StructureInventoryLibrary.SharedData.StudyDatabase = new DataBase_Reader.SqLiteReader(System.IO.Path.GetDirectoryName(Path) + "\\codyTest.sqlite");
            
            
             
                
            LifeSimGIS.ShapefileReader myReader = new LifeSimGIS.ShapefileReader(Path);


            //create the data table that will get written out
            System.Data.DataTable myAttributeTable = new System.Data.DataTable(Name);

            myAttributeTable.Columns.Add(StructureInventoryBaseElement.OccupancyTypeField,typeof(string));
            myAttributeTable.Columns.Add(StructureInventoryBaseElement.FoundationHeightField, typeof(string));
            myAttributeTable.Columns.Add(StructureInventoryBaseElement.StructureValueField, typeof(string));
            myAttributeTable.Columns.Add(StructureInventoryBaseElement.ContentValueField, typeof(string));
            myAttributeTable.Columns.Add(StructureInventoryBaseElement.OtherValueField, typeof(string));
            myAttributeTable.Columns.Add(StructureInventoryBaseElement.VehicleValueField, typeof(string));
            myAttributeTable.Columns.Add(StructureInventoryBaseElement.FirstFloorElevationField, typeof(string));
            myAttributeTable.Columns.Add(StructureInventoryBaseElement.GroundElevationField, typeof(string));


            DataBase_Reader.DataTableView aTable = myReader.GetAttributeTable();

            for(int i = 0;i<aTable.NumberOfRows;i++)
            {
                System.Data.DataRow nextRow = myAttributeTable.NewRow();

                nextRow[StructureInventoryBaseElement.OccupancyTypeField] = aTable.GetCell(OccupancyTypeColumnName,i);
                nextRow[StructureInventoryBaseElement.FoundationHeightField] = aTable.GetCell(FoundationHeightColumnName, i);
                nextRow[StructureInventoryBaseElement.StructureValueField] = aTable.GetCell(OccupancyTypeColumnName, i);
                nextRow[StructureInventoryBaseElement.ContentValueField] = aTable.GetCell(OccupancyTypeColumnName, i);
                nextRow[StructureInventoryBaseElement.OtherValueField] = aTable.GetCell(OccupancyTypeColumnName, i);
                nextRow[StructureInventoryBaseElement.VehicleValueField] = aTable.GetCell(OccupancyTypeColumnName, i);
                nextRow[StructureInventoryBaseElement.FirstFloorElevationField] = aTable.GetCell(FirstFloorElevationColumnName, i);
                nextRow[StructureInventoryBaseElement.GroundElevationField] = aTable.GetCell(GroundElevationColumnName, i);



                myAttributeTable.Rows.Add(nextRow);
            }

            //create an in memory reader and data table view

            DataBase_Reader.InMemoryReader myInMemoryReader = new DataBase_Reader.InMemoryReader(myAttributeTable);
            DataBase_Reader.DataTableView myDTView = myInMemoryReader.GetTableManager(Name);

            //create the geo package writer that will write the data out
            LifeSimGIS.GeoPackageWriter myGeoPackWriter = new LifeSimGIS.GeoPackageWriter(StructureInventoryLibrary.SharedData.StudyDatabase);

            // write the data out
            //myGeoPackWriter.AddFeatures(Name, myReader.ToFeatures(), myReader.GetAttributeTable());
            myGeoPackWriter.AddFeatures(Name, myReader.ToFeatures(), myDTView);




            //Dim gpkgW As New GeoPackageWriter(SharedData.StudyDatabase);
            //StructureInventoryLibrary.geo
        }
        #endregion
        #region Functions
        #endregion
    }
}
