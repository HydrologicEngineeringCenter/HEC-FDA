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
            Array.Sort(ExceedanceProbabilities);
            double[] samples;
            samples = randomProvider.NextRandomSequence(continuousDistribution.SampleSize);
            IDistribution bootstrap = continuousDistribution.Sample(samples);
            double[] y = new double[ExceedanceProbabilities.Length];
            for (int i = 0; i < ExceedanceProbabilities.Length; i++)
            {
                //y values in decreasing order 
                // "1-x" because LP3 is done in Non-Exceedence Probabilities, but we want to speak in exceedence. The 500YR flood should have a tiny probability associated with it. Not a large one 
                y[i] = bootstrap.InverseCDF(1-ExceedanceProbabilities[i]);
            }
            return new PairedData(ExceedanceProbabilities, y);
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
                    double prob = ExceedanceProbabilities[j];
                    double flow = pd.f(prob); 
                    ys[j].AddObservationToHistogram(flow) ;
                }
            }
            return new UncertainPairedData(ExceedanceProbabilities, ys, new CurveMetaData("Exceedance Probs", "Flow Histograms"));
        }
    }
}
