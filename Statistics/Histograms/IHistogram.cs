using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{
    /// <summary>
    /// An interface for Histograms of binned data.
    /// </summary>
    public interface IHistogram: IDistribution, Utilities.IValidate<IHistogram> 
    {
        
        #region Properties
        /// <summary>
        /// A set of bins containing the histogram data.
        /// </summary>
        IBin[] Bins{ get; }
        #endregion

        #region Functions
        /// <summary>
        /// Compares two histograms for value equality.
        /// </summary>
        /// <param name="histogram"> The histogram to be compared to the instance histogram. </param>
        /// <returns> True if the two histograms are equivalent, false otherwise. </returns>
        bool Equals(IHistogram histogram);
        #endregion

        ///// <summary>
        ///// Generates a new histogram by adding a new sample to the pre-existing histogram's bins.
        ///// </summary>
        ///// <param name="sample"> Sample data to add to the histogram. </param>
        ///// <returns> A new histogram. </returns>
        //IHistogram AddSample(IEnumerable<double> sample);
    }
}
