using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Utilities
{
    public static class Validation
    {
        /// <summary>
        /// Checks an object for a null value.
        /// </summary>
        /// <param name="thing"> Object to check for null value. </param>
        /// <param name="throwsNullArgumentException"> if true a null argument exception is thrown when a null object is specified </param>
        /// <returns>  A boolean value or null argument exception (depending on value of the thowsNullArgumentException parameter) </returns>
        public static bool IsNull(object thing, bool throwsNullArgumentException = true) => thing == null ? throwsNullArgumentException ? throw new ArgumentNullException() : true : false;
        /// <summary>
        /// Checks a collection for a null value or count of 0
        /// </summary>
        /// <typeparam name="T"> The collection type </typeparam>
        /// <param name="collection"> The collection to check for a null value or count of 0</param>
        /// <param name="throwsArgumentException"> If true a argument exception is thrown when a null or empty collection is encountered If false the boolean value true is returned when a null or empty collection is encountered.</param>
        /// <returns> A boolean value or arguement exception depending on the specified collection and throwsArguemntException parameters </returns>
        public static bool IsNullOrEmptyCollection<T>(ICollection<T> collection, bool throwsArgumentException = true)
        {
            if (IsNull(collection, false) || collection.Count == 0 || IsNullItemInCollection(collection))
                if (throwsArgumentException) throw new ArgumentException("The specified collection is invalid because it is empty or contains null values.");
                else return true;
            else return false;
        }
        private static bool IsNullItemInCollection<T>(ICollection<T> collection)
        {
            foreach (var i in collection) if (i == null) return true;
            return false;
        }

        /// <summary>
        /// Checks a double for a finite representation of its value.
        /// </summary>
        /// <param name="val"> Double value to check </param>
        /// <param name="throwsArgumentOutOfRangeException"> if true a argument out of range expection is thrown when a non-finite double value is specified. </param>
        /// <returns> A boolean value or argument out of range exception (depending on the value of the throwsArgumentOutOfRangeException parameter) </returns>
        public static bool IsFinite(double val, bool throwsArgumentOutOfRangeException = true) => double.IsNaN(val) || double.IsInfinity(val) ? throwsArgumentOutOfRangeException ? throw new ArgumentOutOfRangeException(string.Format("{0} is invalid because it cannot be represented as a finite value.", val)) : false : true;
        /// <summary>
        /// Truncates an existing range of values on a specified range
        /// </summary>
        /// <param name="min"> The smallest allowable value on the new range </param>
        /// <param name="max"> The largest allowable value on the new range </param>
        /// <param name="unTruncatedRange"></param>
        /// <returns> The most restrictive range possible based on the specified minimum, maximum and untruncated range </returns>
        internal static Tuple<double, double> Truncate(double min, double max, Tuple<double, double> unTruncatedRange)
        {
            if (min > max) throw new ArithmeticException(string.Format("The specified truncated range: [{0}, {1}] is invalid becasue the minimum value: {0} is larger than the maximum value: {1}.", min, max));
            if (min > unTruncatedRange.Item2 || max < unTruncatedRange.Item1) throw new ArgumentOutOfRangeException("The specified truncated range: [" + min + ", " + max + "] is invalid because it does not overlap the provided untruncated range: [" + unTruncatedRange.Item1 + ", " + unTruncatedRange.Item2 + "].");
            return new Tuple<double, double>(Math.Max(min, unTruncatedRange.Item1), Math.Min(max, unTruncatedRange.Item2));
        }
    }
}
