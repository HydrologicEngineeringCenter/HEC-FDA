using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace ImpactArea
{
    internal class Constant: IOrdinate
    {
        private readonly double _Constant;
        
        public bool IsValid { get; }
        public IEnumerable<IMessage> Messages { get; }

        public IMessageLevels State => throw new NotImplementedException();

        internal Constant(double x, bool mustBeFinite = true)
        {
            if (mustBeFinite && !IsConstructable(x, out string msg)) throw new InvalidConstructorArgumentsException(msg);
            else
            {
                _Constant = x;
                IsValid = Validate(new Validation.OrdinateValidator(), out IEnumerable<IMessage> msgs);
                Messages = msgs;
            }          
        }
        
        public bool Validate(IValidator<IOrdinate> validator, out IEnumerable<IMessage> msgs)
        {
            msgs = new List<IMessage>();
            return true;
        }
        private static bool IsConstructable(double x, out string msg)
        {
            if (x.IsFinite())
            {
                msg = $"The value: {x} because it is not a finite numerical value.";
                return false;
            }
            else
            {
                msg = "";
                return true;
            }
        } 
        
        public double Value(double p = 0.50) => _Constant;

        IMessageLevels IValidate<IOrdinate>.Validate(IValidator<IOrdinate> validator, out IEnumerable<IMessage> errors)
        {
            throw new NotImplementedException();
        }
    }
}
