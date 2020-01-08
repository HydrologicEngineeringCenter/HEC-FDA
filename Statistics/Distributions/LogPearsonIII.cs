using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Utilities.Serialization;

namespace Statistics.Distributions
{
    internal class LogPearsonIII: IDistribution //IOrdinate<IDistribution>
    {
        //TODO: Validate
        //TODO: PDF, CDF Functions throw Exceptions

        #region Properties
        public IDistributions Type => IDistributions.LogPearsonIII;
        public double Mean { get; }
        public double Median { get; }
        public double Variance { get; }
        public double StandardDeviation { get; }
        public double Skewness { get; }
        public Utilities.IRange<double> Range { get; }
        //public double Minimum { get; }
        //public double Maximum { get; }
        public int SampleSize { get; }
        //#region IOrdinate Properties
        //public bool IsVariable => true;
        //public Type OrdinateType => typeof(IDistribution);
        //#endregion
        #endregion

        #region Constructor
        public LogPearsonIII(double mean, double standardDeviation, double skew, int sampleSize = int.MaxValue)
        {
            Mean = mean;
            Variance = Math.Pow(standardDeviation, 2);
            StandardDeviation = standardDeviation;
            Skewness = skew;
            SampleSize = sampleSize;
        }
        #endregion

        #region Functions
        #region IDistribution Functions
        public double PDF(double x)
        {
            throw new NotImplementedException();
        }
        public double CDF(double x)
        {
            throw new NotImplementedException();
        }
        public double InverseCDF(double p)
        {
            // this is totally based on Will's code in the statistics library
            Normal norm = new Normal(0, 1);
            double zScore = norm.InverseCDF(p);
            // k is a value described in Bulletin 17b
            double k = Math.Pow(2 / Skewness * ((zScore - Skewness / 6) * Skewness / 6 + 1), 2);
            // raise log x to 10th power
            return Math.Pow(10, Mean + k * StandardDeviation);
        }
        public double Sample(Random r = null) => InverseCDF(r == null ? new Random().NextDouble() : r.NextDouble());
        //public double[] Sample(Random numberGenerator) => Sample(SampleSize, numberGenerator);
        public double[] Sample(int sampleSize, Random numberGenerator)
        {
            if (numberGenerator == null) numberGenerator = new Random();
            double[] sample = new double[sampleSize];
            for (int i = 0; i < sample.Length; i++) sample[i] = InverseCDF(numberGenerator.NextDouble());
            return sample;
        }
        public IDistribution SampleDistribution(Random numberGenerator = null) => Fit(Sample(SampleSize, numberGenerator));
        #endregion
        #region IParameter Functions
        public double Value(double p = 0.5) => InverseCDF(p);
        public string Print() => $"LogPearsonIII(mean: {Mean}, sd: {StandardDeviation}, skew: {Skewness}, sample size: {SampleSize})";
        public bool Equals(IDistribution distribution) => string.Compare(Print(), distribution.Print()) == 0 ? true : false;
        #endregion
        //#region IOrdinate Functions
        //public bool Equals<T>(IOrdinate<T> ordinate) => ordinate.OrdinateType == typeof(IDistribution) ? Equals((IDistribution)ordinate) : false;
        //public double GetValue(double sampleProbability = 0.5) => InverseCDF(sampleProbability);
        //#endregion

        public static LogPearsonIII Fit(IEnumerable<double> sample)
        {
            List<double> log10Sample = new List<double>();
            foreach (double x in sample) log10Sample.Add(Math.Log(x));
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
