using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace FdaTester.ModelTester.UnitTests.Compute_Testing
{
    /// <summary>
    /// Interaction logic for ThresholdHistogramPlot.xaml
    /// </summary>
    public partial class ThresholdHistogramPlot : UserControl
    {
        public ThresholdHistogramPlot()
        {
            InitializeComponent();

            HistogramPlot.Model = new PlotModel();

            HistogramPlot.Model.Title = "Threshold Histogram Plot";
            
            LinearAxis yAxis = new LinearAxis();
            yAxis.Position = AxisPosition.Left;
            yAxis.Title = "Probability";
            
            HistogramPlot.Model.Axes.Add(yAxis);



            CategoryAxis xAxis = new CategoryAxis();
            xAxis.Position = AxisPosition.Bottom;
            xAxis.Title = "Exterior Stage";
            


        }

        public void createHistogram(double[] xValues, double[] yValues)
        {
            //create category series
            for (int i = 0; i < xValues.Length - 1; i++)
            {
                AreaSeries nextAreaSeries = new AreaSeries();

                nextAreaSeries.Points.Add(new DataPoint(xValues[i], 0));
                nextAreaSeries.Points.Add(new DataPoint(xValues[i + 1], 0));
                double weightedValue = .06 * i; //i am just making this up here it is being calculated in WeightedAEP in ordinates function i think
                nextAreaSeries.Points2.Add(new DataPoint(xValues[i], weightedValue));
                nextAreaSeries.Points2.Add(new DataPoint(xValues[i + 1], weightedValue));

                HistogramPlot.Model.Series.Add(nextAreaSeries);

            }
        }


        public void createHistogram(FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction damageFrequencyFunction, FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction failureFunction )
        {


            for (int i = 0; i < damageFrequencyFunction.Function.Count - 1; i++)
            {
                AreaSeries nextAreaSeries = new AreaSeries();

                double pointA = damageFrequencyFunction.Function.get_X(i);
                double pointB = damageFrequencyFunction.Function.get_X(i + 1);

                double areaWeight = GetAreaWeight(failureFunction, pointA , pointB);
                double interval = (damageFrequencyFunction.Function.get_X(i + 1) - damageFrequencyFunction.Function.get_X(i));
                double singleRunWeightedAEP = interval * areaWeight;


                nextAreaSeries.Points.Add(new DataPoint(pointA, 0));
                nextAreaSeries.Points.Add(new DataPoint(pointB, 0));

                nextAreaSeries.Points2.Add(new DataPoint(pointA, singleRunWeightedAEP));
                nextAreaSeries.Points2.Add(new DataPoint(pointB, singleRunWeightedAEP));


                HistogramPlot.Model.Series.Add(nextAreaSeries);


            }




            ////create category series
            //for (int i = 0; i < xValues.Length - 1; i++)
            //{
            //    AreaSeries nextAreaSeries = new AreaSeries();

            //    nextAreaSeries.Points.Add(new DataPoint(xValues[i], 0));
            //    nextAreaSeries.Points.Add(new DataPoint(xValues[i + 1], 0));
            //    double weightedValue = .06 * i; //i am just making this up here it is being calculated in WeightedAEP in ordinates function i think
            //    nextAreaSeries.Points2.Add(new DataPoint(xValues[i], weightedValue));
            //    nextAreaSeries.Points2.Add(new DataPoint(xValues[i + 1], weightedValue));

            //    HistogramPlot.Model.Series.Add(nextAreaSeries);

            //}
        }


        private double GetAreaWeight(FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction failureFunction, double a, double b)
        {
            double lP, uP, avgP = -999999999999, range = b - a; // The giant negative number is just for testing. Remove if test is validated.
            for (int j = 0; j < failureFunction.Function.Count - 1; j++)
            {
                if (failureFunction.Function.get_X(j) <= a)
                {
                    if (j == failureFunction.Function.Count - 1)     // Special Case: failurefunction.Xs[Xs.Length - 1] <= a
                    {
                        return avgP = failureFunction.Function.get_Y(j);
                    }
                    else
                    {
                        if (failureFunction.Function.get_X(j + 1) > a)
                        {
                            lP = failureFunction.Function.get_Y(j) + (a - failureFunction.Function.get_X(j)) / (failureFunction.Function.get_X(j + 1) - failureFunction.Function.get_X(j)) * (failureFunction.Function.get_Y(j + 1) - failureFunction.Function.get_Y(j));

                            if (failureFunction.Function.get_X(j + 1) < b)
                            {
                                avgP = (lP + failureFunction.Function.get_Y(j + 1)) / 2 * (failureFunction.Function.get_X(j + 1) - a) / range;
                            }
                            else
                            {
                                uP = failureFunction.Function.get_Y(j) + (b - failureFunction.Function.get_X(j)) / (failureFunction.Function.get_X(j + 1) - failureFunction.Function.get_X(j)) * (failureFunction.Function.get_Y(j + 1) - failureFunction.Function.get_Y(j));
                                return avgP = (uP + lP) / 2;
                            }
                        }
                    }
                }
                else
                {
                    if (j == failureFunction.Function.Count - 1)     // Special Case: failurefunction.Xs[Xs.Length - 1] < b
                    {
                        return avgP += failureFunction.Function.get_Y(j) * (b - failureFunction.Function.get_X(j)) / range;
                    }
                    else
                    {
                        if (j == 0)
                        {
                            if (failureFunction.Function.get_Y(0) < b)      // Special Case: failurefunction.Xs[0] > a
                            {
                                avgP = failureFunction.Function.get_Y(j) * (failureFunction.Function.get_X(0) - a) / range;
                            }
                            else                                // Special Case: failurefunction.Xs[0] >=b
                            {
                                return avgP = failureFunction.Function.get_Y(j);
                            }
                        }

                        if (failureFunction.Function.get_X(j + 1) < b)
                        {
                            avgP += (failureFunction.Function.get_Y(j) + failureFunction.Function.get_Y(j + 1)) / 2 * (failureFunction.Function.get_X(j + 1) - failureFunction.Function.get_X(j)) / range;
                        }
                        else
                        {
                            uP = failureFunction.Function.get_Y(j) + (b - failureFunction.Function.get_X(j)) / (failureFunction.Function.get_X(j + 1) - failureFunction.Function.get_X(j)) * (failureFunction.Function.get_Y(j + 1) - failureFunction.Function.get_Y(j));
                            return avgP += (failureFunction.Function.get_Y(j) + uP) / 2 * (b - failureFunction.Function.get_X(j)) / range;
                        }
                    }
                }
                //j++;
            }
            throw new Exception("This should be unreachable.");
        }


    }
}
