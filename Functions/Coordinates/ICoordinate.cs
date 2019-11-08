using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    //TODO: Comment
    //TODO: Factory Method

    public interface ICoordinate 
    {
        IOrdinate X { get; }
        IOrdinate Y { get; }

        //ICoordinate<double, double> Sample(double p);
    }
}
