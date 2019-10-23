using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Functions
{
    public static class IFunctionFactory
    {
        public static IFunction Factory(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> coordinates, 
            InterpolationEnum interpolation) => new CoordinatesFunctionConstants(coordinates, interpolation);
    }
}
