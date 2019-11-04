using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Utilities
{
    public static class Integrator
    {
        public static double IntegrateUsingRiemannSum(ICoordinatesFunction<double,double> function)
        {
            double riemannSum = 0;
            for (int i = 0; i < function.Coordinates.Count - 1; i++)
            {
                riemannSum += (function.Coordinates[i + 1].Y + function.Coordinates[i].Y) * 
                    (function.Coordinates[i + 1].X - function.Coordinates[i].X) / 2;
            }
            return riemannSum;
        }
    }
}
