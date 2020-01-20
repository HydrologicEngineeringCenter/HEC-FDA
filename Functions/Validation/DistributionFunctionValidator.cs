using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Functions.Validation
{
    internal class DistributionFunctionValidator : Utilities.IValidator<DistributionFunction>
    {
        public bool IsValid(DistributionFunction entity, out IEnumerable<IMessage> errors)
        {
            errors = ReportErrors(entity);
            return !errors.Any();
        }

        public IEnumerable<IMessage> ReportErrors(DistributionFunction entity)
        {
            List<IMessage> msg = new List<IMessage>();
            //if (entity.Is)
            return msg;
        }
        internal static bool IsConstructable(IDistributedValue distribution, out string msg)
        {
            msg = ReportFatalErrors(distribution);
            return !(msg.Length == 0);
        }
        internal static string ReportFatalErrors(IDistributedValue distribution)
        {
            string msg = "";
            if (distribution.IsNull()) msg = "The function cannot be constructed because the specified distribution is null.";
            return msg;
        }
    }
}
