using System.Collections.Generic;
using Statistics.GraphicalRelationships;
using System.Xml.Linq;
using System;
using HEC.FDA.Model.interfaces;
using HEC.MVVMFramework.Model.Messaging;
using Statistics.Graphical;
using HEC.FDA.Model.utilities;
using System.Linq;

namespace HEC.FDA.Model.paireddata
{
    public class GraphicalUncertainPairedData : ValidationErrorLogger, IPairedDataProducer, ICanBeNull, IMetaData
    {

        #region Properties
        internal GraphicalDistribution GraphicalDistributionWithLessSimple { get; }
        public CurveMetaData CurveMetaData { get; private set; }
        /// <summary>
        /// Exceedance probabilities are the required and the input, combined.
        /// </summary>
        public double[] CombinedExceedanceProbabilities { get; private set; }
        public bool IsNull
        {
            get
            {
                return CurveMetaData.IsNull;
            }
        }
        #endregion

        #region Constructors
        public GraphicalUncertainPairedData()
        {
            CurveMetaData = new();
            GraphicalDistributionWithLessSimple = new();
            CombinedExceedanceProbabilities = new double[] { 0 };
        }
        public GraphicalUncertainPairedData(double[] exceedanceProbabilities, double[] flowOrStageValues, int equivalentRecordLength, CurveMetaData curveMetaData, bool usingStagesNotFlows)
        {
            GraphicalDistributionWithLessSimple = new GraphicalDistribution(exceedanceProbabilities, flowOrStageValues, equivalentRecordLength, usingStagesNotFlows);
            CurveMetaData = curveMetaData;
            //combine required and input probabilities 
            CombinedExceedanceProbabilities =  CombineInputAndRequiredExceedanceProbabilities(exceedanceProbabilities);
        }
        private GraphicalUncertainPairedData(double[] combinedExceedanceProbabilities, GraphicalDistribution graphicalDistributionWithLessSimple, CurveMetaData curveMetaData)
        {
            GraphicalDistributionWithLessSimple = graphicalDistributionWithLessSimple;
            CombinedExceedanceProbabilities = combinedExceedanceProbabilities;
            CurveMetaData = curveMetaData;
        }
        #endregion

        #region Methods
        private double[] ExceedanceToNonExceedance(double[] exceedanceProbabilities)
        {
            double[] nonExceedanceProbabilities = new double[exceedanceProbabilities.Length];
            for (int i = 0; i < nonExceedanceProbabilities.Length; i++)
            {
                nonExceedanceProbabilities[i] = 1 - exceedanceProbabilities[i];
            }
            return nonExceedanceProbabilities;
        }
        //compute with deterministic is an unused argument because graphical returns deterministically for the median random provider 
        public PairedData SamplePairedData(double probability, bool computeIsDeterministic = false)
        {
            double[] y = new double[GraphicalDistributionWithLessSimple.StageOrLogFlowDistributions.Length];

            for (int i = 0; i < GraphicalDistributionWithLessSimple.StageOrLogFlowDistributions.Length; i++)
            {
                if (GraphicalDistributionWithLessSimple.UsingStagesNotFlows)
                {
                    y[i] = GraphicalDistributionWithLessSimple.StageOrLogFlowDistributions[i].InverseCDF(probability);
                }
                else
                {
                    y[i] = Math.Log(GraphicalDistributionWithLessSimple.StageOrLogFlowDistributions[i].InverseCDF(probability));
                }
            }
            PairedData pairedData = new PairedData(ExceedanceToNonExceedance(GraphicalDistributionWithLessSimple.ExceedanceProbabilities), y, CurveMetaData);
            bool isMonotonicallyIncreasing = IsMonotonicallyIncreasing(pairedData);
            if (!isMonotonicallyIncreasing)
            {
                pairedData.ForceStrictMonotonicity();
            }
            double[] expandedStageOrLogFlowValues = InterpolateQuantiles.InterpolateOnX(pairedData.Xvals, CombinedExceedanceProbabilities, pairedData.Yvals);
            if (!GraphicalDistributionWithLessSimple.UsingStagesNotFlows)
            {
                double[] tempArray = new double[expandedStageOrLogFlowValues.Length];
                for (int i = 0; i < expandedStageOrLogFlowValues.Length; i++)
                {
                    tempArray[i] = Math.Exp(expandedStageOrLogFlowValues[i]);
                }
                expandedStageOrLogFlowValues = tempArray;
            }
            PairedData expandedPairedData = new PairedData(ExceedanceToNonExceedance(CombinedExceedanceProbabilities), expandedStageOrLogFlowValues);
            return expandedPairedData;
        }

        private bool IsMonotonicallyIncreasing(IPairedData pairedData)
        {
            for (int i = 1; i < pairedData.Yvals.Length; i++)
            {
                if (pairedData.Yvals[i] < pairedData.Yvals[i - 1])
                {
                    return false;
                }
            }
            return true;
        }

        public bool Equals(GraphicalUncertainPairedData incomingGraphicalUncertainPairedData)
        {
            bool nullMatches = CurveMetaData.IsNull.Equals(incomingGraphicalUncertainPairedData.CurveMetaData.IsNull);
            if (nullMatches && IsNull)
            {
                return true;
            }
            bool erlIsTheSame = GraphicalDistributionWithLessSimple.EquivalentRecordLength.Equals(incomingGraphicalUncertainPairedData.GraphicalDistributionWithLessSimple.EquivalentRecordLength);
            if (!erlIsTheSame)
            {
                return false;
            }
            for (int i = 0; i < GraphicalDistributionWithLessSimple.StageOrLogFlowDistributions.Length; i++)
            {
                bool distributionIsTheSame = GraphicalDistributionWithLessSimple.StageOrLogFlowDistributions[i].Equals(incomingGraphicalUncertainPairedData.GraphicalDistributionWithLessSimple.StageOrLogFlowDistributions[i]);
                if (!distributionIsTheSame)
                {
                    return false;
                }
            }
            for (int i = 0; i < CombinedExceedanceProbabilities.Length; i++)
            {
                bool probabilityIsTheSame = CombinedExceedanceProbabilities[i].Equals(incomingGraphicalUncertainPairedData.CombinedExceedanceProbabilities[i]);
                if (!probabilityIsTheSame)
                {
                    return false;
                }
            }
            return true;
        }

        private double[] CombineInputAndRequiredExceedanceProbabilities(double[] inputExceedanceProbabilities)
        {
            List<double> allProbabilities = DoubleGlobalStatics.RequiredExceedanceProbabilities.ToList();
            foreach (double probability in inputExceedanceProbabilities)
            {
                if (!allProbabilities.Contains(probability))
                {
                    allProbabilities.Add(probability);
                }
            }
            allProbabilities.Sort((a, b) => b.CompareTo(a));
            return allProbabilities.ToArray();
        }

        #endregion
        #region XML Methods
        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("Graphical_Uncertain_Paired_Data");

            XElement curveMetaDataElement = CurveMetaData.WriteToXML();
            curveMetaDataElement.Name = "CurveMetaData";
            masterElement.Add(curveMetaDataElement);

            XElement graphicalElement = GraphicalDistributionWithLessSimple.WriteToXML();
            graphicalElement.Name = "Graphical";
            masterElement.Add(graphicalElement);

            XElement probabilities = new XElement("Probabilities");
            foreach (double probability in CombinedExceedanceProbabilities) 
            {
                XElement rowElement = new XElement("Probability");
                rowElement.SetAttributeValue("Value", probability);
                probabilities.Add(rowElement);
            }
            masterElement.Add(probabilities);
            
            return masterElement;
        }

        public static GraphicalUncertainPairedData ReadFromXML(XElement xElement)
        {
            CurveMetaData metaData = CurveMetaData.ReadFromXML(xElement.Element("CurveMetaData"));
            if (metaData.IsNull)
            {
                return new GraphicalUncertainPairedData();
            }
            else
            {
                GraphicalDistribution graphicalDistributionWithLessSimple = GraphicalDistribution.ReadFromXML(xElement.Element("Graphical"));
                
                List<double> combinedProbabilities = new List<double>();
                foreach (XElement valueElement in xElement.Element("Probabilities").Elements())
                {
                    double value = Convert.ToDouble(valueElement.Attribute("Value").Value);
                    combinedProbabilities.Add(value);
                }
                return new GraphicalUncertainPairedData(combinedProbabilities.ToArray(), graphicalDistributionWithLessSimple, metaData);
            }

        }
        #endregion
    }
}


