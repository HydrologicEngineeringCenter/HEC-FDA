using Functions.Ordinates;
using Statistics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Utilities.Serialization;
using Utilities;

namespace Functions.Coordinates
{
    internal class CoordinateVariableY: ICoordinate, IValidate<CoordinateVariableY>
    {
        #region Properties
        public IOrdinate X { get; }
        public IOrdinate Y { get; }
        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }
        #endregion

        #region Constructor
        public CoordinateVariableY(Constant x, Distribution y)
        {
            if (!Validation.CoordinateVariableYValidator.IsConstructable(x, y, out string msg)) throw new InvalidConstructorArgumentsException(msg);
            else
            {
                X = x;
                Y = y;
                State = Validate(new Validation.CoordinateVariableYValidator(), out IEnumerable<IMessage> msgs);
                Messages = msgs;
            }
        }
        #endregion

        #region Functions
        public IMessageLevels Validate(IValidator<CoordinateVariableY> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }
        public string Print(bool round = false) => $"({X.Print(round)}, {Y.Print(round)})";
        public bool Equals(ICoordinate obj)
        {
            return obj is CoordinateVariableY coord &&
                   X.Equals(coord.X) &&
                   Y.Equals(coord.Y);                  
                   //EqualityComparer<Distribution>.Default.Equals((Distribution)Y, (Distribution)coord.Y);
        }
        public XElement WriteToXML()
        {
            XElement xVal = X.WriteToXML();
            XElement yVal = Y.WriteToXML();
            XElement coordElem = new XElement(SerializationConstants.COORDINATE);
            coordElem.Add(xVal);
            coordElem.Add(yVal);
            return coordElem;
        }
        //public override int GetHashCode()
        //{
        //    var hashCode = 1861411795;
        //    hashCode = hashCode * -1521134295 + X.GetHashCode();
        //    hashCode = hashCode * -1521134295 + EqualityComparer<Distribution>.Default.GetHashCode((Distribution)Y);
        //    return hashCode;
        //}
        #endregion
    }
}
