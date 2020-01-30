using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Statistics;
using Xunit;

namespace StatisticsTests.Histograms
{
    [ExcludeFromCodeCoverage]
    public class TestHistograms: TheoryData<IHistogram>
    {
        public static IHistogram Histogram1 => IHistogramFactory.Factory
            (
            IDataFactory.Factory(new double[] { 1, 2, 2, 3, 3, 3 }),
            0.5, 3.5, 1
            );

    }

    public class HistogramTests
    {
    }
}
