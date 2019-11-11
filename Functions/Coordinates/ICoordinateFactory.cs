using Functions.Coordinates;
using Functions.Ordinates;
using Statistics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    public static class ICoordinateFactory
    {
        public static ICoordinate Factory(double x, double y)
        {
            return new CoordinateConstants(new Constant(x),new Constant(y));
        }

        public static ICoordinate Factory(double x, IDistributedValue y)
        {
            return new CoordinateVariableY(new Constant(x),new Distribution(y));
        }
    }
}
