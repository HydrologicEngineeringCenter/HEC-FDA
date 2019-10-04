using FdaViewModel.FrequencyRelationships;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class FlowFrequencyPersistenceManager :UndoRedoBase, IPersistableWithUndoRedo
    {
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("FlowFrequencyPersistenceManager");
        //ELEMENT_TYPE is used to store the type of element in the log tables.
        private const string ELEMENT_TYPE = "Flow_Freq";

        /// <summary>
        /// The name of the parent table that will hold all elements of this type
        /// </summary>
        public override string TableName { get { return "analytical_frequency_curves"; } }
        public override string ChangeTableName { get { return "analytical_frequency_changes"; } }

        public override string[] TableColumnNames
        {
            get { return new string[] { "Name", "Last Edit Date", "Description", "Mean (of Log)", "Standard Deviation (of Log)", "Skew (of Log)", "Equivalent Years of Record" }; }
        }

        public override Type[] TableColumnTypes
        {
            get { return new Type[] { typeof(string), typeof(string), typeof(string),
                typeof(double), typeof(double), typeof(double), typeof(int) }; }
        }


        public override string[] ChangeTableColumnNames => throw new NotImplementedException();

        public override Type[] ChangeTableColumnTypes => throw new NotImplementedException();


        public FlowFrequencyPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        private object[] GetRowDataFromElement(AnalyticalFrequencyElement element)
        {
            return new object[] { element.Name, element.LastEditDate, element.Description, element.Distribution.GetMean, element.Distribution.GetStDev, element.Distribution.GetG, element.Distribution.GetSampleSize };
        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            double mean = (double)rowData[3];
            double stdev = (double)rowData[4];
            double skew = (double)rowData[5];
            Int64 n = (Int64)rowData[6];
            return new AnalyticalFrequencyElement((string)rowData[0], (string)rowData[1], (string)rowData[2], new Statistics.LogPearsonIII(mean, stdev, skew, (int)n));

        }
        #endregion
        /// <summary>
        /// Flow frequency doesn not save to its own table. All is contained in the parent row
        /// </summary>
        /// <param name="element"></param>
        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(AnalyticalFrequencyElement))
            {
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;

                SaveNewElementToParentTable(GetRowDataFromElement((AnalyticalFrequencyElement)element), TableName, TableColumnNames, TableColumnTypes);
                //SaveElementToChangeTable(element.Name, GetRowDataFromElement((AnalyticalFrequencyElement)element), ChangeTableConstant, TableColumnNames, TableColumnTypes);
                //SaveCurveTable(element.Curve, ChangeTableConstant, editDate);
                //add the rating element to the cache which then raises event that adds it to the owner element
                StudyCacheForSaving.AddElement((AnalyticalFrequencyElement)element);
            }
        }

        public void Remove(ChildElement element)
        {
            //RemoveFromParentTable(element, TableName);
            //DeleteChangeTableAndAssociatedTables(element, ChangeTableConstant);
            //StudyCacheForSaving.RemoveElement((AnalyticalFrequencyElement)element);

        }

        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex )
        {
            string editDate = DateTime.Now.ToString("G");
            elementToSave.LastEditDate = editDate;

            if (DidParentTableRowValuesChange(elementToSave, GetRowDataFromElement((AnalyticalFrequencyElement)elementToSave), oldElement.Name, TableName) )
            {
                UpdateParentTableRow(elementToSave.Name, changeTableIndex, GetRowDataFromElement((AnalyticalFrequencyElement)elementToSave), oldElement.Name, TableName, true, ChangeTableConstant);
                // SaveCurveTable(elementToSave.Curve, ChangeTableConstant, editDate);
                StudyCacheForSaving.UpdateFlowFrequencyElement((AnalyticalFrequencyElement)oldElement, (AnalyticalFrequencyElement)elementToSave);
            }
        }

        public void Load()
        {
            List<ChildElement> flowFreqs = CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (AnalyticalFrequencyElement elem in flowFreqs)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public override void AddValidationRules()
        {
           // throw new NotImplementedException();
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

        public override object[] GetRowDataForChangeTable(ChildElement element)
        {
            throw new NotImplementedException();
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            throw new NotImplementedException();
        }
    }
}
