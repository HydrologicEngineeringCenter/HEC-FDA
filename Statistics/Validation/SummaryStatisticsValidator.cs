using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Utilities.Validation;

namespace Statistics.Validation
{
    class SummaryStatisticsValidator: IValidator<SummaryStatistics>
    {
        public SummaryStatisticsValidator()
        {
        }

        public bool IsValid(SummaryStatistics obj, out IEnumerable<string> messages)
        {
            messages = ReportErrors(obj);
            return !messages.Any();
        }
        public IEnumerable<string> ReportErrors(SummaryStatistics obj)
        {
            IList<string> messages = new List<string>();
            if (obj.SampleSize < 1) messages.Add("The provided sample is invalid because it contains no elements.");
            if (!obj.Minimum.IsFinite() || !obj.Maximum.IsFinite()) messages.Add("The sample contains non-finite elements that will produce illogical or useless results.");
            return messages;
        }
    }
}
