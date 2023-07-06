using System;
using System.Collections.Generic;
using System.Linq;
using Statistics.Distributions;
using Utilities;
using HEC.MVVMFramework.Base.Implementations;
using System.Xml.Linq;

namespace Statistics.GraphicalRelationships
{
    public class GraphicalDistributionWithLessSimple: Validation
    {

        #region Properties
        [Stored(Name = "LowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant", type = typeof(double))]
        private double LowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant { get; }
        [Stored(Name = "HigherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant", type = typeof(double))]
        private double HigherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant { get; }
        [Stored(Name = "StageOrLoggedFlowValues", type = typeof(double[]))]
        private double[] StageOrLoggedFlowValues { get; }
        [Stored(Name ="Using Stages Not Flows", type = typeof(bool))]
        public bool UsingStagesNotFlows { get; }
        [Stored(Name = "Equivalent Record Length", type = typeof(int))]
        public int EquivalentRecordLength { get; }
        [Stored(Name = "Exceedance Probabilities", type = typeof(double[]))]
        public double[] ExceedanceProbabilities { get; internal set; }
        
        [Stored(Name = "FlowOrStageDistributions", type = typeof(IDistribution[]))]
        public ContinuousDistribution[] StageOrLogFlowDistributions { get; internal set; }
        //TODO: Add validation and set these properties 
        public IMessageLevels State { get; private set; }

        public IEnumerable<IMessage> Messages { get; private set; }
        #endregion

        #region Constructor 
        public GraphicalDistributionWithLessSimple()
        {
            EquivalentRecordLength = 10;
            UsingStagesNotFlows = true;
            ExceedanceProbabilities = new double[] { 0 };
            StageOrLogFlowDistributions = new Normal[] { new Normal(0, 1) };
            StageOrLoggedFlowValues = new double[] { 0 };
            LowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant = 0;
            HigherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant = 0;

        }
        /// <summary>
        /// This constructor assumes that exceedance probabilities and flow or stage values have a strictly monotonic relationships.
        /// </summary>
        /// <param name="exceedanceProbabilities"></param> User-provided exceedance probabilities. There should be at least 8.
        /// <param name="stageOrUnloggedFlowValues"></param> User-provided flow or stage values. A value should correspond to a probability. 
        /// <param name="equivalentRecordLength"></param> The equivalent record length in years.
      
        public GraphicalDistributionWithLessSimple(double[] exceedanceProbabilities, double[] stageOrUnloggedFlowValues, int equivalentRecordLength, bool usingStagesNotFlows = true, double higherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant = 0.99, double lowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant = 0.01)
        {//TODO: Validate that ERL > 0
            EquivalentRecordLength = equivalentRecordLength;
            UsingStagesNotFlows = usingStagesNotFlows;
            if (UsingStagesNotFlows)
            {
                StageOrLoggedFlowValues = stageOrUnloggedFlowValues;
            } else
            {
                StageOrLoggedFlowValues = LogFlows(stageOrUnloggedFlowValues);
            }
            LowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant = lowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant;
            HigherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant = higherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant;
            AddRules(exceedanceProbabilities);
            Compute(exceedanceProbabilities);
        }
        private GraphicalDistributionWithLessSimple(double lowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant, double higherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant, double[] stageOrLoggedFlowValues, bool usingStagesNotFlows, int equivalentRecordLength, double[] exceedanceProbabilities, ContinuousDistribution[] stageOrLogFlowDistributions)
        {
            LowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant = lowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant;
            HigherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant = higherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant;
            StageOrLoggedFlowValues = stageOrLoggedFlowValues;
            UsingStagesNotFlows = usingStagesNotFlows;
            EquivalentRecordLength = equivalentRecordLength;
            ExceedanceProbabilities = exceedanceProbabilities;
            StageOrLogFlowDistributions = stageOrLogFlowDistributions;
        }

        #endregion

        #region Methods
        private void Compute(double[] exceedanceProbabilities)
        {
            Validate();
            if (ErrorLevel >= HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Major)
            {
                //message that it cannot be constructed
            }
            else
            {
                ExtendFrequencyCurveBasedOnNormalProbabilityPaper(exceedanceProbabilities);
                StageOrLogFlowDistributions = ConstructContinuousDistributions();
            }
        }
        private void AddRules(double[] exceedanceProbabilities)
        {
                AddSinglePropertyRule(nameof(exceedanceProbabilities), new Rule(() => IsArrayValid(exceedanceProbabilities, (a, b) => (a >= b)), "Exceedance Probabilities must be strictly monotonically decreasing"));
                AddSinglePropertyRule(nameof(StageOrLoggedFlowValues), new Rule(() => IsArrayValid(StageOrLoggedFlowValues, (a, b) => (a <= b)), "Y must be strictly monotonically decreasing"));
        }
        private bool IsArrayValid(double[] arrayOfData, Func<double, double, bool> comparison)
        {
            if (arrayOfData == null) return false;
            for (int i = 0; i < arrayOfData.Length - 1; i++)
            {
                if (comparison(arrayOfData[i], arrayOfData[i + 1]))
                {
                    return false;
                }
            }
            return true;
        }
        private double[] LogFlows(double[] unloggedFlows)
        {
            double[] loggedFlows = new double[unloggedFlows.Length];
            double minFlow = 0.01; //for log conversion not to fail 
            for (int i = 0; i < unloggedFlows.Length; i++)
            {
                if (unloggedFlows[i] < minFlow)
                {
                    loggedFlows[i] = Math.Log(minFlow);
                }
                else
                {
                    loggedFlows[i] = Math.Log(unloggedFlows[i]);
                }
            }
            return loggedFlows;
        }

        public void ExtendFrequencyCurveBasedOnNormalProbabilityPaper(double[] exceedanceProbabilities) //I think we need a better name. 
        {
            double toleratedDifference = 0.0001;
            double maximumExceedanceProbability = 0.9999;
            double minimumExceedanceProbability = 0.0001;

            List<double> finalFlowOrStageValues = new List<double>();
            List<double> finalExceedanceProbabilities = new List<double>();
            for (int i = 0; i < exceedanceProbabilities.Count(); i++)
            {
                finalFlowOrStageValues.Add(StageOrLoggedFlowValues[i]);
                finalExceedanceProbabilities.Add(exceedanceProbabilities[i]);
            }

            //more frequent of the frequency curve
            if (maximumExceedanceProbability - finalExceedanceProbabilities.First() > toleratedDifference)
            { //if the maximum exceedance probability is sufficiently larger than the largest exceedance probabiltiy 


                // let x1 be the lowest value in xvals 
                double smallestInputFlowOrStage = finalFlowOrStageValues[0];

                //insert the maximum probability into the first location 
                finalExceedanceProbabilities.Insert(0, maximumExceedanceProbability);
               
                if (smallestInputFlowOrStage < 0) { finalFlowOrStageValues.Insert(0, 1.001 * smallestInputFlowOrStage); } //if the first value is negative then make it slightly more negative

                if (smallestInputFlowOrStage > 0)
                {
                    finalFlowOrStageValues.Insert(0, .999 * smallestInputFlowOrStage);
                } //insert a slightly smaller value 

                else if (smallestInputFlowOrStage < -1.0e-4)
                {
                    finalFlowOrStageValues[0] = 1.001 * smallestInputFlowOrStage;//why are we doing it a second time?
                }                   
                else
                {
                    finalFlowOrStageValues.Insert(0, -1.0e-4);//so if xl is really close to zero, set the value equal to -1e-4?
                } 
            }
            //less frequent end of the frequency curve
            if (finalExceedanceProbabilities.Last() - minimumExceedanceProbability > toleratedDifference)
            {
                Distributions.Normal standardNormalDistribution = new Distributions.Normal();
                double penultimateInputExceedanceProbability = finalExceedanceProbabilities[finalExceedanceProbabilities.Count - 2];
                double lastInputExceedanceProbability = finalExceedanceProbabilities.Last();
                double zValueOfMin = standardNormalDistribution.InverseCDF(minimumExceedanceProbability);
                double zValueOfPenultimateInputProbability = standardNormalDistribution.InverseCDF(penultimateInputExceedanceProbability);
                double zValueOfLastInputProbability = standardNormalDistribution.InverseCDF(lastInputExceedanceProbability);
                double penultimateInputFlowOrStage = finalFlowOrStageValues[finalFlowOrStageValues.Count - 2];
                double lastInputFlowOrStage = finalFlowOrStageValues.Last();
                double c = (zValueOfLastInputProbability - zValueOfPenultimateInputProbability) / (zValueOfMin - zValueOfPenultimateInputProbability); //TODO: figure out what c represents and give it a good name
                double upperFlowOrStage = ((lastInputFlowOrStage - penultimateInputFlowOrStage) + c * penultimateInputFlowOrStage) / c;
                finalFlowOrStageValues.Add(upperFlowOrStage);
                finalExceedanceProbabilities.Add(minimumExceedanceProbability);
            }
            _StageOrLoggedFlowValues = finalFlowOrStageValues.ToArray();
            ExceedanceProbabilities = finalExceedanceProbabilities.ToArray();
        }


        /// <summary>
        /// This method implements Beth Faber's Less Simple Method for quantifying uncertainty about a graphical frequency relationship 
        /// </summary>
        /// <param name="lowerExceedanceProbabilityHoldStandardErrorConstant"></param>
        /// <param name="higherExceedanceProbabilityHoldStandardErrorConstant"></param>
        /// <returns></returns>
        private double[] ComputeStandardDeviations(double lowerExceedanceProbabilityHoldStandardErrorConstant, double higherExceedanceProbabilityHoldStandardErrorConstant)
        {
            int ixSlopeHiConst = -1;
            int ixSlopeLoConst = -1;


            //  the index at which begin to hold standard error constant 
            double maxDiffHi = 1.0e30;
            double maxDiffLo = 1.0e30;
            double diffHi = 0;
            double diffLo = 0;
            double p;
            for (int i = 0; i < ExceedanceProbabilities.Count(); i++)
            {
                p = ExceedanceProbabilities[i];
                diffHi = Math.Abs(p - lowerExceedanceProbabilityHoldStandardErrorConstant);
                diffLo = Math.Abs(p - higherExceedanceProbabilityHoldStandardErrorConstant);

                if (diffHi < maxDiffHi)
                {
                    ixSlopeHiConst = i;
                    maxDiffHi = diffHi;
                }
                if (diffLo < maxDiffLo)
                {
                    ixSlopeLoConst = i;
                    maxDiffLo = diffLo;
                }
            }


            double p1;
            double p2;
            double slope;
            double standardErrorSquared;
            double[] _scurve = new double[ExceedanceProbabilities.Count()];
            
            for (int i = 1; i < ExceedanceProbabilities.Count() - 1; i++)
            {
                p = 1 - ExceedanceProbabilities[i];
                p2 = 1 - ExceedanceProbabilities[i + 1];
                p1 = 1 - ExceedanceProbabilities[i - 1];
                slope = (StageOrLoggedFlowValues[i + 1] - StageOrLoggedFlowValues[i - 1]) / (p2 - p1);
                _scurve[i] = Equation6StandardError(p, slope);

                //hold slope constant and calculate standard error for the first coordinate
                if (i == 1)
                { 
                    p = 1 - ExceedanceProbabilities[i - 1];
                    _scurve[i - 1] = Equation6StandardError(p, slope);

                }
                //hold slope constant and calculate standard error for the last coordinate
                if (i == ExceedanceProbabilities.Count() -2 )
                {
                    p = 1 - ExceedanceProbabilities[i + 1];
                    standardErrorSquared = (p * (1 - p)) / (Math.Pow(1 / slope, 2.0D) * EquivalentRecordLength);
                    _scurve[i +1 ] = Math.Sqrt(standardErrorSquared);
                }

            }
            // Hold standard Error Constant
                for (int i = ixSlopeHiConst; i < ExceedanceProbabilities.Count(); i++)
                {
                    _scurve[i] = _scurve[ixSlopeHiConst];
                }
                for (int i = 0; i < ixSlopeLoConst; i++)
                {
                    _scurve[i] = _scurve[ixSlopeLoConst];
                }
            return _scurve;
        }
        private ContinuousDistribution[] ConstructContinuousDistributions()
        {
            double[] stageOrLogFlowStandardErrorsComputed = ComputeStandardDeviations(LowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant, HigherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant);

            ContinuousDistribution[] distributionArray = new ContinuousDistribution[stageOrLogFlowStandardErrorsComputed.Length];
            
            if (UsingStagesNotFlows)
            {
                for (int i = 0; i < stageOrLogFlowStandardErrorsComputed.Length; i++)
                {
                    distributionArray[i] = new Distributions.Normal(StageOrLoggedFlowValues[i], stageOrLogFlowStandardErrorsComputed[i]);
                }
                return distributionArray;
            }
            else
            {
                for (int i = 0; i < stageOrLogFlowStandardErrorsComputed.Length; i++)
                {
                    distributionArray[i] = new Distributions.LogNormal(StageOrLoggedFlowValues[i], stageOrLogFlowStandardErrorsComputed[i]);
                }
                return distributionArray;
            }

        }
        /// <summary>
        /// This is Equation 6 from CPD-72a HEC-FDA Technical Reference 
        /// </summary>
        /// <param name="nonExceedanceProbability"></param>
        /// <param name="slope"></param>
        /// <returns></returns>
        private double Equation6StandardError(double nonExceedanceProbability, double slope)
        {
            double standardErrorSquared = (nonExceedanceProbability * (1 - nonExceedanceProbability)) / (Math.Pow(1 / slope, 2.0D) * EquivalentRecordLength);
            double standardError = Math.Pow(standardErrorSquared, 0.5);
            return standardError;
        }
        #endregion

        #region XML Methods
        public static GraphicalDistributionWithLessSimple ReadFromXML(XElement xElement)
        {
            double lowerProb = Convert.ToDouble(xElement.Attribute("LowerProb").Value);
            double upperProb = Convert.ToDouble(xElement.Attribute("UpperProb").Value);
            int erl = Convert.ToInt32(xElement.Attribute("ERL").Value);
            bool usesStageNotFlows = Convert.ToBoolean(xElement.Attribute("UsesStagesNotFLows").Value);
            List<double> exceedanceProbabilities = new();
            foreach (XElement exceedanceProbability in xElement.Element("ExceedanceProbabilities").Elements())
            {
               exceedanceProbabilities.Add(Convert.ToDouble(exceedanceProbability.Attribute("Value").Value));
            }
            List<Normal> stageOrFlowDistributions = new();
            foreach (XElement stageOrFlowDistribution in xElement.Element("StageOrLogFlowDistributions").Elements())
            {
                stageOrFlowDistributions.Add((Normal)ContinuousDistribution.FromXML(stageOrFlowDistribution));
            }
            List<double> inputStageFlowVals = new();
            foreach (XElement stageOrFlowValue in xElement.Element("InputValues").Elements())
            {
                inputStageFlowVals.Add(Convert.ToDouble(stageOrFlowValue.Attribute("Value").Value));
            }
            return new GraphicalDistributionWithLessSimple(lowerProb, upperProb, inputStageFlowVals.ToArray(), usesStageNotFlows, erl, exceedanceProbabilities.ToArray(), stageOrFlowDistributions.ToArray()); ;
        }
        public XElement WriteToXML()
        {
            XElement masterElement = new XElement(this.GetType().Name);

            masterElement.SetAttributeValue("LowerProb", LowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant);
            masterElement.SetAttributeValue("UpperProb", HigherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant);
            masterElement.SetAttributeValue("UsesStagesNotFLows", UsingStagesNotFlows);
            masterElement.SetAttributeValue("ERL", EquivalentRecordLength);
            XElement exceedanceProbabilities = new XElement("ExceedanceProbabilities");
            for (int i = 0; i < ExceedanceProbabilities.Length; i++)
            {
                XElement rowElement = new XElement("Probability");
                rowElement.SetAttributeValue("Value", ExceedanceProbabilities[i]);
                exceedanceProbabilities.Add(rowElement);
            }
            masterElement.Add(exceedanceProbabilities);

            XElement inputStageFlowValues = new XElement("InputValues");
            for (int i = 0; i < StageOrLoggedFlowValues.Length; i++)
            {
                XElement rowElement = new XElement("StageFlow");
                rowElement.SetAttributeValue("Value", StageOrLoggedFlowValues[i]);
                inputStageFlowValues.Add(rowElement);
            }
            masterElement.Add(inputStageFlowValues);

            XElement stageOrLogFlowDistributions = new XElement("StageOrLogFlowDistributions");
            for (int i = 0; i < StageOrLogFlowDistributions.Length; i++)
            {
                stageOrLogFlowDistributions.Add(StageOrLogFlowDistributions[i].ToXML());
            }
            masterElement.Add(stageOrLogFlowDistributions);


            return masterElement;
        }
        #endregion
    }
}
