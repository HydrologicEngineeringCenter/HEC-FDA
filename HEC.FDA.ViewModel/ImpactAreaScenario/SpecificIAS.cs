using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario.Editor;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Utilities;
using Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.ImpactAreaScenario
{
    public class SpecificIAS : BaseViewModel
    {
        #region Notes
        #endregion
        #region Fields
        private const string ID = "ID";
        private const string IAS = "IAS";
        private const string IMPACT_AREA = "ImpactArea";
        private const string HAS_NON_FAILURE_STAGE_DAMAGE = "HasNonFailureStageDamage";
        private const string FREQUENCY_RELATIONSHIP = "FrequencyRelationship";
        private const string INFLOW_OUTFLOW = "InflowOutflow";
        private const string RATING = "Rating";
        private const string LATERAL_STRUCTURE = "LateralStructure";
        private const string EXTERIOR_INTERIOR = "ExteriorInterior";
        private const string STAGE_DAMAGE = "StageDamage";
        private const string THRESHOLDS = "Thresholds";
        private const string SCENARIO_REFLECTS_WITHOUT_PROJ = "ScenarioReflectsWithoutProject";
        private const string DEFAULT_STAGE = "DefaultStage";


        #endregion
        #region Properties

        public ImpactAreaScenarioSimulation Simulation { get; }

        /// <summary>
        /// These are the results after doing a compute. If a compute has not been
        /// done, then this will be null.
        /// </summary>
        //public ImpactAreaScenarioResults ComputeResults { get; set; }

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

        public List<ThresholdRowItem> Thresholds { get; } = new List<ThresholdRowItem>();

        public bool ScenarioReflectsWithoutProj {get;set;}
        public double DefaultStage { get; set; }
        public bool HasNonFailureStageDamage { get; set; }
        public int NonFailureStageDamageID { get; set; }


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
            int leveeFailureID, int stageDamageID, List<ThresholdRowItem> thresholds, bool scenarioReflectsWithoutProj, 
            double defaultStage, bool hasNonFailureStageDamage, int nonFailureStageDamageID) : base()
        {
            HasNonFailureStageDamage = hasNonFailureStageDamage;
            NonFailureStageDamageID = nonFailureStageDamageID;
            DefaultStage = defaultStage;
            ScenarioReflectsWithoutProj = scenarioReflectsWithoutProj;
            ImpactAreaID = impactAreaID;
            FlowFreqID = flowFreqID;
            InflowOutflowID = inflowOutflowID;
            RatingID = ratingID;
            ExtIntStageID = extIntID;
            LeveeFailureID = leveeFailureID;
            StageDamageID = stageDamageID;
            Thresholds.AddRange(thresholds);
        }

        /// <summary>
        /// This ctor is used to load elements from the database.
        /// </summary>
        /// <param name="iasElem"></param>
        public SpecificIAS(XElement iasElem)
        {
            ImpactAreaID = int.Parse(iasElem.Attribute(IMPACT_AREA).Value);
            //this is new, check if it exists first for backwards compatibility. 
            XAttribute hasNonFailure = iasElem.Attribute(HAS_NON_FAILURE_STAGE_DAMAGE);
            if(hasNonFailure != null)
            {
                HasNonFailureStageDamage = Boolean.Parse(hasNonFailure.Value);
            }
            else
            {
                HasNonFailureStageDamage = false;
            }

            FlowFreqID = int.Parse(iasElem.Element(FREQUENCY_RELATIONSHIP).Attribute(ID).Value);
            InflowOutflowID = int.Parse(iasElem.Element(INFLOW_OUTFLOW).Attribute(ID).Value);
            RatingID = int.Parse(iasElem.Element(RATING).Attribute(ID).Value);
            LeveeFailureID = int.Parse(iasElem.Element(LATERAL_STRUCTURE).Attribute(ID).Value);
            ExtIntStageID = int.Parse(iasElem.Element(EXTERIOR_INTERIOR).Attribute(ID).Value);
            StageDamageID = int.Parse(iasElem.Element(STAGE_DAMAGE).Attribute(ID).Value);

            //check if new elements exist before reading
            if (iasElem.Elements(SCENARIO_REFLECTS_WITHOUT_PROJ).Any())
            {
                ScenarioReflectsWithoutProj = Convert.ToBoolean(iasElem.Element(SCENARIO_REFLECTS_WITHOUT_PROJ).Attribute("value").Value);
                DefaultStage = double.Parse(iasElem.Element(DEFAULT_STAGE).Attribute("value").Value);
            }
            XElement thresholdElement = iasElem.Element(THRESHOLDS);
            List<ThresholdRowItem> thresholdRowItems = ReadThresholdsXML(thresholdElement);
            Thresholds.AddRange(thresholdRowItems);
        }

        #endregion

        private string CreateElementDoesntExistMessage(string elemName)
        {
            return "The selected " + elemName + " no longer exists.";
        }

        public string GetSpecificImpactAreaName()
        {
            string name = "";
            ImpactAreaElement impactAreaElem = (ImpactAreaElement)StudyCache.GetChildElementsOfType(typeof(ImpactAreaElement))[0];
            foreach(ImpactAreaRowItem row in impactAreaElem.ImpactAreaRows)
            {
                if(row.ID == ImpactAreaID)
                {
                    name = row.Name;
                }
            }
            return name;
        }

        public FdaValidationResult CanComputeScenario()
        {
            FdaValidationResult vr = new FdaValidationResult();
            vr.AddErrorMessage(DoesScenarioChildElementsStillExist().ErrorMessage);
            //insert the name of the impact area if not valid
            if(!vr.IsValid)
            {
                vr.InsertMessage(0, "Cannot compute scenario configuration for impact area: " + GetSpecificImpactAreaName());
            }
            return vr;
        }

        private FdaValidationResult DoesScenarioChildElementsStillExist()
        {
            FdaValidationResult vr = new FdaValidationResult();
            //required elems
            FrequencyElement freqElem = (FrequencyElement)StudyCache.GetChildElementOfType(typeof(FrequencyElement), FlowFreqID);
            AggregatedStageDamageElement stageDamageElem = (AggregatedStageDamageElement)StudyCache.GetChildElementOfType(typeof(AggregatedStageDamageElement), StageDamageID);

            if(InflowOutflowID != -1)
            {
                InflowOutflowElement inOutElem = (InflowOutflowElement)StudyCache.GetChildElementOfType(typeof(InflowOutflowElement), InflowOutflowID);
                if (inOutElem == null)
                {
                    vr.AddErrorMessage(CreateElementDoesntExistMessage("regulated-unregulated element"));
                }
            }
            if (RatingID != -1)
            {
                StageDischargeElement ratElem = (StageDischargeElement)StudyCache.GetChildElementOfType(typeof(StageDischargeElement), RatingID);
                if (ratElem == null)
                {
                    vr.AddErrorMessage(CreateElementDoesntExistMessage("stage-discharge element"));
                }
            }
            if (ExtIntStageID != -1)
            {
                ExteriorInteriorElement extIntElem = (ExteriorInteriorElement)StudyCache.GetChildElementOfType(typeof(ExteriorInteriorElement), ExtIntStageID);
                if (extIntElem == null)
                {
                    vr.AddErrorMessage(CreateElementDoesntExistMessage("exterior-interior element"));
                }
            }
            if (LeveeFailureID != -1)
            {
                LateralStructureElement leveeElem = (LateralStructureElement)StudyCache.GetChildElementOfType(typeof(LateralStructureElement), LeveeFailureID);
                if (leveeElem == null)
                {
                    vr.AddErrorMessage(CreateElementDoesntExistMessage("lateral structure element"));
                }
            }

            if(freqElem == null)
            {
                vr.AddErrorMessage(CreateElementDoesntExistMessage("frequency element"));
            }
            if (stageDamageElem == null)
            {
                vr.AddErrorMessage(CreateElementDoesntExistMessage("stage damage element"));
            }

        
            return vr;
        }

        private SimulationCreator GetSimulationCreator()
        {
            FrequencyElement freqElem = (FrequencyElement)StudyCache.GetChildElementOfType(typeof(FrequencyElement), FlowFreqID);
            InflowOutflowElement inOutElem = (InflowOutflowElement)StudyCache.GetChildElementOfType(typeof(InflowOutflowElement), InflowOutflowID);
            StageDischargeElement ratElem = (StageDischargeElement)StudyCache.GetChildElementOfType(typeof(StageDischargeElement), RatingID);
            ExteriorInteriorElement extIntElem = (ExteriorInteriorElement)StudyCache.GetChildElementOfType(typeof(ExteriorInteriorElement), ExtIntStageID);
            LateralStructureElement leveeElem = (LateralStructureElement)StudyCache.GetChildElementOfType(typeof(LateralStructureElement), LeveeFailureID);
            AggregatedStageDamageElement stageDamageElem = (AggregatedStageDamageElement)StudyCache.GetChildElementOfType(typeof(AggregatedStageDamageElement), StageDamageID);
            AggregatedStageDamageElement nonFailureStageDamageElem = (AggregatedStageDamageElement)StudyCache.GetChildElementOfType(typeof(AggregatedStageDamageElement), NonFailureStageDamageID);

            SimulationCreator sc = new SimulationCreator(freqElem, inOutElem, ratElem, extIntElem, leveeElem, stageDamageElem, ImpactAreaID, 
                HasNonFailureStageDamage, nonFailureStageDamageElem);

            int thresholdIndex = 1;
            foreach (ThresholdRowItem thresholdRow in Thresholds)
            {
                double thresholdValue = 0;
                if (thresholdRow.ThresholdValue != null)
                {
                    thresholdValue = thresholdRow.ThresholdValue.Value;
                }
                ConvergenceCriteria cc = StudyCache.GetStudyPropertiesElement().GetStudyConvergenceCriteria();
                Threshold threshold = new Threshold(thresholdIndex, cc, thresholdRow.ThresholdType.Metric, thresholdValue);
                sc.WithAdditionalThreshold(threshold);
                thresholdIndex++;
            }

            return sc;
        }

        public ImpactAreaScenarioSimulation CreateSimulation()
        {
            ImpactAreaScenarioSimulation simulation = null;
            SimulationCreator sc = GetSimulationCreator();
            simulation = sc.BuildSimulation();
            return simulation;
        }

        private XElement WriteThresholdsToXML()
        {
            XElement functionsElem = new XElement("Thresholds");

            foreach (ThresholdRowItem row in Thresholds)
            {
                XElement rowElement = row.ToXML();
                functionsElem.Add(rowElement);
            }

            return functionsElem;
        }

        public XElement WriteToXML()
        {
            XElement iasElement = new XElement(IAS);
            iasElement.SetAttributeValue(IMPACT_AREA, ImpactAreaID);
            iasElement.SetAttributeValue(HAS_NON_FAILURE_STAGE_DAMAGE, HasNonFailureStageDamage);

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

            XElement scenarioReflectsWithoutProjElem = new XElement(SCENARIO_REFLECTS_WITHOUT_PROJ);
            scenarioReflectsWithoutProjElem.SetAttributeValue("value", ScenarioReflectsWithoutProj);
            iasElement.Add(scenarioReflectsWithoutProjElem);

            XElement defaultStageElem = new XElement(DEFAULT_STAGE);
            defaultStageElem.SetAttributeValue("value", DefaultStage);
            iasElement.Add(defaultStageElem);

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
                thresholdRows.Add(new ThresholdRowItem(rowElem, i));
            }
            return thresholdRows;
        }

    }
}
