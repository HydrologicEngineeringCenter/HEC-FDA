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
            if (!ValidationExtensions.IsRange(obj.Min, obj.Max)) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The specified range: {obj.Print()} is invalid because it does not represent a logical range."));
            if (obj.Min == obj.Max) msgs.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The specified minimum and maximum values: {obj.Min} are identical. This is allowed but makes results in a range that contains a single point."));
            return msgs;
        }
    }
}
