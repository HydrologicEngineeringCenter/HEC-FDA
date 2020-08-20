using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Functions
{
    /// <summary>
    /// Provides a factory for the construction of objects implementing the <see cref="IFunction"/> interface.
    /// </summary>
    public static class IFunctionFactory
    {
        /// <summary>
        /// Constructs an <see cref="IFunction"/> from a specified set of <see cref="ICoordinate"/>s and interpolation scheme, provided by <see cref="InterpolationEnum"/> (<seealso cref="ICoordinatesFunction"/>).
        /// </summary>
        /// <param name="coordinates"> A set of <see cref="ICoordinate"/>s with <see cref="IOrdinateEnum.Constant"/> <see cref="IOrdinate"/> values. <b>Note</b>: the use of non <see cref="IOrdinateEnum.Constant"/> <see cref="IOrdinate"/> values will cause errors. </param>
        /// <param name="interpolation"> A scheme for interpolating (on not interpolating) between the specified <see cref="ICoordinate"/>s. </param>
        /// <returns> A <see cref="IFunction"/> (<seealso cref="ICoordinatesFunction"/>). </returns>
        public static IFunction Factory(List<ICoordinate> coordinates,
            InterpolationEnum interpolation) => new CoordinatesFunctionConstants(coordinates, interpolation);
        /// <summary>
        /// Constructions an object implementing the <see cref="IFunction"/> from a <see cref="Statistics.IDistribution"/>.
        /// </summary>
        /// <param name="distribution"> The <see cref="Statistics.IDistribution"/> to represent as a function.</param>
        /// <returns> An object implementing the <see cref="IFunction"/> interface. </returns>
        public static IFunction Factory(Statistics.IDistribution distribution)
        {
            return new DistributionFunction(IDistributedOrdinateFactory.Factory(distribution));
        }
    }
}
