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
        public void Bins_SingleBinDataOnMin_Returns_ExpectedSingleBin()
        {
            double[] testData = new double[1] { 0 };
            var testObj = new Histogram(testObj, 0);
        }
    }
}
