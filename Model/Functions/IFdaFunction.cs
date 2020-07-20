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
        /// <summary>
        /// Describes the order of Y value relative to increasing X values.
        /// </summary>
        OrderedSetEnum Order { get; }
        ///// <summary>
        ///// The range of Y values.
        ///// </summary>
        //new IRange<double> Range { get; }
        /// <summary>
        /// The range of X values.
        /// </summary>
        IRange<double> Domain { get; }

        /// <summary>
        /// Describes the X axis of the function <see cref="UnitsEnum"/>, <see cref="ParameterType"/>, etc.
        /// </summary>
        IParameter XSeries { get; }
        /// <summary>
        /// Describes the Y axis of the function <see cref="UnitsEnum"/>, <see cref="ParameterType"/>, etc.
        /// </summary>
        IParameter YSeries { get; }
        ///// <summary>
        ///// The type of the function.
        ///// </summary>
        //IParameterEnum ParameterType { get; }    
        ///// <summary>
        ///// True if the function ordinates are static values, false otherwise.
        ///// </summary>
        //bool IsConstant { get; }
        /// <summary>
        /// A set of coordinates describing the function.
        /// </summary>
        List<ICoordinate> Coordinates { get; } 

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
