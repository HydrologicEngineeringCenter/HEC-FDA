using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Statistics.Validation
{
    internal class SampleStatisticsValidator: Utilities.IValidator<ISummaryStatistics>
    {
        internal SampleStatisticsValidator()
        {
        }

        public IMessageLevels IsValid(ISummaryStatistics obj, out IEnumerable<Utilities.IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max();
        }
        public IEnumerable<Utilities.IMessage> ReportErrors(ISummaryStatistics obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException($"The {typeof(ISummaryStatistics)} object cannot be validated because it is null");
            if (obj.GetType() != typeof(SummaryStatistics)) throw new ArgumentException($"The {this.GetType()} can only validate {typeof(SampleStatistics)} objects but it was called by a {obj.GetType()} object causing an error.");
            if (obj.SampleSize < 5) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The sample statistics are invalid because sample size: {obj.SampleSize} is too small to compute some of the descriptive statistics. For instance, at least 4 observations are required to compute kurtosis."));
            //TODO: check for non finite values.
            return msgs;
        }
    }
}
