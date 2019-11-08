using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Functions
{
    public static class IFunctionFactory
    {
        public static IFunction Factory(List<ICoordinate> coordinates, 
            InterpolationEnum interpolation) => new CoordinatesFunctionConstants(coordinates, interpolation);
    }
}
