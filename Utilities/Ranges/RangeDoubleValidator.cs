using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Ranges
{
    internal class RangeDoubleValidator: IValidator<RangeDouble>
    {
        internal RangeDoubleValidator()
        {
        }

        public IMessageLevels IsValid(RangeDouble obj, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max();
        }
        public IEnumerable<IMessage> ReportErrors(RangeDouble obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException(nameof(obj), "The could not be validated because it is null.");
            if (!ValidationExtensions.IsRange(obj.Min, obj.Max, obj._FiniteRequirement, obj._MoreThanSingleValueRequirement)) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The range: {obj.Print(true)} is invalid. It does not represent a logical range."));
            if (!obj.Min.IsFinite() || !obj.Max.IsFinite())
            {
                if (obj._FiniteRequirement) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The range: {obj.Print(true)} is invalid because it is not a finite range."));
                else msgs.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The range: {obj.Print(true)} is not finite. This is allowed but may generate illogical results."));
            }
            if (obj.Min == obj.Max)
            {
                if (obj._MoreThanSingleValueRequirement) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The range: {obj.Print(true)} is invalid. It does not represent a logical range containing more than a single value."));
                else msgs.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The range minimum and maximum values: {obj.Min} are identical. This is allowed but makes results in a range that contains a single point."));
            }
            return msgs;
        }
    }
}
