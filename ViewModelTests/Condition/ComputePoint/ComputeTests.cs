using Functions;
using Functions.Ordinates;
using Model;
using Model.Condition.ComputePoint.ImpactAreaFunctions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace ModelTests.Condition.ComputePoint
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

            if(inflowFrequency != null && inflowOutflow != null)
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

    }
}
