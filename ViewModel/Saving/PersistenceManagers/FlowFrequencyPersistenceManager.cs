using ViewModel.FrequencyRelationships;
using ViewModel.Utilities;
using Functions;
using Model;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Importer;
using static Importer.ProbabilityFunction;

namespace ViewModel.Saving.PersistenceManagers
{
    public class FlowFrequencyPersistenceManager :UndoRedoBase, IPersistableWithUndoRedo
    {
        private const int ID_COL = 0;
        private const int NAME_COL = 1;
        private const int LAST_EDIT_DATE_COL = 2;
        private const int DESC_COL = 3;
        private const int YEARS_OF_RECORD_COL = 4;

        private const int IS_ANALYTICAL_COL = 5;
        private const int IS_STANDARD_COL = 6;

        private const int MEAN_COL = 7;
        private const int ST_DEV_COL = 8;
        private const int SKEW_COL = 9;

        private const int IS_LOG_FLOW_COL = 10;
        private const int ANALYTICAL_FLOWS_COL = 11;
        private const int GRAPHICAL_FLOWS_COL = 12;

        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("FlowFrequencyPersistenceManager");
        //ELEMENT_TYPE is used to store the type of element in the log tables.
        private const string ELEMENT_TYPE = "Flow_Freq";

        /// <summary>
        /// The name of the parent table that will hold all elements of this type
        /// </summary>
        public override string TableName { get { return "analytical_frequency_curves"; } }
        public override string ChangeTableName { get { return "analytical_frequency_changes"; } }

        public override string[] TableColumnNames
        {
            get { return new string[] { NAME, LAST_EDIT_DATE, DESCRIPTION, "por", "is_analytical", 
                "is_standard", "mean", "standard_deviation", "skew", "is_log_flow", "analytical_flows", "graphical_flows" }; }
        }

        public override Type[] TableColumnTypes
        {
            get 
            { 
                return new Type[] 
                { 
                typeof(string), typeof(string), typeof(string),
                typeof(int), typeof(bool), typeof(bool), typeof(double), typeof(double),
                typeof(double),typeof(bool), typeof(string), typeof(string) 
                }; 
            }
        }


        public override string[] ChangeTableColumnNames
        {
            get
            {
                return new string[] { ELEMENT_ID_COL_NAME, NAME, LAST_EDIT_DATE, DESCRIPTION,
                "por", "is_analytical", "is_standard", "mean", "standard_deviation", "skew", 
                    "is_log_flow", "analytical_flows", "graphical_flows", STATE_INDEX_COL_NAME};
            }

        }

public override Type[] ChangeTableColumnTypes
        {
            get
            {
                return new Type[] 
                {
                    typeof(int), typeof(string), typeof(string), typeof(string),
                    typeof(int), typeof(bool), typeof(bool), typeof(double), typeof(double),
                    typeof(double),typeof(bool), typeof(string), typeof(string) , typeof(int) 
                };
            }
        }


        public FlowFrequencyPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        private object[] GetRowDataFromElement(AnalyticalFrequencyElement element)
        {

            string analyticalFlows = ConvertFlowsToString(element.AnalyticalFlows);
            string graphicalFlows = ConvertFlowsToString(element.GraphicalFlows);
            return new object[]
            {
                element.Name, element.LastEditDate, element.Description, element.POR, element.IsAnalytical, element.IsStandard,
                element.Mean, element.StDev, element.Skew, element.IsLogFlow, analyticalFlows, graphicalFlows
            };

        }

        private string ConvertFlowsToString(List<double> flows)
        {
            if(flows.Count == 0)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            foreach(double d in flows)
            {
                sb.Append(d + ",");
            }
            //remove the last comma
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        private List<double> ConvertStringToFlows(string flows)
        {
            List<double> flowDoubles = new List<double>();
            try
            {
                string[] flowStrings = flows.Split(',');

                foreach (string flow in flowStrings)
                {
                    double d = Convert.ToDouble(flow);
                    flowDoubles.Add(d);
                }
            }
            catch (Exception e)
            {
                //couldn't convert to doubles
            }
            return flowDoubles;
        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            string name = (string)rowData[NAME_COL];
            string desc = (string)rowData[DESC_COL];
            string lastEditDate = (string)rowData[LAST_EDIT_DATE_COL];
            int por = Convert.ToInt32( rowData[YEARS_OF_RECORD_COL]);

            bool isAnalytical = Convert.ToBoolean(rowData[IS_ANALYTICAL_COL]);
            bool isStandard = Convert.ToBoolean(rowData[IS_STANDARD_COL]);

            double mean = Convert.ToDouble( rowData[MEAN_COL]);
            double stdev = Convert.ToDouble(rowData[ST_DEV_COL]);
            double skew = Convert.ToDouble(rowData[SKEW_COL]);

            bool isLogFlow = Convert.ToBoolean(rowData[IS_LOG_FLOW_COL]);
            List<double> analyticalFlows = ConvertStringToFlows((string)rowData[ANALYTICAL_FLOWS_COL]);
            List<double> graphicalFlows = ConvertStringToFlows((string)rowData[GRAPHICAL_FLOWS_COL]);

            IFdaFunction func = CreateFdaFunction(mean, stdev, skew, isLogFlow, por, isAnalytical, isStandard, analyticalFlows);
            return new AnalyticalFrequencyElement(name, lastEditDate, desc, por, isAnalytical, isStandard, mean, stdev, skew,
                isLogFlow, analyticalFlows, graphicalFlows, func);

        }

        public IFdaFunction CreateFdaFunction(double Mean, double StandardDeviation, double Skew, bool IsLogFlow, int PeriodOfRecord,
            bool IsAnalytical, bool IsStandard, List<double> analyticalFlows)
        {
            List<FlowDoubleWrapper> AnalyticalFlows = new List<FlowDoubleWrapper>();
            foreach(double d in analyticalFlows)
            {
                FlowDoubleWrapper fdw = new FlowDoubleWrapper(d);
                AnalyticalFlows.Add(fdw);
            }

            List<double> xs = new List<double>();
            List<double> ys = new List<double>();
            try
            {
                if (IsAnalytical)
                {
                    if (IsStandard)
                    {
                        //todo use mean, st dev, and skew to create the curve

                        //return ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
                        IDistribution dist = IDistributionFactory.FactoryLogPearsonIII(Mean, StandardDeviation, Skew, PeriodOfRecord);
                        if (dist.State < IMessageLevels.Error)
                        {
                            IFunction func = IFunctionFactory.Factory(dist);
                            return IFdaFunctionFactory.Factory(IParameterEnum.InflowFrequency, func);

                        }

                        //return ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
                    }
                    else
                    {
                        List<double> flows = new List<double>();
                        foreach (FlowDoubleWrapper d in AnalyticalFlows)
                        {
                            flows.Add(d.Flow);
                        }

                        IDistribution dist = IDistributionFactory.FactoryFitLogPearsonIII(flows, IsLogFlow, PeriodOfRecord);
                        if (dist.State < IMessageLevels.Error)
                        {
                            IFunction func = IFunctionFactory.Factory(dist);
                            return IFdaFunctionFactory.Factory(IParameterEnum.InflowFrequency, func);
                        }
                        //return ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
            return null;

        }

        #endregion

        #region import from old fda
        public void SaveFDA1Element(ProbabilityFunction pf)
        {
            string pysr = "(" + pf.PlanName + " " + pf.YearName + " " + pf.StreamName + " " + pf.DamageReachName + ") ";
            string description = pysr + pf.Description;

            if (pf.ProbabilityFunctionTypeId == FrequencyFunctionType.ANALYTICAL)
            {
                if (pf.SourceOfStatisticsId == SourceOfStatistics.ENTERED)
                {

                    //LP3 moments
                    double mean = pf.MomentsLp3[0];
                    double stdDev = pf.MomentsLp3[1];
                    double skew = pf.MomentsLp3[2];
                    //call factory to create LP3
                    //Statistics.IDistributionFactory.fa
                    //Functions.IFunctionFactory.Factory()

                    //grab manager and save it.

                }
                else if (pf.SourceOfStatisticsId == SourceOfStatistics.CALCULATED)
                {
                    //analytical synthetic points
                    double flowPoint5 = pf.PointsSynthetic[0];
                    double flowPoint1 = pf.PointsSynthetic[1];
                    double flowPoint01 = pf.PointsSynthetic[2];
                    //call factory to create LP3
                    //Statistics.IDistributionFactory.fa
                    //Functions.IFunctionFactory.Factory()

                    //grab manager and save it.
                }
            }
            else if (pf.ProbabilityFunctionTypeId == FrequencyFunctionType.GRAPHICAL)
            {
                ////get probabilities
                //List<double> probabilities = new List<double>();
                //for (int i = 0; i < pf.NumberOfGraphicalPoints; i++)
                //{
                //    probabilities.Add(pf.ExceedanceProbability[i]);
                //}

                //if (pf.ProbabilityDataTypeId == ProbabilityDataType.DISCHARGE_FREQUENCY)
                //{
                //    Write("\t\tDischarge: ");
                //    for (int i = 0; i < pf.NumberOfGraphicalPoints; i++)
                //        Write($"\t{pf.Discharge[i]}");
                //}
                //else if (pf.ProbabilityDataTypeId == ProbabilityDataType.STAGE_FREQUENCY)
                //{
                //    Write("\t\tStage: ");
                //    for (int i = 0; i < pf.NumberOfGraphicalPoints; i++)
                //        Write($"\t{pf.Stage[i]}");
                //}
                ////User Defined Uncertainty
                //if (pf.UncertTypeSpecification == UncertaintyTypeSpecification.NORMAL)
                //{
                //    Write("\t\tNormal: ");
                //    for (int i = 0; i < pf.NumberOfGraphicalPoints; i++)
                //        Write($"\t{pf._StdDevNormalUserDef[i]}");
                //    Write("\n");
                //}
                //else if (pf.UncertTypeSpecification == UncertaintyTypeSpecification.LOG_NORMAL)
                //{
                //    Write("\t\tLog Normal: ");
                //    for (int i = 0; i < pf.NumberOfGraphicalPoints; i++)
                //        Write($"\t{pf._StdDevLogUserDef[i]}");
                //    Write("\n");
                //}
                //else if (pf.UncertTypeSpecification == UncertaintyTypeSpecification.TRIANGULAR)
                //{
                //    Write("\t\tTriangular High: ");
                //    for (int i = 0; i < pf.NumberOfGraphicalPoints; i++)
                //        Write($"\t{pf._StdDevUpperUserDef[i]}");
                //    Write("\n");
                //    Write("\t\tTriangular Low: ");
                //    for (int i = 0; i < pf.NumberOfGraphicalPoints; i++)
                //        Write($"\t{pf._StdDevLowerUserDef[i]}");
                //}
            }


            //if (pf._ProbabilityDataTypeId == ProbabilityDataType.DISCHARGE_FREQUENCY)
            {
                //List<ICoordinate> flowFreqCoords = new List<ICoordinate>();
                //foreach (Pair_xy xy in )
                //{
                //    double x = xy.GetX();
                //    double y = xy.GetY();
                //    flowFreqCoords.Add(ICoordinateFactory.Factory(x, y));
                //}
                //ICoordinatesFunction coordsFunction = ICoordinatesFunctionsFactory.Factory(flowFreqCoords, InterpolationEnum.Linear);
                //ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory()
                //ImpactAreaFunctionFactory.FactoryFrequency(, ImpactAreaFunctionEnum.InflowFrequency);
            }

        }


        #endregion

        /// <summary>
        /// Flow frequency doesn not save to its own table. All is contained in the parent row
        /// </summary>
        /// <param name="element"></param>
        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(AnalyticalFrequencyElement))
            {
                //save to parent table
                SaveNewElement(element);
                //save to change table
                SaveToChangeTable(element);
                //log message
                Log(FdaLogging.LoggingLevel.Info, "Created new flow frequency curve: " + element.Name, element.Name);
            }
        }

        public void Remove(ChildElement element)
        {
            base.Remove(element);
            //todo: do something here
            //RemoveFromParentTable(element, TableName);
            //DeleteChangeTableAndAssociatedTables(element, ChangeTableConstant);
            //StudyCacheForSaving.RemoveElement((AnalyticalFrequencyElement)element);

        }

        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex )
        {
            base.SaveExisting(oldElement, elementToSave, changeTableIndex);
            //string editDate = DateTime.Now.ToString("G");
            //elementToSave.LastEditDate = editDate;

            //if (DidParentTableRowValuesChange(elementToSave, GetRowDataFromElement((AnalyticalFrequencyElement)elementToSave), oldElement.Name, TableName) )
            //{
            //    UpdateParentTableRow(elementToSave.Name, changeTableIndex, GetRowDataFromElement((AnalyticalFrequencyElement)elementToSave), oldElement.Name, TableName, true, ChangeTableConstant);
            //    // SaveCurveTable(elementToSave.Curve, ChangeTableConstant, editDate);
            //    StudyCacheForSaving.UpdateFlowFrequencyElement((AnalyticalFrequencyElement)oldElement, (AnalyticalFrequencyElement)elementToSave);
            //}
        }

        public void Load()
        {
            List<ChildElement> flowFreqs = CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (AnalyticalFrequencyElement elem in flowFreqs)
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

        public override object[] GetRowDataForChangeTable(ChildElement element)
        {
            if (element.Description == null)
            {
                element.Description = "";
            }

            int elemId = GetElementId(TableName, element.Name);
            AnalyticalFrequencyElement elem = (AnalyticalFrequencyElement)element;
            //the new statId will be one higher than the max that is in the table already.
            int stateId = Storage.Connection.Instance.GetMaxStateIndex(ChangeTableName, elemId, ELEMENT_ID_COL_NAME, STATE_INDEX_COL_NAME) + 1;
            return new object[] { elemId, element.Name, element.LastEditDate, element.Description,
            elem.POR, elem.IsAnalytical, elem.IsStandard, elem.Mean, elem.StDev, elem.Skew, elem.IsLogFlow,
                ConvertFlowsToString(elem.AnalyticalFlows), ConvertFlowsToString(elem.GraphicalFlows), stateId};
                //todo: Refactor: CO
                //elem.Distribution.GetMean, elem.Distribution.GetStDev, elem.Distribution.GetG, elem.Distribution.GetSampleSize, stateId};
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            return GetRowDataFromElement((AnalyticalFrequencyElement)elem);
        }
    }
}
