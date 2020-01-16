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
        private const int ID_COL = 0;
        private const int NAME_COL = 1;
        private const int LAST_EDIT_DATE_COL = 2;
        private const int DESC_COL = 3;
        private const int MEAN_COL = 4;
        private const int ST_DEV_COL = 5;
        private const int SKEW_COL = 6;
        private const int YEARS_OF_RECORD_COL = 7;

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
            get { return new string[] { NAME, LAST_EDIT_DATE, DESCRIPTION, "mean", "standard_deviation", "skew", "equivalent_years_of_record" }; }
        }

        public override Type[] TableColumnTypes
        {
            get { return new Type[] { typeof(string), typeof(string), typeof(string),
                typeof(double), typeof(double), typeof(double), typeof(int) }; }
        }


        public override string[] ChangeTableColumnNames
        {
            get
            {
                return new string[] { ELEMENT_ID_COL_NAME, NAME, LAST_EDIT_DATE, DESCRIPTION,
            "mean_(of_log)", "standard_deviation_(of_log)", "skew_(of_log)", "equivalent_years_of_record", STATE_INDEX_COL_NAME};
            }

        }

public override Type[] ChangeTableColumnTypes
        {
            get
            {
                return new Type[] {typeof(int), typeof(string), typeof(string), typeof(string),
                typeof(double), typeof(double), typeof(double), typeof(int), typeof(int) };
            }
        }


        public FlowFrequencyPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        private object[] GetRowDataFromElement(AnalyticalFrequencyElement element)
        {
            return new object[] { element.Name, element.LastEditDate, element.Description};
            //todo: Refactor: CO
                //element.Distribution.GetMean, element.Distribution.GetStDev, element.Distribution.GetG,
                //element.Distribution.GetSampleSize };
        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            double mean = Convert.ToDouble( rowData[MEAN_COL]);
            double stdev = Convert.ToDouble(rowData[ST_DEV_COL]);
            double skew = Convert.ToDouble(rowData[SKEW_COL]);
            Int64 n = Convert.ToInt64( rowData[YEARS_OF_RECORD_COL]);

            //todo: Refactor: I should be  able to pass the xml string into the factory and it create it.
            //Model.ImpactAreaFunctionFactory.Factory()
            return new AnalyticalFrequencyElement((string)rowData[NAME_COL],
                (string)rowData[LAST_EDIT_DATE_COL], (string)rowData[DESC_COL],null);
                //todo: Refactor: CO and added the null in the line above
                //new Statistics.LogPearsonIII(mean, stdev, skew, (int)n));

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
                //save to parent table
                SaveNewElement(element);
                //save to change table
                SaveToChangeTable(element);
                //log message
                Log(FdaLogging.LoggingLevel.Info, "Created new flow frequency curve: " + element.Name, element.Name);
            }
        }

        public void Remove(ChildElement element)
        {
            //todo: do something here
            //RemoveFromParentTable(element, TableName);
            //DeleteChangeTableAndAssociatedTables(element, ChangeTableConstant);
            //StudyCacheForSaving.RemoveElement((AnalyticalFrequencyElement)element);

        }

        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex )
        {
            base.SaveExisting(oldElement, elementToSave, changeTableIndex);
            //string editDate = DateTime.Now.ToString("G");
            //elementToSave.LastEditDate = editDate;

            //if (DidParentTableRowValuesChange(elementToSave, GetRowDataFromElement((AnalyticalFrequencyElement)elementToSave), oldElement.Name, TableName) )
            //{
            //    UpdateParentTableRow(elementToSave.Name, changeTableIndex, GetRowDataFromElement((AnalyticalFrequencyElement)elementToSave), oldElement.Name, TableName, true, ChangeTableConstant);
            //    // SaveCurveTable(elementToSave.Curve, ChangeTableConstant, editDate);
            //    StudyCacheForSaving.UpdateFlowFrequencyElement((AnalyticalFrequencyElement)oldElement, (AnalyticalFrequencyElement)elementToSave);
            //}
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
            if (element.Description == null)
            {
                element.Description = "";
            }

            int elemId = GetElementId(TableName, element.Name);
            AnalyticalFrequencyElement elem = (AnalyticalFrequencyElement)element;
            //the new statId will be one higher than the max that is in the table already.
            int stateId = Storage.Connection.Instance.GetMaxStateIndex(ChangeTableName, elemId, ELEMENT_ID_COL_NAME, STATE_INDEX_COL_NAME) + 1;
            return new object[] { elemId, element.Name, element.LastEditDate, element.Description };
                //todo: Refactor: CO
                //elem.Distribution.GetMean, elem.Distribution.GetStDev, elem.Distribution.GetG, elem.Distribution.GetSampleSize, stateId};
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            return GetRowDataFromElement((AnalyticalFrequencyElement)elem);
        }
    }
}
