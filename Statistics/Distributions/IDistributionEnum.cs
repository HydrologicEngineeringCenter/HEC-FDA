using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{
    /// <summary>
    /// An enumeration of supported IDistribution types
    /// </summary>
    public enum IDistributionEnum
    {
        /// <summary>
        /// Default value likely to lead to an error.
        /// </summary>
        NotSupported = 0,
        /// <summary>
        /// Normal distribution.
        /// </summary>
        Normal = 1,
        /// <summary>
        /// Continuous Uniform distribution.
        /// </summary>
        Uniform = 2,
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
        /// Log Normal distribution.
        /// </summary>
        LogNormal = 7,
        /// <summary>
        /// Deterministic Distribution
        /// </summary>
        Deterministic = 8,
        /// <summary> 
        /// Empirical Distribution
        /// </summary>
        Empirical = 9,
        /// <summary>
        /// Graphical Distribution where uncertainty is defined using the Less Simple Method.
        /// </summary>
        Graphical = 10,
        /// <summary>
        /// Specific form of a Truncated Normal Distribution with density above or below truncation value(s) reassigned to the truncation value(s). However, the reported measures of central tendency and dispersion continue to be those of the underlying distribution NOT the truncated copy.
        /// </summary>
        TruncatedNormal = 101,
        /// <summary>
        /// Specific form of a Truncated Histogram with density above or below truncation value(s) (e.g. typical upper and lower bounds) reassigned to the truncation value(s). However, the reported measures of central tendency and dispersion continue to be those of the underlying distribution NOT the truncated copy.
        /// </summary>
        TruncatedHistogram = 50,
    }
}
