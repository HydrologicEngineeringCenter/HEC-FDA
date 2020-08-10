//using System;
//using System.Collections.Generic;
//using System.Text;
//using Functions;
//using Utilities;

//namespace Model.Parameters.Series
//{
//    internal sealed class FrequencySeries : ParameterSeriesBase, IValidate<FrequencySeries>
//    {
//        public override UnitsEnum Units => UnitsEnum.Probability;
//        public override IParameterEnum ParameterType { get; }
//        public override string Label { get; }

//        public override IMessageLevels State { get; }
//        public override IEnumerable<IMessage> Messages { get; }

//        internal FrequencySeries(IFdaFunction fx, bool xOrdinate, IParameterEnum parameterType, string label =""): base(fx, xOrdinate)
//        {
//            ParameterType = parameterType;
//            Label = label == "" ? parameterType.ToString() : label;
//            State = Validate(new Validation.FrequencySeriesValidator(), out IEnumerable<IMessage> msgs);
//            Messages = msgs;
//        }

//        public IMessageLevels Validate(IValidator<FrequencySeries> validator, out IEnumerable<IMessage> msgs)
//        {
//            return validator.IsValid(this, out msgs);
//        }
//    }
//}
