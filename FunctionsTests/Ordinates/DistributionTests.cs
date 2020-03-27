using Functions;
using Functions.Ordinates;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Linq;
using Xunit;

namespace FunctionsTests.Ordinates
{
    [ExcludeFromCodeCoverage]

    public class DistributionTests : DistributionTestData
    {
    //    /// <summary>
    //    /// Tests the Distribution constructor with Normal distribution.
    //    /// </summary>
    //    /// <param name="value"></param>

    //    [Theory]
    //    [MemberData(nameof(GoodData_Distributed))]
    //    public void Distribution_GoodInput_Returns_Distribution(IDistributedOrdinate value)
    //    {
    //        Distribution distribution = new Distribution(value);
    //        Assert.NotNull(distribution);
    //    }

    //    /// <summary>
    //    /// Tests the range property returns correct value.
    //    /// </summary>
    //    /// <param name="value"></param>
    //    [Theory]
    //    [MemberData(nameof(GoodData_Distributed))]
    //    public void Range_GoodInput_Returns_Tuple(IDistributedOrdinate value)
    //    {
    //        double min = value.Range.Min;
    //        double max = value.Range.Max;
    //        Distribution dist = new Distribution(value);

    //        Tuple<double, double> range = dist.Range;
    //        Assert.True(range.Item1 == min && range.Item2 == max);
    //    }

    //    ///// <summary>
    //    ///// Tests that the IsDistributed property always returns false.
    //    ///// </summary>
    //    //[Theory]
    //    //[MemberData(nameof(GoodData_Distributed))]
    //    //public void IsDistributed_GoodInput_Returns_Bool(IDistributedValue value)
    //    //{
    //    //    Distribution dist = new Distribution(value);
    //    //    Assert.True(dist.IsDistributed);
    //    //}

    //    /// <summary>
    //    /// Tests the Equals method returns true for Constants that have been given the same value.
    //    /// </summary>

    //    [Theory]
    //    [MemberData(nameof(GoodData_Distributed))]
    //    public void Equals_GoodInput_Returns_Bool(IDistributedOrdinate value)
    //    {
    //        Distribution dist1 = new Distribution(value);
    //        Distribution dist2 = new Distribution(value);

    //        Assert.True(dist1.Equals(dist2));
    //    }

    //    /// <summary>
    //    /// Tests the Equals method returns false for Constants that have been given the different value.
    //    /// </summary>
    
    //    [Fact]
    //    public void Equals_BadInput_Returns_Bool()
    //    {
    //        Distribution dist1 = new Distribution(new Distribution(new Normal(1, 1)));
    //        Distribution dist2 = new Distribution(new Distribution(new Normal(.5, 1)));


    //        Assert.False(dist1.Equals(dist2));
    //    }

    //    /// <summary>
    //    /// Tests that the Value property returns the value passed to the Constant.
    //    /// </summary>
    //    [Fact]
    //    public void Value_GoodInput_Returns_Double()
    //    {
    //        Distribution dist1 = new Distribution(new Distribution(new Normal(1, 1)));
    //        double distValue = dist1.Value(.5);
    //        Assert.True(distValue == 1);
    //    }

    

    }
}
