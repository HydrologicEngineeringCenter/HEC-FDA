using HEC.FDA.Model.paireddata;
using Statistics.Distributions;

namespace Statistics.Graphical
{
    public class InterpolateQuantiles
    {
        /// <summary>
        /// Interpolates on stage for flow for an expanded set of exceedance probabilities 
        /// </summary>
        /// <param name="inputNonexceedanceProbabilities"></param> input non-exceedance probabilities 
        /// <param name="exceedanceProbabilitiesForWhichQuantilesAreRequired"></param> non-exceedance probabilities at which to calculate stage or flow
        /// <param name="inputDataForInterpolation"></param> input stage or flow values
        /// <returns></returns>
        public static double[] InterpolateOnX(double[] inputNonexceedanceProbabilities, double[] exceedanceProbabilitiesForWhichQuantilesAreRequired, double[] inputDataForInterpolation)
        {

            Normal standardNormalDistribution = new Normal();
            double[] inputZs = new double[inputNonexceedanceProbabilities.Length];
            for (int i = 0; i < inputNonexceedanceProbabilities.Length; i++)
            {
                inputZs[i] = standardNormalDistribution.InverseCDF(inputNonexceedanceProbabilities[i]);
            }
            double[] neededZs = new double[exceedanceProbabilitiesForWhichQuantilesAreRequired.Length];
            for (int i = 0; i < exceedanceProbabilitiesForWhichQuantilesAreRequired.Length; i++)
            {
                neededZs[i] = standardNormalDistribution.InverseCDF(1-exceedanceProbabilitiesForWhichQuantilesAreRequired[i]);
            }

            IPairedData nonexceedance_zScore = new PairedData(exceedanceProbabilitiesForWhichQuantilesAreRequired, neededZs);
            IPairedData zScore_stage_flow = new PairedData(inputZs, inputDataForInterpolation);
            IPairedData interpolatedFrequencyCurve = zScore_stage_flow.compose(nonexceedance_zScore);

            //TODO: We do not have this linear interpolation taking place instead of interpolating by way of normal distribution
            //else//out at the tail, use linear interpolation...
            //{
            //    quantiles[i] = inputProbabilities[inputOrdinate - 1] + ((probabilitiesForWhichQuantilesAreRequired[i] - inputDataForInterpolation[inputOrdinate - 1]) / (inputDataForInterpolation[inputOrdinate] - inputDataForInterpolation[inputOrdinate - 1])) * (inputProbabilities[inputOrdinate] - inputProbabilities[inputOrdinate - 1]);
            //}

            return interpolatedFrequencyCurve.Yvals;
        }
    }
}
