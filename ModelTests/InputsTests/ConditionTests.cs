//using System;
//using Model;
//using Model.Inputs;
//using Model.Inputs.Functions;
//using System.Collections.Generic;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace ModelTests.Inputs
//{
//    [TestClass]
//    public class ConditionTests
//    {
//        public static double[] xs = { 0, 1 }, ys = { 2, 3 };
//        public static IFunction aIOFunction = FunctionFactory.CreateNew(new Statistics.CurveIncreasing(xs, ys, true, false), FunctionTypeEnum.InflowOutflow);
//        public static IFunction aRatingFunction = FunctionFactory.CreateNew(new Statistics.CurveIncreasing(xs, ys, true, false), FunctionTypeEnum.Rating);
//        public static IFunctionCompose aEntryFunction = (IFunctionCompose)FunctionFactory.CreateNew(new Statistics.LogPearsonIII(1, 0.5, 0.1, 100), FunctionTypeEnum.InflowFrequency);
//        public List<IFunction> aTransformFunctionList = new List<IFunction> { aRatingFunction, aIOFunction };
//        public static IThreshold aThreshold = new Threshold(ThresholdTypes.ExteriorStage, 10);

//        #region Validate() Tests
//        [TestMethod()]
//        public void Validate_OrderedTransformsReturnValidCondition()
//        {
//            //Arrange
//            List<IFunction> orderedTransformFunctions = new List<IFunction>() { aIOFunction, aRatingFunction };
//            ICondition testCondition = new Condition(aEntryFunction, orderedTransformFunctions, aThreshold);
//            //Act 
//            //Assert
//            Assert.IsTrue(testCondition.Validate());
//        }

//        [TestMethod()]
//        public void Validate_UnOrderedTransformsReturnValidCondition()
//        {
//            //Arrange
//            ICondition testCondition = new Condition(aEntryFunction, aTransformFunctionList, aThreshold);
//            //Act 
//            //Assert
//            Assert.IsTrue(testCondition.Validate());
//        }

//        #endregion

//        #region ReportValidationErrors() Tests
//        [TestMethod()]
//        public void ReportValidationErrors_OrderedTransformsReturnValidCondition()
//        {
//            //Arrange
//            List<IFunction> orderedTransformFunctions = new List<IFunction>() { aIOFunction, aRatingFunction };
//            ICondition testCondition = new Condition(aEntryFunction, orderedTransformFunctions, aThreshold);
//            //Act 
//            testCondition.ReportValidationErrors();

//            //Assert
//            Assert.IsTrue(testCondition.IsValid);
//        }
//        [TestMethod()]
//        public void ReportValidationErrors_UnOrderedTransformsReturnValidCondition()
//        {
//            ICondition testCondition = new Condition(aEntryFunction, aTransformFunctionList, aThreshold);
//            testCondition.ReportValidationErrors();
//            Assert.IsTrue(testCondition.IsValid);
//        }
//        public void ReportValidationErrors_FrequencyFunctionInTransformsReturnsError()
//        {
//            IFunction invalidTransformEntry = aEntryFunction;
             
//        }
//        public void
//        #endregion

       
//    }
//}
