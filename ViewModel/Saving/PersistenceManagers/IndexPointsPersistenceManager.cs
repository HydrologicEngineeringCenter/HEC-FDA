using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            return new IndexPointsChildElement(xmlString, id);

           // List<ImpactAreaRowItem> impactAreaRowItems = GetRowsFromIndexTable(name);
            //return new ImpactAreaElement(name, (string)rowData[DESCRIPTION_COL], impactAreaRowItems, id);
        }


        public override void Load()
        {
            List<ChildElement> indexPointElems = CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (IndexPointsChildElement elem in indexPointElems)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            return new object[] { ((IndexPointsChildElement)elem).ToXML() };
        }
    }
}
