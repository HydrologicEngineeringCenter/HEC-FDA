using Statistics.Distributions;
using System;
using System.Linq;

namespace Statistics.Graphical
{
    public class InterpolateQuantiles
    {
        /// <summary>
        /// Interpolates on stage for flow for an expanded set of exceedance probabilities 
        /// </summary>
        /// <param name="inputProbabilities"></param> input non-exceedance probabilities 
        /// <param name="probabilitiesForWhichQuantilesAreRequired"></param> non-exceedance probabilities at which to calculate stage or flow
        /// <param name="inputDataForInterpolation"></param> input stage or flow values
        /// <returns></returns>
        public static double[] InterpolateOnX(double[] inputProbabilities, double[] probabilitiesForWhichQuantilesAreRequired, double[] inputDataForInterpolation)
        {
            double[] quantiles = new double[probabilitiesForWhichQuantilesAreRequired.Length];
            double tolerance = 0.00001;
            Normal standardNormalDistribution = new Normal();
            double currentExceedanceProbabilityOnWhichToInterpolate;
            int inputOrdinate = 0;

            for (int i = 0; i < probabilitiesForWhichQuantilesAreRequired.Length; i++)
            {
                currentExceedanceProbabilityOnWhichToInterpolate = probabilitiesForWhichQuantilesAreRequired[i];

                //look over input exceedance probabilities 
                for (int j = 0; j < inputProbabilities.Length; j++) 
                {
                    //if the required exceedance probability reasonably matches the input exceedance probability
                    if (Math.Abs((currentExceedanceProbabilityOnWhichToInterpolate - inputProbabilities[j])) < tolerance) 
                    {
                        //get the index of the input flow or exceedance value that corresponds to probability at j 
                        inputOrdinate = j;  
                        break;
                    }
                }
                //if the index is for the first input flow or stage value
                if (inputOrdinate == 0) 
                {
                    //observe that this could lead to 
                    quantiles[i] = inputDataForInterpolation[inputOrdinate];  

                }
                else
                {
                    //for all non-exceedance probabilities less than 0.999 (or exceedance probabilities greater than 0.001) 
                    //So for about the .002 AEP and more frequent 
                    if (inputProbabilities[inputOrdinate - 1] < .999)
                    {
                        double zValueExceedanceProbability = standardNormalDistribution.InverseCDF(currentExceedanceProbabilityOnWhichToInterpolate);
                        double zValueSmallerInputExceedanceProbability = standardNormalDistribution.InverseCDF(inputProbabilities[inputOrdinate - 1]);
                        double zValueLargerExceedanceProbability = standardNormalDistribution.InverseCDF(inputProbabilities[inputOrdinate]);
                        double fractionOfQuantileDifference = ((zValueExceedanceProbability - zValueSmallerInputExceedanceProbability) / (zValueLargerExceedanceProbability - zValueSmallerInputExceedanceProbability));
                        quantiles[i] = fractionOfQuantileDifference * inputDataForInterpolation[inputOrdinate] + (1-fractionOfQuantileDifference)* inputDataForInterpolation[inputOrdinate - 1];
                        
                    }
                    else//out at the tail, use linear interpolation...
                    {
                        quantiles[i] = inputProbabilities[inputOrdinate - 1] + ((probabilitiesForWhichQuantilesAreRequired[i] - inputDataForInterpolation[inputOrdinate - 1]) / (inputDataForInterpolation[inputOrdinate] - inputDataForInterpolation[inputOrdinate - 1])) * (inputProbabilities[inputOrdinate] - inputProbabilities[inputOrdinate - 1]);
                    }
                }
            }
            return quantiles;
        }
    }
}
