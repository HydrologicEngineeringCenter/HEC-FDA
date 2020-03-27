using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{
    /// <summary>
    /// Provides and interface for double precision numbers stored as statistical distributions rather than static values.
    /// </summary>
    public interface IDistribution: Utilities.ISerializeToXML<IDistribution>, Utilities.IMessagePublisher
    {
        #region Properties
        /// <summary>
        /// The type of the statistical distribution (e.g. Normal, Triangular, etc.). 
        /// Supported distributions are listed in <see cref="IDistributionEnum"/> set of enumerated values.
        /// </summary>
        IDistributionEnum Type { get; }
        /// <summary>
        /// The arithmetic average value of the distribution.
        /// </summary>
        double Mean { get; }
        /// <summary>
        /// The "middle" value of the distribution. 
        /// The value of the distribution which separates the smaller half of values from the larger half of values.
        /// </summary>
        double Median { get;}
        /// <summary>
        /// The most likely value.
        /// </summary>
        double Mode { get; }
        /// <summary>
        /// The expected squared deviation of the distribution values from the distribution <see cref="IDistribution.Mean"/>.
        /// </summary>
        double Variance { get;  }
        /// <summary>
        /// The expected absolute value for the deviation of the distribution values from the distribution <see cref="IDistribution.Mean"/>. It is the square root of the <see cref="IDistribution.Variance"/>.
        /// </summary>
        double StandardDeviation { get; }
        /// <summary>
        /// Measures the asymmetry of the distribution.
        /// Zero skew values represent a symmetrical distribution.
        /// Negative skew values represent a left side skew or skew toward values below the mean.
        /// Positive skew values represent a right side skew or skew toward values above the mean.
        /// Skewness can be thought of as the cubed deviation of the distribution values from the mean.
        /// </summary>
        double Skewness { get; }
        /// <summary>
        /// The maximum and minimum value for the distribution.
        /// For continuous distributions which span the real number line from negative to positive infinity, this is approximated a value very close to <see cref="double.NegativeInfinity"/> or <see cref="double.PositiveInfinity"/>.
        /// </summary>
        Utilities.IRange<double> Range { get; }    
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
        /// <returns> The portion of the distribution found at the point of <paramref name="x"/>. </returns>
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
        /// Prints a string describing the distribution.
        /// </summary>
        /// <param name="round"> <see langword="true"/> if some values should be rounded to produce a more readable string. </param>
        /// <returns> A string in the form: <see cref="IDistribution.Type"/>(parameter1: value, parameter2: value, ...). </returns>
        string Print(bool round = false);
        /// <summary>
        /// Prints a string describing the parameterization requirements for the distribution.
        /// </summary>
        /// <param name="printNotes"> Additional notes on the valid parameterization of the distribution, if any exist. </param>
        /// <returns></returns>
        string Requirements(bool printNotes);
        /// <summary>
        /// Compares 2 distributions for <b>value</b> equality.
        /// </summary>
        /// <param name="distribution"> The distribution to be compared to the instance distribution. </param>
        /// <returns> True if the two distributions are equivalent, false otherwise. </returns>
        bool Equals(IDistribution distribution);
        #endregion

        ///// <summary>
        ///// Draws a single random value from the distribution.
        ///// </summary>
        ///// <param name="numberGenerator"> A <see cref="Random"/> Number Generator. If left <see langword="null""/> a random number generator is constructed by the function.</param>
        ///// <returns> A single double value drawn from the distribution. </returns>
        //double Sample(Random numberGenerator);
        ///// <summary>
        ///// Draws a random sample from the distribution, with the specified length (<paramref name="sampleSize"/>).
        ///// </summary>
        ///// <param name="numberGenerator"> A <see cref="Random"/> Number Generator. If left <see langword="null""/> a random number generator is constructed by the function. </param>
        ///// <param name="sampleSize"> The desired sample size. </param>
        ///// <returns> An array of double values drawn from the distribution.</returns>
        //double[] Sample(int sampleSize, Random numberGenerator = null);
        ///// <summary>
        ///// Uses parametric bootstrapping technique to draw a new distribution.
        ///// </summary>
        ///// <param name="numberGenerator"> A <see cref="Random"/> Number Generator. If left <see langword="null"/> a random number generator is constructed by the function.  </param>
        ///// <returns> A new distribution based on the a sample of <see cref="SampleSize"/> values drawn from the distribution. </returns>
        //IDistribution SampleDistribution(Random numberGenerator = null);
    }
}
