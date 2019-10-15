using System;
using System.Collections.Generic;
using System.Text;

using Utilities.Validation;

namespace Statistics.Distributions
{
    /// <summary>
    /// A specific truncated distribution that places density from outside the truncation value(s) at the truncation value(s) without altering the statistics of the underlying distribution.
    /// </summary>
    /// <remarks> Measures of central tendency and dispersion are based on the underlying distribution NOT the truncated copy. </remarks>
    internal class TruncatedDistribution: IDistribution //IOrdinate<IDistribution>
    {
        // TODO: Validation lower bound below mean, median, mode and upper bound above mean, median mode - valid range of lower and upper bounds 

        #region Fields and Properties
        private readonly IDistribution _Distribution;

        #region IDistribution Properties
        public IDistributions Type => (IDistributions)((int)_Distribution.Type * 10);
        public double Mean => _Distribution.Mean;
        public double Median => _Distribution.Median;
        public double Variance => _Distribution.Variance;
        public double StandardDeviation => _Distribution.StandardDeviation;
        public double Skewness => _Distribution.Skewness;
        public double Minimum { get; }
        public double Maximum { get; }
        public int SampleSize => _Distribution.SampleSize;
        #endregion
        //#region IOrdinate Properties
        //public bool IsVariable => true;
        //public Type OrdinateType => typeof(IDistribution);
        //#endregion
        #endregion

        #region Constructor
        public TruncatedDistribution(IDistribution distribution, double lowerBound = double.NegativeInfinity, double upperBound = double.PositiveInfinity)
        {
            _Distribution = distribution;
            Minimum = lowerBound == double.NegativeInfinity ? _Distribution.Minimum: lowerBound;
            Maximum = upperBound == double.PositiveInfinity ? _Distribution.Maximum: upperBound;
        }
        #endregion

        #region Functions
        #region IDistribution Functions
        public double PDF(double x)
        {
            // Inbetween truncation points density is computed normally
            if (x > Minimum && x < Maximum) return _Distribution.PDF(x);
            else
            {
                // IF x is at or below the lower bound
                if (x < Minimum) return 0; // below the lower bound is no density.
                else if (x == Minimum) return _Distribution.CDF(x); // all the density below the lower bound has been added to this point.
                // IF x is at or above the upper bound
                else if (x == Maximum) return _Distribution.CDF(_Distribution.Maximum) - _Distribution.CDF(Maximum); // all the density above the upper bound is added at this point.
                else return 0; // since it must be true that x > Maximum (and there is no denisity above the upper bound).
            }
        }
        public double CDF(double x)
        {
            // Inbetween truncation points density is computed normally
            if (x > Minimum && x < Maximum) return _Distribution.CDF(x);
            else
            {
                if (x < Minimum) return 0; // below the lower bound is no density.
                else if (x == Minimum) return _Distribution.CDF(x); // at the minimum all the underlying distribution density is accumulated.
                else return 1; // since x >= Maximum all the density has been accumulated.
            }
        }
        public double InverseCDF(double p)
        {
            if (!p.IsOnRange(0, 1)) throw new ArgumentOutOfRangeException($"The specified probability: {p} is not on the valid range [0, 1].");
            if (p <= _Distribution.CDF(Minimum)) return Minimum;
            else if (p >= _Distribution.CDF(Maximum)) return Maximum;
            else return _Distribution.InverseCDF(p);
        }

        public double Sample() => InverseCDF(new Random().NextDouble());
        public double[] Sample(Random numberGenerator = null) => Sample(SampleSize, numberGenerator);
        public double[] Sample(int sampleSize, Random numberGenerator = null)
        {
            if (numberGenerator == null) numberGenerator = new Random();
            double[] sample = new double[sampleSize];
            for (int i = 0; i < sample.Length; i++) sample[i] = InverseCDF(numberGenerator.NextDouble());
            return sample;
        }
        public IDistribution SampleDistribution(Random numberGenerator = null)
        {
            IDistribution distribution = IDistributionFactory.Fit(Sample(numberGenerator), _Distribution.Type);
            return new TruncatedDistribution(distribution, Minimum, Maximum);
        }
        public string Print() => $"TruncatedDistribution(distribution: {_Distribution.Print()}, truncated range: [{Minimum}, {Maximum}])";
        public bool Equals(IDistribution distribution) => string.Compare(Print(), distribution.Print()) == 0 ? true : false;
        #endregion
        //#region Iordinate Functions
        //public double GetValue(double sampleProbability) => InverseCDF(sampleProbability);
        //public bool Equals<T>(IOrdinate<T> ordinate) => ordinate.OrdinateType == typeof(IDistribution) ? Equals((IDistribution)ordinate) : false;
        //#endregion
        #endregion
    }
}
