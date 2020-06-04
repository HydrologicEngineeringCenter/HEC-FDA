using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using FdaViewModel.Inventory.OccupancyTypes;
using System.Windows;
using FdaViewModel.Utilities;
using System.Data;
using DatabaseManager;
using FdaViewModel.Saving.PersistenceManagers;

namespace FdaViewModel.Inventory
{
    //[Author(q0heccdm, 6 / 22 / 2017 10:13:12 AM)]
   public  class ImportStructuresFromShapefileVM:Editors.BaseEditorVM
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/22/2017 10:13:12 AM
        #endregion
        #region Fields

        private string _SelectedPath;


        private BaseViewModel _CurrentView;

        private DefineSIAttributesVM _DefineSIAttributes;
        private AttributeLinkingListVM _AttributeLinkingList;

        private ObservableCollection<string> _AvailablePaths;

        private List<string> _StringColumnNames = new List<string>();
        private List<string> _NumericColumnNames = new List<string>();

        private bool _CurrentViewIsEnabled;


        #endregion
        #region Properties
        

        public bool IsInEditMode { get; set; }

        public DefineSIAttributesVM DefineSIAttributes
        {
            get { return _DefineSIAttributes; }
            set { _DefineSIAttributes = value; }
        }
        public AttributeLinkingListVM AttributeLinkingList
        {
            get { return _AttributeLinkingList; }
            set { _AttributeLinkingList = value; }
        }
        public bool CurrentViewIsEnabled
        {
            get { return _CurrentViewIsEnabled; }
            set { _CurrentViewIsEnabled = value; NotifyPropertyChanged(); }
        }
        public BaseViewModel CurrentView
        {
            get { return _CurrentView; }
            set { _CurrentView = value; NotifyPropertyChanged(); }
        }


        public string SelectedPath
        {
            get { return _SelectedPath; }
            set { _SelectedPath = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<string> AvailablePaths
        {
            get { return _AvailablePaths; }
            set { _AvailablePaths = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public ImportStructuresFromShapefileVM(ObservableCollection<string> pointFiles, Editors.EditorActionManager actionManager, bool isInEditMode) :base(actionManager)
        {
            IsInEditMode = isInEditMode;
            AvailablePaths = pointFiles;

            _DefineSIAttributes = new DefineSIAttributesVM(SelectedPath);
            _AttributeLinkingList = new AttributeLinkingListVM();
            CurrentViewIsEnabled = true;
            CurrentView = _DefineSIAttributes;     
            
            
            
        }
        //public ImportStructuresFromShapefileVM() : base()
        //{
        //    _DefineSIAttributes = new DefineSIAttributesVM();
        //    _AttributeLinkingList = new AttributeLinkingListVM();
        //    CurrentViewIsEnabled = true;
        //    CurrentView = _DefineSIAttributes;
        //    //ObservableCollection<string> availPaths = new ObservableCollection<string>();
        //    //foreach(string path in pointShapeFilePaths)
        //    //{
        //    //    availPaths.Add(path);
        //    //}
        //    //AvailablePaths = availPaths;          
            
        //}

        //this constructor is used when opening a previously created structure inventory for editing.

        //public ImportStructuresFromShapefileVM(InventoryElement element, string name, string description,string path, DefineSIAttributesVM defineSIAttributesVM, AttributeLinkingListVM attributeLinkingListVM): base()

        //{
        //    SelectedPath = element.path path;
        //    _DefineSIAttributes = defineSIAttributesVM;
        //    _AttributeLinkingList = attributeLinkingListVM;
        //    Description = description;
        //    IsInEditMode = true;
        //    CurrentViewIsEnabled = true;
        //    //// get the paths for the shapefile
        //    //ObservableCollection<string> availPaths = new ObservableCollection<string>();
        //    //foreach (string path in pointShapeFilePaths)
        //    //{
        //    //    availPaths.Add(path);
        //    //}
        //    //availPaths.Add(defineSIAttributesVM.SelectedPath); // i need to check against duplicate paths at some point
        //    //AvailablePaths = availPaths;

        //    // get all the string and numeric column headers for that shapefile
        //    //SelectedPath = selectedShapefilePath;

        //    //loadUniqueNames(SelectedPath);
           
        //    CurrentView = _DefineSIAttributes;

        //    Name = name;
        //}
        #endregion
        #region Voids
      
        public void PreviousButtonClicked()
        {
            CurrentView = _DefineSIAttributes;
        }

        #region Next Button Click

        private bool NextButtonClickedWhileDefiningSIAttributes()
        {
            //i need validation before moving on to the next screen
            string errorMessage = null;
            if (ValidateSIAttributes(ref errorMessage) == false)
            {
                //Navigate( new Utilities.CustomMessageBoxVM(Utilities.CustomMessageBoxVM.ButtonsEnum.OK,errorMessage));
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK);
                return false;
            }

            List<string> occtypes = new List<string>();

            if (_DefineSIAttributes.OccupancyTypeIsUsingDefault == true)
            {
                occtypes.Add(_DefineSIAttributes.OccupancyTypeDefaultValue);
            }
            else
            {
                occtypes = _DefineSIAttributes.GetUniqueOccupancyTypes(SelectedPath);

            }

            if (IsInEditMode == true)
            {
                _AttributeLinkingList.IsInEditMode = true;
            }
            else
            {
                _AttributeLinkingList = new AttributeLinkingListVM(occtypes, StudyCache.GetChildElementsOfType<OccupancyTypesElement>(), _AttributeLinkingList.OccupancyTypesInStudy);
            }
            CurrentView = _AttributeLinkingList;
            return true;
        }

        private bool NextButtonClickWhileOnAttributeLinkingList()
        {
            string error = "";
            if (ValidateLinkingList(ref error) == false) { return false; }
            //We need to write the data to the sqlite file

            StructureInventoryPersistenceManager manager = Saving.PersistenceFactory.GetStructureInventoryManager();

            // DataBase_Reader.SqLiteReader.CreateSqLiteFile(System.IO.Path.GetDirectoryName(SelectedPath) + "\\codyTest.sqlite");
            StructureInventoryLibrary.SharedData.StudyDatabase = new SQLiteManager(Storage.Connection.Instance.ProjectFile);

            LifeSimGIS.ShapefileReader myReader = new LifeSimGIS.ShapefileReader(SelectedPath);

            //create the data table that will get written out
            DataTable newStructureTable = manager.CreateEmptyStructuresTable();

            //this is the table from the shapefile that was passed in. We will need to map it to our columns defined above.
            DataTableView attributeTableFromFile = myReader.GetAttributeTable();
            if (attributeTableFromFile.ParentDatabase.DataBaseOpen == false)
            {
                attributeTableFromFile.ParentDatabase.Open();
            }

            for (int i = 0; i < attributeTableFromFile.NumberOfRows; i++)
            {
                newStructureTable.Rows.Add(CreateRowForStructure(newStructureTable, attributeTableFromFile, i));
            }

            attributeTableFromFile.ParentDatabase.Close();

            //this line will create the child table in the database.
            manager.Save(newStructureTable, Name, myReader.ToFeatures());
            //this line will add it to the parent table.
            Save();
            return true;
        }

        public bool NextButtonClicked()
        {

            if(CurrentView.GetType() == typeof(DefineSIAttributesVM))
            {

                bool success = NextButtonClickedWhileDefiningSIAttributes();
                if(!success)
                {
                    return false;
                }
            }
            else if(CurrentView.GetType() == typeof(AttributeLinkingListVM))
            {

                bool success = NextButtonClickWhileOnAttributeLinkingList();
                {
                   if(!success)
                    {
                        return false;
                    }
                }
                    
            }

            return true;
        }

        //private DataTable CreateEmptyStructuresTable()
        //{
        //    DataTable newStructureTable = new DataTable(Name);

        //    newStructureTable.Columns.Add(StructureInventoryBaseElement.OccupancyTypeField, typeof(string));
        //    newStructureTable.Columns.Add(StructureInventoryBaseElement.OccupancyTypeGroupName, typeof(string));
        //    newStructureTable.Columns.Add(StructureInventoryBaseElement.FoundationHeightField, typeof(string));
        //    newStructureTable.Columns.Add(StructureInventoryBaseElement.StructureValueField, typeof(string));
        //    newStructureTable.Columns.Add(StructureInventoryBaseElement.ContentValueField, typeof(string));
        //    newStructureTable.Columns.Add(StructureInventoryBaseElement.OtherValueField, typeof(string));
        //    newStructureTable.Columns.Add(StructureInventoryBaseElement.VehicleValueField, typeof(string));
        //    newStructureTable.Columns.Add(StructureInventoryBaseElement.FirstFloorElevationField, typeof(string));
        //    newStructureTable.Columns.Add(StructureInventoryBaseElement.GroundElevationField, typeof(string));
        //    newStructureTable.Columns.Add(StructureInventoryBaseElement.YearField, typeof(string));
        //    newStructureTable.Columns.Add(StructureInventoryBaseElement.ModuleField, typeof(string));
        //    return newStructureTable;
        //}

        

        private DataRow CreateRowForStructure(DataTable newStructureTable, DataTableView attributeTableFromFile, int i)
        {
            //we are going to create a new row in our table that contains everything we need for a single structure.
            DataRow dataRow = newStructureTable.NewRow();

            AssignOcctypeNameAndOcctypeGroupNameToRow(dataRow, attributeTableFromFile, i);

            AssignFoundationHeight_FirstFloorElevation_GroundElevationToRow(dataRow, attributeTableFromFile, i);

            AssignStructureValueToRow(dataRow, attributeTableFromFile, i);

            AssignContentValueToRow(dataRow, attributeTableFromFile, i);

            AssignOtherValueToRow(dataRow, attributeTableFromFile, i);

            AssignVehicleValueToRow(dataRow, attributeTableFromFile, i);

            AssignYearValueToRow(dataRow, attributeTableFromFile, i);

            AssignModuleValueToRow(dataRow, attributeTableFromFile, i);
 
            return dataRow;
        }

      

        private void AssignDamageCategoryToRow(DataRow dataRow, DataTableView attributeTableFromFile, int i)
        {

        }

        private void AssignModuleValueToRow(DataRow dataRow, DataTableView attributeTableFromFile, int i)
        {
            if (_DefineSIAttributes.ModuleIsUsingDefault == false)
            {
                if (_DefineSIAttributes.ModuleColumnName != null)
                {
                    dataRow[StructureInventoryBaseElement.ModuleField] = attributeTableFromFile.GetCell(_DefineSIAttributes.ModuleColumnName, i);

                }
                else
                {
                    dataRow[StructureInventoryBaseElement.ModuleField] = "0";
                }
            }
            else
            {
                dataRow[StructureInventoryBaseElement.ModuleField] = _DefineSIAttributes.ModuleDefaultValue;
            }
        }

        private void AssignYearValueToRow(DataRow dataRow, DataTableView attributeTableFromFile, int i)
        {
            if (_DefineSIAttributes.YearIsUsingDefault == false)
            {
                if (_DefineSIAttributes.YearColumnName != null)
                {
                    dataRow[StructureInventoryBaseElement.YearField] = attributeTableFromFile.GetCell(_DefineSIAttributes.YearColumnName, i);

                }
                else
                {
                    dataRow[StructureInventoryBaseElement.YearField] = "0";
                }
            }
            else
            {
                dataRow[StructureInventoryBaseElement.YearField] = _DefineSIAttributes.YearDefaultValue;
            }
        }

        private void AssignVehicleValueToRow(DataRow dataRow, DataTableView attributeTableFromFile, int i)
        {
            if (_DefineSIAttributes.VehicleValueIsUsingDefault == false)
            {
                if (_DefineSIAttributes.VehicleValueColumnName != null)
                {
                    dataRow[StructureInventoryBaseElement.VehicleValueField] = attributeTableFromFile.GetCell(_DefineSIAttributes.VehicleValueColumnName, i);

                }
                else
                {
                    dataRow[StructureInventoryBaseElement.VehicleValueField] = "0";
                }
            }
            else
            {
                dataRow[StructureInventoryBaseElement.VehicleValueField] = _DefineSIAttributes.VehicleValueDefaultValue;
            }
        }

        private void AssignOtherValueToRow(DataRow dataRow, DataTableView attributeTableFromFile, int i)
        {
            if (_DefineSIAttributes.OtherValueIsUsingDefault == false)
            {
                if (_DefineSIAttributes.OtherValueColumnName != null)
                {
                    dataRow[StructureInventoryBaseElement.OtherValueField] = attributeTableFromFile.GetCell(_DefineSIAttributes.OtherValueColumnName, i);

                }
                else
                {
                    dataRow[StructureInventoryBaseElement.OtherValueField] = "0";
                }
            }
            else
            {
                dataRow[StructureInventoryBaseElement.OtherValueField] = _DefineSIAttributes.OtherValueDefaultValue;
            }
        }

        private void AssignContentValueToRow(DataRow dataRow, DataTableView attributeTableFromFile, int i)
        {
            if (_DefineSIAttributes.ContentValueIsUsingDefault == false)
            {
                if (_DefineSIAttributes.ContentValueColumnName != null)
                {
                    dataRow[StructureInventoryBaseElement.ContentValueField] = attributeTableFromFile.GetCell(_DefineSIAttributes.ContentValueColumnName, i);
                }
                else
                {
                    dataRow[StructureInventoryBaseElement.ContentValueField] = "0";
                }
            }
            else
            {
                dataRow[StructureInventoryBaseElement.ContentValueField] = _DefineSIAttributes.ContentValueDefaultValue;
            }
        }

        private void AssignStructureValueToRow(DataRow dataRow, DataTableView attributeTableFromFile, int i)
        {
            if (_DefineSIAttributes.StructureValueIsUsingDefault == false)
            {
                dataRow[StructureInventoryBaseElement.StructureValueField] = attributeTableFromFile.GetCell(_DefineSIAttributes.StructureValueColumnName, i);
            }
            else
            {
                dataRow[StructureInventoryBaseElement.StructureValueField] = _DefineSIAttributes.StructureValueDefaultValue;
            }
        }

        private void AssignOcctypeNameAndOcctypeGroupNameToRow(DataRow dataRow, DataTableView attributeTableFromFile, int i)
        {
            if (_DefineSIAttributes.OccupancyTypeIsUsingDefault == false)
            {
                //string groupName = "";
                string occTypeKey = attributeTableFromFile.GetCell(_DefineSIAttributes.OccupancyTypeColumnName, i).ToString(); // this is the old occtype from the dbf of the struc inv.
                string combinedNewOccTypeAndGroupName = _AttributeLinkingList.OccupancyTypesDictionary[occTypeKey];
                //deal with the case that no selection was made so the new occtype would be blank
                if (combinedNewOccTypeAndGroupName == "" || combinedNewOccTypeAndGroupName == null)
                {
                    dataRow[StructureInventoryBaseElement.OccupancyTypeField] = occTypeKey;
                    dataRow[StructureInventoryBaseElement.OccupancyTypeGroupName] = "Undefined";

                }
                else
                {
                    string[] occtypeAndGroupName = _AttributeLinkingList.ParseOccTypeNameAndGroupNameFromCombinedString(combinedNewOccTypeAndGroupName);
                    dataRow[StructureInventoryBaseElement.OccupancyTypeField] = occtypeAndGroupName[0];
                    dataRow[StructureInventoryBaseElement.OccupancyTypeGroupName] = occtypeAndGroupName[1];
                }

            }
            else
            {

                //string groupName = "";
                string occTypeKey = _DefineSIAttributes.OccupancyTypeDefaultValue;
                string combinedNewOccTypeAndGroupName = _AttributeLinkingList.OccupancyTypesDictionary[occTypeKey];
                //deal with the case that no selection was made so the new occtype would be blank
                if (combinedNewOccTypeAndGroupName == "" || combinedNewOccTypeAndGroupName == null)
                {
                    dataRow[StructureInventoryBaseElement.OccupancyTypeField] = occTypeKey;
                    dataRow[StructureInventoryBaseElement.OccupancyTypeGroupName] = "Undefined";

                }
                else
                {
                    string[] occtypeAndGroupName = _AttributeLinkingList.ParseOccTypeNameAndGroupNameFromCombinedString(combinedNewOccTypeAndGroupName);

                    dataRow[StructureInventoryBaseElement.OccupancyTypeField] = occtypeAndGroupName[0];
                    dataRow[StructureInventoryBaseElement.OccupancyTypeGroupName] = occtypeAndGroupName[1];
                }


            }
        }
        private void AssignFoundationHeight_FirstFloorElevation_GroundElevationToRow(DataRow dataRow, DataTableView attributeTableFromFile, int i)
        {
            if (_DefineSIAttributes.FirstFloorElevationIsChecked == true)
            {
                if (_DefineSIAttributes.FirstFloorElevationIsUsingDefault == false)
                {
                    dataRow[StructureInventoryBaseElement.FirstFloorElevationField] = attributeTableFromFile.GetCell(_DefineSIAttributes.FirstFloorElevationColumnName, i);
                }
                else
                {
                    dataRow[StructureInventoryBaseElement.FirstFloorElevationField] = _DefineSIAttributes.FirstFloorElevationDefaultValue;
                }
                dataRow[StructureInventoryBaseElement.FoundationHeightField] = 0;
                dataRow[StructureInventoryBaseElement.GroundElevationField] = 0;
            }
            else
            {
                if (_DefineSIAttributes.FoundationHeightIsUsingDefault == false)
                {
                    dataRow[StructureInventoryBaseElement.FoundationHeightField] = attributeTableFromFile.GetCell(_DefineSIAttributes.FoundationHeightColumnName, i);
                }
                else
                {
                    dataRow[StructureInventoryBaseElement.FoundationHeightField] = _DefineSIAttributes.FoundationHeightDefaultValue;
                }

                if (_DefineSIAttributes.GroundElevationIsUsingDefault == false)
                {
                    dataRow[StructureInventoryBaseElement.GroundElevationField] = attributeTableFromFile.GetCell(_DefineSIAttributes.GroundElevationColumnName, i);
                }
                else
                {
                    dataRow[StructureInventoryBaseElement.GroundElevationField] = _DefineSIAttributes.GroundElevationDefaultValue;
                }

                dataRow[StructureInventoryBaseElement.FirstFloorElevationField] = 0;
            }
        }


        #endregion



        private bool ValidateLinkingList(ref string errorMessage)
        {
            //if(_AttributeLinkingList.)
            if(HasFatalError)//if one of the rules have been violated
            {
                return false;
            }
            return true;
        }

        private bool ValidateSIAttributes(ref string errorMessage)
        {
            //this will run through all the "rules". I am not using the OK close control for the buttons so i need to handle
            //that on my own.
            //Validate();
            //if(HasFatalError)
            //{
            //    return false;
            //}

            //here i need to validate all the fields for valid values (no negatives etc)
            //occtype
            if (_DefineSIAttributes.OccupancyTypeIsUsingDefault == true)
            {
                if(_DefineSIAttributes.OccupancyTypeDefaultValue == null || _DefineSIAttributes.OccupancyTypeDefaultValue == "") { errorMessage = "A default occupancy type must be entered"; return false; }
            }
            else
            {
                if (_DefineSIAttributes.OccupancyTypeColumnName == null) { errorMessage = "An occupancy type must be selected"; return false; }

            }

            if(_DefineSIAttributes.FirstFloorElevationIsChecked == true)
            {
                //first floor elevation
                if (_DefineSIAttributes.FirstFloorElevationIsUsingDefault == true)
                {
                    if (_DefineSIAttributes.FirstFloorElevationDefaultValue == null || _DefineSIAttributes.FirstFloorElevationDefaultValue == "") { errorMessage = "A first floor elevation must be entered"; return false; }
                }
                else
                {
                    if (_DefineSIAttributes.FirstFloorElevationColumnName == null) { errorMessage = "A first floor elevation must be entered"; return false; }

                }
            }
            else
            {
                //found height

                if (_DefineSIAttributes.FoundationHeightIsUsingDefault == true)
                {
                    if (_DefineSIAttributes.FoundationHeightDefaultValue == null || _DefineSIAttributes.FoundationHeightDefaultValue == "") { errorMessage = "A foundation height must be entered"; return false; }
                }
                else
                {
                    if (_DefineSIAttributes.FoundationHeightColumnName == null) { errorMessage = "A foundation height must be entered"; return false; }

                }
                //grnd elevation
                if (_DefineSIAttributes.GroundElevationIsUsingDefault == true)
                {
                    if (_DefineSIAttributes.GroundElevationDefaultValue == null || _DefineSIAttributes.GroundElevationDefaultValue == "") { errorMessage = "A ground elevation must be entered"; return false; }
                }
                else
                {
                    if (_DefineSIAttributes.GroundElevationColumnName == null) { errorMessage = "A ground elevation must be entered"; return false; }

                }
            }
            

            
            //struct value
            if (_DefineSIAttributes.StructureValueIsUsingDefault == true)
            {
                if (_DefineSIAttributes.StructureValueDefaultValue == null || _DefineSIAttributes.StructureValueDefaultValue == "") { errorMessage = "A structure value must be entered"; return false; }
            }
            else
            {
                if (_DefineSIAttributes.StructureValueColumnName == null) { errorMessage = "A structure value must be entered"; return false; }

            }
            //cont value
            if (_DefineSIAttributes.ContentValueIsUsingDefault == true)
            {
                if (_DefineSIAttributes.ContentValueDefaultValue == null || _DefineSIAttributes.ContentValueDefaultValue == "") { errorMessage = "A content value must be entered, or the 'Missing' checkbox should be unchecked."; return false; }
            }
            else
            {
                //if (_DefineSIAttributes.ContentValueColumnName == null) { return false; }

            }
            //other value
            if (_DefineSIAttributes.OtherValueIsUsingDefault == true)
            {
                if (_DefineSIAttributes.OtherValueDefaultValue == null || _DefineSIAttributes.OtherValueDefaultValue == "") { errorMessage = "An other value must be entered, or the 'Missing' checkbox should be unchecked."; return false; }
            }
            else
            {
                //if (_DefineSIAttributes.OtherValueColumnName == null) { return false; }

            }
            //vehicle value
            if (_DefineSIAttributes.VehicleValueIsUsingDefault == true)
            {
                if (_DefineSIAttributes.VehicleValueDefaultValue == null || _DefineSIAttributes.VehicleValueDefaultValue == "") { errorMessage = "A vehicle value must be entered, or the 'Missing' checkbox should be unchecked."; return false; }
            }
            else
            {
                //if (_DefineSIAttributes.VehicleValueColumnName == null) { return false; }

            }
            //Year value
            if (_DefineSIAttributes.YearIsUsingDefault == true)
            {
                if (_DefineSIAttributes.YearDefaultValue == null || _DefineSIAttributes.YearDefaultValue == "") { errorMessage = "A year must be entered, or the 'Missing' checkbox should be unchecked."; return false; }
            }
            else
            {
                //if (_DefineSIAttributes.VehicleValueColumnName == null) { return false; }

            }
            //Module value
            if (_DefineSIAttributes.ModuleIsUsingDefault == true)
            {
                if (_DefineSIAttributes.ModuleDefaultValue == null || _DefineSIAttributes.ModuleDefaultValue == "") { errorMessage = "A module must be entered, or the 'Missing' checkbox should be unchecked."; return false; }
            }
            else
            {
                //if (_DefineSIAttributes.VehicleValueColumnName == null) { return false; }

            }


            return true;
        }

       

        public void loadUniqueNames(string path)

        {

            List<string> stringColumnNames = new List<string>();
            List<string> numericColumnNames = new List<string>();


            CurrentViewIsEnabled = true;
            SelectedPath = path; //isnt this bound?? yes but it is not working.
            DatabaseManager.DbfReader dbf = new DatabaseManager.DbfReader(System.IO.Path.ChangeExtension(SelectedPath, ".dbf"));
            DatabaseManager.DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);


            for (int i = 0; i < dtv.ColumnNames.Count(); i++)
            {
                if (dtv.ColumnTypes[i] == typeof(string))
                {
                    stringColumnNames.Add(dtv.ColumnNames[i]);
                }
                else
                {
                    numericColumnNames.Add(dtv.ColumnNames[i]);
                }
            }
            _DefineSIAttributes.StringColumnNames = stringColumnNames;
            _DefineSIAttributes.NumericColumnNames = numericColumnNames;

            CurrentView = _DefineSIAttributes;

        }
        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
            AddRule(nameof(Name), () => !(Name == null), "The Name cannot be blank.");

        }

        /// <summary>
        /// This method saves a new occtype group for this structure.
        /// It also creates the SI element and saves it to the parent table.
        /// </summary>
        public override void Save()
        {
            //create a "master occtype group" for this structure inv
            // 1.) create the string name
            string groupName = Name + " > Occupancy Types";
            //2.) create the list of occtype 
            List<IOccupancyType> newListOfOccType = new List<IOccupancyType>();
            List<string> listOfKeys = AttributeLinkingList.OccupancyTypesDictionary.Keys.ToList();
            for (int i = 0; i < listOfKeys.Count; i++)
            {
                IOccupancyType ot = OccupancyTypeFactory.Factory();
                if (AttributeLinkingList.OccupancyTypesDictionary[listOfKeys[i]] != "")
                {
                    //find the chosen occtype and replace the name with the name from the file
                    string[] occtypeAndGroupName = new string[2];
                    occtypeAndGroupName = AttributeLinkingList.ParseOccTypeNameAndGroupNameFromCombinedString(AttributeLinkingList.OccupancyTypesDictionary[listOfKeys[i]]);
                    ot = GetOcctypeFromGroup(occtypeAndGroupName[0], occtypeAndGroupName[1]);
                    ot.Name = listOfKeys[i];

                }
                else
                {
                    //they made no selection so create an empty occtype
                    ot.Name = listOfKeys[i];
                }
                newListOfOccType.Add(ot);
            }

            //Dictionary<string, bool[]> _OcctypeTabsSelectedDictionary = new Dictionary<string, bool[]>();

            //foreach (IOccupancyType ot in newListOfOccType)
            //{
            //    bool[] tabsCheckedArray = new bool[] { true, true, true, false };
            //    _OcctypeTabsSelectedDictionary.Add(ot.Name, tabsCheckedArray);

            //}
            int newGroupID = Saving.PersistenceFactory.GetOccTypeManager().GetUnusedId();
            OccupancyTypesElement newOccTypeGroup = new OccupancyTypesElement(groupName,newGroupID, newListOfOccType);
            //todo: cody commented out on 2/20/2020 - put back in when occtypes are working
            //Saving.PersistenceFactory.GetOccTypeManager().SaveNew(newOccTypeGroup);

            StructureInventoryBaseElement SIBase = new StructureInventoryBaseElement(Name, Description);
            InventoryElement elementToSave = new InventoryElement(SIBase, false);

            //as of oct 2018 there is no editing, so it should always save as a new element
            Saving.PersistenceManagers.StructureInventoryPersistenceManager manager = Saving.PersistenceFactory.GetStructureInventoryManager();
            if (IsImporter && HasSaved == false)
            {
                OccupancyTypesOwnerElement owner = StudyCache.GetParentElementOfType<OccupancyTypesOwnerElement>();

                //clear the actions while it is saving
                List<NamedAction> actions = new List<NamedAction>();
                foreach (NamedAction act in owner.Actions)
                {
                    actions.Add(act);
                }
                owner.Actions.Clear();

                manager.SaveNew(elementToSave);
                HasSaved = true;
                OriginalElement = elementToSave;
            }
            else
            {
                //manager.SaveExisting((ImpactAreaElement)OriginalElement, elementToSave, 0);
            }
        }

        private IOccupancyType GetOcctypeFromGroup(string occtypeName, string groupName)
        {
            foreach (OccupancyTypes.OccupancyTypesElement group in StudyCache.GetChildElementsOfType<OccupancyTypesElement>())// OccupancyTypes.OccupancyTypesOwnerElement.ListOfOccupancyTypesGroups)
            {
                if (group.Name == groupName)
                {
                    foreach (IOccupancyType ot in group.ListOfOccupancyTypes)
                    {
                        if (ot.Name == occtypeName)
                        {
                            return ot;
                        }
                    }
                }
            }
            return OccupancyTypeFactory.Factory(); // if it gets here then no occtype matching the names given exists. Should we send an error message?
        }

        

        #endregion
        #region Functions
        #endregion
    }
}
