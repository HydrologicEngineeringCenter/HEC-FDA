using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Utilities.Ranges
{
    internal class RangeDouble : IRange<double>, IValidate<RangeDouble>
    {
        private readonly double _MinToPrint;
        private readonly double _MaxToPrint;
        private readonly bool _InclusiveMin;
        private readonly bool _InclusiveMax;
        internal readonly bool _FiniteRequirement;
        internal readonly bool _MoreThanSingleValueRequirement;

        #region Properties
        public double Min { get; }
        public double Max { get; }
        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }
        #endregion

        /// <summary>
        /// Constructs a new <see cref="IRange{T}"/> object.
        /// </summary>
        /// <param name="min"> The intended <see cref="IRange{T}.Min"/>. </param>
        /// <param name="max"></param>
        /// <param name="inclusiveMin"> <see langword="true"/> by default. 
        /// <see langword="true"/> if the <paramref name="min"/> is meant to be included in the range. 
        /// <see langword="false"/> if the first value on the range is intended to be greater than this value, in which case the first value on the range is set to <see cref="IRange{T}.Min"/> + <see cref="double.Epsilon"/>. </param>
        /// <param name="inclusiveMax"> <see langword="true"/> by default. 
        /// <see langword="true"/> if the <paramref name="max"/> is meant to be included in the range. 
        /// <see langword="false"/> if the last value on the range is intended to be less than this value, in which case the last value on the range is set to <see cref="IRange{T}.Max"/> - <see cref="double.Epsilon"/>. </param>
        /// <param name="finiteRequirement"> <see langword="true"/> by default.
        /// <see langword="true"/> if the bounds of the range must be represented by finite numerical values, <see langword="false"/> otherwise. </param>
        /// <param name="maxNotEqualToMinRequirement"> <see langword="true"/> by default.
        /// <see langword="true"/> if the bounds of the range must span more than a single finite numerical value, <see langword="false"/> otherwise.</param>
        internal RangeDouble(double min, double max, bool inclusiveMin, bool inclusiveMax, bool finiteRequirement, bool maxNotEqualToMinRequirement)
        {
            _MinToPrint = min;
            _MaxToPrint = max;
            _InclusiveMin = inclusiveMin;
            _InclusiveMax = inclusiveMax;
            _FiniteRequirement = finiteRequirement;
            _MoreThanSingleValueRequirement = maxNotEqualToMinRequirement;
            Min = inclusiveMin ? min : min + double.Epsilon;
            Max = inclusiveMax ? max : max - double.Epsilon;
            State = Validate(new RangeDoubleValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }
        public IMessageLevels Validate(IValidator<RangeDouble> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }

        public string Print(bool round = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_InclusiveMin ? "[" : "(");
            sb.Append(round ? $"{_MinToPrint.Print()} , {_MaxToPrint.Print()}" : $"{_MinToPrint} , {_MaxToPrint}");
            sb.Append(_InclusiveMax ? "]" : ")");
            return sb.ToString();
        }
        public static string Requirements() => $"range: [{double.MinValue.Print()}, {double.MaxValue.Print()}] with range min less than or equal to range max";
        public bool Equals<T>(IRange<T> range) => range.GetType() == typeof(RangeDouble) && String.Compare(Print(), range.Print()) == 0 ? true : false;
        public bool IsOnRange(double x)
        {
            if (State < IMessageLevels.Error) return x.IsOnRange(Min, Max);
            else throw new InvalidOperationException(Utilities.ValidationExtensions.InvalidOperationExceptionMessage("IRange", Messages));
        }

        internal double ConvertMinToDouble() => Min;
        internal double ConvertMaxToDouble() => Max;
    }
}
