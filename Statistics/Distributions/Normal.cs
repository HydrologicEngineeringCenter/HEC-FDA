using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Utilities.Serialization;

namespace Statistics.Distributions
{
    internal class Normal: IDistribution //IOrdinate<IDistribution>
    {
        //TODO: Sample
        //TODO: Validation

        #region Fields and Properties
        private readonly MathNet.Numerics.Distributions.Normal _Distribution;
        #region IDistribution Properties
        public IDistributions Type => IDistributions.Normal;
        public double Mean => _Distribution.Mean;
        public double Median => _Distribution.Median;
        public double Mode => _Distribution.Mode;
        public double Variance => _Distribution.Variance;
        public double StandardDeviation => _Distribution.StdDev;
        public double Skewness => _Distribution.Skewness;
        public double Minimum => _Distribution.Minimum;
        public double Maximum => _Distribution.Maximum;

        public int SampleSize { get; }
        #endregion
        //#region IOrdinate Properties
        //public bool IsVariable => true;
        //public Type OrdinateType => typeof(IDistribution);
        //#endregion
        #endregion

        #region Constructor
        public Normal(double mean, double sd, int sampleSize = int.MaxValue)
        {
            _Distribution = new MathNet.Numerics.Distributions.Normal(mean, stddev: sd);
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
        public string Print() => $"Normal(mean: {Mean}, sd: {StandardDeviation}, sample size: {SampleSize})";
        public bool Equals(IDistribution distribution) => string.Compare(Print(), distribution.Print()) == 0 ? true : false;
        #endregion
        //#region Iordinate Functions
        //public double GetValue(double sampleProbability) => InverseCDF(sampleProbability);
        //public bool Equals<T>(IOrdinate<T> ordinate) => ordinate.OrdinateType == typeof(IDistribution) ? Equals((IDistribution)ordinate) : false;
        //#endregion

        public static Normal Fit(IEnumerable<double> sample)
        {
            MathNet.Numerics.Distributions.Normal norm = MathNet.Numerics.Distributions.Normal.Estimate(sample);
            return new Normal(norm.Mean, norm.StdDev, sample.Count());
        }

        public XElement WriteToXML()
        {
            XElement ordinateElem = new XElement(SerializationConstants.NORMAL);
            //mean
            ordinateElem.SetAttributeValue(SerializationConstants.MEAN, Mean);
            //st dev
            ordinateElem.SetAttributeValue(SerializationConstants.ST_DEV, StandardDeviation);
            //sample size
            ordinateElem.SetAttributeValue(SerializationConstants.SAMPLE_SIZE, SampleSize);

            return ordinateElem;
        }

        
        #endregion
    }
}
