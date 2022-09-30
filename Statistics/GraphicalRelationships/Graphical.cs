﻿using Statistics.Graphical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Statistics.Distributions;
using Utilities;
using HEC.MVVMFramework.Base.Implementations;

namespace Statistics.GraphicalRelationships
{
    public class Graphical: HEC.MVVMFramework.Base.Implementations.Validation
    {
        #region Fields
        private double _MaximumExceedanceProbability; //the maximum exceedance probability possible in the frequency curve
        private double _MinimumExceedanceProbability; //the minimum exceedanace probability possible in the frequency curve
        private double _Tolerance = 0.0001;
        private int _SampleSize;
        private double[] _ExpandedStageOrLogFlowValues;
        private double[] _StageOrLogFlowStandardErrorsComputed;
        private double[] _FinalExceedanceProbabilities;
        private bool _UsingStagesNotFlows;
        /// <summary>
        /// _InputExceedanceProbabilities represents the 8 or so exceedance probabilities passed into the constructor 
        /// </summary>
        private double[] _InputExceedanceProbabilities; 
        /// <summary>
        /// _InputFlowOrStageValues represent the 8 or so flow or stage values passed into the constructor 
        /// </summary>
        private double[] _InputStageOrUnLoggedFlowValues;
        /// <summary>
        /// _ExceedanceProbabilities represent the interpolated and extrapolated set of exceedance probabilities. These are the compute points in HEC-FDA Version 1.4.3
        /// </summary>
        private double[] _RequiredExceedanceProbabilities = { 0.99900, 0.99000, 0.95000, 0.90000, 0.85000, 0.80000, 0.75000, 0.70000, 0.65000, 0.60000, 0.55000, 0.50000, 0.47500, 0.45000, 0.42500, 0.40000, 0.37500, 0.35000, 0.32500, 0.30000, 0.29000, 0.28000, 0.27000, 0.26000, 0.25000, 0.24000, 0.23000, 0.22000, 0.21000, 0.20000, 0.19500, 0.19000, 0.18500, 0.18000, 0.17500, 0.17000, 0.16500, 0.16000, 0.15500, 0.15000, 0.14500, 0.14000, 0.13500, 0.13000, 0.12500, 0.12000, 0.11500, 0.11000, 0.10500, 0.10000, 0.09500, 0.09000, 0.08500, 0.08000, 0.07500, 0.07000, 0.06500, 0.06000, 0.05900, 0.05800, 0.05700, 0.05600, 0.05500, 0.05400, 0.05300, 0.05200, 0.05100, 0.05000, 0.04900, 0.04800, 0.04700, 0.04600, 0.04500, 0.04400, 0.04300, 0.04200, 0.04100, 0.04000, 0.03900, 0.03800, 0.03700, 0.03600, 0.03500, 0.03400, 0.03300, 0.03200, 0.03100, 0.03000, 0.02900, 0.02800, 0.02700, 0.02600, 0.02500, 0.02400, 0.02300, 0.02200, 0.02100, 0.02000, 0.01950, 0.01900, 0.01850, 0.01800, 0.01750, 0.01700, 0.01650, 0.01600, 0.01550, 0.01500, 0.01450, 0.01400, 0.01350, 0.01300, 0.01250, 0.01200, 0.01150, 0.01100, 0.01050, 0.01000, 0.00950, 0.00900, 0.00850, 0.00800, 0.00750, 0.00700, 0.00650, 0.00600, 0.00550, 0.00500, 0.00490, 0.00450, 0.00400, 0.00350, 0.00300, 0.00250, 0.00200, 0.00195, 0.00190, 0.00185, 0.00180, 0.00175, 0.00170, 0.00165, 0.00160, 0.00155, 0.00150, 0.00145, 0.00140, 0.00135, 0.00130, 0.00125, 0.00120, 0.00115, 0.00110, 0.00105, 0.00100, 0.00095, 0.00090, 0.00085, 0.00080, 0.00075, 0.00070, 0.00065, 0.00060, 0.00055, 0.00050, 0.00045, 0.00040, 0.00035, 0.00030, 0.00025, 0.00020, 0.00015, 0.00010 };
        /// <summary>
        /// _FlowOrStageDistributions represet the set of normal distributions with mean and standard deviation computed using the less simple method
        /// </summary>
        private ContinuousDistribution[] _StageOrLogFlowDistributions;
        #endregion

        #region Properties
        [Stored(Name ="Using Stages Not Flows", type = typeof(bool))]
        public bool UsingStagesNotFlows
        {
            get
            {
                return _UsingStagesNotFlows;
            }
        }

        [Stored(Name = "Sample Size", type = typeof(int))]
        public int SampleSize
        {
            get
            {
                return _SampleSize;
            }
            set
            {
                _SampleSize = value;
            }
        }
        [Stored(Name = "ExceedanceProbabilities", type = typeof(double[]))]
        public double[] ExceedanceProbabilities
        {
            get
            {
                return _FinalExceedanceProbabilities;
            }
            set
            {
                _FinalExceedanceProbabilities = value;
            }
        }
        [Stored(Name = "FlowOrStageDistributions", type = typeof(IDistribution[]))]
        public ContinuousDistribution[] StageOrLogFlowDistributions
        {
            get
            {
                return _StageOrLogFlowDistributions;
            }
            set
            {
                _StageOrLogFlowDistributions = value;
            }
        }
        //TODO: Add validation and set these properties 
        public IMessageLevels State { get; private set; }

        public IEnumerable<IMessage> Messages { get; private set; }
        #endregion

        #region Constructor 
        /// <summary>
        /// Steps to get a complete graphical relationship: 1. Construct Graphical, 2. Compute Confidence Limits, 3. Access Exceedance Probability and FlowOrStageDistribution Public Properties.
        /// ExceedanceProbabilities array and FlowOrStageDistribution IDistribution array can then be used to construct an uncertain paired data object. 
        /// This constructor assumes that exceedance probabilities and flow or stage values have a strictly monotonic relationships.
        /// TODO: Add validation
        /// </summary>
        /// <param name="exceedanceProbabilities"></param> User-provided exceedance probabilities. There should be at least 8.
        /// <param name="stageOrUnloggedFlowValues"></param> User-provided flow or stage values. A value should correspond to a probability. 
        /// <param name="equivalentRecordLength"></param> The equivalent record length in years.
        /// <param name="maximumProbability"></param> The maximum exceedance probability used in the frequency relationship.
        /// <param name="minimumProbability"></param> The minimum exceedance probability used in the frequency relationship. 
      
        public Graphical(double[] exceedanceProbabilities, double[] stageOrUnloggedFlowValues, int equivalentRecordLength, bool usingStagesNotFlows = true, double maximumProbability = 0.9999, double minimumProbability = 0.0001)
        {//TODO: Validate that ERL > 0
            _SampleSize = equivalentRecordLength;
            _InputExceedanceProbabilities = exceedanceProbabilities;
            _MaximumExceedanceProbability = maximumProbability;
            _MinimumExceedanceProbability = minimumProbability;
            _UsingStagesNotFlows = usingStagesNotFlows;
            if (_UsingStagesNotFlows)
            {
                _InputStageOrUnLoggedFlowValues = stageOrUnloggedFlowValues;
            } else
            {
                _InputStageOrUnLoggedFlowValues = LogFlows(stageOrUnloggedFlowValues);
            }
            AddRules();
        }
        #endregion

        #region Methods
        private void AddRules()
        {
                AddSinglePropertyRule(nameof(_InputExceedanceProbabilities), new Rule(() => IsArrayValid(_InputExceedanceProbabilities, (a, b) => (a >= b)), "Exceedance Probabilities must be strictly monotonically decreasing"));
                AddSinglePropertyRule(nameof(_InputStageOrUnLoggedFlowValues), new Rule(() => IsArrayValid(_InputStageOrUnLoggedFlowValues, (a, b) => (a <= b)), "Y must be strictly monotonically decreasing"));
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
        /// <summary>
        /// This method implements the less simple method to compute confidence limits about a graphical frequency relationship. 
        /// </summary>
        /// <param name="useConstantStandardError"></param> True if user wishes to use constant standard error. 
        /// <param name="lowerExceedanceProbabilityHoldStandardErrorConstant"></param> Less frequent end of frequency curve after which to hold standard error constant. Default is 0.99.
        /// <param name="higherExceedanceProbabilityHoldStandardErrorConstant"></param> Infrequent end of frequency curve after which to hold standard error constant. Default is 0.01.
        public void ComputeGraphicalConfidenceLimits(bool useConstantStandardError = true, double lowerExceedanceProbabilityHoldStandardErrorConstant = 0.01, double higherExceedanceProbabilityHoldStandardErrorConstant = 0.99)
        {   
            ExtendFrequencyCurveBasedOnNormalProbabilityPaper();
            List<double> finalProbabilities = GetFinalProbabilities();
            InterpolateQuantiles interpolatedQuantiles = new InterpolateQuantiles(_InputStageOrUnLoggedFlowValues, _InputExceedanceProbabilities);
            _ExpandedStageOrLogFlowValues = interpolatedQuantiles.ComputeQuantiles(finalProbabilities.ToArray());
            _FinalExceedanceProbabilities = finalProbabilities.ToArray();
            _StageOrLogFlowStandardErrorsComputed = ComputeStandardDeviations(useConstantStandardError, lowerExceedanceProbabilityHoldStandardErrorConstant, higherExceedanceProbabilityHoldStandardErrorConstant);
            _StageOrLogFlowDistributions = ConstructContinuousDistributions();
        }

        public void ExtendFrequencyCurveBasedOnNormalProbabilityPaper() //I think we need a better name. 
        {
            List<double> listOfInputFlowOrStageValues = new List<double>();
            List<double> listOfInputExceedanceProbabilities = new List<double>();
            for (int i = 0; i < _InputExceedanceProbabilities.Count(); i++)
            {
                listOfInputFlowOrStageValues.Add(_InputStageOrUnLoggedFlowValues[i]);
                listOfInputExceedanceProbabilities.Add(_InputExceedanceProbabilities[i]);
            }

            //more frequent of the frequency curve
            if (_MaximumExceedanceProbability - listOfInputExceedanceProbabilities.First() > _Tolerance)
            { //if the maximum exceedance probability is sufficiently larger than the largest exceedance probabiltiy 


                // let x1 be the lowest value in xvals 
                double smallestInputFlowOrStage = listOfInputFlowOrStageValues[0];

                //insert the maximum probability into the first location 
                listOfInputExceedanceProbabilities.Insert(0, _MaximumExceedanceProbability);
               
                if (smallestInputFlowOrStage < 0) { listOfInputFlowOrStageValues.Insert(0, 1.001 * smallestInputFlowOrStage); } //if the first value is negative then make it slightly more negative

                if (smallestInputFlowOrStage > 0)
                {
                    listOfInputFlowOrStageValues.Insert(0, .999 * smallestInputFlowOrStage);
                } //insert a slightly smaller value 

                else if (smallestInputFlowOrStage < -1.0e-4)
                {
                    listOfInputFlowOrStageValues[0] = 1.001 * smallestInputFlowOrStage;//why are we doing it a second time?
                }                   
                else
                {
                    listOfInputFlowOrStageValues.Insert(0, -1.0e-4);//so if xl is really close to zero, set the value equal to -1e-4?
                } 
            }
            //less frequent end of the frequency curve
            if (listOfInputExceedanceProbabilities.Last() - _MinimumExceedanceProbability > _Tolerance)
            {
                Distributions.Normal standardNormalDistribution = new Distributions.Normal();
                double penultimateInputExceedanceProbability = listOfInputExceedanceProbabilities[listOfInputExceedanceProbabilities.Count - 2];
                double lastInputExceedanceProbability = listOfInputExceedanceProbabilities.Last();
                double zValueOfMin = standardNormalDistribution.InverseCDF(_MinimumExceedanceProbability);
                double zValueOfPenultimateInputProbability = standardNormalDistribution.InverseCDF(penultimateInputExceedanceProbability);
                double zValueOfLastInputProbability = standardNormalDistribution.InverseCDF(lastInputExceedanceProbability);
                double penultimateInputFlowOrStage = listOfInputFlowOrStageValues[listOfInputFlowOrStageValues.Count - 2];
                double lastInputFlowOrStage = listOfInputFlowOrStageValues.Last();
                double c = (zValueOfLastInputProbability - zValueOfPenultimateInputProbability) / (zValueOfMin - zValueOfPenultimateInputProbability); //TODO: figure out what c represents and give it a good name
                double upperFlowOrStage = ((lastInputFlowOrStage - penultimateInputFlowOrStage) + c * penultimateInputFlowOrStage) / c;
                listOfInputFlowOrStageValues.Add(upperFlowOrStage);
                listOfInputExceedanceProbabilities.Add(_MinimumExceedanceProbability);
            }
            _InputStageOrUnLoggedFlowValues = listOfInputFlowOrStageValues.ToArray();
            _InputExceedanceProbabilities = listOfInputExceedanceProbabilities.ToArray();
        }

        private List<double> GetFinalProbabilities()
        {
            //TODO: I think this code gets the final probabilities used in the graphical frequency relationship? 
            //_take pfreq and standard probablities and iclude them. EVSET

            List<double> finalProbabilities = new List<double>();
            int totalCount = _InputExceedanceProbabilities.Length + _RequiredExceedanceProbabilities.Length;
            int required = 0;
            int provided = 0;
            for (int i = 0; i < totalCount; i++)
            {
                if (required >= _RequiredExceedanceProbabilities.Length)
                {
                    if (_RequiredExceedanceProbabilities[required - 1] < _InputExceedanceProbabilities[provided])
                    {
                        provided++;
                    }
                    else
                    {
                        finalProbabilities.Add(_InputExceedanceProbabilities[provided]);
                        provided++;
                    }
                    continue;
                }
                if (provided >= _InputExceedanceProbabilities.Count())
                {
                    if (_RequiredExceedanceProbabilities[required] > _InputExceedanceProbabilities[provided - 1])
                    {
                        finalProbabilities.Add(_RequiredExceedanceProbabilities[required]);
                        required++;
                    }
                    else
                    {
                        required++;
                    }
                    continue;
                }
                if (Math.Abs(_RequiredExceedanceProbabilities[required] - _InputExceedanceProbabilities[provided]) < .000001)
                {
                    finalProbabilities.Add(_InputExceedanceProbabilities[provided]);
                    provided++;
                    required++;
                    i++;//skip one
                }
                else if (_RequiredExceedanceProbabilities[required] > _InputExceedanceProbabilities[provided])
                {
                    finalProbabilities.Add(_RequiredExceedanceProbabilities[required]);
                    required++;
                }
                else
                {
                    finalProbabilities.Add(_InputExceedanceProbabilities[provided]);
                    provided++;
                }
            }
            return finalProbabilities;
        }

        private double[] ComputeStandardDeviations(bool useConstantStandardError, double lowerExceedanceProbabilityHoldStandardErrorConstant, double higherExceedanceProbabilityHoldStandardErrorConstant)
        {
            int ixSlopeHiConst = -1;
            int ixSlopeLoConst = -1;


            //  the index at which begin to hold standard error constant 
            double maxDiffHi = 1.0e30;
            double maxDiffLo = 1.0e30;
            double diffHi = 0;
            double diffLo = 0;
            double p;
            for (int i = 0; i < _FinalExceedanceProbabilities.Count(); i++)
            {
                p = _FinalExceedanceProbabilities[i];
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
            double[] _scurve = new double[_FinalExceedanceProbabilities.Count()];
            
            for (int i = 1; i < _FinalExceedanceProbabilities.Count() - 1; i++)
            {
                p = 1 - _FinalExceedanceProbabilities[i];
                p2 = 1 - _FinalExceedanceProbabilities[i + 1];
                p1 = 1 - _FinalExceedanceProbabilities[i - 1];
                slope = (_ExpandedStageOrLogFlowValues[i + 1] - _ExpandedStageOrLogFlowValues[i - 1]) / (p2 - p1);
                _scurve[i] = Equation6StandardError(p, slope);

                //hold slope constant and calculate standard error for the first coordinate
                if (i == 1)
                { 
                    p = 1 - _FinalExceedanceProbabilities[i - 1];
                    _scurve[i - 1] = Equation6StandardError(p, slope);

                }
                //hold slope constant and calculate standard error for the last coordinate
                if (i == _FinalExceedanceProbabilities.Count() -2 )
                {
                    p = 1 - _FinalExceedanceProbabilities[i + 1];
                    standardErrorSquared = (p * (1 - p)) / (Math.Pow(1 / slope, 2.0D) * _SampleSize);
                    _scurve[i +1 ] = Math.Sqrt(standardErrorSquared);
                }

            }
            //            !Hold standard Error Constant
            if (useConstantStandardError)
            {
                for (int i = ixSlopeHiConst; i < _FinalExceedanceProbabilities.Count(); i++)
                {
                    _scurve[i] = _scurve[ixSlopeHiConst];
                }
                for (int i = 0; i < ixSlopeLoConst; i++)
                {
                    _scurve[i] = _scurve[ixSlopeLoConst];
                }
            }

            return _scurve;
        }
        private ContinuousDistribution[] ConstructContinuousDistributions()
        {
            ContinuousDistribution[] distributionArray = new ContinuousDistribution[_StageOrLogFlowStandardErrorsComputed.Length];
            
            if (_UsingStagesNotFlows)
            {
                for (int i = 0; i < _StageOrLogFlowStandardErrorsComputed.Length; i++)
                {
                    distributionArray[i] = new Distributions.Normal(_ExpandedStageOrLogFlowValues[i], _StageOrLogFlowStandardErrorsComputed[i]);
                }
                return distributionArray;
            }
            else
            {
                for (int i = 0; i < _StageOrLogFlowStandardErrorsComputed.Length; i++)
                {
                    distributionArray[i] = new Distributions.LogNormal(_ExpandedStageOrLogFlowValues[i], _StageOrLogFlowStandardErrorsComputed[i]);
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
            double standardErrorSquared = (nonExceedanceProbability * (1 - nonExceedanceProbability)) / (Math.Pow(1 / slope, 2.0D) * _SampleSize);
            double standardError = Math.Pow(standardErrorSquared, 0.5);
            return standardError;
        }
        #endregion
    }
}
