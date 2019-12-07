using Functions.Ordinates;
using System;
using System.Collections;
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

        public ICoordinate<double, double> Sample(double p = 0.50) => this;

        public override bool Equals(object obj)
        {
            return obj is CoordinateConstants constants &&
                   X == constants.X &&
                   Y == constants.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
    }
}
