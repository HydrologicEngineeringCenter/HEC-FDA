using FdaViewModel.FlowTransforms;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class InflowOutflowPersistenceManager : UndoRedoBase, IPersistableWithUndoRedo
    {
        //ELEMENT_TYPE is used to store the type in the log tables. Initially i was actually storing the type
        //of the element. But since they get stored as strings if a developer changes the name of the class
        //you would no longer get any of the old logs. So i use this constant.
        private const string ELEMENT_TYPE = "Inflow_Outflow";
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("InflowOutflowPersistenceManager");


        private const string TABLE_NAME = "Inflow_Outflow_Relationships";
        internal override string ChangeTableConstant { get { return "Inflow Outflow - "; } }
        private static readonly string[] TableColNames = { "Name", "Last Edit Date", "Description", "Curve Distribution Type" , "Curve"};
        public static readonly Type[] TableColTypes = { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) };

        public override string TableName { get { return TABLE_NAME; } }
        //TODO MAKE THIS A REAL NAME
        public override string ChangeTableName { get { return "NAME"; } }

        public override int ChangeTableNameColIndex { get { return CHANGE_TABLE_NAME_INDEX; } }

        public override string ChangeTableStateIndexColName { get { return STATE_INDEX_COL_NAME; } }

        public override int ChangeTableLastEditDateIndex { get { return LAST_EDIT_DATE_INDEX; } }

        public override string ChangeTableElementIdColName { get { return ELEMENT_ID_COL_NAME; } }

        public override string[] ChangeTableColumnNames => throw new NotImplementedException();

        public override Type[] ChangeTableColumnTypes => throw new NotImplementedException();

        public override string[] TableColumnNames => throw new NotImplementedException();
        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return TableColTypes; }
        }
        public InflowOutflowPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }


        #region utilities
        private object[] GetRowDataFromElement(InflowOutflowElement element)
        {
            return new object[] { element.Name, element.LastEditDate, element.Description, element.Curve.Distribution, ExtentionMethods.CreateXMLCurveString(element.Curve.Distribution, element.Curve.XValues, element.Curve.YValues) };

        }
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            Statistics.UncertainCurveDataCollection ucdc = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
            InflowOutflowElement inout = new InflowOutflowElement((string)rowData[0], (string)rowData[1], (string)rowData[2], ucdc);
            //inout.Curve.fromSqliteTable(ChangeTableConstant + (string)rowData[1]);
            inout.Curve = ExtentionMethods.GetCurveFromXMLString((string)rowData[4], (Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
            return inout;
        }

        #endregion


        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(InflowOutflowElement))
            {
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;

                SaveNewElementToParentTable(GetRowDataFromElement((InflowOutflowElement)element), TableName, TableColumnNames, TableColumnTypes);
                //SaveElementToChangeTable(element.Name, GetRowDataFromElement((InflowOutflowElement)element), ChangeTableConstant, TableColumnNames, TableColumnTypes);
                //SaveCurveTable(element.Curve, ChangeTableConstant, editDate);                //save the individual table
                //add the rating element to the cache which then raises event that adds it to the owner element
                StudyCacheForSaving.AddElement((InflowOutflowElement)element);
            }
        }

        public void Remove(ChildElement element)
        {
            //RemoveFromParentTable(element, TableName);
            //DeleteChangeTableAndAssociatedTables(element, ChangeTableConstant);
            //StudyCacheForSaving.RemoveElement((InflowOutflowElement)element);
        }

        public void SaveExisting(ChildElement oldElement, Utilities.ChildElement elementToSave, int changeTableIndex  )
        {
            string editDate = DateTime.Now.ToString("G");
            elementToSave.LastEditDate = editDate;

            if (DidParentTableRowValuesChange(elementToSave, GetRowDataFromElement((InflowOutflowElement)elementToSave), oldElement.Name, TableName) || AreCurvesDifferent(oldElement.Curve, elementToSave.Curve))
            {
                UpdateParentTableRow(elementToSave.Name, changeTableIndex, GetRowDataFromElement((InflowOutflowElement)elementToSave), oldElement.Name, TableName, true, ChangeTableConstant);
                //
                //SaveCurveTable(elementToSave.Curve, ChangeTableConstant, editDate);
                // update the existing element. This will actually remove the old element and do an insert at that location with the new element.
                StudyCacheForSaving.UpdateInflowOutflowElement((InflowOutflowElement)oldElement, (InflowOutflowElement)elementToSave);
            }
        }

        public void Load()
        {
            List<ChildElement> inflowOutflows = CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (InflowOutflowElement elem in inflowOutflows)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

      
        private void SaveFlowFreqCurveTable(InflowOutflowElement element, string lastEditDate)
        {
            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }
            element.Curve.toSqliteTable(ChangeTableConstant + lastEditDate);
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
