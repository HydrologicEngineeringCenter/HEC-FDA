using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{
    /// <summary>
    /// Provides an interface for the Bins that make up Histograms
    /// </summary>
    public interface IBin: Utilities.IMessagePublisher
    {
        /// <summary>
        /// Inclusive minimum and exclusive maximum bin values.
        /// </summary>
        Utilities.IRange<double> Range { get; }
        /// <summary>
        /// The <see cref="IBin"/> midpoint value.
        /// </summary>
        /// <remarks>
        /// This value is used to perform most histogram operations.
        /// </remarks>
        double MidPoint { get; }
        /// <summary>
        /// The number of items in the bin.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Compares two bins for <b>value</b> equality.
        /// </summary>
        /// <param name="bin"> The <see cref="IBin"/> to be compared to the instance <see cref="IBin"/>. </param>
        /// <returns> True if the bins are equivalent, false otherwise. </returns>
        bool Equals(IBin bin);
        /// <summary>
        /// Prints a representation of the <see cref="IBin"/> as a string.
        /// </summary>
        /// <param name="round"> <see langword="true"/> if values should be rounded to produce a more readable string. </param>
        /// <returns> A string in the form: Bin(count: value, range: [min, max)). </returns>
        string Print(bool round = false);
    }
}
