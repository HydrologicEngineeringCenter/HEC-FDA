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
        /// Provides a human readable representation of an <see cref="double"/> value, using rounding to reduce decimal values to 2 digits and rounding to shorten very small or large values.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string Print(this double x) => x.IsOnRange(-1000000, 1000000, false, false) ? string.Format("{0:n}", x) : x.ToString("E2");
        /// <summary>
        /// Provides a human readable representation of an <see cref="int"/> value, using scientific notation to shorten the representation of very small or large values.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string Print(this int x) => x.IsOnRange(-1000000, 1000000, false, false) ? string.Format("{0:n0}", x) : x.ToString("E2");
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
        /// <returns> A boolean value or arguement exception depending on the specified collection and throwsArguemntException parameters </returns>
        public static bool IsNullOrEmptyCollection<T>(ICollection<T> collection, bool throwsArgumentException = true)
        {
            if (IsNull(collection) || collection.Count == 0 || IsNullItemInCollection(collection))
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
        /// Test that the <paramref name="min"/> &lt <paramref name="max"/>. Warning: <see cref="double.NaN"/> may produce unexpected results.
        /// </summary>
        /// <typeparam name="T"> An <see cref="IComparable"/> type parameter.</typeparam>
        /// <param name="min"> The minimum of the range. </param>
        /// <param name="max"> The maximum of the range. </param>
        /// <returns> <see langword="true"/> if <paramref name="min"/> &lt <paramref name="max"/> or <see langword="false"/> otherwise. </returns>
        public static bool IsRange<T>(T min, T max) where T : IComparable => min.CompareTo(max) < 0 ? true : false;
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
        public static bool IsOnRange<T>(this T value, T min, T max, bool inclusiveMin = true, bool inclusiveMax = true) where T : IComparable => value.CompareTo(min) > (inclusiveMin ? -1 : 0) && value.CompareTo(max) < (inclusiveMax ? 1 : 0);
        /// <summary>
        /// Converts a <see cref="long"/> value to an <see cref="int"/> value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns> An <see cref="int"/> value. Returns the <see cref="int.MaxValue"/> if the <see cref="long"/> value exceeds the <see cref="int.MaxValue"/>. </returns>
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
        /// <summary>
        /// Evaluates the <paramref name="msgs"/> providing the most severe <see cref="IMessage.Level"/> in the group of messages.
        /// </summary>
        /// <param name="msgs"> An <see cref="IEnumerable{T}"/> set of <see cref="IMessage"/>s. </param>
        /// <returns> The most severe <see cref="IMessage.Level"/> in <paramref name="msgs"/>. </returns>
        public static Utilities.IMessageLevels Max(this IEnumerable<Utilities.IMessage> msgs)
        {
            Utilities.IMessageLevels level = Utilities.IMessageLevels.NotSet;
            foreach (Utilities.IMessage msg in msgs)
            {
                if (msg.Level == Utilities.IMessageLevels.FatalError) return Utilities.IMessageLevels.FatalError;
                if (msg.Level > level) level = msg.Level;
            }
            return level;
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
            return $"The {cls}.{op} operation cannot be performed because the {cls} is invalid. " +
                    $"It contains the following list of messages:" +
                    $"{PrintTabbedListOfMessages(msgs)}";
        }
        /// <summary>
        /// Takes an <see cref="IEnumerable{T}"/> set of <see cref="IMessage"/>s and prints them as multi-line string.
        /// </summary>
        /// <param name="msgs"></param>
        /// <returns> A string representation of the <paramref name="msgs"/>, with each message printed on its own line in the format: [<see cref="IMessage.Level"/>] <see cref="IMessage.Notice"/> </returns>
        public static string PrintTabbedListOfMessages(IEnumerable<IMessage> msgs)
        {
            string msg = "";
            foreach (var m in msgs) msg += $"\n\t[{m.Level}] {m.Notice}";
            return msg;
        }
    }
}
