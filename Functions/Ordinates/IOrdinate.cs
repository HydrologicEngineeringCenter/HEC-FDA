using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Utilities;

namespace Functions
{
    /// <summary>
    /// An shared interface for distributed and invariant numeric values
    /// </summary>
    public interface IOrdinate: IMessagePublisher
    {
        #region Properties
        /// <summary>
        /// Provides the inclusive <see cref="IRange{T}.Min"/> and <see cref="IRange{T}.Max"/> range of <see cref="IOrdinate"/> values (see: <see cref="IRange{T}"/>). 
        /// </summary>
        IRange<double> Range { get; }
        /// <summary>
        /// An <see cref="IOrdinateEnum"/> value which mirrors the <see cref="Statistics.IDistributionEnum"/> value, that lists supported types of the statistical distributions or <see cref="IOrdinateEnum.Constant"/> if the value is not distributed.
        /// </summary>
        IOrdinateEnum Type { get; }
        #endregion

        #region Functions
        /// <summary>
        /// Provides a numerical value for the <see cref="IOrdinate"/>.
        /// </summary>
        /// <param name="p"> A non-exceedance value for the point at which the inverse CDF should be computed. <seealso cref="Statistics.IDistribution.InverseCDF(double)"/> </param>
        /// <returns> 
        /// If the <see cref="IOrdinate"/> represents a <see cref="IOrdinateEnum.Constant"/> an invariant <see cref="double"/> value is returned.
        /// If the <see cref="IOrdinate"/> represents a distributed value then a value is produced by computing the inverse CDF at the point <paramref name="p"/>. <seealso cref="Statistics.IDistribution.InverseCDF(double)"/>
        /// </returns>
        double Value(double p = 0.50);
        /// <summary>
        /// Evaluates the <b>value</b> equality of 2 objects implementing the <see cref="IOrdinate"/> interface.
        /// </summary>
        /// <param name="ordinate"> The specified <see cref="IOrdinate"/> object to be compared to the instance <see cref="IOrdinate"/> object. </param>
        /// <returns> <see langword="true"/> if the two instances are equal in their values, <see langword="false"/> otherwise. </returns>
        bool Equals(IOrdinate ordinate);
        /// <summary>
        /// Provides a string representation of the <see cref="IOrdinate"/>.
        /// </summary>
        /// <param name="round"> <see langword="true"/> if numerical values should be provided in scientific notation and/or rounded to produce a more readable result. Set to <see langword="false"/> by default. </param>
        /// <returns> 
        /// A string summarizing the <see cref="IOrdinate"/> object value:
        /// <list type="bullet">
        /// <item> distributed <see cref="IOrdinate"/> print a string that mirrors the one printed by the <see cref="Statistics.IDistribution.Print(bool)"/> function. </item>
        /// <item> <see cref="IOrdinateEnum.Constant"/> values print a string in the format "Double(value: x), where x is the value of the constant. </item>
        /// </list>
        /// </returns>
        string Print(bool round = false);
        /// <summary>
        /// A method for serializing the <see cref="IOrdinate"/> object to an XML format for persistence in the database.
        /// </summary>
        /// <returns> An <see cref="XElement"/> containing the <see cref="IOrdinate"/> object's attributes. </returns>
        XElement WriteToXML();
        #endregion
    }
}
