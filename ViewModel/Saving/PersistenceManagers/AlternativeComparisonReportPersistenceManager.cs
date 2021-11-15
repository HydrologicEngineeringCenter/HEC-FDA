using FdaLogging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.AlternativeComparisonReport;
using ViewModel.Utilities;

namespace ViewModel.Saving.PersistenceManagers
{
    public class AlternativeComparisonReportPersistenceManager : SavingBase, IElementManager
    {
        public override string TableName => "alternative_comparison_reports";

        public override string[] TableColumnNames => new string[] { NAME, "xml" };

        public override Type[] TableColumnTypes => new Type[] { typeof(string), typeof(string) };

        public AlternativeComparisonReportPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            string xml = (string)rowData[2];
            AlternativeComparisonReportElement elem = new AlternativeComparisonReportElement(xml);
            return elem;
        }

        #region Logging
        //todo: not sure what to do with logging stuff yet. Waiting until a later task to determine
        //if this should all get removed from the base class or not.
        public ObservableCollection<LogItem> GetLogMessages(string elementName)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<LogItem> GetLogMessagesByLevel(LoggingLevel level, string elementName)
        {
            throw new NotImplementedException();
        }

        public void Log(LoggingLevel level, string message, string elementName)
        {
            throw new NotImplementedException();
        }
        #endregion

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            object[] retval = new object[] { elem.Name, ((AlternativeComparisonReportElement)elem).WriteToXML() };
            return retval;
        }


        public void Load()
        {
            List<ChildElement> iasElems = CreateElementsFromRows(TableName, (rowData) => CreateElementFromRowData(rowData));
            foreach (AlternativeComparisonReportElement elem in iasElems)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public void Remove(ChildElement element)
        {
            //remove from the cache first while you can still get the element's id.
            StudyCacheForSaving.RemoveElement((AlternativeComparisonReportElement)element);
            RemoveFromParentTable(element, TableName);
        }

        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex)
        {
            base.SaveExisting(oldElement, elementToSave);
        }

        public void SaveNew(ChildElement element)
        {
            if (element is AlternativeComparisonReportElement)
            {
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;
                SaveNewElementToParentTable(GetRowDataFromElement((AlternativeComparisonReportElement)element), TableName, TableColumnNames, TableColumnTypes);
                StudyCacheForSaving.AddElement((AlternativeComparisonReportElement)element);
            }
        }
    }
}
