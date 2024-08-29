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
        /// The minimum (inclusive) value.
        /// </summary>
        T Min { get; }
        /// <summary>
        /// The maximum (inclusive) value.
        /// </summary>
        T Max { get; }       
        /// <summary>
        /// Prints a representation of the range.
        /// </summary>
        /// <param name="round"> <see langword="true"/> if some values should be rounded to produce a more readable string. </param>
        /// <returns> A string representing the range. </returns>
        string Print(bool round = false);
        /// <summary>
        /// Compares 2 <see cref="IRange{T}"/> for value equality.
        /// </summary>
        /// <typeparam name="U"> The type of range (int, double). </typeparam>
        /// <param name="range"> The <see cref="IRange{T}"/> to be compared to the instance <see cref="IRange{T}"/>.</param>
        /// <returns><see langword="true"/> if the 2 <see cref="IRange{T}"/> are equal in value, <see langword="false"/> otherwise. </returns>
        bool Equals<U>(IRange<U> range);
        bool IsOnRange(T x);
    }
}
