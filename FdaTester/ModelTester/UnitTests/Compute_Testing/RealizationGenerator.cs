using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaTester.ModelTester.UnitTests.Compute_Testing
{
    public class RealizationGenerator
    {

        #region Fields
        private FdaModel.ComputationPoint.Outputs.Realization _TestRealization;
        #endregion

        #region Property
        public FdaModel.ComputationPoint.Outputs.Realization TestRealization
        {
            get
            {
                return _TestRealization;
            }
            private set
            {
                _TestRealization = value;
            }
        }
        #endregion

        #region Constructor
        private RealizationGenerator(FdaModel.ComputationPoint.Outputs.Realization testRealization)
        {
            TestRealization = testRealization;
        }
        #endregion


        #region Functions
        public static RealizationGenerator GenerateRealization()
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
            return new RealizationGenerator(new FdaModel.ComputationPoint.Outputs.Realization(testCondition));
        }

        public static RealizationGenerator PullRealizationFromResult(int i)
        {
            ResultsGenerator testResult = ResultsGenerator.GenerateResult();
            return new RealizationGenerator(testResult.TestResult.Realizations[i]);
        }

        #endregion

    }
}
