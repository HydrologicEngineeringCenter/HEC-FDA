using FdaLogging;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class LeveePersistenceManager : SavingBase
    {
        private const int LAST_EDIT_DATE_COL = 2;
        private const int DESC_COL = 3;
        private const int ELEVATION_COL = 4;
        private const int IS_DEFAULT_COL = 5;
        private const int CURVE_COL = 6;

        //ELEMENT_TYPE is used to store the type in the log tables. Initially i was actually storing the type
        //of the element. But since they get stored as strings if a developer changes the name of the class
        //you would no longer get any of the old logs. So i use this constant.
        private const string ELEMENT_TYPE = "levee";
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("LeveePersistenceManager");

        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return new Type[] { typeof(string), typeof(string), typeof(string), typeof(double), typeof(bool), typeof(string)  }; }
        }
        internal override string ChangeTableConstant
        {
           get { return ""; }
        }

        public override string TableName
        {
            get { return "levee_features"; }
        }

        public override string[] TableColumnNames
        {
            get { return new string[] { NAME,LAST_EDIT_DATE, DESCRIPTION, "elevation","is_default", CURVE }; }
        }

        public LeveePersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        /// <summary>
        /// Turns the element into an object[] for the row in the parent table
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override object[] GetRowDataFromElement(ChildElement element)
        {
            //todo bad casting
            CurveChildElement curveChildElement = element as CurveChildElement; 

            return new object[] { element.Name, element.LastEditDate, element.Description, ((LeveeFeatureElement)element).Elevation, ((LeveeFeatureElement)element).IsDefaultCurveUsed, curveChildElement.ComputeComponentVM.ToXML().ToString() };
        }

        /// <summary>
        /// Creates an element from the row in the parent table.
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            bool isDefault = Convert.ToBoolean(rowData[IS_DEFAULT_COL]);
            string curveXML = (string)rowData[CURVE_COL];
            //UncertainPairedData upd = UncertainPairedData.ReadFromXML(XElement.Parse(curveXML));
            ComputeComponentVM computeComponentVM = new ComputeComponentVM(XElement.Parse(curveXML));

            int id = Convert.ToInt32(rowData[ID_COL]);
            return new LeveeFeatureElement((string)rowData[NAME_COL], (string)rowData[LAST_EDIT_DATE_COL], (string)rowData[DESC_COL], Convert.ToDouble( rowData[ELEVATION_COL]), isDefault, computeComponentVM, id);
        }

        #endregion

        public void SaveNew(ChildElement element)
        {
            base.SaveNew(element);
            Log(LoggingLevel.Info, "Created new levee failure element: " + element.Name, element.Name);
        }
        public void Remove(ChildElement element)
        {
            base.Remove(element);
        }

        public override void Load()
        {
            List<ChildElement> levees = CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (LeveeFeatureElement elem in levees)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public ObservableCollection<LogItem> GetLogMessages(ChildElement element)
        {
            return new ObservableCollection<LogItem>();
        }

        /// <summary>
        /// This will put a log into the log tables. Logs are only unique by element id and
        /// element type. ie. Rating Curve id=3.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="elementName"></param>
        public override void Log(LoggingLevel level, string message, string elementName)
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
        public override ObservableCollection<LogItem> GetLogMessages(string elementName)
        {
            int id = GetElementId(TableName, elementName);
            return RetrieveFromDB.GetLogMessages(id, ELEMENT_TYPE);
        }

        /// <summary>
        /// Gets all the log messages for this element from the specified log level table.
        /// This is used by the MessageExpander to filter by log level
        /// </summary>
        /// <param name="level"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public override ObservableCollection<LogItem> GetLogMessagesByLevel(LoggingLevel level, string elementName)
        {
            int id = GetElementId(TableName, elementName);
            return RetrieveFromDB.GetLogMessagesByLevel(level, id, ELEMENT_TYPE);
        }
    }
}
