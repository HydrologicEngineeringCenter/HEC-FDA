using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Functions;
using Model.Validation;

namespace Model.Parameters.Series
{
    internal class ElevationSeries : ParameterSeriesBase, IElevation<IEnumerable<IOrdinate>>,  IValidate<IElevation<IEnumerable<IOrdinate>>>
    {
        public override UnitsEnum Units { get; }
        public override IParameterEnum ParameterType { get; }
        public override string Label { get;  }
        
        public override IMessageLevels State { get; }
        public override IEnumerable<IMessage> Messages { get; }

        internal ElevationSeries(IFdaFunction fx, bool xOrdinate, IParameterEnum parameterType, UnitsEnum units = UnitsEnum.Foot, string label = ""): base(fx, xOrdinate)
        {
            Units = units;
            ParameterType = parameterType;
            Label = label == "" ? parameterType.ToString() : label;
            State = Validate(new Validation.ElevationValidator<IEnumerable<IOrdinate>>(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }
        public IMessageLevels Validate(IValidator<IElevation<IEnumerable<IOrdinate>>> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }
    }
}
