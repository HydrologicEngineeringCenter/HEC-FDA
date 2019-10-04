using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{
    /// <summary>
    /// An interface for testing the statistical converge of sample data
    /// </summary>
    public interface IConverge
    {
        /// <summary>
        /// True value of the sample percentile has converged within the range of tolerance at the specified confidence level, false otherwise. 
        /// </summary>
        bool IsConverged { get; }
        /// <summary>
        /// The quantile [0, 1] at whcih convergence is tested, set to 0.50 by default.
        /// </summary>
        double Percentile { get; }
        /// <summary>
        /// The confidence level [0, 1] for convergence, set to 0.95 by default.
        /// </summary>
        double ConfidenceLevel { get; }
        /// <summary>
        /// Acceptable deviation in percentile value as a portion of the percentile value [0, 1], set to  0.10 by default. This sets the confidence interval for the test.
        /// </summary>
        double Tolerance { get; }
        /// <summary>
        /// The minimum number of observations that must be collected before convergence is tested, set to 30 by default.
        /// </summary>
        int MinimumSampleSize { get; }
        /// <summary>
        /// The maximum number of observations to be collected before convergence is 'assumed', set to <see cref="int.MaxValue"/> by default.
        /// </summary>
        int MaximumSampleSize { get; }
        
    }
}
