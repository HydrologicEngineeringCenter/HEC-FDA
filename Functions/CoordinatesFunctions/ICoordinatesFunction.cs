using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Functions
{
    public interface ICoordinatesFunction
    {
        IOrdinate F(IOrdinate x);
        IOrdinate InverseF(IOrdinate y);
        List<ICoordinate> Coordinates { get; }
        OrderedSetEnum Order { get; }
        InterpolationEnum Interpolator { get; }

        Tuple<double, double> Domain { get; }

        //bool IsDistributed { get; }

        //IFunction Sample(double p);
        //IFunction Sample(double p, InterpolationEnum interpolator);

    }
}
