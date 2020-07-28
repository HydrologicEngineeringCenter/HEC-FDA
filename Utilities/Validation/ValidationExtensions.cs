using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities
{
    /// <summary>
    /// Provides <see langword="static"/> functions for use in the validation of model parameters.
    /// </summary>
    public static class ValidationExtensions
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
        /// Extension function tests <see cref="IRange{T}"/> variables for finite numerical values.
        /// </summary>
        /// <param name="range"> The <see cref="IRange{T}"/> to be tested. </param>
        /// <returns> <see langword="true"/> if the specified <see cref="IRange{T}"/> contains finite numerical <see cref="IRange{T}.Min"/> and <see cref="IRange{T}.Max"/> values. <see langword="false"/> if the <see cref="IRange{T}.Min"/> or <see cref="IRange{T}.Max"/> values are <see cref="double.NaN"/>, <see cref="double.PositiveInfinity"/>, or <see cref="double.NegativeInfinity"/>. </returns>
        public static bool IsFinite(this IRange<double> range) => range.Min.IsFinite() && range.Max.IsFinite();
        /// <summary>
        /// Tests an <see cref="IEnumerable{T}"/> for <see langword="null"/> or empty data sets.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"> The <see cref="IEnumerable{T}"/> data to be tested. </param>
        /// <returns> <see langword="true"/> if the specified <see cref="IEnumerable{T}"/> is not <see langword="null"/> and contains one or more elements, <see langword="false"/> otherwise. </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> data) => data.IsNull() || !data.Any() ? true : false;
        /// <summary>
        /// Tests if an <see cref="IEnumerable{T}"/> is <see langword="null"/> or contains <see langword="null"/> data elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns> <see langword="true"/> if <see langword="null"/> values are encountered, <see langword="false"/> otherwise. </returns>
        public static bool IsNullItem<T>(this IEnumerable<T> data)
        {
            if (data.IsNullOrEmpty()) return true;
            else
            {
                foreach (var x in data) if(x.IsNull()) return true;
                return false;
            }  
        }
        /// <summary>
        /// Tests if an <see cref="IEnumerable{T}"/> is <see langword="null"/> or contains <see langword="null"/> or non-finite numeric data elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns> <see langword="true"/> if <see langword="null"/> values are encountered, <see langword="false"/> otherwise. </returns>
        public static bool IsNullOrNonFiniteItem(this IEnumerable<double> data)
        {
            if (data.IsNullOrEmpty()) return true;
            else
            {
                foreach (var x in data) if (!x.IsFinite()) return true;
                return false;
            }
        }
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
        /// Checks a collection for a null value or count of 0
        /// </summary>
        /// <typeparam name="T"> The collection type </typeparam>
        /// <param name="collection"> The collection to check for a null value or count of 0</param>
        /// <param name="throwsArgumentException"> If true a argument exception is thrown when a null or empty collection is encountered If false the boolean value true is returned when a null or empty collection is encountered.</param>
        /// <returns> A boolean value or argument exception depending on the specified collection and throwsArguemntException parameters </returns>
        public static bool IsNullOrEmptyCollection<T>(ICollection<T> collection, bool throwsArgumentException = true)
        {
            if (IsNull(collection) || collection.Count == 0 || IsNullItemInCollection(collection))
                if (throwsArgumentException) throw new ArgumentException("The specified collection is invalid because it is empty or contains null values.");
                else return true;
            else return false;
        }
        private static bool IsNullItemInCollection<T>(ICollection<T> collection)
        {
            foreach (var i in collection) if (i.IsNull()) return true;
            return false;
        }
        /// <summary>
        /// Test that the <paramref name="min"/> &lt <paramref name="max"/>.
        /// </summary>
        /// <typeparam name="T"> An <see cref="IComparable"/> type parameter.</typeparam>
        /// <param name="min"> The minimum of the range. </param>
        /// <param name="max"> The maximum of the range. </param>
        /// <param name="finiteRequirement"> <see langword="true"/> If the range must be finite (i.e. must <b>not</b> contain <see cref="double.NegativeInfinity"/>, <see cref="double.PositiveInfinity"/>, or <see cref="double.NaN"/> values), <see langword="false"/> by default. </param>
        /// <param name="notSingleValueRequirement"> <see langword="true"/> If the range must contain more than a single value, <see langword="false"/> by default. </param>
        /// <returns> <see langword="true"/> if <paramref name="min"/> &lt <paramref name="max"/> or <see langword="false"/> otherwise. </returns>
        /// <remarks> If the <paramref name="min"/> equals the <paramref name="max"/> <see langword="true"/> is returned. Note that <see cref="double.NaN"/> values may produce unexpected results. </remarks>
        public static bool IsRange<T>(T min, T max, bool finiteRequirement = false, bool notSingleValueRequirement = false) where T : IComparable //=> min.CompareTo(max) < (allowSingleValueRange ? -1 : 0) ? true : false;
        {
            /*
             * The CompareTo() compares the instance to the parameter value.
             *      a - returns -1 if min < max
             *      b - returns  0 if min = max
             *      c - returns  1 if max > min
             * If a single value range is allowed, then:
             *      - must return -1 (min < max) or 0 (min = max) e.g. a or b is allowed.
             * Otherwise:
             *      - must return -1 (min < max) only a is allowed.
             */
            if (finiteRequirement && typeof(T) == typeof(double) && !Convert.ToDouble(min).IsFinite() || !Convert.ToDouble(max).IsFinite()) return false;
            else
            {
                int threshold = notSingleValueRequirement ? 0 : 1, val = min.CompareTo(max);
                return val < threshold;
            }  
        }
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
        public static bool IsOnRange<T>(this T value, T min, T max, bool inclusiveMin = true, bool inclusiveMax = true) where T : IComparable
        {
            return value.CompareTo(min) > (inclusiveMin ? -1 : 0) && value.CompareTo(max) < (inclusiveMax ? 1 : 0);
        }
        public static bool IsOnRange<T>(this IRange<T> range, IRange<T> comparison)
        {
            //if ((double)range.Min).CompareTo((double)comparison.Min))
            //{

            //}
            return true;
        }
        /// <summary>
        /// Produces an <see cref="InvalidOperationException"/> message for objects implementing the <see cref="IValidate{T}"/> interface attempting an operation in an invalid state. (see: <seealso cref="IValidate{T}.IsValid"/>)
        /// </summary>
        /// <param name="cls"> The interface or class name attempting the operation. </param>
        /// <param name="msgs"> The set of messages provided by the <see cref="IMessagePublisher.Messages"/> property. </param>
        /// <param name="op"> The name of the method producing the exception. </param>
        /// <returns> A string listing the operation name and list of validation errors and messages associated with the <paramref name="cls"/> object. </returns>
        public static string InvalidOperationExceptionMessage(string cls, IEnumerable<IMessage> msgs, [System.Runtime.CompilerServices.CallerMemberName] string op = "")
        {
            return $"The {cls}.{op} operation cannot be performed because the {cls} is invalid. It contains the following list of messages:" +
                    $"{IMessageExtensions.PrintTabbedListOfMessages(msgs)}";
        }
        
    }
}
