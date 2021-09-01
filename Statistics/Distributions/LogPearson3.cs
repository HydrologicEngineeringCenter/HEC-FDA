using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Utilities;
using Utilities.Serialization;

namespace Statistics.Distributions
{
    internal class LogPearson3: IDistribution, IValidate<LogPearson3> 
    {
        internal readonly PearsonIII _Distribution;
        internal readonly IRange<double> _ProbabilityRange; 
        
        #region Properties
        public IDistributionEnum Type => IDistributionEnum.LogPearsonIII;
        public double Mean { get; }
        public double Median { get; }
        public double Variance { get; }
        public double StandardDeviation { get; }
        public double Skewness { get; }
        public Utilities.IRange<double> Range { get; }
        public int SampleSize { get; }
        public IMessageLevels State { get; }
        public IEnumerable<Utilities.IMessage> Messages { get; }

        public double Mode => throw new NotImplementedException();
        #endregion

        #region Constructor
        public LogPearson3(double mean, double standardDeviation, double skew, int sampleSize = int.MaxValue)
        {
            if (!Validation.LogPearson3Validator.IsConstructable(mean, standardDeviation, skew, sampleSize, out string error)) throw new Utilities.InvalidConstructorArgumentsException(error);
            else
            {
                _Distribution = new PearsonIII(mean, standardDeviation, skew, sampleSize);
                Mean = _Distribution.Mean;
                Skewness = _Distribution.Skewness;
                SampleSize = _Distribution.SampleSize;
                StandardDeviation = _Distribution.StandardDeviation;
                Variance = Math.Pow(standardDeviation, 2);
                Median = InverseCDF(0.50);
                _ProbabilityRange = FiniteRange(); 
                Range = IRangeFactory.Factory(InverseCDF(_ProbabilityRange.Min), InverseCDF(_ProbabilityRange.Max)); 
                State = Validate(new Validation.LogPearson3Validator(), out IEnumerable<Utilities.IMessage> msgs);
                Messages = msgs;
            }
        }
        #endregion

        #region Functions
        public IMessageLevels Validate(IValidator<LogPearson3> validator, out IEnumerable<Utilities.IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }

        private IRange<double> FiniteRange()
        {
            double min = InverseCDF(0.0000001), max = InverseCDF(0.9999999), p = 0.0000001, epsilon = 1 / 1000000d;
            while (!min.IsFinite() || !max.IsFinite())
            {
                p += epsilon;
                if (!min.IsFinite()) min = InverseCDF(p);
                if (!max.IsFinite()) max = InverseCDF(1 - p);
                if (p > 0.25) 
                    throw new InvalidConstructorArgumentsException($"The log Pearson III object is not constructable because 50% or more of its distribution returns {double.NegativeInfinity} and {double.PositiveInfinity}.");
            } 
            return IRangeFactory.Factory(p, 1d - p);
        }
        #region IDistribution Functions
        public double PDF(double x)
        {
            if (x < Range.Min || x > Range.Max) return double.Epsilon;
            else
            {
                return _Distribution.PDF(Math.Log10(x))/x/Math.Log(10);

            }          
        }
        public double CDF(double x)
        {
            if (x < Range.Min) return 0;
            if (x == Range.Min) return _ProbabilityRange.Min;
            if (x == Range.Max) return _ProbabilityRange.Max;
            if (x > Range.Max) return 1;
            if (x > 0)
            {
                return _Distribution.CDF(Math.Log10(x));
            }
            else return 0;
        }
        public double InverseCDF(double p)
        {
            
            if (!p.IsFinite()) 
            {
                throw new ArgumentException($"The value of specified probability parameter: {p} is invalid because it is not on the valid probability range: [0, 1].");
            }
            else // Range has been set check p against [_ProbabilityRange.Min, _ProbabilityRange.Max]
            {
                if (Range.IsNull()) // object is being constructued
                { 
                    if (p < 0 || p > 1) throw new ArgumentException($"The value of specified probability parameter: {p} is invalid because it is not on the valid probability range: [0, 1].");
                }
                else
                {
                    if (p <= _ProbabilityRange.Min) return Range.Min;
                    if (p >= _ProbabilityRange.Max) return Range.Max;
                }
            }
            return Math.Pow(10, _Distribution.InverseCDF(p));
        }
        public string Print(bool round = false) => round ? Print(Mean, StandardDeviation, Skewness, SampleSize) : $"log PearsonIII(mean: {Mean}, sd: {StandardDeviation}, skew: {Skewness}, sample size: {SampleSize})";
        public string Requirements(bool printNotes) => RequiredParameterization(printNotes);
        public bool Equals(IDistribution distribution) => string.Compare(Print(), distribution.Print(), StringComparison.InvariantCultureIgnoreCase) == 0 ? true : false;
        #endregion

        internal static string Print(double mean, double sd, double skew, int n) => $"log PearsonIII(mean: {mean.Print()}, sd: {sd.Print()}, skew: {skew.Print()}, sample size: {n.Print()})";
        internal static string RequiredParameterization(bool printNotes = true)
        {
            string s = $"The log PearsonIII distribution requires the following parameterization: {Parameterization()}.";
            if (printNotes) s += RequirementNotes();
            return s;
        }
        internal static string Parameterization() => $"log PearsonIII(mean: (0, {Math.Log10(double.MaxValue).Print()}], sd: (0, {Math.Log10(double.MaxValue).Print()}], skew: [{(Math.Log10(double.MaxValue) * -1).Print()}, {Math.Log10(double.MaxValue).Print()}], sample size: > 0)";
        internal static string RequirementNotes() => $"The distribution parameters are computed from log base 10 random numbers (e.g. the log Pearson III distribution is a distribution of log base 10 Pearson III distributed random values). Therefore the mean and standard deviation parameters must be positive finite numbers and while a large range of numbers are acceptable a relative small rate will produce meaningful results.";

        public static LogPearson3 Fit(IEnumerable<double> sample, bool isLogSample = false)
        {
            return Fit(sample, isLogSample);
        }
        public static LogPearson3 Fit(IEnumerable<double> sample, int sampleSize, bool isLogSample = false)
        {
            return Fit(sample, isLogSample, sampleSize);
        }
        private static LogPearson3 Fit(IEnumerable<double> sample, bool isLogSample = false, int sampleSize = -404)
        {
            List<double> logSample = new List<double>();
            if (!isLogSample) foreach (double x in sample) logSample.Add(Math.Log10(x));
            IData data = sample.IsNullOrEmpty() ? throw new ArgumentNullException(nameof(sample)) : isLogSample ? IDataFactory.Factory(sample) : IDataFactory.Factory(logSample);
            if (!(data.State < IMessageLevels.Error) || data.Elements.Count() < 3) throw new ArgumentException($"The {nameof(sample)} is invalid because it contains an insufficient number of finite, numeric values (3 are required but only {data.Elements.Count()} were provided).");
            ISampleStatistics stats = ISampleStatisticsFactory.Factory(data);
            return new LogPearson3(stats.Mean, stats.StandardDeviation, stats.Skewness, sampleSize == -404 ? stats.SampleSize : sampleSize);
        }

        public string WriteToXML()
        {
            return $"{Mean}, {StandardDeviation}, {Skewness}, {SampleSize}";
        }
        XElement ISerializeToXML<IDistribution>.WriteToXML()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
