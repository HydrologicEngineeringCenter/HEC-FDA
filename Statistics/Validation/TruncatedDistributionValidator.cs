using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Statistics.Distributions;
using System.Linq;

namespace Statistics.Validation
{
    internal class TruncatedDistributionValidator: IValidator<TruncatedDistribution>
    {
        internal TruncatedDistributionValidator()
        {
        }
        public IMessageLevels IsValid(TruncatedDistribution obj, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max();
        }
        public IEnumerable<IMessage> ReportErrors(TruncatedDistribution obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.Range.Messages.Any()) msgs.AddRange(obj.Range.Messages);
            return msgs;
        }
    }
}
