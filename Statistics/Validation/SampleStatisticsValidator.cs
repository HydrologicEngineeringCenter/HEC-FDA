using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Statistics.Validation
{
    internal class SampleStatisticsValidator: Utilities.IValidator<ISampleStatistics>
    {
        internal SampleStatisticsValidator()
        {
        }

        public IMessageLevels IsValid(ISampleStatistics obj, out IEnumerable<Utilities.IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max();
        }
        public IEnumerable<Utilities.IMessage> ReportErrors(ISampleStatistics obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException($"The {typeof(ISampleStatistics)} object cannot be validated because it is null");
            if (obj.SampleSize < 5) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The sample statistics are invalid because sample size: {obj.SampleSize} is too small to compute some of the descriptive statistics. For instance, at least 4 observations are required to compute kurtosis."));
            //TODO: check for non finite values.
            return msgs;
        }
    }
}
