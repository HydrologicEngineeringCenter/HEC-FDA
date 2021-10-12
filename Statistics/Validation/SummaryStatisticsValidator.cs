using System;
using System.Collections.Generic;
using System.Linq;

using Utilities;

namespace Statistics.Validation
{
    class SummaryStatisticsValidator: IValidator<ISampleStatistics>
    {
        public SummaryStatisticsValidator()
        {
        }

        public IMessageLevels IsValid(ISampleStatistics obj, out IEnumerable<IMessage> messages)
        {
            messages = ReportErrors(obj);
            return messages.Max();
        }
        public IEnumerable<IMessage> ReportErrors(ISampleStatistics obj)
        {
            IList<IMessage> messages = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException(nameof(obj), "The summary statistics object could not be validated because it is null.");
            if (obj.SampleSize < 1) messages.Add(IMessageFactory.Factory(IMessageLevels.Error, "The sample is invalid because it contains no elements.", "Many of its properties will be set to double.NaN values."));
            if (obj.SampleSize == 1) messages.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The sample contains {obj.SampleSize} item(s) and therefore it has no dispersion.", $"As a result the sample {nameof(obj.Variance)}, {nameof(obj.StandardDeviation)}, {nameof(obj.Skewness)} parameters are set to 0."));
            if (obj.SampleSize == 2) messages.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The sample contains {obj.SampleSize} item(s) and therefore does not have the degrees of freedom required to compute sample {nameof(obj.Skewness)}.", $"The {nameof(obj.Skewness)} parameter has been set to 0."));
            return messages;
        }


    }
}
