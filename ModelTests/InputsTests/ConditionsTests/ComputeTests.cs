using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Functions;
using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using Model;
using Model.Inputs.Functions.ImpactAreaFunctions;
using Model.Outputs;
using Xunit;
using Xunit.Abstractions;

namespace ModelTests.InputsTests.ConditionsTests
{
    [ExcludeFromCodeCoverage]

    public class ComputeTests
    {
        private readonly ITestOutputHelper output;
        /// <summary>
        /// This lets you write additional output statements.
        /// </summary>
        /// <param name="output"></param>
        public ComputeTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void ComputeTest_1()
        {
            Sampler.RegisterSampler(new ConstantSampler());

            IFrequencyFunction inflowFrequency = ComputeTestData.CreateInflowFrequencyFunction();
            ITransformFunction inflowOutflow = ComputeTestData.CreateInflowOutflowFunction();

            //List<IFdaFunction> funcs = new List<IFdaFunction>();
            //funcs.Add(inflowFrequency);
            //funcs.Add(inflowOutflow);

            //IFrequencyFunction inflowFreq = funcs.Where(func => func.Type == ImpactAreaFunctionEnum.InflowFrequency).FirstOrDefault();
            //IFdaFunction inOut = funcs.Where(func => func.Type == ImpactAreaFunctionEnum.InflowOutflow).FirstOrDefault();

            if (inflowFrequency != null && inflowOutflow != null)
            {
                IFrequencyFunction outflowFrequency = inflowFrequency.Compose(inflowOutflow, .5, .5);
                //IFunction func = Sampler.Sample(inflowFrequency.Function, .5);
                //IFunction func2 = Sampler.Sample(inflowOutflow.Function, .5);

                //OutputCoordinates(func, "inflowFreq");
                //OutputCoordinates(func2, "inflowOutflow");

                //IFunction composed = func.Compose(func2);

                //OutputCoordinates(composed, "Composed");

                //IFdaFunction outflowFrequency = ImpactAreaFunctionFactory.Factory(composed, ImpactAreaFunctionEnum.OutflowFrequency);
                //funcs.Add(outflowFrequency);

                Assert.True(outflowFrequency.Function.F(new Constant(1)).Value() == 5);
                Assert.True(outflowFrequency.Function.F(new Constant(2)).Value() == 6);
                Assert.True(outflowFrequency.Function.F(new Constant(3)).Value() == 7);

            }

        }

        private void OutputCoordinates(IFunction function, string funcName)
        {
            output.WriteLine(funcName + " Coordinates:");
            foreach (ICoordinate coord in function.Coordinates)
            {
                output.WriteLine(coord.X.Value() + " |  " + coord.Y.Value());
            }
            output.WriteLine("");
        }


        //private ImpactAreaElement CreateImpactArea()
        //{
        //    ObservableCollection<ImpactAreaRowItem> rows = new ObservableCollection<ImpactAreaRowItem>();
        //    string name = "imp name";
        //    string desc = "imp desc";

        //    for (int i = 0; i < 10; i++)
        //    {
        //        double indPoint = i;
        //        ImpactAreaRowItem row = new ImpactAreaRowItem(name, indPoint, new ObservableCollection<object>());
        //        rows.Add(row);

        //    }
        //    return new ImpactAreaElement(name, desc, rows);
        //}

        //private ImpactAreaRowItem CreateImpactAreaRowItem(double value)
        //{
        //    return new ImpactAreaRowItem("name", value, new ObservableCollection<object>());
        //}

        [Fact]
        public void ComputeTest()
        {
            //register the samplers
            Sampler.RegisterSampler(new ConstantSampler());
            Sampler.RegisterSampler(new DistributionSampler());
            Sampler.RegisterSampler(new LinkedFunctionsSampler());


            string name = "name";
            string desc = "desc";
            int analysisYear = 1999;
            double thresholdValue = 200000000;
            int seed = 1;

            //ConditionBuilder builder = new ConditionBuilder(name, desc, analysisYear, CreateImpactArea(), CreateImpactAreaRowItem(2),
            //    Model.MetricEnum.InteriorStage, thresholdValue);

            //ConditionsElement conditionElem = builder.build();
            
            //create inflow freq function
            List<double> LP3xs = new List<double>() { 0, .5, 1 };
            List<double> LP3ys = new List<double>() { 100000, 10000, 0 };
            ICoordinatesFunction lpsCoordFunc = ICoordinatesFunctionsFactory.Factory(LP3xs, LP3ys, InterpolationEnum.Linear);
            IFrequencyFunction lp3FreqFunc = (IFrequencyFunction)ImpactAreaFunctionFactory.Factory(lpsCoordFunc, ImpactAreaFunctionEnum.InflowFrequency);

            //create list of transform functions
            List<ITransformFunction> transformFunctions = new List<ITransformFunction>();
            //create rating curve
            List<double> ratFlows = new List<double>() { 0, 100, 10000, 100000 };
            List<double> ratStages = new List<double>() { 0, 1, 10, 100 };
            ICoordinatesFunction ratCoordFunc = ICoordinatesFunctionsFactory.Factory( ratFlows, ratStages, InterpolationEnum.Linear);
            ITransformFunction ratTransFunc = (ITransformFunction)ImpactAreaFunctionFactory.Factory(ratCoordFunc, ImpactAreaFunctionEnum.Rating);
            transformFunctions.Add(ratTransFunc);

            //create interior stage damage transform function
            List<double> intStages = new List<double>() { 0, 1, 10, 100 };
            List<double> damage = new List<double>() { 0, 2000000, 200000000, 2000000000 };
            ICoordinatesFunction intStageDamage = ICoordinatesFunctionsFactory.Factory(intStages, damage, InterpolationEnum.Linear);
            ITransformFunction intStageTransFunc = (ITransformFunction)ImpactAreaFunctionFactory.Factory(intStageDamage, ImpactAreaFunctionEnum.InteriorStageDamage);
            transformFunctions.Add(intStageTransFunc);


            //create the metrics
            List<IMetric> metrics = new List<IMetric>();
            IMetric damageMetric = new Metric(MetricEnum.Damages, thresholdValue);
            metrics.Add( damageMetric);

            ICondition condition = ConditionFactory.Factory(name, analysisYear, lp3FreqFunc, transformFunctions,metrics);
            IRealization realization = condition.Compute(seed);

            double exceedanceProb = realization.Metrics[damageMetric];
            Assert.True( exceedanceProb == .5);
            //ICondition condition = conditionElem.CreateCondition();
            //int seed = 1;
            //IRealization realization = condition.Compute(seed);
        }
    }
}
