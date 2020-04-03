using Functions;
using Functions.CoordinatesFunctions;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;


namespace FunctionsTests.Utilities.Samplers
{
    [ExcludeFromCodeCoverage]
    public class DistributionSamplerTests
    {

        #region Can Sample
        /// <summary> Tests that with a distributed function, the "CanSample()" returns true </summary>
        [Fact]
        public void DistributionSampler_CanSample_GoodInput_Returns_Bool()
        {
            DistributionSampler distSampler = new DistributionSampler();

            List<double> xs = new List<double>() { 0, 1, 2, 3 };
            List<IDistributedOrdinate> ys = new List<IDistributedOrdinate>()
            {
                IDistributedOrdinateFactory.Factory( new Normal(1, 0)),
                IDistributedOrdinateFactory.Factory( new Normal(1, 0)),
                IDistributedOrdinateFactory.Factory( new Normal(1, 0)),
                IDistributedOrdinateFactory.Factory( new Normal(1, 0))
            };
            CoordinatesFunctionVariableYs distributedFunc = (CoordinatesFunctionVariableYs)ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.None);

            Assert.True(distSampler.CanSample(distributedFunc));
        }

        /// <summary> Tests that with a constant function, the "CanSample()" returns false </summary>
        [Fact]
        public void DistributionSampler_CanSample_BadInput_Returns_Bool()
        {
            DistributionSampler distSampler = new DistributionSampler();

            List<double> xs = new List<double>() { 0, 1, 2, 3 };
            List<double> ys = new List<double>() { 10, 11, 12, 13 };

            CoordinatesFunctionConstants constFunc = (CoordinatesFunctionConstants)ICoordinatesFunctionsFactory.Factory(xs, ys);
            Assert.False(distSampler.CanSample(constFunc));
        }

        #endregion

       

        #region Normal Distribution
        /// <summary> Tests that "Sample()" method returns the mean for .5 probability on Normal Distribution. </summary>
        [Fact]
        public void DistributionSampler_Sample_Normal_Point5Probability_Returns_IFunction()
        {
            DistributionSampler distSampler = new DistributionSampler();

            List<double> xs = new List<double>() { 0, 1, 2, 3 };
            List<IDistributedOrdinate> ys = new List<IDistributedOrdinate>()
            {
                IDistributedOrdinateFactory.Factory( new Normal(1, 0)),
                IDistributedOrdinateFactory.Factory( new Normal(2, 1)),
                IDistributedOrdinateFactory.Factory( new Normal(3, 2)),
                IDistributedOrdinateFactory.Factory( new Normal(4, 3))
            };
            CoordinatesFunctionVariableYs distributedFunc = (CoordinatesFunctionVariableYs)ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.None);

            IFunction sampledFunc = distSampler.Sample(distributedFunc, .5);
            //they should have the same number of coordinates.
            Assert.True(distributedFunc.Coordinates.Count == sampledFunc.Coordinates.Count);
            ICoordinate coord = sampledFunc.Coordinates[0];
            ICoordinate coord2 = sampledFunc.Coordinates[1];
            ICoordinate coord3 = sampledFunc.Coordinates[2];
            ICoordinate coord4 = sampledFunc.Coordinates[3];

            Assert.True(coord.X.Value() == 0 && coord.Y.Value() == 1);
            Assert.True(coord2.X.Value() == 1 && coord2.Y.Value() == 2);
            Assert.True(coord3.X.Value() == 2 && coord3.Y.Value() == 3);
            Assert.True(coord4.X.Value() == 3 && coord4.Y.Value() == 4);

        }

        ///// <summary> Tests that "Sample()" method returns the mean for .5 probability on Normal Distribution. </summary>
        //[Fact]
        //public void DistributionSampler_Sample_Normal_ZeroProbability_Returns_IFunction()
        //{
        //    DistributionSampler distSampler = new DistributionSampler();

        //    List<double> xs = new List<double>() { 0 };// { 0, 1, 2, 3 };
        //    List<IDistributedValue> ys = new List<IDistributedValue>()
        //    {
        //        DistributedValueFactory.Factory( new Normal(1, 0)),
        //        //DistributedValueFactory.Factory( new Normal(2, 1)),
        //        //DistributedValueFactory.Factory( new Normal(3, 2)),
        //        //DistributedValueFactory.Factory( new Normal(4, 3))
        //    };
        //    CoordinatesFunctionVariableYs distributedFunc = (CoordinatesFunctionVariableYs)ICoordinatesFunctionsFactory.Factory(xs, ys);

        //    IFunction sampledFunc = distSampler.Sample(distributedFunc, 0);
        //    //they should have the same number of coordinates.
        //    Assert.True(distributedFunc.Coordinates.Count == sampledFunc.Coordinates.Count);
        //    ICoordinate coord = sampledFunc.Coordinates[0];
        //    //ICoordinate coord2 = sampledFunc.Coordinates[1];
        //    //ICoordinate coord3 = sampledFunc.Coordinates[2];
        //    //ICoordinate coord4 = sampledFunc.Coordinates[3];

        //    Assert.True(coord.X.Value() == 0 && coord.Y.Value() == 0);
        //    //Assert.True(coord2.X.Value() == 1 && coord2.Y.Value() == 1);
        //    //Assert.True(coord3.X.Value() == 2 && coord3.Y.Value() == 2);
        //    //Assert.True(coord4.X.Value() == 3 && coord4.Y.Value() == 3);

        //}

        ///// <summary> Tests that "Sample()" method returns the mean for .5 probability on Normal Distribution. </summary>
        //[Fact]
        //public void DistributionSampler_Sample_Normal_OneProbability_Returns_IFunction()
        //{
        //    DistributionSampler distSampler = new DistributionSampler();

        //    List<double> xs = new List<double>() { 0, 1, 2, 3 };
        //    List<IDistributedValue> ys = new List<IDistributedValue>()
        //    {
        //        DistributedValueFactory.Factory( new Normal(1, 0)),
        //        DistributedValueFactory.Factory( new Normal(2, 1)),
        //        DistributedValueFactory.Factory( new Normal(3, 2)),
        //        DistributedValueFactory.Factory( new Normal(4, 3))
        //    };
        //    CoordinatesFunctionVariableYs distributedFunc = (CoordinatesFunctionVariableYs)ICoordinatesFunctionsFactory.Factory(xs, ys);

        //    IFunction sampledFunc = distSampler.Sample(distributedFunc, .99);
        //    //they should have the same number of coordinates.
        //    Assert.True(distributedFunc.Coordinates.Count == sampledFunc.Coordinates.Count);
        //    ICoordinate coord = sampledFunc.Coordinates[0];
        //    ICoordinate coord2 = sampledFunc.Coordinates[1];
        //    ICoordinate coord3 = sampledFunc.Coordinates[2];
        //    ICoordinate coord4 = sampledFunc.Coordinates[3];

        //    Assert.True(coord.X.Value() == 0 && coord.Y.Value() == 1);
        //    Assert.True(coord2.X.Value() == 1 && coord2.Y.Value() == 2);
        //    Assert.True(coord3.X.Value() == 2 && coord3.Y.Value() == 3);
        //    Assert.True(coord4.X.Value() == 3 && coord4.Y.Value() == 4);

        //}
        #endregion

        #region Triangular Distribution
        /// <summary> Tests that the Sample method returns the most likely value for .5 probability for triangular distribution. </summary>
        [Fact]
        public void DistributionSampler_Sample_Triangular_Point5Probability_Returns_IFunction()
        {
            DistributionSampler distSampler = new DistributionSampler();

            List<double> xs = new List<double>() { 0, 1, 2, 3 };
            List<IDistributedOrdinate> ys = new List<IDistributedOrdinate>()
            {
                IDistributedOrdinateFactory.Factory( new Triangular(5, 10, 15)),
                IDistributedOrdinateFactory.Factory( new Triangular(10,20,30)),
                IDistributedOrdinateFactory.Factory( new Triangular(20,30,40)),
                IDistributedOrdinateFactory.Factory( new Triangular(30,40,50))
            };
            CoordinatesFunctionVariableYs distributedFunc = (CoordinatesFunctionVariableYs)ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.None);

            IFunction sampledFunc = distSampler.Sample(distributedFunc, .5);
            //they should have the same number of coordinates.
            Assert.True(distributedFunc.Coordinates.Count == sampledFunc.Coordinates.Count);
            ICoordinate coord = sampledFunc.Coordinates[0];
            ICoordinate coord2 = sampledFunc.Coordinates[1];
            ICoordinate coord3 = sampledFunc.Coordinates[2];
            ICoordinate coord4 = sampledFunc.Coordinates[3];

            Assert.True(coord.X.Value() == 0 && coord.Y.Value() == 10);
            Assert.True(coord2.X.Value() == 1 && coord2.Y.Value() == 20);
            Assert.True(coord3.X.Value() == 2 && coord3.Y.Value() == 30);
            Assert.True(coord4.X.Value() == 3 && coord4.Y.Value() == 40);

        }

        /// <summary> Tests that the Sample method returns the min value for 0.0 probability for triangular distribution. </summary>
        [Fact]
        public void DistributionSampler_Sample_Triangular_ZeroProbability_Returns_IFunction()
        {
            DistributionSampler distSampler = new DistributionSampler();

            List<double> xs = new List<double>() { 0, 1, 2, 3 };
            List<IDistributedOrdinate> ys = new List<IDistributedOrdinate>()
            {
                IDistributedOrdinateFactory.Factory( new Triangular(5, 10, 15)),
                IDistributedOrdinateFactory.Factory( new Triangular(10,20,30)),
                IDistributedOrdinateFactory.Factory( new Triangular(20,30,40)),
                IDistributedOrdinateFactory.Factory( new Triangular(30,40,50))
            };
            CoordinatesFunctionVariableYs distributedFunc = (CoordinatesFunctionVariableYs)ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.None);

            IFunction sampledFunc = distSampler.Sample(distributedFunc, 0.0);
            //they should have the same number of coordinates.
            Assert.True(distributedFunc.Coordinates.Count == sampledFunc.Coordinates.Count);
            ICoordinate coord = sampledFunc.Coordinates[0];
            ICoordinate coord2 = sampledFunc.Coordinates[1];
            ICoordinate coord3 = sampledFunc.Coordinates[2];
            ICoordinate coord4 = sampledFunc.Coordinates[3];

            Assert.True(coord.X.Value() == 0 && coord.Y.Value() == 5);
            Assert.True(coord2.X.Value() == 1 && coord2.Y.Value() == 10);
            Assert.True(coord3.X.Value() == 2 && coord3.Y.Value() == 20);
            Assert.True(coord4.X.Value() == 3 && coord4.Y.Value() == 30);

        }

        /// <summary> Tests that the Sample method returns the max value for 1.0 probability for triangular distribution. </summary>
        [Fact]
        public void DistributionSampler_Sample_Triangular_OneProbability_Returns_IFunction()
        {
            DistributionSampler distSampler = new DistributionSampler();

            List<double> xs = new List<double>() { 0, 1, 2, 3 };
            List<IDistributedOrdinate> ys = new List<IDistributedOrdinate>()
            {
                IDistributedOrdinateFactory.Factory( new Triangular(5, 10, 15)),
                IDistributedOrdinateFactory.Factory( new Triangular(10,20,30)),
                IDistributedOrdinateFactory.Factory( new Triangular(20,30,40)),
                IDistributedOrdinateFactory.Factory( new Triangular(30,40,50))
            };
            CoordinatesFunctionVariableYs distributedFunc = (CoordinatesFunctionVariableYs)ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.None);

            IFunction sampledFunc = distSampler.Sample(distributedFunc, 1.0);
            //they should have the same number of coordinates.
            Assert.True(distributedFunc.Coordinates.Count == sampledFunc.Coordinates.Count);
            ICoordinate coord = sampledFunc.Coordinates[0];
            ICoordinate coord2 = sampledFunc.Coordinates[1];
            ICoordinate coord3 = sampledFunc.Coordinates[2];
            ICoordinate coord4 = sampledFunc.Coordinates[3];

            Assert.True(coord.X.Value() == 0 && coord.Y.Value() == 15);
            Assert.True(coord2.X.Value() == 1 && coord2.Y.Value() == 30);
            Assert.True(coord3.X.Value() == 2 && coord3.Y.Value() == 40);
            Assert.True(coord4.X.Value() == 3 && coord4.Y.Value() == 50);

        }

        #endregion

        #region Uniform Distribution
        /// <summary> Tests that the Sample method returns the max value for 1.0 probability for Uniform distribution. </summary>
        [Fact]
        public void DistributionSampler_Sample_Uniform_OneProbability_Returns_IFunction()
        {
            DistributionSampler distSampler = new DistributionSampler();

            List<double> xs = new List<double>() { 0, 1};
            List<IDistributedOrdinate> ys = new List<IDistributedOrdinate>()
            {
                IDistributedOrdinateFactory.Factory( new Uniform(5, 10)),
                IDistributedOrdinateFactory.Factory( new Uniform(10,20)),
            };
            CoordinatesFunctionVariableYs distributedFunc = (CoordinatesFunctionVariableYs)ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.None);

            IFunction sampledFunc = distSampler.Sample(distributedFunc, 1.0);
            //they should have the same number of coordinates.
            Assert.True(distributedFunc.Coordinates.Count == sampledFunc.Coordinates.Count);
            ICoordinate coord = sampledFunc.Coordinates[0];
            ICoordinate coord2 = sampledFunc.Coordinates[1];
         

        //    Assert.True(coord.X.Value() == 0 && coord.Y.Value() == 10);
        //    Assert.True(coord2.X.Value() == 1 && coord2.Y.Value() == 20);
         

        //}

        ///// <summary> Tests that the Sample method returns the min value for 0.0 probability for Uniform distribution. </summary>
        //[Fact]
        //public void DistributionSampler_Sample_Uniform_ZeroProbability_Returns_IFunction()
        //{
        //    DistributionSampler distSampler = new DistributionSampler();

            List<double> xs = new List<double>() { 0, 1 };
            List<IDistributedOrdinate> ys = new List<IDistributedOrdinate>()
            {
                IDistributedOrdinateFactory.Factory( new Uniform(5, 10)),
                IDistributedOrdinateFactory.Factory( new Uniform(10,20)),
            };
            CoordinatesFunctionVariableYs distributedFunc = (CoordinatesFunctionVariableYs)ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.None);

        //    IFunction sampledFunc = distSampler.Sample(distributedFunc, 0.0);
        //    //they should have the same number of coordinates.
        //    Assert.True(distributedFunc.Coordinates.Count == sampledFunc.Coordinates.Count);
        //    ICoordinate coord = sampledFunc.Coordinates[0];
        //    ICoordinate coord2 = sampledFunc.Coordinates[1];


        //    Assert.True(coord.X.Value() == 0 && coord.Y.Value() == 5);
        //    Assert.True(coord2.X.Value() == 1 && coord2.Y.Value() == 10);

        //}

        ///// <summary> Tests that the Sample method returns the mean value for 0.5 probability for Uniform distribution. </summary>
        //[Fact]
        //public void DistributionSampler_Sample_Uniform_Point5Probability_Returns_IFunction()
        //{
        //    DistributionSampler distSampler = new DistributionSampler();

            List<double> xs = new List<double>() { 0, 1 };
            List<IDistributedOrdinate> ys = new List<IDistributedOrdinate>()
            {
                IDistributedOrdinateFactory.Factory( new Uniform(5, 10)),
                IDistributedOrdinateFactory.Factory( new Uniform(10,20)),
            };
            CoordinatesFunctionVariableYs distributedFunc = (CoordinatesFunctionVariableYs)ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.None);

        //    IFunction sampledFunc = distSampler.Sample(distributedFunc, 0.5);
        //    //they should have the same number of coordinates.
        //    Assert.True(distributedFunc.Coordinates.Count == sampledFunc.Coordinates.Count);
        //    ICoordinate coord = sampledFunc.Coordinates[0];
        //    ICoordinate coord2 = sampledFunc.Coordinates[1];


        //    Assert.True(coord.X.Value() == 0 && coord.Y.Value() == 7.5);
        //    Assert.True(coord2.X.Value() == 1 && coord2.Y.Value() == 15);
        //}


        //#endregion
    }
}
