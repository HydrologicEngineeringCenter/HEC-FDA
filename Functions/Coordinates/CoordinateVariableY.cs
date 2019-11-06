using Functions.Ordinates;
using Statistics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Coordinates
{
    internal class CoordinateVariableY: ICoordinate<Constant, Distribution>
    {
        //TODO: Validate

        public Constant X { get; }
        public Distribution Y { get; }

        public CoordinateVariableY(Constant x, Distribution y)
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

        //public ICoordinate<double, double> Sample(double p) => new CoordinateConstants(X, Y.InverseCDF(p));

        public override bool Equals(object obj)
        {
            return obj is CoordinateVariableY coord &&
                   X == coord.X &&
                   EqualityComparer<Distribution>.Default.Equals(Y, coord.Y);
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Distribution>.Default.GetHashCode(Y);
            return hashCode;
        }
    }
}
