using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Coordinates
{
    internal class CoordinateVariableY: ICoordinate<double, Statistics.IDistribution>
    {
        //TODO: Validate

        public double X { get; }
        public Statistics.IDistribution Y { get; }

        public CoordinateVariableY(double x, Statistics.IDistribution y)
        {
            X = x;
            Y = y;
        }
        //public ScalarCoordinateVariableY(IOrdinate<double> x, IDistribution y)
        //{
        //    XOrdinate = x;
        //    YOrdinate = y;
        //}
        //public ScalarCoordinateVariableY(IOrdinate<double> x, IOrdinate<IDistribution> y)
        //{
        //    XOrdinate = x;
        //    YOrdinate = y;
        //}
        //public ScalarCoordinateVariableY(double x, IOrdinate<IDistribution> y)
        //{
        //    XOrdinate = new Constant(x);
        //    YOrdinate = y;
        //}

        public ICoordinate<double, double> Sample(double p) => new CoordinateConstants(X, Y.InverseCDF(p));
    }
}
