using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Ranges
{
    internal class RangeInteger: IRange<int>, IValidate<RangeInteger>
    {
        public int Min { get; }
        public int Max { get; }
        public bool IsValid { get; }
        public IEnumerable<IMessage> Messages { get; }

        internal RangeInteger(int min, int max, bool isMinInclusive, bool isMaxInclusive)
        {
            Min = isMaxInclusive ? min : min + 1;
            Max = isMaxInclusive ? max : max - 1;
            IsValid = Validate(new RangeIntegerValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }
        public bool Validate(IValidator<RangeInteger> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }
        public string Print(bool round = false) => round ? $"[{Min.Print()}, {Max.Print()}]" : $"[{Min}, {Max}]";
        public static string Requirements() => $"range: [{int.MinValue.Print()}, {int.MaxValue.Print()}] with range min < range max";
        public bool Equals<T>(IRange<T> range) => range.GetType() == typeof(RangeInteger) && Print() == range.Print(); 
        public bool IsOnRange(int x)
        {
            if (IsValid) return x.IsOnRange(Min, Max);
            else throw new InvalidOperationException(Utilities.ValidationExtensions.InvalidOperationExceptionMessage("IRange", Messages));
        }
    }
}
