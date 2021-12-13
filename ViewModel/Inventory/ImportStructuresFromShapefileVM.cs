using DatabaseManager;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using ViewModel.Editors;
using ViewModel.Inventory.OccupancyTypes;
using ViewModel.Saving.PersistenceManagers;
using ViewModel.Utilities;
using ViewModel.Watershed;
using ViewModel.Saving;

namespace ViewModel.Inventory
{
    public  class ImportStructuresFromShapefileVM:BaseEditorVM
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
        private bool _CurrentViewIsEnabled;
        #endregion
        #region Properties
        

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
            set { _SelectedPath = value; SelectedPathChanged(); }
        }

        public ObservableCollection<string> AvailablePaths
        {
            get { return _AvailablePaths; }
            set { _AvailablePaths = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public ImportStructuresFromShapefileVM(ObservableCollection<string> pointFiles, EditorActionManager actionManager) :base(actionManager)
        {
            AvailablePaths = pointFiles;
            _DefineSIAttributes = new DefineSIAttributesVM();
            _DefineSIAttributes.RequestNavigation += Navigate;
            CurrentViewIsEnabled = true;
            CurrentView = _DefineSIAttributes;
        }

        #endregion
        #region Voids

        private void SelectedPathChanged()
        {
            _DefineSIAttributes.Path = SelectedPath;
            //the selected file has changed. I set the second page to null
            //so that it will grab everything fresh.
            _AttributeLinkingList = null;
        }

        public void PreviousButtonClicked()
        {
            CurrentView = _DefineSIAttributes;
        }

        #region Next Button Click
        private bool ValidateRules(ref string errorMessage)
        {
            bool isValid = true;
            Validate();
            if (HasFatalError)
            {
                isValid = false;
                errorMessage = Error;
            }
            return isValid;
        }

        private bool ValidateTerrainFileExists(ref string errorMessage)
        {
            bool isValid = true;
            if (!_DefineSIAttributes.FirstFloorElevationIsSelected && _DefineSIAttributes.FromTerrainFileIsSelected)
            {
                //then the user wants to use the terrain file to get elevations. Validate that the terrain file exists.
                List<TerrainElement> terrainElements = StudyCache.GetChildElementsOfType<TerrainElement>();
                if (terrainElements.Count == 0)
                {
                    errorMessage = "'From Terrain File' has been selected, but no terrain file exists in the study. Import a terrain file to use this option.";
                    isValid = false;
                }
            }
            return isValid;
        }

        private bool ValidateDefineSIAttributes(ref string errorMessage)
        {            
            //validate the property rules like "Name".
            bool isValid = ValidateRules(ref errorMessage);
            if (isValid)
            {
                //validate that all the required selections have been made.
                isValid = _DefineSIAttributes.ValidateSelectionsMade(ref errorMessage);
                if(isValid)
                {
                    isValid = ValidateTerrainFileExists(ref errorMessage);
                    if (isValid)
                    {
                        //if we are still valid, then check for missing data in the database file.
                        StructuresMissingDataManager missingDataManager = _DefineSIAttributes.Validate(ref errorMessage);
                        if (missingDataManager.GetRows().Count > 0)
                        {
                            StructureMissingElevationEditorVM vm = new StructureMissingElevationEditorVM(missingDataManager.GetRows(), _DefineSIAttributes.FirstFloorElevationIsSelected, _DefineSIAttributes.FromTerrainFileIsSelected);
                            DynamicTabVM tab = new DynamicTabVM("Missing Data", vm, "missingData");
                            Navigate(tab);
                            isValid = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK);
                    }
                }
                else
                {
                    MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK);
            }

            return isValid;
        }

        private void SaveStructureInventory()
        {
            StructureInventoryPersistenceManager manager = PersistenceFactory.GetStructureInventoryManager();

            StructureInventoryLibrary.SharedData.StudyDatabase = new SQLiteManager(Storage.Connection.Instance.ProjectFile);

            LifeSimGIS.ShapefileReader myReader = new LifeSimGIS.ShapefileReader(SelectedPath);

            DataTable newStructureTable = _DefineSIAttributes.CreateStructureTable(SelectedPath, _AttributeLinkingList.Rows);
            //this line will create the child table in the database.
            manager.Save(newStructureTable, Name, myReader.ToFeatures());
            //this line will add it to the parent table.
            Save();
        }


        private void SwitchToAttributeLinkingList()
        {
            if(_AttributeLinkingList == null)
            {
                List<string> occtypes = _DefineSIAttributes.GetUniqueOccupancyTypes();
                _AttributeLinkingList = new AttributeLinkingListVM(occtypes);
            }

            CurrentView = _AttributeLinkingList;
        }

        public bool NextButtonClicked()
        {
            string errorMessage = null;
            //todo: the point of this boolean is so that the code behind and make some changes in the view. Ideally
            //this would get changed to use binding and we could get rid of this boolean.
            bool isValid = true;
            if (CurrentView is DefineSIAttributesVM)
            {
                //Run validation before moving on to the next screen
                isValid = ValidateDefineSIAttributes(ref errorMessage);
                if (isValid)
                {
                    SwitchToAttributeLinkingList();
                }           
            }
            else if (CurrentView is AttributeLinkingListVM)
            {
                isValid = _AttributeLinkingList.AreRowsValid();
                if (isValid)
                {
                    SaveStructureInventory();
                }
            }

            return isValid;
        }


        public void SwitchVMs()
        {
            OccupancyTypesGroupRowItemVM vm = new OccupancyTypesGroupRowItemVM();
            CurrentView = vm;
        }

        /// <summary>
        /// This method saves a new occtype group for this structure.
        /// It also creates the SI element and saves it to the parent table.
        /// </summary>
        public override void Save()
        {
            StructureInventoryBaseElement SIBase = new StructureInventoryBaseElement(Name, Description);
            InventoryElement elementToSave = new InventoryElement(SIBase, false);

            StructureInventoryPersistenceManager manager = PersistenceFactory.GetStructureInventoryManager();
            if (IsImporter && HasSaved == false)
            {
                OccupancyTypesOwnerElement owner = StudyCache.GetParentElementOfType<OccupancyTypesOwnerElement>();

                manager.SaveNew(elementToSave);
                HasSaved = true;
                OriginalElement = elementToSave;
            }
        }

        #endregion
        #endregion
        #region Functions
        #endregion
    }
}
