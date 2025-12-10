using System;
using System.Collections.Generic;
using System.Linq;
using Statistics.Distributions;
using Statistics;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Implementations;
using System.Xml.Linq;
using HEC.FDA.Model.utilities;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Model.Messaging;
using HEC.FDA.Model.paireddata;

namespace HEC.FDA.Model.extensions
{
    [StoredProperty("GraphicalDistribution")]
    public class GraphicalDistribution : ValidationErrorLogger
    {
        #region Properties
        [StoredProperty("LowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant")]
        public double LowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant { get; }
        [StoredProperty("HigherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant")]
        public double HigherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant { get; }
        [StoredProperty("StageOrLoggedFlowValues")]
        public double[] StageOrLoggedFlowValues { get; internal set; }
        [StoredProperty("UsingStagesNotFlows")]
        public bool UsingStagesNotFlows { get; }
        [StoredProperty("EquivalentRecordLength")]
        public int EquivalentRecordLength { get; }
        [StoredProperty("ExceedanceProbabilities")]
        public double[] ExceedanceProbabilities { get; internal set; }

        [StoredProperty("StageOrLogFlowDistributions")]
        public ContinuousDistribution[] StageOrLogFlowDistributions { get; internal set; }

        #endregion

        #region Constructor 
        public GraphicalDistribution()
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
        /// Graphical Distribution implements Beth Faber's Less Simple Method for calculating uncertainty about the distribution
        /// See the HEC-FDA Technical Reference for more information on the Less Simple Method
        /// This constructor assumes that exceedance probabilities and flow or stage values have a strictly monotonic relationships.
        /// </summary>
        /// <param name="userInputExceedanceProbabilities"></param> User-provided exceedance probabilities. There should be at least 8.
        /// <param name="stageOrUnloggedFlowValues"></param> User-provided flow or stage values. A value should correspond to a probability. 
        /// <param name="equivalentRecordLength"></param> The equivalent record length in years.

        public GraphicalDistribution(double[] userInputExceedanceProbabilities, double[] stageOrUnloggedFlowValues, int equivalentRecordLength, bool usingStagesNotFlows = true, double higherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant = 0.99, double lowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant = 0.01)
        {
            EquivalentRecordLength = equivalentRecordLength;
            UsingStagesNotFlows = usingStagesNotFlows;
            PairedData extrapolatedFrequencyFunctionWithStagesOrLoggedFlows;
            if (usingStagesNotFlows)
            {
                extrapolatedFrequencyFunctionWithStagesOrLoggedFlows = ExtrapolateFrequencyFunction(userInputExceedanceProbabilities, stageOrUnloggedFlowValues);
            }
            else
            {
                extrapolatedFrequencyFunctionWithStagesOrLoggedFlows = ExtrapolateFrequencyFunction(userInputExceedanceProbabilities, LogFlows(stageOrUnloggedFlowValues));
            }
            ExceedanceProbabilities = FillInputExceedanceProbabilitiesWithRequiredPoints(extrapolatedFrequencyFunctionWithStagesOrLoggedFlows.Xvals);
            StageOrLoggedFlowValues = InterpolateQuantiles.InterpolateOnX(extrapolatedFrequencyFunctionWithStagesOrLoggedFlows.Xvals, ExceedanceProbabilities, extrapolatedFrequencyFunctionWithStagesOrLoggedFlows.Yvals);
            LowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant = lowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant;
            HigherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant = higherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant;
            AddRules(userInputExceedanceProbabilities);
            Validate();
            if (ErrorLevel >= ErrorLevel.Major)
            {
                string message = $"There are major or worse errors associated with a graphical frequency function, confidence intervals cannot be computed." + Environment.NewLine;
                ErrorMessage errorMessage = new(message, ErrorLevel.Major);
                ReportMessage(this, new MessageEventArgs(errorMessage));
            }
            else
            {
                //then we compute uncertainty 
                StageOrLogFlowDistributions = ConstructContinuousDistributions();
            }
        }
        private GraphicalDistribution(double lowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant, double higherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant, double[] stageOrLoggedFlowValues, bool usingStagesNotFlows, int equivalentRecordLength, double[] exceedanceProbabilities, ContinuousDistribution[] stageOrLogFlowDistributions)
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
        private void AddRules(double[] exceedanceProbabilities)
        {
            AddSinglePropertyRule(nameof(EquivalentRecordLength), new Rule(() => EquivalentRecordLength > 0, "Equivalent record length must be greater than 0."));
            AddSinglePropertyRule(nameof(exceedanceProbabilities), new Rule(() => IsArrayValid(exceedanceProbabilities, (a, b) => (a >= b)), "Exceedance Probabilities must be strictly monotonically decreasing"));
            AddSinglePropertyRule(nameof(StageOrLoggedFlowValues), new Rule(() => IsArrayValid(StageOrLoggedFlowValues, (a, b) => (a <= b)), "Y must be strictly monotonically decreasing"));
        }
        private static bool IsArrayValid(double[] arrayOfData, Func<double, double, bool> comparison)
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
        private static double[] FillInputExceedanceProbabilitiesWithRequiredPoints(double[] inputExceedanceProbabilities)
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
        private static double[] LogFlows(double[] unloggedFlows)
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

        //This method adds a minimum and maximum coordinate to the frequency function, extrapolating beyond what the user provided 
        public PairedData ExtrapolateFrequencyFunction(double[] exceedanceProbabilities, double[] userProvidedStageOrLoggedFlowValues)
        {
            double toleratedDifference = 0.0001;
            double maximumExceedanceProbability = 0.9999;
            double minimumExceedanceProbability = 0.0001;

            List<double> ExtrapolatedFlowOrStageValues = new();
            List<double> ExtrapolatedExceedanceProbabilities = new();
            for (int i = 0; i < exceedanceProbabilities.Length; i++)
            {
                ExtrapolatedFlowOrStageValues.Add(userProvidedStageOrLoggedFlowValues[i]);
                ExtrapolatedExceedanceProbabilities.Add(exceedanceProbabilities[i]);
            }

            //more frequent of the frequency curve
            if (maximumExceedanceProbability - ExtrapolatedExceedanceProbabilities.First() > toleratedDifference)
            { //if the maximum exceedance probability is sufficiently larger than the largest exceedance probabiltiy 


                // let x1 be the lowest value in xvals 
                double smallestInputFlowOrStage = ExtrapolatedFlowOrStageValues[0];

                //insert the maximum probability into the first location 
                ExtrapolatedExceedanceProbabilities.Insert(0, maximumExceedanceProbability);

                if (smallestInputFlowOrStage < 0) { ExtrapolatedFlowOrStageValues.Insert(0, 1.001 * smallestInputFlowOrStage); } //if the first value is negative then make it slightly more negative

                if (smallestInputFlowOrStage > 0)
                {
                    ExtrapolatedFlowOrStageValues.Insert(0, .999 * smallestInputFlowOrStage);
                } //insert a slightly smaller value 

                else if (smallestInputFlowOrStage < -1.0e-4)
                {
                    ExtrapolatedFlowOrStageValues[0] = 1.001 * smallestInputFlowOrStage;//why are we doing it a second time?
                }
                else
                {
                    ExtrapolatedFlowOrStageValues.Insert(0, -1.0e-4);//so if xl is really close to zero, set the value equal to -1e-4?
                }
            }
            //less frequent end of the frequency curve
            if (ExtrapolatedExceedanceProbabilities.Last() - minimumExceedanceProbability > toleratedDifference)
            {
                Normal standardNormalDistribution = new();
                double penultimateInputExceedanceProbability = ExtrapolatedExceedanceProbabilities[^2];
                double lastInputExceedanceProbability = ExtrapolatedExceedanceProbabilities.Last();
                double zValueOfMin = standardNormalDistribution.InverseCDF(minimumExceedanceProbability);
                double zValueOfPenultimateInputProbability = standardNormalDistribution.InverseCDF(penultimateInputExceedanceProbability);
                double zValueOfLastInputProbability = standardNormalDistribution.InverseCDF(lastInputExceedanceProbability);
                double penultimateInputFlowOrStage = ExtrapolatedFlowOrStageValues[^2];
                double lastInputFlowOrStage = ExtrapolatedFlowOrStageValues.Last();
                double c = (zValueOfLastInputProbability - zValueOfPenultimateInputProbability) / (zValueOfMin - zValueOfPenultimateInputProbability); //TODO: figure out what c represents and give it a good name
                double upperFlowOrStage = ((lastInputFlowOrStage - penultimateInputFlowOrStage) + c * penultimateInputFlowOrStage) / c;
                ExtrapolatedFlowOrStageValues.Add(upperFlowOrStage);
                ExtrapolatedExceedanceProbabilities.Add(minimumExceedanceProbability);
            }
            PairedData extrapolatedFunction = new PairedData(ExtrapolatedExceedanceProbabilities.ToArray(), ExtrapolatedFlowOrStageValues.ToArray());
            return extrapolatedFunction;
        }

        //TODO: This method can be refactored for clarity.
        /// <summary>
        /// This method implements Beth Faber's Less Simple Method for quantifying uncertainty about a graphical frequency relationship
        /// </summary>
        /// <param name="lowerExceedanceProbabilityHoldStandardErrorConstant">The exceedance probability threshold below which standard error should be held constant (typically 0.5 for high probabilities)</param>
        /// <param name="higherExceedanceProbabilityHoldStandardErrorConstant">The exceedance probability threshold above which standard error should be held constant (typically 0.002 for rare events)</param>
        /// <returns>Array of standard deviations (standard errors) for each exceedance probability point</returns>
        private double[] ComputeStandardDeviations(double lowerExceedanceProbabilityHoldStandardErrorConstant, double higherExceedanceProbabilityHoldStandardErrorConstant)
        {
            return ComputeStandardDeviationsRefactored(lowerExceedanceProbabilityHoldStandardErrorConstant, higherExceedanceProbabilityHoldStandardErrorConstant);
        }

        /// <summary>
        /// REFACTORED VERSION: Implements Beth Faber's Less Simple Method for quantifying uncertainty about a graphical frequency relationship.
        /// This version has improved clarity through better naming, extracted helper methods, and clearer structure.
        /// Mathematically equivalent to the original ComputeStandardDeviations method.
        /// </summary>
        /// <param name="frequentEventThreshold">Exceedance probability for frequent events (e.g., 0.5 = 2-year event)</param>
        /// <param name="rareEventThreshold">Exceedance probability for rare events (e.g., 0.002 = 500-year event)</param>
        /// <returns>Array of standard errors for each exceedance probability point</returns>
        private double[] ComputeStandardDeviationsRefactored(double frequentEventThreshold, double rareEventThreshold)
        {
            int numPoints = ExceedanceProbabilities.Length;
            double[] standardErrors = new double[numPoints];

            // Step 1: Find transition indices where we stop computing and start holding constant
            int frequentEventIndex = FindClosestProbabilityIndex(frequentEventThreshold);
            int rareEventIndex = FindClosestProbabilityIndex(rareEventThreshold);

            // Step 2: Compute standard errors for interior points (points that have neighbors on both sides)
            ComputeInteriorStandardErrors(standardErrors);

            // Step 3: Compute standard errors for boundary points (first and last)
            ComputeBoundaryStandardErrors(standardErrors);

            // Step 4: Apply constant standard errors at distribution tails
            ApplyConstantStandardErrorsAtTails(standardErrors, frequentEventIndex, rareEventIndex);

            return standardErrors;
        }

        /// <summary>
        /// Finds the index of the exceedance probability closest to the target threshold.
        /// </summary>
        private int FindClosestProbabilityIndex(double targetProbability)
        {
            int closestIndex = -1;
            double minDistance = double.MaxValue;

            for (int i = 0; i < ExceedanceProbabilities.Length; i++)
            {
                double distance = Math.Abs(ExceedanceProbabilities[i] - targetProbability);
                if (distance < minDistance)
                {
                    closestIndex = i;
                    minDistance = distance;
                }
            }

            return closestIndex;
        }

        /// <summary>
        /// Computes standard errors for all interior points (excludes first and last points).
        /// Interior points have neighbors on both sides, allowing slope calculation.
        /// </summary>
        private void ComputeInteriorStandardErrors(double[] standardErrors)
        {
            // Process points from index 1 to Length-2 (skipping first and last)
            for (int i = 1; i < ExceedanceProbabilities.Length - 1; i++)
            {
                double nonExceedanceProbability = 1.0 - ExceedanceProbabilities[i];
                double slope = ComputeSlope(ExceedanceProbabilities, StageOrLoggedFlowValues, i);
                standardErrors[i] = Equation6StandardError(nonExceedanceProbability, slope, EquivalentRecordLength);
            }
        }

        /// <summary>
        /// Computes standard errors for boundary points (first and last) using adjacent slopes.
        /// Since we can't compute slope at boundaries, we borrow the slope from the nearest interior point.
        /// </summary>
        private void ComputeBoundaryStandardErrors(double[] standardErrors)
        {
            int lastIndex = ExceedanceProbabilities.Length - 1;

            // First point: use slope from second point (index 1)
            if (ExceedanceProbabilities.Length > 1)
            {
                double nonExceedanceProbFirst = 1.0 - ExceedanceProbabilities[0];
                double slopeAtSecondPoint = ComputeSlope(ExceedanceProbabilities, StageOrLoggedFlowValues, 1);
                standardErrors[0] = Equation6StandardError(nonExceedanceProbFirst, slopeAtSecondPoint, EquivalentRecordLength);
            }

            // Last point: use slope from second-to-last point (index Length-2)
            if (ExceedanceProbabilities.Length > 2)
            {
                double nonExceedanceProbLast = 1.0 - ExceedanceProbabilities[lastIndex];
                double slopeAtSecondToLast = ComputeSlope(ExceedanceProbabilities, StageOrLoggedFlowValues, lastIndex - 1);
                standardErrors[lastIndex] = Equation6StandardError(nonExceedanceProbLast, slopeAtSecondToLast, EquivalentRecordLength);
            }
        }

        /// <summary>
        /// Applies constant standard errors at the tails of the distribution to prevent unrealistic uncertainty.
        /// - Frequent events (low exceedance prob): hold constant from frequentEventIndex to end
        /// - Rare events (high exceedance prob): hold constant from start to rareEventIndex
        /// </summary>
        private void ApplyConstantStandardErrorsAtTails(double[] standardErrors, int frequentEventIndex, int rareEventIndex)
        {
            // Hold constant for frequent events (towards the right of the array - lower exceedance probabilities)
            double frequentEventStandardError = standardErrors[frequentEventIndex];
            for (int i = frequentEventIndex; i < standardErrors.Length; i++)
            {
                standardErrors[i] = frequentEventStandardError;
            }

            // Hold constant for rare events (towards the left of the array - higher exceedance probabilities)
            double rareEventStandardError = standardErrors[rareEventIndex];
            for (int i = 0; i < rareEventIndex; i++)
            {
                standardErrors[i] = rareEventStandardError;
            }
        }

        #region Original Method (Preserved for Reference)
        /// <summary>
        /// ORIGINAL VERSION: This method implements Beth Faber's Less Simple Method for quantifying uncertainty about a graphical frequency relationship
        /// Preserved for verification and comparison with refactored version.
        /// </summary>
        /// <param name="lowerExceedanceProbabilityHoldStandardErrorConstant">The exceedance probability threshold below which standard error should be held constant (typically 0.5 for high probabilities)</param>
        /// <param name="higherExceedanceProbabilityHoldStandardErrorConstant">The exceedance probability threshold above which standard error should be held constant (typically 0.002 for rare events)</param>
        /// <returns>Array of standard deviations (standard errors) for each exceedance probability point</returns>
        private double[] ComputeStandardDeviationsOriginal(double lowerExceedanceProbabilityHoldStandardErrorConstant, double higherExceedanceProbabilityHoldStandardErrorConstant)
        {
            int ixSlopeHiConst = -1;
            int ixSlopeLoConst = -1;

            // PHASE 1: Find the indices closest to our "hold constant" thresholds
            // We're looking for the actual data points nearest to our specified probability thresholds

            // Initialize with very large values so any real difference will be smaller
            double maxDiffHi = 1.0e30;
            double maxDiffLo = 1.0e30;
            double diffHi = 0;
            double diffLo = 0;
            double p;

            // Search through all exceedance probabilities to find closest matches to our thresholds
            for (int i = 0; i < ExceedanceProbabilities.Length; i++)
            {
                p = ExceedanceProbabilities[i];

                // Calculate distance from current probability to each threshold
                diffHi = Math.Abs(p - lowerExceedanceProbabilityHoldStandardErrorConstant);
                diffLo = Math.Abs(p - higherExceedanceProbabilityHoldStandardErrorConstant);

                // Track the index with minimum distance to lowerExceedanceProbabilityHoldStandardErrorConstant (e.g., 0.5)
                if (diffHi < maxDiffHi)
                {
                    ixSlopeHiConst = i;
                    maxDiffHi = diffHi;
                }

                // Track the index with minimum distance to higherExceedanceProbabilityHoldStandardErrorConstant (e.g., 0.002)
                if (diffLo < maxDiffLo)
                {
                    ixSlopeLoConst = i;
                    maxDiffLo = diffLo;
                }
            }

            // PHASE 2: Compute standard errors for each probability point
            // _scurve will hold the standard error (standard deviation) for each exceedance probability
            double[] _scurve = new double[ExceedanceProbabilities.Length];

            // Loop through interior points (not first or last) to calculate standard errors
            // We skip i=0 and i=Last because computing slope requires points on both sides
            for (int i = 1; i < ExceedanceProbabilities.Length - 1; i++)
            {
                // Convert exceedance probability to non-exceedance probability
                // Exceedance P(X > x) = 1 - Non-exceedance P(X <= x)
                p = 1 - ExceedanceProbabilities[i];

                // Calculate the local slope of the frequency curve at this point
                // Slope represents how quickly stage/flow changes with probability
                double slope = ComputeSlope(ExceedanceProbabilities, StageOrLoggedFlowValues, i);

                // Apply Equation 6 from HEC-FDA Technical Reference (CPD-72a)
                // This calculates standard error based on probability, slope, and equivalent record length
                _scurve[i] = Equation6StandardError(p, slope, EquivalentRecordLength);

                // Special handling for the FIRST point (i=0)
                // We can't compute slope at i=0 directly, so we use the slope from i=1
                if (i == 1)
                {
                    p = 1 - ExceedanceProbabilities[i - 1];  // Non-exceedance prob for first point
                    // Use the same slope as i=1 for consistency
                    _scurve[i - 1] = Equation6StandardError(p, slope, EquivalentRecordLength);
                }

                // Special handling for the LAST point (i=Length-1)
                // We can't compute slope at the last index directly, so we use the slope from i=Length-2
                if (i == ExceedanceProbabilities.Length - 2)
                {
                    p = 1 - ExceedanceProbabilities[i + 1];  // Non-exceedance prob for last point
                    // Manually compute standard error using Equation 6 formula
                    double standardErrorSquared = (p * (1 - p)) / (Math.Pow(1 / slope, 2.0D) * EquivalentRecordLength);
                    _scurve[i + 1] = Math.Sqrt(standardErrorSquared);
                }
            }
            // PHASE 3: Hold standard error constant at the tails of the distribution
            // This prevents unrealistic uncertainty estimates at extreme probabilities

            // For LOWER exceedance probabilities (more frequent events, towards 0.5):
            // Hold the standard error constant from ixSlopeHiConst onwards
            // Example: if ixSlopeHiConst is at p=0.5, all probabilities below 0.5 get the same standard error
            for (int i = ixSlopeHiConst; i < ExceedanceProbabilities.Length; i++)
            {
                _scurve[i] = _scurve[ixSlopeHiConst];
            }

            // For HIGHER exceedance probabilities (rare events, towards 0.002):
            // Hold the standard error constant from 0 up to ixSlopeLoConst
            // Example: if ixSlopeLoConst is at p=0.002, all probabilities above 0.002 get the same standard error
            for (int i = 0; i < ixSlopeLoConst; i++)
            {
                _scurve[i] = _scurve[ixSlopeLoConst];
            }

            // Return the complete array of standard errors (one for each exceedance probability point)
            return _scurve;
        }
        #endregion

        public static double ComputeSlope(double[] exceedanceProbabilities, double[] stageOrLoggedFlowValues, int i)
        {
            //step 1: identify the non-exceedance probability and coinciding quantiles for which we're calculating the slope 
            double p = 1 - exceedanceProbabilities[i];
            double q = stageOrLoggedFlowValues[i];

            double p_minus = 1 - exceedanceProbabilities[i - 1];
            double q_minus = stageOrLoggedFlowValues[i - 1];

            double p_plus = 1 - exceedanceProbabilities[i + 1];
            double q_plus = stageOrLoggedFlowValues[i + 1];

            //step 2: identify probability margins that feed into the slope calculator 
            double epsilon = 0.00001;
            double p_minusEpsilon = p - epsilon;
            double p_plusEpsilon = p + epsilon;

            //step 3: interpolate the quantiles at the probability margins 
            double q_minusEpsilon = InterpolateNormally(p, p_minus, q, q_minus, p_minusEpsilon);
            double q_plusEpsilon = InterpolateNormally(p_plus, p, q_plus, q, p_plusEpsilon);

            //step 4: calculate slope between the probability margins 
            double slope = (q_plusEpsilon - q_minusEpsilon) / (p_plusEpsilon - p_minusEpsilon);
            return slope;
        }

        public static double InterpolateNormally(double p, double p_minus, double q, double q_minus, double p_minusEpsilon)
        {
            Normal standardNormal = new();


            double z = standardNormal.InverseCDF(p);
            double z_minus = standardNormal.InverseCDF(p_minus);
            double z_minusEpsilon = standardNormal.InverseCDF(p_minusEpsilon);

            double q_minusEpsilon = q_minus + (z_minusEpsilon - z_minus) / (z - z_minus) * (q - q_minus);

            return q_minusEpsilon;
        }

        private ContinuousDistribution[] ConstructContinuousDistributions()
        {
            double[] stageOrLogFlowStandardErrorsComputed = ComputeStandardDeviations(LowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant, HigherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant);

            ContinuousDistribution[] distributionArray = new ContinuousDistribution[stageOrLogFlowStandardErrorsComputed.Length];

            if (UsingStagesNotFlows)
            {
                for (int i = 0; i < stageOrLogFlowStandardErrorsComputed.Length; i++)
                {
                    distributionArray[i] = new Normal(StageOrLoggedFlowValues[i], stageOrLogFlowStandardErrorsComputed[i]);
                }
                return distributionArray;
            }
            else
            {
                for (int i = 0; i < stageOrLogFlowStandardErrorsComputed.Length; i++)
                {
                    distributionArray[i] = new LogNormal(StageOrLoggedFlowValues[i], stageOrLogFlowStandardErrorsComputed[i]);
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
        public static double Equation6StandardError(double nonExceedanceProbability, double slope, int erl)
        {
            double standardErrorSquared = (nonExceedanceProbability * (1 - nonExceedanceProbability)) / (Math.Pow(1 / slope, 2.0D) * erl);
            double standardError = Math.Pow(standardErrorSquared, 0.5);
            return standardError;
        }
        #endregion

        #region XML Methods
        public static GraphicalDistribution ReadFromXML(XElement xElement)
        {
            GraphicalDistribution graphicalInError = new();

            Type graphicalDistributionType = typeof(GraphicalDistribution);

            string lowerProbTag = Serialization.GetXMLTagFromProperty(graphicalDistributionType, nameof(LowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant));
            if (!double.TryParse(xElement.Attribute(lowerProbTag)?.Value, out double lowerProb))
                return graphicalInError;

            string upperProbTag = Serialization.GetXMLTagFromProperty(graphicalDistributionType, nameof(HigherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant));
            if (!double.TryParse(xElement.Attribute(upperProbTag)?.Value, out double upperProb))
                return graphicalInError;

            string erlTag = Serialization.GetXMLTagFromProperty(graphicalDistributionType, nameof(EquivalentRecordLength));
            if (!int.TryParse(xElement.Attribute(erlTag)?.Value, out int erl))
                return graphicalInError;

            string boolTag = Serialization.GetXMLTagFromProperty(graphicalDistributionType, nameof(UsingStagesNotFlows));
            if (!bool.TryParse(xElement.Attribute(boolTag)?.Value, out bool usesStageNotFlows))
                return graphicalInError;

            string probsTag = Serialization.GetXMLTagFromProperty(graphicalDistributionType, nameof(ExceedanceProbabilities));
            List<double> exceedanceProbabilities = new();
            int i = 0;
            foreach (XElement exceedanceProbability in xElement.Element(probsTag).Elements())
            {
                if (!double.TryParse(exceedanceProbability.Attribute(probsTag)?.Value, out double prob))
                {
                    //serves as null object to be returned in case of error
                    GraphicalDistribution graphical = new();
                    return graphical;
                }
                exceedanceProbabilities.Add(prob);
                i++;
            }

            string distsTag = Serialization.GetXMLTagFromProperty(graphicalDistributionType, nameof(StageOrLogFlowDistributions));
            List<Normal> stageOrFlowDistributions = new();
            foreach (XElement stageOrFlowDistribution in xElement.Element(distsTag).Elements())
            {
                Normal normal = (Normal)ContinuousDistribution.FromXML(stageOrFlowDistribution);
                if (normal != null)
                {
                    stageOrFlowDistributions.Add(normal);
                }
            }

            string valsTag = Serialization.GetXMLTagFromProperty(graphicalDistributionType, nameof(StageOrLoggedFlowValues));
            List<double> inputStageFlowVals = new();

            int j = 0;
            foreach (XElement stageOrFlowValue in xElement.Element(valsTag).Elements())
            {
                if (!double.TryParse(stageOrFlowValue.Attribute(valsTag)?.Value, out double val))
                {
                    //serves as null object to be returned in case of error
                    GraphicalDistribution graphical = new();
                    return graphical;
                }
                inputStageFlowVals.Add(val);
                j++;
            }

            return new GraphicalDistribution(lowerProb, upperProb, inputStageFlowVals.ToArray(), usesStageNotFlows, erl, exceedanceProbabilities.ToArray(), stageOrFlowDistributions.ToArray()); ;
        }
        public XElement WriteToXML()
        {
            XElement masterElement = new(GetType().Name);

            string lowerProbTag = Serialization.GetXMLTagFromProperty(GetType(), nameof(LowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant));
            masterElement.SetAttributeValue(lowerProbTag, LowerExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant);

            string upperProbTag = Serialization.GetXMLTagFromProperty(GetType(), nameof(HigherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant));
            masterElement.SetAttributeValue(upperProbTag, HigherExceedanceProbabilityBeyondWhichToHoldStandardErrorConstant);

            string boolTag = Serialization.GetXMLTagFromProperty(GetType(), nameof(UsingStagesNotFlows));
            masterElement.SetAttributeValue(boolTag, UsingStagesNotFlows);

            string erlTag = Serialization.GetXMLTagFromProperty(GetType(), nameof(EquivalentRecordLength));
            masterElement.SetAttributeValue(erlTag, EquivalentRecordLength);

            string probsTag = Serialization.GetXMLTagFromProperty(GetType(), nameof(ExceedanceProbabilities));
            XElement exceedanceProbabilities = new(probsTag);
            for (int i = 0; i < ExceedanceProbabilities.Length; i++)
            {
                //the name of rowElement does not matter, only the name of the attribute does 
                XElement rowElement = new("Probability");
                rowElement.SetAttributeValue(probsTag, ExceedanceProbabilities[i]);
                exceedanceProbabilities.Add(rowElement);
            }
            masterElement.Add(exceedanceProbabilities);

            string valsTag = Serialization.GetXMLTagFromProperty(GetType(), nameof(StageOrLoggedFlowValues));
            XElement inputStageFlowValues = new(valsTag);
            for (int i = 0; i < StageOrLoggedFlowValues.Length; i++)
            {
                XElement rowElement = new("StageFlow");
                rowElement.SetAttributeValue(valsTag, StageOrLoggedFlowValues[i]);
                inputStageFlowValues.Add(rowElement);
            }
            masterElement.Add(inputStageFlowValues);

            string distsTag = Serialization.GetXMLTagFromProperty(GetType(), nameof(StageOrLogFlowDistributions));
            XElement stageOrLogFlowDistributions = new(distsTag);
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
