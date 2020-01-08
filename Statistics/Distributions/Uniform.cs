using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics.Distributions
{
    internal class Uniform: IDistribution //IOrdinate<IDistribution>
    {
        #region Fields and Properties
        private readonly MathNet.Numerics.Distributions.ContinuousUniform _Distribution;

        #region IDistribution Properties
        public IDistributions Type => IDistributions.Uniform;
        public double Mean => _Distribution.Mean;
        public double Median => _Distribution.Median;
        public double Variance => _Distribution.Variance;
        public double StandardDeviation => _Distribution.StdDev;
        public double Skewness => _Distribution.Skewness;
        //public double Minimum => _Distribution.Minimum;
        //public double Maximum => _Distribution.Maximum;
        public Utilities.IRange<double> Range { get; }
        public int SampleSize { get; }
        #endregion
        //#region IOrdinate Properties
        //public bool IsVariable => true;
        //public Type OrdinateType => typeof(IDistribution);
        //#endregion
        #endregion

        #region Constructor
        public Uniform(double min, double max, int sampleSize = int.MaxValue)
        {
            _Distribution = new MathNet.Numerics.Distributions.ContinuousUniform(lower: min, upper: max);
            Range = Utilities.IRangeFactory.Factory(_Distribution.Minimum, _Distribution.Maximum);
            SampleSize = sampleSize;
        }
        #endregion

        #region Functions
        #region IDistribution Functions
        public double PDF(double x) => _Distribution.Density(x);
        public double CDF(double x) => _Distribution.CumulativeDistribution(x);
        public double InverseCDF(double p) => _Distribution.InverseCumulativeDistribution(p);

        public double Sample(Random r = null) => InverseCDF(r == null ? new Random().NextDouble() : r.NextDouble());
        //public double[] Sample(Random numberGenerator = null) => Sample(SampleSize, numberGenerator);
        public double[] Sample(int sampleSize, Random numberGenerator = null)
        {
            if (numberGenerator == null) numberGenerator = new Random();
            double[] sample = new double[sampleSize];
            for (int i = 0; i < sample.Length; i++) sample[i] = InverseCDF(numberGenerator.NextDouble());
            return sample;
        }
        public IDistribution SampleDistribution(Random numberGenerator = null) => Fit(Sample(SampleSize, numberGenerator));
        public string Print() => $"Uniform(range: {Range.Print()})";
        public bool Equals(IDistribution distribution) => string.Compare(Print(), distribution.Print()) == 0 ? true : false;
        #endregion
        //#region Iordinate Functions
        //public double GetValue(double sampleProbability) => InverseCDF(sampleProbability);
        //public bool Equals<T>(IOrdinate<T> ordinate) => ordinate.OrdinateType == typeof(IDistribution) ? Equals((IDistribution)ordinate) : false;
        //#endregion

        public static Uniform Fit(IEnumerable<double> sample)
        {
            SummaryStatistics stats = new SummaryStatistics(IDataFactory.Factory(sample));
            return new Uniform(stats.Range.Min, stats.Range.Max, stats.SampleSize);
        }
        #endregion
    }
}
