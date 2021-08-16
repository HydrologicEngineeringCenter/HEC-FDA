using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model.Parameters.Years
{
    internal sealed class Year : ParameterRangeBase<Year>, IValidate<Year>
    {
        public override IMessageLevels State { get; }
        public override IEnumerable<IMessage> Messages { get; }
        internal IRange<int> _Range { get; }

        internal Year(IRange<int> range, bool isConstant, IParameterEnum type, string label = "", bool abbreviatedLabel = false) : base(IRangeFactory.Factory(range.Min, range.Max, inclusiveMin: true, inclusiveMax: true, finiteRequirement: true, notSingleValueRequirement: false), isConstant, type, units: UnitsEnum.Year, label, abbreviatedLabel)
        {
            _Range = range;
            State = Validate(new Validation.Parameters.YearValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }

        public override IMessageLevels Validate(IValidator<Year> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }
    }
}
