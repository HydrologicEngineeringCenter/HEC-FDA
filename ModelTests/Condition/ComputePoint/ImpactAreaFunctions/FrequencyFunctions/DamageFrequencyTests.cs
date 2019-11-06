using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;
using Model.Condition.ComputePoint.ImpactAreaFunctions;
using Functions;
using Functions.Utilities;
using Model;
using Functions.Ordinates;
using Functions.CoordinatesFunctions;

namespace ModelTests.Condition.ComputePoint.ImpactAreaFunctions.FrequencyFunctions
{
    [ExcludeFromCodeCoverage]
    public class DamageFrequencyTests
    {
        //NotSet = 0,
        //InflowFrequency = 1,            //AnnualExceedanceChance-> InflowPeakDischarge
        //InflowOutflow = 2,              //InflowPeakDischarge   -> OutflowPeakDischarge
        //OutflowFrequency = 3,           //AnnualExceedanceChance-> OutflowPeakDischarge
        //Rating = 4,                     //OutflowPeakDischarge  -> PeakExteriorStage
        //ExteriorStageFrequency = 5,     //AnnualExceedanceChance-> ExteriorPeakStage
        //ExteriorInteriorStage = 6,      //ExteriorPeakStage     -> InteriorPeakStage
        //InteriorStageFrequency = 7,     //AnnualExceedanceChance-> PeakInteriorStage
        //InteriorStageDamage = 8,        //InteriorPeakStage     -> AggregatedDamage
        //DamageFrequency = 9,            //AnnualExceedanceChance-> AggregatedDamage 
        //LeveeFailure = 10,              //Stage                 -> ChanceOfFailure
        ////UnUsed = 99,                  //Unknown or Ineligible Type (e.g. non-increasing)

        //[Fact]
        //public void test1_SameCoordinates()
        //{
        //    //   Statistics.Distributions.LogPearsonIII lp3 = new Statistics.Distributions.LogPearsonIII(5, 1, 1);
        //    // ImpactAreaFunctionFactory.CreateNew()

        //    List<double> xs = new List<double>() { 0, .25, .5, .75, 1 };
        //    List<double> ys = new List<double>() { 0, 25, 50, 75, 100 };

        //    ICoordinatesFunction<double, double> coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);
        //    ImpactAreaFunctionEnum type = ImpactAreaFunctionEnum.InflowFrequency;

        //    IFrequencyFunction<double> inflowFrequency = ImpactAreaFunctionFactory.CreateNewFrequencyFunction(coordFunction, type);
        //    IComputableFrequencyFunction computableInflowFreq = inflowFrequency.Sample(.5);


        //    coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);
        //    type = ImpactAreaFunctionEnum.InflowOutflow;

        //    ITransformFunction<double> inflowOutflow = ImpactAreaFunctionFactory.CreateNewTransformFunction<double>(coordFunction, type);
        //    IComputableTransformFunction computableInflowOutflow = inflowOutflow.Sample(.5);

        //    IComputableFrequencyFunction composedFunction = computableInflowFreq.Compose(computableInflowOutflow);

        //    Assert.True(true);
        //}


        //[Fact]
        //public void test1_DiffCoordinates()
        //{
        //    //   Statistics.Distributions.LogPearsonIII lp3 = new Statistics.Distributions.LogPearsonIII(5, 1, 1);
        //    // ImpactAreaFunctionFactory.CreateNew()

        //    List<double> xs = new List<double>() { 0, .25, .5, .75, 1 };
        //    List<double> ys = new List<double>() { 0, 25, 50, 75, 100 };

        //    ICoordinatesFunction<double, double> coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);
        //    ImpactAreaFunctionEnum type = ImpactAreaFunctionEnum.InflowFrequency;

        //    IFrequencyFunction<double> inflowFrequency = ImpactAreaFunctionFactory.CreateNewFrequencyFunction(coordFunction, type);
        //    IComputableFrequencyFunction computableInflowFreq = inflowFrequency.Sample(.5);

        //    xs = new List<double>() { 0, 25, 50, 75, 100 };
        //    ys = new List<double>() { 0, 25, 50, 75, 100 };
        //    coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);
        //    type = ImpactAreaFunctionEnum.InflowOutflow;

        //    ITransformFunction<double> inflowOutflow = ImpactAreaFunctionFactory.CreateNewTransformFunction<double>(coordFunction, type);
        //    IComputableTransformFunction computableInflowOutflow = inflowOutflow.Sample(.5);

        //    IComputableFrequencyFunction composedFunction = computableInflowFreq.Compose(computableInflowOutflow);

        //    Assert.True(composedFunction.Type == ImpactAreaFunctionEnum.OutflowFrequency);
        //    Assert.True(composedFunction.F(0) == 0);
        //    Assert.True(composedFunction.F(1) == 100);
        //}

        [Fact]
        public void Sampler_Testing()
        {
            
            Sampler.RegisterSampler(new ConstantSampler());

            List<double> xs = new List<double>() { 0, .25, .5, .75, 1 };
            List<double> ys = new List<double>() { 0, 25, 50, 75, 100 };

            ICoordinatesFunctionBase coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);
            ImpactAreaFunctionEnum type = ImpactAreaFunctionEnum.InflowFrequency;

            IFdaFunction inflowFrequency = ImpactAreaFunctionFactory.CreateFdaFunction(coordFunction, type);
            IFunction computableInflowFreq = Sampler.Sample(inflowFrequency.Function);

            xs = new List<double>() { 0, 25, 50, 75, 100 };
            ys = new List<double>() { 0, 25, 50, 75, 100 };
            coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);
            type = ImpactAreaFunctionEnum.InflowOutflow;

            IFdaFunction inflowOutflow = ImpactAreaFunctionFactory.CreateFdaFunction(coordFunction, type);
            IFunction computeableInflowOutflow = Sampler.Sample(inflowOutflow.Function);

            //IComputableTransformFunction computableInflowOutflow = inflowOutflow.Sample(.5);

            //IComputableFrequencyFunction composedFunction = computableInflowFreq.Compose(computableInflowOutflow);

            IFunction composedFunction = computableInflowFreq.Compose(computeableInflowOutflow);
           // Assert.True(composedFunction.Type == ImpactAreaFunctionEnum.OutflowFrequency);
            Assert.True(composedFunction.F(new Constant(0)).Value() == 0);
            Assert.True(composedFunction.F(new Constant(1)).Value() == 100);
        }



    }
}
