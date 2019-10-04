using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

using Xunit;

using Statistics.Histograms;

namespace StatisticsTests.Histograms
{
    public class HistogramTests
    {
        [Theory]
        [InlineData(2, 1.0, 1.0, 2.0)]
        [InlineData(2, 2.0, 1.0, 3.0)]
        [InlineData(2, 1.0, 1.0, 2.0, 2.0)]
        [InlineData(3, 1.0, 1.0, 2.0, 3.0)]
        public void Histogram_ProperBinning(int nBins, double expectedWidth, params double[] xs)
        {
            Histogram h = new Histogram(xs, nBins: nBins);
            Assert.Equal(expectedWidth, h.Bins[0].Maximum - h.Bins[0].Minimum);
        }
    }
}
