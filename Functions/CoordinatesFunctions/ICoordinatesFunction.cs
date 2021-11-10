using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Utilities;
using Utilities.Serialization;

namespace Functions
{
    /// <summary>
    /// Provides an interface for functions composed of multiple <see cref="ICoordinate"/> objects linked by an function for interpolating between <see cref="ICoordinate"/> values.
    /// </summary>
    public interface ICoordinatesFunction : ISerializeToXML<ICoordinatesFunction>, IValidate<ICoordinatesFunction>
    {

        /// <summary>
        /// A list of the <see cref="ICoordinate"/> objects that define the function, sorted on their <see cref="ICoordinatesFunction.Domain"/>. 
        /// </summary>
        List<ICoordinate> Coordinates { get; }
        /// <summary>
        /// Describes the relationship of the Y values, relative to the X values. <seealso cref="OrderedSetEnum"/>
        /// </summary>
        OrderedSetEnum Order { get; }
        /// <summary>
        /// Describes how values are interpolated between the function's <see cref="Coordinates"/>.
        /// </summary>
        InterpolationEnum Interpolator { get; }
        /// <summary>
        /// The domain of the function, represented by the smallest and largest <see cref="ICoordinate.X"/> <see cref="IOrdinate"/> in the set of <see cref="Coordinates"/>.
        /// </summary>
        Utilities.IRange<double> Domain { get; }
        /// <summary>
        /// Provides the minimum and maximum Y values for the <see cref="IFunction"/>.
        /// </summary>
        Utilities.IRange<double> Range { get; }
        /// <summary>
        /// Describes the <see cref="IOrdinate.Type"/> of the <see cref="ICoordinate.Y"/> values in the set of <see cref="Coordinates"/>. 
        /// </summary>
        Functions.IOrdinateEnum DistributionType { get; }
        /// <summary>
        /// Multiple <see cref="ICoordinatesFunction"/> objects each with their own <see cref="Interpolator"/> and <see cref="DistributionType"/> can be linked together to produce a larger <see cref="ICoordinatesFunction"/> with their shared <see cref="Domain"/> and range.
        /// <see langword="true"/> if the <see cref="ICoordinatesFunction"/> is composed of several smaller <see cref="ICoordinatesFunction"/> and is <see langword="false"/> otherwise.
        /// </summary>
        bool IsLinkedFunction { get; }

        /// <summary>
        /// Finds the <see cref="ICoordinate.Y"/> <see cref="IOrdinate"/> associated with the specified <see cref="ICoordinate.X"/> <see cref="IOrdinate"/>.
        /// </summary>
        /// <param name="x"> An <see cref="ICoordinate.X"/> on the <see cref="Domain"/> of the function (composed of the smallest and largest <see cref="ICoordinate.X"/> values). </param>
        /// <returns> An <see cref="IOrdinate"/> on the range of the <see cref="ICoordinatesFunction"/> or an <see cref="ArgumentOutOfRangeException"/> exception if the specified <paramref name="x"/> <see cref="IOrdinate"/> is not on the <see cref="Domain"/> of the <see cref="ICoordinatesFunction"/>. </returns>
        IOrdinate F(IOrdinate x);
        /// <summary>
        /// Finds the <see cref="ICoordinate.X"/> <see cref="IOrdinate"/> associated with the specified <see cref="ICoordinate.Y"/> <see cref="IOrdinate"/>, provided that the <see cref="ICoordinatesFunction"/> is invertible and <paramref name="y"/> is on the range of the <see cref="ICoordinatesFunction"/>.
        /// </summary>
        /// <param name="y"> A <see cref="IOrdinate"/> on the range of the <see cref="ICoordinatesFunction"/> (composed of the smallest and largest <see cref="ICoordinate.Y"/> values. </param>
        /// <returns> An <see cref="IOrdinate"/> on the domain of the <see cref="ICoordinatesFunction"/> or an <see cref="ArgumentOutOfRangeException"/> exception if the specified <paramref name="y"/> <see cref="IOrdinate"/> is not on the range of the <see cref="ICoordinatesFunction"/> or the <see cref="ICoordinatesFunction"/> is not invertible. </returns>
        IOrdinate InverseF(IOrdinate y);
        /// <summary>
        /// Test for <b>value</b> equality between the instance and specified <see cref="ICoordinatesFunction"/> objects.
        /// </summary>
        /// <param name="coordinateFunction"> The specified <see cref="ICoordinatesFunction"/> to be compared with the instance object. </param>
        /// <returns> <see langword="true"/> if the functions are equal in value, <see langword="false"/> otherwise. </returns>
        bool Equals(ICoordinatesFunction coordinateFunction);
        List<ICoordinate> GetExpandedCoordinates();

    }
}
