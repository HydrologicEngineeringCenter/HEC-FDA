using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ViewModel.Utilities;

namespace ViewModel.Inventory
{
    //[Author(q0heccdm, 6 / 14 / 2017 3:38:41 PM)]
    public class StructureInventoryOwnerElement : Utilities.ParentElement
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
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);

            Utilities.NamedAction addStructureInventory = new Utilities.NamedAction();
            addStructureInventory.Header = "Import From Shapefile...";
            addStructureInventory.Action = AddStructureInventory;

            Utilities.NamedAction addStructureInventoryFromNonGeo = new Utilities.NamedAction();
            addStructureInventoryFromNonGeo.Header = "Import From Fda Version 1...";
            addStructureInventoryFromNonGeo.Action = ImportStructuresFromFDA1;

            //Utilities.NamedAction ImportFromAscii = new Utilities.NamedAction();
            //ImportFromAscii.Header = "Import Exterior Interior Relationship From ASCII";
            //ImportFromAscii.Action = ImportFromASCII;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(addStructureInventory);
            localActions.Add(addStructureInventoryFromNonGeo);

            //localActions.Add(ImportFromAscii);

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
            //ImportFromShapefileVM vm = new ImportFromShapefileVM();
            // get any point shapefiles from the map window
            //List<string> pointShapePaths = new List<string>();
            //ShapefilePathsOfType(ref pointShapePaths, Utilities.VectorFeatureType.Point);

            //get the list of paths that exist in the map window
            ObservableCollection<string> collectionOfPointFiles = new ObservableCollection<string>();
            List<string> pointShapePaths = new List<string>();
            ShapefilePathsOfType(ref pointShapePaths, Utilities.VectorFeatureType.Point);
            foreach (string path in pointShapePaths)
            {
                collectionOfPointFiles.Add(path);
            }

            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                 .WithSiblingRules(this);

            
            ImportStructuresFromShapefileVM vm = new ImportStructuresFromShapefileVM(collectionOfPointFiles, actionManager, false);
            string header = "Import Structure Inventory";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportStructureInventory");
            Navigate(tab, false, false);

        }


        //private Consequences_Assist.ComputableObjects.OccupancyType GetOcctypeFromGroup(string occtypeName, string groupName)
        //{
        //    foreach (OccupancyTypes.OccupancyTypesElement group in OccupancyTypes.OccupancyTypesOwnerElement.ListOfOccupancyTypesGroups)
        //    {
        //        if (group.Name == groupName)
        //        {
        //            foreach (Consequences_Assist.ComputableObjects.OccupancyType ot in group.ListOfOccupancyTypes)
        //            {
        //                if (ot.Name == occtypeName)
        //                {
        //                    return ot;
        //                }
        //            }
        //        }
        //    }
        //    return new Consequences_Assist.ComputableObjects.OccupancyType(); // if it gets here then no occtype matching the names given exists. Should we send an error message?
        //}

        #endregion
        #region Functions
        #endregion




        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        

        //public override string[] TableColumnNames()
        //{
        //    return new string[] { "Name" , "Description"};
        //}

        //public override Type[] TableColumnTypes()
        //{
        //    return new Type[] { typeof(string),typeof(string) };
        //}

        //public override ChildElement CreateElementFromRowData(object[] rowData)
        //{
        //    //name, path, description
        //    if (StructureInventoryLibrary.SharedData.StudyDatabase == null)
        //    {
        //        StructureInventoryLibrary.SharedData.StudyDatabase = new DataBase_Reader.SqLiteReader(Storage.Connection.Instance.ProjectFile);


        //    }
        //    StructureInventoryBaseElement baseElement = new StructureInventoryBaseElement((string)rowData[0], (string)rowData[1]);

        //    InventoryElement invEle = new InventoryElement(baseElement, this);
        //    return invEle;
        //}
        //public override void AddElementFromRowData(object[] rowData)
        //{
        //    AddElement(CreateElementFromRowData(rowData),false);
        //}
    }
}
