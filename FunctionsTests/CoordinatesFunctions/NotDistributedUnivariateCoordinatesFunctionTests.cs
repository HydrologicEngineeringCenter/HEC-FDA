using Functions;
using Functions.Coordinates;
using Functions.CoordinatesFunctions;
using Statistics;
using System;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace FunctionsTests.CoordinatesFunctions
{
    public class NotDistributedUnivariateCoordinatesFunctionTests
    {
    //    #region Test Delegate Functions

    //    public static ICoordinate<IOrdinate, IOrdinate> Square(double val) => (ICoordinate<IOrdinate, IOrdinate>)new CoordinateConstants(val, val * val);
    //    private static readonly Func<double, ICoordinate<IOrdinate, IOrdinate>> map = Square;
    //    #endregion

    //    #region Good Constructor Data Tests
    //    /// <summary> Constructs Good Input Data for Good Constructor Data Tests. </summary>
    //    CoordinateConstants val1 = new CoordinateConstants(0, 1);
    //    CoordinateConstants val2 = new CoordinateConstants(1, 2);
    //    public static TheoryData<IImmutableList<ICoordinate<IOrdinate, IOrdinate>>> GoodDataNotDistributed =>
    //        new TheoryData<IImmutableList<ICoordinate<IOrdinate, IOrdinate>>>
    //        {
    //            { CoordinateFunctionsTestData.ScalarCoordinates(new double[]{0}, new double[]{0})},
    //            {  CoordinateFunctionsTestData.ScalarCoordinates(new double[]{0,0}, new double[]{0,0})},
    //            {  CoordinateFunctionsTestData.ScalarCoordinates(new double[]{Double.MaxValue,0}, new double[]{0,Double.MinValue})},
    //            {  CoordinateFunctionsTestData.ScalarCoordinates(new double[]{Double.MinValue,0}, new double[]{0,Double.MaxValue})},
    //        };
    
    //    public static TheoryData<IImmutableList<ICoordinate<IOrdinate, IOrdinate>>> BadDataDistributed =>
    //        new TheoryData<IImmutableList<ICoordinate<IOrdinate, IOrdinate>>>
    //        {
    //            { ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(new ScalarHistogram(), new ScalarHistogram(), map)) },
    //            { ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(new ScalarDistributed(new Normal()), new ScalarDistributed(new Normal()), map)) },
    //            { ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(new ScalarHistogram(), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarHistogram(), map)) },
    //            { ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarHistogram(), map), new UnivariateCoordinate(new ScalarHistogram(), IScalarFactory.Factory(0), map)) },
    //            { ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(new ScalarDistributed(new Normal()), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarDistributed(new Normal()), map)) }
    //        };
    //    /// <summary> Tests that with good input a NotDistributedUnivariateCoordinatesFunction object is constructed. </summary>
    //    [Theory]
    //    [MemberData(nameof(GoodDataNotDistributed))]
    //    public void NotDistributedUnivariateCoordinatesFunction_GoodInputNoInterpolation_Returns_NotDistributedUnivariateCoordinatesFunction(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> value)
    //    {
    //        IFunction testObj = new CoordinatesFunctionConstants(value);
    //        Assert.True(true);
    //    }
    //    /// <summary> Tests that with good input a NotDistributedUnivariateCoordinatesFunction object is constructed. </summary>
    //    [Theory]
    //    [MemberData(nameof(GoodDataNotDistributed))]
    //    public void NotDistributedUnivariateCoordinatesFunction_GoodLinearInterpolation_Returns_NotDistributedUnivariateCoordinatesFunction(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> value)
    //    {
    //        IFunction testObj = new CoordinatesFunctionConstants(value, InterpolationEnum.Linear);
    //        Assert.True(true);
    //    }
    //    /// <summary> Tests that with good input a NotDistributedUnivariateCoordinatesFunction object is constructed. </summary>
    //    [Theory]
    //    [MemberData(nameof(GoodDataNotDistributed))]
    //    public void NotDistributedUnivariateCoordinatesFunction_GoodPiecewiseInterpolation_Returns_NotDistributedUnivariateCoordinatesFunction(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> value)
    //    {
    //        IFunction testObj = new CoordinatesFunctionConstants(value, InterpolationEnum.Piecewise);
    //        Assert.True(true);
    //    }
    //    #endregion

    //    #region Bad Constructor Data Tests
    //    /// <summary> Tests that with bad input it throws an <see cref="ArgumentException"/>. </summary>
    //    [Theory]
    //    [MemberData(nameof(BadDataDistributed))]
    //    public void NotDistributedUnivariateCoordinatesFunction_BadInput_Throws_ArgumentException(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> value)
    //    {
    //        try
    //        {
    //            IFunction testObj = new CoordinatesFunctionConstants(value);
    //            Assert.True(false);
    //        }
    //        catch (ArgumentException e)
    //        {
    //            string m = e.Message;
    //            Assert.Equal("The specifed set of coordinates is invalid. One or more of the x or y values are distributed.", m);
    //        }
    //    }
    //    #endregion

    //    #region Property Tests
    //    #region Range Property Tests 

    //    #endregion
    //    #region Domain Property Tests 

    //    #endregion
    //    #region Interpolation Property Tests
    //    /// <summary> Tests that with No Interpolation it returns a NoInterpolation enum. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_NoInterpolation_Returns_NoInterpolation()
    //    {
    //        IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map)));
    //        Assert.Equal(InterpolationEnum.NoInterpolation, testObj.Interpolation);
    //    }
    //    /// <summary> Tests that with good input a NotDistributedUnivariateCoordinatesFunction object is constructed. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_GoodLinearInterpolation_Returns_LinearInterpolation()
    //    {
    //        IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map)), InterpolationEnum.Linear);
    //        Assert.Equal(InterpolationEnum.Linear, testObj.Interpolation);
    //    }
    //    /// <summary> Tests that with good input a NotDistributedUnivariateCoordinatesFunction object is constructed. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_GoodPiecewiseInterpolation_Returns_PiecewiseInterpolation()
    //    {
    //        IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map)), InterpolationEnum.Piecewise);
    //        Assert.Equal(InterpolationEnum.Piecewise, testObj.Interpolation);
    //    }
    //    #endregion
    //    #endregion

    //    #region Function Tests
    //    #region F() Tests
    //    /// <summary> Tests that with zero input, No Interpolation it returns an IScalar object. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_FNoInterpolation_Returns_IScalar()
    //    {
    //        IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(1), map)));
    //        var testObj2 = testObj.F(IScalarFactory.Factory(0));
    //        Assert.True(IScalarFactory.Factory(0).Equals(testObj2));
    //    }
    //    /// <summary> Tests that with zero input, No Interpolation it returns an IScalar object. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_FNoInterpolation_Returns_YValueIScalar()
    //    {
    //        IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(2), map)));
    //        var testObj2 = testObj.F(IScalarFactory.Factory(1));
    //        Assert.True(IScalarFactory.Factory(2).Equals(testObj2));
    //    }
    //    /// <summary> Tests that with No Interpolation F() will throw an <see cref="ArgumentOutOfRangeException"/>. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_FNoInterpolation_Throws_ArgumentOutOfRangeException()
    //    {
    //        try
    //        {
    //            IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(1), map)));
    //            var testObj2 = testObj.F(IScalarFactory.Factory(5));
    //            Assert.True(false);
    //        }
    //        catch (ArgumentOutOfRangeException e)
    //        {
    //            var m = e.Message;
    //            Assert.Equal("Specified argument was out of the range of valid values.\r\nParameter name: The specified x value: 5 is invalid because it is not on the domain of the coordinates function [0, 1].", m);
    //        }
    //    }
    //    /// <summary> Tests that with no interpolation F() will throw an <see cref="InvalidOperationException"/>. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_FNoInterpolation_Throws_InvalidOperationException()
    //    {
    //        try
    //        {
    //            IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(1), map)));
    //            var testObj2 = testObj.F(IScalarFactory.Factory(0.5));
    //            Assert.False(true);
    //        }
    //        catch (InvalidOperationException e)
    //        {
    //            var m = e.Message;
    //            Assert.Equal("The F(x) operation cannot produce a result because no interpolation method has been set and the specified x value: 0.5 was not explicitly provided as part of the function domain.", m);
    //        }
    //    }
    //    /// <summary> Tests that with no interpolation F() will throw an <see cref="ArgumentOutOfRangeException"/>. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_FLinearInterpolation_Throws_ArgumentOutOfRangeException()
    //    {
    //        try
    //        {
    //            IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(1), map)), InterpolationEnum.Linear);
    //            var testObj2 = testObj.F(IScalarFactory.Factory(5));
    //            Assert.False(true);
    //        }
    //        catch (ArgumentOutOfRangeException e)
    //        {
    //            var m = e.Message;
    //            Assert.Equal("Specified argument was out of the range of valid values.\r\nParameter name: The specified x value: 5 is invalid because it is not on the domain of the coordinates function [0, 1].", m);
    //        }
    //    }
    //    /// <summary> Tests that with linear interpolation F() will return an IScalar object. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_FLinearInterpolation_Returns_IScalar()
    //    {
    //            IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(1), map)), InterpolationEnum.Linear);
    //            var testObj2 = testObj.F(IScalarFactory.Factory(0.5));
    //            Assert.True(IScalarFactory.Factory(0.5).Equals(testObj2));
    //    }
    //    /// <summary> Tests that with piecewise interpolation F() will return an IScalar object with value rounded up to 1. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_FPiecewiseInterpolation_HigherBound_Returns_IScalar()
    //    {
    //        IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(1), map)), InterpolationEnum.Piecewise);
    //        var testObj2 = testObj.F(IScalarFactory.Factory(0.5));
    //        Assert.True(IScalarFactory.Factory(1).Equals(testObj2));
    //    }
    //    /// <summary> Tests that with linear interpolation F() will return an IScalar object with value rounded down to 0. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_FPiecewiseInterpolation_LowerBound_Returns_IScalar()
    //    {
    //        IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(1), map)), InterpolationEnum.Piecewise);
    //        var testObj2 = testObj.F(IScalarFactory.Factory(0.4));
    //        Assert.True(IScalarFactory.Factory(0).Equals(testObj2));
    //    }
    //    #endregion
    //    #region InverseF() Tests
    //    /// <summary> Tests that with two coordinates, No Interpolation it returns an IScalar object. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_InverseFNoInterpolation_TwoCoordinates_Returns_IScalar()
    //    {
    //        IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(1), map)));
    //        var testObj2 = testObj.InverseF(IScalarFactory.Factory(0));
    //        Assert.True(IScalarFactory.Factory(0).Equals(testObj2));
    //    }
    //    /// <summary> Tests that with three coordinates, No Interpolation it returns an IScalar object. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_InverseFNoInterpolation_ThreeCoordinates_Returns_IScalar()
    //    {
    //        IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(1)), new UnivariateCoordinate(IScalarFactory.Factory(2), IScalarFactory.Factory(2), map)));
    //        var testObj2 = testObj.InverseF(IScalarFactory.Factory(2));
    //        Assert.True(IScalarFactory.Factory(2).Equals(testObj2));
    //    }
    //    /// <summary> Tests that with two coordinates, Linear Interpolation it returns an IScalar object. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_InverseFLinearInterpolation_TwoCoordinates_Returns_IScalar()
    //    {
    //        IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(1), map)), InterpolationEnum.Linear);
    //        var testObj2 = testObj.InverseF(IScalarFactory.Factory(0));
    //        Assert.True(IScalarFactory.Factory(0).Equals(testObj2));
    //    }
    //    /// <summary> Tests that with two coordinates, Piecewise Interpolation it returns an IScalar object. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_InverseFPiecewiseInterpolation_TwoCoordinates_Returns_IScalar()
    //    {
    //        IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(1), map)), InterpolationEnum.Piecewise);
    //        var testObj2 = testObj.InverseF(IScalarFactory.Factory(0));
    //        Assert.True(IScalarFactory.Factory(0).Equals(testObj2));
    //    }
    //    #endregion
    //    #region Compose() Tests
    //    /// <summary> Tests that with basic input, No Interpolation it returns an IUnivariateFunction. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_Compose_SameCoordinates_Returns_Function()
    //    {
    //        IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(1)), new UnivariateCoordinate(IScalarFactory.Factory(2), IScalarFactory.Factory(2), map)));
    //        IFunction testObj2 = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(1)), new UnivariateCoordinate(IScalarFactory.Factory(2), IScalarFactory.Factory(2), map)));
    //        IFunction fog = testObj.Compose(testObj2);
    //        Assert.True(fog.Equals(testObj));
    //    }
    //    /// <summary> Tests that with basic input, Linear Interpolation it returns an IUnivariateFunction. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_Compose_LinearDifferentCoordinates_Returns_Function()
    //    {
    //        IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(2), map), new UnivariateCoordinate(IScalarFactory.Factory(3), IScalarFactory.Factory(4)), new UnivariateCoordinate(IScalarFactory.Factory(5), IScalarFactory.Factory(7), map)), InterpolationEnum.Linear);
    //        IFunction testObj2 = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(4), map), new UnivariateCoordinate(IScalarFactory.Factory(3), IScalarFactory.Factory(2)), new UnivariateCoordinate(IScalarFactory.Factory(6), IScalarFactory.Factory(6), map)), InterpolationEnum.Linear);
    //        IFunction fog = testObj.Compose(testObj2);
    //        IFunction testObj3 = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(3)), new UnivariateCoordinate(IScalarFactory.Factory(1.5), IScalarFactory.Factory(2)), new UnivariateCoordinate(IScalarFactory.Factory(3), IScalarFactory.Factory(3.333333333333333))), InterpolationEnum.Linear);
    //        Assert.True(fog.Equals(testObj3));
    //    }
    //    /// <summary> Tests that with basic input, No Interpolation it throws an <see cref="ArgumentException"/>. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_Compose_DifferentCoordinates_Throw_ArgumentException()
    //    {
    //        try
    //        { 
    //            IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(2), map), new UnivariateCoordinate(IScalarFactory.Factory(3), IScalarFactory.Factory(4)), new UnivariateCoordinate(IScalarFactory.Factory(5), IScalarFactory.Factory(7), map)));
    //            IFunction testObj2 = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(4), map), new UnivariateCoordinate(IScalarFactory.Factory(3), IScalarFactory.Factory(2)), new UnivariateCoordinate(IScalarFactory.Factory(6), IScalarFactory.Factory(6), map)));
    //            IFunction fog = testObj.Compose(testObj2);
    //            Assert.True(false);
    //        }
    //        catch (ArgumentException e)
    //        {
    //            string m = e.Message;
    //            Assert.Equal("The specified collection is invalid because it is empty or contains null values.", m);
    //        }
    //    }
    //    /// <summary> Tests that with zero input, No Interpolation it throws an <see cref="InvalidOperationException"/>. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_BadCompose_Throws_InvalidOperationException()
    //    {
    //        try
    //        {
    //            IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(1)), new UnivariateCoordinate(IScalarFactory.Factory(2), IScalarFactory.Factory(2), map)));
    //            IFunction testObj2 = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(3), IScalarFactory.Factory(3), map), new UnivariateCoordinate(IScalarFactory.Factory(4), IScalarFactory.Factory(4)), new UnivariateCoordinate(IScalarFactory.Factory(5), IScalarFactory.Factory(5), map)));
    //            IFunction fog = testObj.Compose(testObj2);
    //            Assert.True(false);
    //        }
    //        catch (InvalidOperationException e)
    //        {
    //            string m = e.Message;
    //            Assert.Equal("The functional composition operation could not be performed. The range of F: [0, 2] in the composition equation F(G(x)) does not overlap the domain of G: [3, 5].", m);
    //        }
    //    }
    //    /// <summary> Tests that with zero input, No Interpolation it throws an <see cref="InvalidOperationException"/>. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_BadCompose_SameCoordinates_Throws_InvalidOperationException()
    //    {
    //        try
    //        {
    //            IFunction testObj2 = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(1)), new UnivariateCoordinate(IScalarFactory.Factory(2), IScalarFactory.Factory(2), map)));
    //            IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(3), IScalarFactory.Factory(3), map), new UnivariateCoordinate(IScalarFactory.Factory(4), IScalarFactory.Factory(4)), new UnivariateCoordinate(IScalarFactory.Factory(5), IScalarFactory.Factory(5), map)));
    //            IFunction fog = testObj.Compose(testObj2);
    //            Assert.True(false);
    //        }
    //        catch (InvalidOperationException e)
    //        {
    //            string m = e.Message;
    //            Assert.Equal("The functional composition operation could not be performed. The range of F: [3, 5] in the composition equation F(G(x)) does not overlap the domain of G: [0, 2].", m);
    //        }
    //    }
    //    #endregion
    //    #region RiemannSum() Tests
    //    /// <summary> Tests that RiemannSum() will return the expected double. </summary>
    //    [Fact]
    //    public void NotDistributedUnivariateCoordinatesFunction_RiemannSum_Returns_Double()
    //    {
    //        IFunction testObj = new CoordinatesFunctionConstants(ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(1), map)));
    //        Assert.True(testObj.RiemannSum() == 0.5);
    //    }
    //    #endregion
    //    #region Equals() Tests

    //    #endregion
    //    #endregion
    }
}
