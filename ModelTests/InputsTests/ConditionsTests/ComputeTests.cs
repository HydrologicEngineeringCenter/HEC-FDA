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
using Model.Outputs;
using Xunit;
using Xunit.Abstractions;

namespace ModelTests.InputsTests.ConditionsTests
{
    [ExcludeFromCodeCoverage]

    public class ComputeTests:ComputeTestData
    {
        //private readonly ITestOutputHelper output;
        ///// <summary>
        ///// This lets you write additional output statements.
        ///// </summary>
        ///// <param name="output"></param>
        //public ComputeTests(ITestOutputHelper output)
        //{
        //    this.output = output;
        //}

        //[Fact]
        //public void ComputeTest_1()
        //{
        //    Sampler.RegisterSampler(new ConstantSampler());

        //    IFrequencyFunction inflowFrequency = ComputeTestData.CreateInflowFrequencyFunction();
        //    ITransformFunction inflowOutflow = ComputeTestData.CreateInflowOutflowFunction();

        //    //List<IFdaFunction> funcs = new List<IFdaFunction>();
        //    //funcs.Add(inflowFrequency);
        //    //funcs.Add(inflowOutflow);

        //    //IFrequencyFunction inflowFreq = funcs.Where(func => func.Type == ImpactAreaFunctionEnum.InflowFrequency).FirstOrDefault();
        //    //IFdaFunction inOut = funcs.Where(func => func.Type == ImpactAreaFunctionEnum.InflowOutflow).FirstOrDefault();

        //    if (inflowFrequency != null && inflowOutflow != null)
        //    {
        //        IFrequencyFunction outflowFrequency = inflowFrequency.Compose(inflowOutflow, .5, .5);
        //        //IFunction func = Sampler.Sample(inflowFrequency.Function, .5);
        //        //IFunction func2 = Sampler.Sample(inflowOutflow.Function, .5);

        //        //OutputCoordinates(func, "inflowFreq");
        //        //OutputCoordinates(func2, "inflowOutflow");

        //        //IFunction composed = func.Compose(func2);

        //        //OutputCoordinates(composed, "Composed");

        //        //IFdaFunction outflowFrequency = ImpactAreaFunctionFactory.Factory(composed, ImpactAreaFunctionEnum.OutflowFrequency);
        //        //funcs.Add(outflowFrequency);

        //        Assert.True(outflowFrequency.Function.F(new Constant(1)).Value() == 5);
        //        Assert.True(outflowFrequency.Function.F(new Constant(2)).Value() == 6);
        //        Assert.True(outflowFrequency.Function.F(new Constant(3)).Value() == 7);

        //    }

        //}

        //private void OutputCoordinates(IFunction function, string funcName)
        //{
        //    output.WriteLine(funcName + " Coordinates:");
        //    foreach (ICoordinate coord in function.Coordinates)
        //    {
        //        output.WriteLine(coord.X.Value() + " |  " + coord.Y.Value());
        //    }
        //    output.WriteLine("");
        //}


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
            RegisterSamplers();


            string name = "name";
            string desc = "desc";
            int analysisYear = 1999;
            double thresholdValue = 200000000;
            int seed = 1;
            
            //create inflow freq function
            List<double> inflowFreqxs = new List<double>() { 0, .5, 1 };
            List<double> inflowFreqys = new List<double>() { 0, 10000, 100000 };
            IFrequencyFunction inflowFreq = CreateFrequencyFunction(inflowFreqxs, inflowFreqys, InterpolationEnum.Linear, IParameterEnum.InflowFrequency);

            //create list of transform functions
            List<ITransformFunction> transformFunctions = new List<ITransformFunction>();

            //create rating curve
            List<double> ratFlows = new List<double>() { 0, 100, 10000, 100000 };
            List<double> ratStages = new List<double>() { 0, 1, 10, 100 };
            transformFunctions.Add(CreateTransformFunction(ratFlows, ratStages, InterpolationEnum.Linear, IParameterEnum.Rating));

            //create interior stage damage transform function
            List<double> intStages = new List<double>() { 0, 1, 10, 100 };
            List<double> damage = new List<double>() { 0, 2000000, 200000000, 2000000000 };
            transformFunctions.Add(CreateTransformFunction(intStages, damage, InterpolationEnum.Linear, IParameterEnum.InteriorStageDamage));

            //create the metrics
            List<IMetric> metrics = CreateMetrics(new List<IMetricEnum>() { IMetricEnum.Damages }, new List<double>() { thresholdValue });

            ICondition condition = ConditionFactory.Factory(name, analysisYear, inflowFreq, transformFunctions,metrics);

            int randomPacketSize = transformFunctions.Count + 1;
            //TimeStampSeed = (int)new DateTime().Ticks;
           

            IDictionary<IMetric, double> retval =  condition.Compute(GetRandomNumbers(randomPacketSize));

            double exceedanceProb = retval[metrics[0]];
            Assert.True( exceedanceProb == .5);
       
        }

        private List<double> GetRandomNumbers(int numberOfRandomNumbers)
        {
            int Seed = 1;
            List<double> randomNumbers = new List<double>();
            
            Random randomNumberGenerator = new Random(Seed);
            for (int k = 0; k < numberOfRandomNumbers; k++)
            {
                randomNumbers.Add(randomNumberGenerator.NextDouble());
            }
            return randomNumbers;
        }
    }
}
