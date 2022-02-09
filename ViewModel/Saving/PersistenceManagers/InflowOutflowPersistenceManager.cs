using paireddata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using ViewModel.FlowTransforms;
using ViewModel.Utilities;

namespace ViewModel.Saving.PersistenceManagers
{
    public class InflowOutflowPersistenceManager : SavingBase
    {
        private const int LAST_EDIT_DATE_COL = 2;
        private const int DESCRIPTION_COL = 3;
        private const int CURVE_COL = 4;


        //ELEMENT_TYPE is used to store the type in the log tables. Initially i was actually storing the type
        //of the element. But since they get stored as strings if a developer changes the name of the class
        //you would no longer get any of the old logs. So i use this constant.
        private const string ELEMENT_TYPE = "inflow_outflow";
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("InflowOutflowPersistenceManager");

        private const string TABLE_NAME = "inflow_outflow_relationships";
        internal override string ChangeTableConstant { get { return "Inflow Outflow - "; } }
        private static readonly string[] TableColNames = { NAME, LAST_EDIT_DATE, DESCRIPTION, CURVE};
        public static readonly Type[] TableColTypes = { typeof(string), typeof(string), typeof(string), typeof(string) };

        public override string TableName { get { return TABLE_NAME; } }

        public override string[] TableColumnNames
        {
            get { return TableColNames; }
        }
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
            return new object[] { element.Name, element.LastEditDate, element.Description, element.Curve.WriteToXML().ToString() };
        }
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            string curveXML = (string)rowData[CURVE_COL];
            UncertainPairedData upd = UncertainPairedData.ReadFromXML(XElement.Parse(curveXML));

            InflowOutflowElement inout = new InflowOutflowElement((string)rowData[NAME_COL], 
                (string)rowData[LAST_EDIT_DATE_COL], (string)rowData[DESCRIPTION_COL], upd);
            return inout;
        }

        #endregion

        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(InflowOutflowElement))
            {
                //save to parent table
                base.SaveNew(element);
                //log message
                Log(FdaLogging.LoggingLevel.Info, "Created new inflow outflow curve: " + element.Name, element.Name);
            }
        }

        public void Remove(ChildElement element)
        {
            base.Remove(element);
        }

        public override void Load()
        {
            List<ChildElement> inflowOutflows = CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (InflowOutflowElement elem in inflowOutflows)
            {
                StudyCacheForSaving.AddElement(elem);
            }
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
        public override void Log(FdaLogging.LoggingLevel level, string message, string elementName)
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
        public override ObservableCollection<FdaLogging.LogItem> GetLogMessages(string elementName)
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
        public override ObservableCollection<FdaLogging.LogItem> GetLogMessagesByLevel(FdaLogging.LoggingLevel level, string elementName)
        {
            int id = GetElementId(TableName, elementName);
            return FdaLogging.RetrieveFromDB.GetLogMessagesByLevel(level, id, ELEMENT_TYPE);
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            return GetRowDataFromElement((InflowOutflowElement)elem);
        }
    }
}
