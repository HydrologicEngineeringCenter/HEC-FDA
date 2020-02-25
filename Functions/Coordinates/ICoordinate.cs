using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Functions
{
    /// <summary>
    /// Provides an interface for coordinates composed of an <see cref="X"/> <see cref="IOrdinate"/> object and a <see cref="Y"/> <see cref="IOrdinate"/> object.
    /// </summary>
    public interface ICoordinate: Utilities.IMessagePublisher
    {
        /// <summary>
        /// An object implementing the <see cref="IOrdinate"/> interface, representing the independent variable in a functional relationship.
        /// </summary>
        IOrdinate X { get; }
        /// <summary>
        /// An object implementing the <see cref="IOrdinate"/> interface, representing the dependent variable in a functional relationship.
        /// </summary>
        IOrdinate Y { get; }
        /// <summary>
        /// Provides a string representation of the <see cref="ICoordinate"/>.
        /// </summary>
        /// <param name="round"> <see langword="true"/> if numerical values should be provided in scientific notation and/or rounded to produce a more readable result. Set to <see langword="false"/> by default. </param>
        /// <returns> A string in the form: (<see cref="X"/>, <see cref="Y"/>) where the <see cref="X"/> and <see cref="Y"/> <see cref="IOrdinate"/> parameters are represented by their <see cref="IOrdinate.Print(bool)"/> strings. </returns>
        string Print(bool round);
        /// <summary>
        /// Serializes the <see cref="ICoordinate"/> object to an XML format for persistence in the database.
        /// </summary>
        /// <returns> An <see cref="XElement"/> containing the <see cref="ICoordinate"/> parameters. </returns>
        XElement WriteToXML();
        //ICoordinate<double, double> Sample(double p);
    }
}
