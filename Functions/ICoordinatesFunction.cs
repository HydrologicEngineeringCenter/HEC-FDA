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
        IImmutableList<ICoordinate<IOrdinate, IOrdinate>> Coordinates { get; }

        bool IsDistributed { get; }
        Tuple<double, double> Domain { get; }

    }
}
