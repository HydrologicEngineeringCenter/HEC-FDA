using FdaModel.ComputationPoint;
using FdaViewModel.AggregatedStageDamage;
using FdaViewModel.Conditions;
using FdaViewModel.FlowTransforms;
using FdaViewModel.FrequencyRelationships;
using FdaViewModel.GeoTech;
using FdaViewModel.StageTransforms;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class ConditionsPersistenceManager : SavingBase, IPersistable
    {
        private const string TableName = "Conditions";
        internal override string ChangeTableConstant { get { return "????"; } }
        private static readonly string[] TableColumnNames = { "Name", "Description", "AnalysisYear", "ImpactArea",
                "UseFlowFreq","FlowFreq",
                "UseInOutFlow","InOutFlow",
                "UseRating","Rating",
                "UseExtIntStage","ExtIntStage",
                "UseLevee","Levee",
                "UseFailureFunc","FailureFunc",
                "UseStageDamage","StageDamage",
                "UseThreshold","ThresholdType","ThresholdValue" };
        private static readonly Type[] TableColumnTypes = { typeof(string), typeof(string), typeof(int), typeof(string),

                typeof(bool), typeof(string),
                typeof(bool), typeof(string),
                typeof(bool), typeof(string),
                typeof(bool), typeof(string),
                typeof(bool), typeof(string),
                typeof(bool), typeof(string),
                typeof(bool), typeof(string),
                typeof(bool), typeof(string), typeof(double) };





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
                element.UseThreshold, element.ThresholdType,element.ThresholdValue};
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
            if (rowData.Length >= 18)
            {
                //get the impact area
                string selectedImpAreaName = (string)rowData[3];
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
                bool useThreshold = (bool)rowData[18];
                PerformanceThresholdTypes thresholdType = PerformanceThresholdTypes.InteriorStage;
                Enum.TryParse((string)rowData[19], out thresholdType);
                double thresholdValue = (double)rowData[20];

                //get the impAreaRowItem. What is this? do we need it?
                ImpactArea.ImpactAreaRowItem indexLocation = new ImpactArea.ImpactAreaRowItem();
                int analysisYear = Convert.ToInt32(rowData[2]);
                ConditionBuilder builder = new ConditionBuilder((string)rowData[0], (string)rowData[1], analysisYear, selectedImpArea, indexLocation,
                     thresholdType, thresholdValue);

                bool useFlowFreq = (bool)rowData[4];
                if (useFlowFreq)
                {
                    string flowFreqName = (string)rowData[5];
                    AnalyticalFrequencyElement flowFreqElem = GetSelectedElementOfType<AnalyticalFrequencyElement>(StudyCache.GetChildElementsOfType<AnalyticalFrequencyElement>(), flowFreqName);
                    builder.WithAnalyticalFreqElem(flowFreqElem);
                }

                bool useInflowOutflow = (bool)rowData[6];
                if (useInflowOutflow)
                {
                    string infOutName = (string)rowData[7];
                    InflowOutflowElement inOutElem = GetSelectedElementOfType<InflowOutflowElement>(StudyCache.GetChildElementsOfType<InflowOutflowElement>(), infOutName);
                    builder.WithInflowOutflowElem(inOutElem);
                }

                bool useRating = (bool)rowData[8];
                if (useRating)
                {
                    string ratingName = (string)rowData[9];
                    RatingCurveElement ratingElem = GetSelectedElementOfType<RatingCurveElement>(StudyCache.GetChildElementsOfType<RatingCurveElement>(), ratingName);
                    builder.WithRatingCurveElem(ratingElem);
                }

                bool useIntExt = (bool)rowData[10];
                if (useIntExt)
                {
                    string extIntName = (string)rowData[11];
                    ExteriorInteriorElement extIntElem = GetSelectedElementOfType<ExteriorInteriorElement>(StudyCache.GetChildElementsOfType<ExteriorInteriorElement>(), extIntName);
                    builder.WithExtIntStageElem(extIntElem);
                }

                bool useLevee = (bool)rowData[12];
                if (useLevee)
                {
                    string leveeName = (string)rowData[13];
                    LeveeFeatureElement leveeElem = GetSelectedElementOfType<LeveeFeatureElement>(StudyCache.GetChildElementsOfType<LeveeFeatureElement>(), leveeName);
                    builder.WithLevee(leveeElem);
                }

                bool useFailure = (bool)rowData[14];
                if (useFailure)
                {
                    string failureName = (string)rowData[15];
                    FailureFunctionElement failureElem = GetSelectedElementOfType<FailureFunctionElement>(StudyCache.GetChildElementsOfType<FailureFunctionElement>(), failureName);
                    builder.WithFailureFunctionElem(failureElem);
                }

                bool useStageDam = (bool)rowData[16];
                if (useStageDam)
                {
                    string stageDamName = (string)rowData[17];
                    AggregatedStageDamage.AggregatedStageDamageElement stageDamElem = GetSelectedElementOfType<AggregatedStageDamageElement>(StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>(), stageDamName);
                    builder.WithAggStageDamageElem(stageDamElem);
                }

                return builder.build();
            }
            else
            {
                return null;
            }
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
                StudyCacheForSaving.AddConditionsElement((ConditionsElement)element);
                
            }
        }

        public void Remove(ChildElement element)
        {
            RemoveFromParentTable(element, TableName);
            //DeleteChangeTableAndAssociatedTables(element, ChangeTableConstant);
            StudyCacheForSaving.RemoveConditionsElement((ConditionsElement)element);// RemoveRatingElement((RatingCurveElement)element);

        }

        public void SaveExisting(ChildElement oldElement, ChildElement element, int changeTableIndex  )
        {
            if (DidParentTableRowValuesChange(element, GetRowDataFromElement((ConditionsElement)element), oldElement.Name, TableName))
            {
                UpdateParentTableRow(element.Name, changeTableIndex, GetRowDataFromElement((ConditionsElement)element), oldElement.Name, TableName, false, ChangeTableConstant);
                StudyCacheForSaving.UpdateConditionsElement((ConditionsElement)oldElement, (ConditionsElement)element);
            }
        }

        public List<ChildElement> Load()
        {
            return CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
        }


        public override void AddValidationRules()
        {
            // throw new NotImplementedException();
        }


    }
}
