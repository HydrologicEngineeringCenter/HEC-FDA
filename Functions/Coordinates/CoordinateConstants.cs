using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Coordinates
{
    internal class CoordinateConstants: ICoordinate<double, double>
    {
        //TODO: Validation

        public double X { get; }
        public double Y { get; }

        public CoordinateConstants(double x, double y)
        {
            X = x;
            Y = y;
        }
        //public ScalarCoordinateConstants(double x, IOrdinate<double> y)
        //{
        //    XOrdinate = new Constant(x);
        //    YOrdinate = y;
        //}
        //public ScalarCoordinateConstants(IOrdinate<double> x, IOrdinate<double> y)
        //{
        //    XOrdinate = x;
        //    YOrdinate = y;
        //}

        public ICoordinate<double, double> Sample(double p = 0.50) => this;
    }
}
