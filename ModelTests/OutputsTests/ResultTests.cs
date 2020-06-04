using Functions.CoordinatesFunctions;
using Model;
using Model.Inputs.Functions.ImpactAreaFunctions;
using Model.Outputs;
using ModelTests.InputsTests.ConditionsTests;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ModelTests.OutputsTests
{
    public class ResultTests : ComputeTestData
    {
        [Fact]
        public void ComputeTest()
        {
            //register the samplers
            RegisterSamplers();


            string name = "name";
            string desc = "desc";
            int analysisYear = 1999;
            double thresholdValue = 200000000;
            int seed = 1;

            //create inflow freq function
            List<double> inflowFreqxs = new List<double>() { 0, .5, 1 };
            List<double> inflowFreqys = new List<double>() { 0, 10000, 100000 };
            IFrequencyFunction inflowFreq = CreateFrequencyFunction(inflowFreqxs, inflowFreqys, InterpolationEnum.Linear, IFdaFunctionEnum.InflowFrequency);

            //create list of transform functions
            List<ITransformFunction> transformFunctions = new List<ITransformFunction>();

            //create rating curve
            List<double> ratFlows = new List<double>() { 0, 100, 10000, 100000 };
            List<double> ratStages = new List<double>() { 0, 1, 10, 100 };
            transformFunctions.Add(CreateTransformFunction(ratFlows, ratStages, InterpolationEnum.Linear, IFdaFunctionEnum.Rating));

            //create interior stage damage transform function
            List<double> intStages = new List<double>() { 0, 1, 10, 100 };
            List<double> damage = new List<double>() { 0, 2000000, 200000000, 2000000000 };
            transformFunctions.Add(CreateTransformFunction(intStages, damage, InterpolationEnum.Linear, IFdaFunctionEnum.InteriorStageDamage));

            //create the metrics
            List<IMetric> metrics = CreateMetrics(new List<MetricEnum>() { MetricEnum.Damages }, new List<double>() { thresholdValue });

            ICondition condition = ConditionFactory.Factory(name, analysisYear, inflowFreq, transformFunctions, metrics);

            int randomPacketSize = transformFunctions.Count + 1;
            //TimeStampSeed = (int)new DateTime().Ticks;
            
            Result result = new Result(condition, seed);
            result.Compute();
            bool converged = result.Converged;
           // result.Metrics

            //IDictionary<IMetric, double> retval = condition.Compute(GetRandomNumbers(randomPacketSize));

            //double exceedanceProb = retval[metrics[0]];
            //Assert.True(exceedanceProb == .5);
        }
    }
}
