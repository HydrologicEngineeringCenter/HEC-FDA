using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class IndexPointsPersistenceManager : SavingBase
    {
        private static readonly string[] TableColNames = { "xml" };
        private static readonly Type[] TableColTypes = { typeof(string) };
        private const int XML_COL = 1;


        public override string TableName
        {
            get { return "index_points"; }
        }

        public override string[] TableColumnNames
        {
            get { return TableColNames; }
        }
        public override Type[] TableColumnTypes
        {
            get { return TableColTypes; }
        }

        public IndexPointsPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            int id = Convert.ToInt32(rowData[ID_COL]);
            string xmlString = (string)rowData[XML_COL];
            return new IndexPointsElement(xmlString, id);
        }


        public override void Load()
        {
            List<ChildElement> indexPointElems = CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (IndexPointsElement elem in indexPointElems)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            return new object[] { ((IndexPointsElement)elem).ToXML() };
        }
    }
}
