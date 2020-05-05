using Functions;
using Functions.Coordinates;
using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Utilities;
using Xunit;

namespace FunctionsTests.CoordinatesFunctions
{
    [ExcludeFromCodeCoverage]
    public class CoordinatesFunctionConstantsTests : CoordinateFunctionsTestData
    {
        //#region Test Delegate Functions

        //public static ICoordinate<IOrdinate, IOrdinate> Square(double val) => (ICoordinate<IOrdinate, IOrdinate>)new CoordinateConstants(val, val * val);
        //private static readonly Func<double, ICoordinate<IOrdinate, IOrdinate>> map = Square;
        //#endregion

        #region Good Constructor Data Tests
        /// <summary> Tests that with good input a CoordinatesFunctionConstants object is constructed. </summary>
        [Theory]
        [MemberData(nameof(GoodData_Constant))]
        public void CoordinatesFunctionConstants_GoodInputNoInterpolation_Returns_CoordinatesFunctionConstants(List<ICoordinate> value)
        {
            IFunction testObj = new CoordinatesFunctionConstants(value);
            Assert.True(true);
        }
        /// <summary> Tests that with good input a CoordinatesFunctionConstants object is constructed. </summary>
        [Theory]
        [MemberData(nameof(GoodData_Constant))]
        public void CoordinatesFunctionConstants_GoodLinearInterpolation_Returns_CoordinatesFunctionConstants(List<ICoordinate> value)
        {
            IFunction testObj = new CoordinatesFunctionConstants(value, InterpolationEnum.Linear);
            Assert.True(true);
        }
        ///// <summary> Tests that with good input a CoordinatesFunctionConstants object is constructed. </summary>
        [Theory]
        [MemberData(nameof(GoodData_Constant))]
        public void CoordinatesFunctionConstants_GoodPiecewiseInterpolation_Returns_CoordinatesFunctionConstants(List<ICoordinate> value)
        {
            IFunction testObj = new CoordinatesFunctionConstants(value, InterpolationEnum.Piecewise);
            Assert.True(true);
        }

        #endregion
        #region Bad Constructor Data Tests
        ///// <summary> Tests that with bad input it throws an <see cref="ArgumentException"/>. </summary>
        [Theory]
        [MemberData(nameof(BadData_Constant_Nan))]
        public void CoordinatesFunctionConstants_BadInput_NAN_Throws_ArgumentException(List<ICoordinate> value)
        {
            Assert.Throws<ArgumentException>(() => new CoordinatesFunctionConstants(value));
        }

        ///// <summary> Tests that with bad input it throws an <see cref="ArgumentException"/>. </summary>
        [Theory]
        [MemberData(nameof(BadData_Constant_NegativeInfinity))]
        public void CoordinatesFunctionConstants_BadInput_NegativeInfinity_Throws_ArgumentException(List<ICoordinate> value)
        {
            Assert.Throws<ArgumentException>(() => new CoordinatesFunctionConstants(value));
        }

        ///// <summary> Tests that with bad input it throws an <see cref="ArgumentException"/>. </summary>
        [Theory]
        [MemberData(nameof(BadData_Constant_PositiveInfinity))]
        public void CoordinatesFunctionConstants_BadInput_PositiveInfinity_Throws_ArgumentException(List<ICoordinate> value)
        {
            Assert.Throws<ArgumentException>(() => new CoordinatesFunctionConstants(value));
        }

        ///// <summary> Tests that with bad input it throws an <see cref="ArgumentException"/>. </summary>
        [Theory]
        [MemberData(nameof(BadData_Constant_RepeatXs))]
        public void CoordinatesFunctionConstants_BadInput_RepeatXs_Throws_ArgumentException(List<ICoordinate> value)
        {
            Assert.Throws<ArgumentException>(() => new CoordinatesFunctionConstants(value));
        }
        #endregion
        #region Property Tests
        #region Range Property Tests 
        [Theory]
        [InlineData(new double[4] {1d, 2d, 3d, 4d}, new double[4] {1d, 2d, 3d, 4d}, Functions.InterpolationEnum.None)]
        public void Range_StandardGoodData_Returns_ExpectedRange(double[] Xs, double[] Ys, InterpolationEnum interpolation)
        {
            var f = (Functions.CoordinatesFunctions.CoordinatesFunctionConstants)ICoordinatesFunctionsFactory.Factory(Xs, Ys, interpolation);
            Assert.Equal(IRangeFactory.Factory(1d, 4d), f.Range);
        }
        #endregion
        #region Domain Property Tests 

        #endregion
        #region Interpolation Property Tests
        /// <summary> Tests that with No Interpolation it returns a NoInterpolation enum. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_NoInterpolation_Returns_NoInterpolation()
        {
            IFunction testObj = CreateCoordinatesFunctionConstantsBasic();
            Assert.Equal(InterpolationEnum.None, testObj.Interpolator);
        }
        /// <summary> Tests that with Linear Interpolation it returns a Linear Interpolation enum. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_GoodLinearInterpolation_Returns_LinearInterpolation()
        {
            IFunction testObj = CreateCoordinatesFunctionConstantsBasic(InterpolationEnum.Linear);
            Assert.Equal(InterpolationEnum.Linear, testObj.Interpolator);
        }
        /// <summary> Tests that with Piecewise Interpolation it returns a Piecewise Interpolation enum. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_GoodPiecewiseInterpolation_Returns_PiecewiseInterpolation()
        {
            IFunction testObj = CreateCoordinatesFunctionConstantsBasic(InterpolationEnum.Piecewise);
            Assert.Equal(InterpolationEnum.Piecewise, testObj.Interpolator);
        }
        #endregion

        #region Order property tests
        /// <summary>
        /// Tests that the Order type is computed correctly.
        /// </summary>
        [Fact]
        public void CoordinatesFunctionConstants_OrderProperty_Returns_OrderEnum_NonMonotonic_StraightLine()
        {
            List<double> xs = new List<double>() { 1, 2, 3, 4 };
            List<double> ys = new List<double>() { 10, 10, 10, 10 };

            CoordinatesFunctionConstants func = CreateCoordinatesFunctionConstants(xs, ys);
            Assert.True(func.Order == OrderedSetEnum.NonMonotonic);
        }

        /// <summary>
        /// Tests that the Order type is computed correctly.
        /// </summary>
        [Fact]
        public void CoordinatesFunctionConstants_OrderProperty_Returns_OrderEnum_StrictlyIncreasing()
        {
            List<double> xs = new List<double>() { 1, 2, 3, 4 };
            List<double> ys = new List<double>() { 10, 20, 30, 40 };

            CoordinatesFunctionConstants func = CreateCoordinatesFunctionConstants(xs, ys);
            Assert.True(func.Order == OrderedSetEnum.StrictlyIncreasing);
        }
        /// <summary>
        /// Tests that the Order type is computed correctly.
        /// </summary>
        [Fact]
        public void CoordinatesFunctionConstants_OrderProperty_Returns_OrderEnum_StrictlyDecreasing()
        {
            List<double> xs = new List<double>() { 1, 2, 3, 4 };
            List<double> ys = new List<double>() { 10, 9, 8, 7 };

            CoordinatesFunctionConstants func = CreateCoordinatesFunctionConstants(xs, ys);
            Assert.True(func.Order == OrderedSetEnum.StrictlyDecreasing);
        }
        /// <summary>
        /// Tests that the Order type is computed correctly.
        /// </summary>
        [Fact]
        public void CoordinatesFunctionConstants_OrderProperty_Returns_OrderEnum_WeaklyDecreasing()
        {
            List<double> xs = new List<double>() { 1, 2, 3, 4 };
            List<double> ys = new List<double>() { 10, 9, 9, 7 };

            CoordinatesFunctionConstants func = CreateCoordinatesFunctionConstants(xs, ys);
            Assert.True(func.Order == OrderedSetEnum.WeaklyDecreasing);
        }

        /// <summary>
        /// Tests that the Order type is computed correctly.
        /// </summary>
        [Fact]
        public void CoordinatesFunctionConstants_OrderProperty_Returns_OrderEnum_WeaklyIncreasing()
        {
            List<double> xs = new List<double>() { 1, 2, 3, 4 };
            List<double> ys = new List<double>() { 7, 9, 9, 12 };

            CoordinatesFunctionConstants func = CreateCoordinatesFunctionConstants(xs, ys);
            Assert.True(func.Order == OrderedSetEnum.WeaklyIncreasing);
        }

        /// <summary>
        /// Tests that the Order type is computed correctly.
        /// </summary>
        [Fact]
        public void CoordinatesFunctionConstants_OrderProperty_Returns_OrderEnum_NonMonotonic()
        {
            List<double> xs = new List<double>() { 1, 2, 3, 4 };
            List<double> ys = new List<double>() { 7, 9, 8, 12 };

            CoordinatesFunctionConstants func = CreateCoordinatesFunctionConstants(xs, ys);
            Assert.True(func.Order == OrderedSetEnum.NonMonotonic);
        }
        #endregion

        #endregion

        //#region Function Tests
        #region F() Tests
        /// <summary> Tests that the proper Y value gets returned. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_FNoInterpolation_Returns_YValue()
        {
            List<double> xs = new List<double>() { 0, 1 };
            List<double> ys = new List<double>() { 10, 11 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys);
            IOrdinate testObjYValue = testObj.F(new Constant(0));
            Assert.True(10 == testObjYValue.Value());
        }
        /// <summary> Tests that the proper Y value gets returned. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_FNoInterpolation_Returns_YValue_2()
        {
            List<double> xs = new List<double>() { 0, 1 };
            List<double> ys = new List<double>() { 10, 11 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys);
            IOrdinate testObjYValue = testObj.F(new Constant(1));
            Assert.True(11 == testObjYValue.Value());
        }
        /// <summary> Tests that with No Interpolation F() will throw an <see cref="ArgumentOutOfRangeException"/> if the x value is outside the domain. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_FNoInterpolation_Throws_ArgumentOutOfRangeException()
        {
            List<double> xs = new List<double>() { 0, 1 };
            List<double> ys = new List<double>() { 0, 1 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys);
            Assert.Throws<ArgumentOutOfRangeException>(() => testObj.F(new Constant(2)));
        }
        /// <summary> Tests that with no interpolation F() will throw an <see cref="InvalidOperationException"/> if the x value is between values. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_FNoInterpolation_Throws_InvalidOperationException()
        {
            List<double> xs = new List<double>() { 0, 1 };
            List<double> ys = new List<double>() { 10, 11 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys);
            Assert.Throws<InvalidOperationException>(() => testObj.F(new Constant(.5)));
        }
        /// <summary> Tests that with no interpolation F() will throw an <see cref="ArgumentOutOfRangeException"/> if x value is outside the domain. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_FLinearInterpolation_Throws_ArgumentOutOfRangeException()
        {
            List<double> xs = new List<double>() { 0, 1 };
            List<double> ys = new List<double>() { 0, 1 };
            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys, InterpolationEnum.Linear);
            Assert.Throws<ArgumentOutOfRangeException>(() => testObj.F(new Constant(5)));
        }
        /// <summary> Tests that with linear interpolation F() will return an IOrdinate object. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_FLinearInterpolation_Returns_IOrdinate()
        {
            List<double> xs = new List<double>() { 0, 1 };
            List<double> ys = new List<double>() { 10, 11 };
            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys, InterpolationEnum.Linear);
            IOrdinate testObjYValue = testObj.F(new Constant(.5));
            Assert.True(10.5 == testObjYValue.Value());
        }
        /// <summary> Tests that with piecewise interpolation F() will return an IOrdinate object with value rounded up to 1. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_FPiecewiseInterpolation_HigherBound_Returns_IOrdinate()
        {
            List<double> xs = new List<double>() { 0, 1 };
            List<double> ys = new List<double>() { 0, 1 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys, InterpolationEnum.Piecewise);
            var testObjYValue = testObj.F(new Constant(.5));
            Assert.True(1 == testObjYValue.Value());
        }
        /// <summary> Tests that with linear interpolation F() will return an IOrdinate object with value rounded down to 0. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_FPiecewiseInterpolation_LowerBound_Returns_IOrdinate()
        {
            List<double> xs = new List<double>() { 0, 1 };
            List<double> ys = new List<double>() { 0, 1 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys, InterpolationEnum.Piecewise);
            var testObjYValue = testObj.F(new Constant(.4));
            Assert.True(0 == testObjYValue.Value());
        }
        #endregion
        #region InverseF() Tests
        /// <summary> Tests that with two coordinates, No Interpolation it returns an IOrdinate object. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_InverseFNoInterpolation_TwoCoordinates_Returns_IOrdinate()
        {
            List<double> xs = new List<double>() { 0, 1 };
            List<double> ys = new List<double>() { 10, 11 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys);
            var testObjXValue = testObj.InverseF(new Constant(10));
            Assert.True(0 == testObjXValue.Value());
        }
        /// <summary> Tests that with three coordinates, No Interpolation it returns an IOrdinate object. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_InverseFNoInterpolation_ThreeCoordinates_Returns_IOrdinate()
        {
            List<double> xs = new List<double>() { 0, 1, 2 };
            List<double> ys = new List<double>() { 10, 11, 12 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys);
            var testObjXValue = testObj.InverseF(new Constant(12));
            Assert.True(2 == testObjXValue.Value());
        }
        /// <summary> Tests that with two coordinates, Linear Interpolation it returns an IOrdinate object. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_InverseFLinearInterpolation_TwoCoordinates_Returns_IOrdinate()
        {
            List<double> xs = new List<double>() { 0, 1, 2 };
            List<double> ys = new List<double>() { 10, 11, 12 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys, InterpolationEnum.Linear);
            var testObjXValue = testObj.InverseF(new Constant(10.5));

            Assert.True(0.5 == testObjXValue.Value());
        }
        /// <summary> Tests that with two coordinates, Piecewise Interpolation it returns an IOrdinate object. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_InverseFPiecewiseInterpolation_TwoCoordinates_Returns_IOrdiante()
        {
            List<double> xs = new List<double>() { 0, 1, 2 };
            List<double> ys = new List<double>() { 10, 11, 12 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys, InterpolationEnum.Piecewise);
            var testObjXValue = testObj.InverseF(new Constant(10.3));
            Assert.True(0 == testObjXValue.Value());
        }
        #endregion
        #region Compose() Tests

        #region Tests Using Default Data
        /// <summary>
        /// Tests that a function composed with itself will return the same thing as itself
        /// </summary>
        [Fact]
        public void CoordinatesFunctionConstants_Compose_BaseToBase_Returns_IFunction()
        {
            IFunction base1 = CreateCoordinatesFunctionConstants(TestData.DefaultFunctionData.BaseXs_1_10, TestData.DefaultFunctionData.BaseYs_1_10);
            IFunction base2 = CreateCoordinatesFunctionConstants(TestData.DefaultFunctionData.BaseXs_1_10, TestData.DefaultFunctionData.BaseYs_1_10);
            IFunction baseComposed = base1.Compose(base2);
            Assert.True(baseComposed.Equals(base2));
            Assert.True(baseComposed.Equals(base1));

        }

        /// <summary>
        /// Tests that the base function composed with a function completely above it returns an invalid operation exception
        /// </summary>
        [Fact]
        public void CoordinatesFunctionConstants_Compose_BaseToAboveBase_Returns_InvalidOperationException()
        {
            IFunction func1 = CreateCoordinatesFunctionConstants(TestData.DefaultFunctionData.BaseXs_1_10, TestData.DefaultFunctionData.BaseYs_1_10);
            IFunction func2 = CreateCoordinatesFunctionConstants(TestData.DefaultFunctionData.AboveBaseXs_20_29, TestData.DefaultFunctionData.AboveBaseYs_20_29);
            Assert.Throws<InvalidOperationException>(() => func1.Compose(func2));
        }

        /// <summary>
        /// Tests that the base function composed with a function completely below it returns an invalid operation exception
        /// </summary>
        [Fact]
        public void CoordinatesFunctionConstants_Compose_BaseToBelowBase_Returns_InvalidOperationException()
        {
            IFunction func1 = CreateCoordinatesFunctionConstants(TestData.DefaultFunctionData.BaseXs_1_10, TestData.DefaultFunctionData.BaseYs_1_10);
            IFunction func2 = CreateCoordinatesFunctionConstants(TestData.DefaultFunctionData.BelowBaseXs_Neg29_Neg20, TestData.DefaultFunctionData.BelowBaseYs_Neg29_Neg20);
            Assert.Throws<InvalidOperationException>(() => func1.Compose(func2));
        }

        /// <summary>
        /// Tests that the base function composed with a function that is below it but has some overlapping range will pick up points from both functions.
        /// </summary>
        [Fact]
        public void CoordinatesFunctionConstants_Compose_BaseToBelowBaseWithOverlap_Returns_IFunction()
        {
            IFunction func1 = CreateCoordinatesFunctionConstants(TestData.DefaultFunctionData.BaseXs_1_10, TestData.DefaultFunctionData.BaseYs_1_10, InterpolationEnum.Linear);
            IFunction func2 = CreateCoordinatesFunctionConstants(TestData.DefaultFunctionData.BelowAndOverlappingBaseXs_Neg3_5, TestData.DefaultFunctionData.BelowAndOverlappingBaseYs_Neg3_5, InterpolationEnum.Linear);
            IFunction expectedFunc = CreateCoordinatesFunctionConstants(new List<double>() { 1, 5 }, new List<double>() { 1, 5 }, InterpolationEnum.Linear);
            IFunction composedFunc = func1.Compose(func2);
            Assert.True(composedFunc.Equals(expectedFunc));

        }

        [Fact]
        public void CoordinatesFunctionConstants_Compose_BaseToBelowBaseWithOverlap_Returns_IFunction2()
        {
            IFunction func1 = CreateCoordinatesFunctionConstants(new List<double>() { 1, 3 }, new List<double>() { 1, 3 }, InterpolationEnum.Linear);
            IFunction func2 = CreateCoordinatesFunctionConstants(new List<double>() { 0, 2 }, new List<double>() { 0, 2 }, InterpolationEnum.Linear);
            IFunction expectedFunc = CreateCoordinatesFunctionConstants(new List<double>() { 1, 2 }, new List<double>() { 1, 2 }, InterpolationEnum.Linear);
            IFunction composedFunc = func2.Compose(func1);
            Assert.True(composedFunc.Equals(expectedFunc));

        }

        /// <summary>
        /// Tests that the base function composed with a function that is above it but has some overlapping range will pick up points from both functions.
        /// </summary>
        [Fact]
        public void CoordinatesFunctionConstants_Compose_BaseToAboveBaseWithOverlap_Returns_IFunction()
        {
            IFunction func1 = CreateCoordinatesFunctionConstants(TestData.DefaultFunctionData.BaseXs_1_10, TestData.DefaultFunctionData.BaseYs_1_10, InterpolationEnum.Linear);
            IFunction func2 = CreateCoordinatesFunctionConstants(TestData.DefaultFunctionData.AboveAndOverlappingBaseXs_7_15, TestData.DefaultFunctionData.AboveAndOverlappingBaseYs_7_15, InterpolationEnum.Linear);
            IFunction expectedFunc = CreateCoordinatesFunctionConstants(new List<double>() { 7, 10 }, new List<double>() { 7, 10 }, InterpolationEnum.Linear);
            IFunction composedFunc = func1.Compose(func2);
            Assert.True(composedFunc.Equals(expectedFunc));

        }

        /// <summary>
        /// Tests that the base function composed with a function that is inside the base will get the points from the inside function.
        /// </summary>
        [Fact]
        public void CoordinatesFunctionConstants_Compose_BaseToInsideBase_Returns_IFunction()
        {
            IFunction func1 = CreateCoordinatesFunctionConstants(TestData.DefaultFunctionData.BaseXs_1_10, TestData.DefaultFunctionData.BaseYs_1_10, InterpolationEnum.Linear);
            IFunction func2 = CreateCoordinatesFunctionConstants(TestData.DefaultFunctionData.InsideBaseXs_3_7, TestData.DefaultFunctionData.InsideBaseYs_3_7, InterpolationEnum.Linear);
            IFunction expectedFunc = CreateCoordinatesFunctionConstants(new List<double>() { 3, 7 }, new List<double>() { 3, 7 }, InterpolationEnum.Linear);
            IFunction composedFunc = func1.Compose(func2);
            Assert.True(composedFunc.Equals(expectedFunc));

        }

        /// <summary>
        /// Tests that the base function composed with a function that completely contains the base will get the points from the base function.
        /// </summary>
        [Fact]
        public void CoordinatesFunctionConstants_Compose_BaseToContainsBase_Returns_IFunction()
        {
            IFunction func1 = CreateCoordinatesFunctionConstants(TestData.DefaultFunctionData.BaseXs_1_10, TestData.DefaultFunctionData.BaseYs_1_10, InterpolationEnum.Linear);
            IFunction func2 = CreateCoordinatesFunctionConstants(TestData.DefaultFunctionData.ContainsBaseXs_Neg3_13, TestData.DefaultFunctionData.ContainsBaseYs_Neg3_13, InterpolationEnum.Linear);
            IFunction expectedFunc = CreateCoordinatesFunctionConstants(new List<double>() { 1, 10 }, new List<double>() { 1, 10 }, InterpolationEnum.Linear);
            IFunction composedFunc = func1.Compose(func2);
            Assert.True(composedFunc.Equals(expectedFunc));

        }
        #endregion
        /// <summary>
        /// Uses the same values for the xs as the ys. Uses linear interpolation.
        /// </summary>
        /// <param name="func1Vals">An array of doubles that will be used for function1's xvalues and yvalues</param>
        /// <param name="func2Vals">An array of doubles that will be used for function2's xvalues and yvalues</param>
        /// <param name="expectedResult">An array of doubles that will be used for the expected function's xvalues and yvalues</param>
        [Theory]
        [InlineData(new double[] { 1 }, new double[] {1 }, new double[] { 1})]
        [InlineData(new double[] { 1, 3,5,7 }, new double[] { 0,2,4,6 }, new double[] { 1, 2,3,4,5,6 })]
        [InlineData(new double[] { 1, 2 }, new double[] { 0, 3 }, new double[] { 1, 2 })]
        [InlineData(new double[] { 0, 3 }, new double[] { 1, 2 }, new double[] { 1, 2 })]
        [InlineData(new double[] { -1, 1 }, new double[] { 0, 2 }, new double[] { 0, 1 })]
        [InlineData(new double[]{1, 2}, new double[]{ 1, 2 }, new double[] { 1, 2 })]
        public void CoordinatesFunctionConstants_Compose_SimpleIntegers_Returns_IFunction(double[] func1Vals, double[] func2Vals, double[] expectedResult)
        {
            IFunction func1 = CreateCoordinatesFunctionConstants(func1Vals.ToList(), func1Vals.ToList(), InterpolationEnum.Linear);
            IFunction func2 = CreateCoordinatesFunctionConstants(func2Vals.ToList(), func2Vals.ToList(), InterpolationEnum.Linear);
            IFunction expectedFunc = CreateCoordinatesFunctionConstants(expectedResult.ToList(), expectedResult.ToList(), InterpolationEnum.Linear);
            IFunction composedFunc = func1.Compose(func2);
            Assert.True(composedFunc.Equals(expectedFunc));

        }

        /// <summary> Tests that with basic input, No Interpolation it returns an IFunction. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_Compose_SameCoordinates_Returns_IFunction()
        {
            List<double> xs = new List<double>() { 0, 1, 2 };
            List<double> ys = new List<double>() { 0, 1, 2 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys);
            IFunction testObj2 = CreateCoordinatesFunctionConstants(xs, ys);
            IFunction fog = testObj.Compose(testObj2);
            Assert.True(fog.Equals(testObj));
        }

        [Fact]
        public void CoordinatesFunctionConstants_Compose_LP3AndRating_Returns_IFunction()
        {
            List<double> LP3xs = new List<double>() { 0, .5, 1 };
            List<double> LP3ys = new List<double>() { 0, 10000, 100000 };


            List<double> ratFlows = new List<double>() { 0, 100, 10000, 100000 };
            List<double> ratStages = new List<double>() { 0, 1, 10, 100 };

            IFunction lp3CoordFunc = (IFunction)ICoordinatesFunctionsFactory.Factory(LP3xs, LP3ys, InterpolationEnum.Linear);
            IFunction ratCoordFunc = (IFunction)ICoordinatesFunctionsFactory.Factory(ratFlows, ratStages, InterpolationEnum.Linear);

            IFunction fog = lp3CoordFunc.Compose(ratCoordFunc);
            int test = 0;
            //Assert.True(fog.Equals(testObj));
        }
       

        /// <summary> Tests that with basic input, Linear Interpolation it returns an IFunction. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_Compose_LinearDifferentCoordinates_Returns_Function()
        {
            List<double> xs1 = new List<double>() { 0, 3, 5 };
            List<double> ys1 = new List<double>() { 2, 4, 7 };

            List<double> xs2 = new List<double>() { 1, 3, 6 };
            List<double> ys2 = new List<double>() { 4, 2, 6 };

            List<double> xs3 = new List<double>() { 0, 1.5, 3, 4.333333333333333 };
            List<double> ys3 = new List<double>() { 3, 2, 3.333333333333333, 6 };


            IFunction testObj = CreateCoordinatesFunctionConstants(xs1, ys1, InterpolationEnum.Linear);
            IFunction testObj2 = CreateCoordinatesFunctionConstants(xs2, ys2, InterpolationEnum.Linear);
            IFunction fog = testObj.Compose(testObj2);
            IFunction testObj3 = CreateCoordinatesFunctionConstants(xs3, ys3, InterpolationEnum.Linear);
            Assert.True(fog.Equals(testObj3));
        }

     
        /// <summary> Tests that with basic input, No Interpolation it throws an <see cref="ArgumentException"/>. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_Compose_DifferentCoordinates_Throw_ArgumentException()
        {
            List<double> xs1 = new List<double>() { 0, 3, 5 };
            List<double> ys1 = new List<double>() { 2, 4, 7 };

            List<double> xs2 = new List<double>() { 1, 3, 6 };
            List<double> ys2 = new List<double>() { 4, 2, 6 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs1, ys1);
            IFunction testObj2 = CreateCoordinatesFunctionConstants(xs2, ys2);
            Assert.Throws<ArgumentException>(() => testObj.Compose(testObj2));
        }
        /// <summary> Tests that with non overlapping input, No Interpolation it throws an <see cref="InvalidOperationException"/>. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_BadCompose_Throws_InvalidOperationException()
        {
            List<double> xs1 = new List<double>() { 0, 1, 2 };
            List<double> ys1 = new List<double>() { 0, 1, 2 };

            List<double> xs2 = new List<double>() { 3, 4, 5 };
            List<double> ys2 = new List<double>() { 3, 4, 5 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs1, ys1);
            IFunction testObj2 = CreateCoordinatesFunctionConstants(xs2, ys2);

            Assert.Throws<InvalidOperationException>(() => testObj.Compose(testObj2));
        }

        /// <summary> Tests that with zero input, No Interpolation it throws an <see cref="InvalidConstructorArgumentsException"/>. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_BadCompose_NoXsOrYs_Throws_InvalidConstructorArgumentsException()
        {
            //this is not really a compose test, but i wanted to make sure that a coordinates function with zero coordinates could not be created.
            List<double> xs1 = new List<double>();
            List<double> ys1 = new List<double>();

            List<double> xs2 = new List<double>();
            List<double> ys2 = new List<double>();  

            Assert.Throws<InvalidConstructorArgumentsException>(() => CreateCoordinatesFunctionConstants(xs1, ys1));
        }


        #endregion
        #region RiemannSum() Tests
        /// <summary> Tests that RiemannSum() will return the expected double. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_RiemannSum_Returns_Double()
        {
            List<double> xs1 = new List<double>() { 0, 1 };
            List<double> ys1 = new List<double>() { 0, 1 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs1, ys1);
            Assert.True(testObj.TrapizoidalRiemannSum() == 0.5);
        }
        #endregion
        #region Equals() Tests

        #endregion

        #region
        ///// <summary> Tests that sampling a CoordinatesFunctionConstants with no interpolator argument returns itself
        //[Fact]
        //public void CoordinatesFunctionConstants_Sample_Returns_ICoordinatesFunction()
        //{
        //    List<double> xs1 = new List<double>() { 0, 3, 5 };
        //    List<double> ys1 = new List<double>() { 2, 4, 7 };

        //    IFunction testObj = CreateCoordinatesFunctionConstants(xs1, ys1);

        //    Assert.True(testObj == testObj.Sample(.1));
        //}

        ///// <summary> Tests that sampling a CoordinatesFunctionConstants with an interpolator returns 
        ///// an identical function but with the new interpolator set.
        //[Fact]
        //public void CoordinatesFunctionConstants_Sample_Returns_ICoordinatesFunction2()
        //{
        //    List<double> xs1 = new List<double>() { 0, 3, 5 };
        //    List<double> ys1 = new List<double>() { 2, 4, 7 };

        //    IFunction testObj = CreateCoordinatesFunctionConstants(xs1, ys1);

        //    ICoordinatesFunction sampledFunction = testObj.Sample(.1, InterpolationEnum.Piecewise);
        //    Assert.True(sampledFunction.Interpolator == InterpolationEnum.Piecewise);
        //    Assert.True(AreCoordinatesEqual(testObj.Coordinates, sampledFunction.Coordinates));
        //}
        #endregion

    }
}
