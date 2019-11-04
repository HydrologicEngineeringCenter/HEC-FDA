using Functions;
using Functions.CoordinatesFunctions;
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
    public class CoordinatesFunctionVariableYsTests:CoordinateFunctionsTestData
    {
        private readonly ITestOutputHelper output;

        public CoordinatesFunctionVariableYsTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        #region Test Delegate Functions
        //public static ICoordinate<double, IDistribution> Square(double val) => new UnivariateCoordinate(IScalarFactory.Factory(val), IScalarFactory.Factory(val * val));
        //private static readonly Func<double, ICoordinate<double, IDistribution>> map = Square;
        #endregion

        #region Good Constructor Data Tests
        /// <summary> Constructs Good Input Data for Good Constructor Data Tests. </summary>
        //public static TheoryData<List<ICoordinate<double, IDistribution>>> GoodDataNotDistributed =>
        //    new TheoryData<List<ICoordinate<double, IDistribution>>>
        //    {
        //        { new List<ICoordinate<double, IDistribution>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map)) },
        //        { new List<ICoordinate<double, IDistribution>>(new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(0), map)) },
        //        { new List<ICoordinate<double, IDistribution>>(new UnivariateCoordinate(IScalarFactory.Factory(Double.MaxValue), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(Double.MinValue), map)) },
        //        { new List<ICoordinate<double, IDistribution>>(new UnivariateCoordinate(IScalarFactory.Factory(Double.MinValue), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(Double.MaxValue), map)) },
        //        { new List<ICoordinate<double, IDistribution>>(new UnivariateCoordinate(IScalarFactory.Factory(Double.MaxValue), IScalarFactory.Factory(0), null), new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(Double.MinValue), null)) },
        //        { new List<ICoordinate<double, IDistribution>>(new UnivariateCoordinate(IScalarFactory.Factory(Double.MinValue), IScalarFactory.Factory(0), null), new UnivariateCoordinate(IScalarFactory.Factory(0), IScalarFactory.Factory(Double.MaxValue), null)) }
        //    };

        //public static TheoryData<List<ICoordinate<double, IDistribution>>> GoodDataDistributed =>
        //    new TheoryData<List<ICoordinate<double, IDistribution>>>
        //    {
        //        { new List<ICoordinate<double, IDistribution>>(new UnivariateCoordinate(new ScalarHistogram(), new ScalarHistogram(), map)) },
        //        { new List<ICoordinate<double, IDistribution>>(new UnivariateCoordinate(new ScalarDistributed(new Normal()), new ScalarDistributed(new Normal()), map)) },
        //        { new List<ICoordinate<double, IDistribution>>(new UnivariateCoordinate(new ScalarHistogram(), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarHistogram(), map)) },
        //        { new List<ICoordinate<double, IDistribution>>(new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarHistogram(), map), new UnivariateCoordinate(new ScalarHistogram(), IScalarFactory.Factory(0), map)) },
        //        { new List<ICoordinate<double, IDistribution>>(new UnivariateCoordinate(new ScalarDistributed(new Normal()), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarDistributed(new Normal()), map)) },

        //    };

        /// <summary> Tests that for Good input it constructs the object without exception. </summary>
        [Theory]
        [MemberData(nameof(GoodDataDistributed))]
        public void CoordinatesFunctionVariableYs_GoodInput_Returns_CoordinatesFunctionVariableYs(List<ICoordinate<double, IDistribution>> value)
        {
            ICoordinatesFunction<double, IDistribution> testObj = new CoordinatesFunctionVariableYs(value);
            Assert.True(true);
        }
        #endregion

        #region Bad Constructor Data Tests
        /// <summary> Tests that for null input it throws an ArgumentException. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_NullInput_Throws_ArgumentException()
        {
            try
            {
                ICoordinatesFunction<double, IDistribution> testObj = new CoordinatesFunctionVariableYs(null);
                Assert.True(false);
            }
            catch (ArgumentException e)
            {
                string m = e.Message;
                Assert.Equal("The specified collection is invalid because it is empty or contains null values.", m);

            }
        }
        /// <summary> Tests that for bad distributed input it throws an ArgumentException. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_BadDistributedInput_Throws_ArgumentException()
        {
            try
            {
                //ICoordinatesFunction<double, IDistribution> testObj = new CoordinatesFunctionVariableYs(new List<ICoordinate<double, IDistribution>>(new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarDistributed(new Normal()), map), new UnivariateCoordinate(new ScalarDistributed(new Normal()), IScalarFactory.Factory(0), map)));
                Assert.True(false);
            }
            catch (ArgumentException e)
            {
                string m = e.Message;
                Assert.Equal("The specified set of coordinate is invalid. At least one x value maps to more than one y value (e.g. the set does not meet the definition of a function).", m);
            }
        }
        #endregion

       #region Property Tests
        #region IsInvertible Property Tests // from UnivariantCoordinatesFunction
        /// <summary> Tests that for the IsInvertible Property it returns true. </summary>
        [Theory]
        [MemberData(nameof(GoodDataDistributed))]
        public void CoordinatesFunctionVariableYs_IsInvertible(List<ICoordinate<double, IDistribution>> value)
        {
            CoordinatesFunctionVariableYs testObj = new CoordinatesFunctionVariableYs(value);
            Assert.True(testObj.IsInvertible);
        }
        #endregion
        #region Coordinates Property Tests // from UnivariantCoordinatesFunction
        /// <summary> Tests that the coordinates passed in are the same ones that get returned by the Coordinates property. </summary>
        [Theory]
        [MemberData(nameof(GoodDataDistributed))]
        public void CoordinatesFunctionVariableYs_Coordinates_Returns_UnivariantCoordinatesFunction(List<ICoordinate<double, IDistribution>> value)
        {
            List<ICoordinate<double, IDistribution>> coords = value;
            ICoordinatesFunction<double, IDistribution> func = new CoordinatesFunctionVariableYs(value);
            Assert.True(coords.SequenceEqual(func.Coordinates));
        }
        #endregion
        #region IsDistributedXs Property Tests
        /// <summary> Tests that the IsDistributedXs Property returns False. </summary>
        [Theory]
        [MemberData(nameof(GoodDataDistributed))]
        public void CoordinatesFunctionVariableYs_IsDistributedXs_Returns_True(List<ICoordinate<double, IDistribution>> value)
        {
            ICoordinatesFunction<double, IDistribution> testObj = new CoordinatesFunctionVariableYs(value);
            Assert.False(((CoordinatesFunctionVariableYs)testObj).IsDistributedXs);
        }

        #endregion
        #region IsDistributedYs Property Tests
        /// <summary> Tests that the IsDistributedYs Property returns True. </summary>
        [Theory]
        [MemberData(nameof(GoodDataDistributed))]
        public void CoordinatesFunctionVariableYs_IsDistributedYs_Returns_True(List<ICoordinate<double, IDistribution>> value)
        {
            ICoordinatesFunction<double, IDistribution> testObj = new CoordinatesFunctionVariableYs(value);
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
        public void CoordinatesFunctionVariableYs_NotDistributedF_Returns_IScalar(List<ICoordinate<double, IDistribution>> value)
        {
            CoordinatesFunctionVariableYs func = CreateDistributedCoordinatesFunctionBasic();
            Assert.True(func.F(0) == new Normal(1, 0));

            //ICoordinatesFunction<double, IDistribution> testObj = new CoordinatesFunctionVariableYs(value);
            //IOrdinate result = testObj.F(IScalarFactory.Factory(0));
            //Assert.True(result.Equals(IScalarFactory.Factory(0)) || result.Equals(IScalarFactory.Factory(Double.MaxValue)) || result.Equals(IScalarFactory.Factory(Double.MinValue)));
        }
        /// <summary> Tests that the F Function throws an <see cref="ArgumentOutOfRangeException"/>. </summary>
        [Theory]
        [MemberData(nameof(GoodDataDistributed))]
        public void CoordinatesFunctionVariableYs_NotDistributedF_Throws_ArgumentOutOfRangeException(List<ICoordinate<double, IDistribution>> value)
        {
           
                CoordinatesFunctionVariableYs func = CreateDistributedCoordinatesFunctionBasic();
                Assert.Throws<ArgumentOutOfRangeException>(()=> func.F(99));
             
        }
        /// <summary> Tests that the F Function throws an <see cref="ArgumentNullException"/>. </summary>
        [Theory]
        [MemberData(nameof(GoodDataDistributed))]
        public void CoordinatesFunctionVariableYs_NullF_Throws_ArgumentNullException(List<ICoordinate<double, IDistribution>> value)
        {
            CoordinatesFunctionVariableYs func = CreateDistributedCoordinatesFunctionBasic();
            Assert.Throws<ArgumentOutOfRangeException>(() => func.F(double.NaN));
        }
        /// <summary> Tests that the F Function returns an IScalar object. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_F_Returns_IScalar()
        {
            CoordinatesFunctionVariableYs func = CreateDistributedCoordinatesFunctionBasic();
            IDistribution result = func.F(0);
            Assert.Equal(new Normal(1,0), result);
        }
        /// <summary> Tests that the F Function returns a ScalarHistogram object. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_F_Returns_ScalarHistogram()
        {
            //ICoordinatesFunction<double, IDistribution> testObj = new CoordinatesFunctionVariableYs(new List<ICoordinate<double, IDistribution>>(new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarHistogram(), map), new UnivariateCoordinate(new ScalarHistogram(), IScalarFactory.Factory(0), map)));
            //IOrdinate result = testObj.F();
            //Assert.Equal("Histogram(Mean: 0, Standard Deviation: 0, Min: 0, Max: 0, Sample Size: 0, Is Converged: False)", result.Print());
        }
        /// <summary> Tests that the F Function returns a ScalarDistributed object. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_F_Returns_ScalarDistributed()
        {
            //ICoordinatesFunction<double, IDistribution> testObj = new CoordinatesFunctionVariableYs(new List<ICoordinate<double, IDistribution>>(new UnivariateCoordinate(new ScalarDistributed(new Normal()), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarDistributed(new Normal()), map)));
            //IOrdinate result = testObj.F(IScalarFactory.Factory(0));
            //Assert.Equal("Normal(Mean: 0, StandardDeviation: 1)", result.Print());
        }
        /// <summary> Tests that the F Function for Histogram input throws an <see cref="ArgumentOutOfRangeException"/>. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_FHistogram_Throws_ArgumentOutOfRangeException()
        {
            try
            {
               // ICoordinatesFunction<double, IDistribution> testObj = new CoordinatesFunctionVariableYs(new List<ICoordinate<double, IDistribution>>(new UnivariateCoordinate(new ScalarHistogram(), new ScalarHistogram(), map)));
                //IOrdinate result = testObj.F(IScalarFactory.Factory(0));
                Assert.True(false);
            }
            catch (ArgumentOutOfRangeException e)
            {
                string m = e.Message;
                Assert.Equal("Specified argument was out of the range of valid values.\r\nParameter name: The specified x value was not found in any of the coordinates. Interpolation is not supported for coordinates with distributed x or y values", m);
            }
        }
        /// <summary> Tests that the F Function for Distributed input throws an <see cref="ArgumentOutOfRangeException"/>. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_FDistributed_Throws_ArgumentOutOfRangeException()
        {
            try
            {
                //ICoordinatesFunction<double, IDistribution> testObj = new CoordinatesFunctionVariableYs(new List<ICoordinate<double, IDistribution>>(new UnivariateCoordinate(new ScalarDistributed(new Normal()), new ScalarDistributed(new Normal()), map)));
                //IOrdinate result = testObj.F(IScalarFactory.Factory(0));
                Assert.True(false);
            }
            catch (ArgumentOutOfRangeException e)
            {
                string m = e.Message;
                Assert.Equal("Specified argument was out of the range of valid values.\r\nParameter name: The specified x value was not found in any of the coordinates. Interpolation is not supported for coordinates with distributed x or y values", m);
            }
        }
        #endregion
        #region InverseF() Tests
        /// <summary> Tests that the Inverse F Function returns True. </summary>
        [Theory]
        [MemberData(nameof(GoodDataDistributed))]
        public void CoordinatesFunctionVariableYs_InverseF_Returns_IScalar(List<ICoordinate<double, IDistribution>> value)
        {
            //ICoordinatesFunction<double, IDistribution> testObj = new CoordinatesFunctionVariableYs(value);
            //IOrdinate result = testObj.InverseF(IScalarFactory.Factory(0));
            //Assert.NotNull(result);
        }
        /// <summary> Tests that the Inverse F Function returns True. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_InverseF_Throws_ArgumentException()
        {
            try
            {
                //ICoordinatesFunction<double, IDistribution> testObj = new CoordinatesFunctionVariableYs(new List<ICoordinate<double, IDistribution>>(new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(2), map)));
                //IOrdinate result = testObj.InverseF(IScalarFactory.Factory(0));
                Assert.True(false);
            }
            catch (ArgumentException e)
            {
                var m = e.Message;
                Assert.Equal("The specified set of coordinate is invalid. At least one x value maps to more than one y value (e.g. the set does not meet the definition of a function).", m);
            }
        }
        /// <summary> Tests that the Inverse F Function returns False. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_InverseFNull_Throws_ArgumentNullException()
        {
            try
            {
                //ICoordinatesFunction<double, IDistribution> testObj = new CoordinatesFunctionVariableYs(new List<ICoordinate<double, IDistribution>>(new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(0), map)));
                //IOrdinate result = testObj.InverseF(null);
                Assert.True(false);
            }
            catch (ArgumentNullException e)
            {
                var m = e.Message;
                Assert.Equal("Value cannot be null.", m);
            }
        }
        //Is never reached
        /// <summary> Tests that the Inverse F Function when given an input not in the given coordinates, throws an <see cref="ArgumentOutOfRangeException"/>. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_InverseF_Throws_ArgumentOutOfRangeException()
        {
            try
            {
               // IFunctionFamily<IScalar, IScalar> testObj = new CoordinatesFunctionVariableYs(new List<ICoordinate<IScalar, IScalar>>(new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(1), IScalarFactory.Factory(2), map)));
                //IScalar result = testObj.InverseF(IScalarFactory.Factory(5));
                Assert.True(false);
            }
            catch (ArgumentOutOfRangeException e)
            {
                var m = e.Message;
                Assert.Equal("The specified y value was not found in any of the coordinates. Interpolation is not supported for coorindates with distributed x or y values.", m);
            }
        }
        #endregion

        #region Sample

        /// <summary> Tests that the Sample method returns the mean for .5 probability on Normal Distribution. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_Sample_Returns_True_NormalDist1()
        {
            List<ICoordinate<double, IDistribution>> distCoords = DistributedCoordinates(new double[] { 0 }, new IDistribution[] { new Normal(1, 0) });
            ICoordinatesFunction<double, IDistribution> function = new CoordinatesFunctionVariableYs(distCoords);
            ICoordinatesFunction<double, double> sampledFunction = function.Sample(.5);
            List<ICoordinate<double, double>> sampledCoords =  sampledFunction.Coordinates;
            ICoordinate<double, double> coord = sampledCoords[0];
            Assert.True(sampledCoords.Count == 1);
            Assert.True(coord.X == 0 && coord.Y == 1);
            
        }
        /// <summary> Tests that the Sample method returns the mean for .5 probability on Normal Distribution. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_Sample_Returns_True_NormalDist2()
        {
            List<ICoordinate<double, IDistribution>> distCoords = DistributedCoordinates(new double[] { 0 }, new IDistribution[] { new Normal(2, 0) });
            ICoordinatesFunction<double, IDistribution> function = new CoordinatesFunctionVariableYs(distCoords);
            ICoordinatesFunction<double, double> sampledFunction = function.Sample(.5);
            List<ICoordinate<double, double>> sampledCoords = sampledFunction.Coordinates;
            ICoordinate<double, double> coord = sampledCoords[0];
            Assert.True(sampledCoords.Count == 1);
            Assert.True(coord.X == 0 && coord.Y == 2);

        }

        /// <summary> Tests that the Sample method returns the most likely value for .5 probability for triangular distribution. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_Sample_Returns_True_TriangularDist1()
        {
            List<ICoordinate<double, IDistribution>> distCoords = DistributedCoordinates(new double[] { 5 }, new IDistribution[] { new Triangular(10,20,30) });
            ICoordinatesFunction<double, IDistribution> function = new CoordinatesFunctionVariableYs(distCoords);
            ICoordinatesFunction<double, double> sampledFunction = function.Sample(.5);
            List<ICoordinate<double, double>> sampledCoords = sampledFunction.Coordinates;
            ICoordinate<double, double> coord = sampledCoords[0];
            Assert.True(sampledCoords.Count == 1);
            Assert.True(coord.X == 5 && coord.Y == 20);

        }

        /// <summary> Tests that the Sample method returns the min value for 0.0 probability for triangular distribution. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_Sample_Returns_True_TriangularDist2()
        {
            List<ICoordinate<double, IDistribution>> distCoords = DistributedCoordinates(new double[] { 5 }, new IDistribution[] { new Triangular(10, 20, 30) });
            ICoordinatesFunction<double, IDistribution> function = new CoordinatesFunctionVariableYs(distCoords);
            ICoordinatesFunction<double, double> sampledFunction = function.Sample(0.0);
            List<ICoordinate<double, double>> sampledCoords = sampledFunction.Coordinates;
            ICoordinate<double, double> coord = sampledCoords[0];
            Assert.True(sampledCoords.Count == 1);
            Assert.True(coord.X == 5 && coord.Y == 10);

        }

        /// <summary> Tests that the Sample method returns the max value for 1.0 probability for triangular distribution. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_Sample_Returns_True_TriangularDist3()
        {
            List<ICoordinate<double, IDistribution>> distCoords = DistributedCoordinates(new double[] { 5 }, new IDistribution[] { new Triangular(10, 20, 30) });
            ICoordinatesFunction<double, IDistribution> function = new CoordinatesFunctionVariableYs(distCoords);
            ICoordinatesFunction<double, double> sampledFunction = function.Sample(1.0);
            List<ICoordinate<double, double>> sampledCoords = sampledFunction.Coordinates;
            ICoordinate<double, double> coord = sampledCoords[0];
            Assert.True(sampledCoords.Count == 1);
            Assert.True(coord.X == 5 && coord.Y == 30);

        }

        /// <summary> Tests that the Sample method returns the max value for 1.0 probability for Uniform distribution. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_Sample_Returns_True_UniformDist1()
        {
            List<ICoordinate<double, IDistribution>> distCoords = DistributedCoordinates(new double[] { 5 }, new IDistribution[] { new Uniform(10, 30) });
            ICoordinatesFunction<double, IDistribution> function = new CoordinatesFunctionVariableYs(distCoords);
            ICoordinatesFunction<double, double> sampledFunction = function.Sample(1.0);
            List<ICoordinate<double, double>> sampledCoords = sampledFunction.Coordinates;
            ICoordinate<double, double> coord = sampledCoords[0];
            Assert.True(sampledCoords.Count == 1);
            Assert.True(coord.X == 5 && coord.Y == 30);

        }

        /// <summary> Tests that the Sample method returns the min value for 0.0 probability for Uniform distribution. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_Sample_Returns_True_UniformDist2()
        {
            List<ICoordinate<double, IDistribution>> distCoords = DistributedCoordinates(new double[] { 5 }, new IDistribution[] { new Uniform(10, 30) });
            ICoordinatesFunction<double, IDistribution> function = new CoordinatesFunctionVariableYs(distCoords);
            ICoordinatesFunction<double, double> sampledFunction = function.Sample(0.0);
            List<ICoordinate<double, double>> sampledCoords = sampledFunction.Coordinates;
            ICoordinate<double, double> coord = sampledCoords[0];
            Assert.True(sampledCoords.Count == 1);
            Assert.True(coord.X == 5 && coord.Y == 10);

        }

        /// <summary> Tests that the Sample method returns the mean value for 0.5 probability for Uniform distribution. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_Sample_Returns_True_UniformDist3()
        {
            List<ICoordinate<double, IDistribution>> distCoords = DistributedCoordinates(new double[] { 5 }, new IDistribution[] { new Uniform(10, 30) });
            ICoordinatesFunction<double, IDistribution> function = new CoordinatesFunctionVariableYs(distCoords);
            ICoordinatesFunction<double, double> sampledFunction = function.Sample(0.5);
            List<ICoordinate<double, double>> sampledCoords = sampledFunction.Coordinates;
            ICoordinate<double, double> coord = sampledCoords[0];
            Assert.True(sampledCoords.Count == 1);
            Assert.True(coord.X == 5 && coord.Y == 20);
        }

        /// <summary> Tests that the Sample method returns the mean value for 0.5 probability for Uniform distribution. </summary>
        [Fact]
        public void CoordinatesFunctionVariableYs_SampleWithInterpolator_Returns_True_UniformDist3()
        {
            List<ICoordinate<double, IDistribution>> distCoords = DistributedCoordinates(new double[] { 5 }, new IDistribution[] { new Uniform(10, 30) });
            ICoordinatesFunction<double, IDistribution> function = new CoordinatesFunctionVariableYs(distCoords);
            ICoordinatesFunction<double, double> sampledFunction = function.Sample(0.5);
            List<ICoordinate<double, double>> sampledCoords = sampledFunction.Coordinates;
            ICoordinate<double, double> coord = sampledCoords[0];
            Assert.True(sampledCoords.Count == 1);
            Assert.True(coord.X == 5 && coord.Y == 20);
        }
        #endregion

        #endregion
    }
}
