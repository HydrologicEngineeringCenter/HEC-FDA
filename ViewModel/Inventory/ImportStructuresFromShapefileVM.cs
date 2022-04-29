using DatabaseManager;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Watershed;
using HEC.FDA.ViewModel.Saving;

namespace HEC.FDA.ViewModel.Inventory
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

        #endregion
        #region Constructors
        public ImportStructuresFromShapefileVM( EditorActionManager actionManager) :base(actionManager)
        {
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
        private FdaValidationResult ValidateRules()
        {
            FdaValidationResult vr = new FdaValidationResult();
            Validate();
            if (HasErrors)
            {
                //vr.AddErrorMessage( Error);
            }
            return vr;
        }

        private FdaValidationResult ValidateTerrainFileExists()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (!_DefineSIAttributes.FirstFloorElevationIsSelected && _DefineSIAttributes.FromTerrainFileIsSelected)
            {
                //then the user wants to use the terrain file to get elevations. Validate that the terrain file exists.
                List<TerrainElement> terrainElements = StudyCache.GetChildElementsOfType<TerrainElement>();
                if (terrainElements.Count == 0)
                {
                    vr.AddErrorMessage("'From Terrain File' has been selected, but no terrain file exists in the study. Import a terrain file to use this option.");
                }
            }
            return vr;
        }

        private FdaValidationResult ValidateDefineSIAttributes()
        {
            FdaValidationResult vr = new FdaValidationResult();
            //validate the property rules like "Name".
            FdaValidationResult rulesValid = ValidateRules();
            if (!rulesValid.IsValid)
            {
                vr.AddErrorMessage(rulesValid.ErrorMessage);
            }

            //validate that all the required selections have been made.
            FdaValidationResult selectionsResult = _DefineSIAttributes.ValidateSelectionsMade();
            if (!selectionsResult.IsValid)
            {
                vr.AddErrorMessage(selectionsResult.ErrorMessage);
            }

            FdaValidationResult terrainValidation = ValidateTerrainFileExists();
            if (!terrainValidation.IsValid)
            {
                vr.AddErrorMessage(terrainValidation.ErrorMessage);
            }            

            return vr;
        }

        private bool CheckForMissingValues()
        {
            bool missingValues = false;
            StructuresMissingDataManager missingDataManager = _DefineSIAttributes.Validate();
            if (missingDataManager.GetRows().Count > 0)
            {
                StructureMissingElevationEditorVM vm = new StructureMissingElevationEditorVM(missingDataManager.GetRows(), _DefineSIAttributes.FirstFloorElevationIsSelected, _DefineSIAttributes.FromTerrainFileIsSelected);
                DynamicTabVM tab = new DynamicTabVM("Missing Data", vm, "missingData");
                Navigate(tab);
                missingValues = true;
            }
            return missingValues;
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
            bool isValid = false;
            //todo: the point of this boolean is so that the code behind and make some changes in the view. Ideally
            //this would get changed to use binding and we could get rid of this boolean.
            if (CurrentView is DefineSIAttributesVM)
            {
                //Run validation before moving on to the next screen
                FdaValidationResult defineSIResults = ValidateDefineSIAttributes();
                if (defineSIResults.IsValid)
                {
                    bool missingValues = CheckForMissingValues();
                    if (!missingValues)
                    {
                        SwitchToAttributeLinkingList();
                        isValid = true;
                    }
                }  
                else
                {
                    MessageBox.Show(defineSIResults.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (CurrentView is AttributeLinkingListVM)
            {
                FdaValidationResult rowsValidResult = _AttributeLinkingList.AreRowsValid();
                if (rowsValidResult.IsValid)
                {
                    SaveStructureInventory();
                    isValid = true;
                }
                else
                {
                    MessageBox.Show(rowsValidResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return isValid;
        }

        /// <summary>
        /// This method saves a new occtype group for this structure.
        /// It also creates the SI element and saves it to the parent table.
        /// </summary>
        public override void Save()
        {
            StructureInventoryBaseElement SIBase = new StructureInventoryBaseElement(Name, Description);
            int id = PersistenceFactory.GetStructureInventoryManager().GetNextAvailableId();
            InventoryElement elementToSave = new InventoryElement(SIBase, false, id);

            StructureInventoryPersistenceManager manager = PersistenceFactory.GetStructureInventoryManager();
            if (IsCreatingNewElement && HasSaved == false)
            {
                OccupancyTypesOwnerElement owner = StudyCache.GetParentElementOfType<OccupancyTypesOwnerElement>();

                manager.SaveNew(elementToSave);
                HasSaved = true;
                OriginalElement = elementToSave;
            }
        }

        #endregion
        #endregion
    }
}
