using System;
using System.Collections.Generic;
using Functions;
using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using Model.Inputs.Functions.ImpactAreaFunctions;
using Utilities.Serialization;

namespace Model
{
    public interface IFdaFunction : ISerializeToXML<IFdaFunction>
    {
        string XLabel { get; }
        string YLabel { get; }
        ImpactAreaFunctionEnum Type { get; }
        ICoordinatesFunction Function { get; }

        bool Equals(IFdaFunction function);

        //public List<ICoordinate<double, YType>> Coordinates { get; }
        //public InterpolationEnum Interpolator { get; }
        //public OrderedSetEnum Order { get; }
        //IFunction Sample(double p);
        //YType F(double x);
        //double InverseF(YType y);

    }
}
