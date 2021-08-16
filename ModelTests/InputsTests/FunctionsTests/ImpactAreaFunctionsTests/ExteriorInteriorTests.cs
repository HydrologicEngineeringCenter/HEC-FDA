//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace ModelTests.InputsTests.FunctionsTests.ImpactAreaFunctionsTests
//{
//    [TestClass]
//    public class ExteriorInteriorTests
//    {

//        [TestMethod()]
//        public void Validation_NotHigherInteriorStagesReturnsTrue()
//        {
//            Model.Inputs.Functions.OrdinatesFunction testOrdinates = new Model.Inputs.Functions.OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 2, 3 }, new double[] { 2, 2.5d }, true, false));
//            Model.Inputs.Functions.ImpactAreaFunctions.ExteriorInteriorStage testFunction = new Model.Inputs.Functions.ImpactAreaFunctions.ExteriorInteriorStage(testOrdinates, testOrdinates.Ordinates);
//            Assert.IsTrue(testFunction.IsValid);
//        }

//        [TestMethod()]
//        public void Validation_HigherInteriorStagesReturnsFalse()
//        {
//            Model.Inputs.Functions.OrdinatesFunction testOrdinates = new Model.Inputs.Functions.OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 0, 1 }, new double[] { 2, 3 }, true, false));
//            Model.Inputs.Functions.ImpactAreaFunctions.ExteriorInteriorStage testFunction = new Model.Inputs.Functions.ImpactAreaFunctions.ExteriorInteriorStage(testOrdinates, testOrdinates.Ordinates);
//            Assert.IsFalse(testFunction.IsValid);
//        }
//    }
//}
