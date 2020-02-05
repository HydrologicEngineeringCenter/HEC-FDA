using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Inputs.Functions;
using Model.Inputs.Functions.ImpactAreaFunctions;
using Model.Inputs.Conditions;
using Model.Utilities.DataGenerators;
using System.Diagnostics;

namespace ModelTests.Utilities.DataGenerators
{
    [TestClass()]
    public class TestCaseGeneratorsTests
    {
        #region ConditionGenerator() Tests
        //NEED TO FIX THIS>>>
        //[TestMethod()]
        //public void ConditionGenerator_100RandomConditionsValidFunctionOrderReturnsTrue()
        //{
            
        //    Condition testObject;
        //    bool hasFailed = false;
        //    Random numberGenerator = new Random(1);
        //    for (int i = 0; i < 100; i++)
        //    {
        //        testObject = TestCaseGenerators.ConditionGenerator("Test", seed: numberGenerator.Next()) as Condition;
        //        if (!testObject.ValidateFunctionOrder())
        //        {
        //            hasFailed = true;
        //        }     
        //    }
        //    Assert.IsFalse(hasFailed);
        //}
        #endregion

        #region BuildOrderGenerator() Tests
        [TestMethod()]
        public void BuildOrderGenerator_100OrdersNotSetIsAlwaysFalse()
        {
            bool HasFailed = false;
            for (int i = 0; i < 100; i++)
            {
                Tuple<ImpactAreaFunctionEnum, bool>[] testOrder = TestCaseGenerators.BuildOrderGenerator();
                if (testOrder[0].Item1 != ImpactAreaFunctionEnum.NotSet) { HasFailed = true; break; }
                if (testOrder[0].Item2 != false) { HasFailed = true; break; }
            }
            Assert.IsFalse(HasFailed);
        }
        [TestMethod()]
        public void BuildOrderGenerator_100OrdersRatingIsAlwaysTrue()
        {
            bool HasFailed = false;
            for (int i = 0; i < 100; i++)
            {
                Tuple<ImpactAreaFunctionEnum, bool>[] testOrder = TestCaseGenerators.BuildOrderGenerator();
                if (testOrder[4].Item1 != ImpactAreaFunctionEnum.Rating) { HasFailed = true; break; }
                if (testOrder[4].Item2 != true) { HasFailed = true; break; }
            }
            Assert.IsFalse(HasFailed);
        }
        [TestMethod()]
        public void BuildOrderGenerator_100OrdersFinalFunctionNeverBeforeExteriorStageFrequency()
        {
            bool HasFailed = false;
            for (int i = 0; i < 100; i++)
            {
                Tuple<ImpactAreaFunctionEnum, bool>[] testOrder = TestCaseGenerators.BuildOrderGenerator();
                ImpactAreaFunctionEnum lastFunction = ImpactAreaFunctionEnum.NotSet;
                for (int n = 0; n < testOrder.Length; n++)
                {
                    if (testOrder[n].Item2 == true) lastFunction = testOrder[n].Item1;
                }
                if (lastFunction < ImpactAreaFunctionEnum.ExteriorStageFrequency)
                {
                    HasFailed = true;
                    break;
                }
            }
            Assert.IsFalse(HasFailed);
        }
        [TestMethod()]
        public void BuildOrderGenerator_100OrdersFinalFunctionNeverAfterDamageFrequency()
        {
            bool HasFailed = false;
            for (int i = 0; i < 100; i++)
            {
                Tuple<ImpactAreaFunctionEnum, bool>[] testOrder = TestCaseGenerators.BuildOrderGenerator();
                ImpactAreaFunctionEnum lastFunction = ImpactAreaFunctionEnum.NotSet;
                for (int n = 0; n < testOrder.Length; n++)
                {
                    if (testOrder[n].Item2 == true) lastFunction = testOrder[n].Item1;
                }
                if (lastFunction > ImpactAreaFunctionEnum.DamageFrequency) { HasFailed = true; break; }
            }
            Assert.IsFalse(HasFailed);
        }
        [TestMethod()]
        public void BuildOrderGenerator_100DamageFrequencyOrdersAlwaysIncludeStageDamageTransform()
        {
            int i = 0;
            bool HasFailed = false;

            while (i < 100)
            {
                Tuple<ImpactAreaFunctionEnum, bool>[] testOrder = TestCaseGenerators.BuildOrderGenerator();
                if (testOrder[9].Item1 != ImpactAreaFunctionEnum.DamageFrequency) { HasFailed = true; break; }
                if (testOrder[9].Item2 == true)
                {
                    if (testOrder[8].Item1 != ImpactAreaFunctionEnum.InteriorStageDamage) { HasFailed = true; break; }
                    if (testOrder[8].Item2 != true) { HasFailed = true; break; }
                    i++;
                }
            }
            Assert.IsFalse(HasFailed);
        }
        [TestMethod()]
        public void BuildOrderGenerator_100OrdersFinalFunctionAlwaysOdd()
        {
            bool HasFailed = false;
            for (int i = 0; i < 100; i++)
            {
                Tuple<ImpactAreaFunctionEnum, bool>[] testOrder = TestCaseGenerators.BuildOrderGenerator();
                ImpactAreaFunctionEnum lastFunction = ImpactAreaFunctionEnum.NotSet;
                for (int n = 0; n < testOrder.Length; n++)
                {
                    if (testOrder[n].Item2 == true) lastFunction = testOrder[n].Item1;
                }
                if ((int)lastFunction % 2 == 0) { HasFailed = true; break; }
            }
            Assert.IsFalse(HasFailed);
        }
        #endregion
    }
}
