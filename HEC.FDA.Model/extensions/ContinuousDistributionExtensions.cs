using HEC.FDA.Model.interfaces;
using HEC.FDA.Model.paireddata;
using Statistics;
using Statistics.Histograms;
using System.Threading.Tasks;

namespace HEC.FDA.Model.extensions
{
    public static class ContinuousDistributionExtensions
    {
        public static PairedData BootstrapToPairedData(this ContinuousDistribution continuousDistribution, double[] samples, double[] ExceedanceProbabilities )
        {
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

        public static UncertainPairedData BootstrapToUncertainPairedData(this ContinuousDistribution continuousDistribution, double[][] samples,
    double[] ExceedanceProbabilities, double histogramBinWidth = 100)
        {
            int realizations = samples.GetLength(0);
            ThreadsafeInlineHistogram[] ys = new ThreadsafeInlineHistogram[ExceedanceProbabilities.Length];
            for (int iterator = 0; iterator < ys.Length; iterator++)
            {
                ys[iterator] = new ThreadsafeInlineHistogram(histogramBinWidth,new ConvergenceCriteria());
            }
            Parallel.For(0, realizations, i =>
            {
                PairedData pd = continuousDistribution.BootstrapToPairedData(samples[i],ExceedanceProbabilities );
                for (int j = 0; j < ExceedanceProbabilities.Length; j++)
                {
                    double y = pd.Yvals[j];
                        ys[j].AddObservationToHistogram(y,i);
                }
            });
            return new UncertainPairedData(ExceedanceProbabilities, ys, new CurveMetaData("Exceedance Probs", "Flow Histograms"));
        }
    }
}
