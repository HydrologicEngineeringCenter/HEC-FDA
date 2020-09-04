using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Utilities;
using Utilities.Serialization;

namespace Statistics.Distributions
{
    internal class LogPearsonIII: IDistribution, IValidate<LogPearsonIII> 
    {
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
        public LogPearsonIII(double mean, double standardDeviation, double skew, int sampleSize = int.MaxValue)
        {
            if (!Validation.LogPearsonIIIValidator.IsConstructable(mean, standardDeviation, skew, sampleSize, out string error)) throw new Utilities.InvalidConstructorArgumentsException(error);
            else
            {
                Mean = mean;
                Skewness = skew;
                SampleSize = sampleSize;
                StandardDeviation = standardDeviation;
                Variance = Math.Pow(standardDeviation, 2);
                Median = InverseCDF(0.50);
                _ProbabilityRange = FiniteRange(); 
                Range = IRangeFactory.Factory(InverseCDF(_ProbabilityRange.Min), InverseCDF(_ProbabilityRange.Max)); 
                State = Validate(new Validation.LogPearsonIIIValidator(), out IEnumerable<Utilities.IMessage> msgs);
                Messages = msgs;
            }
        }
        #endregion

        #region Functions
        public IMessageLevels Validate(IValidator<LogPearsonIII> validator, out IEnumerable<Utilities.IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }

        private IRange<double> FiniteRange()
        {
            double min = double.NegativeInfinity, max = double.PositiveInfinity, p = 0, epsilon = 1 / 1000000000d;
            while (!(min.IsFinite() && max.IsFinite()))
            {
                p += epsilon;
                if (!min.IsFinite()) min = InverseCDF(p);
                if (!max.IsFinite()) max = InverseCDF(1 - p);
            } 
            return IRangeFactory.Factory(p, 1 - p);
        }


        #region IDistribution Functions
        public double PDF(double x)
        {
            if (x < Range.Min || x > Range.Max) return double.Epsilon;
            else
            {
                PearsonIII p3 = new PearsonIII(Mean, StandardDeviation, Skewness);
                // the last part of this is odd to me. Again, it is based on Will Lehman's Statistics assembly.
                return p3.PDF(Math.Log10(x)) / x / Math.Log(10);
            }          
        }
        public double CDF(double x)
        {
            if (x < Range.Min) return 0;
            if (x == Range.Min) return _ProbabilityRange.Min;
            if (x == Range.Max) return _ProbabilityRange.Max;
            if (x > Range.Max) return 1;
            double p = 0;
            if (x > 0)
            {               
                PearsonIII p3 = new PearsonIII(mean: Mean, sd: StandardDeviation, skew: Skewness);
                // x is not the logged value (this seems strange to me).
                p = p3.CDF(Math.Log10(x));
            }
            return p;
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
            
            // this is totally based on Will's code in the statistics library
            Normal norm = new Normal(0, 1);
            double zScore = norm.InverseCDF(p);
            // k is a value described in Bulletin 17b
            double k = (2 / Skewness) * (Math.Pow((zScore - Skewness / 6) * Skewness / 6 + 1, 3) - 1);
            // log(base10)x  = Mean + k * StandardDeviation 
            return Math.Pow(10, Mean + k * StandardDeviation);
            // so 10 ^ log(base10)x undoes the logging and is the x value we are seeking.
        }
        //public double Sample(Random r = null) => InverseCDF(r == null ? new Random().NextDouble() : r.NextDouble());
        ////public double[] Sample(Random numberGenerator) => Sample(SampleSize, numberGenerator);
        //public double[] Sample(int sampleSize, Random numberGenerator)
        //{
        //    if (numberGenerator == null) numberGenerator = new Random();
        //    double[] sample = new double[sampleSize];
        //    for (int i = 0; i < sample.Length; i++) sample[i] = InverseCDF(numberGenerator.NextDouble());
        //    return sample;
        //}
        //public IDistribution SampleDistribution(Random numberGenerator = null) => Fit(Sample(SampleSize, numberGenerator));
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

        public static LogPearsonIII Fit(IEnumerable<double> sample, bool isLogSample = false)
        {
            return Fit(sample, isLogSample);
        }
        public static LogPearsonIII Fit(IEnumerable<double> sample, int sampleSize, bool isLogSample = false)
        {
            return Fit(sample, isLogSample, sampleSize);
        }
        private static LogPearsonIII Fit(IEnumerable<double> sample, bool isLogSample = false, int sampleSize = -404)
        {
            List<double> logSample = new List<double>();
            if (!isLogSample) foreach (double x in sample) logSample.Add(Math.Log10(x));
            IData data = sample.IsNullOrEmpty() ? throw new ArgumentNullException(nameof(sample)) : isLogSample ? IDataFactory.Factory(sample) : IDataFactory.Factory(logSample);
            if (!(data.State < IMessageLevels.Error) || data.Elements.Count() < 3) throw new ArgumentException($"The {nameof(sample)} is invalid because it contains an insufficient number of finite, numeric values (3 are required but only {data.Elements.Count()} were provided).");
            ISampleStatistics stats = ISampleStatisticsFactory.Factory(data);
            return new LogPearsonIII(stats.Mean, stats.StandardDeviation, stats.Skewness, sampleSize == -404 ? stats.SampleSize : sampleSize);
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
