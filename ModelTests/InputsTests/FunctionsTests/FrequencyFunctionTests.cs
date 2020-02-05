//using System;
//using System.Collections.Generic;
//using Model.Inputs.Functions;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace ModelTests.InputsTests.FunctionsTests
//{
//    [TestClass]
//    public class FrequencyFunctionTests
//    {

//        #region Compose() Tests
//        [TestMethod()]
//        public void Compose_LowerTransformNoOverlapReturns_DoubleNaNYValueOrdinates()
//        {
//            double[] testFlows = new double[] { 1000, 2000, 3000, 4000, 5000 };
//            FrequencyFunction testFunction = new FrequencyFunction(new Statistics.LogPearsonIII(testFlows));
//            double lowestFlow = testFunction.Function.getDistributedVariable(0.0001); //should be ~67.
//            List<Tuple<double, double>> testTransform = new List<Tuple<double, double>>() { new Tuple<double, double>(1, 3),
//                                                                                            new Tuple<double, double>(2, 4) };
//            List<Tuple<double, double>> actualResult = (List<Tuple<double, double>>)testFunction.Compose(testTransform);
//            List<Tuple<double, double>> expectedResult = new List<Tuple<double, double>>() { new Tuple<double, double>(0.0001, double.NaN),
//                                                                                             new Tuple<double, double>(0.9999, double.NaN) };
//            CollectionAssert.AreEqual(expectedResult, actualResult);
//        }

//        [TestMethod()]
//        public void Compose_HigherTransformNoOverlapReturns_DoubleNaNYValueOrdinates()
//        {
//            double[] testFlows = new double[] { 1, 2, 3, 4, 5 };
//            FrequencyFunction testFunction = new FrequencyFunction(new Statistics.LogPearsonIII(testFlows));
//            double highestFlow = testFunction.Function.getDistributedVariable(0.9999); //should be ~9.89.
//            List<Tuple<double, double>> testTransform = new List<Tuple<double, double>>() { new Tuple<double, double>(10, 30),
//                                                                                            new Tuple<double, double>(20, 40) };
//            List<Tuple<double, double>> actualResult = (List<Tuple<double, double>>)testFunction.Compose(testTransform);
//            List<Tuple<double, double>> expectedResult = new List<Tuple<double, double>>() { new Tuple<double, double>(0.0001, double.NaN),
//                                                                                             new Tuple<double, double>(0.9999, double.NaN) };

//            CollectionAssert.AreEqual(expectedResult, actualResult);
//        }

//        [TestMethod()]
//        public void Compose_InBetweenTransformReturns_OrdinatesWithin1PercentofXandYRanges()
//        {
//            double[] testFlows = new double[] { 1, 2, 3, 4, 5 };
//            FrequencyFunction testFunction = new FrequencyFunction(new Statistics.LogPearsonIII(testFlows));
//            List<Tuple<double, double>> testTransform = new List<Tuple<double, double>>() { new Tuple<double, double>(1, 10),
//                                                                                            new Tuple<double, double>(2, 20),
//                                                                                            new Tuple<double, double>(3, 30),
//                                                                                            new Tuple<double, double>(4, 40),
//                                                                                            new Tuple<double, double>(5, 50) };
//            List<Tuple<double, double>> composedResult = (List<Tuple<double, double>>)testFunction.Compose(testTransform);

//            bool inTolerances = true;
//            double xTolerance = 0.011, yTolerance = (50d - 10d) / 10 + .001;
//            for (int i = 0; i < composedResult.Count - 1; i++)
//            {
//                if (composedResult[i + 1].Item1 - composedResult[i].Item1 > xTolerance ||
//                    composedResult[i + 1].Item2 - composedResult[i].Item2 > yTolerance) inTolerances = false;
//            }

//            Assert.IsTrue(inTolerances);
//        }
//        #endregion

//        #region Validate() Tests
//        [TestMethod()]
//        public void Validate_TooHighFrequencyFunctionReturnsFalse()
//        {
//            FrequencyFunction testFrequencyFunction = new FrequencyFunction(new Statistics.LogPearsonIII(new double[] { 10000000, 20000000 }));
//            Assert.AreEqual(testFrequencyFunction.IsValid, false);
//        }

//        [TestMethod()]
//        public void Validate_TooLowFrequencyFunctionReturnsFalse()
//        {
//            FrequencyFunction testFrequencyFunction = new FrequencyFunction(new Statistics.LogPearsonIII(new double[] { -1, 0 }));
//            Assert.AreEqual(testFrequencyFunction.IsValid, false);
//        }
//        #endregion
//    }
//}
