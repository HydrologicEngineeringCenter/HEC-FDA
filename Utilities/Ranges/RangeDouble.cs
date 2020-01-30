using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Utilities.Ranges
{
    internal class RangeDouble : IRange<double>, IValidate<IRange<double>>
    {
        private readonly bool _FiniteRequirement;

        #region Properties
        public double Min { get; }
        public double Max { get; }
        public bool IsValid { get; }
        public IEnumerable<IMessage> Messages { get; }
        #endregion

        internal RangeDouble(double min, double max, bool inclusiveMin, bool inclusiveMax, bool finiteRequirement)
        {
            _FiniteRequirement = finiteRequirement;
            Min = inclusiveMin ? min : min + double.Epsilon;
            Max = inclusiveMax ? max : max - double.Epsilon;
            IsValid = Validate(new RangeDoubleValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }
        public bool Validate(IValidator<IRange<double>> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }

        public string Print(bool round = false) => round ? $"[{Min.Print()}, {Max.Print()}]" : $"[{Min}, {Max}]";
        public static string Requirements() => $"range: [{double.MinValue.Print()}, {double.MaxValue.Print()}] with range min < range max";
        public bool Equals<T>(IRange<T> range) => range.GetType() == typeof(RangeDouble) && Print() == range.Print();
        public bool IsOnRange(double x)
        {
            if (IsValid) return x.IsOnRange(Min, Max);
            else throw new InvalidOperationException(Utilities.ValidationExtensions.InvalidOperationExceptionMessage("IRange", Messages));
        }
    }
}
