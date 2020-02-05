using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Utilities;
using Utilities.Serialization;

namespace Functions
{
    public interface ICoordinatesFunction : ISerializeToXML<ICoordinatesFunction>, IValidate<ICoordinatesFunction>
    {
        /// <summary>
        /// Finds the <see cref="ICoordinate.Y"/> <see cref="IOrdinate"/> associated with the specified <see cref="ICoordinate.X"/> <see cref="IOrdinate"/>.
        /// </summary>
        /// <param name="x"> An <see cref="ICoordinate.X"/> on the <see cref="ICoordinatesFunction.Domain"/>. </param>
        /// <returns> An <see cref="IOrdinate"/> from the range of the <see cref="ICoordinatesFunction"/> or an <see cref="ArgumentOutOfRangeException"/> exception if the specified <see cref="IOrdinate"/> is not on the <see cref="ICoordinatesFunction.Domain"/> of the <see cref="ICoordinatesFunction"/>. </returns>
        IOrdinate F(IOrdinate x);
        /// <summary>
        /// Finds the <see cref="ICoordinate.X"/> <see cref="IOrdinate"/> associated with the specified <see cref="ICoordinate.Y"/> <see cref="IOrdinate"/>, provided that the <see cref="ICoordinatesFunction"/> is invertible.
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        IOrdinate InverseF(IOrdinate y);
        List<ICoordinate> Coordinates { get; }
        OrderedSetEnum Order { get; }
        InterpolationEnum Interpolator { get; }
        Utilities.IRange<double> Domain { get; }

        bool Equals(ICoordinatesFunction function);
        Functions.IOrdinateEnum DistributionType { get; }
        bool IsLinkedFunction { get; }
        //bool IsDistributed { get; }

        //IFunction Sample(double p);
        //IFunction Sample(double p, InterpolationEnum interpolator);

    }
}
