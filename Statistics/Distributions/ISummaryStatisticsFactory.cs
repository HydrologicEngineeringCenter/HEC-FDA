using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics.Distributions
{
    public static class ISummaryStatisticsFactory
    {
        public static ISummaryStatistics Factory(IEnumerable<double> data) => new SummaryStatistics(IDataFactory.Factory(data));
    }
}
