using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Inputs.Functions;
using Model.Inputs.Functions.ImpactAreaFunctions;

namespace ModelTests.InputsTests.FunctionsTests.ImplementationsTests
{
    [TestClass()]
    public class UnUsedTests
    {
        [TestMethod()]
        public void UnUsed_NoTargetTypeReturnsNotSetTwice()
        {
            IFunctionBase testOrdinates = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 0, 1 }, new double[] { 2, 3 }, true, false));
            UnUsed testUnUsed = new UnUsed(testOrdinates, ImpactAreaFunctionEnum.NotSet);
            Assert.AreEqual(testUnUsed.Type, testUnUsed.TargetType);
        }
        [TestMethod()]
        public void UnUsed_TargetTypeReturnsTargetTypeAndTypeNotSet()
        {
            IFunctionBase testOrdinates = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 0, 1 }, new double[] { 2, 3 }, true, false));
            UnUsed testUnUsed = new UnUsed(testOrdinates, ImpactAreaFunctionEnum.InflowFrequency);
            Assert.AreNotEqual(testUnUsed.Type, testUnUsed.TargetType);
        }
    }
}
