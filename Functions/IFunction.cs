using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    public interface IFunction: ICoordinatesFunction<Constant, Constant>
    {
        bool IsInvertible { get; }
        Tuple<double, double> Range { get; }
        double RiemannSum();
        IFunction Compose(IFunction g);
        //Tuple<double, double> Domain { get; }
        //InterpolationEnum Interpolation { get; }
        //OrderedSetEnum Order { get; }

        //double F(double x);
        //double InverseF(double y);
    }
}
