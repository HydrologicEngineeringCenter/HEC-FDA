using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using MathNet.Numerics.Distributions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Utilities;

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
        /// <summary>
        /// Constructs an object implementing the <see cref="IFunction"/> from a <see cref="Statistics.IDistribution"/> truncated on the range: [<paramref name="min"/>, <paramref name="max"/>].
        /// </summary>
        /// <param name="distribution"> The <see cref="Statistics.IDistribution"/> to be truncated. </param>
        /// <param name="min"> The lower bound for the <see cref="IFunction"/>. </param>
        /// <param name="max"> The upper bound for the <see cref="IFunction"/>. </param>
        /// <returns> An statistically based <see cref="IFunction"/> truncated on the specified range. </returns>
        public static IFunction Factory(IFunction distribution, double min, double max)
        {
            if (distribution.IsNull()) throw new ArgumentNullException(nameof(distribution));
            Statistics.IDistribution dist;
            try
            {
                var ord = ((DistributionFunction)distribution)._Distribution;
                dist = ((Distribution)ord)._Distribution;
            }
            catch
            {
                throw new InvalidCastException($"The provided {nameof(IFunction)} could not be cast as a statistical distribution.");
            }
            var trunc = Statistics.IDistributionFactory.FactoryTruncatedDistribution(dist, min, max);
            return new DistributionFunction(IDistributedOrdinateFactory.Factory(trunc));
        }
    }
}
