using System;
using Model.Inputs.Functions;
using Model.Inputs.Functions.ImpactAreaFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModelTests.InputsTests.FunctionsTests.ImplementationsTests
{
    [TestClass()]
    public class ExteriorStageFrequencyTests
    {
        #region Fields and Properties
        private IFunctionBase defaultCurveIncreasing = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 0, 1 }, new double[] { 2, 3 }, true, false));
        private IFunctionBase defaultFrequencyOrdinates = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 0.01, 0.99 }, new double[] { 2, 3 }, true, false));
        //private Statistics.CurveIncreasing defaultTransformOrdinates = new Statistics.CurveIncreasing(new double[] { 2, 3 }, new double[] { 4, 5 }, true, false);
        #endregion 

        #region Validate()
        [TestMethod()]
        public void Validate_GoodOrdinatesReturnTrue()
        {
            ExteriorStageFrequency testExteriorStageFrequency = (ExteriorStageFrequency)ImpactAreaFunctionFactory.CreateNew(defaultFrequencyOrdinates, ImpactAreaFunctionEnum.ExteriorStageFrequency);
            Assert.IsTrue(testExteriorStageFrequency.IsValid);
        }

        [TestMethod()]
        public void Validate_BadProbabilitiesReturnsFalse()
        {
            ExteriorStageFrequency testExteriorStageFrequency = (ExteriorStageFrequency)ImpactAreaFunctionFactory.CreateNew(defaultCurveIncreasing, ImpactAreaFunctionEnum.ExteriorStageFrequency);
            Assert.IsFalse(testExteriorStageFrequency.IsValid);
        }
        #endregion
    }
}
