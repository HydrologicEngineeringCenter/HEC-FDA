using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model.Parameters.Damages
{
    internal sealed class Damage : ParameterRangeBase<Damage>, IValidate<Damage>
    {
        public override IMessageLevels State { get; }
        public override IEnumerable<IMessage> Messages { get; }

        internal Damage(IRange<double> range, bool isConstant, IParameterEnum type, UnitsEnum units = UnitsEnum.NotSet, string label = "", bool abbreviatedLabel = true): base(range, isConstant, type, units, label, abbreviatedLabel)
        {
            // No need for _RangeDefaultUnits (like for Flow) because only Dollars is supported.
            State = Validate(new Validation.DamageValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;        
        }
        public override IMessageLevels Validate(IValidator<Damage> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }
    }
}
