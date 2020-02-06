using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Functions
{
    //TODO: Comment
    //TODO: Factory Method

    public interface ICoordinate: Utilities.IMessagePublisher
    {
        IOrdinate X { get; }
        IOrdinate Y { get; }
        string Print(bool round);
        XElement WriteToXML();
        //ICoordinate<double, double> Sample(double p);

    }
}
