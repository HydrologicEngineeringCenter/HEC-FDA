using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics.Coordinates
{
    internal class ScalarCoordinateConstants: ICoordinate<double, double>
    {
        //TODO: Validation

        public IOrdinate<double> XOrdinate { get; }
        public IOrdinate<double> YOrdinate { get; }

        public ScalarCoordinateConstants(double x, double y)
        {
            XOrdinate = new Constant(x);
            YOrdinate = new Constant(y);
        }
        public ScalarCoordinateConstants(double x, IOrdinate<double> y)
        {
            XOrdinate = new Constant(x);
            YOrdinate = y;
        }
        public ScalarCoordinateConstants(IOrdinate<double> x, IOrdinate<double> y)
        {
            XOrdinate = x;
            YOrdinate = y;
        }
    }
}
