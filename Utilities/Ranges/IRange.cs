using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// Defines the minimum and maximum values of a numerical range.
    /// </summary>
    public interface IRange<T>
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
        /// <returns> A string representing the range. </returns>
        string Print();
        bool Equals<U>(IRange<U> range); 
    }
}
