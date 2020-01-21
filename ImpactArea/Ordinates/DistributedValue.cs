using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace ImpactArea
{
    internal class DistributedValue: IOrdinate
    {
        private readonly Statistics.IDistribution _Distribution;

        public bool IsValid { get; }
        public IEnumerable<IMessage> Messages { get; }

        internal DistributedValue(Statistics.IDistribution distribution)
        {
            if (!IsConstructable(distribution, out string msg)) throw new InvalidConstructorArgumentsException(msg);
            else
            {
                _Distribution = distribution;
                IsValid = Validate(new Validation.OrdinateValidator(), out IEnumerable<IMessage> msgs);
                Messages = msgs;
            }           
        }

        public bool IsConstructable(Statistics.IDistribution obj, out string msg)
        {
            if (obj.IsNull())
            {
                msg = "The ordinate cannot be constructed because it is null.";
                return false;
            }
            else
            {
                msg = "";
                return true;
            }
        }
        public bool Validate(IValidator<IOrdinate> validator, out IEnumerable<IMessage> msgs)
        {
            msgs = _Distribution.Messages;
            return msgs.IsNullOrEmpty() ? true : false;
        }

        public double Value(double p = 0.50) => _Distribution.InverseCDF(p);
    }
}
