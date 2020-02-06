using System;
using System.Collections.Generic;
using System.Text;
using Functions.Coordinates;
using Functions.Ordinates;
using Utilities;

namespace Functions.Validation
{
    internal class CoordinateConstanstsValidator : IValidator<Coordinates.CoordinateConstants>
    {
        public bool IsValid(CoordinateConstants obj, out IEnumerable<IMessage> msgs)
        {
            if (obj.IsNull()) throw new ArgumentNullException($"The {nameof(CoordinateConstants)} could not be validated because it is null.");
            else
            {
                msgs = ReportErrors(obj);
                return msgs.Max() < IMessageLevels.Error;
            }
        }

        public IEnumerable<IMessage> ReportErrors(CoordinateConstants obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.X.Value().IsFinite()) msgs.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The provided x {nameof(IOrdinate)} value: {obj.X.Print(true)} is not a finite numerical value. This is likely to cause errors during computation."));
            if (obj.Y.Value().IsFinite()) msgs.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The provided x {nameof(IOrdinate)} value: {obj.Y.Print(true)} is not a finite numerical value. This is likely to cause errors during computation."));
            return msgs;
        }
        public static bool IsConstructable(Constant x, Constant y, out string msg)
        {
            msg = "";
            if (x.IsNull()) msg += $"The {nameof(ICoordinate)} cannot be constructed because the specified x {nameof(IOrdinate)} in null.";
            if (y.IsNull()) msg += $"The {nameof(ICoordinate)} cannot be constructed because the specified y {nameof(IOrdinate)} is null.";
            return msg.Length == 0;
        }
    }
}
