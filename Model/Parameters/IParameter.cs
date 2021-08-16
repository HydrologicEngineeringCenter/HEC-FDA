using Functions;
using Utilities;

namespace Model
{
    /// <summary>
    /// Parameters used in Fda.
    /// </summary>
    public interface IParameter: IMessagePublisher
    {
        /// <summary>
        /// Parameter type from the enumerated list: <see cref="IParameterEnum"/>.
        /// </summary>
        IParameterEnum ParameterType { get; }
        /// <summary>
        /// A <see cref="string"/> label if none is provided the <see cref="IParameterEnum"/> is used.
        /// </summary>
        string Label { get; }
        /// <summary>
        /// True if the parameter value is a static value, false otherwise. 
        /// </summary>
        bool IsConstant { get; }

        /// <summary>
        /// Print a string representation of the parameter.
        /// </summary>
        /// <param name="round"> <see langword="true"/> if some values should be rounded or displayed in scientific notation to produce a more human readable result. </param>
        /// <param name="abbreviate"> <see langword="true"/> if the unit name should be printed using the <a href="http://www.ieee.org/">IEEE</a> recommended abbreviation, <see langword="false"/> otherwise.</param>
        /// <returns> A string in the form... <see cref="IParameter.ParameterType"/>(the parameter range or value and units), where the range or value is formated as a string based on the <see cref="IRange{T}.Print(bool)"/> or <see cref="IOrdinate.Print(bool)"/> functions. </returns>
        string Print(bool round = false, bool abbreviate = false);    }
}
