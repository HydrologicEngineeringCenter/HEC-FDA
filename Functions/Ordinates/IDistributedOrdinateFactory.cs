using System;
using System.Collections.Generic;
using System.Text;
using Functions.Ordinates;

namespace Functions
{
    /// <summary>
    /// Provides a factory for the construction of objects implementing the <see cref="IDistributedOrdinate"/> interface.
    /// </summary>
    public static class IDistributedOrdinateFactory
    {
        /// <summary>
        /// Constructs a new <see cref="IDistributedOrdinate"/> from the provided <see cref="Statistics.IDistribution"/>.
        /// </summary>
        /// <param name="distribution"> The distribution incorporated. </param>
        /// <returns> A new object implementing the <see cref="IDistributedOrdinate"/> interface. </returns>
        public static IDistributedOrdinate Factory(Statistics.IDistribution distribution)
        {
            return new Distribution(distribution);
        }
        /// <summary>
        /// Constructs a new <see cref="IOrdinateEnum.Normal"/> <see cref="IDistributedOrdinate"/> from the specified parameter values.
        /// </summary>
        /// <param name="mean"> The distribution mean. </param>
        /// <param name="stDev"> The distribution standard deviation. </param>
        /// <param name="sampleSize"> An optional parameter describing the distribution sample size. Leave this parameter blank if a population distribution is intended.</param>
        /// <returns> An object implementing the <see cref="IDistributedOrdinate"/> interface. </returns>
        public static IDistributedOrdinate FactoryNormal(double mean, double stDev, int sampleSize = int.MaxValue)
        {
            return new Distribution(Statistics.IDistributionFactory.FactoryNormal(mean, stDev, sampleSize));
        }
        /// <summary>
        /// Constructs a new <see cref="IOrdinateEnum.TruncatedNormal"/> <see cref="IDistributedOrdinate"/> from the specified parameter values.
        /// </summary>
        /// <param name="mean"> The distribution mean. </param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="sampleSize"> An optional parameter describing the distribution sample size. Leave this parameter blank if a population distribution is intended.</param>
        public static IDistributedOrdinate FactoryTruncatedNormal(double mean, double stDev, double min, double max, int sampleSize = int.MaxValue)
        {
            return new Distribution(Statistics.IDistributionFactory.FactoryTruncatedNormal(mean, stDev, min, max, sampleSize));
        }

        public static IDistributedOrdinate FactoryUniform(double min, double max, int sampleSize = 2147483647)
        {
            return new Distribution(Statistics.IDistributionFactory.FactoryUniform(min, max, sampleSize));
        }

        public static IDistributedOrdinate FactoryTriangular(double mostLikely, double min, double max, int sampleSize = 2147483647)
        {
            return new Distribution(Statistics.IDistributionFactory.FactoryTriangular(min, mostLikely, max, sampleSize));
        }

        public static IDistributedOrdinate FactoryBeta(double alpha, double beta, double min, double max)
        {
            return new Distribution(Statistics.IDistributionFactory.FactoryBeta(alpha, beta, min, max));
        }
    }
}
