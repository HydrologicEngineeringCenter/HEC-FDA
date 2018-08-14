using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using FdaModel.Functions.FrequencyFunctions;
using FdaModel.Functions.OrdinatesFunctions;
using FdaModel.ComputationPoint;


namespace FdaTester.ModelTester.UnitTests.Compute_Testing.Tests
{
    //[Author(q0heccdm, 12 / 13 / 2016 2:10:43 PM)]
    public class Outflow_Rating:BaseComputeTest
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/13/2016 2:10:43 PM
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public Outflow_Rating()
        {
            Name = "Outflow, Rating";

            //1. Create computation point
            //ComputationPoint indexPoint = new ComputationPoint("anIndexPoint", "aStream", "intheWoP", 1999);

            //2. Create lp3 function
            double[] peakFlowData = new double[] { 100, 200, 300, 1000, 5000 };
            LogPearsonIII zero = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(peakFlowData);

            LogPearsonIII zero2 = new LogPearsonIII(3.1, .4, .5, 50);

            // Create ordinates function
            double[] x = new double[] { 100, 200, 500, 1000, 2000 };
            double[] y = new double[] { 200, 300, 600, 1100, 2100 };
            OrdinatesFunction one = new OrdinatesFunction(x, y, FdaModel.Functions.FunctionTypes.InflowOutflow);

            double[] x1 = new double[] { .1f,.2f, .3f, .4f,.5f,.6f };
            double[] y1 = new double[] { 0,1,2,4,5};
            OrdinatesFunction two = new OrdinatesFunction(x1, y1, FdaModel.Functions.FunctionTypes.OutflowFrequency);

            //3.Create ordinates function
            double[] xs = new double[] { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1100, 1200, 1300, 1400, 1500, 1600, 1700, 1800, 1900, 2000 };
            double[] ys = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            OrdinatesFunction three = new OrdinatesFunction(xs, ys, FdaModel.Functions.FunctionTypes.Rating);

            //float[] xs = new float[] { 0,3,5 };
            //float[] ys = new float[] { 10,11,12 };
            //OrdinatesFunction three = new OrdinatesFunction(xs, ys, FdaModel.Functions.FunctionTypes.Rating);

            double[] x2 = new double[] { 0, 100, 200, 500, 1000 };
            double[] y2 = new double[] { 2, 200, 300, 600, 1100 };
            OrdinatesFunction four = new OrdinatesFunction(x2, y2, FdaModel.Functions.FunctionTypes.ExteriorStageFrequency);

            //float[] xval = new float[] { 5, 6, 7, 8, 9, 10, 11 };
            //float[] yval = new float[] { 6, 7, 8, 9, 10, 11, 12 };
            //OrdinatesFunction five = new OrdinatesFunction(xval, yval, FdaModel.Functions.FunctionTypes.ExteriorInteriorStage);

            //float[] x3 = new float[] { 0, 100, 200, 500, 1000 };
            //float[] y3 = new float[] { 2, 200, 300, 600, 1100 };
            //OrdinatesFunction six = new OrdinatesFunction(x3, y3, FdaModel.Functions.FunctionTypes.InteriorStageFrequency);

            double[] x4 = new double[] { 0, 1, 2, 5, 7, 8, 9, 10, 12, 15,20 };
            double[] y4 = new double[] { 3,3,3,600,1100,1300,1800,10000,30000,100000,500000};
            OrdinatesFunction seven = new OrdinatesFunction(x4, y4, FdaModel.Functions.FunctionTypes.InteriorStageDamage);

            //float[] x5 = new float[] { 0, 100, 200, 500, 1000 };
            //float[] y5 = new float[] { 2, 200, 300, 600, 1100 };
            //OrdinatesFunction eight = new OrdinatesFunction(x5, y5, FdaModel.Functions.FunctionTypes.DamageFrequency);

            //float[] x6 = new float[] { 0, 100, 200, 500, 1000 };
            //float[] y6 = new float[] { 2, 200, 300, 600, 1100 };
            //OrdinatesFunction nine = new OrdinatesFunction(x6, y6, FdaModel.Functions.FunctionTypes.LeveeFailure);

            //float[] x7 = new float[] { 0, 100, 200, 500, 1000 };
            //float[] y7 = new float[] { 2, 200, 300, 600, 1100 };
            //OrdinatesFunction ninetyNine = new OrdinatesFunction(x7, y7, FdaModel.Functions.FunctionTypes.UnUsed);

            //4. Create Threshold
            PerformanceThreshold threshold = new PerformanceThreshold(PerformanceThresholdTypes.Damage, 50000d);
            //5. Create computable object
            List<FdaModel.Functions.BaseFunction> myListOfBaseFunctions = new List<FdaModel.Functions.BaseFunction>() { zero, three, seven };
            InputFunctions = myListOfBaseFunctions;
            Condition simpleTest = new Condition(1999, "cody", myListOfBaseFunctions, threshold, null);
            ConditionErrors = simpleTest.Messages.Messages;

            AllComputableFunctions = simpleTest;


            Random randomNumberGenerator = new Random(0);

            FdaModel.ComputationPoint.Outputs.Realization simpleTestResult = new FdaModel.ComputationPoint.Outputs.Realization(simpleTest, false, false);
            RealizationErrors = simpleTestResult.Messages.Messages;

            simpleTestResult.Compute(randomNumberGenerator);
            //FdaModel.ComputationPoint.Outputs.Realization simpleTestResult = simpleTest.ComputeRealization(randomNumberGenerator, true);

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
