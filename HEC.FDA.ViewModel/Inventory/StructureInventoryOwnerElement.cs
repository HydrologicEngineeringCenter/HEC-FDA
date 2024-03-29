﻿using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Windows;

namespace HEC.FDA.ViewModel.Inventory
{
    public class StructureInventoryOwnerElement : ParentElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/14/2017 3:38:41 PM
        #endregion

        #region Constructors
        public StructureInventoryOwnerElement( ) : base()
        {
            Name = StringConstants.STRUCTURE_INVENTORIES;
            IsBold = false;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addStructureInventory = new NamedAction();
            addStructureInventory.Header = StringConstants.IMPORT_STRUCTURE_INVENTORIES_MENU;
            addStructureInventory.Action = AddStructureInventory;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addStructureInventory);
            Actions = localActions;

            StudyCache.StructureInventoryAdded += AddStructureInventoryElement;
            StudyCache.StructureInventoryRemoved += RemoveStructureInventoryElement;
            StudyCache.StructureInventoryUpdated += UpdateStructureInventoryElement;
        }
        #endregion
        #region Voids
        private void UpdateStructureInventoryElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement( e.NewElement);
        }

        private void RemoveStructureInventoryElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        private void AddStructureInventoryElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }

        public void AddStructureInventory(object arg1, EventArgs arg2)
        {
            List<OccupancyTypesElement> occtypeElems = StudyCache.GetChildElementsOfType<OccupancyTypesElement>();
            if (occtypeElems.Count == 0)
            {
                MessageBox.Show("Occupancy types must be imported before importing structure inventories.", "Missing Occtypes", MessageBoxButton.OK);
                return;
            }

            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                 .WithSiblingRules(this);

            ImportStructuresFromShapefileVM vm = new ImportStructuresFromShapefileVM(actionManager);
            vm.RequestNavigation += Navigate;
            
            string header = StringConstants.IMPORT_STRUCTURE_INVENTORIES_HEADER;
            DynamicTabVM tab = new DynamicTabVM(header, vm, StringConstants.IMPORT_STRUCTURE_INVENTORIES_HEADER);
            Navigate(tab, false, false);
        }

        #endregion 

    }
}
