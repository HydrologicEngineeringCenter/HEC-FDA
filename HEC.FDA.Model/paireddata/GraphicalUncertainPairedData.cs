﻿using System.Collections.Generic;
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
        #region Fields 
        private double[] _RandomNumbers;

        #endregion

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
            CombinedExceedanceProbabilities = CombineInputAndRequiredExceedanceProbabilities(exceedanceProbabilities);
        }
        private GraphicalUncertainPairedData(double[] combinedExceedanceProbabilities, GraphicalDistribution graphicalDistributionWithLessSimple, CurveMetaData curveMetaData)
        {
            GraphicalDistributionWithLessSimple = graphicalDistributionWithLessSimple;
            CombinedExceedanceProbabilities = combinedExceedanceProbabilities;
            CurveMetaData = curveMetaData;
        }
        #endregion

        #region Methods
        public void GenerateRandomNumbers(int seed, int size)
        {
            Random random = new Random(seed);
            double[] randos = new double[size];
            for (int i = 0; i < size; i++)
            {
                randos[i] = random.NextDouble();
            }
            _RandomNumbers = randos;
        }
        private static double[] ExceedanceToNonExceedance(double[] exceedanceProbabilities)
        {
            double[] nonExceedanceProbabilities = new double[exceedanceProbabilities.Length];
            for (int i = 0; i < nonExceedanceProbabilities.Length; i++)
            {
                nonExceedanceProbabilities[i] = 1 - exceedanceProbabilities[i];
            }
            return nonExceedanceProbabilities;
        }
        public PairedData SamplePairedData(double probability)
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
            PairedData pairedData = new(ExceedanceToNonExceedance(GraphicalDistributionWithLessSimple.ExceedanceProbabilities), y, CurveMetaData);
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
            PairedData expandedPairedData = new(ExceedanceToNonExceedance(CombinedExceedanceProbabilities), expandedStageOrLogFlowValues, CurveMetaData);
            return expandedPairedData;
        }

        public PairedData SamplePairedData(long iterationNumber, bool computeIsDeterministic = false)
        {
            double probability;
            double[] y = new double[GraphicalDistributionWithLessSimple.StageOrLogFlowDistributions.Length];
            if (computeIsDeterministic)
            {
                probability = 0.5;
            }
            else
            {
                if (_RandomNumbers.Length == 0)
                {
                    throw new Exception("Random numbers have not been created for UPD sampling");
                }
                if (iterationNumber < 0 || iterationNumber >= _RandomNumbers.Length)
                {
                    throw new Exception("Iteration number cannot be less than 0 or greater than the size of the random number array");

                }
                probability = _RandomNumbers[iterationNumber];
            }

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
            PairedData pairedData = new(ExceedanceToNonExceedance(GraphicalDistributionWithLessSimple.ExceedanceProbabilities), y, CurveMetaData);
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
            PairedData expandedPairedData = new(ExceedanceToNonExceedance(CombinedExceedanceProbabilities), expandedStageOrLogFlowValues, CurveMetaData);
            return expandedPairedData;
        }

        private static bool IsMonotonicallyIncreasing(IPairedData pairedData)
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

        private static double[] CombineInputAndRequiredExceedanceProbabilities(double[] inputExceedanceProbabilities)
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
            XElement masterElement = new("Graphical_Uncertain_Paired_Data");

            XElement curveMetaDataElement = CurveMetaData.WriteToXML();
            curveMetaDataElement.Name = "CurveMetaData";
            masterElement.Add(curveMetaDataElement);

            XElement graphicalElement = GraphicalDistributionWithLessSimple.WriteToXML();
            graphicalElement.Name = "Graphical";
            masterElement.Add(graphicalElement);

            XElement probabilities = new("Probabilities");
            foreach (double probability in CombinedExceedanceProbabilities)
            {
                XElement rowElement = new("Probability");
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

                List<double> combinedProbabilities = new();
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


