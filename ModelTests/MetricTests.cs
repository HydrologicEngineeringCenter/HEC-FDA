using Functions;
using Functions.CoordinatesFunctions;
using Model;
using Model.Inputs.Functions.ImpactAreaFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ModelTests
{
    public class MetricTests
    {

        #region Compute
        /// <summary>
        /// Tests that the base function composed with a function that is inside the base will get the points from the inside function.
        /// </summary>
        [Theory]
        [InlineData(new double[] { .1, .2, .3, .4, .5 }, new double[] { 10, 20, 30, 40, 50 }, 50, .5)]
        [InlineData(new double[] { .1, .2, .3, .4, .5 }, new double[] { 10, 20, 30, 40, 50 }, 40, .4)]
        [InlineData(new double[] { .1, .2, .3, .4, .5 }, new double[] { 10, 20, 30, 40, 50 }, 30, .3)]
        [InlineData(new double[] { .1, .2, .3, .4, .5 }, new double[] { 10, 20, 30, 40, 50 }, 20, .2)]
        public void Metric_Compute_Returns_double(double[] xs, double[] ys, double thresholdValue, double expectedValue)
        {
            double dummyProb = .5;//this isn't actually being used for anything
            Sampler.RegisterSampler(new ConstantSampler());

            Metric metric = new Metric(MetricEnum.Damages, thresholdValue);
            ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(xs.ToList(), ys.ToList(), InterpolationEnum.Linear);
            IFrequencyFunction freqFunc = ImpactAreaFunctionFactory.FactoryFrequency(func, ImpactAreaFunctionEnum.DamageFrequency);
            double value = metric.Compute(freqFunc, dummyProb);
            Assert.True(value == expectedValue);

        }

        
        [Fact]
        public void Metric_Compute_BadFunctionType_Returns_Exception()
        {
            double dummyProb = .5;//this isn't actually being used for anything
            Sampler.RegisterSampler(new ConstantSampler());

            Metric metric = new Metric(MetricEnum.Damages, 5000);
            ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(new List<double>() { 1, 2 },new List<double>() { 1, 2 }, InterpolationEnum.Linear);
            IFrequencyFunction freqFunc = ImpactAreaFunctionFactory.FactoryFrequency(func, ImpactAreaFunctionEnum.InflowFrequency);
            Assert.Throws<ArgumentException>(() => metric.Compute(freqFunc, dummyProb));

        }

        #endregion

        #region TargetFunction

        [Theory]
        [InlineData(MetricEnum.Damages, ImpactAreaFunctionEnum.DamageFrequency)]
        [InlineData(MetricEnum.ExteriorStage, ImpactAreaFunctionEnum.ExteriorStageFrequency)]
        [InlineData(MetricEnum.InteriorStage, ImpactAreaFunctionEnum.InteriorStageFrequency)]
        [InlineData(MetricEnum.ExpectedAnnualDamage, ImpactAreaFunctionEnum.DamageFrequency)]
        public void Metric_TargetFunction_Returns_(MetricEnum type, ImpactAreaFunctionEnum expectedType)
        {
            Metric metric = new Metric(type, 5000);
            Assert.True(metric.TargetFunction == expectedType);
        }

        #endregion


    }
}
