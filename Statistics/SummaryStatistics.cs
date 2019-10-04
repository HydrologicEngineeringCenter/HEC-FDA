using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{
    internal class SummaryStatistics
    {
        //TODO: Validation

        private readonly MathNet.Numerics.Statistics.DescriptiveStatistics _SampleStatistics;

        public double Mean => _SampleStatistics.Mean;
        public double Variance => _SampleStatistics.Variance;
        public double StandardDeviation => _SampleStatistics.StandardDeviation;
        public double Skewness => _SampleStatistics.Skewness;
        public double Minimum => _SampleStatistics.Minimum;
        public double Maximum => _SampleStatistics.Maximum;
        public int SampleSize => (int)_SampleStatistics.Count;

        public SummaryStatistics(IEnumerable<double> sample)
        {
            _SampleStatistics = new MathNet.Numerics.Statistics.DescriptiveStatistics(sample);
        }
    }

    
    //public class SummaryStatistics
    //{
    //    public double Mean { get; }
    //    public double Median { get; }
    //    public double Mode { get; }
    //    public double Variance { get; }
    //    public double Skewness { get; }
    //    public double Minimum { get; }
    //    public double Maximum { get; }
    //    public int SampleSize { get; }

    //    public SummaryStatistics(double mean, double median, double mode, double variance, double skewness, double minimum, double maximum, int n)
    //    {
    //        Mean = mean;
    //        Median = median;
    //        Mode = mode;
    //        Variance = variance;
    //        Skewness = skewness;
    //        Minimum = minimum;
    //        Maximum = maximum;
    //        SampleSize = n;
    //    }
    //}
}
