using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Functions
{
    //TODO: Comment
    //TODO: Factory Method

    public interface ICoordinate 
    {
        IOrdinate X { get; }
        IOrdinate Y { get; }

        XElement WriteToXML();
        //ICoordinate<double, double> Sample(double p);

    }
}
