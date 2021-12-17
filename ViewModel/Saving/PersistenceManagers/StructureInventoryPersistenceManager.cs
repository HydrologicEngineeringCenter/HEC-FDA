using DatabaseManager;
using ViewModel.Inventory;
using ViewModel.Inventory.OccupancyTypes;
using ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Storage;

namespace ViewModel.Saving.PersistenceManagers
{
    public class StructureInventoryPersistenceManager : SavingBase, IElementManager
    {
        public static readonly string STRUCTURE_INVENTORY_TABLE_CONSTANT = "structure_inventory_";

        public static readonly string STRUCTURE_ID = "StructureID";
        public static readonly string BEG_DAM_DEPTH = "BeginningDamageDepth";
        public static readonly string YEAR_IN_CONSTRUCTION = "YearInConstruction";
        public static readonly string NOTES = "Notes";
        public static readonly string OTHER = "Other";

        private const int ID_COL = 0;
        private const int NAME_COL = 1;
        private const int DESC_COL = 2;
        private const int IS_OLD_FDA = 3;

        //ELEMENT_TYPE is used to store the type in the log tables. Initially i was actually storing the type
        //of the element. But since they get stored as strings if a developer changes the name of the class
        //you would no longer get any of the old logs. So i use this constant.
        private const string ELEMENT_TYPE = "structure_inventory";
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("StructureInventoryPersistenceManager");


        private const string TABLE_NAME = "structure_inventories";
        internal override string ChangeTableConstant
        {
            get { return STRUCTURE_INVENTORY_TABLE_CONSTANT; }
        }

        public override string TableName
        {
            get { return TABLE_NAME; }
        }

        public override string[] TableColumnNames
        {
            get
            {
                return TableColNames;
            }
        }

        private static readonly string[] TableColNames = { NAME, DESCRIPTION, "is_import_from_oldFDA" };
        private static readonly Type[] TableColTypes = { typeof(string), typeof(string), typeof(string) };
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

        private string[] ChildTableColumns = new string[] {
        StructureInventoryBaseElement.fidField,
        StructureInventoryBaseElement.geomField,
        StructureInventoryBaseElement.OccupancyTypeField,
        StructureInventoryBaseElement.damCatField,
        StructureInventoryBaseElement.OccupancyTypeGroupName,
        StructureInventoryBaseElement.FoundationHeightField,
        StructureInventoryBaseElement.StructureValueField,
        StructureInventoryBaseElement.ContentValueField,
        StructureInventoryBaseElement.OtherValueField,
        StructureInventoryBaseElement.VehicleValueField,
        StructureInventoryBaseElement.FirstFloorElevationField,
        StructureInventoryBaseElement.GroundElevationField,
        StructureInventoryBaseElement.YearField,
        StructureInventoryBaseElement.ModuleField
        };

        private Type[] ChildTableTypes = new Type[] {typeof(int), typeof(string),
        typeof(string),
        typeof(string),
        typeof(string),
        typeof(double),
        typeof(double),
        typeof(double),
        typeof(double),
        typeof(double),
        typeof(double),
        typeof(double),
        typeof(int),
        typeof(string)
        };



        #region utilities
        public DataTable CreateEmptyStructuresTable()
        {
            DataTable newStructureTable = new DataTable(Name);

            newStructureTable.Columns.Add(STRUCTURE_ID, typeof(string));

            newStructureTable.Columns.Add(StructureInventoryBaseElement.fidField, typeof(string));
            newStructureTable.Columns.Add(StructureInventoryBaseElement.geomField, typeof(string));

            newStructureTable.Columns.Add(StructureInventoryBaseElement.OccupancyTypeField, typeof(string));
            newStructureTable.Columns.Add(StructureInventoryBaseElement.damCatField, typeof(string));

            newStructureTable.Columns.Add(StructureInventoryBaseElement.OccupancyTypeGroupName, typeof(string));
            newStructureTable.Columns.Add(StructureInventoryBaseElement.FoundationHeightField, typeof(string));
            newStructureTable.Columns.Add(StructureInventoryBaseElement.StructureValueField, typeof(string));
            newStructureTable.Columns.Add(StructureInventoryBaseElement.ContentValueField, typeof(string));
            newStructureTable.Columns.Add(StructureInventoryBaseElement.OtherValueField, typeof(string));
            newStructureTable.Columns.Add(StructureInventoryBaseElement.VehicleValueField, typeof(string));
            newStructureTable.Columns.Add(StructureInventoryBaseElement.FirstFloorElevationField, typeof(string));
            newStructureTable.Columns.Add(StructureInventoryBaseElement.GroundElevationField, typeof(string));
            newStructureTable.Columns.Add(StructureInventoryBaseElement.YearField, typeof(string));
            newStructureTable.Columns.Add(StructureInventoryBaseElement.ModuleField, typeof(string));

            newStructureTable.Columns.Add(BEG_DAM_DEPTH, typeof(string));
            newStructureTable.Columns.Add(YEAR_IN_CONSTRUCTION, typeof(string));
            newStructureTable.Columns.Add(NOTES, typeof(string));
            newStructureTable.Columns.Add(OTHER, typeof(string));

            return newStructureTable;
        }

        /// <summary>
        /// Saves the datatable.
        /// </summary>
        /// <param name="structureDataTable">The data. Each row is a structure.</param>
        /// <param name="name">The name of the table in the db.</param>
        /// <param name="features">Geometry data for the structures.</param>
        public void Save(DataTable structureDataTable, string name, LifeSimGIS.VectorFeatures features)
        {
            InMemoryReader myInMemoryReader = new InMemoryReader(structureDataTable);
            DataTableView myDTView = myInMemoryReader.GetTableManager(name);

            //create the geo package writer that will write the data out
            LifeSimGIS.GeoPackageWriter myGeoPackWriter = new LifeSimGIS.GeoPackageWriter(StructureInventoryLibrary.SharedData.StudyDatabase);

            // write the data out
            //myGeoPackWriter.AddFeatures(Name, myReader.ToFeatures(), myReader.GetAttributeTable());
            string tableConst = STRUCTURE_INVENTORY_TABLE_CONSTANT;
            myGeoPackWriter.AddFeatures(tableConst + name, features, myDTView);
        }

        /// <summary>
        /// This is called when importing a structure inventory from an old fda study.
        /// </summary>
        /// <param name="structureData"></param>
        /// <param name="structuresName"></param>
        public void SaveNew(DataTable structureData, string structuresName)
        {  
            string tableName = STRUCTURE_INVENTORY_TABLE_CONSTANT + structuresName;
            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                Storage.Connection.Instance.CreateTable(tableName, ChildTableColumns, ChildTableTypes);
                DataTableView tbl = Storage.Connection.Instance.GetTable(tableName);
                
                for(int i = 0;i<structureData.Rows.Count;i++)
                {
                    tbl.AddRow(structureData.Rows[i].ItemArray);
                }

                tbl.ApplyEdits();
            }
        }

        private object[] GetRowDataFromElement(InventoryElement element)
        {
            return new object[] { element.Name, element.Description, element.IsImportedFromOldFDA };
        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            if (StructureInventoryLibrary.SharedData.StudyDatabase == null)
            {
                StructureInventoryLibrary.SharedData.StudyDatabase = new SQLiteManager(Storage.Connection.Instance.ProjectFile);
            }
            StructureInventoryBaseElement baseElement = new StructureInventoryBaseElement((string)rowData[NAME_COL], (string)rowData[DESC_COL]);
            bool isImportedFromOldFDA = Convert.ToBoolean( rowData[IS_OLD_FDA]);

            InventoryElement invEle = new InventoryElement(baseElement, isImportedFromOldFDA);
            return invEle;
        }
        #endregion

        public void Remove(ChildElement element)
        {
            RemoveFromParentTable(element, TableName);
            string inventoryTable = STRUCTURE_INVENTORY_TABLE_CONSTANT + element.Name;
            RemoveTable(inventoryTable);
            RemoveFromGeopackageTable(inventoryTable);
            StudyCacheForSaving.RemoveElement((InventoryElement)element);

        }

        /// <summary>
        /// This is to be used when importing a structure inventory from an old fda study
        /// </summary>
        /// <param name="name"></param>
        public void SaveNewInventoryToParentTable(string name, string description = "")
        {
            StructureInventoryLibrary.SharedData.StudyDatabase = new SQLiteManager(ViewModel.Storage.Connection.Instance.ProjectFile);
            StructureInventoryBaseElement baseElem = new StructureInventoryBaseElement(name, description);
            InventoryElement elem = new InventoryElement(baseElem, true);
            SaveNew(elem);
        }

        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(InventoryElement))
            {
                SaveNewElement(element);
            }
        }
 
        /// <summary>
        /// Loading a structure inventory does not get all the structure data. It only brings in the name. If you want the structure data
        /// you will need to read it in when you need it. This is happening when adding the structure to the map window.
        /// </summary>
        public void Load()
        {
            List<ChildElement> structures = CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (InventoryElement elem in structures)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        /// <summary>
        /// Right now there is no way to edit structs other than through the map window. This call only works for "rename". It will not 
        /// update any of the structs values, only its name
        /// </summary>
        /// <param name="oldElement"></param>
        /// <param name="elementToSave"></param>
        /// <param name="changeTableIndex"></param>
        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex )
        {
            base.SaveExisting(oldElement, elementToSave);
            if (DidParentTableRowValuesChange(elementToSave, GetRowDataFromElement((InventoryElement)elementToSave), oldElement.Name, TableName) )
            {
                //if the name has changed then we need to change the name in the geo package contents table
                if (!oldElement.Name.Equals(elementToSave.Name))
                {
                    string childTable = ChangeTableConstant + oldElement.Name;
                    string newChildTableName = ChangeTableConstant + elementToSave.Name;
                    RenameInventoryInGeoPackageTable(childTable, newChildTableName);
                }
            }
        }

        private void RenameInventoryInGeoPackageTable(string oldName, string newName)
        {
            LifeSimGIS.GeoPackageWriter myGeoPackWriter = new LifeSimGIS.GeoPackageWriter(StructureInventoryLibrary.SharedData.StudyDatabase);
            myGeoPackWriter.RenameFeatures(oldName, newName);
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
            return GetRowDataFromElement((InventoryElement)elem);
        }

        public void UpdateOccTypeNames( string oldGroupName,string newGroupName, string oldOcctypeName, string newOcctypeName)
        {
            string[] keys = new string[] { oldOcctypeName, oldGroupName };
            string[] keyColNames = new string[] { StructureInventoryBaseElement.OccupancyTypeField, StructureInventoryBaseElement.OccupancyTypeGroupName };

            //update the whole row
            object[] newValues = new object[] { newOcctypeName, newGroupName };

            //i need to loop over all of the inventory tables and update all the occtypes
            List<string> names = GetStructureInventoryNames();
            foreach(string name in names)
            {
                string tableName = STRUCTURE_INVENTORY_TABLE_CONSTANT + name;
                UpdateTableRowWithCompoundKey(tableName, keys, keyColNames, keyColNames, newValues);
            }
        }

        private List<string> GetStructureInventoryNames()
        {
            List<string> names = new List<string>();
            if (!Connection.Instance.IsOpen)
            {
                Connection.Instance.Open();
            }

            DataTable table = Connection.Instance.GetDataTable(TableName);
            foreach (DataRow row in table.Rows)
            {
                
                names.Add(row.ItemArray[NAME_COL].ToString());
            }
            return names;
        }

    }
}
