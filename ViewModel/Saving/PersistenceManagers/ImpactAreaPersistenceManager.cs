using DatabaseManager;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class ImpactAreaPersistenceManager : SavingBase
    {
        private static readonly string[] TableColNames = { "xml" };
        private static readonly Type[] TableColTypes = { typeof(string) };

        private const string TABLE_NAME = "impact_area_set";
        private const int XML_COL = 1;

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

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            if (rowData.Length > 2)
            {
                string name = (string)rowData[NAME_COL];
                List<ImpactAreaRowItem> impactAreaRowItems = GetRowsFromIndexTable(name);
                int id = Convert.ToInt32(rowData[ID_COL]);
                return new ImpactAreaElement(name, (string)rowData[DESCRIPTION_COL], impactAreaRowItems, id);
            }
            else
            {
                int id = Convert.ToInt32(rowData[ID_COL]);
                string xmlString = (string)rowData[XML_COL];
                return new ImpactAreaElement(xmlString, id);
            }
        }


        #endregion

        #region Backwards Compatible Database
        public static string IMPACT_AREA_TABLE_PREFIX = "impact_areas -";
        private const int INDEX_TABLE_ID_COL = 0;
        private const int INDEX_TABLE_NAME_COL = 2;
        private const int DESCRIPTION_COL = 2;

        private List<ImpactAreaRowItem> GetRowsFromIndexTable(string impactAreaSetName)
        {
            List<ImpactAreaRowItem> items = new List<ImpactAreaRowItem>();
            DataTableView indexTable = Connection.Instance.GetTable(IMPACT_AREA_TABLE_PREFIX + impactAreaSetName);
            foreach (object[] row in indexTable.GetRows(0, indexTable.NumberOfRows - 1))
            {
                int id = (int)row[INDEX_TABLE_ID_COL];
                string name = row[INDEX_TABLE_NAME_COL].ToString();
                items.Add(new ImpactAreaRowItem(id, name));
            }
            return items;
        }
        #endregion

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
            return new object[] { ((ImpactAreaElement)elem).ToXML() };
        }
    }
}
