using Functions;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FunctionsTests.CoordinatesFunctions
{
    
    public class DistributedUnivariateCoordinatesFunctionTests
    {
        //private readonly ITestOutputHelper output;

        //public DistributedUnivariateCoordinatesFunctionTests(ITestOutputHelper output)
        //{
        //    this.output = output;
        //}

        //#region Test Delegate Functions
        //public static ICoordinate<IOrdinate, IOrdinate> Square(double val) => new UnivariateCoordinate(IScalarFactory.Factory(val), IScalarFactory.Factory(val * val));
        //private static readonly Func<double, ICoordinate<IOrdinate, IOrdinate>> map = Square;
        //#endregion

        //#region Good Constructor Data Tests
        ///// <summary> Constructs Good Input Data for Good Constructor Data Tests. </summary>
        //public static TheoryData<IImmutableList<ICoordinate<IOrdinate, IOrdinate>>> GoodDataNotDistributed =>
        //    new TheoryData<IImmutableList<ICoordinate<IOrdinate, IOrdinate>>>
        //    {
        //        { ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map)) },
        //        { ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map)) },
        //        { ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(Double.MaxValue), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(Double.MinValue), map)) },
        //        { ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(Double.MinValue), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(Double.MaxValue), map)) },
        //        { ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(Double.MaxValue), IScalarFactory.Factory(0), null), new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(Double.MinValue), null)) },
        //        { ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(Double.MinValue), IScalarFactory.Factory(0), null), new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(Double.MaxValue), null)) }
        //    };

        //public static TheoryData<IImmutableList<ICoordinate<IOrdinate, IOrdinate>>> GoodDataDistributed =>
        //    new TheoryData<IImmutableList<ICoordinate<IOrdinate, IOrdinate>>>
        //    {
        //        { ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(new ScalarHistogram(), new ScalarHistogram(), map)) },
        //        { ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(new ScalarDistributed(new Normal()), new ScalarDistributed(new Normal()), map)) },
        //        { ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(new ScalarHistogram(), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarHistogram(), map)) },
        //        { ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarHistogram(), map), new UnivariateCoordinate(new ScalarHistogram(), IScalarFactory.Factory(0), map)) },
        //        { ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(new ScalarDistributed(new Normal()), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarDistributed(new Normal()), map)) },
                
        //    };

        ///// <summary> Tests that for Good input it constructs the object without exception. </summary>
        //[Theory]
        //[MemberData(nameof(GoodDataNotDistributed))]
        //public void DistributedUnivariateCoordinatesFunction_GoodInput_Returns_DistributedUnivariateCoordinatesFunction(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> value)
        //{
        //    ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(value);
        //    Assert.True(true);
        //}
        //#endregion

        //#region Bad Constructor Data Tests
        ///// <summary> Tests that for null input it throws an ArgumentException. </summary>
        //[Fact]
        //public void DistributedUnivariateCoordinatesFunction_NullInput_Throws_ArgumentException()
        //{
        //    try
        //    {
        //        ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(null);
        //        Assert.True(false);
        //    }
        //    catch (ArgumentException e)
        //    {
        //        string m = e.Message;
        //        Assert.Equal("The specified collection is invalid because it is empty or contains null values.", m);
                
        //    }
        //}
        ///// <summary> Tests that for bad distributed input it throws an ArgumentException. </summary>
        //[Fact]
        //public void DistributedUnivariateCoordinatesFunction_BadDistributedInput_Throws_ArgumentException()
        //{
        //    try
        //    {
        //        ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarDistributed(new Normal()), map), new UnivariateCoordinate(new ScalarDistributed(new Normal()), IScalarFactory.Factory(0), map)));
        //        Assert.True(false);
        //    }
        //    catch (ArgumentException e)
        //    {
        //        string m = e.Message;
        //        Assert.Equal("The specified set of coordinate is invalid. At least one x value maps to more than one y value (e.g. the set does not meet the definition of a function).", m);
        //    }
        //}
        //#endregion

        //#region Property Tests
        //#region IsInvertible Property Tests // from UnivariantCoordinatesFunction
        ///// <summary> Tests that for the IsInvertible Property it returns true. </summary>
        //[Theory]
        //[MemberData(nameof(GoodDataNotDistributed))]
        //public void DistributedUnivariateCoordinatesFunction_IsInvertible(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> value)
        //{
        //    ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(value);
        //    Assert.True(((CoordinatesFunction)testObj).IsInvertible);
        //}
        //#endregion
        //#region Coordinates Property Tests // from UnivariantCoordinatesFunction
        ///// <summary> Tests that for the Coordinates Property it returns a UnivariantCoordinatesFunction </summary>
        //[Theory]
        //[MemberData(nameof(GoodDataNotDistributed))]
        //public void DistributedUnivariateCoordinatesFunction_Coordinates_Returns_UnivariantCoordinatesFunction(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> value)
        //{
        //    ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(value);
        //    IImmutableList<ICoordinate<IOrdinate, IOrdinate>> testObj2 = ((CoordinatesFunction)testObj).Coordinates;
        //    Assert.True(true);
        //}
        //#endregion
        //#region IsDistributedXs Property Tests
        ///// <summary> Tests that the IsDistributedXs Property returns True. </summary>
        //[Theory]
        //[MemberData(nameof(GoodDataDistributed))]
        //public void DistributedUnivariateCoordinatesFunction_IsDistributedXs_Returns_True(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> value)
        //{
        //    ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(value);
        //    Assert.True(((CoordinatesFunctionVariableYs)testObj).IsDistributedXs);
        //}
        ///// <summary> Tests that the IsDistributedXs Property returns False. </summary>
        //[Theory]
        //[MemberData(nameof(GoodDataNotDistributed))]
        //public void DistributedUnivariateCoordinatesFunction_IsDistributedXs_Returns_False(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> value)
        //{
        //    ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(value);
        //    Assert.False(((CoordinatesFunctionVariableYs)testObj).IsDistributedXs);
        //}
        //#endregion
        //#region IsDistributedYs Property Tests
        ///// <summary> Tests that the IsDistributedYs Property returns True. </summary>
        //[Theory]
        //[MemberData(nameof(GoodDataDistributed))]
        //public void DistributedUnivariateCoordinatesFunction_IsDistributedYs_Returns_True(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> value)
        //{
        //    ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(value);
        //    Assert.True(((CoordinatesFunctionVariableYs)testObj).IsDistributedYs);
        //}
        ///// <summary> Tests that the IsDistributedYs Property returns False. </summary>
        //[Theory]
        //[MemberData(nameof(GoodDataNotDistributed))]
        //public void DistributedUnivariateCoordinatesFunction_IsDistributedYs_Returns_False(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> value)
        //{
        //    ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(value);
        //    Assert.False(((CoordinatesFunctionVariableYs)testObj).IsDistributedYs);
        //}
        //#endregion
        //#region Order Property Tests // No setters here yet
        /////// <summary></summary>
        ////[Fact]
        ////public void DistributedUnivariateCoordinatesFunction_OrderProperty()
        ////{
        ////}
        //#endregion
        //#endregion

        //#region Function Tests
        //#region F() Tests
        ///// <summary> Tests that the F Function returns True. </summary>
        //[Theory]
        //[MemberData(nameof(GoodDataNotDistributed))]
        //public void DistributedUnivariateCoordinatesFunction_NotDistributedF_Returns_IScalar(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> value)
        //{
        //    ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(value);
        //    IOrdinate result = testObj.F(IScalarFactory.Factory(0));
        //    Assert.True(result.Equals(IScalarFactory.Factory(0)) || result.Equals(IScalarFactory.Factory(Double.MaxValue)) || result.Equals(IScalarFactory.Factory(Double.MinValue)));
        //}
        ///// <summary> Tests that the F Function throws an <see cref="ArgumentOutOfRangeException"/>. </summary>
        //[Theory]
        //[MemberData(nameof(GoodDataNotDistributed))]
        //public void DistributedUnivariateCoordinatesFunction_NotDistributedF_Throws_ArgumentOutOfRangeException(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> value)
        //{
        //    try
        //    {
        //        ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(value);
        //        IOrdinate result = testObj.F(IScalarFactory.Factory(1));
        //        Assert.True(false);
        //    }
        //    catch (ArgumentOutOfRangeException e)
        //    {
        //        string m = e.Message;
        //        Assert.Equal("Specified argument was out of the range of valid values.\r\nParameter name: The specified x value was not found in any of the coordinates. Interpolation is not supported for coordinates with distributed x or y values", m);
        //    }
        //}
        ///// <summary> Tests that the F Function throws an <see cref="ArgumentNullException"/>. </summary>
        //[Theory]
        //[MemberData(nameof(GoodDataNotDistributed))]
        //public void DistributedUnivariateCoordinatesFunction_NullF_Throws_ArgumentNullException(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> value)
        //{
        //    try
        //    {
        //        ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(value);
        //        IOrdinate result = testObj.F(null);
        //        Assert.True(false);
        //    }
        //    catch (ArgumentNullException e)
        //    {
        //        string m = e.Message;
        //        Assert.Equal("Value cannot be null.", m);
        //    }
        //}
        ///// <summary> Tests that the F Function returns an IScalar object. </summary>
        //[Fact]
        //public void DistributedUnivariateCoordinatesFunction_F_Returns_IScalar()
        //{
        //    ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(1), map)));
        //    IOrdinate result = testObj.F(IScalarFactory.Factory(0));
        //    Assert.Equal("1", result.Print());
        //}
        ///// <summary> Tests that the F Function returns a ScalarHistogram object. </summary>
        //[Fact]
        //public void DistributedUnivariateCoordinatesFunction_F_Returns_ScalarHistogram()
        //{
        //    ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarHistogram(), map), new UnivariateCoordinate(new ScalarHistogram(), IScalarFactory.Factory(0), map)));
        //    IOrdinate result = testObj.F(IScalarFactory.Factory(0));
        //    Assert.Equal("Histogram(Mean: 0, Standard Deviation: 0, Min: 0, Max: 0, Sample Size: 0, Is Converged: False)", result.Print());
        //}
        ///// <summary> Tests that the F Function returns a ScalarDistributed object. </summary>
        //[Fact]
        //public void DistributedUnivariateCoordinatesFunction_F_Returns_ScalarDistributed()
        //{
        //    ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(new ScalarDistributed(new Normal()), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarDistributed(new Normal()), map)));
        //    IOrdinate result = testObj.F(IScalarFactory.Factory(0));
        //    Assert.Equal("Normal(Mean: 0, StandardDeviation: 1)", result.Print());
        //}
        ///// <summary> Tests that the F Function for Histogram input throws an <see cref="ArgumentOutOfRangeException"/>. </summary>
        //[Fact]
        //public void DistributedUnivariateCoordinatesFunction_FHistogram_Throws_ArgumentOutOfRangeException()
        //{
        //    try
        //    {
        //        ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(new ScalarHistogram(), new ScalarHistogram(), map)));
        //        IOrdinate result = testObj.F(IScalarFactory.Factory(0));
        //        Assert.True(false);
        //    }
        //    catch (ArgumentOutOfRangeException e)
        //    {
        //        string m = e.Message;
        //        Assert.Equal("Specified argument was out of the range of valid values.\r\nParameter name: The specified x value was not found in any of the coordinates. Interpolation is not supported for coordinates with distributed x or y values", m);
        //    }
        //}
        ///// <summary> Tests that the F Function for Distributed input throws an <see cref="ArgumentOutOfRangeException"/>. </summary>
        //[Fact]
        //public void DistributedUnivariateCoordinatesFunction_FDistributed_Throws_ArgumentOutOfRangeException()
        //{
        //    try
        //    {
        //        ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(new ScalarDistributed(new Normal()), new ScalarDistributed(new Normal()), map)));
        //        IOrdinate result = testObj.F(IScalarFactory.Factory(0));
        //        Assert.True(false);
        //    }
        //    catch (ArgumentOutOfRangeException e)
        //    {
        //        string m = e.Message;
        //        Assert.Equal("Specified argument was out of the range of valid values.\r\nParameter name: The specified x value was not found in any of the coordinates. Interpolation is not supported for coordinates with distributed x or y values", m);
        //    }
        //}
        //#endregion
        //#region InverseF() Tests
        ///// <summary> Tests that the Inverse F Function returns True. </summary>
        //[Theory]
        //[MemberData(nameof(GoodDataNotDistributed))]
        //public void DistributedUnivariateCoordinatesFunction_InverseF_Returns_IScalar(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> value)
        //{
        //    ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(value);
        //    IOrdinate result = testObj.InverseF(IScalarFactory.Factory(0));
        //    Assert.NotNull(result);
        //}
        ///// <summary> Tests that the Inverse F Function returns True. </summary>
        //[Fact]
        //public void DistributedUnivariateCoordinatesFunction_InverseF_Throws_ArgumentException()
        //{
        //    try
        //    {
        //        ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(2), map)));
        //        IOrdinate result = testObj.InverseF(IScalarFactory.Factory(0));
        //        Assert.True(false);
        //    }
        //    catch (ArgumentException e)
        //    {
        //        var m = e.Message;
        //        Assert.Equal("The specified set of coordinate is invalid. At least one x value maps to more than one y value (e.g. the set does not meet the definition of a function).", m);
        //    }
        //}
        ///// <summary> Tests that the Inverse F Function returns False. </summary>
        //[Fact]
        //public void DistributedUnivariateCoordinatesFunction_InverseFNull_Throws_ArgumentNullException()
        //{
        //    try
        //    {
        //        ICoordinatesFunction<IOrdinate, IOrdinate> testObj = new CoordinatesFunctionVariableYs(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(0), map)));
        //        IOrdinate result = testObj.InverseF(null);
        //        Assert.True(false);
        //    }
        //    catch (ArgumentNullException e)
        //    {
        //        var m = e.Message;
        //        Assert.Equal("Value cannot be null.", m);
        //    }
        //}
        ////Is never reached
        /////// <summary> Tests that the Inverse F Function when given an input not in the given coordinates, throws an <see cref="ArgumentOutOfRangeException"/>. </summary>
        ////[Fact]
        ////public void DistributedUnivariateCoordinatesFunction_InverseF_Throws_ArgumentOutOfRangeException()
        ////{
        ////    try
        ////    {
        ////        IFunctionFamily<IScalar, IScalar> testObj = new DistributedUnivariateCoordinatesFunction(ImmutableList.Create<ICoordinate<IScalar, IScalar>>(new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(2), map)));
        ////        IScalar result = testObj.InverseF(IScalarFactory.Factory(5));
        ////        Assert.True(false);
        ////    }
        ////    catch (ArgumentOutOfRangeException e)
        ////    {
        ////        var m = e.Message;
        ////        Assert.Equal("The specified y value was not found in any of the coordinates. Interpolation is not supported for coorindates with distributed x or y values.", m);
        ////    }
        ////}
        //#endregion
        //#endregion
    }
}
