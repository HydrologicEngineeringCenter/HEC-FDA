using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml.Linq;
using ViewModel.FrequencyRelationships;
using ViewModel.Utilities;
using ViewModel.FrequencyRelationships;
using System.Xml.Linq;

namespace ViewModel.Saving.PersistenceManagers
{
    public class FlowFrequencyPersistenceManager :UndoRedoBase, IPersistableWithUndoRedo
    {
        public static readonly string FLOW_FREQUENCY = "FlowFrequency";
        public static readonly string NAME = "Name";
        public static readonly string DESCRIPTION = "Description";
        public static readonly string LAST_EDIT_DATE = "LastEditDate";
        public static readonly string IS_ANALYTICAL = "IsAnalytical";
        public static readonly string ANALYTICAL_DATA = "AnalyticalData";
        public static readonly string USES_MOMENTS = "UsesMoments";
        public static readonly string POR = "POR";
        public static readonly string MOMENTS = "Moments";
        public static readonly string MEAN = "Mean";
        public static readonly string ST_DEV = "StDev";
        public static readonly string SKEW = "Skew";
        public static readonly string FIT_TO_FLOWS = "FitToFlows";
        public static readonly string IS_LOG = "IsLog";
        public static readonly string FLOWS = "Flows";

        private const int NAME_COL = 1;
        private const int DESC_COL = 2;
        private const int XML_COL = 3;

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
            get{return new string[] { NAME,DESCRIPTION, "XML" };}
        }

        public override Type[] TableColumnTypes
        {
            get  { return new Type[] { typeof(string), typeof(string), typeof(string)}; }
        }

        public override string[] ChangeTableColumnNames
        {
            get{return new string[] { ELEMENT_ID_COL_NAME, NAME, "XML", STATE_INDEX_COL_NAME };}
        }

        public override Type[] ChangeTableColumnTypes
        {
            get{return new Type[]{typeof(int), typeof(string), typeof(string), typeof(int)};}
        }

        public FlowFrequencyPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        private object[] GetRowDataFromElement(AnalyticalFrequencyElement element)
        {
            return new object[]{element.Name, element.Description, WriteFlowFrequencyToXML(element) };
        }

        private string ConvertFlowsToString(List<double> flows)
        {
            if(flows.Count == 0)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            foreach(double d in flows)
            {
                sb.Append(d + ",");
            }
            //remove the last comma
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            return new AnalyticalFrequencyElement((string)rowData[NAME_COL], (string)rowData[DESC_COL], (string)rowData[XML_COL]);
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
            base.Remove(element);
        }

        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex )
        {
            base.SaveExisting(oldElement, elementToSave, changeTableIndex);
        }

        public void Load()
        {
            List<ChildElement> flowFreqs = CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (AnalyticalFrequencyElement elem in flowFreqs)
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
            int elemId = GetElementId(TableName, element.Name);
            int stateId = Storage.Connection.Instance.GetMaxStateIndex(ChangeTableName, elemId, ELEMENT_ID_COL_NAME, STATE_INDEX_COL_NAME) + 1;
            return new object[]{elemId, element.Name, WriteFlowFrequencyToXML((AnalyticalFrequencyElement)element), stateId};
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            return GetRowDataFromElement((AnalyticalFrequencyElement)elem);
        }

        public string WriteFlowFrequencyToXML(AnalyticalFrequencyElement elem)
        {
            XElement flowFreqElem = new XElement(FLOW_FREQUENCY);
            flowFreqElem.SetAttributeValue(NAME, elem.Name);
            flowFreqElem.SetAttributeValue(DESCRIPTION, elem.Description);
            flowFreqElem.SetAttributeValue(LAST_EDIT_DATE, elem.LastEditDate);
            flowFreqElem.SetAttributeValue(IS_ANALYTICAL, elem.IsAnalytical);
            if(elem.IsAnalytical)
            {
                XElement analyticalElem = new XElement(ANALYTICAL_DATA);
                flowFreqElem.Add(analyticalElem);
                analyticalElem.SetAttributeValue(USES_MOMENTS, elem.IsStandard);
                analyticalElem.SetAttributeValue(POR, elem.POR);
                //if (elem.IsStandard)
                {
                    XElement momentsElem = new XElement(MOMENTS);
                    analyticalElem.Add(momentsElem);
                    momentsElem.SetAttributeValue(MEAN, elem.Mean);
                    momentsElem.SetAttributeValue(ST_DEV, elem.StDev);
                    momentsElem.SetAttributeValue(SKEW, elem.Skew);
                }
                //else //fit to flows
                {
                    XElement fitToFlowsElem = new XElement(FIT_TO_FLOWS);
                    analyticalElem.Add(fitToFlowsElem);
                    fitToFlowsElem.SetAttributeValue(IS_LOG, elem.IsLogFlow);
                    fitToFlowsElem.SetAttributeValue(FLOWS, ConvertFlowsToString(elem.AnalyticalFlows));
                }
            }
            else //graphical
            {

            }
            return flowFreqElem.ToString();
        }

    }
}
