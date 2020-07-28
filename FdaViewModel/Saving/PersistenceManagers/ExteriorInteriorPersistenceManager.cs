using FdaViewModel.StageTransforms;
using FdaViewModel.Utilities;
using Functions;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class ExteriorInteriorPersistenceManager : UndoRedoBase, IPersistableWithUndoRedo
    {
        private const int NAME_COL = 1;
        private const int LAST_EDIT_DATE_COL = 2;
        private const int DESC_COL = 3;
        private const int CURVE_DIST_TYPE_COL = 4;
        private const int CURVE_TYPE_COL = 5;
        private const int CURVE_COL = 6;

        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("ExteriorInteriorPersistenceManager");
        //ELEMENT_TYPE is used to store the type of element in the log tables.
        private const string ELEMENT_TYPE = "exterior_interior";
        /// <summary>
        /// The name of the parent table that will hold all elements of this type
        /// </summary>
        public override string TableName { get { return "exterior_interior_curves"; } }

        /// <summary>
        /// The name of the change table that will hold the various states of elements.
        /// </summary>
        public override string ChangeTableName { get { return "exterior_interior_changes"; } }

        


        /// <summary>
        /// Names of the columns in the parent table
        /// </summary>
        public override string[] TableColumnNames
        {
            get
            {
               
                return new string[] {NAME, LAST_EDIT_DATE, DESCRIPTION, CURVE_DISTRIBUTION_TYPE, CURVE_TYPE, CURVE};
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
            {
                return new string[] { ELEMENT_ID_COL_NAME, NAME, LAST_EDIT_DATE, DESCRIPTION, CURVE_DISTRIBUTION_TYPE, CURVE_TYPE, CURVE, STATE_INDEX_COL_NAME };
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

        public ExteriorInteriorPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }


        #region utilities
        public override object[] GetRowDataFromElement(ChildElement element)
        {
            //todo: why are all these properties on child element. I was expecting to have to cast this element to an ext int.
            return new object[] { element.Name, element.LastEditDate, element.Description,
                element.Curve.DistributionType, element.Curve.GetType(),
                element.Curve.WriteToXML().ToString() };
        }

        public override object[] GetRowDataForChangeTable(ChildElement element)
        {
            if (element.Description == null)
            {
                element.Description = "";
            }

            int id = GetElementId(TableName, element.Name);
            //the new statId will be one higher than the max that is in the table already.
            int stateId = Storage.Connection.Instance.GetMaxStateIndex(ChangeTableName, id, ELEMENT_ID_COL_NAME, STATE_INDEX_COL_NAME) + 1;
            return new object[] {id, element.Name, element.LastEditDate, element.Description,
                element.Curve.DistributionType, element.Curve.GetType(),
                element.Curve.WriteToXML().ToString(), stateId};

        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            ICoordinatesFunction coordinatesFunction = ICoordinatesFunctionsFactory.Factory((String)rowData[CURVE_COL]);
            IFunction func = IFunctionFactory.Factory(coordinatesFunction.Coordinates, coordinatesFunction.Interpolator);
            IFdaFunction function = IFdaFunctionFactory.Factory(func, IParameterEnum.ExteriorInteriorStage);

            //Statistics.UncertainCurveIncreasing emptyCurve = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum),
            //(string)rowData[CURVE_DIST_TYPE_COL]));
            ExteriorInteriorElement ele = new ExteriorInteriorElement((string)rowData[CHANGE_TABLE_NAME_INDEX], (string)rowData[LAST_EDIT_DATE_COL],
                (string)rowData[DESC_COL], function);
            //ele.Curve = ExtentionMethods.GetCurveFromXMLString((string)rowData[CURVE_COL], (Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum),
              //              (string)rowData[CURVE_DIST_TYPE_COL])); 
            return ele;
        }
        #endregion


        public void SaveNew(ChildElement element)
        {
            //save to parent table
            SaveNewElement(element);
            //save to change table
            SaveToChangeTable(element);
            //log message
            Log(FdaLogging.LoggingLevel.Info, "Created new exterior interior curve: " + element.Name, element.Name);

        }

        public void Remove(ChildElement element)
        {
            base.Remove(element);
        }

       
        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex )
        {
            //this will save to the parent table and to the change table
            base.SaveExisting(oldElement, elementToSave, changeTableIndex);
            //log that we are saving
            if (!oldElement.Name.Equals(elementToSave.Name))
            {
                Log(FdaLogging.LoggingLevel.Info, "Saved exterior interior curve with name change from " + oldElement.Name +
                    " to " + elementToSave.Name + ".", elementToSave.Name);
            }
            else
            {
                Log(FdaLogging.LoggingLevel.Info, "Saved exterior interior curve: " + elementToSave.Name, elementToSave.Name);
            }
        }

        public void Load()
        {
            List<ChildElement> exteriorInteriors = CreateElementsFromRows( TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (ExteriorInteriorElement elem in exteriorInteriors)
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
