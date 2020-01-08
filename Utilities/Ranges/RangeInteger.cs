using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Ranges
{
    internal class RangeInteger: IRange<int>
    {
        public int Min { get; }
        public int Max { get; }

        internal RangeInteger(int min, int max, bool isMinInclusive, bool isMaxInclusive)
        {
            Min = isMaxInclusive ? min : min + 1;
            Max = isMaxInclusive ? max : max - 1;
            if (!Utilities.Validate.IsRange(Min, Max)) throw new Utilities.InvalidConstructorArgumentsException($"The provided min and max values: {min}, {max} created an invalid range of: [{Min}, {Max}].");
        }

        public string Print() => $"[{Min}, {Max}]";
        public bool Equals<T>(IRange<T> range) => range.GetType() == typeof(RangeInteger) && Print() == range.Print(); 
    }
}
