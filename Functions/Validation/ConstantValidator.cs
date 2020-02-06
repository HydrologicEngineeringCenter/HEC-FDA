using System;
using System.Collections.Generic;
using System.Text;
using Functions.Ordinates;
using Utilities;

namespace Functions.Validation
{
    internal class ConstantValidator : IValidator<Ordinates.Constant>
    {
        public bool IsValid(Constant obj, out IEnumerable<IMessage> msgs)
        {
            if (obj.IsNull()) throw new ArgumentNullException($"The {nameof(Constant)} cannot be validated because it is null.");
            else
            {
                msgs = ReportErrors(obj);
                return msgs.Max() < IMessageLevels.Error;
            }
        }
        public IEnumerable<IMessage> ReportErrors(Constant obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (!obj.Value().IsFinite()) msgs.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The specified value: {obj.Value()} is not a finite numerical number. This is likely to cause errors in computation."));
            return msgs;
        }
    }
}
