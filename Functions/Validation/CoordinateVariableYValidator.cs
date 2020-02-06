using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Functions.Coordinates;
using Functions.Ordinates;
using Utilities;

namespace Functions.Validation
{
    internal class CoordinateVariableYValidator : IValidator<Coordinates.CoordinateVariableY>
    {
        public bool IsValid(CoordinateVariableY obj, out IEnumerable<IMessage> msgs)
        {
            if (obj.IsNull()) throw new ArgumentNullException($"The {nameof(CoordinateVariableY)} cannot be validated because it is null.");
            else
            {
                msgs = ReportErrors(obj);
                return msgs.Max() < IMessageLevels.Error;
            }
        }

        public IEnumerable<IMessage> ReportErrors(CoordinateVariableY obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.X.Messages.Any()) msgs.Add(IMessageFactory.Factory(obj.X.Messages.Max(), $"The X {nameof(IOrdinate)} contains the following messages: \r\n {obj.X.Messages.PrintTabbedListOfMessages()}"));
            if (obj.Y.Messages.Any()) msgs.Add(IMessageFactory.Factory(obj.Y.Messages.Max(), $"The Y {nameof(IOrdinate)} contains the following messages: \r\n {obj.Y.Messages.PrintTabbedListOfMessages()}"));
            return msgs;
        }
        public static bool IsConstructable(Constant x, Distribution y, out string msg)
        {
            msg = "";
            if (x.IsNull()) msg += $"The {nameof(ICoordinate)} cannot be constructed because the specified x {nameof(IOrdinate)} in null.";
            if (y.IsNull()) msg += $"The {nameof(ICoordinate)} cannot be constructed because the specified y {nameof(IOrdinate)} is null.";
            return msg.Length == 0;
        }
    }
}
