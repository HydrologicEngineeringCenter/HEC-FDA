using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using FdaModel;
using FdaModel.Functions.FrequencyFunctions;
using FdaModel.Functions.OrdinatesFunctions;
using FdaModel.OtherComputationPointElements;


namespace FdaTester.ModelTester.UnitTests.Compute_Testing
{
    //[Author(q0heccdm, 11 / 4 / 2016 1:12:50 PM)]
    class SimpleTestCase
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 11/4/2016 1:12:50 PM
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public static void SimpleTest( )
        {
            //1. Create computation point
            ComputationPoint indexPoint = new ComputationPoint("anIndexPoint", "aStream", "intheWoP", 1999);

            //2. Create lp3 function
            double[ ] peakFlowData = new double[ ] { 100, 200, 300, 400, 500 };
            LogPearsonIII myLP3 = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII( peakFlowData);

            //3. Create ordinates function
            float[ ] xs = new float[ ] { 0, 100, 200, 500, 1000 };
            float[ ] ys = new float[ ] { 0, 1, 2, 5, 10 };
            OrdinatesFunction rating = (OrdinatesFunction) OrdinatesFunction.OrdinatesFunctionFactory( xs, ys, FdaModel.Functions.FunctionTypes.Rating);

            //4. Create Threshold
            PerformanceThreshold threshold = new PerformanceThreshold(indexPoint, PerformanceThresholdTypes.OtherExteriorStage, 5d);

            //5. Create computable object
            List<FdaModel.Functions.BaseFunction> myListOfBaseFunctions = new List<FdaModel.Functions.BaseFunction>( ) { myLP3, rating };
            //ComputableFunctions simpleTest = new ComputableFunctions(myListOfBaseFunctions, threshold, null);



            Random randomNumberGenerator = new Random(0);
           // ComputedObject simpleTestResult = simpleTest.ComputeRealization(randomNumberGenerator, true);

            //double aep = simpleTestResult.AnnualExceedanceProbabilities;
            double expectedresult = 1 - myLP3.Function.GetCDF(500);

            //System.Windows.MessageBox.Show("AEP: " + aep + "   Expected Result:" + expectedresult);
        }

        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
    }
}
