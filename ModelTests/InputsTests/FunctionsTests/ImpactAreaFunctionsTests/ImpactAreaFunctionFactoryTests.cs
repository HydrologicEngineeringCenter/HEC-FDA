using System;
using System.Collections.Generic;
using Model.Inputs.Functions;
using Moq;
using Model.Inputs.Functions.ImpactAreaFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModelTests.InputsTests.FunctionsTests
{
    [TestClass]
    public class ComputationPointFunctionFactoryTests
    {
        #region Fields and Properties
        private static double[] testXs = new double[2] { 0, 1 }, testYs = new double[2] { 2, 3 };
        private Statistics.CurveIncreasing testCurveIncreasing = new Statistics.CurveIncreasing(testXs, testYs, true, false);
        private Statistics.LogPearsonIII testLP3 = new Statistics.LogPearsonIII(2, .1, .1, 100);
        #endregion

        #region CreateNew() "Constructor" Tests
        //[TestMethod()]
        //public void CreateNew_InflowFrequencyLP3ReturnsInflowFrequencyImplementation()
        //{
        //    ComputationPointFunctionBase testInflowFrequency = ComputationPointFunctionFactory.CreateNew(testLP3, ComputationPointFunctionEnum.InflowFrequency);
        //    Assert.IsTrue(testInflowFrequency.GetType() == typeof(InflowFrequency));
        //}

        //[TestMethod()]
        //public void CreateNew_InvalidTypeLP3ReurnsUnUsedImplementation()
        //{
        //    ComputationPointFunctionBase testInflowOutflow = ComputationPointFunctionFactory.CreateNew(testLP3, ComputationPointFunctionEnum.InflowOutflow);
        //    Assert.IsTrue(testInflowOutflow.GetType() == typeof(UnUsed));
        //}

        //[TestMethod()]
        //public void CreateNew_InflowOutflowCurveIncreasingReturnsInflowOutflowImplementation()
        //{
        //    ComputationPointFunctionBase testInflowOutflow = ComputationPointFunctionFactory.CreateNew(testCurveIncreasing, ComputationPointFunctionEnum.InflowOutflow);
        //    Assert.IsTrue(testInflowOutflow.GetType() == typeof(InflowOutflow));
        //}

        //[TestMethod()]
        //public void CreateNew_OutflowFrequencyLP3ReturnsUnUsedImplementation()
        //{
        //    ComputationPointFunctionBase testOutflowFrequency = ComputationPointFunctionFactory.CreateNew(testLP3, ComputationPointFunctionEnum.OutflowFrequency);
        //    Assert.IsTrue(testOutflowFrequency.GetType() == typeof(UnUsed));
        //}

        //[TestMethod()]
        //public void CreateNew_OutflowFrequencyCurveIncreasingReturnsOutflowFrequencyImplementation()
        //{
        //    ComputationPointFunctionBase testOutflowFrequency = ComputationPointFunctionFactory.CreateNew(testCurveIncreasing, ComputationPointFunctionEnum.OutflowFrequency);
        //    Assert.IsTrue(testOutflowFrequency.GetType() == typeof(OutflowFrequency));
        //}

        //[TestMethod()]
        //public void CreateNew_RatingCurveIncreasingReturnsRatingImplementation()
        //{
        //    ComputationPointFunctionBase testRating = ComputationPointFunctionFactory.CreateNew(testCurveIncreasing, ComputationPointFunctionEnum.Rating);
        //    Assert.IsTrue(testRating.GetType() == typeof(Rating));
        //}

        //[TestMethod()]
        //public void CreateNew_InvalidTypeCurveIncreasingReturnsUnUsedImplementation()
        //{
        //    ComputationPointFunctionBase testInflowFrequency = ComputationPointFunctionFactory.CreateNew(testCurveIncreasing, ComputationPointFunctionEnum.InflowFrequency);
        //    Assert.IsTrue(testInflowFrequency.GetType() == typeof(UnUsed));
        //}
        #endregion

        #region FunctionRegistryTests
        //[TestMethod()]
        //public void CreateNew_GoodFunctionReturnsNewFunctionListInstanceWithOneFunction()
        //{
        //    ComputationPointFunctionBase testFunction = ComputationPointFunctionFactory.CreateNew(new Statistics.CurveIncreasing(new double[] { 0, 1 }, new double[] { 2, 3 }, true, false), ComputationPointFunctionEnum.InflowOutflow);
        //    IList<Tuple<string, ComputationPointFunctionBase>> testList = ComputationPointFunctionRegistry.Instance.CompleteList;
        //    Assert.AreEqual(testList.Count, ComputationPointFunctionRegistry.Instance.CompleteList.Count);
        //    Assert.AreEqual(1, testList.Count);
        //}
        #endregion
    }
}
