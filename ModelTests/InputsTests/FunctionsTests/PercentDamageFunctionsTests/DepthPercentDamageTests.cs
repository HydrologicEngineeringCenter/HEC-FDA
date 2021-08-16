using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Inputs.Functions;
using Model.Inputs.Functions.PercentDamageFunctions;

namespace ModelTests.InputsTests.FunctionsTests.PercentDamageFunctionsTests
{
    [TestClass()]
    public class DepthPercentDamageTests
    {
        #region Sample() Tests
        [TestMethod()]
        public void Sample_GoodDataUnchanged()
        {
            double[] xs = new double[3] { 0, 1, 2 }, ys = new double[] { 0, 0.5, 1 };
            IPercentDamageFunction testFunction = new DepthPercentDamage(new OrdinatesFunction(new Statistics.CurveIncreasing(xs, ys, true, true)));
            testFunction.Sample(0.5);
            //CollectionAssert.AreEqual(testFunction, testFunction.Sample(0.5));
        }

        #endregion

        #region Validate() Tests
        [TestMethod()]
        public void Validate_GoodDataReturnsTrue()
        {
            IFunctionBase testFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[3] { 0, 1, 2 }, new double[3] { 0, 0.5, 1 }, true, false));
            DepthPercentDamage testObject = new DepthPercentDamage(testFunction);
            Assert.IsTrue(testObject.Validate());
        }
        [TestMethod()]
        public void Validate_NotIncreasingXsReturnsFalse()
        {
            IFunctionBase testFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[3] { 2, 1, 3 }, new double[3] { 0, 0.5, 1 }, true, false));
            DepthPercentDamage testObject = new DepthPercentDamage(testFunction);
            Assert.IsFalse(testObject.Validate());
        }
        [TestMethod()]
        public void Validate_BadXsReturnsFalse()
        {
            IFunctionBase testFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[2] { 1, 1 }, new double[2] { 0, 0.5 }, true, false));
            DepthPercentDamage testObject = new DepthPercentDamage(testFunction);
            Assert.IsFalse(testObject.Validate());
        }
        [TestMethod()]
        public void Validate_NegativeYReturnsFalse()
        {
            IFunctionBase testFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[3] { 0, 1, 2 }, new double[3] { -0.1, 0.5, 1 }, true, false));
            DepthPercentDamage testObject = new DepthPercentDamage(testFunction);
            Assert.IsFalse(testObject.Validate());
        }
        [TestMethod()]
        public void Validate_YGreaterThan1ReturnsFalse()
        {
            IFunctionBase testFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[2] { 0, 1 }, new double[2] { 0, 1.1 }, true, false));
            DepthPercentDamage testObject = new DepthPercentDamage(testFunction);
            Assert.IsFalse(testObject.Validate());
        }
        #endregion

    }
}
