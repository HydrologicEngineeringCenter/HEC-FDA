using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    /// <summary>
    /// Enumerates supported ordinate types, mirrors <see cref="Statistics.IDistributionEnum"/>.
    /// </summary>
    public enum IOrdinateEnum
    {
        /// <summary>
        /// Default value likely to lead to an error.
        /// </summary>
        NotSupported = -1,
        /// <summary>
        /// Single y value for every x.
        /// </summary>
        Constant = 0,
        /// <summary>
        /// Normal distribution.
        /// </summary>
        Normal = 1,
        /// <summary>
        /// Continuous Uniform distribution.
        /// </summary>
        Uniform = 2,
        /// <summary>
        /// Scaled Beta distribution.
        /// </summary>
        Beta4Parameters = 3,
        /// <summary>
        /// Triangular distribution.
        /// </summary>
        Triangular = 4,
        /// <summary>
        /// Histogram constructed from recorded data observations.
        /// </summary>
        Histogram = 5,
        /// <summary>
        /// Log Pearson Type III distribution.
        /// </summary>
        LogPearsonIII = 6,
        /// <summary>
        /// Specific form of a Truncated Normal Distribution with density above or below truncation value(s) reassigned to the truncation value(s). However, the reported measures of central tendency and dispersion continue to be those of the underlying distribution NOT the truncated copy.
        /// </summary>
        TruncatedNormal = 10,
        /// <summary>
        /// Specific form of a Truncated Uniform Distribution with density above or below truncation value(s) reassigned to the truncation value(s). However, the reported measures of central tendency and dispersion continue to be those of the underlying distribution NOT the truncated copy.
        /// </summary>
        TruncatedUniform = 20,
        /// <summary>
        /// Specific form of a Truncated 4 Parameter Beta Distribution with density above or below truncation value(s) (e.g. typical upper and lower bounds) reassigned to the truncation value(s). However, the reported measures of central tendency and dispersion continue to be those of the underlying distribution NOT the truncated copy.
        /// </summary>
        TruncatedBeta4Parameter = 30,
        /// <summary>
        /// Specific form of a Truncated Triangular Distribution with density above or below truncation value(s) (e.g. typical upper and lower bounds) reassigned to the truncation value(s). However, the reported measures of central tendency and dispersion continue to be those of the underlying distribution NOT the truncated copy.
        /// </summary>
        TruncatedTriangular = 40,
        /// <summary>
        /// Specific form of a Truncated Histogram with density above or below truncation value(s) (e.g. typical upper and lower bounds) reassigned to the truncation value(s). However, the reported measures of central tendency and dispersion continue to be those of the underlying distribution NOT the truncated copy.
        /// </summary>
        TruncatedHistogram = 50,
    }
}
