using Statistics.Distributions;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.Model.paireddata
{
    public class InterpolateQuantiles
    {
        /// <summary>
        /// Interpolates on stage for flow for an expanded set of exceedance probabilities 
        /// </summary>
        /// <param name="inputExceedanceProbabilities"></param> input exceedance probabilities 
        /// <param name="exceedanceProbabilitiesForWhichQuantilesAreRequired"></param> non-exceedance probabilities at which to calculate stage or flow
        /// <param name="inputDataForInterpolation"></param> input stage or flow values
        /// <returns></returns>
        public static double[] InterpolateOnX(IReadOnlyList<double> inputExceedanceProbabilities, IReadOnlyList<double> exceedanceProbabilitiesForWhichQuantilesAreRequired, IReadOnlyList<double> inputDataForInterpolation)
        {

            Normal standardNormalDistribution = new();
            double[] inputZs = new double[inputExceedanceProbabilities.Count];
            for (int i = 0; i < inputExceedanceProbabilities.Count; i++)
            {
                inputZs[i] = standardNormalDistribution.InverseCDF(1-inputExceedanceProbabilities[i]);
            }
            double[] neededZs = new double[exceedanceProbabilitiesForWhichQuantilesAreRequired.Count];
            for (int i = 0; i < exceedanceProbabilitiesForWhichQuantilesAreRequired.Count; i++)
            {
                neededZs[i] = standardNormalDistribution.InverseCDF(1-exceedanceProbabilitiesForWhichQuantilesAreRequired[i]);
            }
            PairedData nonexceedance_zScore = new PairedData(exceedanceProbabilitiesForWhichQuantilesAreRequired, neededZs);
            PairedData zScore_stage_flow = new PairedData(inputZs, inputDataForInterpolation);
            PairedData interpolatedFrequencyCurve = zScore_stage_flow.compose(nonexceedance_zScore);

            return interpolatedFrequencyCurve.Yvals.ToArray();
        }
    }
}
