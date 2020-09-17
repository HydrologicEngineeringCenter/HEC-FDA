using System;
using System.Collections.Generic;
using Functions;
using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using Utilities;
using Utilities.Serialization;

namespace Model
{
    /// <summary>
    /// Provides a interface for functions in the Fda.
    /// Wraps <see cref="IFunction" />
    /// </summary>
    public interface IFdaFunction : IParameter, ISerializeToXML<IFdaFunction>
    {
        #region Properties
        //todo: Get rid of this. Turn the _Function in the base back to a private. Use IFdaFunction in the curve editors instead of ICoordFunction
        ICoordinatesFunction Function { get; }
        /// <summary>
        /// Describes the order of Y value relative to increasing X values.
        /// </summary>
        OrderedSetEnum Order { get; }
        /// <summary>
        /// The range of Y values.
        /// </summary>
        IRange<double> Range { get; }
        /// <summary>
        /// The range of X values.
        /// </summary>
        IRange<double> Domain { get; }
        /// <summary>
        /// The method of interpolation.
        /// </summary>
        InterpolationEnum Interpolator { get; }

        /// <summary>
        /// Describes the X axis of the function <see cref="UnitsEnum"/>, <see cref="ParameterType"/>, etc.
        /// </summary>
        IParameterRange XSeries { get; }
        /// <summary>
        /// Describes the Y axis of the function <see cref="UnitsEnum"/>, <see cref="ParameterType"/>, etc.
        /// </summary>
        IParameterRange YSeries { get; }
        /// <summary>
        /// A set of coordinates describing the function.
        /// </summary>
        List<ICoordinate> Coordinates { get; }

        /// <summary>
        /// <see langword="true"/> if the <see cref="IFdaFunction"/> is comprised of other linked <see cref="IFunction"/>s, <see langword="false"/> otherwise.
        /// </summary>
        bool IsLinkedFunction { get; }
        /// <summary>
        /// Describes the <see cref="IOrdinate.Type"/> of the <see cref="ICoordinate.Y"/> values in the set of <see cref="Coordinates"/>. 
        /// </summary>
        IOrdinateEnum DistributionType { get; }
        #endregion
        #region Functions
        /// <summary>
        /// Computes <see cref="ICoordinatesFunction.F(IOrdinate)"/> returning a y value for the provided x value.
        /// </summary>
        /// <param name="x"> An x value to find. </param>
        /// <returns> A y ordinate. </returns>
        IOrdinate F(IOrdinate x);
        /// <summary>
        /// Computes <see cref="ICoordinatesFunction.InverseF(IOrdinate)"/> returning a x value for the provided y values.
        /// </summary>
        /// <param name="y"> A y value to find. </param>
        /// <returns> An x ordinate, provided that <see cref="IFunction.IsInvertible"/> is <see langword="true"/> (e.g. the function is invertible), otherwise an exception is raised. </returns>
        IOrdinate InverseF(IOrdinate y);

        /// <summary>
        /// Tests for values equality between two <see cref="IFdaFunction"/> objects.
        /// </summary>
        /// <param name="function"> The specified <see cref="IFdaFunction"/> to compared to the instance object. </param>
        /// <returns> <see langword="true"/> if the two objects are equal in value, <see langword="false"/> otherwise. </returns>
        bool Equals(IFdaFunction function);
        #endregion


        #region Replacement
        ///// <summary>
        ///// A enumerated value describing the type of function (e.g. Rating, Stage-Damage, etc.).
        ///// </summary>
        //IFdaFunctionEnum Type { get; }
        ///// <summary>
        ///// A string describing the X values or independent variable values.
        ///// </summary>
        //IParameter
        ///// <summary>
        ///// A string describing the Y values or dependent variable values.
        ///// </summary>
        //string YLabel { get; }

        ///// <summary>
        ///// The <see cref="ICoordinatesFunction"/> containing the X-Y values.
        ///// </summary>
        //ICoordinatesFunction Function { get; }
        ///// <summary>
        ///// Tests for values equality between two <see cref="IFdaFunction"/> objects.
        ///// </summary>
        ///// <param name="function"> The specified <see cref="IFdaFunction"/> to compared to the instance object. </param>
        ///// <returns> <see langword="true"/> if the two objects are equal in value, <see langword="false"/> otherwise. </returns>
        //bool Equals(IFdaFunction function);
        #endregion
    }
}
