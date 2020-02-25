using System;
using System.Collections.Generic;
using System.Text;
using Functions.Ordinates;

namespace Functions
{
    public static class IDistributedOrdinateFactory
    {
        public static IDistributedOrdinate Factory(Statistics.IDistribution distribution)
        {
            return new Distribution(distribution);
        }

      
        public static IDistributedOrdinate FactoryNormal(double mean, double stDev, int sampleSize = 2147483647)
        {
            return new Distribution(Statistics.IDistributionFactory.FactoryNormal(mean, stDev, sampleSize));
        }

        public static IDistributedOrdinate FactoryTruncatedNormal(double mean, double stDev, double min, double max, int sampleSize = 2147483647)
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
