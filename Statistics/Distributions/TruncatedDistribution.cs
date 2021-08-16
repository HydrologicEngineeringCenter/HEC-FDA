using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Utilities;
using Utilities.Serialization;

namespace Statistics.Distributions
{
    /// <summary>
    /// A specific truncated distribution that places density from outside the truncation value(s) at the truncation value(s) without altering the statistics of the underlying distribution.
    /// </summary>
    /// <remarks> Measures of central tendency and dispersion are based on the underlying distribution NOT the truncated copy. </remarks>
    internal class TruncatedDistribution : IDistribution, IValidate<TruncatedDistribution>
    {
        // TODO: Validation lower bound below mean, median, mode and upper bound above mean, median mode - valid range of lower and upper bounds 

        #region Fields and Properties
        private readonly IDistribution _Distribution;

        public IDistributionEnum Type => (IDistributionEnum)((int)_Distribution.Type * 10);
        public double Mean => _Distribution.Mean;
        public double Median => _Distribution.Median;
        public double Variance => _Distribution.Variance;
        public double StandardDeviation => _Distribution.StandardDeviation;
        public double Skewness => _Distribution.Skewness;
        public Utilities.IRange<double> Range { get; }
        public int SampleSize => _Distribution.SampleSize;
        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }

        public double Mode => _Distribution.Mode;
        #endregion

        #region Constructor
        public TruncatedDistribution(IDistribution distribution, double lowerBound = double.NegativeInfinity, double upperBound = double.PositiveInfinity)
        {
            if (distribution.IsNull()) throw new ArgumentNullException(nameof(distribution), "The specified distribution parameter is invalid because it is null.");
            _Distribution = distribution;
            IMessageBoard msgBoard = IMessageBoardFactory.Factory(_Distribution);
            Range = Utilities.IRangeFactory.Factory(lowerBound == double.NegativeInfinity ? _Distribution.Range.Min : lowerBound, upperBound == double.PositiveInfinity ? _Distribution.Range.Max : upperBound);
            State = Validate(new Validation.TruncatedDistributionValidator(), out IEnumerable<IMessage> msgs);
            msgBoard.PostMessages(msgs);
            Messages = msgBoard.ReadMessages();
        }
        #endregion

        #region Functions
        public IMessageLevels Validate(IValidator<TruncatedDistribution> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }
        
        #region IDistribution Functions
        public double PDF(double x)
        {
            // In between truncation points density is computed normally
            if (x > Range.Min && x < Range.Max) return _Distribution.PDF(x);
            else
            {
                // IF x is at or below the lower bound
                if (x < Range.Min) return 0; // below the lower bound is no density.
                else if (x == Range.Min) return _Distribution.CDF(x); // all the density below the lower bound has been added to this point.
                // IF x is at or above the upper bound
                else if (x == Range.Max) return _Distribution.CDF(_Distribution.Range.Max) - _Distribution.CDF(Range.Max); // all the density above the upper bound is added at this point.
                else return 0; // since it must be true that x > Maximum (and there is no density above the upper bound).
            }
        }
        public double CDF(double x)
        {
            // In between truncation points density is computed normally
            if (x > Range.Min && x < Range.Max) return _Distribution.CDF(x);
            else
            {
                if (x < Range.Min) return 0; // below the lower bound is no density.
                else if (x == Range.Min) return _Distribution.CDF(x); // at the minimum all the underlying distribution density is accumulated.
                else return 1; // since x >= Maximum all the density has been accumulated.
            }
        }
        public double InverseCDF(double p)
        {
            if (!p.IsOnRange(0, 1)) throw new ArgumentOutOfRangeException($"The specified probability: {p} is not on the valid range [0, 1].");
            if (p <= _Distribution.CDF(Range.Min)) return Range.Min;
            else if (p >= _Distribution.CDF(Range.Max)) return Range.Max;
            else return _Distribution.InverseCDF(p);
        }

        //public double Sample(Random r = null) => InverseCDF(r == null ? new Random().NextDouble() : r.NextDouble());
        ////public double[] Sample(Random numberGenerator = null) => Sample(SampleSize, numberGenerator);
        //public double[] Sample(int sampleSize, Random numberGenerator = null)
        //{
        //    if (numberGenerator == null) numberGenerator = new Random();
        //    double[] sample = new double[sampleSize];
        //    for (int i = 0; i < sample.Length; i++) sample[i] = InverseCDF(numberGenerator.NextDouble());
        //    return sample;
        //}
        //public IDistribution SampleDistribution(Random numberGenerator = null)
        //{
        //    IDistribution distribution = IDistributionFactory.Fit(Sample(SampleSize, numberGenerator), _Distribution.Type);
        //    return new TruncatedDistribution(distribution, Range.Min, Range.Max);
        //}
        public string Print(bool round = false) => round ? Print(_Distribution, Range) : $"TruncatedDistribution(distribution: {_Distribution.Print()}, truncated range: {Range.Print()})";
        public string Requirements(bool printNotes)
        {
            string msg = $"The truncated distribution requires the following parameterization: TruncatedDistribution(distribution: {_Distribution.Requirements(false)}, truncated range: [{double.NegativeInfinity}, {double.PositiveInfinity}] with range min < range max)";
            if (printNotes) msg += RequirementNotes();
            return msg;
        }
        public bool Equals(IDistribution distribution) => string.Compare(Print(), distribution.Print()) == 0 ? true : false;
        #endregion

        internal static string Print(IDistribution distribution, IRange<double> range) => $"TruncatedDistribution(distribution: {distribution.Print(true)}, truncated range: {range.Print(true)})";
        internal static string RequiredParameterization(bool printNotes)
        {
            string msg = $"Truncated distributions replace values on the tail(s) of an IDistribution with specified maximum or minimum values. Thus a valid IDistribution and logical range: max > min are required as Truncated Distribution parameters.";
            if (printNotes) msg += RequirementNotes();
            return msg;
        }
        internal static string RequirementNotes() => $"If the minimum value is set to {double.NegativeInfinity} the left hand tail of the IDistribution will not be truncated. Similarly, if the maximum is set to {double.PositiveInfinity} the right hand tail of the IDistribution will not be truncated.";
        #endregion

        XElement ISerializeToXML<IDistribution>.WriteToXML()
        {
            throw new NotImplementedException();
        }
    }
}
