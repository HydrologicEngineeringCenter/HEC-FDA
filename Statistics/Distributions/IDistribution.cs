using System.Xml.Linq;

namespace Statistics
{
    /// <summary>
    /// Provides and interface for double precision numbers stored as statistical distributions rather than static values.
    /// </summary>
    public interface IDistribution
    {
        #region Properties
        /// <summary>
        /// The type of the statistical distribution (e.g. Normal, Triangular, etc.). 
        /// Supported distributions are listed in <see cref="IDistributionEnum"/> set of enumerated values.
        /// </summary>
        IDistributionEnum Type { get; } 
        /// <summary>
        /// The sample size used to fit the distribution.
        /// </summary>
        /// <remarks> If the distribution was not fit from a sample use the desired length of samples or <see cref="int.MaxValue"/> if the distribution is assumed to be a population distribution. </remarks>
        int SampleSize { get; }
        bool Truncated { get; }
        #endregion
        #region Voids
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
        XElement ToXML();
        IDistribution Sample(double[] randomPacket);
        IDistribution Fit(double[] data);
        #endregion

    }
}
