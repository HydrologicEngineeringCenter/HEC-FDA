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
        public static ICoordinate<double, double> Factory(double x, double y)
        {
            return (ICoordinate<double, double>)new CoordinateConstants(x,y);
        }

        public static ICoordinate<double, IDistribution> Factory(double x, IDistribution y)
        {
            return (ICoordinate<double, IDistribution>)new CoordinateVariableY(x, y);
        }
    }
}
