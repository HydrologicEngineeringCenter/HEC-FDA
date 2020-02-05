using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MathNet.Numerics.Statistics;

using Utilities;

namespace Statistics
{
    public static class IDistributionFactory
    {
        //TODO: Validate
        //TODO: Call other constructors with inputs and IDistributions Enum (may require switch case on enum values) 
        
        public static string PrintParamaterizationRequirements(IDistributionEnum type)
        {
            switch (type) 
            {
                case IDistributionEnum.Histogram:
                    return Histograms.Histogram.RequiremedParameterization(true);
                case IDistributionEnum.Beta4Parameters:
                    return Distributions.Beta4Parameters.RequiredParameterization(true);
                case IDistributionEnum.LogPearsonIII:
                    return Distributions.LogPearsonIII.RequiredParameterization(true);
                case IDistributionEnum.Normal:
                    return Distributions.Normal.RequiredParameterization(true);
                case IDistributionEnum.Triangular:
                    return Distributions.Triangular.RequiredParameterization(true);
                case IDistributionEnum.Uniform:
                    return Distributions.Uniform.RequiredParameterization(true);
                case IDistributionEnum.TruncatedBeta4Parameter:
                case IDistributionEnum.TruncatedHistogram:
                case IDistributionEnum.TruncatedNormal:
                case IDistributionEnum.TruncatedTriangular:
                case IDistributionEnum.TruncatedUniform:
                    return Distributions.TruncatedDistribution.RequiredParameterization(true);
                case IDistributionEnum.NotSupported:
                default:
                    throw new NotImplementedException();
            }

        }

        /// <summary>
        /// Generates a parametric bootstrap sample, yielding a new <see cref="IDistribution"/> based on the specified <paramref name="distribution"/>.
        /// </summary>
        /// <param name="distribution"> The distribution to be sampled. </param>
        /// <param name="sampleProbabilities"> A set of random numbers on the range [0, 1] used by the <see cref="IDistribution.InverseCDF(double)"/> function to generate the bootstrap sample. 
        /// If set to the default <see langword="null"/> value a new <see cref="Random"/> number generator is created and a sample the length of the <see cref="IDistribution.SampleSize"/> is generated internally by the <see cref="Random.NextDouble"/> function. </param>
        /// <returns> A new <see cref="IDistribution"/> based on a random sample of values taken from the specified <paramref name="distribution"/>. </returns>
        public static IDistribution Sample(IDistribution distribution, IEnumerable<double> sampleProbabilities = null)
        {
            if (distribution.IsNull()) throw new ArgumentNullException(paramName: nameof(distribution));
            if (sampleProbabilities.IsNull()) sampleProbabilities = GenerateSampleProbabilities(distribution.SampleSize);
            if (sampleProbabilities.IsNullOrNonFiniteItem()) throw new ArgumentNullException(nameof(sampleProbabilities), "The specified or generated sample probabilities contain one or more null or non-finite numerical elements.");
            List<double> randoms = new List<double>();
            foreach (var p in sampleProbabilities) randoms.Add(distribution.InverseCDF(p));
            return Fit(randoms, distribution.Type);
        }
        private static IEnumerable<double> GenerateSampleProbabilities(int n)
        {
            Random rng = new Random();
            List<double> probs = new List<double>();
            for (int i = 0; i < n; i++) probs.Add(rng.NextDouble());
            return probs;
        }

        public static IDistribution Fit(IEnumerable<double> sample, IDistributionEnum returnType)
        {
            if ((int)returnType >= 10)
            {
                SummaryStatistics stats = new SummaryStatistics(IDataFactory.Factory(sample));
                return Fit(sample, stats.Range.Min, stats.Range.Max, returnType);
            }
            else
            {
                switch (returnType)
                {
                    case IDistributionEnum.Normal:
                        return Distributions.Normal.Fit(sample);
                    case IDistributionEnum.Uniform:
                        return Distributions.Uniform.Fit(sample);
                    case IDistributionEnum.Beta4Parameters:
                        return Distributions.Beta4Parameters.Fit(sample);
                    case IDistributionEnum.Triangular:
                        return Distributions.Triangular.Fit(sample);
                    case IDistributionEnum.Histogram:
                        return (IDistribution)IHistogramFactory.Factory(IDataFactory.Factory(sample), nBins: 100);
                    case IDistributionEnum.LogPearsonIII:
                        return Distributions.LogPearsonIII.Fit(sample);
                    default:
                        throw new NotImplementedException($"An unexpected error occured. The requested return type: {returnType} is unsupported");
                }
            }          
        }
        public static IHistogram Fit(IEnumerable<double> sample, int nBins)
        {
            return IHistogramFactory.Factory(IDataFactory.Factory(sample), nBins);
        }
        public static IDistribution Fit(IEnumerable<double> sample, double minimum, double maximum, IDistributionEnum returnType)
        {
            if ((int)returnType < 10)
            {
                return Fit(sample, returnType);
            }
            else
            {
                IDistribution distribution = (IDistribution)Fit(sample, (int)returnType / 10);
                return new Distributions.TruncatedDistribution(distribution, minimum, maximum);
            }
        }

        public static IDistribution FactoryNormal(double mean, double stDev, int sample = 2147483647)
        {
            return new Distributions.Normal(mean, stDev, sample);
        }
        public static IDistribution FactoryTriangular(double min, double mostLikely, double max, int sample = 2147483647)
        {
            return new Distributions.Triangular(min, mostLikely, max, sample);
        }
        public static IDistribution FactoryUniform(double min, double max, int sample = 2147483647)
        {
            return new Distributions.Uniform(min, max, sample);
        }
        public static IDistribution FactoryTruncatedNormal(double mean, double stDev, double min, double max, int sample = 2147483647)
        {
            IDistribution normal = new Distributions.Normal(mean, stDev, sample);
            return new Distributions.TruncatedDistribution(normal, min, max);
        }
        /// <summary>
        /// Constructs a scaled beta distribution.
        /// </summary>
        /// <param name="alpha"> Exponential shape parameter, must be positive, alpha > 0. </param>
        /// <param name="beta"> Exponential shape parameter, must be positive, beta > 0. </param>
        /// <param name="location"> The lower bound or minimum of the scaled distribution (e.g. shift from an unscaled distribution). </param>
        /// <param name="scale"> The range of the distribution (e.g. upper bound or maximum minus the lower bound or minimum), must be positive scale > 0. </param>
        /// <returns> A scaled beta distribution. </returns>
        public static IDistribution FactoryBeta(double alpha, double beta, double location, double scale)
        {
            return new Distributions.Beta4Parameters(alpha, beta, location, scale); 
        }
    }
}
