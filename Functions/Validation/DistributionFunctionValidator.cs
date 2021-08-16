using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Functions.Validation
{
    internal class DistributionFunctionValidator : Utilities.IValidator<ICoordinatesFunction>
    {
        public IMessageLevels IsValid(ICoordinatesFunction obj, out IEnumerable<IMessage> msgs)
        {
            if (obj.GetType() != typeof(DistributionFunction)) throw new ArgumentException($"The {this.GetType()} can only validated {typeof(DistributionFunction)} objects. It was called by a {obj.GetType()} object.");
            else
            {
                msgs = ReportErrors(obj);
                return msgs.Max();
            }  
        }
        public IEnumerable<IMessage> ReportErrors(ICoordinatesFunction obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.State > IMessageLevels.Message) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The provided distribution is invalid. Check the details for more information.", $"Distribution error messages:{obj.Messages.PrintTabbedListOfMessages()}"));
            return msgs;
        }
        internal static bool IsConstructable(IDistributedOrdinate distribution, out string msg)
        {
            msg = ReportFatalErrors(distribution);
            return !(msg.Length == 0);
        }
        internal static string ReportFatalErrors(IDistributedOrdinate distribution)
        {
            string msg = "";
            if (distribution.IsNull()) msg = "The function cannot be constructed because the specified distribution is null.";
            return msg;
        }
    }
}
