using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Utilities;
using Utilities.Serialization;

namespace Statistics.Distributions
{
    internal class Triangular: IDistribution, IValidate<Triangular> 
    {
        //TODO: Sample
        #region Fields and Properties
        private readonly MathNet.Numerics.Distributions.Triangular _Distribution;

        #region IDistribution Properties
        public IDistributionEnum Type => IDistributionEnum.Triangular;
        public double Mean => _Distribution.Mean;
        public double Median => _Distribution.Median;
        public double Variance => _Distribution.Variance;
        public double StandardDeviation => _Distribution.StdDev;
        public double Skewness => _Distribution.Skewness;
        public IRange<double> Range { get; }
        public int SampleSize { get; }
        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }
        public double Mode => _Distribution.Mode;
        #endregion
        #endregion

        #region Constructor
        public Triangular(double min, double mode, double max, int sampleSize = int.MaxValue)
        {
            IRange<double> range = IRangeFactory.Factory(min, max,true, true, true, false);
            if (!Validation.TriangularValidator.IsConstructable(mode, range, out string error)) throw new InvalidConstructorArgumentsException(error);
            else
            {
                _Distribution = new MathNet.Numerics.Distributions.Triangular(lower: min, upper: max, mode: mode);
                Range = range;
                SampleSize = sampleSize;
                State = Validate(new Validation.TriangularValidator(), out IEnumerable<IMessage> msgs);
                Messages = msgs;
            }
        }
        #endregion

        #region Functions
        public IMessageLevels Validate(IValidator<Triangular> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }
        internal static string Print(double mode, IRange<double> range) => $"Triangular(mode: {mode.Print()}, range: [{range.Min.Print()}, {range.Max.Print()}])";
        internal static string RequiredParameterization(bool printNotes)
        {
            string s = $"The Triangular distribution requires the following parameterization: {Parameterization()}.";
            if (printNotes) s += RequirementNotes();
            return s;
        }
        internal static string Parameterization() => $"Triangular(mode: range minimum \u2264 mode \u2264 range maximum, {Validation.Resources.DoubleRangeRequirements()}, sample size: > 0)";
        internal static string RequirementNotes() => "The mode parameter is also sometimes referred to as the most likely value.";
        
        #region IDistribution Functions
        public double PDF(double x) => _Distribution.Density(x);
        public double CDF(double x) =>  _Distribution.CumulativeDistribution(x);
        public double InverseCDF(double p) => p.IsOnRange(0,1) ? _Distribution.InverseCumulativeDistribution(p): throw new ArgumentOutOfRangeException($"The specified probability parameter, p: {p} is invalid because it it not on the valid range [0, 1].");

        //public double Sample(Random r = null) => InverseCDF(r == null ? new Random().NextDouble() : r.NextDouble());
        ////public double[] Sample(Random numberGenerator = null) => Sample(SampleSize, numberGenerator);
        //public double[] Sample(int sampleSize, Random numberGenerator = null)
        //{
        //    if (numberGenerator == null) numberGenerator = new Random();
        //    double[] sample = new double[SampleSize];
        //    for (int i = 0; i < sample.Length; i++) sample[i] = InverseCDF(numberGenerator.NextDouble());
        //    return sample;
        //}
        //public IDistribution SampleDistribution(Random numberGenerator = null) => Fit(Sample(SampleSize, numberGenerator));
        public string Print(bool round = false) => round ? Print(_Distribution.Mode, Range) : $"Triangular(mode: {_Distribution.Mode}, range: {Range.Print()}, sample size: {SampleSize})";
        public string Requirements(bool printNotes) => RequiredParameterization(printNotes);
        public bool Equals(IDistribution distribution) => string.Compare(Print(), distribution.Print()) == 0 ? true : false;
        #endregion

        //internal static string Print(double mode, IRange<double> range) => $"Triangular(mode: {mode.Print()}, range: [{range.Min.Print()}, {range.Max.Print()}])";
        //internal static string RequiredParameterization(bool printNotes)
        //{
        //    string s = $"The Triangular distribution requires the following parameterization: {Parameterization()}.";
        //    if (printNotes) s += RequirementNotes();
        //    return s;
        //}
       // internal static string Parameterization() => $"Triangular(mode: range minimum \u2264 mode \u2264 range maximum, {Validation.Resources.DoubleRangeRequirements()}, sample size: > 0)";
       // internal static string RequirementNotes() => "The mode parameter is also sometimes referred to as the mostly likely value.";

        public static Triangular Fit(IEnumerable<double> sample)
        {
            IData data = sample.IsNullOrEmpty() ? throw new ArgumentNullException(nameof(sample)) : IDataFactory.Factory(sample);
            if (!(data.State < IMessageLevels.Error) || data.Elements.Count() < 3) throw new ArgumentException($"The {nameof(sample)} is invalid because it contains an insufficient number of finite, numeric values (3 are required but only {data.Elements.Count()} were provided).");
            ISampleStatistics stats = ISampleStatisticsFactory.Factory(data);
            return new Triangular(stats.Range.Min, stats.Mean, stats.Range.Max, stats.SampleSize);
        }

        public XElement WriteToXML()
        {
            XElement ordinateElem = new XElement(SerializationConstants.TRIANGULAR);
            //min
            ordinateElem.SetAttributeValue(SerializationConstants.MIN, Range.Min);
            //most likely
            ordinateElem.SetAttributeValue(SerializationConstants.MODE, Mode);
            //max
            ordinateElem.SetAttributeValue(SerializationConstants.MAX, Range.Max);
            //sample size
            ordinateElem.SetAttributeValue(SerializationConstants.SAMPLE_SIZE, SampleSize);

            return ordinateElem;
        }
        #endregion
    }
}
