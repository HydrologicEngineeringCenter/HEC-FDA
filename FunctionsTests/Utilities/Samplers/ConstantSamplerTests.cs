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

    public class ConstantSamplerTests
    {
        /// <summary> Tests that with a constant function, the "CanSample()" returns true </summary>
        [Fact]
        public void ConstantSampler_CanSample_GoodInput_Returns_Bool()
        {
            ConstantSampler constSampler = new ConstantSampler();

            List<double> xs = new List<double>() { 0, 1, 2, 3 };
            List<double> ys = new List<double>() { 10, 11, 12, 13 };
            CoordinatesFunctionConstants constFunc = (CoordinatesFunctionConstants)ICoordinatesFunctionsFactory.Factory(xs, ys);
            Assert.True(constSampler.CanSample(constFunc));
        }

        ///// <summary> Tests that with a non constant function, the "CanSample()" returns false </summary>
        //[Fact]
        //public void ConstantSampler_CanSample_BadInput_Returns_Bool()
        //{
        //    ConstantSampler constSampler = new ConstantSampler();

            List<double> xs = new List<double>() { 0, 1, 2, 3 };
            List<IDistributedOrdinate> ys = new List<IDistributedOrdinate>()
            {
                IDistributedOrdinateFactory.Factory( new Normal(1, 0)),
                IDistributedOrdinateFactory.Factory( new Normal(1, 0)),
                IDistributedOrdinateFactory.Factory( new Normal(1, 0)),
                IDistributedOrdinateFactory.Factory( new Normal(1, 0))
            };
            CoordinatesFunctionVariableYs distributedFunc = (CoordinatesFunctionVariableYs)ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.None);
            Assert.False(constSampler.CanSample(distributedFunc));
        }

        /// <summary> Tests that the "Sample()" method returns the constant function passed in.</summary>
        [Fact]
        public void ConstantSampler_Sample_Returns_IFunction()
        {
            ConstantSampler constSampler = new ConstantSampler();

            List<double> xs = new List<double>() { 0, 1, 2, 3 };
            List<double> ys = new List<double>() { 10, 11, 12, 13 };
            CoordinatesFunctionConstants constFunc = (CoordinatesFunctionConstants)ICoordinatesFunctionsFactory.Factory(xs, ys);
            IFunction ifunc = constSampler.Sample(constFunc, .5);
            //the constant sampler should be returning back the same function.
            Assert.True(constFunc == ifunc);
        }
    }
}
