using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{
    /// <summary>
    /// Provides and interface for double precision numbers stored as distributions rather than static values.
    /// </summary>
    public interface IDistribution //IOrdinate<IDistribution>
    {
        #region Properties
        /// <summary>
        /// Enumerated type of the distribution.
        /// </summary>
        IDistributions Type { get; }

        double Mean { get; }
        double Median { get;}
        double Variance { get;  }
        double StandardDeviation { get; }
        double Skewness { get; }
        double Minimum { get; }
        double Maximum { get; }
        /// <summary>
        /// The sample size used to fit the distribution.
        /// </summary>
        /// <remarks> If the distribution was not fit from a sample use the desired length of samples or <see cref="int.MaxValue"/> if the distribution is assumed to be a population distribution. </remarks>
        int SampleSize { get; }
        #endregion

        #region Functions
        /// <summary>
        /// Computes the density of the distribution at the point x.
        /// </summary>
        /// <param name="x"> A value from the distribution. </param>
        /// <returns> The portion of the distibution found at the point of <paramref name="x"/>. </returns>
        double PDF(double x);        
        /// <summary>
        /// Computes the CDF of the distribution.
        /// </summary>
        /// <param name="x"> A value from the distribution. </param>
        /// <returns> The non-exceedance probability of <paramref name="x"/>. </returns>
        double CDF(double x);
        /// <summary>
        /// Computes the inverse CDF of the distribution.
        /// </summary>
        /// <param name="p"> A non-exceedance probability. </param>
        /// <returns> The value from the distribution at the point of the non-exceedance probability <paramref name="p"/>.</returns>
        double InverseCDF(double p);
        /// <summary>
        /// Draws a single random value from the distribution.
        /// </summary>
        /// <param name="numberGenerator"> A <see cref="Random"/> Number Generator. If left <see langword="null""/> a random number generator is constructed by the function.</param>
        /// <returns> A single double value drawn from the distribution. </returns>
        double Sample(Random numberGenerator);
        
        ///// <summary>
        ///// Draws a random sample from the distribution, with the same length as the distribution <see cref="IDistribution.SampleSize"/>.
        ///// </summary>
        ///// <param name="numberGenerator"> A <see cref="Random"/> Number Generator. If it is <see langword="null""/> a random number generator is constructed by the function. </param>
        ///// <returns> An array of double values drawn from the distribution. </returns>
        /////double[] Sample(Random numberGenerator = null);
        
        /// <summary>
        /// Draws a random sample from the distribution, with the specified length (<paramref name="sampleSize"/>).
        /// </summary>
        /// <param name="numberGenerator"> A <see cref="Random"/> Number Generator. If left <see langword="null""/> a random number generator is constructed by the function. </param>
        /// <param name="sampleSize"> The desired sample size. </param>
        /// <returns> An array of double values drawn from the distribution.</returns>
        double[] Sample(int sampleSize, Random numberGenerator = null);
        /// <summary>
        /// Uses parametric bootstrapping technique to draw a new distribution.
        /// </summary>
        /// <param name="numberGenerator"> A <see cref="Random"/> Number Generator. If left <see langword="null"/> a random number generator is constructed by the function.  </param>
        /// <returns> A new distribution based on the a sample of <see cref="SampleSize"/> values drawn from the distribution. </returns>
        IDistribution SampleDistribution(Random numberGenerator = null);
        /// <summary>
        /// Prints a string describing the distribution.
        /// </summary>
        /// <returns> A string in the form: <see cref="IDistribution.Type"/>(parameter1: value, parameter2: value, ...). </returns>
        string Print();
        /// <summary>
        /// Compares two distributions for value equality.
        /// </summary>
        /// <param name="distribution"> The distribution to be compared to the instance distribution. </param>
        /// <returns> True if the two distributions are equivalent, false otherwise. </returns>
        bool Equals(IDistribution distribution);
        #endregion
    }
}
