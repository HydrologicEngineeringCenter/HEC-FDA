using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using FdaModel.Functions.OrdinatesFunctions;
using FdaModel.Functions.FrequencyFunctions;
using FdaModel.ComputationPoint;

namespace FDAUnitTests
{
    [TestClass]
    public class RealizationTests:FdaTester.ModelTester.UnitTests.Compute_Testing.BaseComputeTest
    {

        private TestContext testContextInstance;
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





        public void LoadDataSet1()
        {
            /////////////////////////////////////////////////
            //    ***********   WARNING  *************
            //   Do not change any of these values. It will break the tests. If you want
            //   to test different values then create another dataset
            //   and add the logic to the switch case in the test.

            //////////////////////////////////////////////////

            double[] peakFlowData = new double[] { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000 };
            LogPearsonIII zero = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(peakFlowData);
            _zero = zero;     

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

            double[] x7 = new double[] { 0, 100, 200, 500, 1000 };
            double[] y7 = new double[] { 2, 200, 300, 600, 1100 };
            OrdinatesFunction ninetyNine = new OrdinatesFunction(x7, y7, FdaModel.Functions.FunctionTypes.UnUsed);

            _ninetynine = ninetyNine;
        }


        private List<FdaModel.Functions.BaseFunction> GetInputListFromFunctionSequence(List<int> inputSequence)
        {
            List<FdaModel.Functions.BaseFunction> inputFunctionsList = new List<FdaModel.Functions.BaseFunction>();

            foreach (int num in inputSequence)
            {
                switch (num)
                {
                    case 0:
                        {
                            inputFunctionsList.Add(_zero);
                            break;
                        }
                    case 1:
                        {
                            inputFunctionsList.Add(_one);
                            break;

                        }
                    case 2:
                        {
                            inputFunctionsList.Add(_two);
                            break;
                        }
                    case 3:
                        {
                            inputFunctionsList.Add(_three);
                            break;

                        }
                    case 4:
                        {
                            inputFunctionsList.Add(_four);
                            break;
                        }
                    case 5:
                        {
                            inputFunctionsList.Add(_five);
                            break;

                        }
                    case 6:
                        {
                            inputFunctionsList.Add(_six);
                            break;
                        }
                    case 7:
                        {
                            inputFunctionsList.Add(_seven);
                            break;

                        }
                    case 8:
                        {
                            inputFunctionsList.Add(_eight);
                            break;
                        }
                    case 9:
                        {
                            inputFunctionsList.Add(_nine);
                            break;

                        }
                    case 99:
                        {
                            inputFunctionsList.Add(_ninetynine);
                            break;
                        }

                }
            }

            return inputFunctionsList;
        }



        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\CSV Files\\ComputeNewMethodTest.csv", "ComputeNewMethodTest#csv", DataAccessMethod.Sequential)]//, DeploymentItem("ConditionTests\\OrdinatesFunctionInputs.csv")]

        [TestMethod]
        public void Compute_NewMethod_Test() //need to write another one to test the legacy compute
        {
            //
            char space = ' ';
            //get the data set
            string dataSetString = TestContext.DataRow["DataSet"].ToString();
            int dataSet = Convert.ToInt16( dataSetString);

            switch(dataSet)
            {
                case 1:
                    {
                        LoadDataSet1();
                        break;
                    }
            }

            string FunctionSequence = TestContext.DataRow["FunctionSequence"].ToString();    
            string[] FunctionSequenceArray = FunctionSequence.Split(space);
            List<int> FunctionSequenceList = new List<int>();
            foreach (string item in FunctionSequenceArray)
            {
                FunctionSequenceList.Add(Convert.ToInt16(item));
            }

            //load up the input functions list

            List<FdaModel.Functions.BaseFunction> inputFunctions = GetInputListFromFunctionSequence(FunctionSequenceList);

            //create a threshold

            string ThresholdEnumString = TestContext.DataRow["ThresholdEnum"].ToString();
            int thresholdEnum = Convert.ToInt16(ThresholdEnumString);

            string ThresholdValueString = TestContext.DataRow["ThresholdValue"].ToString();
            int thresholdValue = Convert.ToInt16(ThresholdValueString);
            
            PerformanceThreshold threshold = new PerformanceThreshold((PerformanceThresholdTypes)thresholdEnum, thresholdValue);

            string isLateralStructureString = TestContext.DataRow["IsLateralStructure"].ToString();
            bool isLateralStructure = Convert.ToBoolean(isLateralStructureString);

            Condition myCondition;
            if (isLateralStructure == true)
            {
                //get the elevation height
                string elevationHeightString = TestContext.DataRow["ElevationHeight"].ToString();
                double elevationHeight = Convert.ToDouble(elevationHeightString);
                LateralStructure myLatStruct = new LateralStructure( elevationHeight);

                 myCondition = new Condition(2017, "planet earth", inputFunctions, threshold, myLatStruct);


            }
            else
            {

                myCondition = new Condition(2017, "planet earth", inputFunctions, threshold);

           }


            FdaModel.ComputationPoint.Outputs.Realization myRealization = new FdaModel.ComputationPoint.Outputs.Realization(myCondition, false, false);
            Random randomNumberGenerator = new Random(0);
            myRealization.Compute(randomNumberGenerator);

            // ********************  write the data to a file ***********************************
            //////////////////////////////////////////////////////////////////////////////////////

            string testName = TestContext.DataRow["TestName"].ToString();
            RealizationErrors = myRealization.Messages.Messages;
            MyComputedObject = myRealization;

            //FdaTester.ModelTester.UnitTests.Compute_Testing.Tests.InflowFrequency_InflowOutflow_Rating_0_1_3_ temp = new FdaTester.ModelTester.UnitTests.Compute_Testing.Tests.InflowFrequency_InflowOutflow_Rating_0_1_3_();

            //get the errors
            string sourceFile = @"C:\Users\q0heccdm\AppData\Local\Temp\1\HEC\FdaModel_LogFile.txt";


            //for (int i = 0; i < 10; i++)
            //{
            //      Compute_Testing.Tests.AllFunctionsTest aft = new Compute_Testing.Tests.AllFunctionsTest();
            object[] errMesArray = new object[] { "this is a test" };
            if (System.IO.File.Exists(sourceFile))
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(sourceFile);
                List<string> errMesList = new List<string>();
                while (sr.ReadLine() != null)
                {
                    errMesList.Add(sr.ReadLine());
                }
                errMesArray = errMesList.ToArray();

                //    string destFile = @"C:\Users\q0heccdm\Documents\HEC FDA\Compute Testing\Compute Error Messages\Error Messages_" + i + ".txt";
                //  System.IO.File.Copy(sourceFile, destFile, true);

                sr.Close();
                System.IO.File.Delete(sourceFile);
            }

                //((FdaTester.ModelTester.UnitTests.Compute_Testing.BaseComputeTest)myRealization).WriteDataOut(((FdaTester.ModelTester.UnitTests.Compute_Testing.BaseComputeTest)temp).MyComputedObject, @"C:\Users\q0heccdm\Documents\HEC FDA\Compute Testing\Compute Test Results\Single Test_" + testName +".txt", errMesArray);
            ComputeWriter.WriteRealization(myRealization, @"..\Testing\Compute Testing\Compute Test Results\Single Test_" + testName + ".txt", errMesArray);

            // *********************  end writing ********************************************
            ////////////////////////////////////////////////////////////////////////////////////

            string isPerformanceOnlyString = TestContext.DataRow["IsPerformanceOnly"].ToString();
            bool isPerformanceOnly = Convert.ToBoolean(isPerformanceOnlyString);

            string expectedAEPString = TestContext.DataRow["ExpectedAEP"].ToString();
            double expectedAEP = Convert.ToDouble(expectedAEPString);


            if (isPerformanceOnly == true)
            {
                Assert.AreEqual(expectedAEP, 1 - myRealization.AnnualExceedanceProbability, .001);
            }
            else
            {
                string expectedEADString = TestContext.DataRow["ExpectedEAD"].ToString();
                double expectedEAD = Convert.ToDouble(expectedEADString);

                Assert.AreEqual(expectedAEP, 1 - myRealization.AnnualExceedanceProbability, .001);
                Assert.AreEqual(expectedEAD, myRealization.ExpectedAnnualDamage, 1);


            }

            

        }
    }
}
