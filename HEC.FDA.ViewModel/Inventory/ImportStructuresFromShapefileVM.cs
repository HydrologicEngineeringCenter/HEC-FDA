﻿using HEC.FDA.Model.structures;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Watershed;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
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
        private readonly InventoryColumnSelectionsVM _ColumnSelections;
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
        public bool SelectedPathEnabled { get; }
        #endregion
        #region Constructors
        public ImportStructuresFromShapefileVM( EditorActionManager actionManager) :base(actionManager)
        {
            _ColumnSelections = new InventoryColumnSelectionsVM();
            CurrentViewIsEnabled = true;
            CurrentView = _ColumnSelections;
            SelectedPathEnabled = true;
        }

        public ImportStructuresFromShapefileVM(ChildElement elem, EditorActionManager actionManager) : base(elem, actionManager)
        {
            InventoryElement inventoryElement = elem as InventoryElement;
            _SelectedPath = inventoryElement.GetFilePath(".shp");
            _ColumnSelections = new InventoryColumnSelectionsVM(inventoryElement.SelectionMappings, inventoryElement.GetFilePath(".dbf"));
            _OcctypeLinking = new InventoryOcctypeLinkingVM(_SelectedPath, _ColumnSelections.OccupancyTypeRow.SelectedItem, inventoryElement.OcctypeMapping);
            CurrentViewIsEnabled = true;
            CurrentView = _ColumnSelections;
            SelectedPathEnabled = false;
        }

        #endregion

        private void SelectedPathChanged()
        {
            //the selected file has changed. I set the second page to null
            //so that it will grab everything fresh.
            _OcctypeLinking = null;
            string error = "";
            bool validShapefile = RASHelper.ShapefileIsValid(SelectedPath, ref error);
            bool isPoint = RASHelper.IsPointShapefile(SelectedPath, ref error);
            if (!validShapefile || !isPoint)
            {
                MessageBox.Show(error, "Invalid Shapefile", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            } 
            else
            {
                _ColumnSelections.Path = SelectedPath;
            }
        }

        public void PreviousButtonClicked()
        {
            CurrentView = _ColumnSelections;
        }

        private FdaValidationResult ValidateRules()
        {
            FdaValidationResult vr = new();
            Validate();
            if (HasFatalError)
            {
                vr.AddErrorMessage(Error);
            }
            return vr;
        }

        private FdaValidationResult ValidateTerrainFileExists()
        {
            FdaValidationResult vr = new();
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
            FdaValidationResult vr = new();
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
                StructureMissingElevationEditorVM vm = new(missingDataManager);
                DynamicTabVM tab = new("Missing Data", vm, "missingData",false,false);
                Navigate(tab);
                missingValues = true;
            }
            return missingValues;
        }

        private void SwitchToOcctypeLinkingVM()
        {
            _OcctypeLinking ??= new InventoryOcctypeLinkingVM(_SelectedPath,_ColumnSelections.OccupancyTypeRow.SelectedItem);
            //when we switch to the occtype linking vm, we need to check if the user has switched the occtype column name.
            //if it is the same as it was before, then this call won't do anything.
            _OcctypeLinking.UpdateOcctypeColumnSelectionName(_ColumnSelections.OccupancyTypeRow.SelectedItem);
            CurrentView = _OcctypeLinking;
        }

        public bool NextButtonClicked()
        {
            if(Storage.Connection.Instance.ProjectionFile == null && ((TerrainElement)StudyCache.GetParentElementOfType<TerrainOwnerElement>().Elements[0] == null))
            {
                MessageBox.Show("Please set your project projection in the study properties.", "Missing Projection", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
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
                        SwitchToOcctypeLinkingVM();
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
            //the validation before saving is done in the NextButtonClicked() method.
            int id = GetElementID<InventoryElement>();
            StructureSelectionMapping mapping = _ColumnSelections.CreateSelectionMapping();
            Dictionary<string, OcctypeReference> occtypeMappings = _OcctypeLinking.CreateOcctypeMapping();
            InventoryElement elementToSave = new(Name, Description, mapping, occtypeMappings, false, id);

            if (IsCreatingNewElement)
            {
                StudyFilesManager.CopyFilesWithSameName(SelectedPath, Name, elementToSave.GetType());
            }
            else
            {
                StudyFilesManager.RenameDirectory(OriginalElement.Name, Name, elementToSave.GetType());
            }

            Save(elementToSave);
        }

    }
}
