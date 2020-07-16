using System;
using System.Collections.Generic;
using System.Text;
using Functions;
using Utilities;

namespace Model.Elevations
{
    internal class Elevation : IElevation, IValidate<IElevation>
    {
        public bool IsConstant { get; }
        public UnitsEnum Units { get; }
        public IParameterEnum ParameterType { get; }
        public string Label { get; }
        public IOrdinate Parameter { get; }
        public IRange<double> Range => Parameter.Range;
        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }

        public Elevation(IOrdinate value, UnitsEnum units, IParameterEnum type, string label)
        {
            if (!Validation.ElevationValidator.IsConstructable(type, value, units, out string msg)) throw new InvalidConstructorArgumentsException(msg);
            else
            {
                Units = units;
                Parameter = value;
                IsConstant = Parameter.Type == IOrdinateEnum.Constant ? true : false;
                ParameterType = type;
                Label = label == "" ? type.ToString() : label;
                State = Validate(new Validation.ElevationValidator(), out IEnumerable<IMessage> msgs);
                Messages = msgs;
            }
        }

        public IMessageLevels Validate(IValidator<IElevation> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }

        public string Print(bool round = false, bool abbreviate = false) => ParameterType.ToString() + ": " + PrintValue(round, abbreviate);
        public string PrintValue(bool round = false, bool abbreviate = false) => UnitsUtilities.Print(this, round, abbreviate);
    }
}
