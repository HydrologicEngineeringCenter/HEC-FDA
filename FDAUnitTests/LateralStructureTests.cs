using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace FDAUnitTests
{
    [TestClass]
    public class LateralStructureTests
    {

        private TestContext testContextInstance;


        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\CSV Files\\CreateInteriorExteriorFunctionTest.csv", "CreateInteriorExteriorFunctionTest#csv", DataAccessMethod.Sequential)]//, DeploymentItem("ConditionTests\\OrdinatesFunctionInputs.csv")]

        [TestMethod]
        public void CreateInteriorExteriorTest()
        {
            string elev = TestContext.DataRow["elevation"].ToString();
            double elevation = Convert.ToDouble(elev);

            string hasFailure = TestContext.DataRow["hasFailureFunction"].ToString();
            bool hasFailureFunction = Convert.ToBoolean(hasFailure);

            string Func1xs = TestContext.DataRow["xValues"].ToString();
            char space = ' ';
            string[] Func1xArray = Func1xs.Split(space);
            List<double> Func1xList = new List<double>();
            foreach (string item in Func1xArray)
            {
                Func1xList.Add(Convert.ToDouble(item));
            }
            double[] Func1xValues = Func1xList.ToArray();

            //Get function 1 ys
            string Func1ys = TestContext.DataRow["yValues"].ToString();
            string[] Func1yArray = Func1ys.Split(space);
            List<double> Func1yList = new List<double>();
            foreach (string item in Func1yArray)
            {
                Func1yList.Add(Convert.ToDouble(item));
            }
            double[] Func1yValues = Func1yList.ToArray();


            string expectedXs = TestContext.DataRow["expectedXs"].ToString();
            string[] expectedXsArray = expectedXs.Split(space);
            List<double> expectedXsList = new List<double>();
            foreach (string item in expectedXsArray)
            {
                expectedXsList.Add(Convert.ToDouble(item));
            }
            double[] expectedxValues = expectedXsList.ToArray();

            //Get the expected array of ys
            string expectedYs = TestContext.DataRow["expectedYs"].ToString();
            string[] expectedYsArray = expectedYs.Split(space);
            List<double> expectedYsList = new List<double>();
            foreach (string item in expectedYsArray)
            {
                expectedYsList.Add(Convert.ToDouble(item));
            }
            double[] expectedyValues = expectedYsList.ToArray();


            FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction exteriorStageFunction = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(Func1xValues, Func1yValues, FdaModel.Functions.FunctionTypes.ExteriorInteriorStage);

            FdaModel.ComputationPoint.LateralStructure latStruct = new FdaModel.ComputationPoint.LateralStructure( elevation);

            FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction resultFunction = latStruct.CreateInteriorExteriorFunction(exteriorStageFunction, hasFailureFunction);


            for (int i = 0; i < expectedyValues.Length; i++)
            {
                Assert.AreEqual(resultFunction.Function.XValues[i], expectedxValues[i], .01, "x value missmatch. result function xValue: " + resultFunction.Function.XValues[i] + " expected x value: " + expectedxValues[i]);
                Assert.AreEqual(resultFunction.Function.YValues[i], expectedyValues[i], .01, "y value missmatch. result function yValue: " + resultFunction.Function.YValues[i] + " expected y value: " + expectedyValues[i]);

            }
           

        }
    }
}
