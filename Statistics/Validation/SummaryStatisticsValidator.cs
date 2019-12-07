using System;
using System.Collections.Generic;
using System.Linq;

using Utilities;

namespace Statistics.Validation
{
    class SummaryStatisticsValidator: IValidator<ISummaryStatistics>
    {
        public SummaryStatisticsValidator()
        {
        }

        public bool IsValid(ISummaryStatistics obj, out IEnumerable<IMessage> messages)
        {
            messages = ReportErrors(obj);
            return !messages.Any();
        }
        public IEnumerable<IMessage> ReportErrors(ISummaryStatistics obj)
        {
            IList<IMessage> messages = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException($"The {obj.GetType()} is invalid because it is null");
            if (obj.SampleSize < 1) messages.Add(IMessageFactory.Factory(IMessageLevels.Error, "The sample is invalid because it contains no elements. As a result many of its properties will be set to double.NaN values."));
            if (obj.SampleSize == 1) messages.Add(IMessageFactory.Factory(IMessageLevels.Message, "The sample contains a single data element, therefore it has no dispersion as a result the Variance, StandardDeviation and Skewness parameters are set to 0."));
            return messages;
        }
    }
}
