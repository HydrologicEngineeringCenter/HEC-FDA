using System;
using System.Collections.Generic;
using System.Text;

using Utilities.Validation;

namespace Statistics.Distributions
{
    internal class Triangular: IDistribution, IOrdinate<IDistribution>
    {
        //TODO: Sample
        //TODO: Validation

        #region Fields and Properties
        private readonly MathNet.Numerics.Distributions.Triangular _Distribution;

        #region IDistribution Properties
        public IDistributions Type => IDistributions.Triangular;
        public double Mean => _Distribution.Mean;
        public double Median => _Distribution.Median;
        public double Variance => _Distribution.Variance;
        public double StandardDeviation => _Distribution.StdDev;
        public double Skewness => _Distribution.Skewness;
        public double Minimum => _Distribution.Minimum;
        public double Maximum => _Distribution.Maximum;
        public int SampleSize { get; }
        #endregion
        #region IOrdinate Properties
        public bool IsVariable => true;
        public Type OrdinateType => typeof(IDistribution);
        #endregion
        #endregion

        #region Constructor
        public Triangular(double min, double mostLikely, double max, int sampleSize = int.MaxValue)
        {
            _Distribution = new MathNet.Numerics.Distributions.Triangular(lower: min, upper: max, mode: mostLikely);
            SampleSize = sampleSize;
        }
        #endregion

        #region Functions
        #region IDistribution Functions
        public double PDF(double x) => _Distribution.Density(x);
        public double CDF(double x) =>  _Distribution.CumulativeDistribution(x);
        public double InverseCDF(double p) => p.IsOnRange(0,1) ? _Distribution.InverseCumulativeDistribution(p): throw new ArgumentOutOfRangeException($"The specified probability parameter, p: {p} is invalid because it it not on the valid range [0, 1].");

        public double Sample() => InverseCDF(new Random().NextDouble());
        public double[] Sample(Random numberGenerator = null) => Sample(SampleSize, numberGenerator);
        public double[] Sample(int sampleSize, Random numberGenerator = null)
        {
            if (numberGenerator == null) numberGenerator = new Random();
            double[] sample = new double[SampleSize];
            for (int i = 0; i < sample.Length; i++) sample[i] = InverseCDF(numberGenerator.NextDouble());
            return sample;
        }
        public IDistribution SampleDistribution(Random numberGenerator = null) => Fit(Sample(numberGenerator));
        public string Print() => $"Triangular(mean: {Mean}, range: [{Minimum}, {Maximum}], sample size: {SampleSize})";
        public bool Equals(IDistribution distribution) => string.Compare(Print(), distribution.Print()) == 0 ? true : false;
        #endregion
        #region Iordinate Functions
        public double GetValue(double sampleProbability) => InverseCDF(sampleProbability);
        public bool Equals<T>(IOrdinate<T> ordinate) => ordinate.OrdinateType == typeof(IDistribution) ? Equals((IDistribution)ordinate) : false;
        #endregion

        public static Triangular Fit(IEnumerable<double> data)
        {
            SummaryStatistics stats = new SummaryStatistics(data);
            return new Triangular(stats.Minimum, stats.Mean, stats.Maximum, stats.SampleSize);
        }
        #endregion
    }
}
