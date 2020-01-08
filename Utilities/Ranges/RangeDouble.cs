using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Ranges
{
    internal class RangeDouble : IRange<double>
    {
        #region Properties
        public double Min { get; }
        public double Max { get; }
        #endregion

        public RangeDouble(double min, double max, bool inclusiveMin, bool inclusiveMax)
        {
            Min = inclusiveMin ? min : min + double.Epsilon;
            Max = inclusiveMax ? max : max - double.Epsilon;
            if (!Utilities.Validate.IsRange(Min, Max)) throw new Utilities.InvalidConstructorArgumentsException($"The provided min and max values: {min}, {max} created an invalid range of: [{Min}, {Max}].");
        }
        public string Print() => $"[{Min}, {Max}]";
        public bool Equals<T>(IRange<T> range) => range.GetType() == typeof(RangeDouble) && Print() == range.Print();
    }
}
