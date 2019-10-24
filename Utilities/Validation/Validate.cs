using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities
{
    /// <summary>
    /// Provides <see langword="static"/> functions for use in the validation of model parameters.
    /// </summary>
    public static class Validate
    {
        /// <summary>
        /// Extension function tests for <see langword="null"/> references.
        /// </summary>
        /// <param name="obj"> The<see cref="object"/> to be checked for a <see langword="null"/> value. </param>
        /// <returns> <see langword="true"/> if the specified <see cref="object"/> is <see langword="null"/>, <see langword="false"/> otherwise. </returns>
        public static bool IsNull(this object obj) => obj == null ? true : false;
        /// <summary>
        /// Extension function tests <see cref="double"/> variables for finite numerical values.
        /// </summary>
        /// <param name="x"> The <see cref="double"/> value to be tested. </param>
        /// <returns> <see langword="true"/> if the specified <see cref="double"/> a finite numerical value. <see langword="false"/> if the <see cref="double"/> value is <see cref="double.NaN"/>, <see cref="double.PositiveInfinity"/>, or <see cref="double.NegativeInfinity"/>. </returns>
        public static bool IsFinite(this double x) => !double.IsNaN(x) && !double.IsInfinity(x) ? true : false;
        /// <summary>
        /// Tests an <see cref="IEnumerable{T}"/> for <see langword="null"/> or empty data sets.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"> The <see cref="IEnumerable{T}"/> data to be tested. </param>
        /// <returns> <see langword="true"/> if the specified <see cref="IEnumerable{T}"/> is not <see langword="null"/> and contains one or more elements, <see langword="false"/> otherwise. </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> data) => data.IsNull() || !data.Any() ? true : false;
        /// <summary>
        /// Tests a specified <see cref="IEnumerable{T}"/> of <see cref="Type"/> <see cref="double"/> for a required number of finite elements.
        /// </summary>
        /// <param name="data"> The specified <see cref="IEnumerable{T}"/> of <see cref="Type"/> <see cref="double"/> values to test. </param>
        /// <param name="nRequiredFiniteElements"> The required number finite elements. </param>
        /// <returns> <see langword="true"/> if the specified <see cref="IEnumerable{T}"/> of <see cref="double"/> <see langword="false"/> otherwise. </returns>
        public static bool IsFiniteSample(this IEnumerable<double> data, int nRequiredFiniteElements = 1)
        {
            if (data.IsNull()) throw new ArgumentNullException(paramName: nameof(data), "The sample cannot be evaluated because it is null.");
            if (nRequiredFiniteElements < 1) throw new ArgumentOutOfRangeException(paramName: nameof(nRequiredFiniteElements), message:$"The specified required number of finite elements {nRequiredFiniteElements} is invalid because it not a greater than or equal to 1.");
            var n = data.Count(x => x.IsFinite());
            return n >= nRequiredFiniteElements;
        }
        /// <summary>
        /// Test that the <paramref name="min"/> &lt <paramref name="max"/>. Warning: <see cref="double.NaN"/> may produce unexpected results.
        /// </summary>
        /// <typeparam name="T"> An <see cref="IComparable"/> type parameter.</typeparam>
        /// <param name="min"> The minimum of the range. </param>
        /// <param name="max"> The maximum of the range. </param>
        /// <returns> <see langword="true"/> if <paramref name="min"/> &lt <paramref name="max"/> or <see langword="false"/> otherwise. </returns>
        public static bool IsRange<T>(T min, T max) where T : IComparable => min.CompareTo(max) < 0 ? true : false;
        public static bool IsOnRange<T>(this T value, T min, T max, bool inclusiveMin = true, bool inclusiveMax = true) where T : IComparable => value.CompareTo(min) > (inclusiveMin ? -1 : 0) && value.CompareTo(max) < (inclusiveMax ? 1 : 0);
        public static int CastToInt(this long value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch (OverflowException)
            {
                return int.MaxValue;
            }
        }
    }
}
