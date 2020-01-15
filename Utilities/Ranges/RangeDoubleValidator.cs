using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Ranges
{
    internal class RangeDoubleValidator: IValidator<IRange<double>>
    {
        internal RangeDoubleValidator()
        {
        }

        public bool IsValid(IRange<double> obj, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max() < IMessageLevels.Error;
        }
        public IEnumerable<IMessage> ReportErrors(IRange<double> obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException(nameof(obj), "The could not be validated because it is null.");
            if (!Validate.IsRange(obj.Min, obj.Max))
            {
                if (!obj.Min.IsFinite() || !obj.Max.IsFinite()) 
                    msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The specified range: {obj.Print()} is invalid because it does not represent a finite logical range."));
                else 
                    msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The specified range: {obj.Print()} is invalid because it does not represent a logical range."));
            }
            return msgs;
        }
    }
}
