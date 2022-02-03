using paireddata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using ViewModel.StageTransforms;
using ViewModel.Storage;
using ViewModel.Utilities;

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
            
            return new object[] { element.Name, element.LastEditDate, element.Description, element.Curve, 
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
            int stateId = Connection.Instance.GetMaxStateIndex(ChangeTableName, elemId, ELEMENT_ID_COL_NAME, STATE_INDEX_COL_NAME) + 1;
            return new object[] {elemId, element.Name, element.LastEditDate, element.Description,
                element.Curve, element.Curve.GetType(),
                element.Curve.WriteToXML().ToString(), stateId};

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

            //ICoordinatesFunction coordinatesFunction = ICoordinatesFunctionsFactory.Factory((String)rowData[CURVE_COL]);
            //IFdaFunction function = IFdaFunctionFactory.Factory( IParameterEnum.Rating, coordinatesFunction);
            RatingCurveElement rc = new RatingCurveElement((string)rowData[CHANGE_TABLE_NAME_INDEX], (string)rowData[LAST_EDIT_DATE_COL],
                (string)rowData[DESC_COL], upd);
            return rc;
        }

        #endregion

        #region Import from old fda
        //public void SaveFDA1Elements(RatingFunctionList ratingCurves)
        //{
        //    foreach (KeyValuePair<string, RatingFunction> rat in ratingCurves.RatingFunctions)
        //    {
        //        SaveRatingFunction(rat.Value);
        //    }
        //}

        //private void SaveRatingFunction(RatingFunction rat)
        //{
        //    string pysr = "(" + rat.PlanName + " " + rat.YearName + " " + rat.StreamName + " " + rat.DamageReachName + ") ";
        //    string description = pysr + rat.Description;
        //    double[] stages = rat.GetStage();
        //    double[] flows = rat.GetDischarge();
        //    //these arrays might have a bunch of "Study.badNumber" (-901). I need to get rid of them by only grabbing the correct number of points.
        //    List<double> stagesList = new List<double>();
        //    List<double> flowsList = new List<double>();
        //    for (int i = 0; i < rat.NumberOfPoints; i++)
        //    {
        //        stagesList.Add(stages[i]);
        //        flowsList.Add(flows[i]);
        //    }
        //    //always use linear. This is the only option in Old Fda.
        //    ICoordinatesFunction func = Functions.ICoordinatesFunctionsFactory.Factory(stagesList, flowsList, InterpolationEnum.Linear);
        //    IFdaFunction rating = IFdaFunctionFactory.Factory(IParameterEnum.Rating, (IFunction)func);
        //    //add the plan year stream reach for the description
        //    RatingCurveElement elem = new RatingCurveElement(rat.Name, rat.CalculationDate, description, rating);
        //    SaveNew(elem);
        //}


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
