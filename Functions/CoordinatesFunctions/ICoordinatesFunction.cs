using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Functions
{
    public interface ICoordinatesFunction<XType, YType>
    {
        YType F(XType x);
        XType InverseF(YType y);
        IImmutableList<ICoordinate<XType, YType>> Coordinates { get; }
        OrderedSetEnum Order { get; }
        InterpolationEnum Interpolator { get; }

        bool IsDistributed { get; }
        Tuple<double, double> Domain { get; }

        ICoordinatesFunction<double,double> Sample(double p);
        ICoordinatesFunction<double, double> Sample(double p, InterpolationEnum interpolator);

    }
}
