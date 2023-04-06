using HEC.FDA.Model.interfaces;
using HEC.FDA.Model.paireddata;
using Statistics;
using Statistics.Histograms;

namespace HEC.FDA.Model.extensions
{
    public static class ContinuousDistributionExtensions
    {
        public static PairedData BootstrapToPairedData(this ContinuousDistribution continuousDistribution, IProvideRandomNumbers randomProvider, double[] ExceedanceProbabilities, int ordinatesToSample = 0)
        {
            double[] samples;
            if (ordinatesToSample <= 0)
            {
                ordinatesToSample = (int)continuousDistribution.SampleSize;
            }
            samples = randomProvider.NextRandomSequence(ordinatesToSample);
            IDistribution bootstrap = continuousDistribution.Sample(samples);
            double[] x = new double[ExceedanceProbabilities.Length];
            double[] y = new double[ExceedanceProbabilities.Length];
            for (int i = 0; i < ExceedanceProbabilities.Length; i++)
            {
                //same exceedance probs as graphical and as 1.4.3
                double prob = 1 - ExceedanceProbabilities[i];
                x[i] = prob;
                //y values in increasing order 
                y[i] = bootstrap.InverseCDF(prob);
            }
            return new PairedData(x, y);
        }

        public static UncertainPairedData BootstrapToUncertainPairedData(this ContinuousDistribution continuousDistribution, IProvideRandomNumbers randomProvider, double[] ExceedanceProbabilities,int realizations , double histogramBinWidth, int ordinatesToSample = 0)
        {
            Histogram[] ys = new Histogram[ExceedanceProbabilities.Length];
            for (int iterator = 0; iterator < ys.Length; iterator++)
            {
                ys[iterator] = new Histogram(histogramBinWidth);
            }

            for(int i=0; i<realizations; i++)
            {
                PairedData pd = continuousDistribution.BootstrapToPairedData( randomProvider, ExceedanceProbabilities, ordinatesToSample);
                for(int j=0;j<ExceedanceProbabilities.Length-1;j++)
                {
                    ys[j].AddObservationToHistogram(pd.f(ExceedanceProbabilities[j]));
                }
            }
            return new UncertainPairedData(ExceedanceProbabilities, ys, new CurveMetaData("Exceedance Probs", "Flow Histograms"));
        }
    }
}
