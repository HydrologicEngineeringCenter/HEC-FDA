using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Statistics
{
    //public static class ISummaryStatisticsFactory
    //{
    //    public static ISampleStatistics Factory(IEnumerable<double> data) => !data.IsNullOrEmpty() ? Factory(IDataFactory.Factory(data)) : throw new ArgumentException($"The {typeof(ISampleStatistics)} cannot be constructed because the specified {typeof(IEnumerable<double>)} is null or empty.");
    //    public static ISampleStatistics Factory(IData data) => !data.IsNull() ? new SummaryStatistics(data) : throw new ArgumentNullException(nameof(data));
    //    public static ISampleStatistics Factory(IBin[] bins) => !bins.IsNullOrEmpty() ? new Histograms.HistogramStatistics(bins) : throw new ArgumentException($"The {typeof(ISampleStatistics)} cannot be constructed because the specified {typeof(IBin)}s are null or empty.");
    //}
}
