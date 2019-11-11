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
    }
}
