using DatabaseManager;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class ImpactAreaPersistenceManager : SavingBase
    {
        private static readonly string[] TableColNames = { NAME, DESCRIPTION };
        private static readonly Type[] TableColTypes = { typeof(string), typeof(string) };
        public static string IMPACT_AREA_TABLE_PREFIX = "impact_areas -";

        private const string TABLE_NAME = "impact_area_set";
        private const int NAME_COL = 1;
        private const int DESCRIPTION_COL = 2;
        private const int INDEX_TABLE_NAME_COL = 2;
        private const int INDEX_TABLE_ID_COL = 0;

        public override string TableName
        {
            get { return TABLE_NAME; }
        }

        public override string[] TableColumnNames
        {
            get{return TableColNames;}
        }

        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return TableColTypes; }
        }

        public ImpactAreaPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        private object[] GetRowDataFromElement(ImpactAreaElement element)
        {
            return new object[] { element.Name, element.Description };
        }
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            string name = (string)rowData[NAME_COL];
            ObservableCollection<ImpactAreaRowItem> impactAreaRowItems = GetRowsFromIndexTable(name);
            int id = Convert.ToInt32(rowData[ID_COL]);
            return new ImpactAreaElement(name, (string)rowData[DESCRIPTION_COL], impactAreaRowItems, id);
        }

        private ObservableCollection<ImpactAreaRowItem> GetRowsFromIndexTable(string impactAreaSetName)
        {
            ObservableCollection<ImpactAreaRowItem> items = new ObservableCollection<ImpactAreaRowItem>();
            DataTableView indexTable = Connection.Instance.GetTable(IMPACT_AREA_TABLE_PREFIX + impactAreaSetName);
            foreach (object[] row in indexTable.GetRows(0, indexTable.NumberOfRows - 1))
            {
                int id = (int)row[INDEX_TABLE_ID_COL];
                string name = row[INDEX_TABLE_NAME_COL].ToString();
                items.Add( new ImpactAreaRowItem(id, name));
            }
            return items;
        }

        private void SaveImpactAreaTable(ImpactAreaElement element)
        {
            LifeSimGIS.ShapefileReader shp = new LifeSimGIS.ShapefileReader(element.SelectedPath);
            LifeSimGIS.PolygonFeatures polyFeatures = (LifeSimGIS.PolygonFeatures)shp.ToFeatures();
            WriteImpactAreaTableToSqlite(element,polyFeatures);
        }
        private void WriteImpactAreaTableToSqlite(ImpactAreaElement element, LifeSimGIS.PolygonFeatures polyFeatures)
        {
            if (!Connection.Instance.IsConnectionNull)
            {
                if (Connection.Instance.TableNames().Contains(IMPACT_AREA_TABLE_PREFIX + element.Name))
                {
                    //already exists... delete
                    Connection.Instance.DeleteTable(IMPACT_AREA_TABLE_PREFIX + element.Name);
                }
                LifeSimGIS.GeoPackageWriter gpw = new LifeSimGIS.GeoPackageWriter(Connection.Instance.Reader);
                DataTable dt = new DataTable(IMPACT_AREA_TABLE_PREFIX + element.Name);
                dt.Columns.Add("Name", typeof(string));

                foreach (ImpactAreaRowItem row in element.ImpactAreaRows)
                {
                    dt.Rows.Add(row.Name);
                }

                InMemoryReader imr = new InMemoryReader(dt);
                gpw.AddFeatures(IMPACT_AREA_TABLE_PREFIX + element.Name, polyFeatures, imr.GetTableManager(imr.TableNames[0]));
            }
        }
        private void UpdateExistingTable(ImpactAreaElement element)
        {
            DataTableView dtv = Connection.Instance.GetTable(IMPACT_AREA_TABLE_PREFIX + element.Name);

            object[] nameArray = new object[element.ImpactAreaRows.Count];
            for(int i = 0;i<element.ImpactAreaRows.Count;i++)
            {
                nameArray[i] = element.ImpactAreaRows[i].Name;
            }
            dtv.EditColumn(2, nameArray);
            dtv.ApplyEdits();
        }

        #endregion

        public void SaveNew(ChildElement element)
        {
            if (element is ImpactAreaElement)
            {
                //the row items have id's of -1 at this point because they have not been saved. When the element gets added to the cache
                //they still have -1 which causes problems down the road. Because we only ever have one impact area element, i can just 
                //assign the id's. This is assuming that the database will start the id's at 1. 
                ImpactAreaElement impactAreaElement = (ImpactAreaElement)element;
                for(int i= 0;i<impactAreaElement.ImpactAreaRows.Count;i++)
                {
                    impactAreaElement.ImpactAreaRows[i].ID = i+1;
                }
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;
                SaveNewElementToParentTable(GetRowDataFromElement(impactAreaElement), TableName, TableColumnNames, TableColumnTypes);
                SaveImpactAreaTable(impactAreaElement);
                StudyCacheForSaving.AddElement(impactAreaElement);
            }
        }  

        public void Remove(ChildElement element)
        {
            RemoveFromParentTable(element, TableName);
            string impAreaTable = IMPACT_AREA_TABLE_PREFIX + element.Name;
            RemoveTable(impAreaTable);
            RemoveFromGeopackageTable(impAreaTable);
            StudyCacheForSaving.RemoveElement(element);
        }

        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave)
        {
            base.SaveExisting(elementToSave);

            if (!oldElement.Name.Equals(elementToSave.Name))
            {
                string oldName = IMPACT_AREA_TABLE_PREFIX + oldElement.Name;
                string newName = IMPACT_AREA_TABLE_PREFIX + elementToSave.Name;
                if (StructureInventoryLibrary.SharedData.StudyDatabase == null)
                {
                    StructureInventoryLibrary.SharedData.StudyDatabase = new SQLiteManager(Connection.Instance.ProjectFile);
                }
                LifeSimGIS.GeoPackageWriter myGeoPackWriter = new LifeSimGIS.GeoPackageWriter(StructureInventoryLibrary.SharedData.StudyDatabase);
                myGeoPackWriter.RenameFeatures(oldName, newName);
            }
            UpdateExistingTable((ImpactAreaElement)elementToSave);
        }

        public override void Load()
        {
            List<ChildElement> impAreas = CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (ImpactAreaElement elem in impAreas)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            return GetRowDataFromElement((ImpactAreaElement)elem);
        }
    }
}
