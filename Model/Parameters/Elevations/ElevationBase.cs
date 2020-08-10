using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model.Parameters.Elevations
{
    internal abstract class ElevationBase : ParameterRangeBase<ElevationBase>
    {
        public override IMessageLevels State { get; }
        public override IEnumerable<IMessage> Messages { get; }
        internal IRange<double> _RangeDefaultUnits { get; }

        internal ElevationBase(IRange<double> range, bool isConstant, IParameterEnum parameterType, UnitsEnum units = UnitsEnum.Foot, string label = "", bool abbreviatedLabel = true): base(range, isConstant, parameterType, units, label, abbreviatedLabel)
        {
            _RangeDefaultUnits = range.ConvertLenghts(Units, ParameterType.DefaultUnits());
            State = Validate(new Validation.ElevationValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }
        public override IMessageLevels Validate(IValidator<ElevationBase> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }
    }
}
