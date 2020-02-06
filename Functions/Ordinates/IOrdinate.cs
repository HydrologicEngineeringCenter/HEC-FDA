using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Utilities;

namespace Functions
{
    /// <summary>
    /// An interface for invariant and distributed numeric function parameters
    /// </summary>
    public interface IOrdinate: IMessagePublisher
    {
        #region Properties
        /// <summary>
        /// Provides the inclusive <see cref="IRange{T}.Min"/> and <see cref="IRange{T}.Max"/> range of <see cref="IOrdinate"/> values. <see cref="IRange{T}"/> 
        /// </summary>
        IRange<double> Range { get; }
        ///// <summary>
        ///// Provides the lower and upper bound of the scalar values.
        ///// </summary>
        //Tuple<double, double> Range { get; }
        /// <summary>
        /// Enumerated type of distribution. Values which are NOT distributed are given the enumerated value of: 'Constant'.
        /// </summary>
        IOrdinateEnum Type { get; }
        #endregion

        #region Functions
        /// <summary>
        /// Computes the value of the ordinate at the specified non-exceedance probability, <paramref name="p"/>.
        /// </summary>
        /// <param name="p"> The non-exceedance value at which the Inverse CDF should be computed. <seealso cref="Statistics.IDistribution.InverseCDF(double)"/> </param>
        /// <returns> A <see cref="double"/> value associated with the value of the specified probability parameter, <paramref name="p"/>. If the <see cref="IOrdinate"/> represents a <see cref="IOrdinateEnum.Constant"/> this will yield a single result for all possible values of the parameter <paramref name="p"/>. </returns>
        double Value(double p = 0.50);
        /// <summary>
        /// Evaluates the value equality of 2 <see cref="IOrdinate"/>s.
        /// </summary>
        /// <param name="ordinate"> The specified <see cref="IOrdinate"/> to be compared to the instance <see cref="IOrdinate"/>. </param>
        /// <returns> <see langword="true"/> if the two instances are equal in their values, <see langword="false"/> otherwise. </returns>
        bool Equals(IOrdinate ordinate);
        /// <summary>
        /// Provides a string representation of the <see cref="IOrdinate"/>.
        /// </summary>
        /// <param name="round"> <see langword="true"/> if numerical values should be provided in scientific notation and/or rounded to produce a more readable result. Set to <see langword="false"/> by default. </param>
        /// <returns> A string summarizing the <see cref="IOrdinate"/> value. </returns>
        string Print(bool round = false);

        XElement WriteToXML();
        #endregion
    }
}
