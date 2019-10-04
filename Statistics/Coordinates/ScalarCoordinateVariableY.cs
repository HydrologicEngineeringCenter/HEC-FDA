using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics.Coordinates.Ordinates
{
    internal class ScalarCoordinateVariableY: ICoordinate<double, IDistribution>
    {
        //TODO: Validate

        public IOrdinate<double> XOrdinate { get; }
        public IOrdinate<IDistribution> YOrdinate { get; }

        public ScalarCoordinateVariableY(double x, IDistribution y)
        {
            XOrdinate = new Constant(x);
            YOrdinate = y;
        }
        public ScalarCoordinateVariableY(IOrdinate<double> x, IDistribution y)
        {
            XOrdinate = x;
            YOrdinate = y;
        }
        public ScalarCoordinateVariableY(IOrdinate<double> x, IOrdinate<IDistribution> y)
        {
            XOrdinate = x;
            YOrdinate = y;
        }
        public ScalarCoordinateVariableY(double x, IOrdinate<IDistribution> y)
        {
            XOrdinate = new Constant(x);
            YOrdinate = y;
        }
    }
}
