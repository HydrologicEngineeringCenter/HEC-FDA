using Functions;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;


namespace Model.Parameters.Elevations
{
    internal class Elevation : ElevationBase, IParameter, IValidate<ElevationBase>
    {
        //public string Label { get; }
        //public UnitsEnum Units { get; }
        //public IParameterEnum ParameterType { get; }
        public override IRange<double> Range { get; }
        public override bool IsConstant { get; }

        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }

        public Elevation(IFunction fx, IParameterEnum parameterType, string label = "", UnitsEnum units = UnitsEnum.Foot): base(parameterType, label, units)
        {
            Range = fx.Range;
            State = Validate(new Validation.ElevationValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }

        public IMessageLevels Validate(IValidator<ElevationBase> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }
        public string Print(bool round = false, bool abbreviate = false)
        {
            throw new NotImplementedException();
        }

        public string PrintValue(bool round = false, bool abbreviate = false)
        {
            throw new NotImplementedException();
        }
    }
}
