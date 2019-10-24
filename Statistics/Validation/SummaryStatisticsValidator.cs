using System;
using System.Collections.Generic;
using System.Linq;

using Utilities;

namespace Statistics.Validation
{
    class SummaryStatisticsValidator: IValidator<SummaryStatistics>
    {
        public SummaryStatisticsValidator()
        {
        }

        public bool IsValid(SummaryStatistics obj, out IEnumerable<IMessage> messages)
        {
            messages = ReportErrors(obj);
            return !messages.Any();
        }
        public IEnumerable<IMessage> ReportErrors(SummaryStatistics obj)
        {
            IList<IMessage> messages = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException($"The {obj.GetType()} is invalid because it is null");
            if (obj.SampleSize < 1) messages.Add(IMessageFactory.Factory(IMessageLevels.Error, "The provided sample is invalid because it contains no elements."));
            if (!obj.Minimum.IsFinite() || !obj.Maximum.IsFinite()) messages.Add(IMessageFactory.Factory(IMessageLevels.Error, "The sample contains non-finite elements that will produce illogical or useless results."));
            return messages;
        }
    }
}
