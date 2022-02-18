using System;
using System.Collections.Generic;
using System.Linq;


namespace Statistics
{
    /// <summary>
    /// Provide factory methods for the construction of objects implementing the <see cref="IDistribution"/> interface.
    /// </summary>
    public static class IDistributionFactory
    {
        //TODO: Validate
        //TODO: Call other constructors with inputs and IDistributions Enum (may require switch case on enum values) 
        
        /// <summary>
        /// Prints a <see cref="string"/> describing the parameterization requirement for the specified <see cref="IDistributionEnum"/>.
        /// </summary>
        /// <param name="type"> The type of distribution to analyze. </param>
        /// <returns> A <see cref="string"/> description. </returns>
        public static string PrintParamaterizationRequirements(IDistributionEnum type)
        {
            switch (type) 
            {
                case IDistributionEnum.Histogram:
                    throw new NotImplementedException("Parametrization is not implemented for histograms");
                case IDistributionEnum.LogPearsonIII:
                    return Distributions.LogPearson3.RequiredParameterization(true);
                case IDistributionEnum.Normal:
                    return Distributions.Normal.RequiredParameterization(true);
                case IDistributionEnum.Triangular:
                    return Distributions.Triangular.RequiredParameterization(true);
                case IDistributionEnum.Uniform:
                    return Distributions.Uniform.RequiredParameterization(true);
                case IDistributionEnum.TruncatedHistogram:
                case IDistributionEnum.TruncatedNormal:
                    //return Distributions.TruncatedDistribution.RequiredParameterization(true);
                case IDistributionEnum.NotSupported:
                default:
                    throw new NotImplementedException();
            }
        }

        internal static Statistics.Histograms.Histogram Fit(IEnumerable<double> sample, int nBins)
        {
            double min = sample.Min();
            double max = sample.Max();
            double binWidth = (min - max) / nBins;
            Statistics.Histograms.Histogram histogram = new Statistics.Histograms.Histogram(binWidth);
            histogram.AddObservationsToHistogram(sample.ToArray());
            return histogram;
        }
        /// <summary>
        /// Constructs a <see cref="IDistributionEnum.Normal"/> distribution.
        /// </summary>
        /// <param name="mean"> The mean of the distribution. </param>
        /// <param name="stDev"> The standard deviation of the distribution. </param>
        /// <param name="sampleSize"> An optional sample size parameter. If a population rather than sample distribution is intended leave this parameter blank. Set to <see cref="int.MaxValue"/> by default. </param>
        /// <returns> A new <see cref="IDistribution"/>. </returns>
        public static IDistribution FactoryNormal(double mean, double stDev, int sampleSize = int.MaxValue)
        {
            return new Distributions.Normal(mean, stDev, sampleSize);
        }
        /// <summary>
        /// Constructs a <see cref="IDistributionEnum.Triangular"/> distribution.
        /// </summary>
        /// <param name="min"> The minimum value of the distribution. </param>
        /// <param name="mostLikely"> The most likely value of the distribution. </param>
        /// <param name="max"> The maximum value of the distribution. </param>
        /// <param name="sampleSize"> An optional sample size parameter. If a population rather than sample distribution is intended leave this parameter blank. Set to <see cref="int.MaxValue"/> by default. </param>
        /// <returns> A new <see cref="IDistribution"/>. </returns>
        public static IDistribution FactoryTriangular(double min, double mostLikely, double max, int sampleSize = int.MaxValue)
        {
            return new Distributions.Triangular(min, mostLikely, max, sampleSize);
        }
        /// <summary>
        /// Constructs a <see cref="IDistributionEnum.Uniform"/> distribution.
        /// </summary>
        /// <param name="min"> The minimum value of the distribution. </param>
        /// <param name="max"> The maximum value of the distribution. </param>
        /// <param name="sampleSize"> An optional sample size parameter. If a population rather than sample distribution is intended leave this parameter blank. Set to <see cref="int.MaxValue"/> by default. </param>
        /// <returns> A new <see cref="IDistribution"/>. </returns>
        public static IDistribution FactoryUniform(double min, double max, int sampleSize = int.MaxValue)
        {
            return new Distributions.Uniform(min, max, sampleSize);
        }
        /// <summary>
        /// Constructs a Truncated <see cref="IDistributionEnum.Normal"/> distribution.
        /// </summary>
        /// <param name="mean"> The mean of the distribution. </param>
        /// <param name="stDev"> The standard deviation of the distribution. </param>
        /// <param name="min"> The lower (minimum) truncation value. </param>
        /// <param name="max"> The upper (maximum) truncation value. </param>
        /// <param name="sampleSize"> An optional sample size parameter. If a population rather than sample distribution is intended leave this parameter blank. Set to <see cref="int.MaxValue"/> by default. </param>
        /// <returns> A new <see cref="IDistribution"/>. </returns>
        public static IDistribution FactoryTruncatedNormal(double mean, double stDev, double min, double max, int sampleSize = int.MaxValue)
        {
            IDistribution normal = new Distributions.TruncatedNormal(mean, stDev, min, max, sampleSize);
            return normal;
        }
        /// <summary>
        /// Constructs a <see cref="IDistributionEnum.LogPearsonIII"/> <see cref="IDistribution"/>
        /// </summary>
        /// <param name="mean"> The mean of the logged distribution (or sample data). NOTE: this is the mean of the logged data, NOT the log of the mean. </param>
        /// <param name="stDev"> The standard deviation of the logged distribution (or sample data). NOTE: this is the standard deviation of the logged data, NOT the log of the standard deviation. </param>
        /// <param name="skew"> The skew of the logged distribution (or sample data). NOTE: this is the skew of the logged data, NOT the log of the skew.  </param>
        /// <param name="sampleSize"> An optional parameter describing the sample size used to calculated the sample <paramref name="mean"/>, <paramref name="stDev"/> and <paramref name="skew"/>. Leave blank if a population distribution is intended. </param>
        /// <returns> A <see cref="Statistics.Distributions.LogPearson3"/> object returned as an implementation of the  <see cref="IDistribution"/> interface. </returns>
        public static IDistribution FactoryLogPearsonIII(double mean, double stDev, double skew, int sampleSize = int.MaxValue)
        {
            return new Distributions.LogPearson3(mean, stDev, skew, sampleSize);
        }
        /// <summary>
        /// Constructs a <see cref="IDistributionEnum.LogPearsonIII"/> <see cref="IDistribution"/> bound on the range specified by the <paramref name="min"/> and <paramref name="max"/> values."/>
        /// </summary>
        /// <param name="mean"> The mean of the logged distribution (or sample data). NOTE: this is the mean of the logged data, NOT the log of the mean. </param>
        /// <param name="stDev"> The standard deviation of the logged distribution (or sample data). NOTE: this is the standard deviation of the logged data, NOT the log of the standard deviation. </param>
        /// <param name="skew"> The skew of the logged distribution (or sample data). NOTE: this is the skew of the logged data, NOT the log of the skew.  </param>
        /// <param name="sampleSize"> An optional parameter describing the sample size used to calculated the sample <paramref name="mean"/>, <paramref name="stDev"/> and <paramref name="skew"/>. Leave blank if a population distribution is intended. </param>
        /// <param name="min"> The lower (minimum) truncation value. </param>
        /// <param name="max"> The upper (maximum) truncation value. </param>
        /// <returns> A <see cref="Statistics.Distributions.LogPearson3"/> object bound on the range: [<paramref name="min"/>, <paramref name="max"/>] returned as an implementation of the  <see cref="IDistribution"/> interface. </returns>
        /// <returns></returns>
        public static IDistribution FactoryTruncatedLogPearsonIII(double mean, double stDev, double skew, double min = 0, double max = double.PositiveInfinity, int sampleSize = int.MaxValue)
        {
            IDistribution lpIII = new Distributions.LogPearson3(mean, stDev, skew, sampleSize);
            return FactoryTruncatedLogPearsonIII(lpIII, min, max);
        }
        /// <summary>
        /// Constructs a <see cref="IDistributionEnum.LogPearsonIII"/> <see cref="IDistribution"/> bound on the range specified by the <paramref name="min"/> and <paramref name="max"/> values."/>
        /// </summary>
        /// <param name="lpIII"> The <see cref="Statistics.Distributions.LogPearson3"/> distribution to be bound. </param>
        /// <param name="min"> The lower (minimum) truncation value. </param>
        /// <param name="max"> The upper (maximum) truncation value. </param>
        /// <returns> A <see cref="Statistics.Distributions.LogPearson3"/> object bound on the range: [<paramref name="min"/>, <paramref name="max"/>] returned as an implementation of the  <see cref="IDistribution"/> interface. </returns>
        public static IDistribution FactoryTruncatedLogPearsonIII(IDistribution lpIII, double min = 0, double max = double.PositiveInfinity)
        {
            if (lpIII==null) throw new ArgumentNullException(nameof(lpIII));
            if (lpIII.Type != IDistributionEnum.LogPearsonIII) throw new ArgumentException($"The {nameof(FactoryTruncatedLogPearsonIII)} factory requires a {nameof(IDistributionEnum.LogPearsonIII)} {nameof(lpIII)} parameter, instead a {nameof(lpIII.Type)} was provided.");
            Statistics.Distributions.LogPearson3 ldist = (Statistics.Distributions.LogPearson3)lpIII;
            return new Statistics.Distributions.TruncatedLogPearson3(ldist.Mean,ldist.StandardDeviation,ldist.Skewness, min, max, lpIII.SampleSize);
        }
        /// <summary>
        /// Constructs a <see cref="IDistribution"/> bound on the range specified by the <paramref name="min"/> and <paramref name="max"/> values."/>
        /// </summary>
        /// <param name="distribution"> The <see cref="IDistribution"/> to be bound. </param>
        /// <param name="min"> The lower (minimum) truncation value. </param>
        /// <param name="max"> The upper (maximum) truncation value. </param>
        /// <returns> A <see cref="IDistribution"/> bound on the range: [<paramref name="min"/>, <paramref name="max"/>] returned as an implementation of the  <see cref="IDistribution"/> interface. </returns>
        public static IDistribution FactoryTruncatedDistribution(IDistribution distribution, double min = double.NegativeInfinity, double max = double.PositiveInfinity)
        {
            if (distribution == null) throw new ArgumentNullException(nameof(distribution));
            switch (distribution.Type) 
            {
                case IDistributionEnum.Normal:
                    Statistics.Distributions.Normal ndist = (Statistics.Distributions.Normal)distribution;
                    return FactoryTruncatedNormal(ndist.Mean, ndist.StandardDeviation, min, max, distribution.SampleSize);
                case IDistributionEnum.LogPearsonIII:
                    return FactoryTruncatedLogPearsonIII(distribution, min, max);
                default:
                    throw new NotSupportedException($"The specified {distribution.Type} distribution is not a supported truncated distribution type.");
            }

        }
    }
}
