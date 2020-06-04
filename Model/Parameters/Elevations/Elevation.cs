using System;
using System.Collections.Generic;
using System.Text;
using Functions;
using Utilities;

namespace Model.Elevations
{
    internal class Elevation : IElevation<IOrdinate>, IValidate<IElevation<IOrdinate>>
    {
        public UnitsEnum Units { get; }
        public IParameterEnum ParameterType { get; }
        public string Label { get; }
        public IOrdinate Parameter { get; }
        public IRange<double> Range => Parameter.Range;
        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }

        public Elevation(IOrdinate value, UnitsEnum units, IParameterEnum type, string label)
        {
            if (!Validation.ElevationValidator<IOrdinate>.IsConstructable(type, value, units, out string msg)) throw new InvalidConstructorArgumentsException(msg);
            else
            {
                Units = units;
                Parameter = value;
                ParameterType = type;
                Label = label == "" ? type.ToString() : label;
                State = Validate(new Validation.ElevationValidator<IOrdinate>(), out IEnumerable<IMessage> msgs);
                Messages = msgs;
            }
        }

        public IMessageLevels Validate(IValidator<IElevation<IOrdinate>> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }

        public string Print(bool round = false, bool abbreviate = false) => ParameterType.ToString() + ": " + PrintValue(round, abbreviate);
        public string PrintValue(bool round = false, bool abbreviate = false) => UnitsUtilities.Print(this, round, abbreviate);
    }
}
