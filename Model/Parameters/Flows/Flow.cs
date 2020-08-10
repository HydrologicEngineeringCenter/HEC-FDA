using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model.Parameters.Flows
{
    internal sealed class Flow : ParameterRangeBase<Flow>, IValidate<Flow>
    {
        public override IMessageLevels State { get; }
        public override IEnumerable<IMessage> Messages { get; }
        internal IRange<double> _RangeDefaultUnits { get; }

        internal Flow(IRange<double> range, bool isConstant, IParameterEnum type, UnitsEnum units = UnitsEnum.NotSet, string label = "", bool abbreviatedLabel = false) : base(range, isConstant, type, units, label, abbreviatedLabel)
        {
            _RangeDefaultUnits = Range.ConvertFlows(Units, ParameterType.DefaultUnits());
            State = Validate(new Validation.Parameters.FlowValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }

        public override IMessageLevels Validate(IValidator<Flow> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }
    }
}

