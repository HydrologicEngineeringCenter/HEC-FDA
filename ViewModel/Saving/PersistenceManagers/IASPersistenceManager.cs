using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class IASPersistenceManager : SavingBase
    {
        //todo: get rid of the logging stuff once we know what we are doing with logging.

        //ELEMENT_TYPE is used to store the type in the log tables. Initially i was actually storing the type
        //of the element. But since they get stored as strings if a developer changes the name of the class
        //you would no longer get any of the old logs. So i use this constant.
        private const string ELEMENT_TYPE = "IAS";
        private const int XML_COLUMN = 2;
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("IASPersistenceManager");

        private const string TABLE_NAME = "ImpactAreaScenarios";
        private static readonly string[] ColumnNames = { "Name","XML"};

        private static readonly Type[] TableColTypes = { typeof(string), typeof(string)};


        /// <summary>
        /// Column names for the main conditions table.
        /// </summary>
        public override string[] TableColumnNames { get { return ColumnNames; } }
        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return TableColTypes; }
        }
        /// <summary>
        /// The table name for the main conditions table.
        /// </summary>
        public override string TableName { get { return TABLE_NAME; } }

        #region constructor
        /// <summary>
        /// The persistence manager for the conditions object. This handles all the interaction between FDA and the database.
        /// </summary>
        /// <param name="studyCache"></param>
        public IASPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #endregion

        #region utilities
        /// <summary>
        /// Gets the row from the element that will go into the main table.
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            object[] retval = null;
            IASElementSet element = elem as IASElementSet;
            if (element != null)
            {
                retval = new object[] { element.Name, element.WriteToXML() };
            }
            return retval;
        }

        /// <summary>
        /// Converts the row in the main table into an actual condition element.
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            string xml = (string)rowData[XML_COLUMN];
            return new IASElementSet(xml);
        }

        #endregion

        /// <summary>
        /// Saves a new conditions element to the database
        /// </summary>
        /// <param name="element"></param>
        public void SaveNew(ChildElement element)
        {
            if (element is IASElementSet iasElem)
            {
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;
                SaveNewElementToParentTable(GetRowDataFromElement(iasElem), TableName, TableColumnNames, TableColumnTypes);
                StudyCacheForSaving.AddElement(iasElem);
            }
        }

        /// <summary>
        /// Deletes an element from the parent table.
        /// </summary>
        /// <param name="element"></param>
        public void Remove(ChildElement element)
        {
            if (element is IASElementSet iasElem)
            {
                //remove from the cache first while you can still get the element's id.
                StudyCacheForSaving.RemoveElement(iasElem);
                RemoveFromParentTable(element, TableName);
            }
        }

        /// <summary>
        /// Updates and existing row in the database.
        /// </summary>
        /// <param name="oldElement"></param>
        /// <param name="element"></param>
        /// <param name="changeTableIndex"></param>
        public void SaveExisting(ChildElement oldElement, ChildElement element, int changeTableIndex)
        {
            base.SaveExisting(oldElement, element);
        }

        /// <summary>
        /// Reads the tables and creates all the conditions.
        /// </summary>
        public override void Load()
        {
            List<ChildElement> iasElems = CreateElementsFromRows(TableName, rowData => CreateElementFromRowData(rowData));
            foreach (IASElementSet elem in iasElems)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        #region logging

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
        #endregion

        private string WasAnalyticalFrequencyElementModified(IASElementSet iasElems,ChildElement elem, int elemID )
        {
            List<SpecificIAS> iasList = iasElems.SpecificIASElements.Where(ias => ias.FlowFreqID == elemID).ToList();
            return iasList.Count > 0 ? CreateTooltipMessage("Analytical Frequency", elem.Name) : null;
        }

        private string WasInflowOutflowModified(IASElementSet iasElems, ChildElement elem, int elemID)
        {
            List<SpecificIAS> iasList = iasElems.SpecificIASElements.Where(ias => ias.InflowOutflowID == elemID).ToList();
            return iasList.Count > 0 ? CreateTooltipMessage("Inflow-Outflow", elem.Name) : null;
        }

        private string WasRatingModified(IASElementSet iasElems, ChildElement elem, int elemID)
        {
            List<SpecificIAS> iasList = iasElems.SpecificIASElements.Where(ias => ias.RatingID == elemID).ToList();
            return iasList.Count > 0 ? CreateTooltipMessage("Rating", elem.Name) : null;
        }

        private string WasExteriorInteriorModified(IASElementSet iasElems, ChildElement elem, int elemID)
        {
            List<SpecificIAS> iasList = iasElems.SpecificIASElements.Where(ias => ias.ExtIntStageID == elemID).ToList();
            return iasList.Count > 0 ? CreateTooltipMessage("Exterior-Interior Stage", elem.Name) : null;
        }

        private string WasStageDamageModified(IASElementSet iasElems, ChildElement elem, int elemID)
        {
            List<SpecificIAS> iasList = iasElems.SpecificIASElements.Where(ias => ias.StageDamageID == elemID).ToList();
            return iasList.Count > 0 ? CreateTooltipMessage("Aggregates Stage-Damage", elem.Name) : null;
        }

        private string WasLeveeElementModified(IASElementSet iasElems, ChildElement elem, int elemID)
        {
            List<SpecificIAS> iasList = iasElems.SpecificIASElements.Where(ias => ias.LeveeFailureID == elemID).ToList();
            return iasList.Count > 0 ? CreateTooltipMessage("Levee-Failure", elem.Name) : null;
        }

        private string CreateTooltipMessage(string curveType, string curveName)
        {
            return curveType + " " + curveName + " was modified since last save.";
        }

        /// <summary>
        /// The elemID is required here because the modified element, if it was removed, no longer has a valid id.
        /// </summary>
        /// <param name="iasSet"></param>
        /// <param name="elemModified"></param>
        /// <param name="elemID"></param>
        private void CheckIASSetForBaseDataModified(IASElementSet iasSet, ChildElement elemModified, int elemID)
        {
            string msg = null;

            if (elemModified is AnalyticalFrequencyElement)
            {
                msg = WasAnalyticalFrequencyElementModified(iasSet,elemModified, elemID);
            }
            else if (elemModified is InflowOutflowElement)
            {
                msg = WasInflowOutflowModified(iasSet, elemModified, elemID);
            }
            else if (elemModified is RatingCurveElement)
            {
                msg = WasRatingModified(iasSet, elemModified, elemID);
            }
            else if (elemModified is LeveeFeatureElement)
            {
                msg = WasLeveeElementModified(iasSet, elemModified, elemID);
            }
            else if (elemModified is ExteriorInteriorElement)
            {
                msg = WasExteriorInteriorModified(iasSet, elemModified, elemID);
            }
            else if (elemModified is AggregatedStageDamageElement)
            {
                msg = WasStageDamageModified(iasSet, elemModified, elemID);
            }
            if (msg != null)
            {
                iasSet.ToolTip = msg;
                iasSet.UpdateTreeViewHeader(iasSet.Name + "*");
            }
        }

        /// <summary>
        /// This will update the condition element in the database. This will trigger
        /// an update to the study cache and the study tree as well.
        /// </summary>
        /// <param name="elem">The child element that has been removed</param>
        /// <param name="originalID">The original id </param>
        public void UpdateIASTooltipsChildElementModified(ChildElement elem, int originalID)
        {
            List<IASElementSet> conditionsElements = StudyCache.GetChildElementsOfType<IASElementSet>();

            foreach(IASElementSet set in conditionsElements)
            {
                CheckIASSetForBaseDataModified(set, elem, originalID);
            }

        }

    }
}
