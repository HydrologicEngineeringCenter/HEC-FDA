using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model.Parameters.Probabilities
{
    internal sealed class Probability : ParameterRangeBase<Probability>, IValidate<Probability>
    {        
        public override IMessageLevels State { get; }
        public override IEnumerable<IMessage> Messages { get; }

        internal Probability(IRange<double> range, bool isConstant, IParameterEnum type, UnitsEnum units = UnitsEnum.NotSet, string label = "", bool abbreviate = false): base(range, isConstant, type, units, label, abbreviate)
        {
            State = Validate(new Validation.ProbabilityValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }

        public override IMessageLevels Validate(IValidator<Probability> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }

        public override string Print(bool round = false, bool abbreviate = false)
        {
            return $"{ParameterType.Print(abbreviate)}(range: {Range.Print(round)})";
        }
    }
}
