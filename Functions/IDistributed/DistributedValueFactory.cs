using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    public static class DistributedValueFactory
    {
        public static IDistributedValue Factory(Statistics.IDistribution distribution)
        {
            return new DistributedValue(distribution);
        }

        public static IDistributedValue FactoryNormal(double mean, double stDev, int sampleSize = 2147483647)
        {
            return new DistributedValue(Statistics.IDistributionFactory.FactoryNormal(mean, stDev, sampleSize));
        }

        public static IDistributedValue FactoryTruncatedNormal(double mean, double stDev, double min, double max, int sampleSize = 2147483647)
        {
            return new DistributedValue(Statistics.IDistributionFactory.FactoryTruncatedNormal(mean, stDev, min, max, sampleSize));
        }

        public static IDistributedValue FactoryUniform(double min, double max, int sampleSize = 2147483647)
        {
            return new DistributedValue(Statistics.IDistributionFactory.FactoryUniform(min, max, sampleSize));
        }

        public static IDistributedValue FactoryTriangular(double mostLikely, double min, double max, int sampleSize = 2147483647)
        {
            return new DistributedValue(Statistics.IDistributionFactory.FactoryTriangular(min, mostLikely, max, sampleSize));
        }

        public static IDistributedValue FactoryBeta(double alpha, double beta, double min, double max)
        {
            return new DistributedValue(Statistics.IDistributionFactory.FactoryBeta(alpha, beta, min, max));
        }
    }
}
