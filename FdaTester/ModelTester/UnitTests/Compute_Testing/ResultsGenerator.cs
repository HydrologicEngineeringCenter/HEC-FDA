using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaTester.ModelTester.UnitTests.Compute_Testing
{
    public class ResultsGenerator
    {
        #region Fields
        private FdaModel.ComputationPoint.Outputs.Result _TestResult;
        #endregion

        #region Property
        public FdaModel.ComputationPoint.Outputs.Result TestResult
        {
            get
            {
                return _TestResult;
            }
            private set
            {
                _TestResult = value;
            }
        }
        #endregion

        #region Constructor
        private ResultsGenerator(FdaModel.ComputationPoint.Outputs.Result testResult)
        {
            TestResult = testResult;
        }
        #endregion


        #region Functions
        public static ResultsGenerator GenerateResult()
        {
            //1. Test Functions
            Functions.FrequencyFunctionGenerator frequencyFunction = Functions.FrequencyFunctionGenerator.LogPearsonGenerator();
            Functions.OrdinatesFunctionGenerator stageFlowFunction = Functions.OrdinatesFunctionGenerator.FromFrequencyFunctionOrdinatesGenerator(frequencyFunction.LPIIIFunction, FdaModel.Functions.FunctionTypes.Rating);
            Functions.OrdinatesFunctionGenerator stageDamageFunction = Functions.OrdinatesFunctionGenerator.FromRelatedOrdinatesFunctionOrdinatesGenerator(stageFlowFunction.OrdinatesFunction, FdaModel.Functions.FunctionTypes.InteriorStageDamage);
            List<FdaModel.Functions.BaseFunction> computeFunctions = new List<FdaModel.Functions.BaseFunction> { frequencyFunction.LPIIIFunction, stageFlowFunction.OrdinatesFunction, stageDamageFunction.OrdinatesFunction };

            //2. Test Threshold
            Random randomNumberGenerator = new Random();
            int randomThresholdIndex = randomNumberGenerator.Next(0, stageDamageFunction.OrdinatesFunction.Function.Count);
            FdaModel.ComputationPoint.PerformanceThreshold threshold = new FdaModel.ComputationPoint.PerformanceThreshold(FdaModel.ComputationPoint.PerformanceThresholdTypes.Damage, stageDamageFunction.OrdinatesFunction.Function.get_Y(randomThresholdIndex));

            //3. Test Condition
            FdaModel.ComputationPoint.Condition testCondition = new FdaModel.ComputationPoint.Condition(DateTime.Now.Year, "A Location", computeFunctions, threshold);

            //4. Generate Result
            //FdaModel.ComputationPoint.Outputs.Realization testRealization = new FdaModel.ComputationPoint.Outputs.Realization(testCondition);
            //testRealization.Compute(randomNumberGenerator);

            return new ResultsGenerator(new FdaModel.ComputationPoint.Outputs.Result(testCondition, 1000));
        }

        #endregion

    }
}
