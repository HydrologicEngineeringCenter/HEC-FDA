using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// Extension methods for primitives and utilities objects for use in the Fda solution.
    /// </summary>
    public static class ExtensionMethods
    {
        const double _3 = 0.001;
        const double _4 = 0.0001;
        const double _5 = 0.00001;
        const double _6 = 0.000001;
        const double _7 = 0.0000001;
        /// <summary>
        /// Checks <see cref="double"/> for value equality within 3 significant digits.
        /// </summary>
        /// <param name="left"> The instance <see cref="double"/>. </param>
        /// <param name="right"> The <see cref="double"/> to be compared to instance value. </param>
        /// <returns> <see langword="true"/> if the values are equal, <see langword="false"/> otherwise. </returns>
        public static bool Equals3DigitPrecision(this double left, double right)
        {
            return Math.Abs(left - right) < _3;
        }
        /// <summary>
        /// Checks <see cref="double"/> for value equality within 4 significant digits.
        /// </summary>
        /// <param name="left"> The instance <see cref="double"/>. </param>
        /// <param name="right"> The <see cref="double"/> to be compared to instance value. </param>
        /// <returns> <see langword="true"/> if the values are equal, <see langword="false"/> otherwise. </returns>
        public static bool Equals4DigitPrecision(this double left, double right)
        {
            return Math.Abs(left - right) < _4;
        }
        /// <summary>
        /// Checks <see cref="double"/> for value equality within 5 significant digits.
        /// </summary>
        /// <param name="left"> The instance <see cref="double"/>. </param>
        /// <param name="right"> The <see cref="double"/> to be compared to instance value. </param>
        /// <returns> <see langword="true"/> if the values are equal, <see langword="false"/> otherwise. </returns>
        public static bool Equals5DigitPrecision(this double left, double right)
        {
            return Math.Abs(left - right) < _5;
        }
        /// <summary>
        /// Checks <see cref="double"/> for value equality within 6 significant digits.
        /// </summary>
        /// <param name="left"> The instance <see cref="double"/>. </param>
        /// <param name="right"> The <see cref="double"/> to be compared to instance value. </param>
        /// <returns> <see langword="true"/> if the values are equal, <see langword="false"/> otherwise. </returns>
        public static bool Equals6DigitPrecision(this double left, double right)
        {
            return Math.Abs(left - right) < _6;
        }
        /// <summary>
        /// Checks <see cref="double"/> for value equality within 7 significant digits.
        /// </summary>
        /// <param name="left"> The instance <see cref="double"/>. </param>
        /// <param name="right"> The <see cref="double"/> to be compared to instance value. </param>
        /// <returns> <see langword="true"/> if the values are equal, <see langword="false"/> otherwise. </returns>
        public static bool Equals7DigitPrecision(this double left, double right)
        {
            return Math.Abs(left - right) < _7;
        }
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
    }
}
