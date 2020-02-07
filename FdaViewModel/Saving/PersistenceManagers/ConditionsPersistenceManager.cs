
using FdaViewModel.AggregatedStageDamage;
using FdaViewModel.Conditions;
using FdaViewModel.FlowTransforms;
using FdaViewModel.FrequencyRelationships;
using FdaViewModel.GeoTech;
using FdaViewModel.StageTransforms;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class ConditionsPersistenceManager : SavingBase, IElementManager
    {
        private const int ID_COL = 0;
        private const int NAME_COL = 1;
        private const int DESC_COL = 2;
        private const int ANALYSIS_YEAR_COL = 3;
        private const int IMPACT_AREA_COL = 4;
        private const int USE_FLOW_FREQ_COL = 5;
        private const int FLOW_FREQ_COL = 6;
        private const int USE_INFLOW_OUTFLOW_COL = 7;
        private const int INFLOW_OUTFLOW_COL = 8;
        private const int USE_RATING_COL = 9;
        private const int RATING_COL = 10;
        private const int USE_EXT_INT_COL = 11;
        private const int EXT_INT_COL = 12;
        private const int USE_LEVEE_COL = 13;
        private const int LEVEE_COL = 14;
        private const int USE_FAILURE_FUNC_COL = 15;
        private const int FAILURE_FUNC_COL = 16;
        private const int USE_STAGE_DAMAGE_COL = 17;
        private const int STAGE_DAMAGE_COL = 18;
        private const int USE_THRESHOLD_COL = 19;
        private const int THRESHOLD_TYPE_COL = 20;
        private const int THRESHOLD_VALUE_COL = 21;

        //ELEMENT_TYPE is used to store the type in the log tables. Initially i was actually storing the type
        //of the element. But since they get stored as strings if a developer changes the name of the class
        //you would no longer get any of the old logs. So i use this constant.
        private const string ELEMENT_TYPE = "Condition";
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("ConditionsPersistenceManager");

        private const string TABLE_NAME = "Conditions";
        internal override string ChangeTableConstant { get { return "????"; } }
        private static readonly string[] ColumnNames = { "Name", "Description", "AnalysisYear", "ImpactArea",
                "UseFlowFreq","FlowFreq",
                "UseInOutFlow","InOutFlow",
                "UseRating","Rating",
                "UseExtIntStage","ExtIntStage",
                "UseLevee","Levee",
                "UseFailureFunc","FailureFunc",
                "UseStageDamage","StageDamage",
                "UseThreshold","ThresholdType","ThresholdValue" };
        private static readonly Type[] TableColTypes = { typeof(string), typeof(string), typeof(int), typeof(string),

                typeof(bool), typeof(string),
                typeof(bool), typeof(string),
                typeof(bool), typeof(string),
                typeof(bool), typeof(string),
                typeof(bool), typeof(string),
                typeof(bool), typeof(string),
                typeof(bool), typeof(string),
                typeof(bool), typeof(string), typeof(double) };



        public override string[] TableColumnNames { get { return ColumnNames; } }
        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return TableColTypes; }
        }
        public override string TableName { get { return TABLE_NAME; } }
        #region constructor
        public ConditionsPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #endregion




        #region utilities
        private object[] GetRowDataFromElement(Conditions.ConditionsElement element)
        {
            string FlowFreqName = (element.AnalyticalFlowFrequency == null) ? "" : element.AnalyticalFlowFrequency.Name;
            string InfOutName = (element.InflowOutflowElement == null) ? "" : element.InflowOutflowElement.Name;
            string  RatingName = (element.RatingCurveElement == null) ? "" : element.RatingCurveElement.Name;
            string ExtIntName = (element.ExteriorInteriorElement == null) ? "" : element.ExteriorInteriorElement.Name;
            string LeveeName = (element.LeveeElement == null) ? "" : element.LeveeElement.Name;
            string FailureFuncName = (element.FailureFunctionElement == null) ? "" : element.FailureFunctionElement.Name;
            string StageDamageName = (element.StageDamageElement == null) ? "" : element.StageDamageElement.Name;

            object[] retval = new object[] { element.Name, element.Description, element.AnalysisYear, element.ImpactAreaElement.Name,
                element.UseAnalyiticalFlowFrequency, FlowFreqName,
                element.UseInflowOutflow, InfOutName,
                element.UseRatingCurve,RatingName,
                element.UseExteriorInteriorStage,ExtIntName,
                element.UseLevee,LeveeName,
                element.UseFailureFunction,FailureFuncName,
                element.UseAggregatedStageDamage, StageDamageName,
                element.UseThreshold, element.MetricType,element.ThresholdValue};
            //db won't allow anything to be null, so if it is I make it an empty string
            for(int i = 0;i<retval.Length;i++)
            {
                if(retval[i] == null)
                {
                    retval[i] = "";
                }
            }
            return retval;
        }


        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
                //get the impact area
                string selectedImpAreaName = (string)rowData[IMPACT_AREA_COL];
                ImpactArea.ImpactAreaElement selectedImpArea = null;
                List<ImpactArea.ImpactAreaElement> impactAreas = StudyCache.GetChildElementsOfType<ImpactArea.ImpactAreaElement>();
                foreach (ImpactArea.ImpactAreaElement impArea in impactAreas)
                {
                    if (impArea.Name.Equals(selectedImpAreaName))
                    {
                        selectedImpArea = impArea;
                    }
                }
                if (selectedImpArea == null)
                {
                    //what do we do?
                }

                //threshold stuff
                bool useThreshold = Convert.ToBoolean( rowData[USE_THRESHOLD_COL]);
                Model.MetricEnum thresholdType = Model.MetricEnum.InteriorStage;
                Enum.TryParse((string)rowData[THRESHOLD_TYPE_COL], out thresholdType);
                double thresholdValue = Convert.ToDouble( rowData[THRESHOLD_VALUE_COL]);

                //get the impAreaRowItem. What is this? do we need it?
                ImpactArea.ImpactAreaRowItem indexLocation = new ImpactArea.ImpactAreaRowItem();
                int analysisYear = Convert.ToInt32(rowData[ANALYSIS_YEAR_COL]);
                ConditionBuilder builder = new ConditionBuilder((string)rowData[NAME_COL], (string)rowData[DESC_COL], analysisYear, selectedImpArea, indexLocation,
                     thresholdType, thresholdValue);

                bool useFlowFreq = Convert.ToBoolean(rowData[USE_FLOW_FREQ_COL]);
                if (useFlowFreq)
                {
                    string flowFreqName = (string)rowData[FLOW_FREQ_COL];
                    AnalyticalFrequencyElement flowFreqElem = GetSelectedElementOfType<AnalyticalFrequencyElement>(StudyCache.GetChildElementsOfType<AnalyticalFrequencyElement>(), flowFreqName);
                builder.WithAnalyticalFreqElem(flowFreqElem);
                    
                }

                bool useInflowOutflow = Convert.ToBoolean(rowData[USE_INFLOW_OUTFLOW_COL]);
                if (useInflowOutflow)
                {
                    string infOutName = (string)rowData[INFLOW_OUTFLOW_COL];
                    InflowOutflowElement inOutElem = GetSelectedElementOfType<InflowOutflowElement>(StudyCache.GetChildElementsOfType<InflowOutflowElement>(), infOutName);
                    builder.WithInflowOutflowElem(inOutElem);
                }

                bool useRating = Convert.ToBoolean(rowData[USE_RATING_COL]);
                if (useRating)
                {
                    string ratingName = (string)rowData[RATING_COL];
                    RatingCurveElement ratingElem = GetSelectedElementOfType<RatingCurveElement>(StudyCache.GetChildElementsOfType<RatingCurveElement>(), ratingName);
                    builder.WithRatingCurveElem(ratingElem);
                }

                bool useIntExt = Convert.ToBoolean(rowData[USE_EXT_INT_COL]);
                if (useIntExt)
                {
                    string extIntName = (string)rowData[EXT_INT_COL];
                    ExteriorInteriorElement extIntElem = GetSelectedElementOfType<ExteriorInteriorElement>(StudyCache.GetChildElementsOfType<ExteriorInteriorElement>(), extIntName);
                    builder.WithExtIntStageElem(extIntElem);
                }

                bool useLevee = Convert.ToBoolean(rowData[USE_LEVEE_COL]);
                if (useLevee)
                {
                    string leveeName = (string)rowData[LEVEE_COL];
                    LeveeFeatureElement leveeElem = GetSelectedElementOfType<LeveeFeatureElement>(StudyCache.GetChildElementsOfType<LeveeFeatureElement>(), leveeName);
                    builder.WithLevee(leveeElem);
                }

                bool useFailure = Convert.ToBoolean(rowData[USE_FAILURE_FUNC_COL]);
                if (useFailure)
                {
                    string failureName = (string)rowData[FAILURE_FUNC_COL];
                    FailureFunctionElement failureElem = GetSelectedElementOfType<FailureFunctionElement>(StudyCache.GetChildElementsOfType<FailureFunctionElement>(), failureName);
                    builder.WithFailureFunctionElem(failureElem);
                }

                bool useStageDam = Convert.ToBoolean(rowData[USE_STAGE_DAMAGE_COL]);
                if (useStageDam)
                {
                    string stageDamName = (string)rowData[STAGE_DAMAGE_COL];
                    AggregatedStageDamage.AggregatedStageDamageElement stageDamElem = GetSelectedElementOfType<AggregatedStageDamageElement>(StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>(), stageDamName);
                    builder.WithAggStageDamageElem(stageDamElem);
                }

                return builder.build();
        }

        private T GetSelectedElementOfType<T>(List<T> elements, string name) where T : ChildElement
        {
            //List<T> elems = GetElementsOfType<T>();
            foreach (T elem in elements)
            {
                if (elem.Name.Equals(name))
                {
                    return elem;
                }
            }
            return null;
        }

        #endregion

        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(ConditionsElement))
            {
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;

                SaveNewElementToParentTable(GetRowDataFromElement((ConditionsElement)element), TableName, TableColumnNames, TableColumnTypes);
                //SaveElementToChangeTable(element.Name, GetRowDataFromElement((ConditionsElement)element), ChangeTableConstant, TableColumnNames, TableColumnTypes);
                //SaveCurveTable(element.Curve, ChangeTableConstant, editDate);
                //add the rating element to the cache which then raises event that adds it to the owner element
                StudyCacheForSaving.AddElement((ConditionsElement)element);
                
            }
        }

        public void Remove(ChildElement element)
        {
            RemoveFromParentTable(element, TableName);
            //DeleteChangeTableAndAssociatedTables(element, ChangeTableConstant);
            StudyCacheForSaving.RemoveElement((ConditionsElement)element);// RemoveRatingElement((RatingCurveElement)element);

        }

        public void SaveExisting(ChildElement oldElement, ChildElement element, int changeTableIndex  )
        {
            base.SaveExisting(oldElement, element);
            //if (DidParentTableRowValuesChange(element, GetRowDataFromElement((ConditionsElement)element), oldElement.Name, TableName))
            //{
            //    UpdateParentTableRow(element.Name, changeTableIndex, GetRowDataFromElement((ConditionsElement)element), oldElement.Name, TableName, false, ChangeTableConstant);
            //    StudyCacheForSaving.UpdateConditionsElement((ConditionsElement)oldElement, (ConditionsElement)element);
            //}
        }

        public void Load()
        {
            List<ChildElement> conditions = CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (ConditionsElement elem in conditions)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }


        public override void AddValidationRules()
        {
            // throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
