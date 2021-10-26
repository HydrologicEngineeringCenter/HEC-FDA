using Model;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using ViewModel.ImpactAreaScenario.Editor;

namespace ViewModel.ImpactAreaScenario
{
    public class SpecificIAS
    {
        #region Notes
        #endregion
        #region Fields
        private const string ID = "ID";
        private const string IAS = "IAS";
        private const string IMPACT_AREA = "ImpactArea";
        private const string FREQUENCY_RELATIONSHIP = "FrequencyRelationship";
        private const string INFLOW_OUTFLOW = "InflowOutflow";
        private const string RATING = "Rating";
        private const string LATERAL_STRUCTURE = "LateralStructure";
        private const string EXTERIOR_INTERIOR = "ExteriorInterior";
        private const string STAGE_DAMAGE = "StageDamage";
        private const string THRESHOLDS = "Thresholds";


        #endregion
        #region Properties

        /// <summary>
        /// These are the results after doing a compute. If a compute has not been
        /// done, then this will be null.
        /// </summary>
        public IConditionLocationYearResult ComputeResults { get; set; }



        /// <summary>
        /// The impact area ID for the selected impact area. It will be -1 if no selection was made.
        /// </summary>
        public int ImpactAreaID { get; set; }
        /// <summary>
        /// The flow freq ID for the selected flow freq. It will be -1 if no selection was made.
        /// </summary>
        public int FlowFreqID { get; set; }
        /// <summary>
        /// The inflow outflow ID for the selected inflow outflow. It will be -1 if no selection was made.
        /// </summary>
        public int InflowOutflowID { get; set; }
        /// <summary>
        /// The rating ID for the selected rating. It will be -1 if no selection was made.
        /// </summary>
        public int RatingID { get; set; }
        /// <summary>
        /// The levee failure ID for the selected levee failure. It will be -1 if no selection was made.
        /// </summary>
        public int LeveeFailureID { get; set; }
        /// <summary>
        /// The exterior interior stage ID for the selected ext int stage. It will be -1 if no selection was made.
        /// </summary>
        public int ExtIntStageID { get; set; }
        /// <summary>
        /// The stage damage ID for the selected stage damage. It will be -1 if no selection was made.
        /// </summary>
        public int StageDamageID { get; set; }


        public List<ThresholdRowItem> Thresholds { get; set; }

        #endregion
        #region Constructors

/// <summary>
/// This is a specific IAS element. The IAS element set will hold a list of these.
/// </summary>
/// <param name="impactAreaID"></param>
/// <param name="flowFreqID"></param>
/// <param name="inflowOutflowID"></param>
/// <param name="ratingID"></param>
/// <param name="extIntID"></param>
/// <param name="leveeFailureID"></param>
/// <param name="stageDamageID"></param>
/// <param name="thresholds"></param>
        public SpecificIAS(int impactAreaID, int flowFreqID, int inflowOutflowID, int ratingID, int extIntID, 
            int leveeFailureID, int stageDamageID, List<ThresholdRowItem> thresholds) : base()
        {
            ImpactAreaID = impactAreaID;
            FlowFreqID = flowFreqID;
            InflowOutflowID = inflowOutflowID;
            RatingID = ratingID;
            ExtIntStageID = extIntID;
            LeveeFailureID = leveeFailureID;
            StageDamageID = stageDamageID;
            Thresholds = thresholds;
        }

        /// <summary>
        /// This ctor is used to load elements from the database.
        /// </summary>
        /// <param name="iasElem"></param>
        public SpecificIAS(XElement iasElem)
        {
            List<ThresholdRowItem> thresholdRows = new List<ThresholdRowItem>();

            ImpactAreaID = Int32.Parse(iasElem.Attribute(IMPACT_AREA).Value);

            FlowFreqID = Int32.Parse(iasElem.Element(FREQUENCY_RELATIONSHIP).Attribute(ID).Value);
            InflowOutflowID = Int32.Parse(iasElem.Element(INFLOW_OUTFLOW).Attribute(ID).Value);
            RatingID = Int32.Parse(iasElem.Element(RATING).Attribute(ID).Value);
            LeveeFailureID = Int32.Parse(iasElem.Element(LATERAL_STRUCTURE).Attribute(ID).Value);
            ExtIntStageID = Int32.Parse(iasElem.Element(EXTERIOR_INTERIOR).Attribute(ID).Value);
            StageDamageID = Int32.Parse(iasElem.Element(STAGE_DAMAGE).Attribute(ID).Value);

            Thresholds = ReadThresholdsXML(iasElem.Element(THRESHOLDS));        
        }
       
        #endregion
        
        /// <summary>
        /// Intentionally leaving commented out for now. - Cody 10/26/21
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void ComputeCondition(object arg1, EventArgs arg2)
        {

            //EnterSeedVM enterSeedVM = new EnterSeedVM();
            //string header = "Enter Seed Value";
            //DynamicTabVM tab = new DynamicTabVM(header, enterSeedVM, "EnterSeed");
            //Navigate(tab, true, true);

            //int seedValue = enterSeedVM.Seed;

            //IConditionLocationYearSummary condition = null;

            //IFrequencyFunction frequencyFunction = GetFrequencyFunction();
            //List<ITransformFunction> transformFunctions = GetTransformFunctions();

            //bool hasLeveeFailure = LeveeFailureID != -1;
            //if (hasLeveeFailure)
            //{
            //    LeveeFeatureElement leveeFailureElement = (LeveeFeatureElement)StudyCache.GetChildElementOfType(typeof(LeveeFeatureElement), LeveeFailureID);
            //    ILateralStructure latStruct = ILateralStructureFactory.Factory(leveeFailureElement.Elevation, (ITransformFunction)leveeFailureElement.Curve); 
            //    //todo: Need to handle multiple thresholds
            //    condition = Saving.PersistenceFactory.GetIASManager().CreateIConditionLocationYearSummary(ImpactAreaID,
            //        AnalysisYear, frequencyFunction, transformFunctions, leveeFailureElement, ThresholdType, ThresholdValue);
            //}
            //else 
            //{ 
            //    condition = Saving.PersistenceFactory.GetIASManager().CreateIConditionLocationYearSummary(ImpactAreaID,
            //        AnalysisYear, frequencyFunction, transformFunctions, ThresholdType, ThresholdValue);
            //}

            //if (condition == null)
            //{
            //    return;
            //}

            //IConvergenceCriteria convergenceCriteria = IConvergenceCriteriaFactory.Factory();
            //Dictionary<IMetric, IConvergenceCriteria> metricsDictionary = new Dictionary<IMetric, IConvergenceCriteria>();
            //foreach (IMetric metric in condition.Metrics)
            //{
            //    metricsDictionary.Add(metric, IConvergenceCriteriaFactory.Factory());
            //}

            //IReadOnlyDictionary<IMetric, IConvergenceCriteria> metrics = new ReadOnlyDictionary<IMetric, IConvergenceCriteria>(metricsDictionary);

            //IConditionLocationYearResult result = new ConditionLocationYearResult(condition, metrics, seedValue);
            //result.Compute();
            //ComputeResults = result;
            //Saving.PersistenceFactory.GetIASManager().SaveConditionResults(result, this.GetElementID(), frequencyFunction, transformFunctions);

            //DisplayResults(result);

        }



        private XElement WriteThresholdsToXML()
        {
            XElement functionsElem = new XElement("Thresholds");

            foreach (ThresholdRowItem row in Thresholds)
            {
                XElement rowElement = new XElement("Row");
                rowElement.SetAttributeValue("Type", row.ThresholdType);
                rowElement.SetAttributeValue("Value", row.ThresholdValue);
                functionsElem.Add(rowElement);
            }

            return functionsElem;
        }

        
    

        public XElement WriteToXML()
        {
            XElement iasElement = new XElement(IAS);
            iasElement.SetAttributeValue(IMPACT_AREA, ImpactAreaID);

            XElement freqRelationshipElem = new XElement(FREQUENCY_RELATIONSHIP);
            freqRelationshipElem.SetAttributeValue(ID, FlowFreqID);
            iasElement.Add(freqRelationshipElem);

            XElement inflowOutflowElem = new XElement(INFLOW_OUTFLOW);
            inflowOutflowElem.SetAttributeValue(ID, InflowOutflowID);
            iasElement.Add(inflowOutflowElem);

            XElement ratingElem = new XElement(RATING);
            ratingElem.SetAttributeValue(ID, RatingID);
            iasElement.Add(ratingElem);

            XElement latElem = new XElement(LATERAL_STRUCTURE);
            latElem.SetAttributeValue(ID, LeveeFailureID);
            iasElement.Add(latElem);

            XElement extIntElem = new XElement(EXTERIOR_INTERIOR);
            extIntElem.SetAttributeValue(ID, ExtIntStageID);
            iasElement.Add(extIntElem);

            XElement stageDamageElem = new XElement(STAGE_DAMAGE);
            stageDamageElem.SetAttributeValue(ID, StageDamageID);
            iasElement.Add(stageDamageElem);

            iasElement.Add(WriteThresholdsToXML());

            return iasElement;
        }


        private List<ThresholdRowItem> ReadThresholdsXML(XElement thresholdsElem)
        {
            List<ThresholdRowItem> thresholdRows = new List<ThresholdRowItem>();
            IEnumerable<XElement> rows = thresholdsElem.Elements("Row");
            int i = 0;
            foreach (XElement rowElem in rows)
            {
                i++;
                string thresholdType = rowElem.Attribute("Type").Value;
                IMetricEnum metricEnum = ConvertStringToMetricEnum(thresholdType);
                double thresholdValue = Double.Parse(rowElem.Attribute("Value").Value);
                thresholdRows.Add(new ThresholdRowItem(i, metricEnum, thresholdValue));
            }
            return thresholdRows;
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

    }
}
