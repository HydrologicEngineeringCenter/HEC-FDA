using Functions;
using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FunctionsTests.CoordinatesFunctions
{
    [ExcludeFromCodeCoverage]
    public class CoordinatesFunctionVariableYsTests : CoordinateFunctionsTestData
    {
        private readonly ITestOutputHelper output;

        public CoordinatesFunctionVariableYsTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        #region Test Delegate Functions
        //public static ICoordinate Square(double val) => new UnivariateCoordinate(IScalarFactory.Factory(val), IScalarFactory.Factory(val * val));
        //private static readonly Func<double, ICoordinate> map = Square;
        #endregion

        #region Good Constructor Data Tests
        /// <summary> Constructs Good Input Data for Good Constructor Data Tests. </summary>
        //public static TheoryData<List<ICoordinate>> GoodDataNotDistributed =>
        //    new TheoryData<List<ICoordinate>>
        //    {
        //        { new List<ICoordinate>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map)) },
        //        { new List<ICoordinate>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map)) },
        //        { new List<ICoordinate>(new UnivariateCoordinate(IScalarFactory.Factory(Double.MaxValue), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(Double.MinValue), map)) },
        //        { new List<ICoordinate>(new UnivariateCoordinate(IScalarFactory.Factory(Double.MinValue), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(Double.MaxValue), map)) },
        //        { new List<ICoordinate>(new UnivariateCoordinate(IScalarFactory.Factory(Double.MaxValue), IScalarFactory.Factory(0), null), new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(Double.MinValue), null)) },
        //        { new List<ICoordinate>(new UnivariateCoordinate(IScalarFactory.Factory(Double.MinValue), IScalarFactory.Factory(0), null), new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(Double.MaxValue), null)) }
        //    };

        //public static TheoryData<List<ICoordinate>> GoodDataDistributed =>
        //    new TheoryData<List<ICoordinate>>
        //    {
        //        { new List<ICoordinate>(new UnivariateCoordinate(new ScalarHistogram(), new ScalarHistogram(), map)) },
        //        { new List<ICoordinate>(new UnivariateCoordinate(new ScalarDistributed(new Normal()), new ScalarDistributed(new Normal()), map)) },
        //        { new List<ICoordinate>(new UnivariateCoordinate(new ScalarHistogram(), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarHistogram(), map)) },
        //        { new List<ICoordinate>(new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarHistogram(), map), new UnivariateCoordinate(new ScalarHistogram(), IScalarFactory.Factory(0), map)) },
        //        { new List<ICoordinate>(new UnivariateCoordinate(new ScalarDistributed(new Normal()), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarDistributed(new Normal()), map)) },

        //    };

        /// <summary> Tests that for Good input it constructs the object without exception. </summary>
        [Theory]
        [MemberData(nameof(GoodDataDistributed))]
        public void CoordinatesFunctionVariableYs_GoodInput_Returns_CoordinatesFunctionVariableYs(List<ICoordinate> value)
        {
            ICoordinatesFunction testObj = new CoordinatesFunctionVariableYs(value);
            Assert.True(true);
        }
        #endregion

        #region Bad Constructor Data Tests
        /// <summary> Tests that for null input it throws an ArgumentException. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_NullInput_Throws_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new CoordinatesFunctionVariableYs(null));
        }
        /// <summary> Tests that for bad distributed input it throws an ArgumentException. </summary>
        [Theory]
        [MemberData(nameof(BadDataDistributed_RepeatXs))]
        public void CoordinatesFunctionVariableYs_BadDistributedInput_Throws_ArgumentException(List<ICoordinate> value)
        {
          
                Assert.Throws<ArgumentException>(()=> new CoordinatesFunctionVariableYs(value));
           
        }
        #endregion

        #region Property Tests
        #region IsInvertible Property Tests // from UnivariantCoordinatesFunction
        /// <summary> Tests that for the IsInvertible Property it returns true. </summary>
        [Theory]
        [MemberData(nameof(GoodDataDistributed))]
        public void CoordinatesFunctionVariableYs_IsInvertible(List<ICoordinate> value)
        {
            CoordinatesFunctionVariableYs testObj = new CoordinatesFunctionVariableYs(value);
            Assert.True(testObj.IsInvertible);
        }
        #endregion
        #region Coordinates Property Tests // from UnivariantCoordinatesFunction
        /// <summary> Tests that the coordinates passed in are the same ones that get returned by the Coordinates property. </summary>
        [Theory]
        [MemberData(nameof(GoodDataDistributed))]
        public void CoordinatesFunctionVariableYs_Coordinates_Returns_UnivariantCoordinatesFunction(List<ICoordinate> value)
        {
            List<ICoordinate> coords = value;
            ICoordinatesFunction func = new CoordinatesFunctionVariableYs(value);
            Assert.True(coords.SequenceEqual(func.Coordinates));
        }
        #endregion
        #region IsDistributedXs Property Tests
        /// <summary> Tests that the IsDistributedXs Property returns False. </summary>
        //[Theory]
        //[MemberData(nameof(GoodDataDistributed))]
        //public void CoordinatesFunctionVariableYs_IsDistributedXs_Returns_True(List<ICoordinate> value)
        //{
        //    ICoordinatesFunction testObj = new CoordinatesFunctionVariableYs(value);
        //    Assert.False(((CoordinatesFunctionVariableYs)testObj).IsDistributedXs);
        //}

        #endregion
        #region IsDistributedYs Property Tests
        /// <summary> Tests that the IsDistributedYs Property returns True. </summary>
        [Theory]
        [MemberData(nameof(GoodDataDistributed))]
        public void CoordinatesFunctionVariableYs_IsDistributedYs_Returns_True(List<ICoordinate> value)
        {
            ICoordinatesFunction testObj = new CoordinatesFunctionVariableYs(value);
            Assert.True(((CoordinatesFunctionVariableYs)testObj).IsDistributedYs);
        }
        #endregion
        #region Order Property Tests // No setters here yet
        ///// <summary></summary>
        //[Fact]
        //public void CoordinatesFunctionVariableYs_OrderProperty()
        //{
        //}
        #endregion
        #endregion

        #region Function Tests
        #region F() Tests
        /// <summary> Tests that the F Function returns True. </summary>
        [Theory]
        [MemberData(nameof(GoodDataDistributed))]
        public void CoordinatesFunctionVariableYs_NotDistributedF_Returns_IScalar(List<ICoordinate> value)
        {
            ICoordinate firstCoord = value[0];

            CoordinatesFunctionVariableYs func = new CoordinatesFunctionVariableYs(value);
            IOrdinate fOfX = func.F(firstCoord.X);
            Assert.True(fOfX.Equals(firstCoord.Y));

        }
        ///// <summary> Tests that the F Function throws an <see cref="ArgumentOutOfRangeException"/>. </summary>
        //[Theory]
        //[MemberData(nameof(GoodDataDistributed))]
        //public void CoordinatesFunctionVariableYs_NotDistributedF_Throws_ArgumentOutOfRangeException(List<ICoordinate> value)
        //{

        //    CoordinatesFunctionVariableYs func = CreateDistributedCoordinatesFunctionBasic();
        //    Assert.Throws<ArgumentOutOfRangeException>(() => func.F(new Constant(99)));

        //}
        ///// <summary> Tests that the F Function throws an <see cref="ArgumentNullException"/>. </summary>
        //[Theory]
        //[MemberData(nameof(GoodDataDistributed))]
        //public void CoordinatesFunctionVariableYs_NullF_Throws_ArgumentNullException(List<ICoordinate> value)
        //{
        //    CoordinatesFunctionVariableYs func = CreateDistributedCoordinatesFunctionBasic();
        //    Assert.Throws<ArgumentOutOfRangeException>(() => func.F(new Constant(double.NaN)));
        //}




        ///// <summary> Tests that the F Function for Histogram input throws an <see cref="ArgumentOutOfRangeException"/>. </summary>
        //[Fact]
        //public void CoordinatesFunctionVariableYs_FHistogram_Throws_ArgumentOutOfRangeException()
        //{
        //    try
        //    {
        //        // ICoordinatesFunction testObj = new CoordinatesFunctionVariableYs(new List<ICoordinate>(new UnivariateCoordinate(new ScalarHistogram(), new ScalarHistogram(), map)));
        //        //IOrdinate result = testObj.F(IScalarFactory.Factory(0));
        //        Assert.True(false);
        //    }
        //    catch (ArgumentOutOfRangeException e)
        //    {
        //        string m = e.Message;
        //        Assert.Equal("Specified argument was out of the range of valid values.\r\nParameter name: The specified x value was not found in any of the coordinates. Interpolation is not supported for coordinates with distributed x or y values", m);
        //    }
        //}

        /// <summary> Tests that the F Function for Distributed input throws an <see cref="ArgumentOutOfRangeException"/>. </summary>
        [Theory]
        [MemberData(nameof(GoodDataDistributed))]
        public void CoordinatesFunctionVariableYs_FDistributed_Throws_ArgumentOutOfRangeException(List<ICoordinate> value)
        {
            ICoordinatesFunction testObj = new CoordinatesFunctionVariableYs(value);
            Assert.Throws<ArgumentOutOfRangeException>(() => testObj.F(new Distribution(new Normal(1, 0))));
        }
        #endregion
        #region InverseF() Tests
        /// <summary> Tests that the Inverse F Function returns the correct IOrdinate. </summary>
        [Theory]
        [MemberData(nameof(GoodDataDistributed))]
        public void CoordinatesFunctionVariableYs_InverseF_Returns_IScalar(List<ICoordinate> value)
        {
            ICoordinate firstCoord = value[0];
            ICoordinatesFunction testObj = new CoordinatesFunctionVariableYs(value);
            IOrdinate result = testObj.InverseF(firstCoord.Y);
            Assert.True(firstCoord.X.Equals(result));
        }

        ///// <summary> Tests that the Inverse F Function throws ArgumentException if input is out of range. </summary>
        //[Theory]
        //[MemberData(nameof(GoodDataDistributed))]
        //public void CoordinatesFunctionVariableYs_InverseF_Throws_ArgumentException(List<ICoordinate> value)
        //{
        //    ICoordinatesFunction testObj = new CoordinatesFunctionVariableYs(value);
        //    Assert.Throws<ArgumentOutOfRangeException>(() => testObj.InverseF(new Distribution(new Distribution(new Normal(99, 0)))));
        //}
        /// <summary> Tests that the Inverse F Function returns ArgumentNullException if input is null. </summary>
        [Theory]
        [MemberData(nameof(GoodDataDistributed))]
        public void CoordinatesFunctionVariableYs_InverseFNull_Throws_ArgumentNullException(List<ICoordinate> value)
        {
           
                ICoordinatesFunction testObj = new CoordinatesFunctionVariableYs(value);
                Assert.Throws<ArgumentNullException>(()=> testObj.InverseF(null));
            
        }
       
        #endregion

        #region Sample

        ///// <summary> Tests that the Sample method returns the mean for .5 probability on Normal Distribution. </summary>
        //[Fact]
        //public void CoordinatesFunctionVariableYs_Sample_Returns_True_NormalDist1()
        //{
        //    List<ICoordinate> distCoords = DistributedCoordinates(new double[] { 0 }, new IDistribution[] { new Normal(1, 0) });
        //    ICoordinatesFunction function = new CoordinatesFunctionVariableYs(distCoords);
        //    ICoordinatesFunction<double, double> sampledFunction = function.Sample(.5);
        //    List<ICoordinate<double, double>> sampledCoords =  sampledFunction.Coordinates;
        //    ICoordinate<double, double> coord = sampledCoords[0];
        //    Assert.True(sampledCoords.Count == 1);
        //    Assert.True(coord.X == 0 && coord.Y == 1);

        //}
        ///// <summary> Tests that the Sample method returns the mean for .5 probability on Normal Distribution. </summary>
        //[Fact]
        //public void CoordinatesFunctionVariableYs_Sample_Returns_True_NormalDist2()
        //{
        //    List<ICoordinate> distCoords = DistributedCoordinates(new double[] { 0 }, new IDistribution[] { new Normal(2, 0) });
        //    ICoordinatesFunction function = new CoordinatesFunctionVariableYs(distCoords);
        //    ICoordinatesFunction<double, double> sampledFunction = function.Sample(.5);
        //    List<ICoordinate<double, double>> sampledCoords = sampledFunction.Coordinates;
        //    ICoordinate<double, double> coord = sampledCoords[0];
        //    Assert.True(sampledCoords.Count == 1);
        //    Assert.True(coord.X == 0 && coord.Y == 2);

        //}

        ///// <summary> Tests that the Sample method returns the most likely value for .5 probability for triangular distribution. </summary>
        //[Fact]
        //public void CoordinatesFunctionVariableYs_Sample_Returns_True_TriangularDist1()
        //{
        //    List<ICoordinate> distCoords = DistributedCoordinates(new double[] { 5 }, new IDistribution[] { new Triangular(10,20,30) });
        //    ICoordinatesFunction function = new CoordinatesFunctionVariableYs(distCoords);
        //    ICoordinatesFunction<double, double> sampledFunction = function.Sample(.5);
        //    List<ICoordinate<double, double>> sampledCoords = sampledFunction.Coordinates;
        //    ICoordinate<double, double> coord = sampledCoords[0];
        //    Assert.True(sampledCoords.Count == 1);
        //    Assert.True(coord.X == 5 && coord.Y == 20);

        //}

        ///// <summary> Tests that the Sample method returns the min value for 0.0 probability for triangular distribution. </summary>
        //[Fact]
        //public void CoordinatesFunctionVariableYs_Sample_Returns_True_TriangularDist2()
        //{
        //    List<ICoordinate> distCoords = DistributedCoordinates(new double[] { 5 }, new IDistribution[] { new Triangular(10, 20, 30) });
        //    ICoordinatesFunction function = new CoordinatesFunctionVariableYs(distCoords);
        //    ICoordinatesFunction<double, double> sampledFunction = function.Sample(0.0);
        //    List<ICoordinate<double, double>> sampledCoords = sampledFunction.Coordinates;
        //    ICoordinate<double, double> coord = sampledCoords[0];
        //    Assert.True(sampledCoords.Count == 1);
        //    Assert.True(coord.X == 5 && coord.Y == 10);

        //}

        ///// <summary> Tests that the Sample method returns the max value for 1.0 probability for triangular distribution. </summary>
        //[Fact]
        //public void CoordinatesFunctionVariableYs_Sample_Returns_True_TriangularDist3()
        //{
        //    List<ICoordinate> distCoords = DistributedCoordinates(new double[] { 5 }, new IDistribution[] { new Triangular(10, 20, 30) });
        //    ICoordinatesFunction function = new CoordinatesFunctionVariableYs(distCoords);
        //    ICoordinatesFunction<double, double> sampledFunction = function.Sample(1.0);
        //    List<ICoordinate<double, double>> sampledCoords = sampledFunction.Coordinates;
        //    ICoordinate<double, double> coord = sampledCoords[0];
        //    Assert.True(sampledCoords.Count == 1);
        //    Assert.True(coord.X == 5 && coord.Y == 30);

        //}

        ///// <summary> Tests that the Sample method returns the max value for 1.0 probability for Uniform distribution. </summary>
        //[Fact]
        //public void CoordinatesFunctionVariableYs_Sample_Returns_True_UniformDist1()
        //{
        //    List<ICoordinate> distCoords = DistributedCoordinates(new double[] { 5 }, new IDistribution[] { new Uniform(10, 30) });
        //    ICoordinatesFunction function = new CoordinatesFunctionVariableYs(distCoords);
        //    ICoordinatesFunction<double, double> sampledFunction = function.Sample(1.0);
        //    List<ICoordinate<double, double>> sampledCoords = sampledFunction.Coordinates;
        //    ICoordinate<double, double> coord = sampledCoords[0];
        //    Assert.True(sampledCoords.Count == 1);
        //    Assert.True(coord.X == 5 && coord.Y == 30);

        //}

        ///// <summary> Tests that the Sample method returns the min value for 0.0 probability for Uniform distribution. </summary>
        //[Fact]
        //public void CoordinatesFunctionVariableYs_Sample_Returns_True_UniformDist2()
        //{
        //    List<ICoordinate> distCoords = DistributedCoordinates(new double[] { 5 }, new IDistribution[] { new Uniform(10, 30) });
        //    ICoordinatesFunction function = new CoordinatesFunctionVariableYs(distCoords);
        //    ICoordinatesFunction<double, double> sampledFunction = function.Sample(0.0);
        //    List<ICoordinate<double, double>> sampledCoords = sampledFunction.Coordinates;
        //    ICoordinate<double, double> coord = sampledCoords[0];
        //    Assert.True(sampledCoords.Count == 1);
        //    Assert.True(coord.X == 5 && coord.Y == 10);

        //}

        ///// <summary> Tests that the Sample method returns the mean value for 0.5 probability for Uniform distribution. </summary>
        //[Fact]
        //public void CoordinatesFunctionVariableYs_Sample_Returns_True_UniformDist3()
        //{
        //    List<ICoordinate> distCoords = DistributedCoordinates(new double[] { 5 }, new IDistribution[] { new Uniform(10, 30) });
        //    ICoordinatesFunction function = new CoordinatesFunctionVariableYs(distCoords);
        //    ICoordinatesFunction<double, double> sampledFunction = function.Sample(0.5);
        //    List<ICoordinate<double, double>> sampledCoords = sampledFunction.Coordinates;
        //    ICoordinate<double, double> coord = sampledCoords[0];
        //    Assert.True(sampledCoords.Count == 1);
        //    Assert.True(coord.X == 5 && coord.Y == 20);
        //}

        ///// <summary> Tests that the Sample method returns the mean value for 0.5 probability for Uniform distribution. </summary>
        //[Fact]
        //public void CoordinatesFunctionVariableYs_SampleWithInterpolator_Returns_True_UniformDist3()
        //{
        //    List<ICoordinate> distCoords = DistributedCoordinates(new double[] { 5 }, new IDistribution[] { new Uniform(10, 30) });
        //    ICoordinatesFunction function = new CoordinatesFunctionVariableYs(distCoords);
        //    ICoordinatesFunction<double, double> sampledFunction = function.Sample(0.5);
        //    List<ICoordinate<double, double>> sampledCoords = sampledFunction.Coordinates;
        //    ICoordinate<double, double> coord = sampledCoords[0];
        //    Assert.True(sampledCoords.Count == 1);
        //    Assert.True(coord.X == 5 && coord.Y == 20);
        //}
        #endregion

        #endregion
    }
}
