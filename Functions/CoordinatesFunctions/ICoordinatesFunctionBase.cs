using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    public interface ICoordinatesFunctionBase
    {
        OrderedSetEnum Order { get; }
        InterpolationEnum Interpolator { get; }

        Tuple<double, double> Domain { get; }
    }
}
