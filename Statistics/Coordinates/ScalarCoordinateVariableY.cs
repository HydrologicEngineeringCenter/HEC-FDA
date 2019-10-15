using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics.Coordinates.Ordinates
{
    internal class ScalarCoordinateVariableY: ICoordinate<double, IDistribution>
    {
        //TODO: Validate

        public double X { get; }
        public IDistribution Y { get; }

        public ScalarCoordinateVariableY(double x, IDistribution y)
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

        public ICoordinate<double, double> Sample(double p) => new ScalarCoordinateConstants(X, Y.InverseCDF(p));
    }
}
