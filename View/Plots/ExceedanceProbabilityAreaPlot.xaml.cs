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

namespace Fda.Plots
{
    /// <summary>
    /// Interaction logic for ExceedanceProbabilityAreaPlot.xaml
    /// </summary>
    public partial class ExceedanceProbabilityAreaPlot : UserControl
    {

        public static readonly DependencyProperty CurvesProperty = DependencyProperty.Register("Curves", typeof(List<FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction>), typeof(ExceedanceProbabilityAreaPlot), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(CurveChangedCallBack)));

        

        public List<FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction> Curves
        {
            get { return (List<FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction>)GetValue(CurvesProperty); }
            set { SetValue(CurvesProperty, value); }
        }

       

        

        private static void CurveChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExceedanceProbabilityAreaPlot owner = d as ExceedanceProbabilityAreaPlot;

            List<FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction> listOfOrdFuncs = e.NewValue as List<FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction>;

            FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction failureFunction = listOfOrdFuncs[0];
            FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction exteriorStageFreq = listOfOrdFuncs[1];

            double weightedAEP = 0;
            for (int i = 0; i < exteriorStageFreq.Function.Count - 1; i++)
            {
                double areaWeight = GetAreaWeight(failureFunction, exteriorStageFreq.Function.get_X(i), exteriorStageFreq.Function.get_X(i + 1));
                double interval = (exteriorStageFreq.Function.get_X(i + 1) - exteriorStageFreq.Function.get_X(i));
                weightedAEP += interval * areaWeight;
                double singleRunWeightedAEP = interval * areaWeight;




                AreaSeries nextAreaSeries = new AreaSeries();

                double pointA = exteriorStageFreq.Function.get_X(i);
                double pointB = exteriorStageFreq.Function.get_X(i + 1);


                nextAreaSeries.Points.Add(new DataPoint(pointA, 0));
                nextAreaSeries.Points.Add(new DataPoint(pointB, 0));

                nextAreaSeries.Points2.Add(new DataPoint(pointA, singleRunWeightedAEP));
                nextAreaSeries.Points2.Add(new DataPoint(pointB, singleRunWeightedAEP));


                owner.OxyPlot1.Model.Series.Add(nextAreaSeries);
                owner.txt_AEP.Text = "AEP: " + weightedAEP.ToString("0.000");

            }
            //return weightedAEP;

        }




        public ExceedanceProbabilityAreaPlot()
        {
            InitializeComponent();


            OxyPlot1.Model = new PlotModel();
            OxyPlot1.Model.Title = "Exceedance Probability Plot";

            LinearAxis xAxis = new LinearAxis();
            xAxis.Position = AxisPosition.Bottom;
            xAxis.Title = "Exterior Stage";
            LinearAxis yAxis = new LinearAxis();
            yAxis.Position = AxisPosition.Left;
            yAxis.Title = "Probability";
            OxyPlot1.Model.Axes.Add(xAxis);
            OxyPlot1.Model.Axes.Add(yAxis);


        }


        public void someDumbThing()
        {





            //double weightedAEP = 0;
            //for (int i = 0; i < _ExteriorStageFrequency.Function.Count - 1; i++)
            //{
            //    double areaWeight = GetAreaWeight(_FailureFunction, _ExteriorStageFrequency.Function.get_X(i), _ExteriorStageFrequency.Function.get_X(i + 1));
            //    double interval = (_ExteriorStageFrequency.Function.get_X(i + 1) - _ExteriorStageFrequency.Function.get_X(i));
            //    weightedAEP += interval * areaWeight;
            //    //double singleRunWeightedAEP = interval * areaWeight;//this is just for testing. Delete me!
            //}
            //return weightedAEP;


        }

        private static double GetAreaWeight(FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction failureFunction, double a, double b)
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
