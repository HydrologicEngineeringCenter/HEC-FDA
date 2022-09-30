namespace HEC.FDA.Statistics.Distributions
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
        Triangular = 3,
        /// <summary>
        /// Log Pearson Type III distribution.
        /// </summary>
        LogPearsonIII = 4,
        /// <summary>
        /// Log Normal distribution.
        /// </summary>
        LogNormal = 5,
        /// <summary>
        /// Deterministic Distribution
        /// </summary>
        Deterministic = 6,
        /// <summary>
        /// IHistogram includes Histogram and ThreadsafeInlineHistogram 
        /// </summary>
        IHistogram = 7,
        /// <summary>
        /// Specific form of a Truncated Normal Distribution with density above or below truncation value(s) reassigned to the truncation value(s). However, the reported measures of central tendency and dispersion continue to be those of the underlying distribution NOT the truncated copy.
        /// </summary>
        TruncatedNormal = 101,

    }
}
