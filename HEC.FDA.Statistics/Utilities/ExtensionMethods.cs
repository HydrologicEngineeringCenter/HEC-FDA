using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// Extension methods for primitives and utilities objects for use in the Fda solution.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Extension function tests <see cref="double"/> variables for finite numerical values.
        /// </summary>
        /// <param name="x"> The <see cref="double"/> value to be tested. </param>
        /// <returns> <see langword="true"/> if the specified <see cref="double"/> a finite numerical value. <see langword="false"/> if the <see cref="double"/> value is <see cref="double.NaN"/>, <see cref="double.PositiveInfinity"/>, or <see cref="double.NegativeInfinity"/>. </returns>
        public static bool IsFinite(this double x) => !double.IsNaN(x) && !double.IsInfinity(x) ? true : false;
        /// <summary>
        /// Tests if the value is on the range specified by the <paramref name="min"/> and <paramref name="max"/> parameters.
        /// </summary>
        /// <typeparam name="T"> A <see cref="Type"/> that implements the <see cref="IComparable"/> interface. </typeparam>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="inclusiveMin"> <see langword="true"/> if the <paramref name="min"/> is a valid part of the range, <see langword="false"/> otherwise. </param>
        /// <param name="inclusiveMax"> <see langword="true"/> if the <paramref name="max"/> is a valid part of the range, <see langword="false"/> otherwise. </param>
        /// <returns> <see langword="true"/> if the value is on the specified range, <see langword="false"/> otherwise. </returns>
        public static bool IsOnRange<T>(this T value, T min, T max, bool inclusiveMin = true, bool inclusiveMax = true) where T : INumber<T>
        {
            return value.CompareTo(min) > (inclusiveMin ? -1 : 0) && value.CompareTo(max) < (inclusiveMax ? 1 : 0);
        }
        /// <summary>
        /// Tests an <see cref="IEnumerable{T}"/> for <see langword="null"/> or empty data sets.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"> The <see cref="IEnumerable{T}"/> data to be tested. </param>
        /// <returns> <see langword="true"/> if the specified <see cref="IEnumerable{T}"/> is not <see langword="null"/> and contains one or more elements, <see langword="false"/> otherwise. </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> data) => data == null || !data.Any() ? true : false;
    }
}
