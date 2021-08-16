using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace ImpactArea.Validation
{
    internal class OrdinateValidator: IValidator<IOrdinate>
    {
        public bool IsValid(IOrdinate obj, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max() < IMessageLevels.Error;
        }
        public IEnumerable<IMessage> ReportErrors(IOrdinate obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            return msgs;
        }

        IMessageLevels IValidator<IOrdinate>.IsValid(IOrdinate entity, out IEnumerable<IMessage> errors)
        {
            throw new NotImplementedException();
        }
    }
}
