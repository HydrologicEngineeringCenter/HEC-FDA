//using System;
//using Model.Inputs.Functions;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace ModelTests
//{
//    [TestClass()]
//    public class DecoratorTests
//    {
//        [TestMethod()]
//        public void GetXfromY_OrdinatesAndFrequencyFunctionComputeDifferentExpectedResults()
//        {
//            //Arrange
//            double y = 2.5;
//            double[] xs = new double[2] { 0, 1 }, ys = new double[2] { 2, 3 };
//            Statistics.CurveIncreasing testOrdinatesFunction = new Statistics.CurveIncreasing(xs, ys, true, false);
//            IFunction testFunction1 = FunctionFactory.CreateNew(testOrdinatesFunction, FunctionTypeEnum.NotSet);
//            IFunction testFunction2 = FunctionFactory.CreateNew(new Statistics.LogPearsonIII(1, 0.1, 0, 100), FunctionTypeEnum.NotSet);

//            //Act
//            double actualExpect0Point5 = testFunction1.GetXfromY(y);
//            double actualExpectNeg1Point9 = testFunction2.GetXfromY(y);

//            //Assert
//            Assert.AreEqual(0.5, actualExpect0Point5);
//            Assert.AreEqual(-1.90, actualExpectNeg1Point9, 0.1);
//            Assert.AreNotEqual(actualExpect0Point5, actualExpectNeg1Point9);
//        }
//    }

//}
