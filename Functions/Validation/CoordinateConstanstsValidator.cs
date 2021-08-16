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
        public IMessageLevels IsValid(CoordinateConstants obj, out IEnumerable<IMessage> msgs)
        {
            if (obj.IsNull()) throw new ArgumentNullException($"The {nameof(CoordinateConstants)} could not be validated because it is null.");
            else
            {
                msgs = ReportErrors(obj);
                return msgs.Max();
            }
        }

        public IEnumerable<IMessage> ReportErrors(CoordinateConstants obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (!obj.X.Value().IsFinite() || !obj.Y.Value().IsFinite()) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, 
                $"The {obj.Print()} contains non-finite or non-numerical values. This is likely to cause errors during computation."));
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
