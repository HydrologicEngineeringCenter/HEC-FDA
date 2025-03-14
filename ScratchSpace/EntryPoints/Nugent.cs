using Statistics.Distributions;
using Statistics;
using HEC.FDA.Model.extensions;

namespace ScratchSpace.EntryPoints;

internal class Nugent
{
    public static void EntryPoint()
    {

        //[InlineData(new double[] { 0.999, 0.5, 0.2, 0.1, 0.02, 0.01, 0.005, 0.001, 0.0001 }, new double[] { 80, 11320, 18520, 23810, 35010, 39350, 42850, 47300, 52739.48924 }, 50, false, new double[] { .2244, .1164, .1293, .1924, .2030, .2030 })]
        

            double[] exceedanceProbabilities = new double[] { 0.999, 0.5, 0.2, 0.1, 0.02, 0.01, 0.005, 0.001, 0.0001 };
            double[] flowOrStageValues = new double[] { 80, 11320, 18520, 23810, 35010, 39350, 42850, 47300, 52739.48924 }; 
            int equivalentRecordLength = 50;
            bool usingStagesNotFlows = true;
            double[] expectedSD = new double[] { 936.69, 1951.72, 3213.97, 6066.15, 7900.75, 7900.75 };

            GraphicalDistribution graphical = new GraphicalDistribution(exceedanceProbabilities, flowOrStageValues, equivalentRecordLength, usingStagesNotFlows);
            Statistics.ContinuousDistribution[] actualDistributions = graphical.StageOrLogFlowDistributions;
            for (int i = 2; i < 8; i++)
            {
                double actual;
                if (usingStagesNotFlows)
                {
                    actual = ((Normal)actualDistributions[i]).StandardDeviation;
                }
                else
                {
                    actual = ((LogNormal)actualDistributions[i]).StandardDeviation;
                }

            double relativeError = Math.Abs((actual - expectedSD[i - 2]) / expectedSD[i - 2]);
            Console.WriteLine($"actual: {actual}, expected:{expectedSD[i-2]}, {relativeError}" + Environment.NewLine);
            }
    }
}
