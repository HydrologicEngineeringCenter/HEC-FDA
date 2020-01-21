using System;
using System.Collections.Generic;
using Utilities;

namespace ImpactArea
{
    public class Elevation: IElevation, IValidate<Elevation>
    {
        public IOrdinate Height { get; }
        public ElevationEnum Type { get; }
        public bool IsValid { get; }
        public IEnumerable<IMessage> Messages { get; }

        internal Elevation(IOrdinate height, ElevationEnum type)
        {
            if (!Validation.ElevationValidator.IsConstructable(height, out string msg)) throw new Utilities.InvalidConstructorArgumentsException(msg);
            else
            {
                Type = type;
                Height = height;
                IsValid = Validate(new Validation.ElevationValidator(), out IEnumerable<IMessage> msgs);
                Messages = msgs;
            }
        }

        public bool Validate(IValidator<Elevation> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }
    }
}
