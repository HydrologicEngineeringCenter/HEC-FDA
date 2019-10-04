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
    public class FailureFunctionPersistenceManager : UndoRedoBase, IPersistableWithUndoRedo
    {
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("FailureFunctionPersistenceManager");
        //ELEMENT_TYPE is used to store the type of element in the log tables.
        private const string ELEMENT_TYPE = "failure_function";
        public override string TableName { get { return "failure_functions"; } }

        public override string ChangeTableName { get { return "failure_function_changes"; } }


        internal override string ChangeTableConstant { get { return "Failure Function - "; } }
        //todo change to string constants in base class

        /// <summary>
        /// Names of the columns in the parent table
        /// </summary>
        public override string[] TableColumnNames
        {
            get
            {
                return new string[] { NAME, LAST_EDIT_DATE, DESCRIPTION,"associated_levee_feature", CURVE_DISTRIBUTION_TYPE, CURVE };
            }
        }
        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) }; }
        }

        /// <summary>
        /// Names of the columns in the change table
        /// </summary>
        public override string[] ChangeTableColumnNames
        {
            get
            {//todo make a constant string
                return new string[] { ELEMENT_ID_COL_NAME, NAME, LAST_EDIT_DATE, DESCRIPTION, "associated_levee_feature", CURVE_DISTRIBUTION_TYPE, CURVE, STATE_INDEX_COL_NAME };
            }
        }

        /// <summary>
        /// Types for the columns in the change table
        /// </summary>
        public override Type[] ChangeTableColumnTypes
        {
            get
            {
                return new Type[]{ typeof(int), typeof(string), typeof(string), typeof(string), typeof(string),
                    typeof(string), typeof(string), typeof(int) };
            }
        }

        public FailureFunctionPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        public override object[] GetRowDataFromElement(ChildElement element)
        {
            if (element.Description == null)
            {
                element.Description = "";
            }
            FailureFunctionElement elem = (FailureFunctionElement)element;
            return new object[] { element.Name, element.LastEditDate, element.Description, elem.SelectedLateralStructure.Name, element.Curve.Distribution, ExtentionMethods.CreateXMLCurveString(element.Curve.Distribution, element.Curve.XValues, element.Curve.YValues) };

        }

        /// <summary>
        /// Turns the element into an object[] for the row in the change table
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override object[] GetRowDataForChangeTable(ChildElement element)
        {
            if (element.Description == null)
            {
                element.Description = "";
            }
            FailureFunctionElement elem = (FailureFunctionElement)element;
            int elemId = GetElementId(TableName, element.Name);
            //the new statId will be one higher than the max that is in the table already.
            int stateId = Storage.Connection.Instance.GetMaxStateIndex(ChangeTableName, elemId, ELEMENT_ID_COL_NAME, STATE_INDEX_COL_NAME) + 1;
            return new object[] {elemId, element.Name, element.LastEditDate, element.Description, elem.SelectedLateralStructure.Name,
                element.Curve.Distribution,
                ExtentionMethods.CreateXMLCurveString(element.Curve.Distribution, element.Curve.XValues, element.Curve.YValues),
                stateId};

        }


        /// <summary>
        /// Creates an element from the row in the parent table.
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            List<LeveeFeatureElement> ele = StudyCacheForSaving.LeveeElements;
            LeveeFeatureElement lfe = null;
            foreach (LeveeFeatureElement element in ele)
            {
                if (element.Name == (string)rowData[3])
                {
                    lfe = element;
                }
            }
            Statistics.UncertainCurveDataCollection ucdc = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[4]));
            FailureFunctionElement failure = new FailureFunctionElement((string)rowData[0], (string)rowData[1], (string)rowData[2], ucdc, lfe);
            //failure.Curve.fromSqliteTable(ChangeTableConstant + (string)rowData[1]);
            failure.Curve = ExtentionMethods.GetCurveFromXMLString((string)rowData[5], (Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[4]));
            return failure;
        }
        #endregion


        /// <summary>
        /// Saves a new element to the parent table and the change table
        /// </summary>
        /// <param name="element"></param>
        public void SaveNew(ChildElement element)
        {
            //save to parent table
            SaveNewElement(element);
            //save to change table
            SaveToChangeTable(element);
            //log message
            Log(FdaLogging.LoggingLevel.Info, "Created new failure function: " + element.Name, element.Name);

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
        /// Update the row in the parent table and add row to the change table. Update element in the study cache.
        /// </summary>
        /// <param name="oldElement"></param>
        /// <param name="elementToSave"></param>
        /// <param name="changeTableIndex"></param>
        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex  )
        {
            //this will save to the parent table and to the change table
            base.SaveExisting(oldElement, elementToSave, changeTableIndex);
            //log that we are saving
            if (!oldElement.Name.Equals(elementToSave.Name))
            {
                Log(FdaLogging.LoggingLevel.Info, "Saved rating curve with name change from " + oldElement.Name +
                    " to " + elementToSave.Name + ".", elementToSave.Name);
            }
            else
            {
                Log(FdaLogging.LoggingLevel.Info, "Saved rating curve: " + elementToSave.Name, elementToSave.Name);
            }
        }

        /// <summary>
        /// Loads the elements from the parent table and puts them into the study cache.
        /// </summary>
        public void Load()
        {
            List<ChildElement> failures = CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (FailureFunctionElement elem in failures)
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

       
    }
}
