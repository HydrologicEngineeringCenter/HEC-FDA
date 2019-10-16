using FdaViewModel.AggregatedStageDamage;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class StageDamagePersistenceManager : UndoRedoBase, IPersistableWithUndoRedo
    {
        //ELEMENT_TYPE is used to store the type in the log tables. Initially i was actually storing the type
        //of the element. But since they get stored as strings if a developer changes the name of the class
        //you would no longer get any of the old logs. So i use this constant.
        private const string ELEMENT_TYPE = "Stage_Damage";
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("StageDamagePersistenceManager");


        private const string TABLE_NAME = "stage_damage_relationships";
        internal override string ChangeTableConstant { get { return "Aggregated Stage Damage Function - "; } }
        private static readonly string[] TableColNames = { NAME, LAST_EDIT_DATE, DESCRIPTION, CURVE_DISTRIBUTION_TYPE, "creation_method", CURVE };
    

    private static readonly Type[] TableColTypes = { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) };
        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return TableColTypes; }
        }

        public override string TableName { get { return TABLE_NAME; } }

        /// <summary>
        /// The name of the change table that will hold the various states of elements.
        /// </summary>
        public override string ChangeTableName { get { return "stage_damage_changes"; } }

        /// <summary>
        /// Names of the columns in the change table
        /// </summary>
        public override string[] ChangeTableColumnNames
        {
            get
            {
                return new string[] { ELEMENT_ID_COL_NAME, NAME, LAST_EDIT_DATE, DESCRIPTION, CURVE_DISTRIBUTION_TYPE, "creation_method", CURVE, STATE_INDEX_COL_NAME };
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


        public override string[] TableColumnNames
        {
            get
            {
                return TableColNames;
            }
        }

        public StageDamagePersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }



        #region utilities
        private object[] GetRowDataFromElement(AggregatedStageDamageElement element)
        {
            return new object[] { element.Name, element.LastEditDate, element.Description, element.Curve.Distribution, element.Method, ExtentionMethods.CreateXMLCurveString(element.Curve.Distribution, element.Curve.XValues, element.Curve.YValues) };

        }
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            //todo: make the ints constancts at the top of the class
            Statistics.UncertainCurveDataCollection emptyCurve = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[4]));
            AggregatedStageDamageElement asd = new AggregatedStageDamageElement((string)rowData[1], (string)rowData[2], (string)rowData[3], emptyCurve, (CreationMethodEnum)Enum.Parse(typeof(CreationMethodEnum), (string)rowData[5]));
            //asd.Curve.fromSqliteTable(ChangeTableConstant + (string)rowData[1]);
            asd.Curve = ExtentionMethods.GetCurveFromXMLString((string)rowData[6], (Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[4]));
            return asd;
        }
        #endregion


        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(AggregatedStageDamageElement))
            {
                //save to parent table
                SaveNewElement(element);
                //save to change table
                SaveToChangeTable(element);
                //log message
                Log(FdaLogging.LoggingLevel.Info, "Created new stage damage curve: " + element.Name, element.Name);
            }
        }
        public void Remove(ChildElement element)
        {
            //RemoveFromParentTable(element, TableName);
            //DeleteChangeTableAndAssociatedTables(element, ChangeTableConstant);
            //StudyCacheForSaving.RemoveElement((AggregatedStageDamageElement)element);
        }
        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex)
        {
            if (elementToSave.Description == null) { elementToSave.Description = ""; }

            if (DidParentTableRowValuesChange(elementToSave, GetRowDataFromElement((AggregatedStageDamageElement)elementToSave), oldElement.Name, TableName) || AreCurvesDifferent(oldElement.Curve, elementToSave.Curve))
            {
                base.SaveExisting(oldElement, elementToSave, changeTableIndex);
            }
        }

        public void Load()
        {
            List<ChildElement> stageDamages = CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (AggregatedStageDamageElement elem in stageDamages)
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

        public override object[] GetRowDataForChangeTable(ChildElement element)
        {

            if (element.Description == null)
            {
                element.Description = "";
            }

            int elemId = GetElementId(TableName, element.Name);
            //the new statId will be one higher than the max that is in the table already.
            int stateId = Storage.Connection.Instance.GetMaxStateIndex(ChangeTableName, elemId, ELEMENT_ID_COL_NAME, STATE_INDEX_COL_NAME) + 1;
            return new object[] {elemId, element.Name, element.LastEditDate, element.Description,
                element.Curve.Distribution, ((AggregatedStageDamageElement)element).Method,
                ExtentionMethods.CreateXMLCurveString(element.Curve.Distribution, element.Curve.XValues, element.Curve.YValues),
                stateId};
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            return GetRowDataFromElement((AggregatedStageDamageElement)elem);
        }
    }
}
