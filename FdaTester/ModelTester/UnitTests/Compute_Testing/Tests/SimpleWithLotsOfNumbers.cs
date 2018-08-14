using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using FdaModel;
using FdaModel.Functions.FrequencyFunctions;
using FdaModel.Functions.OrdinatesFunctions;
using FdaModel.OtherComputationPointElements;

namespace FdaTester.ModelTester.UnitTests.Compute_Testing.Tests
{
    //[Author(q0heccdm, 12 / 13 / 2016 11:37:33 AM)]
    class SimpleWithLotsOfNumbers:BaseComputeTest
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/13/2016 11:37:33 AM
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public SimpleWithLotsOfNumbers():base()
        {
            Name = "Simple with lots of Numbers";
            //1. Create computation point
            ComputationPoint indexPoint = new ComputationPoint("anIndexPoint", "aStream", "intheWoP", 1999);

            //2. Create lp3 function
            List<double> peakFlowList = new List<double>();
            for (int i = 1; i < 1000; i++)
            {
                peakFlowList.Add(i * 10);
            }
            double[] peakFlowData = peakFlowList.ToArray();
            //double[] peakFlowData = new double[] { 100, 200, 300, 400, 500 };

            LogPearsonIII myLP3 = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII( peakFlowData);


            //3. Create ordinates function
            
            List<float> xsFloat = new List<float>();
            for (int i = 1; i < 1000; i++)
            {
                xsFloat.Add(i * 10);
            }

            List<float> ysFloat = new List<float>();
            for (int i = 1; i < 1000; i++)
            {
                ysFloat.Add(i * 10);
            }

            float[] xs = xsFloat.ToArray();
            float[] ys = ysFloat.ToArray();
            OrdinatesFunction rating = (OrdinatesFunction)OrdinatesFunction.OrdinatesFunctionFactory( xs, ys, FdaModel.Functions.FunctionTypes.Rating);

            //4. Create Threshold
            PerformanceThreshold threshold = new PerformanceThreshold(indexPoint, PerformanceThresholdTypes.OtherExteriorStage, 1000d);

            //5. Create computable object
            List<FdaModel.Functions.BaseFunction> myListOfBaseFunctions = new List<FdaModel.Functions.BaseFunction>() { myLP3, rating };
            ComputableFunctions simpleTest = new ComputableFunctions(myListOfBaseFunctions, threshold, null);



            Random randomNumberGenerator = new Random(0);
            ComputedObject simpleTestResult = simpleTest.ComputeRealization(randomNumberGenerator, true);

            //double aep = simpleTestResult.AnnualExceedanceProbabilities;
            // double expectedresult = 1 - myLP3.Function.GetCDF(500);

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
