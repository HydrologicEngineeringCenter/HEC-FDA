using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    public interface IFunction : ICoordinatesFunction
    {
        bool IsInvertible { get; }
        //Tuple<double, double> Range { get; }
        Utilities.IRange<double> Range { get; }
        double RiemannSum();
        IFunction Compose(IFunction g);
        
    }
}
