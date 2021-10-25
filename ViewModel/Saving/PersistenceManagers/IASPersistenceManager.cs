
using ViewModel.AggregatedStageDamage;
using ViewModel.ImpactAreaScenario;
using ViewModel.FlowTransforms;
using ViewModel.FrequencyRelationships;
using ViewModel.GeoTech;
using ViewModel.ImpactArea;
using ViewModel.StageTransforms;
using ViewModel.Utilities;
using Functions;
using Model;
using Model.Conditions.Locations;
using Model.Conditions.Locations.Years;
using Model.Conditions.Locations.Years.Realizations;
using Model.Conditions.Locations.Years.Results;
using Model.Samples;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using ViewModel.ImpactAreaScenario.Editor;
using System.Xml.Linq;

namespace ViewModel.Saving.PersistenceManagers
{

    //Cleaned 10/20/20

    //todo: Are we going to do a change table for conditions?
    //todo: i think eventually i need to be writing something out for the criteria but for now it is just an empty ctor so
    //i don't need to write anything out
    //todo: is the "log" methods necessary in here?

    /// <summary>
    /// The persistence manager for conditions. All database interaction for a condition object should go through here.
    /// </summary>
    public class IASPersistenceManager : SavingBase, IElementManager
    {
        private const int ID_COL = 0;
        private const int NAME_COL = 1;
        private const int DESC_COL = 2;
        private const int ANALYSIS_YEAR_COL = 3;
        private const int IMPACT_AREA_COL = 4;
        private const int FLOW_FREQ_COL = 5;
        private const int INFLOW_OUTFLOW_COL = 6;
        private const int RATING_COL = 7;
        private const int LEVEE_FAILURE_COL = 8;
        private const int EXT_INT_COL = 9;
        private const int STAGE_DAMAGE_COL = 10;
        private const int THRESHOLDS_COL = 11;
        //private const int THRESHOLD_VALUE_COL = 12;
        private const int SEED_COL = 12;

        //ELEMENT_TYPE is used to store the type in the log tables. Initially i was actually storing the type
        //of the element. But since they get stored as strings if a developer changes the name of the class
        //you would no longer get any of the old logs. So i use this constant.
        private const string ELEMENT_TYPE = "Condition";
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("ConditionsPersistenceManager");

        private const string TABLE_NAME = "Conditions";
        internal override string ChangeTableConstant { get { return "????"; } }
        private static readonly string[] ColumnNames =
            { "Name","XML"};

        private static readonly Type[] TableColTypes =
            { typeof(string), typeof(string)};


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
            IASElementSet element = (IASElementSet)elem;
            //int seed = 999; //todo: make this the actual seed.
            //string thresholds = WriteThresholdsToXML(element.Thresholds).ToString();
            //object[] retval = new object[] { element.Name, element.Description, element.AnalysisYear, element.ImpactAreaID,
            //    element.FlowFreqID, element.InflowOutflowID, element.RatingID,
            //    element.LeveeFailureID, element.ExtIntStageID, element.StageDamageID, thresholds, seed};
            object[] retval = new object[] {element.Name, element.WriteToXML() };
            return retval;
        }

        private IMetricEnum ConvertStringToMetricEnum(string metric)
        {
            switch (metric.ToUpper())
            {
                case "NOTSET":
                    {
                        return IMetricEnum.NotSet;
                    }
                case "EXTERIORSTAGEAEP":
                case "EXTERIORSTAGE":
                    {
                        return IMetricEnum.ExteriorStage;
                    }
                case "INTERIORSTAGEAEP":
                case "INTERIORSTAGE":
                    {
                        return IMetricEnum.InteriorStage;
                    }
                case "DAMAGEAEP":
                case "DAMAGES":
                    {
                        return IMetricEnum.Damages;
                    }
                case "EAD":
                case "EXPECTEDANNUALDAMAGE":
                    {
                        return IMetricEnum.ExpectedAnnualDamage;
                    }
            }
            throw new Exception("Could not convert string: " + metric + " to an IMetricEnum.");
        }

        /// <summary>
        /// Converts the row in the main table into an actual condition element.
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            string xml = (string)rowData[2];
            IASElementSet elem = new IASElementSet(xml);
            return elem;

            //string name = (string)rowData[NAME_COL];
            //string description = (string)rowData[DESC_COL];
            //int year = Convert.ToInt32(rowData[ANALYSIS_YEAR_COL]);
            //int impAreaID = Convert.ToInt32(rowData[IMPACT_AREA_COL]);
            //int flowFreqID = Convert.ToInt32(rowData[FLOW_FREQ_COL]);
            //int infOutflowID = Convert.ToInt32(rowData[INFLOW_OUTFLOW_COL]);
            //int ratingID = Convert.ToInt32(rowData[RATING_COL]);
            //int extIntID = Convert.ToInt32(rowData[EXT_INT_COL]);
            //int leveeFailureID = Convert.ToInt32(rowData[LEVEE_FAILURE_COL]);
            //int stageDamageID = Convert.ToInt32(rowData[STAGE_DAMAGE_COL]);
            //string thresholdsString = (string)rowData[THRESHOLDS_COL];
            ////double thresholdValue = Convert.ToInt32(rowData[THRESHOLD_VALUE_COL]);

            //List<AdditionalThresholdRowItem> thresholdRowItems = ReadThresholdsXML(thresholdsString);

            //IASElement elem = new IASElement(name, description, year, impAreaID, flowFreqID, infOutflowID,
            //    ratingID, extIntID, leveeFailureID, stageDamageID, thresholdRowItems);
            //return elem;
        }

        
        //private void ReadIASSetFromXML(string xml)
        //{
        //    XDocument doc = XDocument.Parse(xml);
        //    XElement setElem = doc.Element(IAS_SET);
        //    string setName = setElem.Attribute(NAME).Value;
        //    string description = setElem.Attribute(DESCRIPTION).Value;
        //    int year = Int32.Parse( setElem.Attribute(YEAR).Value);

        //}

        

        

        #endregion

        /// <summary>
        /// Saves a new conditions element to the database
        /// </summary>
        /// <param name="element"></param>
        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(IASElementSet))
            {
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;
                SaveNewElementToParentTable(GetRowDataFromElement((IASElementSet)element), TableName, TableColumnNames, TableColumnTypes);
                StudyCacheForSaving.AddElement((IASElementSet)element);
            }
        }

        


        /// <summary>
        /// Deletes an element from the parent table.
        /// </summary>
        /// <param name="element"></param>
        public void Remove(ChildElement element)
        {
            int id = element.GetElementID();
            RemoveFromParentTable(element, TableName);
            StudyCacheForSaving.RemoveElement((IASElementSet)element, id);
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
        public void Load()
        {
            List<ChildElement> iasElems = CreateElementsFromRows(TableName, (rowData) => CreateElementFromRowData(rowData));
            foreach (IASElementSet elem in iasElems)
            {
                //IReadOnlyDictionary<IMetric, IHistogram> metricsDictionary = ReadMetricsHistogramTable(elem);
                //if(metricsDictionary.Count == 0)
                //{
                //    //then there is no compute data
                //}
                //else
                //{
                //    ////note that these elements are not in the study cache yet. 
                //    //int elementID = elem.GetElementID();
                //    //int seed = GetSeed(elementID);
                //    //IReadOnlyList<IConditionLocationYearRealizationSummary> realizationSummaries = CreateRealizations(elementID);

                //    //IConditionLocationYearSummary conditionLocationYearSummary = CreateIConditionLocationYearSummary(elem);
                //    //IReadOnlyDictionary<IMetric, IConvergenceCriteria> convergenceCriteria = CreateConvergenceCriteria(elementID);
                //    //IConditionLocationYearResult result = new ConditionLocationYearResult(conditionLocationYearSummary, convergenceCriteria, seed, metricsDictionary, realizationSummaries);
                //    //elem.ComputeResults = result;
                //}
                //IASElementSet set = new IASElementSet(elem.Name, elem.Description, elem.AnalysisYear, new List<AdditionalThresholdRowItem>(), new List<IASElement>() { elem });
                StudyCacheForSaving.AddElement(elem);
            }
        }


        //private IReadOnlyDictionary<IMetric, IConvergenceCriteria> CreateConvergenceCriteria(int conditionID)
        //{
        //    //todo: i think eventually i need to be writing something out for the criteria but for now it is just an empty ctor so
        //    //i don't need to write anything out

        //    //get the metrics
        //    List<IMetric> metrics = GetMetricsFromMetricsTable(conditionID);

        //    IConvergenceCriteria convergenceCriteria = IConvergenceCriteriaFactory.Factory();
        //    Dictionary<IMetric, IConvergenceCriteria> metricsDictionary = new Dictionary<IMetric, IConvergenceCriteria>();
        //    foreach (IMetric metric in metrics)
        //    {
        //        metricsDictionary.Add(metric, IConvergenceCriteriaFactory.Factory());
        //    }

        //    IReadOnlyDictionary<IMetric, IConvergenceCriteria> retval = new ReadOnlyDictionary<IMetric, IConvergenceCriteria>(metricsDictionary);
        //    return retval;
        //}

        //private int GetSeed(int conditionsID)
        //{
        //    DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(COMPUTE_SEEDS_TABLENAME);
        //    foreach(object[] row in tbl.GetRows(0, tbl.NumberOfRows-1))
        //    {
        //        if(Convert.ToInt32( row[COMPUTE_SEEDS_CONDITION_ID_COL]) == conditionsID)
        //        {
        //            return Convert.ToInt32(row[COMPUTE_SEEDS_SEED_COL]);
        //        }
        //    }
        //    throw new Exception("The condition with ID " + conditionsID + " was not found in the table: " + COMPUTE_SEEDS_TABLENAME);
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


        //todo: I think this method is responsible for adding a * and message on the ias element. Is this still
        //what we want.

        /// <summary>
        /// This will update the condition element in the database. This will trigger
        /// an update to the study cache and the study tree as well.
        /// </summary>
        /// <param name="elem">The child element that has been removed</param>
        /// <param name="newID">The new ID that will replace the existing one in the condition database (-1)</param>
        public void UpdateConditionsChildElementRemoved(ChildElement elem, int originalID, int newID)
        {
            //List<IASElementSet> conditionsElements = StudyCache.GetChildElementsOfType<IASElementSet>();
            ////update the db and save existing. This should prompt the change event
            ////in the cache which tells the cond owner element to update its children.
            ////the owner could then check wich ones are open and call an update from there?
            //if (elem is ImpactAreaElement)
            //{
            //    foreach (IASElement condElem in conditionsElements)
            //    {
            //        if (condElem.ImpactAreaID == originalID)
            //        {
            //            IASElement newElement = (IASElement)condElem.CloneElement(condElem);
            //            newElement.ImpactAreaID = newID;
            //            SaveExisting(condElem, newElement);
            //        }
            //    }
            //}
            //else if (elem is AnalyticalFrequencyElement)
            //{
            //    foreach (IASElement condElem in conditionsElements)
            //    {
            //        if (condElem.FlowFreqID == originalID)
            //        {
            //            IASElement newElement = (IASElement)condElem.CloneElement(condElem);
            //            newElement.FlowFreqID = newID;
            //            SaveExisting(condElem, newElement);
            //        }
            //    }
            //}
            //else if (elem is InflowOutflowElement)
            //{
            //    foreach (IASElement condElem in conditionsElements)
            //    {
            //        if (condElem.InflowOutflowID == originalID)
            //        {
            //            IASElement newElement = (IASElement)condElem.CloneElement(condElem);
            //            newElement.InflowOutflowID = newID;
            //            SaveExisting(condElem, newElement);
            //        }
            //    }
            //}
            //else if (elem is RatingCurveElement)
            //{
            //    //only update the conditions that were actually using this element
            //    foreach (IASElement condElem in conditionsElements)
            //    {
            //        if (condElem.RatingID == originalID)
            //        {
            //            IASElement newElement = (IASElement)condElem.CloneElement(condElem);
            //            newElement.RatingID = newID;
            //            newElement.UpdateTreeViewHeader(newElement.Name + "*");
            //            newElement.ToolTip = "Rating curve " + elem.Name + " was deleted. Condition is out of sync.";
            //            SaveExisting(condElem, newElement);
            //        }
            //    }
            //}
            //else if (elem is LeveeFeatureElement)
            //{
            //    //only update the conditions that were actually using this element
            //    foreach (IASElement condElem in conditionsElements)
            //    {
            //        if (condElem.LeveeFailureID == originalID)
            //        {
            //            IASElement newElement = (IASElement)condElem.CloneElement(condElem);
            //            newElement.LeveeFailureID = newID;
            //            SaveExisting(condElem, newElement);
            //        }
            //    }
            //}
            //else if (elem is ExteriorInteriorElement)
            //{
            //    //only update the conditions that were actually using this element
            //    foreach (IASElement condElem in conditionsElements)
            //    {
            //        if (condElem.ExtIntStageID == originalID)
            //        {
            //            IASElement newElement = (IASElement)condElem.CloneElement(condElem);
            //            newElement.ExtIntStageID = newID;
            //            SaveExisting(condElem, newElement);
            //        }
            //    }
            //}
            //else if (elem is AggregatedStageDamageElement)
            //{
            //    //only update the conditions that were actually using this element
            //    foreach (IASElement condElem in conditionsElements)
            //    {
            //        if (condElem.StageDamageID == originalID)
            //        {
            //            IASElement newElement = (IASElement)condElem.CloneElement(condElem);
            //            newElement.StageDamageID = newID;
            //            SaveExisting(condElem, newElement);
            //        }
            //    }
            //}
        }

  

        /// <summary>
        /// This saves all the results of a compute.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="conditionsID"></param>
        /// <param name="frequencyFunction"></param>
        /// <param name="transformFunctions"></param>
        //public void SaveConditionResults(IConditionLocationYearResult result, int conditionsID, IFrequencyFunction frequencyFunction,
        //    List<ITransformFunction> transformFunctions)
        //{
        //    //i want to keep the metric ids in sync with each other so i get them here
        //    IEnumerable<IMetric> metrics = result.Metrics.Keys;
        //    SaveMetricTable(conditionsID, metrics);
        //    SaveMetricsHistogramTable(result.Metrics,metrics, conditionsID);
        //    SaveConditionResultFunctions(conditionsID, frequencyFunction, transformFunctions, result.ConditionLocationTime.Parameters);
        //    SaveRealizations(result.Realizations, conditionsID);
        //    //todo: at some point we might need a convergence criteria table
        //    SaveComputeSeed(conditionsID, result.Seed);
        //}

        private const string COMPUTE_SEEDS_TABLENAME = "ComputeSeeds";
        private const int COMPUTE_SEEDS_CONDITION_ID_COL = 0;
        private const int COMPUTE_SEEDS_SEED_COL = 1;

        //private void SaveComputeSeed(int conditionID, int seed)
        //{
        //    string[] columnNames = new string[]{ "ConditionID", "Seed" };

        //    DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(COMPUTE_SEEDS_TABLENAME);
        //    if (tbl == null)
        //    {
        //        Storage.Connection.Instance.CreateTable(COMPUTE_SEEDS_TABLENAME, columnNames, new Type[] { typeof(int), typeof(int) });
        //        tbl = Storage.Connection.Instance.GetTable(COMPUTE_SEEDS_TABLENAME);
        //    }

        //    //create the table row
        //    object[] newRow = new object[] { conditionID, seed };
        //    //if the conditionsID is already in the table then we want to update, not add a new row.
        //    bool rowUpdated = false;
        //    int i = 0;
        //    foreach(object[] row in tbl.GetRows(0, tbl.NumberOfRows-1))
        //    {
        //        if(Convert.ToInt32(row[COMPUTE_SEEDS_CONDITION_ID_COL]) == conditionID)
        //        {
        //            //replace the row
        //            tbl.EditRow(i, newRow);
        //            rowUpdated = true;
        //        }
        //        i++;
        //    }

        //    if(!rowUpdated)
        //    {
        //        tbl.AddRow(new object[] { conditionID, seed });
        //    }
        //    tbl.ApplyEdits();
        //}

        #region Save Metrics
        private const int METRIC_ID_COL = 0;
        private const int METRIC_THRESHOLD_TYPE_COL = 1;
        private const int METRIC_THRESHOLD_VALUE_COL = 2;

        private const string METRIC_ID_NAME = "MetricID";
        private const string METRIC_TYPE_NAME = "ThresholdType";
        private const string METRIC_THRESHOLD_VALUE_NAME = "ThresholdValue";

        private const string METRIC_TABLE_NAME_BASE = "ConditionMetrics_";
        //private void SaveMetricTable(int conditionElementID, IEnumerable<IMetric> metrics)
        //{
        //    //metric id, thresholdType, ThresholdValue
        //    string[] columnNames = new string[] { METRIC_ID_NAME, METRIC_TYPE_NAME, METRIC_THRESHOLD_VALUE_NAME };
        //    Type[] columnTypes = new Type[] { typeof(int), typeof(string), typeof(double) };
        //    string tableName = METRIC_TABLE_NAME_BASE + conditionElementID;

        //    //clobber the table everytime
        //    DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(tableName);
        //    if (tbl != null)
        //    {
        //        Storage.Connection.Instance.DeleteTable(tableName);
        //    }

        //    Storage.Connection.Instance.CreateTable(tableName, columnNames, columnTypes);
        //    tbl = Storage.Connection.Instance.GetTable(tableName);

        //    //create the table rows
        //    List<object[]> rows = new List<object[]>();
        //    int i = 1;
        //    foreach(IMetric metric in metrics)
        //    {
        //        int id = i;
        //        string thresholdType = metric.ParameterType.ToString();
        //        double thresholdValue = metric.Ordinate.Value();
        //        rows.Add(new object[] { id, thresholdType, thresholdValue });
        //        i++;
        //    }


        //    tbl.AddRows(rows);
        //    tbl.ApplyEdits();
        //}


        private string _MetricHistogramTableNameBase = "ConditionMetricHistograms_";
        private int _MetricFunctionCol = 2;

        private Type _MetricIDType = typeof(int);
        private Type _MetricMeanValueType = typeof(double);
        private Type _MetricFunctionType = typeof(string);

        private string _MetricIDName = "ID";
        private string _MetricMeanValueName = "Mean";
        private string _MetricFunctionName = "Function";

        /// <summary>
        /// The metrics list is passed in here so that we can keep the metrics syncronized. The list is in the order
        /// of their id's.
        /// </summary>
        /// <param name="metricDictionary"></param>
        /// <param name="metrics"></param>
        /// <param name="conditionElementID"></param>
        //private void SaveMetricsHistogramTable(IReadOnlyDictionary<IMetric, IHistogram> metricDictionary, IEnumerable<IMetric> metrics, int conditionElementID)
        //{
        //    string[] columnNames = new string[] { _MetricIDName, _MetricMeanValueName, _MetricFunctionName };
        //    Type[] columnTypes = new Type[] { _MetricIDType, _MetricMeanValueType, _MetricFunctionType };
        //    string tableName = _MetricHistogramTableNameBase + conditionElementID;

        //    //clobber the table everytime
        //    DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(tableName);
        //    if (tbl != null)
        //    {
        //        Storage.Connection.Instance.DeleteTable(tableName);
        //    }
            
        //        Storage.Connection.Instance.CreateTable(tableName, columnNames, columnTypes);
        //        tbl = Storage.Connection.Instance.GetTable(tableName);
            

        //    //get the rows for the table -> all the bins
        //    List<object[]> rows = new List<object[]>();
        //    int i = 1;
        //    foreach (IMetric metric in metrics)
        //    {
        //        IHistogram histo = metricDictionary[metric];
        //        double meanValue = histo.Mean;
        //        string curve = histo.WriteToXML().ToString();
        //        rows.Add(new object[] { i , meanValue, curve });
        //        i++;
        //    }

        //    tbl.AddRows(rows);
        //    tbl.ApplyEdits();
        //}

        //private IReadOnlyDictionary<IMetric, IHistogram> ReadMetricsHistogramTable(IASElement conditionsElement)
        //{
        //    int condElemID = conditionsElement.GetElementID();
        //    Dictionary<IMetric, IHistogram> metricsDictionary = new Dictionary<IMetric, IHistogram>();
        //    string metricHistogramTable = _MetricHistogramTableNameBase + condElemID;
        //    DatabaseManager.DataTableView metricsHistogramTable = Storage.Connection.Instance.GetTable(metricHistogramTable);

        //    if (metricsHistogramTable != null)
        //    {
        //        List<object[]> rows = metricsHistogramTable.GetRows(0, metricsHistogramTable.NumberOfRows - 1);
        //        foreach (object[] row in rows)
        //        {
        //            //i need to get the metric from the metric table
        //            int metricId = Convert.ToInt32( row[METRIC_ID_COL]);
        //            IMetric metric = GetMetricFromMetricTable(metricId, condElemID);
        //            IHistogram histogram = GetHistogramFromRowData(row);
        //            metricsDictionary.Add(metric, histogram);
        //        }
        //    }
        //    return metricsDictionary;
        //}

        //private IMetric GetMetricFromMetricTable(int id, int conditionElementID)
        //{
        //    string metricTableName = METRIC_TABLE_NAME_BASE + conditionElementID;
        //    DatabaseManager.DataTableView metricsTable = Storage.Connection.Instance.GetTable(metricTableName);
        //    List<object[]> rows = metricsTable.GetRows(0, metricsTable.NumberOfRows - 1);
        //    foreach(object[] row in rows)
        //    {
        //        int rowID = Convert.ToInt32(row[METRIC_ID_COL]);
        //        if(rowID == id)
        //        {
        //            //we found the metric
        //            string thresholdType = Convert.ToString(row[METRIC_THRESHOLD_TYPE_COL]);
        //            IMetricEnum metricEnum = ConvertStringToMetricEnum(thresholdType);
        //            double thresholdValue = Convert.ToDouble(row[METRIC_THRESHOLD_VALUE_COL]);
        //            IMetric metric = IMetricFactory.Factory(metricEnum, thresholdValue);
        //            return metric;
        //        }
        //    }
        //    //we never found it
        //    throw new Exception("Could not locat a metric in the " + metricTableName + " table with an id of " + id);
        //}

        //private List<IMetric> GetMetricsFromMetricsTable(int conditionElementID)
        //{
        //    List<IMetric> metrics = new List<IMetric>();
        //    string metricTableName = METRIC_TABLE_NAME_BASE + conditionElementID;
        //    DatabaseManager.DataTableView metricsTable = Storage.Connection.Instance.GetTable(metricTableName);
        //    List<object[]> rows = metricsTable.GetRows(0, metricsTable.NumberOfRows - 1);
        //    foreach (object[] row in rows)
        //    {
                
        //            //we found the metric
        //            string thresholdType = Convert.ToString(row[METRIC_THRESHOLD_TYPE_COL]);
        //            IMetricEnum metricEnum = ConvertStringToMetricEnum(thresholdType);
        //            double thresholdValue = Convert.ToDouble(row[METRIC_THRESHOLD_VALUE_COL]);
        //            IMetric metric = IMetricFactory.Factory(metricEnum, thresholdValue);
        //            metrics.Add( metric);
        //    }
        //    return metrics;
        //}

        //private IHistogram GetHistogramFromRowData(object[] row)
        //{
        //    string histogramXMLString = Convert.ToString( row[_MetricFunctionCol]);
        //    return IHistogramFactory.Factory(histogramXMLString);
        //}

        #endregion

        private const int REALIZATION_ID_COL = 0;
        private const int REALIZATION_RESULTS_COL = 1;

        private const string REALIZATION_ID_NAME = "ID";
        private const string REALIZATION_RESULTS_NAME = "Results";

        private const string REALIZATION_TABLE_NAME_BASE = "ConditionRealizations_";

        //private void SaveRealizations(IReadOnlyList<IConditionLocationYearRealizationSummary> realizations, int conditionsID)
        //{
        //    string[] columnNames = new string[] { REALIZATION_ID_NAME, REALIZATION_RESULTS_NAME };
        //    Type[] columnTypes = new Type[] { typeof(int), typeof(string) };
        //    string tableName = REALIZATION_TABLE_NAME_BASE + conditionsID;

        //    //clobber the table everytime
        //    DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(tableName);
        //    if (tbl != null)
        //    {
        //        Storage.Connection.Instance.DeleteTable(tableName);
        //    }

        //    Storage.Connection.Instance.CreateTable(tableName, columnNames, columnTypes);
        //    tbl = Storage.Connection.Instance.GetTable(tableName);

        //    //create the rows
        //    List<object[]> rows = new List<object[]>();
        //    //i need to get the metrics from the metrics table and then store these
        //    //results in the order of the metric id's.

        //    //read the metrics table
        //    List<IMetricEnum> orderedMetrics = GetMetricTypesInMetricIDOrder(conditionsID);

        //    foreach (IConditionLocationYearRealizationSummary realization in realizations)
        //    {
        //        int id = realization.ID;
        //        //get all the results
        //        string resultsListAsCSV = GetRealizationResultsAsCSV(realization, orderedMetrics);
        //        object[] row = new object[] { id, resultsListAsCSV };
        //        rows.Add(row);
        //    }

        //    tbl.AddRows(rows);
        //    tbl.ApplyEdits();

        //}

        //private string GetRealizationResultsAsCSV(IConditionLocationYearRealizationSummary realization, List<IMetricEnum> metrics)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    for(int i = 0;i<metrics.Count;i++)
        //    {
        //        double result = GetResultFromRealization(realization, metrics[i]);
        //        if(i == metrics.Count -1)
        //        {
        //            sb.Append(result);
        //        }
        //        else
        //        {
        //            sb.Append(result).Append(",");
        //        }
        //    }
        //    return sb.ToString();
        //}

        //private double GetResultFromRealization(IConditionLocationYearRealizationSummary realization, IMetricEnum metricEnum)
        //{
        //    //the IMetric actually holds a parameterType not a metricEnum so i have to convert it
        //    IParameterEnum targetParameterEnum = GetCorrespondingParameterType(metricEnum);
        //    IReadOnlyDictionary<IMetric, double> realizationMetrics = realization.Metrics;
        //    double result = -1;
           
        //    //find the right entry in the dictionay that matches this metric enum
        //    foreach (KeyValuePair<IMetric, double> entry in realizationMetrics)
        //    {

        //        if(entry.Key.ParameterType == targetParameterEnum)
        //        {
        //            result = entry.Value;
        //        }
        //    }

        //    return result;
        //}

        //private IParameterEnum GetCorrespondingParameterType(IMetricEnum type)
        //{
        //    switch (type)
        //    {
        //        case IMetricEnum.ExteriorStage:
        //            return IParameterEnum.ExteriorStageAEP;
        //        case IMetricEnum.InteriorStage:
        //            return IParameterEnum.InteriorStageAEP;
        //        case IMetricEnum.Damages:
        //            return IParameterEnum.DamageAEP;
        //        case IMetricEnum.ExpectedAnnualDamage:
        //            return IParameterEnum.EAD;
        //        default:
        //            throw new InvalidOperationException($"The specified metric type: {type.ToString()} was not successfully matched with a valid target function.");
        //    }
        //}

        //private List<IMetricEnum> GetMetricTypesInMetricIDOrder(int conditionsID)
        //{
        //    string metricTableName = METRIC_TABLE_NAME_BASE + conditionsID;

        //    DatabaseManager.DataTableView metricTable = Storage.Connection.Instance.GetTable(metricTableName);
        //    Dictionary<int, IMetricEnum> metricsFromMetricsTable = new Dictionary<int, IMetricEnum>();
        //    List<object[]> metricRows = metricTable.GetRows(0, metricTable.NumberOfRows - 1);
        //    foreach (object[] row in metricRows)
        //    {
        //        string metricEnumString = Convert.ToString(row[METRIC_THRESHOLD_TYPE_COL]);
        //        IMetricEnum metricEnum = ConvertStringToMetricEnum(metricEnumString);
        //        int metricID = Convert.ToInt32(row[METRIC_ID_COL]);
        //        metricsFromMetricsTable.Add(metricID, metricEnum);
        //    }
        //    //order the metricEnums by value
        //    IOrderedEnumerable<KeyValuePair<int, IMetricEnum>> orderedMetricRows = metricsFromMetricsTable.OrderBy(i => i.Key);
        //    List<IMetricEnum> orderedMetricEnums = new List<IMetricEnum>();
        //    foreach (KeyValuePair<int, IMetricEnum> pair in orderedMetricRows)
        //    {
        //        orderedMetricEnums.Add(pair.Value);
        //    }
        //    return orderedMetricEnums;
        //}

        //private IReadOnlyList<IConditionLocationYearRealizationSummary> CreateRealizations( int conditionID)
        //{
        //    string tableName = REALIZATION_TABLE_NAME_BASE + conditionID;
        //    List<IConditionLocationYearRealizationSummary> realizationSummaries = new List<IConditionLocationYearRealizationSummary>();
        //    List<IMetric> metrics = GetMetricsFromMetricsTable(conditionID);

        //    DataTable table = Storage.Connection.Instance.GetDataTable(tableName);
        //    foreach (DataRow row in table.Rows)
        //    {
        //        object[] rowVals = row.ItemArray;
        //        int realizationID = Convert.ToInt32(rowVals[REALIZATION_ID_COL]);
        //        string thresholdResultsString = Convert.ToString(rowVals[REALIZATION_RESULTS_COL]);
              
        //        //split the thresholdResults
        //        List<double> thresholdResultsList = new List<double>();
        //        string[] thresholdResults = thresholdResultsString.Split(',');
        //        foreach (string result in thresholdResults)
        //        {
        //            thresholdResultsList.Add(Convert.ToDouble(result));
        //        }

        //        //read the metrics from the metrics table 
        //        IReadOnlyDictionary<IMetric, double> results = CreateMetricsResultDictionary(metrics, thresholdResultsList);

        //        IConditionLocationYearRealizationSummary realization = CreateRealizationSummary( results, realizationID, conditionID);
        //        realizationSummaries.Add(realization);

        //        thresholdResultsList.Clear();
        //    }
        //    return realizationSummaries;
        //}

        //private IConditionLocationYearRealizationSummary CreateRealizationSummary(IReadOnlyDictionary<IMetric, double> metrics, int id, int conditionElemID)
        //{
        //    //read the functions from the functions table
        //    string functionsTableName = FUNCTION_TABLE_BASE + conditionElemID;
        //    DatabaseManager.DataTableView functionsTable = Storage.Connection.Instance.GetTable(functionsTableName);
        //    List<object[]> rows = functionsTable.GetRows(0, functionsTable.NumberOfRows - 1);
            
        //    List<IFdaFunction> functions = new List<IFdaFunction>();
        //    List<bool> isSampledList = new List<bool>();

        //    List<double> probabilities = new List<double>();
        //    foreach(object[] row in rows)
        //    {
        //        ICoordinatesFunction coordinatesFunction = ICoordinatesFunctionsFactory.Factory((String)row[FUNCTIONS_FUNCTION_COL]);
        //        IFunction func = IFunctionFactory.Factory(coordinatesFunction.Coordinates, coordinatesFunction.Interpolator);

        //        string paramString = Convert.ToString(row[FUNCTIONS_PARAMETER_COL]);
        //        IParameterEnum paramEnum = ConvertStringToParameterType(paramString);
        //        functions.Add( IFdaFunctionFactory.Factory(paramEnum, func));
        //        isSampledList.Add( Convert.ToBoolean(row[FUNCTIONS_IS_SAMPLED_COL]));
        //        probabilities.Add(-1);
        //    }

        //    IReadOnlyDictionary<IParameterEnum, ISampledParameter<IFdaFunction>> fxs = CreateSampledFunctions(functions, isSampledList, probabilities);
        //    return IConditionLocationYearRealizationSummaryFactory.Factory(fxs, metrics, id);
        //}

        //private IParameterEnum ConvertStringToParameterType(string param)
        //{
        //    if(param.Equals(IParameterEnum.NotSet.ToString()))
        //    {
        //        return IParameterEnum.NotSet;
        //    }
        //    if (param.Equals(IParameterEnum.InflowFrequency.ToString()))
        //    {
        //        return IParameterEnum.InflowFrequency;
        //    }

        //    if (param.Equals(IParameterEnum.InflowOutflow.ToString()))
        //    {
        //        return IParameterEnum.InflowOutflow;
        //    }

        //    if (param.Equals(IParameterEnum.OutflowFrequency.ToString()))
        //    {
        //        return IParameterEnum.OutflowFrequency;
        //    }

        //    if (param.Equals(IParameterEnum.Rating.ToString()))
        //    {
        //        return IParameterEnum.Rating;
        //    }

        //    if (param.Equals(IParameterEnum.ExteriorStageFrequency.ToString()))
        //    {
        //        return IParameterEnum.ExteriorStageFrequency;
        //    }

        //    if (param.Equals(IParameterEnum.InteriorStageFrequency.ToString()))
        //    {
        //        return IParameterEnum.InteriorStageFrequency;
        //    }

        //    if (param.Equals(IParameterEnum.InteriorStageDamage.ToString()))
        //    {
        //        return IParameterEnum.InteriorStageDamage;
        //    }

        //    if (param.Equals(IParameterEnum.DamageFrequency.ToString()))
        //    {
        //        return IParameterEnum.DamageFrequency;
        //    }

        //    if (param.Equals(IParameterEnum.LateralStructureFailure.ToString()))
        //    {
        //        return IParameterEnum.LateralStructureFailure;
        //    }

        //    throw new Exception("Could not translate " + param + " to an IParameterEnum.");

        //}

        //private IReadOnlyDictionary<IParameterEnum, ISampledParameter<IFdaFunction>> CreateSampledFunctions(List<IFdaFunction> functions, List<bool> isSampledList, List<double> probabilities)
        //{
        //    Dictionary<IParameterEnum, ISampledParameter<IFdaFunction>> sampledFunctions = new Dictionary<IParameterEnum, ISampledParameter<IFdaFunction>>();
        //    for(int i= 0;i<functions.Count;i++)
        //    {
        //        IFdaFunction func = functions[i];
        //        bool isSampled = isSampledList[i];
        //        double prob = probabilities[i];
        //        sampledFunctions.Add(func.ParameterType, ISampledFunctionFactory.Factory(func, isSampled, prob));
        //    }
        //    return sampledFunctions;
        //}     

        //private IReadOnlyDictionary<IMetric, double> CreateMetricsResultDictionary(List<IMetric> metrics, List<double> results)
        //{
        //    Dictionary<IMetric, double> retval = new Dictionary<IMetric, double>();
        //    for (int i = 0;i<metrics.Count;i++)
        //    {
        //        retval.Add(metrics[i], results[i]);
        //    }
        //    return retval;
        //}  

        /// <summary>
        /// This loads the IConditionLocationYearSummary from the db
        /// </summary>
        /// <param name="conditionsElementID"></param>
        /// <returns></returns>
        //public IConditionLocationYearSummary CreateIConditionLocationYearSummary(IASElement condElem)
        //{
        //    return CreateIConditionLocationYearSummary(condElem.ImpactAreaID, condElem.AnalysisYear, condElem.FlowFreqID, condElem.InflowOutflowID,
        //        condElem.RatingID, condElem.LeveeFailureID, condElem.ExtIntStageID, condElem.StageDamageID, condElem.Thresholds);
        //}

        //public IConditionLocationYearSummary CreateIConditionLocationYearSummary(int impactAreaID, int year, IFrequencyFunction freqFunction, List<ITransformFunction> transformFunctions,
        //      List<AdditionalThresholdRowItem> thresholds, string label = "")
        //{
        //    ImpactAreaElement impArea = (ImpactAreaElement)StudyCache.GetChildElementOfType(typeof(ImpactAreaElement), impactAreaID);
        //    ILocation location = new Location(impArea.Name, impArea.Description);
        //    List<IMetric> metrics = new List<IMetric>();
        //    metrics.Add(IMetricFactory.Factory()); //this is the ead metric
        //    foreach(AdditionalThresholdRowItem row in thresholds)
        //    {
        //        metrics.Add(IMetricFactory.Factory(row.ThresholdType, row.ThresholdValue));
        //    }
            
        //    return new ConditionLocationYearNoLateralStructure(location, year, freqFunction, transformFunctions, metrics);

        //}

        //public IConditionLocationYearSummary CreateIConditionLocationYearSummary(int impactAreaID, int year, IFrequencyFunction freqFunction, List<ITransformFunction> transformFunctions,
        //      LeveeFeatureElement leveeFailureElement, IMetricEnum thresholdType, double thresholdValue, string label = "")
        //{
        //    ImpactAreaElement impArea = (ImpactAreaElement)StudyCache.GetChildElementOfType(typeof(ImpactAreaElement), impactAreaID);
        //    ILocation location = new Location(impArea.Name, impArea.Description);
        //    List<IMetric> metrics = new List<IMetric>();
        //    metrics.Add(IMetricFactory.Factory()); //this is the ead metric
        //    if (thresholdType != IMetricEnum.NotSet)
        //    {
        //        metrics.Add(IMetricFactory.Factory(thresholdType, thresholdValue));
        //    }
        //    ILateralStructure latStruct = ILateralStructureFactory.Factory(leveeFailureElement.Elevation, (ITransformFunction)leveeFailureElement.Curve); ;
        //    return new ConditionLocationYearWithLateralStructure(location, year, freqFunction, transformFunctions, latStruct, metrics);

        //}

        //public IConditionLocationYearSummary CreateIConditionLocationYearSummary(int impactAreaID, int year, int frequencyFxID, int inflowOutflowID, 
        //    int ratingID, int lateralStructureID, int extIntID, int stageDamageID, IMetricEnum thresholdType, double thresholdValue, string label = "")
        //{
        //    //if (!Validate())
        //    {
        //        //todo: show errors in popup?
        //        //return null;
        //    }
        //    bool hasInflowOutflow = inflowOutflowID != -1;
        //    bool hasLeveeFailure = lateralStructureID != -1;
        //    bool hasExtInt = extIntID != -1;
        //    try
        //    {
        //        //required params
        //        ImpactAreaElement impArea = (ImpactAreaElement)StudyCache.GetChildElementOfType(typeof(ImpactAreaElement), impactAreaID);
        //        AnalyticalFrequencyElement flowFreqElement = (AnalyticalFrequencyElement)StudyCache.GetChildElementOfType(typeof(AnalyticalFrequencyElement), frequencyFxID);
        //        RatingCurveElement ratingElement = (RatingCurveElement)StudyCache.GetChildElementOfType(typeof(RatingCurveElement), ratingID);
        //        AggregatedStageDamageElement stageDamageElement = (AggregatedStageDamageElement)StudyCache.GetChildElementOfType(typeof(AggregatedStageDamageElement), stageDamageID);

        //        //possible other functions
        //        InflowOutflowElement inflowOutflowElement = null;
        //        LeveeFeatureElement leveeFailureElement = null;
        //        ExteriorInteriorElement extIntElement = null;
        //        if (hasInflowOutflow)
        //        {
        //            inflowOutflowElement = (InflowOutflowElement)StudyCache.GetChildElementOfType(typeof(InflowOutflowElement), inflowOutflowID);
        //        }
        //        if (hasLeveeFailure)
        //        {
        //            leveeFailureElement = (LeveeFeatureElement)StudyCache.GetChildElementOfType(typeof(LeveeFeatureElement), lateralStructureID);
        //        }
        //        if (hasExtInt)
        //        {
        //            extIntElement = (ExteriorInteriorElement)StudyCache.GetChildElementOfType(typeof(ExteriorInteriorElement), extIntID);
        //        }

        //        List<ITransformFunction> transforms = new List<ITransformFunction>();
        //        if (hasInflowOutflow)
        //        {
        //            transforms.Add((ITransformFunction)inflowOutflowElement.Curve);
        //        }
        //        transforms.Add((ITransformFunction)ratingElement.Curve);
           
        //        if (hasExtInt)
        //        {
        //            transforms.Add((ITransformFunction)extIntElement.Curve);
        //        }
        //        transforms.Add((ITransformFunction)stageDamageElement.Curve);

        //        ILocation location = new Location(impArea.Name, impArea.Description);
        //        List<IMetric> metrics = new List<IMetric>();
        //        metrics.Add(IMetricFactory.Factory()); //this is the ead metric
        //        if (thresholdType != IMetricEnum.NotSet)
        //        {
        //            metrics.Add(IMetricFactory.Factory(thresholdType, thresholdValue));
        //        }

        //        IFrequencyFunction inflowFreqFunc = (IFrequencyFunction)flowFreqElement.Curve;

        //        if (hasLeveeFailure)
        //        {
        //            double leveeHeight = leveeFailureElement.Elevation;
        //            ILateralStructure lateralStructure = ILateralStructureFactory.Factory(leveeHeight, (ITransformFunction)leveeFailureElement.Curve);
        //            return new ConditionLocationYearWithLateralStructure(location, year, inflowFreqFunc, transforms, lateralStructure, metrics);
        //        }
        //        else
        //        {
        //            return new ConditionLocationYearNoLateralStructure(location, year, inflowFreqFunc, transforms, metrics);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show("Error in trying to retrieve one of the sub elements to create the condition.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        return null;
        //    }
        //}

        //public void SaveConditionResultFunctions(int conditionID, IFrequencyFunction freqFunction, List<ITransformFunction> transformFunctions,
        //    IReadOnlyDictionary<IParameterEnum, bool> isSampledDictionary)
        //{
        //    string tableName = "ConditionsResults_" + conditionID;
        //    List<IFdaFunction> functions = new List<IFdaFunction>();
        //    functions.Add(freqFunction);
        //    functions.AddRange(transformFunctions);
        //    SaveFunctionsTable(conditionID, functions, isSampledDictionary);

        //}



        private const int FUNCTIONS_PARAMETER_COL = 0;
        private const int FUNCTIONS_FUNCTION_COL = 1;
        private const int FUNCTIONS_IS_SAMPLED_COL = 2;

        private const string FUNCTIONS_PARAMETER_NAME = "ParameterType";
        private const string FUNCTIONS_FUNCTION_NAME = "Function";
        private const string FUNCTIONS_IS_SAMPLED_NAME = "IsSampled";

        private Type FUNCTIONS_PARAMETER_TYPE = typeof(string);
        private  Type FUNCTIONS_FUNCTION_TYPE = typeof(string);
        private  Type FUNCTIONS_IS_SAMPLED_TYPE = typeof(string);

        private const string FUNCTION_TABLE_BASE = "ConditionResultFunctions_";
        private void SaveFunctionsTable(int conditionID, List<IFdaFunction> functions, IReadOnlyDictionary<IParameterEnum, bool> isSampledDictionary)
        {
            string[] columnNames = new string[3];
            columnNames[FUNCTIONS_PARAMETER_COL] = FUNCTIONS_PARAMETER_NAME;
            columnNames[FUNCTIONS_FUNCTION_COL] = FUNCTIONS_FUNCTION_NAME;
            columnNames[FUNCTIONS_IS_SAMPLED_COL] = FUNCTIONS_IS_SAMPLED_NAME;

            Type[] columnTypes = new Type[3];
            columnTypes[FUNCTIONS_PARAMETER_COL] = FUNCTIONS_PARAMETER_TYPE;
            columnTypes[FUNCTIONS_FUNCTION_COL] = FUNCTIONS_FUNCTION_TYPE;
            columnTypes[FUNCTIONS_IS_SAMPLED_COL] = FUNCTIONS_IS_SAMPLED_TYPE;

            //create the rows to save
            List<object[]> rows = new List<object[]>();
            for(int i = 0;i<functions.Count;i++)
            {
                object[] row = new object[3];
                IFdaFunction func = functions[i];

                if(func.ParameterType == IParameterEnum.InflowFrequency)
                {
                    int asdf = 0;
                    //IDistribution dist = (IDistribution)func.Function.Distribution;
                    //i think i can get this done if i make the "DistributionFunction" public and add a prop for the distributionOrdinate.
                }

                string funcParameterEnum = func.ParameterType.ToString();
                string funcXML = func.WriteToXML().ToString();
                bool isSampled = isSampledDictionary[func.ParameterType];

                row[FUNCTIONS_PARAMETER_COL] = funcParameterEnum;
                row[FUNCTIONS_FUNCTION_COL] = funcXML;
                row[FUNCTIONS_IS_SAMPLED_COL] = isSampled;

                rows.Add(row);
            }

            //save out the rows
            string tableName = FUNCTION_TABLE_BASE + conditionID;
            //if the table already exists then delete it
            DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(tableName);
            if (tbl != null)
            {
                Storage.Connection.Instance.DeleteTable(tableName);
            }
            
            Storage.Connection.Instance.CreateTable(tableName, columnNames, columnTypes);
            tbl = Storage.Connection.Instance.GetTable(tableName);
            tbl.AddRows(rows);
            tbl.ApplyEdits();
        }

    }
}
