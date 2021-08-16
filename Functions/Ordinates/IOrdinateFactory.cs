using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    public static class IOrdinateFactory
    {
        public static IOrdinate Factory(double x) => new Ordinates.Constant(x);
        public static IOrdinate Factory(Statistics.IDistribution distribution) => new Ordinates.Distribution(distribution);
    }
}
