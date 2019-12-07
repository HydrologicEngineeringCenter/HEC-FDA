using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{
    public static class ISummaryStatisticsFactory
    {
        public static ISummaryStatistics Factory(IEnumerable<double> data) => Factory(IDataFactory.Factory(data));
        public static ISummaryStatistics Factory(IData data) => new SummaryStatistics(data);
        public static ISummaryStatistics Factory(IBin[] bins) => new Histograms.HistogramStatistics(bins);
    }
}
