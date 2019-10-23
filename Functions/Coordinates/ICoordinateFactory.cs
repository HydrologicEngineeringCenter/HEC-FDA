using Functions.Coordinates;
using Statistics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    public static class ICoordinateFactory
    {
        public static ICoordinate<IOrdinate, IOrdinate> Factory(double x, double y)
        {
            return (ICoordinate<IOrdinate, IOrdinate>)new CoordinateConstants(x, y);
        }

        public static ICoordinate<IOrdinate, IOrdinate> Factory(double x, IDistribution y)
        {
            return (ICoordinate<IOrdinate, IOrdinate>)new CoordinateVariableY(x, y);
        }
    }
}
