using Functions;
using Functions.Coordinates;
using Functions.CoordinatesFunctions;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace FunctionsTests.CoordinatesFunctions
{
    [ExcludeFromCodeCoverage]
    public class CoordinatesFunctionConstantsTests:CoordinateFunctionsTestData
    {

        //#region Test Delegate Functions

        //public static ICoordinate<IOrdinate, IOrdinate> Square(double val) => (ICoordinate<IOrdinate, IOrdinate>)new CoordinateConstants(val, val * val);
        //private static readonly Func<double, ICoordinate<IOrdinate, IOrdinate>> map = Square;
        //#endregion

        #region Good Constructor Data Tests

        /// <summary> Tests that with good input a CoordinatesFunctionConstants object is constructed. </summary>
        [Theory]
        [MemberData(nameof( GoodData_Constant))]
        public void CoordinatesFunctionConstants_GoodInputNoInterpolation_Returns_CoordinatesFunctionConstants(List<ICoordinate<double, double>> value)
        {
            IFunction testObj = new CoordinatesFunctionConstants(value);
            Assert.True(true);
        }
        /// <summary> Tests that with good input a CoordinatesFunctionConstants object is constructed. </summary>
        [Theory]
        [MemberData(nameof(GoodData_Constant))]
        public void CoordinatesFunctionConstants_GoodLinearInterpolation_Returns_CoordinatesFunctionConstants(List<ICoordinate<double, double>> value)
        {
            IFunction testObj = new CoordinatesFunctionConstants(value, InterpolationEnum.Linear);
            Assert.True(true);
        }
        ///// <summary> Tests that with good input a CoordinatesFunctionConstants object is constructed. </summary>
        [Theory]
        [MemberData(nameof(GoodData_Constant))]
        public void CoordinatesFunctionConstants_GoodPiecewiseInterpolation_Returns_CoordinatesFunctionConstants(List<ICoordinate<double, double>> value)
        {
            IFunction testObj = new CoordinatesFunctionConstants(value, InterpolationEnum.Piecewise);
            Assert.True(true);
        }
        #endregion

        #region Bad Constructor Data Tests
        ///// <summary> Tests that with bad input it throws an <see cref="ArgumentException"/>. </summary>
        [Theory]
        [MemberData(nameof(BadData_Constant_Nan))]
        public void CoordinatesFunctionConstants_BadInput_NAN_Throws_ArgumentException(List<ICoordinate<double, double>> value)
        {
            Assert.Throws<Exception>(() => new CoordinatesFunctionConstants(value));
        }

        ///// <summary> Tests that with bad input it throws an <see cref="ArgumentException"/>. </summary>
        [Theory]
        [MemberData(nameof(BadData_Constant_NegativeInfinity))]
        public void CoordinatesFunctionConstants_BadInput_NegativeInfinity_Throws_ArgumentException(List<ICoordinate<double, double>> value)
        {
            Assert.Throws<Exception>(() => new CoordinatesFunctionConstants(value));
        }

        ///// <summary> Tests that with bad input it throws an <see cref="ArgumentException"/>. </summary>
        [Theory]
        [MemberData(nameof(BadData_Constant_PositiveInfinity))]
        public void CoordinatesFunctionConstants_BadInput_PositiveInfinity_Throws_ArgumentException(List<ICoordinate<double, double>> value)
        {
            Assert.Throws<Exception>(() => new CoordinatesFunctionConstants(value));
        }

        ///// <summary> Tests that with bad input it throws an <see cref="ArgumentException"/>. </summary>
        [Theory]
        [MemberData(nameof(BadData_Constant_RepeatXs))]
        public void CoordinatesFunctionConstants_BadInput_RepeatXs_Throws_ArgumentException(List<ICoordinate<double, double>> value)
        {
            Assert.Throws<ArgumentException>(() => new CoordinatesFunctionConstants(value));
        }
        #endregion

        #region Property Tests
        #region Range Property Tests 

        #endregion
        #region Domain Property Tests 

        #endregion
        #region Interpolation Property Tests
        /// <summary> Tests that with No Interpolation it returns a NoInterpolation enum. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_NoInterpolation_Returns_NoInterpolation()
        {
            IFunction testObj = CreateCoordinatesFunctionConstantsBasic();
            Assert.Equal(InterpolationEnum.NoInterpolation, testObj.Interpolator);
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
        #endregion

        //#region Function Tests
        #region F() Tests
        /// <summary> Tests that with zero input, No Interpolation it returns an IScalar object. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_FNoInterpolation_Returns_YValue()
        {
            List<double> xs = new List<double>() { 0, 1 };
            List<double> ys = new List<double>() { 10, 11 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys);
            var testObjYValue = testObj.F(0);
            Assert.True(10 == testObjYValue);
        }
        /// <summary> Tests that with zero input, No Interpolation it returns an IScalar object. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_FNoInterpolation_Returns_YValue_2()
        {
            List<double> xs = new List<double>() { 0, 1 };
            List<double> ys = new List<double>() { 10, 11 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys);
            var testObjYValue = testObj.F(1);
            Assert.True(11 == testObjYValue);
        }
        /// <summary> Tests that with No Interpolation F() will throw an <see cref="ArgumentOutOfRangeException"/>. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_FNoInterpolation_Throws_ArgumentOutOfRangeException()
        {
            List<double> xs = new List<double>() { 0, 1 };
            List<double> ys = new List<double>() { 0, 1 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys);
            Assert.Throws<ArgumentOutOfRangeException>(() => testObj.F(2));
        }
        /// <summary> Tests that with no interpolation F() will throw an <see cref="InvalidOperationException"/>. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_FNoInterpolation_Throws_InvalidOperationException()
        {
                List<double> xs = new List<double>() { 0, 1 };
                List<double> ys = new List<double>() { 10, 11 };

                IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys);
                Assert.Throws<InvalidOperationException>(()=> testObj.F(.5));
        }
        /// <summary> Tests that with no interpolation F() will throw an <see cref="ArgumentOutOfRangeException"/>. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_FLinearInterpolation_Throws_ArgumentOutOfRangeException()
        {
            List<double> xs = new List<double>() { 0, 1 };
            List<double> ys = new List<double>() { 0, 1 };
            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys, InterpolationEnum.Linear);
            Assert.Throws<ArgumentOutOfRangeException>(() => testObj.F(5));
        }
        /// <summary> Tests that with linear interpolation F() will return an IScalar object. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_FLinearInterpolation_Returns_IScalar()
        {
            List<double> xs = new List<double>() { 0, 1 };
            List<double> ys = new List<double>() { 10, 11 };
            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys, InterpolationEnum.Linear);
            var testObjYValue = testObj.F(.5);
            Assert.True( 10.5 == testObjYValue);
        }
        /// <summary> Tests that with piecewise interpolation F() will return an IScalar object with value rounded up to 1. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_FPiecewiseInterpolation_HigherBound_Returns_IScalar()
        {
            List<double> xs = new List<double>() { 0, 1 };
            List<double> ys = new List<double>() { 0, 1 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys, InterpolationEnum.Piecewise);
            var testObjYValue = testObj.F(.5);
            Assert.True(1 == testObjYValue);
        }
        /// <summary> Tests that with linear interpolation F() will return an IScalar object with value rounded down to 0. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_FPiecewiseInterpolation_LowerBound_Returns_IScalar()
        {
            List<double> xs = new List<double>() { 0, 1 };
            List<double> ys = new List<double>() { 0, 1 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys, InterpolationEnum.Piecewise);
            var testObjYValue = testObj.F(.4);
            Assert.True(0 == testObjYValue);
        }
        #endregion
        #region InverseF() Tests
        /// <summary> Tests that with two coordinates, No Interpolation it returns an IScalar object. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_InverseFNoInterpolation_TwoCoordinates_Returns_IScalar()
        {
            List<double> xs = new List<double>() { 0, 1 };
            List<double> ys = new List<double>() { 10, 11 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys);
            var testObjXValue = testObj.InverseF(10);
            Assert.True(0 == testObjXValue);
        }
        /// <summary> Tests that with three coordinates, No Interpolation it returns an IScalar object. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_InverseFNoInterpolation_ThreeCoordinates_Returns_IScalar()
        {
            List<double> xs = new List<double>() { 0, 1, 2 };
            List<double> ys = new List<double>() { 10, 11, 12 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys);
            var testObjXValue = testObj.InverseF(12);
            Assert.True(2 == testObjXValue);
        }
        /// <summary> Tests that with two coordinates, Linear Interpolation it returns an IScalar object. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_InverseFLinearInterpolation_TwoCoordinates_Returns_IScalar()
        {
            List<double> xs = new List<double>() { 0, 1, 2 };
            List<double> ys = new List<double>() { 10, 11, 12 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys, InterpolationEnum.Linear);
            var testObjXValue = testObj.InverseF(10.5);
           
            Assert.True(0.5 == testObjXValue);
        }
        /// <summary> Tests that with two coordinates, Piecewise Interpolation it returns an IScalar object. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_InverseFPiecewiseInterpolation_TwoCoordinates_Returns_IScalar()
        {
            List<double> xs = new List<double>() { 0, 1, 2 };
            List<double> ys = new List<double>() { 10, 11, 12 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys, InterpolationEnum.Piecewise);
            var testObjXValue = testObj.InverseF(10.3);
            Assert.True( 0 == testObjXValue);
        }
        #endregion
        #region Compose() Tests
        /// <summary> Tests that with basic input, No Interpolation it returns an IUnivariateFunction. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_Compose_SameCoordinates_Returns_Function()
        {
            List<double> xs = new List<double>() { 0, 1, 2 };
            List<double> ys = new List<double>() { 0, 1, 2 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs, ys);
            IFunction testObj2 = CreateCoordinatesFunctionConstants(xs, ys);
            IFunction fog = testObj.Compose(testObj2);
            Assert.True(fog.Equals(testObj));
        }
        /// <summary> Tests that with basic input, Linear Interpolation it returns an IUnivariateFunction. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_Compose_LinearDifferentCoordinates_Returns_Function()
        {
            List<double> xs1 = new List<double>() { 0, 3, 5 };
            List<double> ys1 = new List<double>() { 2, 4, 7 };

            List<double> xs2 = new List<double>() { 1, 3, 6 };
            List<double> ys2 = new List<double>() { 4, 2, 6 };

            List<double> xs3 = new List<double>() { 0, 1.5, 3 };
            List<double> ys3 = new List<double>() { 3, 2, 3.333333333333333 };


            IFunction testObj = CreateCoordinatesFunctionConstants(xs1, ys1, InterpolationEnum.Linear);
            IFunction testObj2 = CreateCoordinatesFunctionConstants(xs2,ys2, InterpolationEnum.Linear);
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
        /// <summary> Tests that with zero input, No Interpolation it throws an <see cref="InvalidOperationException"/>. </summary>
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

        #endregion
        #region RiemannSum() Tests
        /// <summary> Tests that RiemannSum() will return the expected double. </summary>
        [Fact]
        public void CoordinatesFunctionConstants_RiemannSum_Returns_Double()
        {
            List<double> xs1 = new List<double>() { 0, 1 };
            List<double> ys1 = new List<double>() { 0, 1 };

            IFunction testObj = CreateCoordinatesFunctionConstants(xs1, ys1);
            Assert.True(testObj.RiemannSum() == 0.5);
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

        //    ICoordinatesFunction<double, double> sampledFunction = testObj.Sample(.1, InterpolationEnum.Piecewise);
        //    Assert.True(sampledFunction.Interpolator == InterpolationEnum.Piecewise);
        //    Assert.True(AreCoordinatesEqual(testObj.Coordinates, sampledFunction.Coordinates));
        //}
        #endregion

    }
}
