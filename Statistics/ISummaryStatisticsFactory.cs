using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Statistics
{
    public static class ISummaryStatisticsFactory
    {
        public static ISummaryStatistics Factory(IEnumerable<double> data) => !data.IsNullOrEmpty() ? Factory(IDataFactory.Factory(data)) : throw new ArgumentException($"The {typeof(ISummaryStatistics)} cannot be constructed because the specified {typeof(IEnumerable<double>)} is null or empty.");
        public static ISummaryStatistics Factory(IData data) => !data.IsNull() ? new SummaryStatistics(data) : throw new ArgumentNullException(nameof(data));
        public static ISummaryStatistics Factory(IBin[] bins) => !bins.IsNullOrEmpty() ? new Histograms.HistogramStatistics(bins) : throw new ArgumentException($"The {typeof(ISummaryStatistics)} cannot be constructed because the specified {typeof(IBin)}s are null or empty.");
    }
}
