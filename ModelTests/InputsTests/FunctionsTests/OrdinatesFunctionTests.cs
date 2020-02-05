using System;
using Model.Inputs.Functions;
using Model.Inputs.Functions.ImpactAreaFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ModelTests.InputsTests.FunctionsTests
{
    [TestClass()]
    public class OrdinatesFunctionTests
    {
        #region Fields and Properties
        private Statistics.CurveIncreasing defaultCurveIncreasing = new Statistics.CurveIncreasing(new double[] { 0, 1 }, new double[] { 2, 3 }, true, false);
        private Statistics.CurveIncreasing defaultFrequencyOrdinates = new Statistics.CurveIncreasing(new double[] { 0.01, 0.99 }, new double[] { 2, 3 }, true, false);
        private Statistics.CurveIncreasing defaultTransformOrdinates = new Statistics.CurveIncreasing(new double[] { 2, 3 }, new double[] { 4, 5 }, true, false);
        #endregion

        #region GetOrdinates() Test
        [TestMethod()]
        public void GetOrdinates_FunctionsAsExpected()
        {
            OrdinatesFunction testObject = new OrdinatesFunction(defaultCurveIncreasing);
            List<Tuple<double, double>> expected = new List<Tuple<double, double>> { new Tuple<double, double>(0, 2), new Tuple<double, double>(1, 3) };
            CollectionAssert.AreEqual(expected, (List<Tuple<double, double>>)testObject.GetOrdinates());
        }
        #endregion

        #region Validate() Tests
        [TestMethod()]
        public void Validate_GoodDataReturnsTrue()
        {
            OrdinatesFunction testOrdinatesFunction = new OrdinatesFunction(defaultCurveIncreasing);
            Assert.IsTrue(testOrdinatesFunction.IsValid);
        }
        [TestMethod()]
        public void Validate_SingleOrdinateReturnsFalse()
        {
            OrdinatesFunction testOrdinatesFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 1 }, new double[] { 2 }, true, false));
            Assert.IsFalse(testOrdinatesFunction.IsValid);
        }
        [TestMethod()]
        public void Validate_RepeatingOrdinatesRemoved()
        {
            Statistics.CurveIncreasing testFunction = new Statistics.CurveIncreasing(new double[] { 0, 1, 1 }, new double[] { 2, 3, 3 }, true, false);
            OrdinatesFunction actualOrdinatesFunction = new OrdinatesFunction(testFunction);
            OrdinatesFunction expectOrdinatesFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 0, 1 }, new double[] { 2, 3 }, true, false));
            CollectionAssert.AreEqual((List<Tuple<double, double>>)expectOrdinatesFunction.Ordinates, (List<Tuple<double, double>>)actualOrdinatesFunction.Ordinates);
        }

        [TestMethod()]
        public void Validate_RepeatingOrdinatesRemovedReturnsTrue()
        {
            OrdinatesFunction actualOrdinatesFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 0, 1, 1 }, new double[] { 2, 3, 3 }, true, false));
            Assert.IsTrue(actualOrdinatesFunction.IsValid);
        }

        [TestMethod()]
        public void Validate_TooManyRepeatingOrdinatesRemovedReturnsFalse()
        {
            OrdinatesFunction testOrdinatesFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[3] { 0, 0, 0 }, new double[3] { 2, 2, 2 }, true, false));
            Assert.IsFalse(testOrdinatesFunction.IsValid);
            Assert.AreEqual(1, testOrdinatesFunction.Ordinates.Count);
        }
        #endregion

        #region ValidateFrequencyValues() Tests
        [TestMethod()]
        public void ValidateFrequencyValues_GoodValuesReturnTrue()
        {
            OrdinatesFunction testOrdinatesFunction = new OrdinatesFunction(defaultFrequencyOrdinates);
            Assert.IsTrue(testOrdinatesFunction.ValidateFrequencyValues());
        }

        [TestMethod()]
        public void ValidateFrequencyValues_BadValuesReturnsFalse()
        {
            OrdinatesFunction testOrdinatesFunction = new OrdinatesFunction(defaultCurveIncreasing);
            Assert.IsFalse(testOrdinatesFunction.ValidateFrequencyValues());
        }
        #endregion

        #region GetXfromY() Tests
        [TestMethod()]
        public void GetXfromY_TinyYReturnsSmallestX()
        {
            double y = -1;
            IFunctionBase testOrdinatesFunction = new OrdinatesFunction(defaultCurveIncreasing);
            Assert.AreEqual(0, testOrdinatesFunction.GetXfromY(y));
        }

        public void GetXfromY_SmallestYReturnsSmallestX()
        {
            double y = 2;
            IFunctionBase testOrdinatesFunction = new OrdinatesFunction(defaultCurveIncreasing);
            Assert.AreEqual(0, testOrdinatesFunction.GetXfromY(y));
        }

        [TestMethod()]
        public void GetXfromY_GiantYReturnsLargestX()
        {
            double y = 100;
            IFunctionBase testOrdinatesFunction = new OrdinatesFunction(defaultCurveIncreasing);
            Assert.AreEqual(1, testOrdinatesFunction.GetXfromY(y));
        }

        [TestMethod()]
        public void GetXfromY_LargestYReturnsLargestX()
        {
            double y = 3;
            IFunctionBase testOrdinatesFunction = new OrdinatesFunction(defaultCurveIncreasing);
            Assert.AreEqual(1, testOrdinatesFunction.GetXfromY(y));
        }

        [TestMethod()]
        public void GetXfromY_ReturnsInterpolatedXForInbetweenY()
        {
            double y = 2.5;
            IFunctionBase testOrdinatesFunction = new OrdinatesFunction(defaultCurveIncreasing);
            Assert.AreEqual(0.5, testOrdinatesFunction.GetXfromY(y));
        }

        [TestMethod()]
        public void GetXfromY_FindsRightInterpolationOrdinatesForInbetweenY()
        {
            //Arrange
            double y = 7.3;
            double[] testXs = new double[5] { 0, 1, 2, 3, 4 }, testYs = new double[5] { 5, 6, 7, 8, 9 };
            Statistics.CurveIncreasing testCurveIncreasing = new Statistics.CurveIncreasing(testXs, testYs, true, false);
            IFunctionBase testOrdinatesFunction = new OrdinatesFunction(testCurveIncreasing);
            Assert.AreEqual(2.3, testOrdinatesFunction.GetXfromY(y));
        }

        [TestMethod()]
        public void GetXfromY_FindsRightInterpolationOrdinatesForMatchingYs()
        {
            //Arrange
            double y = 8;
            double[] testXs = new double[5] { 0, 1, 2, 3, 4 }, testYs = new double[5] { 5, 6, 7, 8, 9 };
            Statistics.CurveIncreasing testCurveIncreasing = new Statistics.CurveIncreasing(testXs, testYs, true, false);
            IFunctionBase testOrdinatesFunction = new OrdinatesFunction(testCurveIncreasing);
            Assert.AreEqual(3, testOrdinatesFunction.GetXfromY(y));
        }
        #endregion

        #region GetYfromX() Tests
        [TestMethod()]
        public void GetYfromX_TinyXReturnsSmallestY()
        {
            double x = -1;
            IFunctionBase testOrdinatesFunction = new OrdinatesFunction(defaultCurveIncreasing);
            Assert.AreEqual(2, testOrdinatesFunction.GetYfromX(x));
        }
        [TestMethod()]
        public void GetYfromX_SmallestXReturnsSmallestY()
        {
            double x = 0;
            IFunctionBase testOrdinatesFunction = new OrdinatesFunction(defaultCurveIncreasing);
            Assert.AreEqual(2, testOrdinatesFunction.GetYfromX(x));
        }

        [TestMethod()]
        public void GetYfromX_GiantXReturnsLargestY()
        {
            double x = 100;
            IFunctionBase testOrdinatesFunction = new OrdinatesFunction(defaultCurveIncreasing);
            Assert.AreEqual(3, testOrdinatesFunction.GetYfromX(x));
        }

        [TestMethod()]
        public void GetYfromX_LargestXReturnsLargestY()
        {
            double x = 1;
            IFunctionBase testOrdinatesFunction = new OrdinatesFunction(defaultCurveIncreasing);
            Assert.AreEqual(3, testOrdinatesFunction.GetYfromX(x));
        }

        [TestMethod()]
        public void GetYfromX_InbetweenXReturnsInterpolatedY()
        {
            double x = 0.5;
            IFunctionBase testOrdinatesFunction = new OrdinatesFunction(defaultCurveIncreasing);
            Assert.AreEqual(2.5, testOrdinatesFunction.GetYfromX(x));
        }

        [TestMethod()]
        public void GetYfromX_InbetweenXReturnsRightInterpolationOrdinatesY()
        {
            //Arrange
            double x = 2.3;
            double[] testXs = new double[5] { 0, 1, 2, 3, 4 }, testYs = new double[5] { 5, 6, 7, 8, 9 };
            Statistics.CurveIncreasing testCurveIncreasing = new Statistics.CurveIncreasing(testXs, testYs, true, false);
            IFunctionBase testOrdinatesFunction = new OrdinatesFunction(testCurveIncreasing);
            Assert.AreEqual(7.3, testOrdinatesFunction.GetYfromX(x));
        }

        [TestMethod()]
        public void GetYfromX_MatchingXsReturnsRightInterpolationOrdinatesY()
        {
            //Arrange
            double x = 3;
            double[] testXs = new double[5] { 0, 1, 2, 3, 4 }, testYs = new double[5] { 5, 6, 7, 8, 9 };
            Statistics.CurveIncreasing testCurveIncreasing = new Statistics.CurveIncreasing(testXs, testYs, true, false);
            IFunctionBase testOrdinatesFunction = new OrdinatesFunction(testCurveIncreasing);
            Assert.AreEqual(8, testOrdinatesFunction.GetYfromX(x));
        }
        #endregion

        #region Compose() Tests

        [TestMethod()]
        public void Compose_LowerTransformNoOverlapReturns_DoubleNaNYValueOrdinates()
        {
            // 0, 1 => 2, 3 || 0, 1 => 4, 5
            OrdinatesFunction testOrdinates = new OrdinatesFunction(defaultCurveIncreasing);
            OrdinatesFunction testTransform = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 0, 1 }, new double[] { 4, 5 }, true, false));
            List<Tuple<double, double>> actualResult = (List<Tuple<double, double>>)testOrdinates.Compose(testTransform.Ordinates);
            List<Tuple<double, double>> expectedResult = new List<Tuple<double, double>>() { new Tuple<double, double>(0, double.NaN),
                                                                                             new Tuple<double, double>(1, double.NaN) };
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod()]
        public void Compose_LowerTransformWithPerfectMatchReturns_MatchedOverlappingOrdinates()
        {
            // 0, 1 => 2, 3 || 1, 2, 3 => 3, 4, 5
            OrdinatesFunction testOrdinates = new OrdinatesFunction(defaultCurveIncreasing);
            OrdinatesFunction testTransform = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 1, 2, 3 }, new double[] { 3, 4, 5 }, true, false));
            List<Tuple<double, double>> actualResult = (List<Tuple<double, double>>)testOrdinates.Compose(testTransform.Ordinates);
            List<Tuple<double, double>> expectedResult = new List<Tuple<double, double>>() { new Tuple<double, double>(0, 4),
                                                                                             new Tuple<double, double>(1, 5) };
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod()]
        public void Compose_HigherTransformNoOverlapReturns_DoubleNaNYValueOrdinates()
        {
            // 0, 1 => 2, 3 || 4, 5 => 6, 7
            OrdinatesFunction testOrdinates = new OrdinatesFunction(defaultCurveIncreasing);
            OrdinatesFunction testTransform = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 4, 5 }, new double[] { 4, 5 }, true, false));
            List<Tuple<double, double>> actualResult = (List<Tuple<double, double>>)testOrdinates.Compose(testTransform.Ordinates);
            List<Tuple<double, double>> expectedResult = new List<Tuple<double, double>>() { new Tuple<double, double>(0, double.NaN),
                                                                                             new Tuple<double, double>(1, double.NaN) };
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod()]
        public void Compose_HigherTransformWithPerfectMatchReturns_MatchedOverlappingOrdinates()
        {
            //0, 1 => 2, 3 || 2, 3, 4 => 4, 5, 6
            OrdinatesFunction testOrdinates = new OrdinatesFunction(defaultCurveIncreasing);
            OrdinatesFunction testTransform = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 2, 3, 4 }, new double[] { 4, 5, 6 }, true, false));
            List<Tuple<double, double>> actualResult = (List<Tuple<double, double>>)testOrdinates.Compose(testTransform.Ordinates);
            List<Tuple<double, double>> expectedResult = new List<Tuple<double, double>>() { new Tuple<double, double>(0, 4),
                                                                                             new Tuple<double, double>(1, 5) };
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod()]
        public void Compose_PerfectMatchReturns_MatchedTransformedOrdinates()
        {
            // 0, 1 => 2, 3 || 2, 3 => 4, 5
            OrdinatesFunction testOrdinates = new OrdinatesFunction(defaultCurveIncreasing);
            OrdinatesFunction testTransform = new OrdinatesFunction(defaultTransformOrdinates);
            List<Tuple<double, double>> actualResult = (List<Tuple<double, double>>)testOrdinates.Compose(testTransform.Ordinates);
            List<Tuple<double, double>> expectedResult = new List<Tuple<double, double>>() { new Tuple<double, double>(0, 4),
                                                                                             new Tuple<double, double>(1, 5) };
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod()]
        public void Compose_PerfectMatchPlusExtraTranformPointReturns_AllOrdinates()
        {
            // 0, 1 => 2, 3 || 2, 2.25, 2.50, 2.75, 3 => 4, 4.25, 4.50, 4.75, 5
            OrdinatesFunction testOrdinates = new OrdinatesFunction(defaultCurveIncreasing);
            OrdinatesFunction testTransform = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 2, 2.25, 2.5, 2.75, 3 }, new double[] { 4, 4.25, 4.5, 4.75, 5 }, true, false));
            List<Tuple<double, double>> actualResult = (List<Tuple<double, double>>)testOrdinates.Compose(testTransform.Ordinates);
            List<Tuple<double, double>> expectedResult = new List<Tuple<double, double>>() { new Tuple<double, double>(0, 4),
                                                                                             new Tuple<double, double>(0.25, 4.25),
                                                                                             new Tuple<double, double>(0.5, 4.5),
                                                                                             new Tuple<double, double>(0.75, 4.75),
                                                                                             new Tuple<double, double>(1, 5) };
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod()]
        public void Compose_InBetweenPerfectMatchReturns_MatchedTransformedOrdinatesPlusConstants()
        {
            OrdinatesFunction testOrdinates = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 0, 0.25, 0.5, 0.75, 1 }, new double[] { 2, 3, 4, 5, 6 }, true, false));
            OrdinatesFunction testTransform = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 3, 4, 5 }, new double[] { 7, 8, 9 }, true, false));
            List<Tuple<double, double>> actualResult = (List<Tuple<double, double>>)testOrdinates.Compose(testTransform.Ordinates);
            List<Tuple<double, double>> expectedResult = new List<Tuple<double, double>>() { new Tuple<double, double>(0, 7),
                                                                                             new Tuple<double, double>(0.25, 7),
                                                                                             new Tuple<double, double>(0.50, 8),
                                                                                             new Tuple<double, double>(0.75, 9),
                                                                                             new Tuple<double, double>(1, 9) };
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod()]
        public void Compose_HigherandLowerTransformWithPerfectMatchReturns_MatchedTransformOrdinates()
        {

        }

        [TestMethod()]
        public void Compose_LowerTransformWithOverlapandExtraPointsReturns_MatchedPoints()
        {
            OrdinatesFunction testOrdinates = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 0, 0.25, 0.5, 0.75, 1 }, new double[] { 25, 40, 50, 75, 100 }, true, false));
            OrdinatesFunction testTransform = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 0, 10, 40, 70 }, new double[] { 10, 20, 50, 80 }, true, false));
            List<Tuple<double, double>> actualResult = (List<Tuple<double, double>>)testOrdinates.Compose(testTransform.Ordinates);
            List<Tuple<double, double>> expectedResult = new List<Tuple<double, double>>() { new Tuple<double, double>(0, 35),
                                                                                             new Tuple<double, double>(0.25, 50),
                                                                                             new Tuple<double, double>(0.50, 60),
                                                                                             new Tuple<double, double>(0.70, 80),
                                                                                             new Tuple<double, double>(0.75, 80),
                                                                                             new Tuple<double, double>(1, 80) };
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }
        #endregion
    }
}
