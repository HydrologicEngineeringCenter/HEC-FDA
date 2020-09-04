using Model.Parameters.Years;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model.Validation.Parameters
{
    internal class YearValidator : IValidator<Year>
    {
        public IMessageLevels IsValid(Year obj, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max();
        }

        public IEnumerable<IMessage> ReportErrors(Year obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException(nameof(obj));
            else
            {
                if (obj._Range.Min < 0)
                    msgs.Add(IMessageFactory.Factory(IMessageLevels.Error,
                        $"The {obj.ParameterType.Print()} parameter contains year values on the range: {obj.Range.Print(true)} {obj.Units.Print(true)}, " +
                        $"this is outside the allowable range of: [0, {int.MaxValue.Print()}] {obj.Units.Print()}."));
            }
            return msgs;
        }
    }
}
