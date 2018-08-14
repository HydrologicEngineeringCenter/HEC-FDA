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
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace FdaTester.ModelTester.UnitTests.Compute_Testing.Tests
{
    //[Author(q0heccdm, 12 / 15 / 2016 11:19:43 AM)]
    public class InflowFrequency_InflowOutflow_Rating_0_1_3_:BaseComputeTest
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/15/2016 11:19:43 AM
        #endregion
        #region Fields
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
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public InflowFrequency_InflowOutflow_Rating_0_1_3_():base()
        {
            Name = "InflowFrequency_InflowOutflow_Rating (0_1_3)";

            //1. Create computation point
            //ComputationPoint indexPoint = new ComputationPoint("anIndexPoint", "aStream", "intheWoP", 1999);

            //2. Create lp3 function
            //double[] peakFlowData = new double[] { 100, 200, 300,400,500,600,700,800,900, 1000,1500,2000,2500,3000,3500,4000,4500, 5000 };
            //LogPearsonIII zero = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII( peakFlowData);

            //LogPearsonIII zero1 = new LogPearsonIII(4,.4, .5, 50);

            //double[] xblah = new double[] { 0,0,0,3,5 };
            //double[] yblah = new double[] { 0,.2,.5,.7,.9};
            //OrdinatesFunction testOrdinatesFunction = new OrdinatesFunction(xblah, yblah, FdaModel.Functions.FunctionTypes.LeveeFailure);

            //// Create ordinates function
            //double[] x = new double[] {  100, 200, 500, 1000,2000 };
            //double[] y = new double[] {  200, 300, 600, 1100, 2100 };
            //OrdinatesFunction one = new OrdinatesFunction(x, y, FdaModel.Functions.FunctionTypes.InflowOutflow);

            //double[] x1 = new double[] { .2f,.3f,.4f,.5f,.6f,.7f,.8f,.9f};
            //double[] y1 = new double[] {  4000,5000,10000,20000,30000,50000,80000,120000 };
            //OrdinatesFunction two = new OrdinatesFunction(x1, y1, FdaModel.Functions.FunctionTypes.OutflowFrequency);

            ////3. Create ordinates function
            //double[] xs = new double[] { 100,200,500,1000,2000};
            //double[] ys = new double[] { 1,2,5,10,20};
            //OrdinatesFunction three = new OrdinatesFunction( xs, ys, FdaModel.Functions.FunctionTypes.Rating);

            //double[] x2 = new double[] { .2f, .3f, .4f, .5f, .6f, .7f, .8f, .9f };
            //double[] y2 = new double[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            //OrdinatesFunction four = new OrdinatesFunction(x2, y2, FdaModel.Functions.FunctionTypes.ExteriorStageFrequency);

            //double[] xval = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            //double[] yval = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            //OrdinatesFunction five = new OrdinatesFunction(xval, yval, FdaModel.Functions.FunctionTypes.ExteriorInteriorStage);

            //double[] x3 = new double[] { .2f, .3f, .4f, .5f, .6f, .7f, .8f, .9f };
            //double[] y3 = new double[] { 2, 200, 300, 600, 1100,2000,3000,4000 };
            //OrdinatesFunction six = new OrdinatesFunction(x3, y3, FdaModel.Functions.FunctionTypes.InteriorStageFrequency);

            //double[] x4 = new double[] { 0, 1, 2, 5, 7,8,9,10,12,15,20 };
            //double[] y4 = new double[] { 1, 2, 3, 600, 1100,1300,1800,10000,30000,100000,500000 };
            //OrdinatesFunction seven = new OrdinatesFunction(x4, y4, FdaModel.Functions.FunctionTypes.InteriorStageDamage);

            //double[] x5 = new double[] { .2f, .3f, .4f, .5f, .6f, .7f, .8f, .9f };
            //double[] y5 = new double[] { 2, 200, 300, 600, 1100,2000,3000,4000 };
            //OrdinatesFunction eight = new OrdinatesFunction(x5, y5, FdaModel.Functions.FunctionTypes.DamageFrequency);

            //double[] x62 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            //double[] y62 = new double[] { 0, .05f, .1f, .2f, .3f, .4f, .7f, .8f, .9f, .95f, 1 };
            //OrdinatesFunction nine = new OrdinatesFunction(x62, y62, FdaModel.Functions.FunctionTypes.LeveeFailure);

            //double[] x6 = new double[] { 0, 5,10 };
            //double[] y6 = new double[] { 0, .7f, 1 };
            //OrdinatesFunction nine2 = new OrdinatesFunction(x6, y6, FdaModel.Functions.FunctionTypes.LeveeFailure);

            //double[] x7 = new double[] { 0, 100, 200, 500, 1000 };
            //double[] y7 = new double[] { 2, 200, 300, 600, 1100 };
            //OrdinatesFunction ninetyNine = new OrdinatesFunction(x7, y7, FdaModel.Functions.FunctionTypes.UnUsed);


            //double[] y8 = new double[] { 0, 5,7};
            //double[] x8 = new double[] { 99,100,101};
            //Statistics.CurveIncreasing curve1 = new Statistics.CurveIncreasing(x8, y8, true, true);

            //OrdinatesFunction curve1Ord = new OrdinatesFunction(curve1, FdaModel.Functions.FunctionTypes.OutflowFrequency);

            //double[] x10 = new double[] { 0,  7 };
            //double[] y10 = new double[] { 99,  101 };
            //Statistics.CurveIncreasing curveOne = new Statistics.CurveIncreasing(x10, y10, true, true);


            //double[] x9 = new double[] { 1,3,6,8 };
            //double[] y9 = new double[] { 50,51,52,53};
            //Statistics.CurveIncreasing curve2 = new Statistics.CurveIncreasing(x9, y9, true, true);

            //OrdinatesFunction curve2Ord = new OrdinatesFunction(curve2, FdaModel.Functions.FunctionTypes.Rating);

            //double[] x11 = new double[] { 4,5,8,9,10 };
            //double[] y11 = new double[] { 100,101,102,103,104 };
            //Statistics.CurveIncreasing testCurve1 = new Statistics.CurveIncreasing(x11, y11, true, true);

            //double[] x13 = new double[] { 0,7,9,10,11 };
            //double[] y13 = new double[] { 99,101,102,103,104 };
            //Statistics.CurveIncreasing testCurve1_1 = new Statistics.CurveIncreasing(x13, y13, true, true);

            //double[] x14 = new double[] { 0,1,2,3,4,8,9,10,12,14};
            //double[] y14 = new double[] { 100,101,102,103,104,105,106,107,108,109 };
            //Statistics.CurveIncreasing testCurve1_2 = new Statistics.CurveIncreasing(x14, y14, true, true);

            //double[] x16 = new double[] { 0, 1, 2, 3, 4 };
            //double[] y16 = new double[] { 100, 101, 102, 103, 104};
            //Statistics.CurveIncreasing testCurve1_3 = new Statistics.CurveIncreasing(x16, y16, true, true);

            //double[] x12 = new double[] { 0,1,2,3,6,7};
            //double[] y12 = new double[] { 50,51,52,53,54,55};
            //Statistics.CurveIncreasing testCurve2 = new Statistics.CurveIncreasing(x12, y12, true, true);

            //double[] x15 = new double[] { 5,6,7,11,13,15 };
            //double[] y15 = new double[] { 50,51,52,53,54,55 };
            //Statistics.CurveIncreasing testCurve2_1 = new Statistics.CurveIncreasing(x15, y15, true, true);

            //double[] x17 = new double[] { 0,1,2,3,4};
            //double[] y17 = new double[] { 50, 51, 52, 53, 54 };
            //Statistics.CurveIncreasing testCurve2_2 = new Statistics.CurveIncreasing(x17, y17, true, true);

            //double[] codyX = new double[] { .1f, .2f, .3f, .4f, .5f, .6f, .7f, .8f, .9f, 1 };
            //double[] codyY = new double[] { 10,11,12,13,14,15,16,17,18,19};
            //OrdinatesFunction composeTest1 = new OrdinatesFunction(codyX, codyY, FdaModel.Functions.FunctionTypes.OutflowFrequency);

            //double[] codyX2 = new double[] {7,8,9,10,11,12,13,14,15,16 };
            //double[] codyY2 = new double[] { 0,1,2,3,4,5,6,7,8,9 };
            //OrdinatesFunction composeTest2 = new OrdinatesFunction(codyX2, codyY2, FdaModel.Functions.FunctionTypes.Rating);

            ////***************************************

            ////OrdinatesFunction testOrd = OrdinatesFunction.CombineCurves(testCurve1_3, testCurve2_2, true);


            ////****************************************

            //double[] x18 = new double[] { 1,2,3};
            //double[] y18 = new double[] {.5,1,2};
            //OrdinatesFunction testReportErrors = new OrdinatesFunction(x18, y18, FdaModel.Functions.FunctionTypes.LeveeFailure);


            //testReportErrors.ReportErrors();
            double[] x62 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            double[] y62 = new double[] { .01, .05f, .1f, .2f, .3f, .4f, .7f, .8f, .9f, .95f, 1 };
            OrdinatesFunction nine = new OrdinatesFunction(x62, y62, FdaModel.Functions.FunctionTypes.LeveeFailure);
            nine.ReportErrors();

            LoadDataSet1();

            

            List<FdaModel.Functions.BaseFunction> myInputList =  GetInputFunctions(new List<int>() { 0,3,5,7,9 });
            PerformanceThreshold threshold = new PerformanceThreshold( PerformanceThresholdTypes.Damage, 2000);
            
            //List<FdaModel.Functions.BaseFunction> myListOfBaseFunctions = new List<FdaModel.Functions.BaseFunction>() { zero,three,five,nine};
            InputFunctions = myInputList;

            LateralStructure myLateralStruct = new LateralStructure( 10);

            Condition simpleTest = new Condition(2008,"russian river",myInputList, threshold,myLateralStruct); //bool call Validate
            ConditionErrors = simpleTest.Messages.Messages;
            AllComputableFunctions = simpleTest;


            Random randomNumberGenerator = new Random(0);

            FdaModel.ComputationPoint.Outputs.Realization simpleTestResult = new FdaModel.ComputationPoint.Outputs.Realization(simpleTest,true,false); //bool oldCompute, bool performance only
           
            simpleTestResult.Compute(randomNumberGenerator);
            RealizationErrors = simpleTestResult.Messages.Messages;



            MyComputedObject = simpleTestResult;






            //WindowWithPlots wwp = new WindowWithPlots();
            //wwp.XYPlot.createLineSeries(x6, y6);
            //wwp.Histogram.createHistogram(simpleTestResult.Functions[6].GetOrdinatesFunction(), simpleTestResult.Functions[simpleTestResult.Functions.Count-1].GetOrdinatesFunction());
            //wwp.Show();


            //LineSeries series1 = new LineSeries();
            //OxyPlot.Series.ColumnSeries myColumnSeries = new OxyPlot.Series.ColumnSeries();



            //for (int i = 0;i<x6.Length;i++)
            //{
            //    series1.Points.Add(new DataPoint(x6[i], y6[i]));

            //}




            ////create category series
            //for (int i = 0; i < x6.Length-1; i++)
            //{
            //    AreaSeries nextAreaSeries = new AreaSeries();

            //    nextAreaSeries.Points.Add(new DataPoint(x6[i], 0));
            //    nextAreaSeries.Points.Add(new DataPoint(x6[i + 1], 0));
            //    double weightedValue = .06 * i; //i am just making this up here it is being calculated in WeightedAEP in ordinates function i think
            //    nextAreaSeries.Points2.Add(new DataPoint(x6[i], weightedValue));
            //    nextAreaSeries.Points2.Add(new DataPoint(x6[i+1], weightedValue));

            //    wwp.Histogram.HistogramPlot.Model.Series.Add(nextAreaSeries);

            //}


            ////List<Tuple<double, double>> myList = new List<Tuple<double, double>>();
            ////for(int j = 0; j<100;j++)
            ////{
            ////    myList.Add(new Tuple<double, double>(j, j * 2));
            ////}


            ////foreach (Tuple<double, double> t in myList)
            ////{
            ////    series1.Points.Add(new DataPoint(t.Item1, t.Item2));
            ////}






            ////PlotModel myPM = new OxyPlot.PlotModel();
            ////myPM.Series.Add(series1);

            ////XYOxyPlot2DVM myVM = new XYOxyPlot2DVM(myList,"test title", "test x", "test y");
            ////myVM.ShowWindowWithPlots();


            //wwp.XYPlot.OxyPlot1.Model.Series.Add(series1);
            //wwp.XYPlot.pl
            ////WindowWithPlots wwp = new WindowWithPlots(myPM,"this is a great title");
            //wwp.Show();




        }
        private Series getSeries1Data(List<Tuple<double, double>> myList)
        {

            LineSeries series1 = new LineSeries();

            foreach (Tuple<double, double> t in myList)
            {
                series1.Points.Add(new DataPoint(t.Item1, t.Item2));
            }
            return series1;
        }
        #endregion
        #region Voids
        private void LoadDataSet1()
        {
            /////////////////////////////////////////////////
            //    ***********   WARNING  *************
            //   Do not change any of these values. It will break the tests. If you want
            //   to test different values then create another dataset
            //   

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
        #endregion
        #region Functions

        private List<FdaModel.Functions.BaseFunction> GetInputFunctions(List<int> inputSequence)
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
        #endregion
    }
}
