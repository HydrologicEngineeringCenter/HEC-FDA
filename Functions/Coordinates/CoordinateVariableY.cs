using Functions.Ordinates;
using Statistics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Coordinates
{
    internal class CoordinateVariableY: ICoordinate<double, IDistribution>
    {
        //TODO: Validate

        public double X { get; }
        public IDistribution Y { get; }

        public CoordinateVariableY(double x, IDistribution y)
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

        public override bool Equals(object obj)
        {
            return obj is CoordinateVariableY coord &&
                   X == coord.X &&
                   EqualityComparer<IDistribution>.Default.Equals(Y, coord.Y);
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IDistribution>.Default.GetHashCode(Y);
            return hashCode;
        }
    }
}
