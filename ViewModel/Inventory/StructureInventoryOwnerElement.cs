using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using ViewModel.Inventory.OccupancyTypes;
using ViewModel.Utilities;

namespace ViewModel.Inventory
{
    public class StructureInventoryOwnerElement : ParentElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/14/2017 3:38:41 PM
        #endregion
        #region Fields
        #endregion
        #region Properties
        
        #endregion
        #region Constructors
        public StructureInventoryOwnerElement( ) : base()
        {
            Name = "Structure Inventories";
            IsBold = false;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addStructureInventory = new NamedAction();
            addStructureInventory.Header = "Import From Shapefile...";
            addStructureInventory.Action = AddStructureInventory;

            NamedAction addStructureInventoryFromNonGeo = new NamedAction();
            addStructureInventoryFromNonGeo.Header = "Import From Fda Version 1...";
            addStructureInventoryFromNonGeo.Action = ImportStructuresFromFDA1;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addStructureInventory);
            localActions.Add(addStructureInventoryFromNonGeo);

            Actions = localActions;

            StudyCache.StructureInventoryAdded += AddStructureInventoryElement;
            StudyCache.StructureInventoryRemoved += RemoveStructureInventoryElement;
            StudyCache.StructureInventoryUpdated += UpdateStructureInventoryElement;
        }
        #endregion
        #region Voids
        private void UpdateStructureInventoryElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }

        private void RemoveStructureInventoryElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        private void AddStructureInventoryElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }

        public void ImportStructuresFromFDA1(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                 .WithSiblingRules(this);

            ImportStructuresFromFDA1VM vm = new ImportStructuresFromFDA1VM( actionManager);
            string header = "Import Structure Inventory";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportStructureInventoryFromFDA1");
            Navigate(tab, false, false);
        }
        public void AddStructureInventory(object arg1, EventArgs arg2)
        {
            List<OccupancyTypesElement> occtypeElems = StudyCache.GetChildElementsOfType<OccupancyTypesElement>();
            if (occtypeElems.Count == 0)
            {
                MessageBox.Show("Occupancy types must be imported before importing structure inventories.", "Missing Occtypes", MessageBoxButton.OK);
                return;
            }

            //get the list of paths that exist in the map window
            ObservableCollection<string> collectionOfPointFiles = new ObservableCollection<string>();
            List<string> pointShapePaths = new List<string>();
            ShapefilePathsOfType(ref pointShapePaths, VectorFeatureType.Point);
            foreach (string path in pointShapePaths)
            {
                collectionOfPointFiles.Add(path);
            }

            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                 .WithSiblingRules(this);

            ImportStructuresFromShapefileVM vm = new ImportStructuresFromShapefileVM(collectionOfPointFiles, actionManager);
            vm.RequestNavigation += Navigate;
            
            string header = "Import Structure Inventory";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportStructureInventory");
            Navigate(tab, false, false);
        }

        #endregion
        #region Functions
        #endregion   

    }
}
