using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics.Coordinates
{
    public interface ICoordinateFunction<XType, YType>
    {
        //TODO: Optionally enforce common X, Y types across all coordinates (Functions may have a solution here)

        bool IsInvertable { get; }
        ICoordinate<XType, YType>[] Coordinates { get; }
        //Interpolator --Description: Linear, Piecewise, NoInterpolation ...
        //Interpolator<YType, XType>

        YType F(XType x);
        XType InverseF(YType y);
        ICoordinate<double, double> Sample(double p);
    }
}
