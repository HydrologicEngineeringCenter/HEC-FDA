using System;
using System.Collections.Generic;
using Functions;
using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using Model.Condition.ComputePoint.ImpactAreaFunctions;

namespace Model
{
    public interface IFdaFunction<IOrdinate> :ICoordinatesFunction<Constant,IOrdinate>
    {
       public string XLabel { get; }
        public string YLabel { get; }
        ImpactAreaFunctionEnum Type { get; }

        //public List<ICoordinate<double, YType>> Coordinates { get; }
        //public InterpolationEnum Interpolator { get; }
        //public OrderedSetEnum Order { get; }
        //IFunction Sample(double p);
        //YType F(double x);
        //double InverseF(YType y);

    }
}
