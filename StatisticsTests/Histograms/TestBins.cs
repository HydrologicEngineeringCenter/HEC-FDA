using Statistics;
using Statistics.Histograms;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Utilities;
using Xunit;

namespace StatisticsTests.Histograms
{
    /// <summary>
    /// Contains sample bis for testing
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TestBins: TheoryData<IBin, Utilities.IRange<double>, double, int>
    {
        /// <summary>
        /// Bin(count: 1, range: [0.5, 1.5])
        /// </summary>
        public static IBin Bin1 => new Bin(0.5, 1.5, 1);
        /// <summary>
        /// Bin(count: 2, range: [1.5, 2.5])
        /// </summary>
        public static IBin Bin2 => new Bin(1.5, 2.5, 2);
        /// <summary>
        /// Bin(count: 3, range: [2.5, 3.5])
        /// </summary>
        public static IBin Bin3 => new Bin(2.5, 3.5, 3);

        public TestBins()
        {
            Add(Bin1, IRangeFactory.Factory(0.5, 1.5), 1d, 1);
            Add(Bin2, IRangeFactory.Factory(1.5, 2.5), 2d, 2);
            Add(Bin3, IRangeFactory.Factory(2.5, 3.5), 3d, 3);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Test 
    {
        //[Theory]
        //[ClassData(typeof(TestBins))]
        //public void TestMidPoint(IBin bin, double min, double max, double midPoint, int n)
        //{
        //    Assert.Equal(midPoint, bin.MidPoint);
        //}
    }

    
}

