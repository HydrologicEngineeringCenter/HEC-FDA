using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Coordinates
{
    /// <summary>
    /// This class exists so that a list of CoordinateConstants and CoordinateVariableY
    /// </summary>
    internal class CoordinateOrdinateY : ICoordinate
    {
        public IOrdinate X { get; }

        public IOrdinate Y { get; }

        public CoordinateOrdinateY(Constant x, IOrdinate y)
        {
            X = x;
            Y = y;
        }

        //public ICoordinate<Constant, Constant> Sample(double p)
        //{
        //    return new CoordinateConstants(X, new Constant(Y.Value(p)));
        //}

        public override bool Equals(object obj)
        {
            return obj is CoordinateOrdinateY coord &&
                   X == coord.X &&
                   EqualityComparer<IOrdinate>.Default.Equals(Y, coord.Y);
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IOrdinate>.Default.GetHashCode(Y);
            return hashCode;
        }
    }
}
