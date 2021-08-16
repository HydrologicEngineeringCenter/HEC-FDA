using System;
using System.Collections.Generic;
using System.Text;
using Functions;
using Utilities;

namespace Model
{
    /// <summary>
    /// Provides descriptive parameters.
    /// Wraps <see cref="Functions.IOrdinate"/> and <see cref="Statistics.IDistribution"/> dependencies,
    /// providing a shared interface for distributed and invariant numeric values.
    /// </summary>
    public interface IFdaOrdinate : IMessagePublisher
    {
        /// <summary>
        /// Provides the inclusive <see cref="IRange{T}.Min"/> and <see cref="IRange{T}.Max"/> range of <see cref="IOrdinate"/> values (see: <see cref="IRange{T}"/>). 
        /// </summary>
        IRange<double> Range { get; }
        /// <summary>
        /// An <see cref="IFdaOrdinateEnum"/> value, 
        /// mirrors the <see cref="Functions.IOrdinateEnum"/> and <see cref="Statistics.IDistributionEnum"/> lists of supported statistical distributions,
        /// including <see cref="IOrdinateEnum.Constant"/> if the value is not distributed.
        /// </summary>
        IOrdinateEnum Type { get; }

        /// <summary>
        /// Returns a value for the ordinate.
        /// </summary>
        /// <param name="p"> Non-exceedence probability for the value distribution, set to median by default.
        /// If the value is not distributed a constant value is return regardless of the specified parameter value. </param>
        /// <returns> A <see cref="double"/> precision value from the specified distribution. </returns>
        double Value(double p = 0.50);
        /// <summary>
        /// Computes value equality for 2 <see cref="IFdaOrdinate"/>s.
        /// </summary>
        /// <param name="ordinate"> The <see cref="IFdaOrdinate"/> to be compared to the instance variable. </param>
        /// <returns> <see langword="true"/> if the <see cref="IFdaOrdinate"/>s are equal in value, <see langword="false"/> otherwise. </returns>
        bool Equals(IFdaOrdinate ordinate);
        /// <summary>
        /// Provides a string representation of the <see cref="IFdaOrdinate"/>.
        /// </summary>
        /// <param name="round"> <see langword="true"/> if numerical values should be provided in scientific notation and/or rounded to produce a more readable result. Set to <see langword="false"/> by default. </param>
        /// <returns> 
        /// A string summarizing the <see cref="IFdaOrdinate"/> object value in the form:
        /// <see cref="IParameter{T}.ParameterType"/> (<see cref="IParameter{T}.Units"/>): <see cref="IOrdinate.Print(bool)"/>
        /// </returns>
        /// <remarks> note: <see cref="Print(bool)"/> adds to <see cref="IOrdinate.Print(bool)"/> which mirrors <see cref="Statistics.IDistribution.Print(bool)"/>. </remarks>
        string Print(bool round = false);
    }
}
