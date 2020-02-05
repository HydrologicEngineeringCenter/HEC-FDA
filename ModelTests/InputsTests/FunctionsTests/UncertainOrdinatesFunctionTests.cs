using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Inputs.Functions;

namespace ModelTests.InputsTests.FunctionsTests
{
    [TestClass()]
    public class UncertainOrdinatesFunctionTests
    {
        #region Fields
        internal UncertainOrdinatesFunction defaultNormalFunction = new UncertainOrdinatesFunction(new Statistics.UncertainCurveIncreasing(new double[] { 1, 2 }, new Statistics.ContinuousDistribution[] { new Statistics.Normal(0, 1), new Statistics.Normal(0, 1) }, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal));
        internal UncertainOrdinatesFunction defaultTriangularFunction = new UncertainOrdinatesFunction(new Statistics.UncertainCurveIncreasing(new double[] { 1, 2 }, new Statistics.ContinuousDistribution[] { new Statistics.Triangular(-1, 1, 0), new Statistics.Triangular(-1, 1, 0) }, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular));
        internal UncertainOrdinatesFunction defaultUniformFunction = new UncertainOrdinatesFunction(new Statistics.UncertainCurveIncreasing(new double[] { 1, 2 }, new Statistics.ContinuousDistribution[] { new Statistics.Uniform(0, 1), new Statistics.Uniform(0, 1) }, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform));
        #endregion

        [TestMethod()]
        public void SampleFunction_Normal50PercentNormalDistributionReturnsCentralTendency()
        {
            List<Tuple<double, double>> testSampledOrdinates = new List<Tuple<double, double>>(defaultNormalFunction.Sample(0.50d).GetOrdinates());
            List<Tuple<double, double>> testCentralOrdinates = new List<Tuple<double, double>>(defaultNormalFunction.GetOrdinates());
            CollectionAssert.AreEqual(testCentralOrdinates, testSampledOrdinates);
        }
        [TestMethod()]
        public void SampleFunction_NormalExtremeLargeValueDoesNOTReturnBadResult()
        {
            IFunctionBase testSampledFunction = defaultNormalFunction.Sample(0.9999);
            Assert.IsTrue(testSampledFunction.IsValid);
        }
        [TestMethod()]
        public void SampleFunction_NormalExtremeSmallValueDoesNOTReturnBadResult()
        {
            IFunctionBase testSampledFunction = defaultNormalFunction.Sample(0.0001);
            Assert.IsTrue(testSampledFunction.IsValid);
        }
        [TestMethod()]
        public void SampleFunction_NormalTooBigBadInputReturnsMAXlessthan8SDs()
        {
            double expectedValue = 8;
            IFunctionBase testSampledFunction = defaultNormalFunction.Sample(2);
            IList<Tuple<double, double>> actualOrdinates = testSampledFunction.GetOrdinates();

            bool hasFailed = false;
            foreach (var ordinate in actualOrdinates)
            {
                if (ordinate.Item2 > expectedValue) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void SampleFunction_NormalTooSmallBadInputReturnsMINlessthan8SDs()
        {
            double expectedValue = -8;
            IFunctionBase testSampledFunction = defaultNormalFunction.Sample(-1);
            IList<Tuple<double, double>> actualOrdinates = testSampledFunction.GetOrdinates();

            bool hasFailed = false;
            foreach (var ordinate in actualOrdinates)
            {
                if (ordinate.Item2 < expectedValue) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void SampleFunction_NormalPlus1SDReturnsGoodValue()
        {
            double expectedValue = 1;
            double tolerance = 0.05;
            IFunctionBase testSampledFunction = defaultNormalFunction.Sample(0.839);
            IList<Tuple<double, double>> actualOrdinates = testSampledFunction.GetOrdinates();

            bool hasFailed = false;
            foreach (var ordinate in actualOrdinates)
            {
                if (Math.Abs(ordinate.Item2 - expectedValue) > tolerance) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void SampleFunction_TriangularMostLikelyValueReturnsCentralTendency()
        {
            double expectedValue = 0;
            IFunctionBase testSampledFunction = defaultTriangularFunction.Sample(0.5);
            IList<Tuple<double, double>> actualOrdinates = testSampledFunction.GetOrdinates();

            bool hasFailed = false;
            foreach (var ordinate in actualOrdinates)
            {
                if (!(ordinate.Item2 == expectedValue)) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void SampleFunction_TriangularInputAboveMaxReturnsXXX()
        {
            double expectedValue = 1;
            IFunctionBase testSampledFunction = defaultTriangularFunction.Sample(2);
            IList<Tuple<double, double>> actualOrdinates = testSampledFunction.GetOrdinates();

            bool hasFailed = false;
            foreach (var ordinate in actualOrdinates)
            {
                if (!(ordinate.Item2 == expectedValue)) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void SampleFunction_TriangularTooSmallInputReturnsMin()
        {
            double expectedValue = -1;
            IFunctionBase testSampledFunction = defaultTriangularFunction.Sample(-10);
            IList<Tuple<double, double>> actualOrdinates = testSampledFunction.GetOrdinates();

            bool hasFailed = false;
            foreach (var ordinate in actualOrdinates)
            {
                if (!(ordinate.Item2 == expectedValue)) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void SampleFunction_TriagularInbetweenInputReturnsInterpolatedValue()
        {
            double expectedValue = 0.50;
            double expectedProbability = 1 - (1 - expectedValue) * (1 - expectedValue) / ((1 - -1) * (1 - 0)); //Function for CDF
            IFunctionBase testSampledFunction = defaultTriangularFunction.Sample(expectedProbability);
            IList<Tuple<double, double>> actualOrdinates = testSampledFunction.GetOrdinates();

            bool hasFailed = false;
            foreach (var ordinate in actualOrdinates)
            {
                if (!(ordinate.Item2 == expectedValue)) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void SampleFunction_UniformMedianInputReturnsCentralTendency()
        {
            double expectedValue = 0.50;
            IFunctionBase testSampledFunction = defaultUniformFunction.Sample(0.50);
            IList<Tuple<double, double>> actualOrdinates = testSampledFunction.GetOrdinates();

            bool hasFailed = false;
            foreach (var ordinate in actualOrdinates)
            {
                if (!(ordinate.Item2 == expectedValue)) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void SampleFunction_UniformTooSmallInputReturnsMin()
        {
            double expectedValue = 0;
            IFunctionBase testSampledFunction = defaultUniformFunction.Sample(-1);
            IList<Tuple<double, double>> actualOrdinates = testSampledFunction.GetOrdinates();

            bool hasFailed = false;
            foreach (var ordinate in actualOrdinates)
            {
                if (!(ordinate.Item2 == expectedValue)) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void SampleFunction_UniformTooBigInputReturnsMax()
        {
            double expectedValue = 1;
            IFunctionBase testSampledFunction = defaultUniformFunction.Sample(2);
            IList<Tuple<double, double>> actualOrdinates = testSampledFunction.GetOrdinates();

            bool hasFailed = false;
            foreach (var ordinate in actualOrdinates)
            {
                if (!(ordinate.Item2 == expectedValue)) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void SampleFunction_UniformInbetweenInputReturnsGoodValue()
        {
            double expectedValue = 0.75;
            IFunctionBase testSampledFunction = defaultUniformFunction.Sample(0.75);
            IList<Tuple<double, double>> actualOrdinates = testSampledFunction.GetOrdinates();

            bool hasFailed = false;
            foreach (var ordinate in actualOrdinates)
            {
                if (!(ordinate.Item2 == expectedValue)) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
    }
}
