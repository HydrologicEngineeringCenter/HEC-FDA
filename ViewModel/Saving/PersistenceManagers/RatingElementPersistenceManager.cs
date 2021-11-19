using ViewModel.StageTransforms;
using ViewModel.Utilities;
using Functions;
using Model;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Saving.PersistenceManagers
{
    public class RatingElementPersistenceManager :UndoRedoBase, IPersistableWithUndoRedo
    {
        private const int NAME_COL = 1;
        private const int LAST_EDIT_DATE_COL = 2;
        private const int DESC_COL = 3;
        private const int CURVE_DIST_TYPE_COL = 4;
        private const int CURVE_TYPE_COL = 5;
        private const int CURVE_COL = 6;

        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("RatingElementPersistenceManager");
        //ELEMENT_TYPE is used to store the type of element in the log tables.
        private const string ELEMENT_TYPE = "rating_curve";
        /// <summary>
        /// The name of the parent table that will hold all elements of this type
        /// </summary>
        public override string TableName { get { return "rating_curves"; } }
        /// <summary>
        /// The name of the change table that will hold the various states of elements.
        /// </summary>
        public override string ChangeTableName { get { return "rating_curve_changes"; } }

     

        /// <summary>
        /// Names of the columns in the parent table
        /// </summary>
        public override string[] TableColumnNames
        {
            get
            {
                return new string[]{ NAME, LAST_EDIT_DATE, DESCRIPTION, CURVE_DISTRIBUTION_TYPE, CURVE_TYPE, CURVE};
            }
        }
        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return new Type[]{ typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) }; }
        }



        /// <summary>
        /// Names of the columns in the change table
        /// </summary>
        public override string[] ChangeTableColumnNames
        {
            get
            {
                return new string[]{ ELEMENT_ID_COL_NAME,NAME, LAST_EDIT_DATE, DESCRIPTION, CURVE_DISTRIBUTION_TYPE, CURVE_TYPE, CURVE , STATE_INDEX_COL_NAME};
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
            
            return new object[] { element.Name, element.LastEditDate, element.Description, element.Curve.DistributionType, 
                element.Curve.GetType(), element.Curve.WriteToXML().ToString()};

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

            int elemId = GetElementId(TableName, element.Name);
            //the new stateId will be one higher than the max that is in the table already.
            int stateId = Storage.Connection.Instance.GetMaxStateIndex(ChangeTableName, elemId, ELEMENT_ID_COL_NAME, STATE_INDEX_COL_NAME) + 1;
            return new object[] {elemId, element.Name, element.LastEditDate, element.Description,
                element.Curve.DistributionType, element.Curve.GetType(),
                element.Curve.WriteToXML().ToString(), stateId};

        }
       
        /// <summary>
        /// Creates an element from the row in the parent table.
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            ICoordinatesFunction coordinatesFunction = ICoordinatesFunctionsFactory.Factory((String)rowData[CURVE_COL]);
            //IFunction func = IFunctionFactory.Factory(coordinatesFunction.Coordinates, coordinatesFunction.Interpolator);
            IFdaFunction function = IFdaFunctionFactory.Factory( IParameterEnum.Rating, coordinatesFunction);

            //Statistics.UncertainCurveIncreasing emptyCurve = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum),
                //(string)rowData[CURVE_DIST_TYPE_COL]));
            RatingCurveElement rc = new RatingCurveElement((string)rowData[CHANGE_TABLE_NAME_INDEX], (string)rowData[LAST_EDIT_DATE_COL],
                (string)rowData[DESC_COL], function);
            //rc.Curve = ExtentionMethods.GetCurveFromXMLString((string)rowData[CURVE_COL], (Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum),
            //    (string)rowData[CURVE_DIST_TYPE_COL]));
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
            SaveNewElement(element);
            //save to change table
            SaveToChangeTable(element);
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
        /// Update the row in the parent table and add row to the change table. Update element in the study cache.
        /// </summary>
        /// <param name="oldElement"></param>
        /// <param name="elementToSave"></param>
        /// <param name="changeTableIndex"></param>
        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave,int changeTableIndex )
        {
            //this will save to the parent table and to the change table
            base.SaveExisting(oldElement, elementToSave, changeTableIndex);
            //log that we are saving
            //if (!oldElement.Name.Equals(elementToSave.Name))
            //{
            //    Log(FdaLogging.LoggingLevel.Info, "Saved rating curve with name change from " + oldElement.Name +
            //        " to " + elementToSave.Name + ".", elementToSave.Name);
            //}
            //else
            //{
            //    Log(FdaLogging.LoggingLevel.Info, "Saved rating curve: " + elementToSave.Name, elementToSave.Name);
            //}
            //UpdateLastSaved(elementToSave.Name);
        }

        /// <summary>
        /// Loads the elements from the parent table and puts them into the study cache.
        /// </summary>
        public void Load()
        {
            List<ChildElement> ratings = CreateElementsFromRows( TableName, rowData => CreateElementFromRowData(rowData));
            foreach (RatingCurveElement elem in ratings)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        //private void UpdateLastSaved(string elementName)
        //{
        //    int elementId = GetElementId(TableName, elementName);
        //    LOGGER.UpdateLastSaved( ELEMENT_TYPE, elementId);
            
        //}

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
            return FdaLogging.RetrieveFromDB.GetLogMessages( id, ELEMENT_TYPE);
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
