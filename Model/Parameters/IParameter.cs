using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using static System.Net.WebRequestMethods;

namespace Model
{
    /// <summary>
    /// Parameters used in Fda.
    /// </summary>
    public interface IParameter //<T>
    {
        /// <summary>
        /// The units of measurement.
        /// </summary>
        UnitsEnum Units { get; }
        /// <summary>
        /// Parameter type from the enumerated list: <see cref="IParameterEnum"/>.
        /// </summary>
        IParameterEnum ParameterType { get; }
        /// <summary>
        /// A <see cref="string"/> label if none is provided the <see cref="IParameterEnum"/> is used.
        /// </summary>
        string Label { get; }
        ///// <summary>
        ///// The parameter object, value or values.
        ///// </summary>
        //T Parameter { get; }
        /// <summary>
        /// The range of parameter values.
        /// </summary>
        IRange<double> Range { get; }
        /// <summary>
        /// True if the parameter value is a static value, false otherwise. 
        /// </summary>
        bool IsConstant { get; }

        /// <summary>
        /// Print a string representation of the parameter.
        /// </summary>
        /// <param name="round"> <see langword="true"/> if some values should be rounded or displayed in scientific notation to produce a more human readable result. </param>
        /// <param name="abbreviate"> <see langword="true"/> if the unit name should be printed using the <a href="http://www.ieee.org/">IEEE</a> recommended abbreviation, <see langword="false"/> otherwise.</param>
        /// <returns> A string in the form... <see cref="IParameter{T}.ParameterType"/>: <see cref="IParameter{T}.Parameter"/> <see cref="IParameter{T}.Units"/>, where <see cref="IParameter{T}.Parameter"/> is formated as a string based on the type T. </returns>
        string Print(bool round = false, bool abbreviate = false);
        /// <summary>
        /// Prints a string representation of the <see cref="IParameter{T}.Parameter"/> value and its units.
        /// </summary>
        /// <param name="round"> <see langword="true"/> if some values should be rounded or displayed in scientific notation to produce a more human readable result. </param>
        /// <param name="abbreviate"> <see langword="true"/> if the unit name should be printed using the <a href="http://www.ieee.org/">IEEE</a> recommended abbreviation, <see langword="false"/> otherwise.</param>
        /// <returns> A string in the form... <see cref="IParameter{T}.Parameter"/> <see cref="IParameter{T}.Units"/>, where <see cref="IParameter{T}.Parameter"/> is formated as a string based on the type T. </returns>
        string PrintValue(bool round = false, bool abbreviate = false);
    }
}
