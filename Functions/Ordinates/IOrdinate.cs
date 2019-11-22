using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    /// <summary>
    /// An interface for invariant and distributed numeric function parameters
    /// </summary>
    public interface IOrdinate
    {
        /// <summary>
        /// Provides the lower and upper bound of the scalar values
        /// </summary>
        Tuple<double, double> Range { get; }
        /// <summary>
        /// Returns the value of the scalar parameter as a double
        /// </summary>
        /// <param name="p"> The non-exceedance value for distributed parameters </param>
        /// <returns> A double representing either the value of the invariant parameter or value for the specified non-exceedance value of the distributed parameter </returns>
        double Value(double p = 0.50);

        bool Equals(IOrdinate scalar);

        /// <summary>
        /// True if the parameter is distributed or variant in anyway, false otherwise
        /// </summary>
        bool IsDistributed { get; }
        /// <summary>
        /// Provides a string representation of the parameter
        /// </summary>
        /// <returns> A string summarizing the parameter values </returns>
        string Print();

        string WriteToXML();
    }
}
