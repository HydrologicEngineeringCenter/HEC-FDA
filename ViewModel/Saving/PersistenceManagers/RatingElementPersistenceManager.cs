using paireddata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class RatingElementPersistenceManager : SavingBase
    {
        private const int LAST_EDIT_DATE_COL = 2;
        private const int DESC_COL = 3;
        private const int CURVE_COL = 4;

        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("RatingElementPersistenceManager");
        //ELEMENT_TYPE is used to store the type of element in the log tables.
        private const string ELEMENT_TYPE = "rating_curve";
        /// <summary>
        /// The name of the parent table that will hold all elements of this type
        /// </summary>
        public override string TableName { get { return "rating_curves"; } }

        /// <summary>
        /// Names of the columns in the parent table
        /// </summary>
        public override string[] TableColumnNames
        {
            get {return new string[]{ NAME, LAST_EDIT_DATE, DESCRIPTION, CURVE};}
        }
        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return new Type[]{ typeof(string), typeof(string), typeof(string), typeof(string) }; }
        }      

        #region constructor
        public RatingElementPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #endregion

        #region utilities

        /// <summary>
        /// Turns the element into an object[] for the row in the parent table
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override object[] GetRowDataFromElement(ChildElement element)
        {
            if(element.Description == null)
            {
                element.Description = "";
            }
            
            return new object[] { element.Name, element.LastEditDate, element.Description, element.Curve.WriteToXML().ToString()};
        }
       
        /// <summary>
        /// Creates an element from the row in the parent table.
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            string curveXML = (string)rowData[CURVE_COL];
            UncertainPairedData upd = UncertainPairedData.ReadFromXML(XElement.Parse(curveXML));
            RatingCurveElement rc = new RatingCurveElement((string)rowData[NAME_COL], (string)rowData[LAST_EDIT_DATE_COL],
                (string)rowData[DESC_COL], upd);
            return rc;
        }

        #endregion

        /// <summary>
        /// Saves a new element to the parent table and the change table
        /// </summary>
        /// <param name="element"></param>
        public void SaveNew(ChildElement element)
        {
            //save to parent table
            base.SaveNew(element);
            //log message
            Log(FdaLogging.LoggingLevel.Info, "Created new rating curve: " + element.Name, element.Name);
        }
       
        /// <summary>
        /// Remove the element from the parent table, all references to it in the change table, and all references to it in the log tables.
        /// </summary>
        /// <param name="element"></param>
        public void Remove(ChildElement element)
        {
            base.Remove(element);
        }

        /// <summary>
        /// Loads the elements from the parent table and puts them into the study cache.
        /// </summary>
        public override void Load()
        {
            List<ChildElement> ratings = CreateElementsFromRows( TableName, rowData => CreateElementFromRowData(rowData));
            foreach (RatingCurveElement elem in ratings)
            {
                StudyCacheForSaving.AddElement(elem);
            }
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
            return FdaLogging.RetrieveFromDB.GetLogMessages( id, ELEMENT_TYPE);
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
    }
}
