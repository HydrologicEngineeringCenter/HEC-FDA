using HEC.FDA.Model.extensions;
using HEC.FDA.Model.paireddata;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.Model.utilities
{
    /// <summary>
    /// Static utility class for computing uncertainty in graphical frequency relationships.
    /// Implements methods from HEC-FDA Technical Reference Manual (CPD-72a).
    /// </summary>
    public static class GraphicalFrequencyUncertaintyCalculators
    {
        /// <summary>
        /// Default lower exceedance probability threshold (e.g., 0.01 = 100-year event).
        /// Below this threshold, standard errors are held constant.
        /// </summary>
        private const double LESS_SIMPLE_FREQUENT_EVENT_THRESHOLD = 0.01;

        /// <summary>
        /// Default higher exceedance probability threshold (e.g., 0.99 = annual event).
        /// Above this threshold, standard errors are held constant.
        /// </summary>
        private const double LESS_SIMPLE_RARE_EVENT_THRESHOLD = 0.99;

        private const double LESS_SIMPLE_MAX_EXCEED_PROB = 0.9999;
        private const double LESS_SIMPLE_MIN_EXCEED_PROB = 0.0001;
        private const double MAX_INTERVAL_FOR_EXTRAPOLATION = 0.0001;

        /// <summary>
        /// Implements Beth Faber's "Less Simple Method" for quantifying uncertainty about a graphical frequency relationship.
        /// This method replicates the behavior of GraphicalDistribution exactly, including extrapolation and filling
        /// with required exceedance probabilities.
        /// </summary>
        /// <param name="exceedanceProbabilities">Array of exceedance probabilities (e.g., [0.5, 0.2, 0.1, 0.04, 0.02, 0.01, 0.002])</param>
        /// <param name="stagesOrFlows">Array of stage values (feet) or flow values (cfs) corresponding to each exceedance probability</param>
        /// <param name="usingStagesNotFlows">True if using stages, False if using flows. Determines whether to use Normal or LogNormal distributions.</param>
        /// <param name="equivalentRecordLength">The equivalent record length (in years) for the frequency relationship. Default is 10.</param>
        /// <param name="curveMetaData">Metadata for the resulting uncertain paired data. If null, creates default metadata.</param>
        /// <returns>Expanded exceedence probabilities with Normal distributions (for stages) or LogNormal distributions (for flows)</returns>
        /// <exception cref="ArgumentNullException">Thrown when exceedanceProbabilities or stagesOrFlows is null</exception>
        /// <exception cref="ArgumentException">Thrown when array lengths don't match or arrays have insufficient data points</exception>
        public static (double[],ContinuousDistribution[]) LessSimpleMethod(
            double[] exceedanceProbabilities,
            double[] stagesOrFlows,
            bool usingStagesNotFlows,
            int equivalentRecordLength = 10,
            CurveMetaData curveMetaData = null)
        {
            double frequentEventThreshold = LESS_SIMPLE_FREQUENT_EVENT_THRESHOLD;
            double rareEventThreshold = LESS_SIMPLE_RARE_EVENT_THRESHOLD;
            // Validation
            if (exceedanceProbabilities == null)
            {
                throw new ArgumentNullException(nameof(exceedanceProbabilities), "Exceedance probabilities cannot be null.");
            }

            if (stagesOrFlows == null)
            {
                throw new ArgumentNullException(nameof(stagesOrFlows), "Stages or flows cannot be null.");
            }

            if (exceedanceProbabilities.Length != stagesOrFlows.Length)
            {
                throw new ArgumentException("Exceedance probabilities and stages/flows must have the same length.");
            }

            if (exceedanceProbabilities.Length < 2)
            {
                throw new ArgumentException("At least 2 data points are required for the Less Simple Method.");
            }

            if (equivalentRecordLength < 1)
            {
                throw new ArgumentException("Equivalent record length must be at least 1 year.", nameof(equivalentRecordLength));
            }

            // Use default metadata if none provided
            CurveMetaData metadata = curveMetaData ?? new CurveMetaData();

            // Step 1: Convert flows to log space if using flows (matching GraphicalDistribution behavior)
            double[] stageOrLoggedFlowValues;
            if (usingStagesNotFlows)
            {
                stageOrLoggedFlowValues = stagesOrFlows;
            }
            else
            {
                stageOrLoggedFlowValues = LogFlows(stagesOrFlows);
            }

            // Step 2: Extrapolate the frequency function to cover 0.9999 to 0.0001
            PairedData extrapolatedFrequencyFunction = ExtrapolateFrequencyFunction(exceedanceProbabilities, stageOrLoggedFlowValues);

            // Step 3: Fill with required exceedance probabilities
            double[] filledExceedanceProbabilities = FillInputExceedanceProbabilitiesWithRequiredPoints(extrapolatedFrequencyFunction.Xvals);

            // Step 4: Interpolate quantiles for the filled probabilities
            double[] interpolatedStageOrLogFlowValues = InterpolateQuantiles.InterpolateOnX(
                extrapolatedFrequencyFunction.Xvals,
                filledExceedanceProbabilities,
                extrapolatedFrequencyFunction.Yvals);

            // Step 5: Compute standard errors using the Less Simple Method
            double[] standardErrors = ComputeStandardErrors(
                filledExceedanceProbabilities,
                interpolatedStageOrLogFlowValues,
                equivalentRecordLength,
                frequentEventThreshold,
                rareEventThreshold);

            // Step 6: Construct distributions based on whether we're using stages or flows
            ContinuousDistribution[] distributions = ConstructDistributions(
                interpolatedStageOrLogFlowValues,
                standardErrors,
                usingStagesNotFlows);

            // Step 7: Create and return UncertainPairedData
            return (filledExceedanceProbabilities, distributions);
        }

        /// <summary>
        /// Converts flow values to log space for LogNormal distribution.
        /// </summary>
        private static double[] LogFlows(double[] unloggedFlows)
        {
            double[] loggedFlows = new double[unloggedFlows.Length];
            double minFlow = 0.01; // for log conversion not to fail
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
        /// Extrapolates the frequency function to cover the range 0.9999 to 0.0001.
        /// </summary>
        private static PairedData ExtrapolateFrequencyFunction(double[] exceedanceProbabilities, double[] stageOrLoggedFlowValues)
        {
            double toleratedDifference = MAX_INTERVAL_FOR_EXTRAPOLATION;
            double maximumExceedanceProbability = LESS_SIMPLE_MAX_EXCEED_PROB;
            double minimumExceedanceProbability = LESS_SIMPLE_MIN_EXCEED_PROB;

            List<double> extrapolatedFlowOrStageValues = new();
            List<double> extrapolatedExceedanceProbabilities = new();
            for (int i = 0; i < exceedanceProbabilities.Length; i++)
            {
                extrapolatedFlowOrStageValues.Add(stageOrLoggedFlowValues[i]);
                extrapolatedExceedanceProbabilities.Add(exceedanceProbabilities[i]);
            }

            // More frequent end of the frequency curve
            if (maximumExceedanceProbability - extrapolatedExceedanceProbabilities.First() > toleratedDifference)
            {
                double smallestInputFlowOrStage = extrapolatedFlowOrStageValues[0];
                extrapolatedExceedanceProbabilities.Insert(0, maximumExceedanceProbability);

                // Note: This matches the original GraphicalDistribution logic exactly, including the
                // independent first if statement and the second if-else chain
                if (smallestInputFlowOrStage < 0)
                {
                    extrapolatedFlowOrStageValues.Insert(0, 1.001 * smallestInputFlowOrStage);
                }

                if (smallestInputFlowOrStage > 0)
                {
                    extrapolatedFlowOrStageValues.Insert(0, 0.999 * smallestInputFlowOrStage);
                }
                else if (smallestInputFlowOrStage < -1.0e-4)
                {
                    extrapolatedFlowOrStageValues[0] = 1.001 * smallestInputFlowOrStage;
                }
                else
                {
                    extrapolatedFlowOrStageValues.Insert(0, -1.0e-4);
                }
            }

            // Less frequent end of the frequency curve
            if (extrapolatedExceedanceProbabilities.Last() - minimumExceedanceProbability > toleratedDifference)
            {
                Normal standardNormalDistribution = new();
                double penultimateInputExceedanceProbability = extrapolatedExceedanceProbabilities[^2];
                double lastInputExceedanceProbability = extrapolatedExceedanceProbabilities.Last();
                double zValueOfMin = standardNormalDistribution.InverseCDF(minimumExceedanceProbability);
                double zValueOfPenultimateInputProbability = standardNormalDistribution.InverseCDF(penultimateInputExceedanceProbability);
                double zValueOfLastInputProbability = standardNormalDistribution.InverseCDF(lastInputExceedanceProbability);
                double penultimateInputFlowOrStage = extrapolatedFlowOrStageValues[^2];
                double lastInputFlowOrStage = extrapolatedFlowOrStageValues.Last();
                double c = (zValueOfLastInputProbability - zValueOfPenultimateInputProbability) / (zValueOfMin - zValueOfPenultimateInputProbability);
                double upperFlowOrStage = ((lastInputFlowOrStage - penultimateInputFlowOrStage) + c * penultimateInputFlowOrStage) / c;
                extrapolatedFlowOrStageValues.Add(upperFlowOrStage);
                extrapolatedExceedanceProbabilities.Add(minimumExceedanceProbability);
            }

            return new PairedData(extrapolatedExceedanceProbabilities.ToArray(), extrapolatedFlowOrStageValues.ToArray());
        }

        /// <summary>
        /// Fills the input exceedance probabilities with required points from DoubleGlobalStatics.
        /// </summary>
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

        /// <summary>
        /// Computes standard errors for each exceedance probability using Beth Faber's Less Simple Method.
        /// </summary>
        private static double[] ComputeStandardErrors(
            double[] exceedanceProbabilities,
            double[] stagesOrFlows,
            int equivalentRecordLength,
            double frequentEventThreshold,
            double rareEventThreshold)
        {
            int numPoints = exceedanceProbabilities.Length;
            double[] standardErrors = new double[numPoints];

            // Step 1: Find transition indices where we stop computing and start holding constant
            int frequentEventIndex = FindClosestProbabilityIndex(exceedanceProbabilities, frequentEventThreshold);
            int rareEventIndex = FindClosestProbabilityIndex(exceedanceProbabilities, rareEventThreshold);

            // Step 2: Compute standard errors for interior points (points that have neighbors on both sides)
            ComputeInteriorStandardErrors(
                exceedanceProbabilities,
                stagesOrFlows,
                equivalentRecordLength,
                standardErrors);

            // Step 3: Compute standard errors for boundary points (first and last)
            ComputeBoundaryStandardErrors(
                exceedanceProbabilities,
                stagesOrFlows,
                equivalentRecordLength,
                standardErrors);

            // Step 4: Apply constant standard errors at distribution tails
            ApplyConstantStandardErrorsAtTails(standardErrors, frequentEventIndex, rareEventIndex);

            return standardErrors;
        }

        /// <summary>
        /// Finds the index of the exceedance probability closest to the target threshold.
        /// </summary>
        private static int FindClosestProbabilityIndex(double[] exceedanceProbabilities, double targetProbability)
        {
            int closestIndex = -1;
            double minDistance = double.MaxValue;

            for (int i = 0; i < exceedanceProbabilities.Length; i++)
            {
                double distance = Math.Abs(exceedanceProbabilities[i] - targetProbability);
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
        private static void ComputeInteriorStandardErrors(
            double[] exceedanceProbabilities,
            double[] stagesOrFlows,
            int equivalentRecordLength,
            double[] standardErrors)
        {
            // Process points from index 1 to Length-2 (skipping first and last)
            for (int i = 1; i < exceedanceProbabilities.Length - 1; i++)
            {
                double nonExceedanceProbability = 1.0 - exceedanceProbabilities[i];
                double slope = ComputeSlope(exceedanceProbabilities, stagesOrFlows, i);
                standardErrors[i] = Equation6StandardError(nonExceedanceProbability, slope, equivalentRecordLength);
            }
        }

        /// <summary>
        /// Computes standard errors for boundary points (first and last) using adjacent slopes.
        /// Since we can't compute slope at boundaries, we borrow the slope from the nearest interior point.
        /// </summary>
        private static void ComputeBoundaryStandardErrors(
            double[] exceedanceProbabilities,
            double[] stagesOrFlows,
            int equivalentRecordLength,
            double[] standardErrors)
        {
            int lastIndex = exceedanceProbabilities.Length - 1;

            // First point: use slope from second point (index 1)
            if (exceedanceProbabilities.Length > 1)
            {
                double nonExceedanceProbFirst = 1.0 - exceedanceProbabilities[0];
                double slopeAtSecondPoint = ComputeSlope(exceedanceProbabilities, stagesOrFlows, 1);
                standardErrors[0] = Equation6StandardError(nonExceedanceProbFirst, slopeAtSecondPoint, equivalentRecordLength);
            }

            // Last point: use slope from second-to-last point (index Length-2)
            if (exceedanceProbabilities.Length > 2)
            {
                double nonExceedanceProbLast = 1.0 - exceedanceProbabilities[lastIndex];
                double slopeAtSecondToLast = ComputeSlope(exceedanceProbabilities, stagesOrFlows, lastIndex - 1);
                standardErrors[lastIndex] = Equation6StandardError(nonExceedanceProbLast, slopeAtSecondToLast, equivalentRecordLength);
            }
        }

        /// <summary>
        /// Applies constant standard errors at the tails of the distribution to prevent unrealistic uncertainty.
        /// - Frequent events (low exceedance prob): hold constant from frequentEventIndex to end
        /// - Rare events (high exceedance prob): hold constant from start to rareEventIndex
        /// </summary>
        private static void ApplyConstantStandardErrorsAtTails(
            double[] standardErrors,
            int frequentEventIndex,
            int rareEventIndex)
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

        /// <summary>
        /// Computes the slope of the frequency curve at a given point using normal probability interpolation.
        /// </summary>
        /// <param name="exceedanceProbabilities">Array of exceedance probabilities</param>
        /// <param name="stageOrLoggedFlowValues">Array of stage or logged flow values</param>
        /// <param name="i">Index at which to compute the slope (must be between 1 and Length-2)</param>
        /// <returns>The slope at the specified point</returns>
        private static double ComputeSlope(double[] exceedanceProbabilities, double[] stageOrLoggedFlowValues, int i)
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

        /// <summary>
        /// Interpolates a quantile value at a target probability using normal probability transformation.
        /// </summary>
        private static double InterpolateNormally(double p, double p_minus, double q, double q_minus, double p_target)
        {
            Normal standardNormal = new Normal();

            double z = standardNormal.InverseCDF(p);
            double z_minus = standardNormal.InverseCDF(p_minus);
            double z_target = standardNormal.InverseCDF(p_target);

            double q_target = q_minus + (z_target - z_minus) / (z - z_minus) * (q - q_minus);

            return q_target;
        }

        /// <summary>
        /// Calculates standard error using Equation 6 from HEC-FDA Technical Reference (CPD-72a).
        /// SE² = [p(1-p)] / [(1/slope)² × ERL]
        /// </summary>
        /// <param name="nonExceedanceProbability">Non-exceedance probability (1 - exceedance probability)</param>
        /// <param name="slope">Local slope of the frequency curve in z space</param>
        /// <param name="equivalentRecordLength">Equivalent record length in years</param>
        /// <returns>Standard error (standard deviation) for the stage/flow at this probability</returns>
        private static double Equation6StandardError(double nonExceedanceProbability, double slope, int erl)
        {
            double standardErrorSquared = (nonExceedanceProbability * (1 - nonExceedanceProbability)) / (Math.Pow(1 / slope, 2.0D) * erl);
            double standardError = Math.Sqrt(standardErrorSquared);
            return standardError;
        }

        /// <summary>
        /// Constructs an array of distributions (Normal or LogNormal) based on means and standard errors.
        /// </summary>
        /// <param name="means">Array of mean values (stages or flows)</param>
        /// <param name="standardErrors">Array of standard errors</param>
        /// <param name="usingStagesNotFlows">True for Normal distributions (stages), False for LogNormal (flows)</param>
        /// <returns>Array of continuous distributions</returns>
        private static ContinuousDistribution[] ConstructDistributions(
            double[] means,
            double[] standardErrors,
            bool usingStagesNotFlows)
        {
            ContinuousDistribution[] distributions = new ContinuousDistribution[means.Length];

            if (usingStagesNotFlows)
            {
                // Use Normal distributions for stages
                for (int i = 0; i < means.Length; i++)
                {
                    distributions[i] = new Normal(means[i], standardErrors[i]);
                }
            }
            else
            {
                // Use LogNormal distributions for flows
                for (int i = 0; i < means.Length; i++)
                {
                    distributions[i] = new LogNormal(means[i], standardErrors[i]);
                }
            }

            return distributions;
        }
    }
}
