using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FdaModel.Functions.OrdinatesFunctions;
using FdaModel.Functions.FrequencyFunctions;
using System.Collections.Generic;
using FdaModel.ComputationPoint;
using System.Text;


namespace FDAUnitTests
{
    [TestClass]
    public class FrequencyFunctionTests
    {


        private LogPearsonIII _zero;
        private LogPearsonIII _zero1;
        private LogPearsonIII _zero2;
        private LogPearsonIII _zero3;
        private LogPearsonIII _zero4;
        private LogPearsonIII _zero5;
        private LogPearsonIII _zero6;
        private LogPearsonIII _zero7;
        private LogPearsonIII _zero8;


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

        [TestInitialize]
        public void Initialize()
        {

            double[] peakFlowData = new double[] { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000 };
            LogPearsonIII zero = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(peakFlowData);
            _zero = zero;

            double[] peakFlowData2 = new double[] { 0, 500, 2000, 10000, 20000,40000,100000 };
            LogPearsonIII zero8 = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(peakFlowData);
            _zero8 = zero8;

            LogPearsonIII zero1 = new LogPearsonIII(4, .4, .5, 50);
            _zero1 = zero1;


            LogPearsonIII zero2 = new LogPearsonIII(2, .2, .2, 2);
            _zero2 = zero2;

            LogPearsonIII zero3 = new LogPearsonIII(5, .5, .5, 5);
            _zero3 = zero3;

            LogPearsonIII zero4 = new LogPearsonIII(1, 0, -1.5, 1);
            _zero4 = zero4;
            LogPearsonIII zero5 = new LogPearsonIII(9, .5, 1.5, 300);
            _zero5 = zero5;
            LogPearsonIII zero6 = new LogPearsonIII(9, 0, -1, 200);
            _zero6 = zero6;
            LogPearsonIII zero7 = new LogPearsonIII(3, .25, 0, 100);
            _zero7 = zero7;
           
        }



        [TestMethod]
        public void GetOrdinatesFunction_Test_Monotonic_Increasing_8()
        {
            OrdinatesFunction ord = _zero8.GetOrdinatesFunction();
            Assert.IsTrue(ord.Function.XValues.Count == ord.Function.YValues.Count);
            for (int i = 0; i < ord.Function.Count - 1; i++)
            {
                Assert.IsTrue(ord.Function.get_X(i) < ord.Function.get_X(i + 1));
                Assert.IsTrue(ord.Function.get_Y(i) < ord.Function.get_Y(i + 1));
            }

        }

        [TestMethod]
        public void GetOrdinatesFunction_Test_Monotonic_Increasing_7()
        {
            OrdinatesFunction ord = _zero7.GetOrdinatesFunction();
            Assert.IsTrue(ord.Function.XValues.Count == ord.Function.YValues.Count);
            for (int i = 0; i < ord.Function.Count - 1; i++)
            {
                Assert.IsTrue(ord.Function.get_X(i) < ord.Function.get_X(i + 1));
                Assert.IsTrue(ord.Function.get_Y(i) < ord.Function.get_Y(i + 1));
            }

        }

        [TestMethod]
        public void GetOrdinatesFunction_Test_Monotonic_Increasing_4()
        {
            OrdinatesFunction ord = _zero4.GetOrdinatesFunction();
            Assert.IsTrue(ord.Function.XValues.Count == ord.Function.YValues.Count);
            for (int i = 0; i < ord.Function.Count - 1; i++)
            {
                Assert.IsTrue(ord.Function.get_X(i) < ord.Function.get_X(i + 1));
                Assert.IsTrue(ord.Function.get_Y(i) < ord.Function.get_Y(i + 1));
            }

        }
        [TestMethod]
        public void GetOrdinatesFunction_Test_Monotonic_Increasing_5()
        {
            OrdinatesFunction ord = _zero5.GetOrdinatesFunction();
            Assert.IsTrue(ord.Function.XValues.Count == ord.Function.YValues.Count);
            for (int i = 0; i < ord.Function.Count - 1; i++)
            {
                Assert.IsTrue(ord.Function.get_X(i) < ord.Function.get_X(i + 1));
                Assert.IsTrue(ord.Function.get_Y(i) < ord.Function.get_Y(i + 1));
            }

        }
        [TestMethod]
        public void GetOrdinatesFunction_Test_Monotonic_Increasing_6()
        {
            OrdinatesFunction ord = _zero6.GetOrdinatesFunction();
            Assert.IsTrue(ord.Function.XValues.Count == ord.Function.YValues.Count);
            for (int i = 0; i < ord.Function.Count - 1; i++)
            {
                Assert.IsTrue(ord.Function.get_X(i) < ord.Function.get_X(i + 1));
                Assert.IsTrue(ord.Function.get_Y(i) < ord.Function.get_Y(i + 1));
            }

        }


        [TestMethod]
        public void GetOrdinatesFunction_Test_Monotonic_Increasing()
        {
            OrdinatesFunction ord = _zero.GetOrdinatesFunction();
            Assert.IsTrue(ord.Function.XValues.Count == ord.Function.YValues.Count);

            for (int i = 0; i < ord.Function.Count - 1; i++)
            {
                Assert.IsTrue(ord.Function.get_X(i) < ord.Function.get_X(i + 1));
                Assert.IsTrue(ord.Function.get_Y(i) < ord.Function.get_Y(i + 1));
            }

        }


        [TestMethod]
        public void GetOrdinatesFunction_Test_Monotonic_Increasing_1()
        {
            OrdinatesFunction ord = _zero1.GetOrdinatesFunction();
            Assert.IsTrue(ord.Function.XValues.Count == ord.Function.YValues.Count);
            for (int i = 0; i < ord.Function.Count - 1; i++)
            {
                Assert.IsTrue(ord.Function.get_X(i) < ord.Function.get_X(i + 1));
                Assert.IsTrue(ord.Function.get_Y(i) < ord.Function.get_Y(i + 1));
            }

        }

        [TestMethod]
        public void GetOrdinatesFunction_Test_Monotonic_Increasing_2()
        {
            OrdinatesFunction ord = _zero2.GetOrdinatesFunction();
            Assert.IsTrue(ord.Function.XValues.Count == ord.Function.YValues.Count);

            for (int i = 0; i < ord.Function.Count - 1; i++)
            {
                Assert.IsTrue(ord.Function.get_X(i) < ord.Function.get_X(i + 1));
                Assert.IsTrue(ord.Function.get_Y(i) < ord.Function.get_Y(i + 1));
            }

        }

        [TestMethod]
        public void GetOrdinatesFunction_Test_Monotonic_Increasing_3()
        {
            OrdinatesFunction ord = _zero3.GetOrdinatesFunction();
            Assert.IsTrue(ord.Function.XValues.Count == ord.Function.YValues.Count);

            for (int i = 0; i < ord.Function.Count - 1; i++)
            {
                Assert.IsTrue(ord.Function.get_X(i) < ord.Function.get_X(i + 1));
                Assert.IsTrue(ord.Function.get_Y(i) < ord.Function.get_Y(i + 1));
            }

        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\CSV Files\\FrequencyFunctionComposeTest.csv", "FrequencyFunctionComposeTest#csv", DataAccessMethod.Sequential)]//, DeploymentItem("ConditionTests\\OrdinatesFunctionInputs.csv")]

        [TestMethod]
        public void FrequencyFunctionCompose_Completely_Overllaping_Range_Test()
        {
 
            int functionTwo = Convert.ToInt32(TestContext.DataRow["Function2Type"]);
            FdaModel.Functions.FunctionTypes functionTwoType = getFunctionType(functionTwo);
            int expectedFuntionType = Convert.ToInt32(TestContext.DataRow["ExpectedOutputType"]);

            //Get function 1 xs
            string Func1xs = TestContext.DataRow["lp3Flows"].ToString();
            char space = ' ';
            string[] Func1xArray = Func1xs.Split(space);
            List<double> Func1xList = new List<double>();
            foreach (string item in Func1xArray)
            {
                Func1xList.Add(Convert.ToDouble(item));
            }
            double[] Func1xValues = Func1xList.ToArray();

            LogPearsonIII lp3 = new LogPearsonIII(Func1xValues);


            //double[] peakFlowData = new double[] { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000 };
            //LogPearsonIII zero = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(peakFlowData);

            string Func2xs = TestContext.DataRow["Function2xValues"].ToString();
            string[] Func2xArray = Func2xs.Split(space);
            List<double> Func2xList = new List<double>();
            foreach (string item in Func2xArray)
            {
                Func2xList.Add(Convert.ToDouble(item));
            }
            double[] Func2xValues = Func2xList.ToArray();

            //Get function 2 ys
            string Func2ys = TestContext.DataRow["Function2yValues"].ToString();
            string[] Func2yArray = Func2ys.Split(space);
            List<double> Func2yList = new List<double>();
            foreach (string item in Func2yArray)
            {
                Func2yList.Add(Convert.ToDouble(item));
            }
            double[] Func2yValues = Func2yList.ToArray();


            OrdinatesFunction ord = lp3.GetOrdinatesFunction();
            //FdaModel.Functions.FrequencyFunctions.LogPearsonIII functionA = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(2,.2,.2,200);
            FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction functionB = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(Func2xValues, Func2yValues, functionTwoType);

            //System.IO.StreamWriter sw = new System.IO.StreamWriter(@"C: \Users\q0heccdm\Documents\HEC FDA\Random Stuff\FrequencyComposeTesting.csv");
           
            //sw.WriteLine("Increasing,inflow,outflow,Increasing");

            bool firstValueIncreasing = false;
            bool secondValueIncreasing = false;
            bool xValuesAlwaysIncreasing = true;
            bool yValuesAlwaysIncreasing = true;

            for (int i = 0; i <functionB.Function.Count; i++)
            {
                
                //sw.WriteLine("t,"+functionB.Function.get_X(i) + "," + functionB.Function.get_Y(i) + ",t");
            }
            List<FdaModel.Utilities.Messager.ErrorMessage> errors = new List<FdaModel.Utilities.Messager.ErrorMessage>();
            OrdinatesFunction result = lp3.Compose(functionB,ref errors);

            //get 1% of y values
            double onePercentOfMaxY = functionB.Function.get_Y(functionB.Function.Count - 1) / 100;

            //sw.WriteLine("Increasing,frequency,outflow,Increasing");
            for (int i = 0; i < result.Function.Count; i++)
            {
                


                  if(i==0)
                {
                //sw.WriteLine("t,"+result.Function.get_X(i) + "," + result.Function.get_Y(i)+",t");

                }
                else
                {
                    Assert.IsTrue((result.Function.get_X(i) - result.Function.get_X(i - 1)) < .01);
                    Assert.IsTrue((result.Function.get_Y(i) - result.Function.get_Y(i - 1)) < onePercentOfMaxY);


                    if (result.Function.get_X(i) > result.Function.get_X(i - 1))
                    {firstValueIncreasing = true;}
                    else
                    { firstValueIncreasing = false; xValuesAlwaysIncreasing = false; }
                    if (result.Function.get_Y(i) > result.Function.get_Y(i - 1))
                    { secondValueIncreasing = true; }
                    else
                    { secondValueIncreasing = false; yValuesAlwaysIncreasing = false; }
                    //sw.WriteLine((firstValueIncreasing == true ? "t" : "False")+"," + result.Function.get_X(i) + "," + result.Function.get_Y(i) + "," +(secondValueIncreasing==true?"t":"False"));

                }

            }
            //sw.Close();

            Assert.IsTrue(xValuesAlwaysIncreasing == true,"X values are not increasing");
            Assert.IsTrue(yValuesAlwaysIncreasing == true,"Y values are not increasing");
            Assert.IsTrue(result.Function.get_Y(0) == functionB.Function.get_Y(0),"The first y value does not match the first y value from the ordinates function. Result y = " + result.Function.get_Y(0)+ " ord y =" + functionB.Function.get_Y(0));
            Assert.IsTrue(result.Function.get_Y(result.Function.Count - 1) == functionB.Function.get_Y(functionB.Function.Count - 1), "last y values do not match. Result y = " + result.Function.get_Y(result.Function.Count - 1) + " ord y =" + functionB.Function.get_Y(functionB.Function.Count - 1));
        
        }


        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\CSV Files\\FrequencyFunctionComposeOrdinatesFunctionHigherRangeTest.csv", "FrequencyFunctionComposeOrdinatesFunctionHigherRangeTest#csv", DataAccessMethod.Sequential)]//, DeploymentItem("ConditionTests\\OrdinatesFunctionInputs.csv")]

        [TestMethod]
        public void FrequencyFunctionCompose_Ordinates_Function_Has_Higher_Values_Test()
        {

            int functionTwo = Convert.ToInt32(TestContext.DataRow["Function2Type"]);
            FdaModel.Functions.FunctionTypes functionTwoType = getFunctionType(functionTwo);
            int expectedFuntionType = Convert.ToInt32(TestContext.DataRow["ExpectedOutputType"]);

            //Get function 1 xs
            string Func1xs = TestContext.DataRow["lp3Flows"].ToString();
            char space = ' ';
            string[] Func1xArray = Func1xs.Split(space);
            List<double> Func1xList = new List<double>();
            foreach (string item in Func1xArray)
            {
                Func1xList.Add(Convert.ToDouble(item));
            }
            double[] Func1xValues = Func1xList.ToArray();

            LogPearsonIII lp3 = new LogPearsonIII(Func1xValues);


            //double[] peakFlowData = new double[] { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000 };
            //LogPearsonIII zero = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(peakFlowData);

            string Func2xs = TestContext.DataRow["Function2xValues"].ToString();
            string[] Func2xArray = Func2xs.Split(space);
            List<double> Func2xList = new List<double>();
            foreach (string item in Func2xArray)
            {
                Func2xList.Add(Convert.ToDouble(item));
            }
            double[] Func2xValues = Func2xList.ToArray();

            //Get function 2 ys
            string Func2ys = TestContext.DataRow["Function2yValues"].ToString();
            string[] Func2yArray = Func2ys.Split(space);
            List<double> Func2yList = new List<double>();
            foreach (string item in Func2yArray)
            {
                Func2yList.Add(Convert.ToDouble(item));
            }
            double[] Func2yValues = Func2yList.ToArray();


            OrdinatesFunction ord = lp3.GetOrdinatesFunction();
            //FdaModel.Functions.FrequencyFunctions.LogPearsonIII functionA = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(2,.2,.2,200);
            FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction functionB = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(Func2xValues, Func2yValues, functionTwoType);

            //System.IO.StreamWriter sw = new System.IO.StreamWriter(@"C: \Users\q0heccdm\Documents\HEC FDA\Random Stuff\FrequencyComposeTesting.csv");

            //sw.WriteLine("Increasing,inflow,outflow,Increasing");

            bool firstValueIncreasing = false;
            bool secondValueIncreasing = false;
            bool xValuesAlwaysIncreasing = true;
            bool yValuesAlwaysIncreasing = true;

            for (int i = 0; i < functionB.Function.Count; i++)
            {

                //sw.WriteLine("t," + functionB.Function.get_X(i) + "," + functionB.Function.get_Y(i) + ",t");
            }
            List<FdaModel.Utilities.Messager.ErrorMessage> errors = new List<FdaModel.Utilities.Messager.ErrorMessage>();
            OrdinatesFunction result = lp3.Compose(functionB, ref errors);

            //get 1% of y values
            double onePercentOfMaxY = functionB.Function.get_Y(functionB.Function.Count - 1) / 100;

            //sw.WriteLine("Increasing,frequency,outflow,Increasing");
            for (int i = 0; i < result.Function.Count; i++)
            {



                if (i == 0)
                {
                    //sw.WriteLine("t," + result.Function.get_X(i) + "," + result.Function.get_Y(i) + ",t");

                }
                else
                {
                    Assert.IsTrue((result.Function.get_X(i) - result.Function.get_X(i - 1)) < .01);
                    Assert.IsTrue((result.Function.get_Y(i) - result.Function.get_Y(i - 1)) < onePercentOfMaxY);


                    if (result.Function.get_X(i) > result.Function.get_X(i - 1))
                    { firstValueIncreasing = true; }
                    else
                    { firstValueIncreasing = false; xValuesAlwaysIncreasing = false; }
                    if (result.Function.get_Y(i) > result.Function.get_Y(i - 1))
                    { secondValueIncreasing = true; }
                    else
                    { secondValueIncreasing = false; yValuesAlwaysIncreasing = false; }
                    //sw.WriteLine((firstValueIncreasing == true ? "t" : "False") + "," + result.Function.get_X(i) + "," + result.Function.get_Y(i) + "," + (secondValueIncreasing == true ? "t" : "False"));

                }

            }
            //sw.Close();

            Assert.IsTrue(xValuesAlwaysIncreasing == true, "X values are not increasing");
            Assert.IsTrue(yValuesAlwaysIncreasing == true, "Y values are not increasing");
            Assert.IsTrue(result.Function.get_Y(0) == functionB.Function.get_Y(0), "The first y value does not match the first y value from the ordinates function. Result y = " + result.Function.get_Y(0) + " ord y =" + functionB.Function.get_Y(0));
            //Assert.IsTrue(result.Function.get_Y(result.Function.Count - 1) == functionB.Function.get_Y(functionB.Function.Count - 1), "last y values do not match. Result y = " + result.Function.get_Y(result.Function.Count - 1) + " ord y =" + functionB.Function.get_Y(functionB.Function.Count - 1));

        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\CSV Files\\FrequencyFunctionComposeOrdinatesFunctionLowerRangeTest.csv", "FrequencyFunctionComposeOrdinatesFunctionLowerRangeTest#csv", DataAccessMethod.Sequential)]//, DeploymentItem("ConditionTests\\OrdinatesFunctionInputs.csv")]

        [TestMethod]
        public void FrequencyFunctionCompose_Ordinates_Function_Has_Lower_Values_Test()
        {

            int functionTwo = Convert.ToInt32(TestContext.DataRow["Function2Type"]);
            FdaModel.Functions.FunctionTypes functionTwoType = getFunctionType(functionTwo);
            int expectedFuntionType = Convert.ToInt32(TestContext.DataRow["ExpectedOutputType"]);

            //Get function 1 xs
            string Func1xs = TestContext.DataRow["lp3Flows"].ToString();
            char space = ' ';
            string[] Func1xArray = Func1xs.Split(space);
            List<double> Func1xList = new List<double>();
            foreach (string item in Func1xArray)
            {
                Func1xList.Add(Convert.ToDouble(item));
            }
            double[] Func1xValues = Func1xList.ToArray();

            LogPearsonIII lp3 = new LogPearsonIII(Func1xValues);


            //double[] peakFlowData = new double[] { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000 };
            //LogPearsonIII zero = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(peakFlowData);

            string Func2xs = TestContext.DataRow["Function2xValues"].ToString();
            string[] Func2xArray = Func2xs.Split(space);
            List<double> Func2xList = new List<double>();
            foreach (string item in Func2xArray)
            {
                Func2xList.Add(Convert.ToDouble(item));
            }
            double[] Func2xValues = Func2xList.ToArray();

            //Get function 2 ys
            string Func2ys = TestContext.DataRow["Function2yValues"].ToString();
            string[] Func2yArray = Func2ys.Split(space);
            List<double> Func2yList = new List<double>();
            foreach (string item in Func2yArray)
            {
                Func2yList.Add(Convert.ToDouble(item));
            }
            double[] Func2yValues = Func2yList.ToArray();


            OrdinatesFunction ord = lp3.GetOrdinatesFunction();
            //FdaModel.Functions.FrequencyFunctions.LogPearsonIII functionA = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(2,.2,.2,200);
            FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction functionB = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(Func2xValues, Func2yValues, functionTwoType);

            //System.IO.StreamWriter sw = new System.IO.StreamWriter(@"C: \Users\q0heccdm\Documents\HEC FDA\Random Stuff\FrequencyComposeTesting.csv");

           // sw.WriteLine("Increasing,inflow,outflow,Increasing");

            bool firstValueIncreasing = false;
            bool secondValueIncreasing = false;
            bool xValuesAlwaysIncreasing = true;
            bool yValuesAlwaysIncreasing = true;

            for (int i = 0; i < functionB.Function.Count; i++)
            {

                //sw.WriteLine("t," + functionB.Function.get_X(i) + "," + functionB.Function.get_Y(i) + ",t");
            }
            List<FdaModel.Utilities.Messager.ErrorMessage> errors = new List<FdaModel.Utilities.Messager.ErrorMessage>();
            OrdinatesFunction result = lp3.Compose(functionB, ref errors);

            //get 1% of y values
            double onePercentOfMaxY = functionB.Function.get_Y(functionB.Function.Count - 1) / 100;

            //sw.WriteLine("Increasing,frequency,outflow,Increasing");
            for (int i = 0; i < result.Function.Count; i++)
            {



                if (i == 0)
                {
                    //sw.WriteLine("t," + result.Function.get_X(i) + "," + result.Function.get_Y(i) + ",t");

                }
                else
                {
                    Assert.IsTrue((result.Function.get_X(i) - result.Function.get_X(i - 1)) < .01);
                    Assert.IsTrue((result.Function.get_Y(i) - result.Function.get_Y(i - 1)) < onePercentOfMaxY);


                    if (result.Function.get_X(i) > result.Function.get_X(i - 1))
                    { firstValueIncreasing = true; }
                    else
                    { firstValueIncreasing = false; xValuesAlwaysIncreasing = false; }
                    if (result.Function.get_Y(i) > result.Function.get_Y(i - 1))
                    { secondValueIncreasing = true; }
                    else
                    { secondValueIncreasing = false; yValuesAlwaysIncreasing = false; }
                    //sw.WriteLine((firstValueIncreasing == true ? "t" : "False") + "," + result.Function.get_X(i) + "," + result.Function.get_Y(i) + "," + (secondValueIncreasing == true ? "t" : "False"));

                }

            }
           // sw.Close();

            Assert.IsTrue(xValuesAlwaysIncreasing == true, "X values are not increasing");
            Assert.IsTrue(yValuesAlwaysIncreasing == true, "Y values are not increasing");
            //Assert.IsTrue(result.Function.get_Y(0) == functionB.Function.get_Y(0), "The first y value does not match the first y value from the ordinates function. Result y = " + result.Function.get_Y(0) + " ord y =" + functionB.Function.get_Y(0));
            Assert.IsTrue(result.Function.get_Y(result.Function.Count - 1) == functionB.Function.get_Y(functionB.Function.Count - 1), "last y values do not match. Result y = " + result.Function.get_Y(result.Function.Count - 1) + " ord y =" + functionB.Function.get_Y(functionB.Function.Count - 1));

        }


        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\CSV Files\\FrequencyFunctionComposeOrdinatesFunctionEntirelyAboveTest.csv", "FrequencyFunctionComposeOrdinatesFunctionEntirelyAboveTest#csv", DataAccessMethod.Sequential)]//, DeploymentItem("ConditionTests\\OrdinatesFunctionInputs.csv")]

        [TestMethod]
        public void FrequencyFunctionCompose_Ordinates_Function_Is_Entirely_Above_LP3_Test()
        {
            //  In this test there should be no overlap and the resulting function should be null

            int functionTwo = Convert.ToInt32(TestContext.DataRow["Function2Type"]);
            FdaModel.Functions.FunctionTypes functionTwoType = getFunctionType(functionTwo);
            int expectedFuntionType = Convert.ToInt32(TestContext.DataRow["ExpectedOutputType"]);

            //Get function 1 xs
            string Func1xs = TestContext.DataRow["lp3Flows"].ToString();
            char space = ' ';
            string[] Func1xArray = Func1xs.Split(space);
            List<double> Func1xList = new List<double>();
            foreach (string item in Func1xArray)
            {
                Func1xList.Add(Convert.ToDouble(item));
            }
            double[] Func1xValues = Func1xList.ToArray();

            LogPearsonIII lp3 = new LogPearsonIII(Func1xValues);


            //double[] peakFlowData = new double[] { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000 };
            //LogPearsonIII zero = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(peakFlowData);

            string Func2xs = TestContext.DataRow["Function2xValues"].ToString();
            string[] Func2xArray = Func2xs.Split(space);
            List<double> Func2xList = new List<double>();
            foreach (string item in Func2xArray)
            {
                Func2xList.Add(Convert.ToDouble(item));
            }
            double[] Func2xValues = Func2xList.ToArray();

            //Get function 2 ys
            string Func2ys = TestContext.DataRow["Function2yValues"].ToString();
            string[] Func2yArray = Func2ys.Split(space);
            List<double> Func2yList = new List<double>();
            foreach (string item in Func2yArray)
            {
                Func2yList.Add(Convert.ToDouble(item));
            }
            double[] Func2yValues = Func2yList.ToArray();


            OrdinatesFunction ord = lp3.GetOrdinatesFunction();
            //FdaModel.Functions.FrequencyFunctions.LogPearsonIII functionA = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(2,.2,.2,200);
            FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction functionB = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(Func2xValues, Func2yValues, functionTwoType);

            List<FdaModel.Utilities.Messager.ErrorMessage> errors = new List<FdaModel.Utilities.Messager.ErrorMessage>();
            OrdinatesFunction result = lp3.Compose(functionB, ref errors);


            Assert.IsTrue(result == null,"The resulting ordinates function was not null");

            
        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\CSV Files\\FrequencyFunctionComposeOrdinatesFunctionEntirelyBelowTest.csv", "FrequencyFunctionComposeOrdinatesFunctionEntirelyBelowTest#csv", DataAccessMethod.Sequential)]//, DeploymentItem("ConditionTests\\OrdinatesFunctionInputs.csv")]

        [TestMethod]
        public void FrequencyFunctionCompose_Ordinates_Function_Is_Entirely_Below_LP3_Test()
        {
            //  In this test there should be no overlap and the resulting function should be null

            int functionTwo = Convert.ToInt32(TestContext.DataRow["Function2Type"]);
            FdaModel.Functions.FunctionTypes functionTwoType = getFunctionType(functionTwo);
            int expectedFuntionType = Convert.ToInt32(TestContext.DataRow["ExpectedOutputType"]);

            //Get function 1 xs
            string Func1xs = TestContext.DataRow["lp3Flows"].ToString();
            char space = ' ';
            string[] Func1xArray = Func1xs.Split(space);
            List<double> Func1xList = new List<double>();
            foreach (string item in Func1xArray)
            {
                Func1xList.Add(Convert.ToDouble(item));
            }
            double[] Func1xValues = Func1xList.ToArray();

            LogPearsonIII lp3 = new LogPearsonIII(Func1xValues);


            //double[] peakFlowData = new double[] { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000 };
            //LogPearsonIII zero = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(peakFlowData);

            string Func2xs = TestContext.DataRow["Function2xValues"].ToString();
            string[] Func2xArray = Func2xs.Split(space);
            List<double> Func2xList = new List<double>();
            foreach (string item in Func2xArray)
            {
                Func2xList.Add(Convert.ToDouble(item));
            }
            double[] Func2xValues = Func2xList.ToArray();

            //Get function 2 ys
            string Func2ys = TestContext.DataRow["Function2yValues"].ToString();
            string[] Func2yArray = Func2ys.Split(space);
            List<double> Func2yList = new List<double>();
            foreach (string item in Func2yArray)
            {
                Func2yList.Add(Convert.ToDouble(item));
            }
            double[] Func2yValues = Func2yList.ToArray();


            OrdinatesFunction ord = lp3.GetOrdinatesFunction();
            //FdaModel.Functions.FrequencyFunctions.LogPearsonIII functionA = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(2,.2,.2,200);
            FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction functionB = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(Func2xValues, Func2yValues, functionTwoType);

            List<FdaModel.Utilities.Messager.ErrorMessage> errors = new List<FdaModel.Utilities.Messager.ErrorMessage>();
            OrdinatesFunction result = lp3.Compose(functionB, ref errors);


            Assert.IsTrue(result == null, "The resulting ordinates function was not null");


        }


        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\CSV Files\\FrequencyFunctionComposeOrdinatesFunctionAboveAndBelowTest.csv", "FrequencyFunctionComposeOrdinatesFunctionAboveAndBelowTest#csv", DataAccessMethod.Sequential)]//, DeploymentItem("ConditionTests\\OrdinatesFunctionInputs.csv")]

        [TestMethod]
        public void FrequencyFunctionCompose_Ordinates_Function_Is_Above_And_Below_LP3_Test()
        {
            int functionTwo = Convert.ToInt32(TestContext.DataRow["Function2Type"]);
            FdaModel.Functions.FunctionTypes functionTwoType = getFunctionType(functionTwo);
            int expectedFuntionType = Convert.ToInt32(TestContext.DataRow["ExpectedOutputType"]);

            //Get function 1 xs
            string Func1xs = TestContext.DataRow["lp3Flows"].ToString();
            char space = ' ';
            string[] Func1xArray = Func1xs.Split(space);
            List<double> Func1xList = new List<double>();
            foreach (string item in Func1xArray)
            {
                Func1xList.Add(Convert.ToDouble(item));
            }
            double[] Func1xValues = Func1xList.ToArray();

            LogPearsonIII lp3 = new LogPearsonIII(Func1xValues);


            //double[] peakFlowData = new double[] { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000 };
            //LogPearsonIII zero = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(peakFlowData);

            string Func2xs = TestContext.DataRow["Function2xValues"].ToString();
            string[] Func2xArray = Func2xs.Split(space);
            List<double> Func2xList = new List<double>();
            foreach (string item in Func2xArray)
            {
                Func2xList.Add(Convert.ToDouble(item));
            }
            double[] Func2xValues = Func2xList.ToArray();

            //Get function 2 ys
            string Func2ys = TestContext.DataRow["Function2yValues"].ToString();
            string[] Func2yArray = Func2ys.Split(space);
            List<double> Func2yList = new List<double>();
            foreach (string item in Func2yArray)
            {
                Func2yList.Add(Convert.ToDouble(item));
            }
            double[] Func2yValues = Func2yList.ToArray();


            OrdinatesFunction ord = lp3.GetOrdinatesFunction();
            //FdaModel.Functions.FrequencyFunctions.LogPearsonIII functionA = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(2,.2,.2,200);
            FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction functionB = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(Func2xValues, Func2yValues, functionTwoType);

            //System.IO.StreamWriter sw = new System.IO.StreamWriter(@"C: \Users\q0heccdm\Documents\HEC FDA\Random Stuff\FrequencyComposeTesting.csv");

           // sw.WriteLine("Increasing,inflow,outflow,Increasing");

            bool firstValueIncreasing = false;
            bool secondValueIncreasing = false;
            bool xValuesAlwaysIncreasing = true;
            bool yValuesAlwaysIncreasing = true;

            for (int i = 0; i < functionB.Function.Count; i++)
            {

                //sw.WriteLine("t," + functionB.Function.get_X(i) + "," + functionB.Function.get_Y(i) + ",t");
            }
            List<FdaModel.Utilities.Messager.ErrorMessage> errors = new List<FdaModel.Utilities.Messager.ErrorMessage>();
            OrdinatesFunction result = lp3.Compose(functionB, ref errors);

            //get 1% of y values
            double onePercentOfMaxY = functionB.Function.get_Y(functionB.Function.Count - 1) / 100;

            //sw.WriteLine("Increasing,frequency,outflow,Increasing");
            for (int i = 0; i < result.Function.Count; i++)
            {



                if (i == 0)
                {
                    //sw.WriteLine("t," + result.Function.get_X(i) + "," + result.Function.get_Y(i) + ",t");

                }
                else
                {
                    Assert.IsTrue((result.Function.get_X(i) - result.Function.get_X(i - 1)) < .01,"x values increased by more than 1 percent.");
                    Assert.IsTrue((result.Function.get_Y(i) - result.Function.get_Y(i - 1)) < onePercentOfMaxY, "y values increased by more than 1 percent.");


                    if (result.Function.get_X(i) > result.Function.get_X(i - 1))
                    { firstValueIncreasing = true; }
                    else
                    { firstValueIncreasing = false; xValuesAlwaysIncreasing = false; }
                    if (result.Function.get_Y(i) > result.Function.get_Y(i - 1))
                    { secondValueIncreasing = true; }
                    else
                    { secondValueIncreasing = false; yValuesAlwaysIncreasing = false; }
                   // sw.WriteLine((firstValueIncreasing == true ? "t" : "False") + "," + result.Function.get_X(i) + "," + result.Function.get_Y(i) + "," + (secondValueIncreasing == true ? "t" : "False"));

                }

            }
            //sw.Close();

            Assert.IsTrue(xValuesAlwaysIncreasing == true, "X values are not increasing");
            Assert.IsTrue(yValuesAlwaysIncreasing == true, "Y values are not increasing");
            //Assert.IsTrue(result.Function.get_Y(0) == functionB.Function.get_Y(0), "The first y value does not match the first y value from the ordinates function. Result y = " + result.Function.get_Y(0) + " ord y =" + functionB.Function.get_Y(0));
            //Assert.IsTrue(result.Function.get_Y(result.Function.Count - 1) == functionB.Function.get_Y(functionB.Function.Count - 1), "last y values do not match. Result y = " + result.Function.get_Y(result.Function.Count - 1) + " ord y =" + functionB.Function.get_Y(functionB.Function.Count - 1));


        }


        private FdaModel.Functions.FunctionTypes getFunctionType(int number)
        {
            FdaModel.Functions.FunctionTypes funcType = FdaModel.Functions.FunctionTypes.UnUsed;


            switch (number)
            {
                case 0:
                    {
                        funcType = FdaModel.Functions.FunctionTypes.InflowFrequency;
                        break;
                    }
                case 1:
                    {
                        funcType = FdaModel.Functions.FunctionTypes.InflowOutflow;
                        break;
                    }
                case 2:
                    {
                        funcType = FdaModel.Functions.FunctionTypes.OutflowFrequency;
                        break;
                    }
                case 3:
                    {
                        funcType = FdaModel.Functions.FunctionTypes.Rating;
                        break;
                    }
                case 4:
                    {
                        funcType = FdaModel.Functions.FunctionTypes.ExteriorStageFrequency;
                        break;
                    }
                case 5:
                    {
                        funcType = FdaModel.Functions.FunctionTypes.ExteriorInteriorStage;
                        break;
                    }
                case 6:
                    {
                        funcType = FdaModel.Functions.FunctionTypes.InteriorStageFrequency;
                        break;
                    }
                case 7:
                    {
                        funcType = FdaModel.Functions.FunctionTypes.InteriorStageDamage;
                        break;
                    }
                case 8:
                    {
                        funcType = FdaModel.Functions.FunctionTypes.DamageFrequency;
                        break;
                    }
                case 9:
                    {
                        funcType = FdaModel.Functions.FunctionTypes.LeveeFailure;
                        break;
                    }
                case 99:
                    {
                        funcType = FdaModel.Functions.FunctionTypes.UnUsed;
                        break;
                    }
            }


            return funcType;
        }


    }
}
