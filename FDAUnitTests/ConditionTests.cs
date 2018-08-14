using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FdaModel.Functions.OrdinatesFunctions;
using FdaModel.Functions.FrequencyFunctions;
using System.Collections.Generic;
using FdaModel.ComputationPoint;
using System.Text;


namespace FdaUnitTests
{
    [TestClass]
    public class ConditionTests
    {
        private LogPearsonIII _zero;
        private OrdinatesFunction _one;
        private OrdinatesFunction _two;
        private OrdinatesFunction _three;
        private OrdinatesFunction _four;
        private OrdinatesFunction _five;
        private OrdinatesFunction _six;
        private OrdinatesFunction _seven;
        private OrdinatesFunction _eight;
        private OrdinatesFunction _nine;
        private OrdinatesFunction _ninetynine;


        [TestInitialize]
        public void Initialize()
        {

            double[] peakFlowData = new double[] { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000 };
            LogPearsonIII zero = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(peakFlowData);
            _zero = zero;

            LogPearsonIII zero1 = new LogPearsonIII(4, .4, .5, 50);

            double[] xblah = new double[] { 0, 0, 0, 3, 5 };
            double[] yblah = new double[] { 0, .2, .5, .7, .9 };
            OrdinatesFunction testOrdinatesFunction = new OrdinatesFunction(xblah, yblah, FdaModel.Functions.FunctionTypes.LeveeFailure);

            // Create ordinates function
            double[] x = new double[] { 100, 200, 500, 1000, 2000 };
            double[] y = new double[] { 200, 300, 600, 1100, 2100 };
            OrdinatesFunction one = new OrdinatesFunction(x, y, FdaModel.Functions.FunctionTypes.InflowOutflow);

            _one = one;

            double[] x1 = new double[] { .2f, .3f, .4f, .5f, .6f, .7f, .8f, .9f };
            double[] y1 = new double[] { 4000, 5000, 10000, 20000, 30000, 50000, 80000, 120000 };
            OrdinatesFunction two = new OrdinatesFunction(x1, y1, FdaModel.Functions.FunctionTypes.OutflowFrequency);

            _two = two;

            //3. Create ordinates function
            double[] xs = new double[] { 100, 200, 500, 1000, 2000 };
            double[] ys = new double[] { 1, 2, 5, 10, 20 };
            OrdinatesFunction three = new OrdinatesFunction(xs, ys, FdaModel.Functions.FunctionTypes.Rating);

            _three = three;

            double[] x2 = new double[] { .2f, .3f, .4f, .5f, .6f, .7f, .8f, .9f };
            double[] y2 = new double[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            OrdinatesFunction four = new OrdinatesFunction(x2, y2, FdaModel.Functions.FunctionTypes.ExteriorStageFrequency);

            _four = four;

            double[] xval = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            double[] yval = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            OrdinatesFunction five = new OrdinatesFunction(xval, yval, FdaModel.Functions.FunctionTypes.ExteriorInteriorStage);

            _five = five;

            double[] x3 = new double[] { .2f, .3f, .4f, .5f, .6f, .7f, .8f, .9f };
            double[] y3 = new double[] { 2, 200, 300, 600, 1100, 2000, 3000, 4000 };
            OrdinatesFunction six = new OrdinatesFunction(x3, y3, FdaModel.Functions.FunctionTypes.InteriorStageFrequency);

            _six = six;

            double[] x4 = new double[] { 0, 1, 2, 5, 7, 8, 9, 10, 12, 15, 20 };
            double[] y4 = new double[] { 1, 2, 3, 600, 1100, 1300, 1800, 10000, 30000, 100000, 500000 };
            OrdinatesFunction seven = new OrdinatesFunction(x4, y4, FdaModel.Functions.FunctionTypes.InteriorStageDamage);

            _seven = seven;

            double[] x5 = new double[] { .2f, .3f, .4f, .5f, .6f, .7f, .8f, .9f };
            double[] y5 = new double[] { 2, 200, 300, 600, 1100, 2000, 3000, 4000 };
            OrdinatesFunction eight = new OrdinatesFunction(x5, y5, FdaModel.Functions.FunctionTypes.DamageFrequency);

            _eight = eight;

            double[] x62 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            double[] y62 = new double[] { 0, .05f, .1f, .2f, .3f, .4f, .7f, .8f, .9f, .95f, 1 };
            OrdinatesFunction nine = new OrdinatesFunction(x62, y62, FdaModel.Functions.FunctionTypes.LeveeFailure);

            _nine = nine;

            double[] x6 = new double[] { 0, 5, 10 };
            double[] y6 = new double[] { 0, .7f, 1 };
            OrdinatesFunction nine2 = new OrdinatesFunction(x6, y6, FdaModel.Functions.FunctionTypes.LeveeFailure);

            double[] x7 = new double[] { 0, 100, 200, 500, 1000 };
            double[] y7 = new double[] { 2, 200, 300, 600, 1100 };
            OrdinatesFunction ninetyNine = new OrdinatesFunction(x7, y7, FdaModel.Functions.FunctionTypes.UnUsed);

            _ninetynine = ninetyNine;
        }



        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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


        //[TestMethod]
        //public void OrderListTest()
        //{
        //    List<FdaModel.Functions.BaseFunction> myListOfBaseFunctions = new List<FdaModel.Functions.BaseFunction>() { _zero, _one };

        //    PerformanceThreshold threshold = new PerformanceThreshold(PerformanceThresholdTypes.Damage, 5000);
        //    LateralStructure myLateralStruct = new LateralStructure(true, 10);


        //    Condition simpleTest = new Condition(2008, "russian river", myListOfBaseFunctions, threshold, myLateralStruct, true); //bool call Validate

        //    simpleTest.Validate();

        //    List<FdaModel.Functions.BaseFunction> expectedOutputList = new List<FdaModel.Functions.BaseFunction>() { _zero, _one };

        //    StringBuilder outputString = new StringBuilder();
        //    for (int i = 0; i < simpleTest.Functions.Count; i++)
        //    {
        //        outputString.Append("Func Type " + i + " = " + simpleTest.Functions[i].FunctionType + " | " + expectedOutputList[i].FunctionType + Environment.NewLine);
        //    }

        //    Assert.AreEqual(simpleTest.Functions.Count, expectedOutputList.Count,"not the same length");
        //    for (int i = 0; i < simpleTest.Functions.Count; i++)
        //    {
        //        Assert.AreEqual(simpleTest.Functions[i], expectedOutputList[i]);

        //    }


        //}

        //[DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\ComposeTestingFile.csv", "ComposeTestingFile#csv", DataAccessMethod.Sequential)]//, DeploymentItem("ConditionTests\\OrdinatesFunctionInputs.csv")]

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\CSV Files\\ValidationTestInputs.csv", "ValidationTestInputs#csv", DataAccessMethod.Sequential)]//, DeploymentItem("ConditionTests\\OrdinatesFunctionInputs.csv")]
        [TestMethod]
        public void ValidateTest()
        {

            List<int> listOfInputFunctions = new List<int>();
            List<int> listOfOutputFunctions = new List<int>();

            string inputs = TestContext.DataRow["Inputs"].ToString();
            char space = ' ';
            string[] inputsArray = inputs.Split(space);
            foreach(string s in inputsArray)
            {
                listOfInputFunctions.Add(Convert.ToInt32(s));
            }

            string outputs = TestContext.DataRow["Outputs"].ToString();
            string[] outputsArray = outputs.Split(space);
            foreach (string s in outputsArray)
            {
                listOfOutputFunctions.Add(Convert.ToInt32(s));
            }

            //int firstFunction = Convert.ToInt32(TestContext.DataRow["cody"]);
            //listOfInputFunctions.Add(firstFunction);
            //int secondFunction = Convert.ToInt32(TestContext.DataRow["jenna"]);
            //listOfInputFunctions.Add(secondFunction);

            List<FdaModel.Functions.BaseFunction> inputFunctions = new List<FdaModel.Functions.BaseFunction>();

            foreach (int func in listOfInputFunctions)
            {
                
                switch (func)
                {
                    case 0:
                        {
                            inputFunctions.Add(_zero);
                            break;
                        }
                    case 1:
                        {
                            inputFunctions.Add(_one);
                            break;
                        }
                    case 2:
                        {
                            inputFunctions.Add(_two);
                            break;
                        }
                    case 3:
                        {
                            inputFunctions.Add(_three);
                            break;
                        }
                    case 4:
                        {
                            inputFunctions.Add(_four);
                            break;
                        }
                    case 5:
                        {
                            inputFunctions.Add(_five);
                            break;
                        }
                    case 6:
                        {
                            inputFunctions.Add(_six);
                            break;
                        }
                    case 7:
                        {
                            inputFunctions.Add(_seven);
                            break;
                        }
                    case 8:
                        {
                            inputFunctions.Add(_eight);
                            break;
                        }
                    case 9:
                        {
                            inputFunctions.Add(_nine);
                            break;
                        }
                    case 99:
                        {
                            inputFunctions.Add(_ninetynine);
                            break;
                        }
                }
            }





            PerformanceThreshold threshold = new PerformanceThreshold(PerformanceThresholdTypes.Damage, 5000);
            LateralStructure myLateralStruct = new LateralStructure( 10);


            Condition simpleTest = new Condition(2008, "russian river", inputFunctions, threshold, myLateralStruct); //bool call Validate

            StringBuilder outputString = new StringBuilder();
            outputString.Append("calculated: ");

            for (int i = 0; i < simpleTest.Functions.Count; i++)
            {
                outputString.Append((int)simpleTest.Functions[i].FunctionType + ",");
            }
            outputString.Append("Expected: ");

            for (int i = 0; i < listOfOutputFunctions.Count; i++)
            {
                outputString.Append(listOfOutputFunctions[i] + ",");
            }

            Assert.AreEqual(simpleTest.Functions.Count, listOfOutputFunctions.Count,"not equal lengths\n" + outputString);
            for (int i = 0; i < listOfOutputFunctions.Count; i++)
            {
                Assert.AreEqual((int)simpleTest.Functions[i].FunctionType, listOfOutputFunctions[i],outputString.ToString());
            }


        }

    }
}
