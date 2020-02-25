using System;
using System.Collections.Generic;
using Functions;
using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using Model.Inputs.Functions.ImpactAreaFunctions;
using Utilities;
using Utilities.Serialization;

namespace Model
{
    /// <summary>
    /// Provides a interface for functions used in the Fda program.
    /// </summary>
    public interface IFdaFunction : ISerializeToXML<IFdaFunction>
    {
        /// <summary>
        /// A string describing the X values or independent variable values.
        /// </summary>
        string XLabel { get; }
        /// <summary>
        /// A string describing the Y values or dependent variable values.
        /// </summary>
        string YLabel { get; }
        /// <summary>
        /// A enumerated value describing the type of function (e.g. Rating, Stage-Damage, etc.).
        /// </summary>
        ImpactAreaFunctionEnum Type { get; }
        /// <summary>
        /// The <see cref="ICoordinatesFunction"/> containing the X-Y values.
        /// </summary>
        ICoordinatesFunction Function { get; }
        /// <summary>
        /// Tests for values equality between two <see cref="IFdaFunction"/> objects.
        /// </summary>
        /// <param name="function"> The specified <see cref="IFdaFunction"/> to compared to the instance object. </param>
        /// <returns> <see langword="true"/> if the two objects are equal in value, <see langword="false"/> otherwise. </returns>
        bool Equals(IFdaFunction function);

        //public List<ICoordinate<double, YType>> Coordinates { get; }
        //public InterpolationEnum Interpolator { get; }
        //public OrderedSetEnum Order { get; }
        //IFunction Sample(double p);
        //YType F(double x);
        //double InverseF(YType y);

    }
}
