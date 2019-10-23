using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Functions.Utilities
{
    /// <summary>
    /// A non-thread safe class providing numbers for sampling from distributions
    /// </summary>
    public interface ISampleGenerator
    {
        /// <summary>
        /// The total count of numbers provided (e.g. sample size)
        /// </summary>
        int N { get; set; }
        /// <summary>
        /// The random number seed, set to 0 for non-random samples
        /// </summary>
        int Seed { get; }

        /// <summary>
        /// Returns a sample of size n, updates the total sample size by incrementing it by n
        /// </summary>
        /// <param name="n"> The desired number of values </param>
        /// <returns> n double values </returns>
        double[] DrawSample(int n);
    }
}
