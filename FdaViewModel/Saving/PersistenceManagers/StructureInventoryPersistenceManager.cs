using FdaViewModel.Inventory;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class StructureInventoryPersistenceManager : SavingBase, IElementManager
    {
        //ELEMENT_TYPE is used to store the type in the log tables. Initially i was actually storing the type
        //of the element. But since they get stored as strings if a developer changes the name of the class
        //you would no longer get any of the old logs. So i use this constant.
        private const string ELEMENT_TYPE = "Structure_Inventory";
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("StructureInventoryPersistenceManager");


        private const string TABLE_NAME = "Structure Inventories";
        internal override string ChangeTableConstant { get { return "Structure Inventory - "; } }

        public override string TableName
        {
            get { return TABLE_NAME; }
        }

        public override string[] TableColumnNames => throw new NotImplementedException();

        private static readonly string[] TableColNames = { "Name", "Description" };
        private static readonly Type[] TableColTypes = { typeof(string), typeof(string) };
        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return TableColTypes; }
        }


        public StructureInventoryPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }


        #region utilities
        private object[] GetRowDataFromElement(InventoryElement element)
        {
            return new object[] { element.Name, element.Description };

        }
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            return null;
            //name, path, description
            //if (StructureInventoryLibrary.SharedData.StudyDatabase == null)
            //{
            //    StructureInventoryLibrary.SharedData.StudyDatabase = new DataBase_Reader.SqLiteReader(Storage.Connection.Instance.ProjectFile);
            //}
            //StructureInventoryBaseElement baseElement = new StructureInventoryBaseElement((string)rowData[0], (string)rowData[1]);

            //InventoryElement invEle = new InventoryElement(baseElement);
            //return invEle;
        }
        #endregion


        public void Remove(ChildElement element)
        {
            RemoveFromParentTable(element, TableName);
            StudyCacheForSaving.RemoveElement((InventoryElement)element);

        }



        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(InventoryElement))
            {

                SaveNewElementToParentTable(GetRowDataFromElement((InventoryElement)element), TableName, TableColumnNames, TableColumnTypes);
                //WriteAttributeTable(((InventoryElement)element).DefineSIAttributes, ((InventoryElement)element).AttributeLinkingList, ((InventoryElement)element).DefineSIAttributes.Path);
                StudyCacheForSaving.AddElement((InventoryElement)element);
            }
        }

       

        public void Load()
        {
            List<Utilities.ChildElement> structures = CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (InventoryElement elem in structures)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }


        /// <summary>
        /// Right now there is no way to edit structs other than through the map window. This call only works for "rename". It will not 
        /// update any of the structs falues, only its name
        /// </summary>
        /// <param name="oldElement"></param>
        /// <param name="elementToSave"></param>
        /// <param name="changeTableIndex"></param>
        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex )
        {
            if (DidParentTableRowValuesChange(elementToSave, GetRowDataFromElement((InventoryElement)elementToSave), oldElement.Name, TableName) )
            {
                UpdateParentTableRow(elementToSave.Name, changeTableIndex, GetRowDataFromElement((InventoryElement)elementToSave), oldElement.Name, TableName, false, ChangeTableConstant);
                string childTable = ChangeTableConstant + oldElement.Name;
                string newChildTableName = ChangeTableConstant + elementToSave.Name;
                if(Storage.Connection.Instance.TableNames().Contains(childTable))
                {
                    Storage.Connection.Instance.RenameTable(childTable, newChildTableName);
                }
                //SaveCurveTable(elementToSave.Curve, ChangeTableConstant, editDate);
                // update the existing element. This will actually remove the old element and do an insert at that location with the new element.
                StudyCacheForSaving.UpdateStructureInventoryElement((InventoryElement)oldElement, (InventoryElement)elementToSave);
            }
        }





        public void WriteAttributeTable(DefineSIAttributesVM _DefineSIAttributes, AttributeLinkingListVM _AttributeLinkingList, string SelectedPath)
        {
            // DataBase_Reader.SqLiteReader.CreateSqLiteFile(System.IO.Path.GetDirectoryName(SelectedPath) + "\\codyTest.sqlite");
           // StructureInventoryLibrary.SharedData.StudyDatabase = new DataBase_Reader.SqLiteReader(Storage.Connection.Instance.ProjectFile);

            LifeSimGIS.ShapefileReader myReader = new LifeSimGIS.ShapefileReader(SelectedPath);

            //create the data table that will get written out
            System.Data.DataTable myAttributeTable = new System.Data.DataTable(Name);

            myAttributeTable.Columns.Add(StructureInventoryBaseElement.OccupancyTypeField, typeof(string));
            myAttributeTable.Columns.Add(StructureInventoryBaseElement.OccupancyTypeGroupName, typeof(string));
            myAttributeTable.Columns.Add(StructureInventoryBaseElement.FoundationHeightField, typeof(string));
            myAttributeTable.Columns.Add(StructureInventoryBaseElement.StructureValueField, typeof(string));
            myAttributeTable.Columns.Add(StructureInventoryBaseElement.ContentValueField, typeof(string));
            myAttributeTable.Columns.Add(StructureInventoryBaseElement.OtherValueField, typeof(string));
            myAttributeTable.Columns.Add(StructureInventoryBaseElement.VehicleValueField, typeof(string));
            myAttributeTable.Columns.Add(StructureInventoryBaseElement.FirstFloorElevationField, typeof(string));
            myAttributeTable.Columns.Add(StructureInventoryBaseElement.GroundElevationField, typeof(string));
            myAttributeTable.Columns.Add(StructureInventoryBaseElement.YearField, typeof(string));
            myAttributeTable.Columns.Add(StructureInventoryBaseElement.ModuleField, typeof(string));


            DatabaseManager.DataTableView attributeTable = myReader.GetAttributeTable();
            if (attributeTable.ParentDatabase.DataBaseOpen == false)
            {
                attributeTable.ParentDatabase.Open();
            }
            System.Data.DataRow nextRow;

            for (int i = 0; i < attributeTable.NumberOfRows; i++)
            {
                nextRow = myAttributeTable.NewRow();


                if (_DefineSIAttributes.OccupancyTypeIsUsingDefault == false)
                {
                    //string groupName = "";
                    string occTypeKey = attributeTable.GetCell(_DefineSIAttributes.OccupancyTypeColumnName, i).ToString(); // this is the old occtype from the dbf of the struc inv.
                    string combinedNewOccTypeAndGroupName = _AttributeLinkingList.OccupancyTypesDictionary[occTypeKey];
                    //deal with the case that no selection was made so the new occtype would be blank
                    if (combinedNewOccTypeAndGroupName == "" || combinedNewOccTypeAndGroupName == null)
                    {
                        nextRow[StructureInventoryBaseElement.OccupancyTypeField] = occTypeKey;
                        nextRow[StructureInventoryBaseElement.OccupancyTypeGroupName] = "Undefined";

                    }
                    else
                    {
                        string[] occtypeAndGroupName = _AttributeLinkingList.ParseOccTypeNameAndGroupNameFromCombinedString(combinedNewOccTypeAndGroupName);
                        nextRow[StructureInventoryBaseElement.OccupancyTypeField] = occtypeAndGroupName[0];
                        nextRow[StructureInventoryBaseElement.OccupancyTypeGroupName] = occtypeAndGroupName[1];
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
                        nextRow[StructureInventoryBaseElement.OccupancyTypeField] = occTypeKey;
                        nextRow[StructureInventoryBaseElement.OccupancyTypeGroupName] = "Undefined";

                    }
                    else
                    {
                        string[] occtypeAndGroupName = _AttributeLinkingList.ParseOccTypeNameAndGroupNameFromCombinedString(combinedNewOccTypeAndGroupName);

                        nextRow[StructureInventoryBaseElement.OccupancyTypeField] = occtypeAndGroupName[0];
                        nextRow[StructureInventoryBaseElement.OccupancyTypeGroupName] = occtypeAndGroupName[1];
                    }


                }



                if (_DefineSIAttributes.FirstFloorElevationIsChecked == true)
                {
                    if (_DefineSIAttributes.FirstFloorElevationIsUsingDefault == false)
                    {
                        nextRow[StructureInventoryBaseElement.FirstFloorElevationField] = attributeTable.GetCell(_DefineSIAttributes.FirstFloorElevationColumnName, i);
                    }
                    else
                    {
                        nextRow[StructureInventoryBaseElement.FirstFloorElevationField] = _DefineSIAttributes.FirstFloorElevationDefaultValue;
                    }
                    nextRow[StructureInventoryBaseElement.FoundationHeightField] = 0;
                    nextRow[StructureInventoryBaseElement.GroundElevationField] = 0;


                }
                else
                {

                    if (_DefineSIAttributes.FoundationHeightIsUsingDefault == false)
                    {
                        nextRow[StructureInventoryBaseElement.FoundationHeightField] = attributeTable.GetCell(_DefineSIAttributes.FoundationHeightColumnName, i);
                    }
                    else
                    {
                        nextRow[StructureInventoryBaseElement.FoundationHeightField] = _DefineSIAttributes.FoundationHeightDefaultValue;
                    }

                    if (_DefineSIAttributes.GroundElevationIsUsingDefault == false)
                    {
                        nextRow[StructureInventoryBaseElement.GroundElevationField] = attributeTable.GetCell(_DefineSIAttributes.GroundElevationColumnName, i);
                    }
                    else
                    {
                        nextRow[StructureInventoryBaseElement.GroundElevationField] = _DefineSIAttributes.GroundElevationDefaultValue;
                    }


                    nextRow[StructureInventoryBaseElement.FirstFloorElevationField] = 0;

                }



                if (_DefineSIAttributes.StructureValueIsUsingDefault == false)
                {
                    nextRow[StructureInventoryBaseElement.StructureValueField] = attributeTable.GetCell(_DefineSIAttributes.StructureValueColumnName, i);
                }
                else
                {
                    nextRow[StructureInventoryBaseElement.StructureValueField] = _DefineSIAttributes.StructureValueDefaultValue;
                }


                if (_DefineSIAttributes.ContentValueIsUsingDefault == false)
                {
                    if (_DefineSIAttributes.ContentValueColumnName != null)
                    {
                        nextRow[StructureInventoryBaseElement.ContentValueField] = attributeTable.GetCell(_DefineSIAttributes.ContentValueColumnName, i);
                    }
                    else
                    {
                        nextRow[StructureInventoryBaseElement.ContentValueField] = "0";
                    }
                }
                else
                {
                    nextRow[StructureInventoryBaseElement.ContentValueField] = _DefineSIAttributes.ContentValueDefaultValue;
                }

                if (_DefineSIAttributes.OtherValueIsUsingDefault == false)
                {
                    if (_DefineSIAttributes.OtherValueColumnName != null)
                    {
                        nextRow[StructureInventoryBaseElement.OtherValueField] = attributeTable.GetCell(_DefineSIAttributes.OtherValueColumnName, i);

                    }
                    else
                    {
                        nextRow[StructureInventoryBaseElement.OtherValueField] = "0";
                    }
                }
                else
                {
                    nextRow[StructureInventoryBaseElement.OtherValueField] = _DefineSIAttributes.OtherValueDefaultValue;
                }

                if (_DefineSIAttributes.VehicleValueIsUsingDefault == false)
                {
                    if (_DefineSIAttributes.VehicleValueColumnName != null)
                    {
                        nextRow[StructureInventoryBaseElement.VehicleValueField] = attributeTable.GetCell(_DefineSIAttributes.VehicleValueColumnName, i);

                    }
                    else
                    {
                        nextRow[StructureInventoryBaseElement.VehicleValueField] = "0";
                    }
                }
                else
                {
                    nextRow[StructureInventoryBaseElement.VehicleValueField] = _DefineSIAttributes.VehicleValueDefaultValue;
                }

                if (_DefineSIAttributes.YearIsUsingDefault == false)
                {
                    if (_DefineSIAttributes.YearColumnName != null)
                    {
                        nextRow[StructureInventoryBaseElement.YearField] = attributeTable.GetCell(_DefineSIAttributes.YearColumnName, i);

                    }
                    else
                    {
                        nextRow[StructureInventoryBaseElement.YearField] = "0";
                    }
                }
                else
                {
                    nextRow[StructureInventoryBaseElement.YearField] = _DefineSIAttributes.YearDefaultValue;
                }

                if (_DefineSIAttributes.ModuleIsUsingDefault == false)
                {
                    if (_DefineSIAttributes.ModuleColumnName != null)
                    {
                        nextRow[StructureInventoryBaseElement.ModuleField] = attributeTable.GetCell(_DefineSIAttributes.ModuleColumnName, i);

                    }
                    else
                    {
                        nextRow[StructureInventoryBaseElement.ModuleField] = "0";
                    }
                }
                else
                {
                    nextRow[StructureInventoryBaseElement.ModuleField] = _DefineSIAttributes.ModuleDefaultValue;
                }


                myAttributeTable.Rows.Add(nextRow);
            }

            attributeTable.ParentDatabase.Close();

            //create an in memory reader and data table view

            DatabaseManager.InMemoryReader myInMemoryReader = new DatabaseManager.InMemoryReader(myAttributeTable);
            DatabaseManager.DataTableView myDTView = myInMemoryReader.GetTableManager(Name);

            //create the geo package writer that will write the data out
           // LifeSimGIS.GeoPackageWriter myGeoPackWriter = new LifeSimGIS.GeoPackageWriter(StructureInventoryLibrary.SharedData.StudyDatabase);

            // write the data out
            //myGeoPackWriter.AddFeatures(Name, myReader.ToFeatures(), myReader.GetAttributeTable());
           // myGeoPackWriter.AddFeatures("Structure Inventory - " + Name, myReader.ToFeatures(), myDTView);
        }

        public ObservableCollection<FdaLogging.LogItem> GetLogMessages(ChildElement element)
        {
            return new ObservableCollection<FdaLogging.LogItem>();
        }

        /// <summary>
        /// This will put a log into the log tables. Logs are only unique by element id and
        /// element type. ie. Rating Curve id=3.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="elementName"></param>
        public void Log(FdaLogging.LoggingLevel level, string message, string elementName)
        {
            int elementId = GetElementId(TableName, elementName);
            LOGGER.Log(level, message, ELEMENT_TYPE, elementId);
        }

        /// <summary>
        /// This will look in the parent table for the element id using the element name. 
        /// Then it will sweep through the log tables pulling out any logs with that id
        /// and element type. 
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public ObservableCollection<FdaLogging.LogItem> GetLogMessages(string elementName)
        {
            int id = GetElementId(TableName, elementName);
            return FdaLogging.RetrieveFromDB.GetLogMessages(id, ELEMENT_TYPE);
        }
        /// <summary>
        /// Gets all the log messages for this element from the specified log level table.
        /// This is used by the MessageExpander to filter by log level
        /// </summary>
        /// <param name="level"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public ObservableCollection<FdaLogging.LogItem> GetLogMessagesByLevel(FdaLogging.LoggingLevel level, string elementName)
        {
            int id = GetElementId(TableName, elementName);
            return FdaLogging.RetrieveFromDB.GetLogMessagesByLevel(level, id, ELEMENT_TYPE);
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            throw new NotImplementedException();
        }
    }
}
