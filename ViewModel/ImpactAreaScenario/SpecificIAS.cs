using compute;
using metrics;
using Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using ViewModel.AggregatedStageDamage;
using ViewModel.FlowTransforms;
using ViewModel.FrequencyRelationships;
using ViewModel.GeoTech;
using ViewModel.ImpactArea;
using ViewModel.ImpactAreaScenario.Editor;
using ViewModel.StageTransforms;

namespace ViewModel.ImpactAreaScenario
{
    public class SpecificIAS:BaseViewModel
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
        public metrics.Results ComputeResults { get; set; }

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
            Thresholds.AddRange(thresholds);
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

            Thresholds.AddRange( ReadThresholdsXML(iasElem.Element(THRESHOLDS)));
        }
       
        #endregion
        
        /// <summary>
        /// Intentionally leaving commented out for now. - Cody 10/26/21
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public metrics.Results ComputeScenario(object arg1, EventArgs arg2)
        {
            AnalyticalFrequencyElement freqElem = (AnalyticalFrequencyElement)StudyCache.GetChildElementOfType(typeof(AnalyticalFrequencyElement), FlowFreqID);
            InflowOutflowElement inOutElem = (InflowOutflowElement)StudyCache.GetChildElementOfType(typeof(InflowOutflowElement), InflowOutflowID);
            RatingCurveElement ratElem = (RatingCurveElement)StudyCache.GetChildElementOfType(typeof(RatingCurveElement), RatingID);
            ExteriorInteriorElement extIntElem = (ExteriorInteriorElement)StudyCache.GetChildElementOfType(typeof(ExteriorInteriorElement), ExtIntStageID);
            LeveeFeatureElement leveeElem = (LeveeFeatureElement)StudyCache.GetChildElementOfType(typeof(LeveeFeatureElement), LeveeFailureID);
            AggregatedStageDamageElement stageDamageElem = (AggregatedStageDamageElement)StudyCache.GetChildElementOfType(typeof(AggregatedStageDamageElement), StageDamageID);

            SimulationCreator sc = new SimulationCreator(freqElem, inOutElem, ratElem, extIntElem, leveeElem,
                stageDamageElem, ImpactAreaID);

            Simulation simulation = sc.BuildSimulation();

            MeanRandomProvider mrp = new MeanRandomProvider();
            try
            {
                //todo: how many iterations?
                //metrics.Results result = simulation.Compute(mrp, 1);
                //Console.WriteLine("Mean ead: " + result.ExpectedAnnualDamageResults.MeanEAD("InteriorStageDamage"));
                //double ead = result.ExpectedAnnualDamageResults.MeanEAD("InteriorStageDamage");
                //double total = result.ExpectedAnnualDamageResults.MeanEAD("Total");
                //ComputeResults = result;
                //return result;
                return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }



        private XElement WriteThresholdsToXML()
        {
            XElement functionsElem = new XElement("Thresholds");

            foreach (ThresholdRowItem row in Thresholds)
            {
                XElement rowElement = new XElement("Row");
                rowElement.SetAttributeValue("Type", row.ThresholdType.Metric);
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
                ThresholdType metricEnum = ConvertStringToMetricEnum(thresholdType);
                double thresholdValue = Double.Parse(rowElem.Attribute("Value").Value);
                thresholdRows.Add(new ThresholdRowItem(i, metricEnum.Metric, thresholdValue));
            }
            return thresholdRows;
        }

        private ThresholdType ConvertStringToMetricEnum(string metric)
        {
            switch (metric.ToUpper())
            {
                case "NOTSET":
                    {
                        return new ThresholdType( ThresholdEnum.NotSupported, "Not Set");
                    }
                case "EXTERIORSTAGEAEP":
                case "EXTERIORSTAGE":
                    {
                        return new ThresholdType(ThresholdEnum.ExteriorStage, "Exterior Stage");
                    }
                case "INTERIORSTAGEAEP":
                case "INTERIORSTAGE":
                    {
                        return new ThresholdType(ThresholdEnum.InteriorStage, "Interior Stage");
                    }
                case "DAMAGEAEP":
                case "DAMAGES":
                    {
                        return new ThresholdType(ThresholdEnum.Damage, "Damages");
                    }
                //case "EAD":
                //case "EXPECTEDANNUALDAMAGE":
                    {
                        //todo: do what?
                        //return new ThresholdType( IMetricEnum.ExpectedAnnualDamage, "Expected Annual Damage");
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException("Could not convert string: " + metric + " to an IMetricEnum.");
                    }
            }
        }

    }
}
