
using Functions.CoordinatesFunctions;
using System;
using System.Collections.Immutable;
using Xunit;

namespace FunctionsTests
{
    public class IFunctionFactoryTests
    {
        //#region Good Constructor Data Tests
        ///// <summary> Tests that for zero an IUnivariateFunction object with no Interpolation is constructed without exception. </summary>
        //[Fact]
        //public void IFunctionFactory_ZeroCoordinate_NoInterpolation_Returns_IUnivariateFunction()
        //{
        //    ICoordinate coordinate = ICoordinateFactory.Factory(IScalarFactory.Factory(0), IScalarFactory.Factory(0));
        //    List<ICoordinate> list = new List<ICoordinate>(coordinate);
        //    IFunction testObj = IFunctionFactory.Factory(list, InterpolationEnum.NoInterpolation);
        //    Assert.True(true);
        //}
        ///// <summary> Tests that for zero an IUnivariateFunction object with Linear Interpolation is constructed without exception. </summary>
        //[Fact]
        //public void IFunctionFactory_ZeroCoordinate_LinearInterpolation_Returns_IUnivariateFunction()
        //{
        //    ICoordinate coordinate = ICoordinateFactory.Factory(IScalarFactory.Factory(0), IScalarFactory.Factory(0));
        //    List<ICoordinate> list = new List<ICoordinate>(coordinate);
        //    IFunction testObj = IFunctionFactory.Factory(list, InterpolationEnum.Linear);
        //    Assert.True(true);
        //}
        ///// <summary> Tests that for zero an IUnivariateFunction object with Piecewise Interpolation is constructed without exception. </summary>
        //[Fact]
        //public void IFunctionFactory_ZeroCoordinate_PiecewiseInterpolation_Returns_IUnivariateFunction()
        //{
        //    ICoordinate coordinate = ICoordinateFactory.Factory(IScalarFactory.Factory(0), IScalarFactory.Factory(0));
        //    List<ICoordinate> list = new List<ICoordinate>(coordinate);
        //    IFunction testObj = IFunctionFactory.Factory(list, InterpolationEnum.Piecewise);
        //    Assert.True(true);
        //}
        //#endregion

        //#region Bad Constructor Data Tests
        ///// <summary> Tests that an empty list parameter with no Interpolation throws an ArgumentException. </summary>
        //[Fact]
        //public void IFunctionFactory_EmptyList_NoInterpolation_Throws_Exception()
        //{
        //    try
        //    {
        //        ICoordinate coordinate = ICoordinateFactory.Factory(IScalarFactory.Factory(0), IScalarFactory.Factory(0));
        //        List<ICoordinate> list = ImmutableList<ICoordinate>.Empty;
        //        IFunction testObj = IFunctionFactory.Factory(list, InterpolationEnum.NoInterpolation);
        //        Assert.True(false);
        //    }
        //    catch (ArgumentException e)
        //    {
        //        string m = e.Message;
        //        Assert.Equal("The specified collection is invalid because it is empty or contains null values.", m);
        //    }
        //}
        ///// <summary> Tests that an empty list parameter with Linear Interpolation throws an ArgumentException. </summary>
        //[Fact]
        //public void IFunctionFactory_EmptyList_LinearInterpolation_Throws_Exception()
        //{
        //    try
        //    {
        //        ICoordinate coordinate = ICoordinateFactory.Factory(IScalarFactory.Factory(0), IScalarFactory.Factory(0));
        //        List<ICoordinate> list = ImmutableList<ICoordinate>.Empty;
        //        IFunction testObj = IFunctionFactory.Factory(list, InterpolationEnum.Linear);
        //        Assert.True(false);
        //    }
        //    catch (ArgumentException e)
        //    {
        //        string m = e.Message;
        //        Assert.Equal("The specified collection is invalid because it is empty or contains null values.", m);
        //    }
        //}
        ///// <summary> Tests that an empty list parameter with Piecewise Interpolation throws an ArgumentException. </summary>
        //[Fact]
        //public void IFunctionFactory_EmptyList_PiecewiseInterpolation_Throws_Exception()
        //{
        //    try
        //    {
        //        ICoordinate coordinate = ICoordinateFactory.Factory(IScalarFactory.Factory(0), IScalarFactory.Factory(0));
        //        List<ICoordinate> list = ImmutableList<ICoordinate>.Empty;
        //        IFunction testObj = IFunctionFactory.Factory(list, InterpolationEnum.Piecewise);
        //        Assert.True(false);
        //    }
        //    catch (ArgumentException e)
        //    {
        //        string m = e.Message;
        //        Assert.Equal("The specified collection is invalid because it is empty or contains null values.", m);
        //    }
        //}
        //#endregion
    }
}
