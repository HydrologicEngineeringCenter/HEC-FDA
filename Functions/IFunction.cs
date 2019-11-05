using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    public interface IFunction: ICoordinatesFunction<double, double>
    {
        bool IsInvertible { get; }
        Tuple<double, double> Range { get; }
        //Tuple<double, double> Domain { get; }
        //InterpolationEnum Interpolation { get; }
        //OrderedSetEnum Order { get; }

        //double F(double x);
        //double InverseF(double y);
        double RiemannSum();
        IFunction Compose(IFunction g);
    }
}
