using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Functions.CoordinatesFunctions
{
    interface ICoordinatesFunctions<XType, YType> :ICoordinatesFunction<XType, YType>
    {
        List<ICoordinatesFunction<XType, YType>> Functions { get; }
        //todo put list of interpolator enum
    }
}
