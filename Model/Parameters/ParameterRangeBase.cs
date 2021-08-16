using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model.Parameters
{
    internal abstract class ParameterRangeBase<T>: IParameterRange, IValidate<T>
    {        
        public string Label { get; }
        public UnitsEnum Units { get; }
        public bool IsConstant { get; }
        public IRange<double> Range { get; }
        public IParameterEnum ParameterType { get; }

        public abstract IMessageLevels State { get; }
        public abstract IEnumerable<IMessage> Messages { get; }

        internal ParameterRangeBase(IRange<double> range, bool isConstant, IParameterEnum type, UnitsEnum units = UnitsEnum.NotSet, string label = "", bool abbreviatedLabel = true)
        {
            if (range.IsNull()) throw new ArgumentException(nameof(range));
            else
            {
                Range = range;
                ParameterType = type;
                IsConstant = isConstant;
                Units = units == UnitsEnum.NotSet ? type.UnitsDefault() : units;
                Label = label == "" ? $"{PrintLabel(abbreviatedLabel)}" : label;
            }
        }

        public abstract IMessageLevels Validate(IValidator<T> validator, out IEnumerable<IMessage> msgs);

        private string PrintLabel(bool abbreviate = false)
        {
            return $"{ParameterType.Print(abbreviate)} ({Units.Print(abbreviate)})";
        }
        public virtual string Print(bool round = false, bool abbreviate = false)
        {
            return $"{ParameterType.Print(abbreviate)}(range: {Range.Print(round)} {Units.Print(abbreviate)})";
        }

    }
}
