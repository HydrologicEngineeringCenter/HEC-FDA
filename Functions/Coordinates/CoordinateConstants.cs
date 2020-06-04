using Functions.Ordinates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Utilities;

namespace Functions.Coordinates
{
    internal class CoordinateConstants: ICoordinate, IValidate<CoordinateConstants>
    {
        #region Properties
        public IOrdinate X { get; }
        public IOrdinate Y { get; }
        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }
        #endregion

        #region Constructor
        public CoordinateConstants(Constant x, Constant y)
        {
            if (!Validation.CoordinateConstanstsValidator.IsConstructable(x, y, out string msg)) throw new Utilities.InvalidConstructorArgumentsException(msg);
            else
            {
                X = x;
                Y = y;
                State = Validate(new Validation.CoordinateConstanstsValidator(), out IEnumerable<IMessage> msgs);
                Messages = msgs;
            }
        }
        #endregion

        #region Functions
        public IMessageLevels Validate(IValidator<CoordinateConstants> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }

        public string Print(bool round = false) => $"({X.Print(round)}, {Y.Print(round)})";
        public bool Equals(ICoordinate obj)
        {
            return obj is CoordinateConstants constants &&
                   X.Equals(constants.X) &&
                   Y.Equals(constants.Y);
        }
        public XElement WriteToXML()
        {
            XElement xVal = X.WriteToXML();
            XElement yVal = Y.WriteToXML();
            XElement coordElem = new XElement("Coordinate");
            coordElem.Add(xVal);
            coordElem.Add(yVal);
            return coordElem;
        }
        //public override int GetHashCode()
        //{
        //    var hashCode = 1861411795;
        //    hashCode = hashCode * -1521134295 + X.GetHashCode();
        //    hashCode = hashCode * -1521134295 + Y.GetHashCode();
        //    return hashCode;
        //}
        #endregion
    }
}
