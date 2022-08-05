using HEC.FDA.ViewModel.AlternativeComparisonReport;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class AlternativeComparisonReportPersistenceManager : SavingBase
    {
        private const int XML_COL = 2;
        public override string TableName => "alternative_comparison_reports";

        public override string[] TableColumnNames => new string[] { NAME, "xml" };

        public override Type[] TableColumnTypes => new Type[] { typeof(string), typeof(string) };

        public AlternativeComparisonReportPersistenceManager(FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            AlternativeComparisonReportElement elem = null;
            if (rowData[XML_COL] is string xml)
            {
                int id = Convert.ToInt32(rowData[ID_COL]);
                elem = new AlternativeComparisonReportElement(xml,id);
            }
            return elem;
        }       

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            object[] retval = null;
            if (elem is AlternativeComparisonReportElement altElement)
            {
                retval = new object[] { elem.Name, altElement.WriteToXML() };
            }
            return retval;
        }

        public override void Load()
        {
            List<ChildElement> iasElems = CreateElementsFromRows(TableName, (rowData) => CreateElementFromRowData(rowData));
            foreach (AlternativeComparisonReportElement elem in iasElems)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public void SaveNew(ChildElement element)
        {
            if (element is AlternativeComparisonReportElement altElem)
            {
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;
                SaveNewElementToTable(GetRowDataFromElement(altElem), TableName, TableColumnNames, TableColumnTypes);
                StudyCacheForSaving.AddElement(altElem);
            }
        }

    }
}
