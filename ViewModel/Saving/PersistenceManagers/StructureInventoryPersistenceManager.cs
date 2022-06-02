using DatabaseManager;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Data;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class StructureInventoryPersistenceManager : SavingBase
    {
        public static readonly string STRUCTURE_INVENTORY_TABLE_CONSTANT = "structure_inventory_";

        public static readonly string STRUCTURE_ID = "StructureID";
        public static readonly string BEG_DAM_DEPTH = "BeginningDamageDepth";
        public static readonly string YEAR_IN_CONSTRUCTION = "YearInConstruction";
        public static readonly string NOTES = "Notes";
        public static readonly string NumberOfStructures = "NumberOfStructures";

        private const int NAME_COL = 1;
        private const int DESC_COL = 2;
        private const int IS_OLD_FDA = 3;

        private const string TABLE_NAME = "structure_inventories";

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
        StructureInventoryBaseElement.OccupancyTypeGroup,
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
        typeof(int),
        typeof(string),
        typeof(int),
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
            DataTable newStructureTable = new DataTable("EmptyTable");

            newStructureTable.Columns.Add(STRUCTURE_ID, typeof(string));

            newStructureTable.Columns.Add(StructureInventoryBaseElement.fidField, typeof(string));
            newStructureTable.Columns.Add(StructureInventoryBaseElement.geomField, typeof(string));

            newStructureTable.Columns.Add(StructureInventoryBaseElement.OccupancyTypeField, typeof(int));
            newStructureTable.Columns.Add(StructureInventoryBaseElement.damCatField, typeof(string));

            newStructureTable.Columns.Add(StructureInventoryBaseElement.OccupancyTypeGroup, typeof(int));
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
            newStructureTable.Columns.Add(NumberOfStructures, typeof(string));

            return newStructureTable;
        }

        /// <summary>
        /// Saves the datatable.
        /// </summary>
        /// <param name="structureDataTable">The data. Each row is a structure.</param>
        /// <param name="name">The name of the table in the db.</param>
        /// <param name="features">Geometry data for the structures.</param>
        public void Save(DataTable structureDataTable, int id, LifeSimGIS.VectorFeatures features)
        {
            InMemoryReader myInMemoryReader = new InMemoryReader(structureDataTable);
            DataTableView myDTView = myInMemoryReader.GetTableManager(id.ToString());

            //create the geo package writer that will write the data out
            LifeSimGIS.GeoPackageWriter myGeoPackWriter = new LifeSimGIS.GeoPackageWriter(StructureInventoryLibrary.SharedData.StudyDatabase);

            // write the data out
            //myGeoPackWriter.AddFeatures(Name, myReader.ToFeatures(), myReader.GetAttributeTable());
            string tableConst = STRUCTURE_INVENTORY_TABLE_CONSTANT;
            myGeoPackWriter.AddFeatures(STRUCTURE_INVENTORY_TABLE_CONSTANT + id, features, myDTView);
        }

        /// <summary>
        /// This is called when importing a structure inventory from an old fda study.
        /// </summary>
        /// <param name="structureData"></param>
        /// <param name="structuresName"></param>
        public void SaveNew(DataTable structureData, string structuresName)
        {  
            string tableName = STRUCTURE_INVENTORY_TABLE_CONSTANT + structuresName;
            if (!Connection.Instance.IsConnectionNull)
            {
                List<string> colNames = new List<string>();
                List<Type> colTypes = new List<Type>();
                foreach(DataColumn col in structureData.Columns)
                {
                    colNames.Add( col.ColumnName);
                    colTypes.Add(col.DataType);
                }
                Connection.Instance.CreateTable(tableName, colNames.ToArray(), colTypes.ToArray());
                DataTableView tbl = Connection.Instance.GetTable(tableName);
                
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
            int id = Convert.ToInt32(rowData[ID_COL]);
            StructureInventoryBaseElement baseElement = new StructureInventoryBaseElement((string)rowData[NAME_COL], (string)rowData[DESC_COL], id);
            bool isImportedFromOldFDA = Convert.ToBoolean( rowData[IS_OLD_FDA]);

            InventoryElement invEle = new InventoryElement(baseElement, isImportedFromOldFDA, id);
            return invEle;
        }
        #endregion

        public void Remove(ChildElement element)
        {
            RemoveFromParentTable(element, TableName);
            string inventoryTable = STRUCTURE_INVENTORY_TABLE_CONSTANT + element.ID;
            RemoveTable(inventoryTable);
            RemoveFromGeopackageTable(inventoryTable);
            StudyCacheForSaving.RemoveElement((InventoryElement)element);
        }

        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(InventoryElement))
            {
                base.SaveNew(element);
            }
        }
 
        /// <summary>
        /// Loading a structure inventory does not get all the structure data. It only brings in the name. If you want the structure data
        /// you will need to read it in when you need it. This is happening when adding the structure to the map window.
        /// </summary>
        public override void Load()
        {
            List<ChildElement> structures = CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (InventoryElement elem in structures)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            return GetRowDataFromElement((InventoryElement)elem);
        }
    }
}
