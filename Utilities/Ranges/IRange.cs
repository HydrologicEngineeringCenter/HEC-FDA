using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// Defines the minimum and maximum values of a numerical range.
    /// </summary>
    public interface IRange<T>: IMessagePublisher
    {
        /// <summary>
        /// The minimum value.
        /// </summary>
        T Min { get; }
        /// <summary>
        /// The maximum value.
        /// </summary>
        T Max { get; }
        /// <summary>
        /// Prints a representation of the range.
        /// </summary>
        /// <param name="round"> <see langword="true"/> if some values should be rounded to produce a more readable string. </param>
        /// <returns> A string representing the range. </returns>
        string Print(bool round = false);
        bool Equals<U>(IRange<U> range); 
    }
}
