using FdaViewModel.GeoTech;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class LeveePersistenceManager : SavingBase, IElementManager
    {
        //ELEMENT_TYPE is used to store the type in the log tables. Initially i was actually storing the type
        //of the element. But since they get stored as strings if a developer changes the name of the class
        //you would no longer get any of the old logs. So i use this constant.
        private const string ELEMENT_TYPE = "levee";
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("LeveePersistenceManager");


        private const string TABLE_NAME = "levee_features";
        private static readonly string[] TableColNames = { NAME, DESCRIPTION, "elevation" };
        private static readonly Type[] TableColTypes = { typeof(string), typeof(string), typeof(double) };
        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return TableColTypes; }
        }
        internal override string ChangeTableConstant
        {
           get { return ""; }
        }

        public override string TableName
        {
            get { return TABLE_NAME; }
        }

        public override string[] TableColumnNames
        {
            get { return TableColNames; }
        }


        public LeveePersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        private object[] GetRowDataFromElement(LeveeFeatureElement element)
        {
            return new object[] { element.Name, element.Description, element.Elevation };

        }
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            //todo: make constants
            return new LeveeFeatureElement((string)rowData[1], (string)rowData[2], Convert.ToDouble( rowData[3]));
        }
        #endregion

        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(LeveeFeatureElement))
            {
                SaveNewElementToParentTable(GetRowDataFromElement((LeveeFeatureElement)element), TableName, TableColumnNames, TableColumnTypes);
                StudyCacheForSaving.AddElement((LeveeFeatureElement)element);
            }
        }
        public void Remove(ChildElement element)
        {
            RemoveFromParentTable(element, TableName);
            StudyCacheForSaving.RemoveElement((LeveeFeatureElement)element);

        }
        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex  )
        {
            if (DidParentTableRowValuesChange(elementToSave, GetRowDataFromElement((LeveeFeatureElement)elementToSave), oldElement.Name, TableName))
            {
                base.SaveExisting(oldElement, elementToSave);
            }
        }

        public void Load()
        {
            List<ChildElement> levees = CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (LeveeFeatureElement elem in levees)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
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

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            return GetRowDataFromElement((LeveeFeatureElement)elem);
        }
    }
}
