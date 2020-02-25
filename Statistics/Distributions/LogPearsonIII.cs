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
                Range = IRangeFactory.Factory(InverseCDF(0), InverseCDF(1));
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
        internal static string Print(double mean, double sd, double skew, int n) => $"log PearsonIII(mean: {mean.Print()}, sd: {sd.Print()}, skew: {skew.Print()}, sample size: {n.Print()})";
        internal static string RequiredParameterization(bool printNotes = true)
        {
            string s = $"The log PearsonIII distribution requires the following parameterization: {Parameterization()}.";
            if (printNotes) s += RequirementNotes();
            return s;
        }
        internal static string Parameterization() => $"log PearsonIII(mean: (0, {Math.Log10(double.MaxValue).Print()}], sd: (0, {Math.Log10(double.MaxValue).Print()}], skew: [{(Math.Log10(double.MaxValue) * - 1).Print()}, {Math.Log10(double.MaxValue).Print()}], sample size: > 0)";       
        internal static string RequirementNotes() => $"The distribution parameters are computed from log base 10 random numbers (e.g. the log Pearson III distribution is a distribution of log base 10 Pearson III distributed random values). Therefore the mean and standard deviation parameters must be positive finite numbers and while a large range of numbers are acceptable a relative small rate will produce meaningful results.";
        
        #region IDistribution Functions
        public double PDF(double x)
        {
            PearsonIII p3 = new PearsonIII(Mean, StandardDeviation, Skewness);
            // the last part of this is odd to me. Again, it is based on Will Lehman's Statistics assembly.
            return p3.PDF(Math.Log10(x)) / x / Math.Log(10); 
        }
        public double CDF(double x)
        {
            // if x <= 0 then p = 0.
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
        public bool Equals(IDistribution distribution) => string.Compare(Print(), distribution.Print()) == 0 ? true : false;
        #endregion

        public static LogPearsonIII Fit(IEnumerable<double> sample)
        {
            List<double> log10Sample = new List<double>();
            foreach (double x in sample) log10Sample.Add(Math.Log10(x));
            SummaryStatistics stats = new SummaryStatistics(IDataFactory.Factory(log10Sample));
            return new LogPearsonIII(stats.Mean, stats.StandardDeviation, stats.Skewness, stats.SampleSize);
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
