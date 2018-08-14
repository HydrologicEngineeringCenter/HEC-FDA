using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using FdaModel.Functions.FrequencyFunctions;
using FdaModel.Functions.OrdinatesFunctions;
using FdaModel.OtherComputationPointElements;

namespace FdaTester.ModelTester.UnitTests.Compute_Testing.Tests
{
    //[Author(q0heccdm, 12 / 22 / 2016 10:36:38 AM)]
    public class AllFunctionsTest:BaseComputeTest
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/22/2016 10:36:38 AM
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors

        public AllFunctionsTest():base()
        {
            Name = "All Functions";

            //1. Create computation point
            ComputationPoint indexPoint = new ComputationPoint("anIndexPoint", "aStream", "intheWoP", 1999);

            //2. Create lp3 function
           // double[] peakFlowData = new double[] { 100, 200, 300, 400, 1000 };
            //LogPearsonIII myLP3 = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(peakFlowData);

            // Create ordinates function
            float[] x = new float[] { 0, 100, 200, 500, 1000 };
            float[] y = new float[] { 2, 200, 300, 600, 1100 };
            //OrdinatesFunction inflowOutflow = (OrdinatesFunction)OrdinatesFunction.OrdinatesFunctionFactory(x, y, FdaModel.Functions.FunctionTypes.InflowOutflow);

            int numOfFunctions = 5;
            int functionRange = 10;// this number should be one more than the one max value.
            Random rand = new Random();
            List<int> usedNumbers = new List<int>();
            bool alreadyHaveEven = false;

            List<FdaModel.Functions.BaseFunction> myListOfBaseFunctions = new List<FdaModel.Functions.BaseFunction>();

            for (int i =0;i<numOfFunctions;i++)
            {
                int funcType = rand.Next(0, functionRange);
                //if(usedNumbers.Contains(funcType) || (alreadyHaveEven ==true && funcType %2 == 0))
                //{
                //    i--;
                //    continue;
                //}
                //else if(funcType %2 == 0)
                //{
                //    alreadyHaveEven = true;
                //}
                //usedNumbers.Add(funcType);
                FdaModel.Functions.FunctionTypes tempFuncType = (FdaModel.Functions.FunctionTypes)funcType; 
                OrdinatesFunction temp = (OrdinatesFunction)OrdinatesFunction.OrdinatesFunctionFactory(x, y, tempFuncType);
                myListOfBaseFunctions.Add(temp);
            }

            Console.WriteLine("input Functions:");
            InputFunctions = new List<FdaModel.Functions.BaseFunction>();
            foreach(FdaModel.Functions.BaseFunction func in myListOfBaseFunctions)
            {
                InputFunctions.Add(func);

                Console.WriteLine((int)func.FunctionType);
            }
            //4. Create Threshold
            PerformanceThreshold threshold = new PerformanceThreshold(indexPoint, (PerformanceThresholdTypes)rand.Next(0,5), 200d);

            //5. Create computable object
            ComputableFunctions simpleTest = new ComputableFunctions(myListOfBaseFunctions, threshold, null);

            AllComputableFunctions = simpleTest;



            Random randomNumberGenerator = new Random(0);
            ComputedObject simpleTestResult = simpleTest.ComputeRealization(randomNumberGenerator, true);

            //double aep = simpleTestResult.AnnualExceedanceProbabilities;
            //double expectedresult = 1 - myLP3.Function.GetCDF(500);

            //System.Windows.MessageBox.Show("AEP: " + aep + "   Expected Result:" + expectedresult);
            MyComputedObject = simpleTestResult;
        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
    }
}
