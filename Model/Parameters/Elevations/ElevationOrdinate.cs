using System;
using System.Collections.Generic;
using System.Text;
using Functions;
using Utilities;

namespace Model.Parameters.Elevations
{
    internal class ElevationOrdinate : ElevationBase, IElevation, IValidate<ElevationBase>
    {
        public IOrdinate Parameter { get; }
        public override IRange<double> Range => Parameter.Range;
        public override bool IsConstant { get; }
        //public UnitsEnum Units { get; }
        //public IParameterEnum ParameterType { get; }
        //public string Label { get; }       
        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }

        public ElevationOrdinate(IOrdinate value, IParameterEnum type, UnitsEnum units = UnitsEnum.Foot, string label = ""): base(type, label, units)
        {
            if (!Validation.ElevationValidator.IsConstructable(type, value, units, out string msg)) throw new InvalidConstructorArgumentsException(msg);
            else
            {
                //Units = units;
                //ParameterType = type;
                //Label = label == "" ? $"{type.ToString()} ({units.Print()})": label;
                Parameter = value;
                IsConstant = Parameter.Type == IOrdinateEnum.Constant ? true : false;   
                State = Validate(new Validation.ElevationValidator(), out IEnumerable<IMessage> msgs);
                Messages = msgs;
            }
        }

        public IMessageLevels Validate(IValidator<ElevationBase> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }

        public string Print(bool round = false, bool abbreviate = false) => ParameterType.ToString() + ": " + PrintValue(round, abbreviate);
        public string PrintValue(bool round = false, bool abbreviate = false) => UnitsUtilities.Print(this, round, abbreviate);
    }
}
