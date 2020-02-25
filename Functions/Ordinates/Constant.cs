using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Utilities;
using Utilities.Serialization;

namespace Functions.Ordinates
{
    public class Constant : IOrdinate, IValidate<Constant>
    {
        #region Fields and Properties
        private readonly double _ConstantValue;

        public IRange<double> Range { get; }
        public IOrdinateEnum Type => IOrdinateEnum.Constant;
        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }
        #endregion

        #region Constructor
        public Constant(double value)
        {
            _ConstantValue = value;
            Range = IRangeFactory.Factory(_ConstantValue, _ConstantValue);
            State = Validate(new Validation.ConstantValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }
        #endregion

        #region Functions
        public IMessageLevels Validate(IValidator<Constant> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }

        public double Value(double p = 0.5)
        {
            return _ConstantValue;
        }
        public bool Equals(IOrdinate constant)
        {
            return constant.Print().Equals(Print());
        }
        public string Print(bool round = false)
        {
            if (round) return $"Double(value: {_ConstantValue.Print()})";
            else return $"Double(value: {_ConstantValue}";
        }

        public XElement WriteToXML()
        {
            XElement ordinateElem = new XElement(SerializationConstants.ORDINATE);
            ordinateElem.SetAttributeValue(SerializationConstants.TYPE, SerializationConstants.CONSTANT);

            XElement constantElem = new XElement(SerializationConstants.CONSTANT);
            constantElem.SetAttributeValue(SerializationConstants.VALUE, _ConstantValue);

            ordinateElem.Add(constantElem);
            return ordinateElem;
        }
#endregion
    }
}
