using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Ranges
{
    internal class RangeIntegerValidator: IValidator<RangeInteger>
    {
        internal RangeIntegerValidator()
        {
        }

        public bool IsValid(RangeInteger obj, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max() < IMessageLevels.Error;
        }
        public IEnumerable<IMessage> ReportErrors(RangeInteger obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (!Validate.IsRange(obj.Min, obj.Max)) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The specified range: {obj.Print()} is invalid because it does not represent a logical range."));
            return msgs;
        }
    }
}
