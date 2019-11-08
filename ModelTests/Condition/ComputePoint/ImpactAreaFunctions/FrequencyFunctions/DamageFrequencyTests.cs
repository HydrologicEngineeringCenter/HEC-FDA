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
using Statistics;
using Statistics.Distributions;

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
        public void Sampler_Testing_ConstantFunctions_NoInterpolator()
        {
            
            Sampler.RegisterSampler(new ConstantSampler());

            List<double> xs = new List<double>() { 0, .25, .5, .75, 1 };
            List<double> ys = new List<double>() { 0, 25, 50, 75, 100 };

            ICoordinatesFunction coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);
            ImpactAreaFunctionEnum type = ImpactAreaFunctionEnum.InflowFrequency;

            IFdaFunction inflowFrequency = ImpactAreaFunctionFactory.CreateFdaFunction(coordFunction, type);
            IFunction computableInflowFreq = Sampler.Sample(inflowFrequency.Function, .5);

            xs = new List<double>() { 0, 25, 50, 75, 100 };
            ys = new List<double>() { 0, 25, 50, 75, 100 };
            coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);
            type = ImpactAreaFunctionEnum.InflowOutflow;

            IFdaFunction inflowOutflow = ImpactAreaFunctionFactory.CreateFdaFunction(coordFunction, type);
            IFunction computeableInflowOutflow = Sampler.Sample(inflowOutflow.Function, .5);

            //IComputableTransformFunction computableInflowOutflow = inflowOutflow.Sample(.5);

            //IComputableFrequencyFunction composedFunction = computableInflowFreq.Compose(computableInflowOutflow);

            IFunction composedFunction = computableInflowFreq.Compose(computeableInflowOutflow);
           // Assert.True(composedFunction.Type == ImpactAreaFunctionEnum.OutflowFrequency);
            Assert.True(composedFunction.F(new Constant(0)).Value() == 0);
            Assert.True(composedFunction.F(new Constant(1)).Value() == 100);
        }

        [Fact]
        public void Sampler_Testing_ConstantFunctions_LinearInterpolator()
        {

            Sampler.RegisterSampler(new ConstantSampler());

            List<double> xs = new List<double>() { 0, .25, .5, .75, 1 };
            List<double> ys = new List<double>() { 0, 100, 1000, 2000, 3000 };

            ICoordinatesFunction coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
            ImpactAreaFunctionEnum type = ImpactAreaFunctionEnum.InflowFrequency;

            IFdaFunction inflowFrequency = ImpactAreaFunctionFactory.CreateFdaFunction(coordFunction, type);
            IFunction computableInflowFreq = Sampler.Sample(inflowFrequency.Function, .5);

            xs = new List<double>() { 0, 50, 150, 275, 1000 };
            ys = new List<double>() { 0, 10, 110, 750, 10000 };
            coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
            type = ImpactAreaFunctionEnum.InflowOutflow;

            IFdaFunction inflowOutflow = ImpactAreaFunctionFactory.CreateFdaFunction(coordFunction, type);
            IFunction computeableInflowOutflow = Sampler.Sample(inflowOutflow.Function, .5);

            //IComputableTransformFunction computableInflowOutflow = inflowOutflow.Sample(.5);

            //IComputableFrequencyFunction composedFunction = computableInflowFreq.Compose(computableInflowOutflow);

            IFunction composedFunction = computableInflowFreq.Compose(computeableInflowOutflow);

            //x  |  y
            //0    0
            //.25   60

            // Assert.True(composedFunction.Type == ImpactAreaFunctionEnum.OutflowFrequency);
            Assert.True(composedFunction.F(new Constant(0)).Value() == 0);
            Assert.True(composedFunction.F(new Constant(.25)).Value() == 60);
        }

        /// <summary>
        /// This one is failing because the coordinatesFunctionConstants.IsZOffOverlap() is returning true
        /// during the compute. I think these should be valid values. It might be because there is no interpolator. 
        /// Try this one with an interpolator.
        /// </summary>
        [Fact]
        public void Sampler_Testing_OneDistributedFunction_OneConstantFunction_1_NoInterpolator()
        {
            Sampler.RegisterSampler(new ConstantSampler());
            Sampler.RegisterSampler(new DistributionSampler());

            List<double> xs = new List<double>() { 0, .25, .5, .75, 1 };
            List<IDistribution> ys = new List<IDistribution>()
                   {
                       new Normal(1,2),
                       new Triangular(3,4,5),
                       new Uniform(5,6),
                       new Normal(6,1),
                       new Uniform(7,8),

                   };

            ICoordinatesFunction coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);
            ImpactAreaFunctionEnum type = ImpactAreaFunctionEnum.InflowFrequency;

            IFdaFunction inflowFrequency = ImpactAreaFunctionFactory.CreateFdaFunction(coordFunction, type);
            IFunction computableInflowFreq = Sampler.Sample(inflowFrequency.Function, .5);

            xs = new List<double>() { 0, 25, 50, 75, 100 };
            List<double> ys2 = new List<double>() { 0, 25, 50, 75, 100 };
            coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys2);
            type = ImpactAreaFunctionEnum.InflowOutflow;

            IFdaFunction inflowOutflow = ImpactAreaFunctionFactory.CreateFdaFunction(coordFunction, type);
            IFunction computeableInflowOutflow = Sampler.Sample(inflowOutflow.Function, .5);

            //IComputableTransformFunction computableInflowOutflow = inflowOutflow.Sample(.5);

            //IComputableFrequencyFunction composedFunction = computableInflowFreq.Compose(computableInflowOutflow);

            IFunction composedFunction = computableInflowFreq.Compose(computeableInflowOutflow);
            // Assert.True(composedFunction.Type == ImpactAreaFunctionEnum.OutflowFrequency);
            Assert.True(composedFunction.F(new Constant(0)).Value() == 0);
            Assert.True(composedFunction.F(new Constant(1)).Value() == 100);
        }

        [Fact]
        public void Sampler_Testing_OneDistributedFunction_OneConstantFunction_2_NoInterpolator()
        {
            Sampler.RegisterSampler(new ConstantSampler());
            Sampler.RegisterSampler(new DistributionSampler());

            List<double> xs = new List<double>() { 0, .25, .5, .75, 1 };
            List<IDistribution> ys = new List<IDistribution>()
                   {
                       new Normal(1,2),
                       new Triangular(3,4,5),
                       new Uniform(5,6),
                       new Normal(6,1),
                       new Uniform(7,8),

                   };

            ICoordinatesFunction coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);
            ImpactAreaFunctionEnum type = ImpactAreaFunctionEnum.InflowFrequency;

            IFdaFunction inflowFrequency = ImpactAreaFunctionFactory.CreateFdaFunction(coordFunction, type);
            IFunction computableInflowFreq = Sampler.Sample(inflowFrequency.Function, .5);

            xs = new List<double>() { 0,1,2,3,4 };
            List<double> ys2 = new List<double>() { 0, 25, 50, 75, 100 };
            coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys2);
            type = ImpactAreaFunctionEnum.InflowOutflow;

            IFdaFunction inflowOutflow = ImpactAreaFunctionFactory.CreateFdaFunction(coordFunction, type);
            IFunction computeableInflowOutflow = Sampler.Sample(inflowOutflow.Function, .5);

            //IComputableTransformFunction computableInflowOutflow = inflowOutflow.Sample(.5);

            //IComputableFrequencyFunction composedFunction = computableInflowFreq.Compose(computableInflowOutflow);

            IFunction composedFunction = computableInflowFreq.Compose(computeableInflowOutflow);
            // Assert.True(composedFunction.Type == ImpactAreaFunctionEnum.OutflowFrequency);
            Assert.True(composedFunction.F(new Constant(0)).Value() == 25);
            Assert.True(composedFunction.F(new Constant(.25)).Value() == 100);
        }

        [Fact]
        public void Sampler_Testing_TwoDistributedFunctions_NoInterpolator()
        {
            Sampler.RegisterSampler(new ConstantSampler());
            Sampler.RegisterSampler(new DistributionSampler());

            List<double> xs = new List<double>() { 0, .25, .5, .75, 1 };
            List<IDistribution> ys = new List<IDistribution>()
                   {
                       new Normal(1,2),
                       new Triangular(3,4,5),
                       new Uniform(5,6),
                       new Normal(6,1),
                       new Uniform(7,8),

                   };

            ICoordinatesFunction coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);
            ImpactAreaFunctionEnum type = ImpactAreaFunctionEnum.InflowFrequency;

            IFdaFunction inflowFrequency = ImpactAreaFunctionFactory.CreateFdaFunction(coordFunction, type);
            IFunction computableInflowFreq = Sampler.Sample(inflowFrequency.Function, .5);

            xs = new List<double>() { 0, 4, 5, 6, 7};
            List<double> ys2 = new List<double>() { 0, 25, 50, 75, 100 };
            coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys2);
            type = ImpactAreaFunctionEnum.InflowOutflow;

            IFdaFunction inflowOutflow = ImpactAreaFunctionFactory.CreateFdaFunction(coordFunction, type);
            IFunction computeableInflowOutflow = Sampler.Sample(inflowOutflow.Function, .5);

            //IComputableTransformFunction computableInflowOutflow = inflowOutflow.Sample(.5);

            //IComputableFrequencyFunction composedFunction = computableInflowFreq.Compose(computableInflowOutflow);

            IFunction composedFunction = computableInflowFreq.Compose(computeableInflowOutflow);
            // Assert.True(composedFunction.Type == ImpactAreaFunctionEnum.OutflowFrequency);
            Assert.True(composedFunction.F(new Constant(.25)).Value() == 25);
            Assert.True(composedFunction.F(new Constant(.75)).Value() == 75);
        }

        [Fact]
        public void TestingLinkedFunction_ConstantFunctions_NoInterpolator()
        {

            Sampler.RegisterSampler(new LinkedFunctionsSampler());


            List<double> xs = new List<double>() { 0, .1, .2, .3, .4 };
            List<double> ys = new List<double>() { 0, 25, 50, 75, 100 };

            ICoordinatesFunction coordFunction1 = ICoordinatesFunctionsFactory.Factory(xs, ys);

            List<double> xs2 = new List<double>() { .5, .6, .7, .8, .9 };
            List<double> ys2 = new List<double>() { 150, 250, 500, 750, 1000 };

            ICoordinatesFunction coordFunction2 = ICoordinatesFunctionsFactory.Factory(xs2, ys2);

            List<ICoordinatesFunction> funcs = new List<ICoordinatesFunction>() { coordFunction1, coordFunction2 };
            List<InterpolationEnum> enumsBetweenFuncs = new List<InterpolationEnum>() { InterpolationEnum.Linear };
            ICoordinatesFunction linkedFunction = ICoordinatesFunctionsFactory.Factory(funcs, enumsBetweenFuncs);

            ImpactAreaFunctionEnum type = ImpactAreaFunctionEnum.InflowFrequency;

            IFdaFunction inflowFrequency = ImpactAreaFunctionFactory.CreateFdaFunction(linkedFunction, type);
            IFunction computableInflowFreq = Sampler.Sample(inflowFrequency.Function, .5);

            Assert.True(true);
          
        }

        [Fact]
        public void TestingLinkedFunction_ConstantAndDistributedFunctions_NoInterpolator()
        {

            Sampler.RegisterSampler(new LinkedFunctionsSampler());


            List<double> xs = new List<double>() { 0, .1, .2, .3, .4 };
            List<double> ys = new List<double>() { 0, 25, 50, 75, 100 };

            ICoordinatesFunction coordFunction1 = ICoordinatesFunctionsFactory.Factory(xs, ys);

            List<double> xs2 = new List<double>() {.5,.6,.7,.8,.9 };
            List<IDistribution> ys2 = new List<IDistribution>()
                   {
                       new Normal(1,2),
                       new Triangular(3,4,5),
                       new Uniform(5,6),
                       new Normal(6,1),
                       new Uniform(7,8),

                   };

            ICoordinatesFunction coordFunction2 = ICoordinatesFunctionsFactory.Factory(xs2, ys2);

            List<ICoordinatesFunction> funcs = new List<ICoordinatesFunction>() { coordFunction1, coordFunction2 };
            List<InterpolationEnum> enumsBetweenFuncs = new List<InterpolationEnum>() { InterpolationEnum.Linear };
            ICoordinatesFunction linkedFunction = ICoordinatesFunctionsFactory.Factory(funcs, enumsBetweenFuncs);

            ImpactAreaFunctionEnum type = ImpactAreaFunctionEnum.InflowFrequency;

            IFdaFunction inflowFrequency = ImpactAreaFunctionFactory.CreateFdaFunction(linkedFunction, type);
            IFunction computableInflowFreq = Sampler.Sample(inflowFrequency.Function, .5);

            Assert.True(true);

        }

    }
}
