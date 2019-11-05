using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Functions
{
    public interface ICoordinatesFunction<XType, YType>:ICoordinatesFunctionBase
    {
        YType F(XType x);
        XType InverseF(YType y);
        List<ICoordinate<XType, YType>> Coordinates { get; }
        //OrderedSetEnum Order { get; }
        //InterpolationEnum Interpolator { get; }

        //bool IsDistributed { get; }
        //Tuple<double, double> Domain { get; }

        //IFunction Sample(double p);
        //IFunction Sample(double p, InterpolationEnum interpolator);

    }
}
