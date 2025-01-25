namespace Statistics
{
    /// <summary>
    /// An enumeration of supported IDistribution types
    /// </summary>
    public enum IDistributionEnum
    {
        NotSupported = 0,
        Normal = 1,
        Uniform = 2,
        Triangular = 3,
        LogPearsonIII = 4,
        LogNormal = 5,
        Deterministic = 6,
        /// <summary>
        /// IHistogram includes Histogram and ThreadsafeInlineHistogram 
        /// The histogram is bin counts and bin starts 
        /// </summary>
        IHistogram = 7,
        /// <summary>
        /// Empirical distribution is frequencies and values 
        /// </summary>
        Empirical = 8,
        /// <summary>
        /// Specific form of a Truncated Normal Distribution with density above or below truncation value(s) reassigned to the truncation value(s). However, the reported measures of central tendency and dispersion continue to be those of the underlying distribution NOT the truncated copy.
        /// </summary>
        TruncatedNormal = 101,

    }
}
