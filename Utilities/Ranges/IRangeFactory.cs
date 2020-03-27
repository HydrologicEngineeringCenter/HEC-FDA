using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// Provides methods for the creation of objects implementing the <see cref="IRange{T}"/> interface.
    /// </summary>
    public static class IRangeFactory
    {
        /// <summary>
        /// A method for the construction of <see cref="double"/> precision <see cref="IRange{double}"/> object instances.
        /// </summary>
        /// <param name="min"> The <see cref="IRange.Min"/>.</param>
        /// <param name="max"> The <see cref="IRange.Max"/>. </param>
        /// <param name="inclusiveMin"> <see langword="true"/> if the <see cref="IRange.Min"/> value on the range (<see langword="true"/> by default). </param>
        /// <param name="inclusiveMax"> <see langword="true"/> if the <see cref="IRange.Max"/> value on the range (<see langword="true"/> by default). </param>
        /// <param name="finiteRequirement"> <see langword="true"/> if ranges containing <see cref="double.NaN"/>, <see cref="double.NegativeInfinity"/>, or <see cref="double.PositiveInfinity"/> values should be marked in an <see cref="IMessageLevels.Error"/> <see cref="IMessagePublisher.State"/>, <see langword="false"/> otherwise. </param>
        /// <param name="notSingleValueRequirement"> <see langword="true"> if ranges with the <see cref="IRange.Min"/> equal to the <see cref="IRange.Max"/> should be marked in an <see cref="IMessageLevels.Error"/> <see cref="IMessagePublisher.State"/>, <see langword="false"/> otherwise. </param>
        /// <returns> An object implementing the <see cref="IRange{T}"/> interface. </returns>
        public static IRange<double> Factory(double min, double max, bool inclusiveMin = true, bool inclusiveMax = true, bool finiteRequirement = true, bool notSingleValueRequirement = true) => new Ranges.RangeDouble(min, max, inclusiveMin, inclusiveMax, finiteRequirement, notSingleValueRequirement);
        public static IRange<int> Factory(int min, int max) => new Ranges.RangeInteger(min, max, true, true);
    }
}
