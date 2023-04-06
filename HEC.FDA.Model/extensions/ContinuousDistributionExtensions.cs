using HEC.FDA.Model.interfaces;
using HEC.FDA.Model.paireddata;
using Statistics;
using Statistics.Histograms;
using System;

namespace HEC.FDA.Model.extensions
{
    public static class ContinuousDistributionExtensions
    {
        public static PairedData BootstrapToPairedData(this ContinuousDistribution continuousDistribution, IProvideRandomNumbers randomProvider, double[] ExceedanceProbabilities)
        {
            double[] samples = randomProvider.NextRandomSequence(continuousDistribution.SampleSize);
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

        public static UncertainPairedData BootstrapToUncertainPairedData(this ContinuousDistribution continuousDistribution, IProvideRandomNumbers randomProvider, double[] ExceedanceProbabilities,int realizations = 10000 , double histogramBinWidth = 0.5 )
        {
            Histogram[] ys = new Histogram[ExceedanceProbabilities.Length];
            for (int iterator = 0; iterator < ys.Length; iterator++)
            {
                ys[iterator] = new Histogram(histogramBinWidth);
            }

            for(int i=0; i<realizations; i++)
            {
                PairedData pd = continuousDistribution.BootstrapToPairedData( randomProvider, ExceedanceProbabilities);
                for(int j=0;j<ExceedanceProbabilities.Length;j++)
                {
                    ys[j].AddObservationToHistogram(pd.Yvals[j]) ;
                }
            }
            return new UncertainPairedData(ExceedanceProbabilities, ys, new CurveMetaData("Exceedance Probs", "Flow Histograms"));
        }
    }
}
