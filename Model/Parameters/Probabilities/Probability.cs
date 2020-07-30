using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model.Parameters.Probabilities
{
    internal class Probability : IParameter, IValidate<Probability>
    {
        public UnitsEnum Units { get; }
        public IParameterEnum ParameterType { get; }
        public string Label { get; }
        public IRange<double> Range { get; }
        public bool IsConstant { get; }

        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }

        internal Probability(IRange<double> range, bool isConstant, IParameterEnum type, UnitsEnum units = UnitsEnum.NotSet, string label = "")
        {
            if (range.IsNull()) throw new ArgumentNullException(nameof(range));
            else
            {
                Range = range;
                IsConstant = isConstant;
                ParameterType = type;
                Units = units == UnitsEnum.NotSet ? type.DefaultUnits(): units;
                Label = label == "" ? type.Print() : label;
                State = Validate(new Validation.ProbabilityValidator(), out IEnumerable<IMessage> msgs);
                Messages = msgs;
            }
        }

        public IMessageLevels Validate(IValidator<Probability> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }

        public string Print(bool round = false, bool abbreviate = false)
        {
            return $"{ParameterType.Print(abbreviate)}";
        }
        public string PrintValue(bool round = false, bool abbreviate = false)
        {
            return $"{ParameterType.Print(abbreviate)} on the range: {Range.Print(round)}.";
        }
    }
}
