using DatabaseManager;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Watershed;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;

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
        private InventoryColumnSelectionsVM _ColumnSelections;
        private InventoryOcctypeLinkingVM _OcctypeLinking;
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
            _ColumnSelections = new InventoryColumnSelectionsVM();
            _ColumnSelections.RequestNavigation += Navigate;
            CurrentViewIsEnabled = true;
            CurrentView = _ColumnSelections;
        }

        #endregion
        #region Voids
        private void SelectedPathChanged()
        {
            //the selected file has changed. I set the second page to null
            //so that it will grab everything fresh.
            _OcctypeLinking = null;
            if (File.Exists(Path.ChangeExtension(SelectedPath, "dbf")))
            {
                _ColumnSelections.Path = SelectedPath;
            }
        }

        public void PreviousButtonClicked()
        {
            CurrentView = _ColumnSelections;
        }

        #region Next Button Click
        private FdaValidationResult ValidateRules()
        {
            FdaValidationResult vr = new FdaValidationResult();
            Validate();
            if (HasFatalError)
            {
                vr.AddErrorMessage(Error);
            }
            return vr;
        }

        private FdaValidationResult ValidateTerrainFileExists()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (!_ColumnSelections.FirstFloorElevationIsSelected && _ColumnSelections.FromTerrainFileIsSelected)
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
            FdaValidationResult selectionsResult = _ColumnSelections.ValidateSelectionsMade();
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
            StructuresMissingDataManager missingDataManager = _ColumnSelections.Validate();
            if (missingDataManager.GetRows().Count > 0)
            {
                StructureMissingElevationEditorVM vm = new StructureMissingElevationEditorVM(missingDataManager.GetRows(), _ColumnSelections.FirstFloorElevationIsSelected, _ColumnSelections.FromTerrainFileIsSelected);
                DynamicTabVM tab = new DynamicTabVM("Missing Data", vm, "missingData");
                Navigate(tab);
                missingValues = true;
            }
            return missingValues;
        }


        private void SwitchToAttributeLinkingList()
        {
            if(_OcctypeLinking == null)
            {
                List<string> occtypes = _ColumnSelections.GetUniqueOccupancyTypes();
                _OcctypeLinking = new InventoryOcctypeLinkingVM(occtypes);
            }

            CurrentView = _OcctypeLinking;
        }

        public bool NextButtonClicked()
        {
            bool isValid = false;
            if (CurrentView is InventoryColumnSelectionsVM)
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
            else if (CurrentView is InventoryOcctypeLinkingVM)
            {
                FdaValidationResult rowsValidResult = _OcctypeLinking.AreRowsValid();
                if (rowsValidResult.IsValid)
                {
                    Save();
                    isValid = true;
                }
                else
                {
                    MessageBox.Show(rowsValidResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return isValid;
        }


        public override void Save()
        {
            //copy shapefile
            //create mapping object and pass into the ctor of the inventory element, then write it all to xml

            StructureInventoryPersistenceManager manager = PersistenceFactory.GetStructureInventoryManager();
            int id = manager.GetNextAvailableId();

            InventorySelectionMapping mapping = new InventorySelectionMapping(_ColumnSelections, _OcctypeLinking.CreateOcctypeMapping());
            InventoryElement elementToSave = new InventoryElement(Name, Description, mapping, false, id);
            Save(elementToSave);

        }

        #endregion
        #endregion
    }
}
