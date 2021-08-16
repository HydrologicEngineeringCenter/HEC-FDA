using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Utilities.Serialization;
using Utilities;
using System.Linq;

namespace Statistics.Distributions
{
    internal class Uniform: IDistribution, IValidate<Uniform>
    {
        //TODO: Validation
        #region Fields and Properties
        private readonly MathNet.Numerics.Distributions.ContinuousUniform _Distribution;

        #region IDistribution Properties
        public IDistributionEnum Type => IDistributionEnum.Uniform;
        public double Mean => _Distribution.Mean;
        public double Median => _Distribution.Median;
        public double Variance => _Distribution.Variance;
        public double StandardDeviation => _Distribution.StdDev;
        public double Skewness => _Distribution.Skewness;
        public Utilities.IRange<double> Range { get; }
        public double Mode => _Distribution.Mode;
        public int SampleSize { get; }
        #endregion
        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }
        #endregion

        #region Constructor
        public Uniform(double min, double max, int sampleSize = int.MaxValue)
        {
            _Distribution = new MathNet.Numerics.Distributions.ContinuousUniform(lower: min, upper: max);
            Range = Utilities.IRangeFactory.Factory(_Distribution.Minimum, _Distribution.Maximum);
            SampleSize = sampleSize;
            State = Validate(new Validation.UniformValidator(), out IEnumerable<Utilities.IMessage> msgs);
            Messages = msgs;
        }
        #endregion

        #region Functions
        public IMessageLevels Validate(IValidator<Uniform> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }
        
        #region IDistribution Functions
        public double PDF(double x) => _Distribution.Density(x);
        public double CDF(double x) => _Distribution.CumulativeDistribution(x);
        public double InverseCDF(double p) => _Distribution.InverseCumulativeDistribution(p);

        //public double Sample(Random r = null) => InverseCDF(r == null ? new Random().NextDouble() : r.NextDouble());
        ////public double[] Sample(Random numberGenerator = null) => Sample(SampleSize, numberGenerator);
        //public double[] Sample(int sampleSize, Random numberGenerator = null)
        //{
        //    if (numberGenerator == null) numberGenerator = new Random();
        //    double[] sample = new double[sampleSize];
        //    for (int i = 0; i < sample.Length; i++) sample[i] = InverseCDF(numberGenerator.NextDouble());
        //    return sample;
        //}
        //public IDistribution SampleDistribution(Random numberGenerator = null) => Fit(Sample(SampleSize, numberGenerator));
        public string Print(bool round = false) => round ? Print(Range) : $"Uniform(range: {Range.Print()})";
        public string Requirements(bool printNotes) => RequiredParameterization(printNotes);
        public bool Equals(IDistribution distribution) => string.Compare(Print(), distribution.Print()) == 0 ? true : false;
        #endregion

        internal static string Print(IRange<double> range) => $"Uniform(range: {range.Print(true)})";
        internal static string RequiredParameterization(bool printNotes = false) => $"The Uniform distribution requires the following parameterization: {Parameterization()}.";
        internal static string Parameterization() => $"Uniform({Validation.Resources.DoubleRangeRequirements()})";

        public static Uniform Fit(IEnumerable<double> sample)
        {
            IData data = sample.IsNullOrEmpty() ? throw new ArgumentNullException(nameof(sample)) : IDataFactory.Factory(sample);
            if (!(data.State < IMessageLevels.Error) || data.Elements.Count() < 3) throw new ArgumentException($"The {nameof(sample)} is invalid because it contains an insufficient number of finite, numeric values (3 are required but only {data.Elements.Count()} were provided).");
            ISampleStatistics stats = ISampleStatisticsFactory.Factory(data);
            return new Uniform(stats.Range.Min, stats.Range.Max, stats.SampleSize);
        }

        public XElement WriteToXML()
        {
            XElement ordinateElem = new XElement(SerializationConstants.UNIFORM);
            //min
            ordinateElem.SetAttributeValue(SerializationConstants.MIN, Range.Min);
            //max
            ordinateElem.SetAttributeValue(SerializationConstants.MAX, Range.Max);
            //sample size
            ordinateElem.SetAttributeValue(SerializationConstants.SAMPLE_SIZE, SampleSize);

            return ordinateElem;
        }
        #endregion
    }
}
