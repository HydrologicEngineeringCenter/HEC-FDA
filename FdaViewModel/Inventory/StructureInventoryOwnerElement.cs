using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using FdaViewModel.Utilities;

namespace FdaViewModel.Inventory
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
        public StructureInventoryOwnerElement(Utilities.ParentElement owner) : base(owner)
        {
            Name = "Structure Inventories";
            IsBold = false;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);

            Utilities.NamedAction addStructureInventory = new Utilities.NamedAction();
            addStructureInventory.Header = "Import From Shapefile...";
            addStructureInventory.Action = AddStructureInventory;

            Utilities.NamedAction addStructureInventoryFromNonGeo = new Utilities.NamedAction();
            addStructureInventoryFromNonGeo.Header = "Import From Non Geo Referenced File...";
            addStructureInventoryFromNonGeo.Action = AddStructureInventory;

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
        public void AddStructureInventory(object arg1, EventArgs arg2)
        {
            //ImportFromShapefileVM vm = new ImportFromShapefileVM();
            // get any point shapefiles from the map window
            //List<string> pointShapePaths = new List<string>();
            //ShapefilePathsOfType(ref pointShapePaths, Utilities.VectorFeatureType.Point);
            ImportStructuresFromShapefileVM vm = new ImportStructuresFromShapefileVM();
            Navigate(vm);
            if (!vm.WasCanceled)
            {
                if (!vm.HasError)
                {
                    //the data has been written to a sql lite file in the "save" method of the vm.

                    //create a "master occtype group" for this structure inv
                    // 1.) create the string name
                    string groupName = vm.Name + " > Occupancy Types";
                    //2.) create the list of occtype 
                    List<Consequences_Assist.ComputableObjects.OccupancyType> newListOfOccType = new List<Consequences_Assist.ComputableObjects.OccupancyType>();
                    List<string> listOfKeys = vm.AttributeLinkingList.OccupancyTypesDictionary.Keys.ToList();
                    for(int i = 0;i<listOfKeys.Count;i++)
                    {
                        Consequences_Assist.ComputableObjects.OccupancyType ot = new Consequences_Assist.ComputableObjects.OccupancyType();
                        if(vm.AttributeLinkingList.OccupancyTypesDictionary[listOfKeys[i]] != "")
                        {
                            //find the chosen occtype and replace the name with the name from the file
                            string[] occtypeAndGroupName = new string[2];
                            occtypeAndGroupName = vm.AttributeLinkingList.ParseOccTypeNameAndGroupNameFromCombinedString(vm.AttributeLinkingList.OccupancyTypesDictionary[listOfKeys[i]]);
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

                    Dictionary<string, bool[]> _OcctypeTabsSelectedDictionary = new Dictionary<string, bool[]>();

                    foreach (Consequences_Assist.ComputableObjects.OccupancyType ot in newListOfOccType)
                    {
                        bool[] tabsCheckedArray = new bool[] { true, true, true, false };
                        _OcctypeTabsSelectedDictionary.Add(ot.Name, tabsCheckedArray);

                    }

                    OccupancyTypes.OccupancyTypesElement newOccTypeGroup = new OccupancyTypes.OccupancyTypesElement(groupName, newListOfOccType,_OcctypeTabsSelectedDictionary);
                    OccupancyTypes.OccupancyTypesOwnerElement.ListOfOccupancyTypesGroups.Add(newOccTypeGroup);

                    // i think this is the place i should create the SI base object;
                    StructureInventoryBaseElement SIBase = new StructureInventoryBaseElement(vm.Name,vm.Description); 
                    InventoryElement ele = new InventoryElement(SIBase);
                    AddElement(ele);
                }
            }
        }

        
        private Consequences_Assist.ComputableObjects.OccupancyType GetOcctypeFromGroup(string occtypeName ,string groupName)
        {
            foreach (OccupancyTypes.OccupancyTypesElement group in OccupancyTypes.OccupancyTypesOwnerElement.ListOfOccupancyTypesGroups)
            {
                if (group.Name == groupName)
                {
                    foreach (Consequences_Assist.ComputableObjects.OccupancyType ot in group.ListOfOccupancyTypes)
                    {
                        if (ot.Name == occtypeName)
                        {
                            return ot;
                        }
                    }
                }
            }
            return new Consequences_Assist.ComputableObjects.OccupancyType(); // if it gets here then no occtype matching the names given exists. Should we send an error message?
        }

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
