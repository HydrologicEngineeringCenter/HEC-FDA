using System;
using Model.Inputs.Functions;
using Model.Inputs.Functions.ImpactAreaFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModelTests.InputsTests.FunctionsTests.ImplementationsTests
{
    [TestClass()]
    public class InflowFrequencyTests
    {
        #region Fields and Properties
        private OrdinatesFunction testOrdinates = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 1, 2 }, new double[] { 3, 4 }, true, false));
        private IFunctionCompose testInflowFrequency = ImpactAreaFunctionFactory.CreateNew(new FrequencyFunction(new Statistics.LogPearsonIII(1, 0.1, 0.1, 100)));
        #endregion

        #region Constructor Tests
        [TestMethod()]
        public void InflowFrequency_FunctionFactoryCreatesInflowFrequencyFunction()
        {
            Assert.AreEqual(typeof(InflowFrequency), testInflowFrequency.GetType());
        }
        #endregion

        #region Compose() Tests
        [TestMethod()]
        public void Compose_InflowOutflowInputReturnsOutflowFrequency()
        {
            IFunctionCompose actual = testInflowFrequency.Compose(ImpactAreaFunctionFactory.CreateNew(testOrdinates, testOrdinates.Ordinates, ImpactAreaFunctionEnum.InflowOutflow));
            Assert.AreEqual(typeof(OutflowFrequency), actual.GetType());
        }
        [TestMethod()]
        public void Compose_RatingInputReturnsExteriorStageFrequency()
        {
            IFunctionCompose actual = testInflowFrequency.Compose(ImpactAreaFunctionFactory.CreateNew(testOrdinates, testOrdinates.Ordinates, ImpactAreaFunctionEnum.Rating));
            Assert.AreEqual(typeof(ExteriorStageFrequency), actual.GetType());
        }
        [TestMethod()]
        public void Compose_RatingInputConvertsUseTypeToOutflowFrequency()
        {
            InflowFrequency inflowFrequencyCopy = (InflowFrequency)testInflowFrequency;
            IFunctionCompose actual = inflowFrequencyCopy.Compose(ImpactAreaFunctionFactory.CreateNew(testOrdinates, testOrdinates.Ordinates, ImpactAreaFunctionEnum.Rating));
            Assert.AreEqual(ImpactAreaFunctionEnum.OutflowFrequency, inflowFrequencyCopy.UseType);
        }

        [TestMethod()]
        public void Compose_InvalidTransformTypeInputReturnsNull()
        {

        }
        #endregion

        #region Validate() Tests
        [TestMethod()]
        public void Validate_GoodFrequencyFunctionReturnsTrue()
        {
            Assert.AreEqual(testInflowFrequency.IsValid, true);
        }

        [TestMethod()]
        public void Validate_NoFrequencyFunctionReturnsFalse()
        {
            OrdinatesFunction testOrdinatesFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 0, 1 }, new double[] { 2, 3 }, true, false));
            InflowFrequency testInflowFrequency = new InflowFrequency(testOrdinatesFunction); //shouldn't construct this way.
            Assert.AreEqual(testInflowFrequency.IsValid, false);
        }
        #endregion
    }
}
