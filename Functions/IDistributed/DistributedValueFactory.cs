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
    }
}
