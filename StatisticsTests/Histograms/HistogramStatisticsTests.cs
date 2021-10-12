using Statistics;
using Statistics.Histograms;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace StatisticsTests.Histograms
{

    [ExcludeFromCodeCoverage]
    public class HistogramStatisticsTests
    {
        [Theory]
        [InlineData(1, 0, 5, 3)]
        public void HistogramStatistics_Mean(double binWidth, double min, double max, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(binWidth, min, max);
            Histogram.AddObservationsToHistogram(histogram, obs);
            double actual = histogram.Mean;
            Assert.Equal(expected, actual);
        }
    }
}
