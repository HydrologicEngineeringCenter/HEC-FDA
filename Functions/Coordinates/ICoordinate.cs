using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    //TODO: Comment
    //TODO: Factory Method

    public interface ICoordinate<out XType, out YType>
    {
        XType X { get; }
        YType Y { get; }

        //ICoordinate<double, double> Sample(double p);
    }
}
