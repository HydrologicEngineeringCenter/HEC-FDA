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
using System.Windows.Shapes;

namespace FdaTester.ModelTester.UnitTests.Compute_Testing
{
    /// <summary>
    /// Interaction logic for WindowWithPlots.xaml
    /// </summary>
    public partial class WindowWithPlots : Window
    {
        public WindowWithPlots()
        {

            InitializeComponent();

        }


        public WindowWithPlots(OxyPlot.PlotModel theModel, string title)
        {
            InitializeComponent();
            XYPlot.OxyPlot1.Model = theModel;
            XYPlot.OxyPlot1.Model.Title = title;
        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    OxyPlot.Series.LineSeries mySeries = new OxyPlot.Series.LineSeries();
        //    mySeries.Points.Add(new OxyPlot.DataPoint(0, 0));
        //    mySeries.Points.Add(new OxyPlot.DataPoint(1, 1));
        //    mySeries.Points.Add(new OxyPlot.DataPoint(2, 2));
        //    mySeries.Points.Add(new OxyPlot.DataPoint(3, 3));

        //    XYPlot.Plot1Model = new OxyPlot.PlotModel();

        //    XYPlot.Plot1Model.Series.Add(mySeries);
        //    XYPlot.Plot1Model.InvalidatePlot(true);
        //}

        private void btn_me_Click(object sender, RoutedEventArgs e)
        {

            

            List<Tuple<double, double>> myList = new List<Tuple<double, double>>();
            for (int j = 0; j < 100; j++)
            {
                myList.Add(new Tuple<double, double>(j, j * 2));
            }
            XYPlot = new XYOxyPlot2D(myList, "myTitle", "xaxis", "yaxis");

            XYPlot.Plot1Model.InvalidatePlot(true);

            //OxyPlot.Series.LineSeries mySeries = new OxyPlot.Series.LineSeries();
            //mySeries.Points.Add(new OxyPlot.DataPoint(0, 0));
            //mySeries.Points.Add(new OxyPlot.DataPoint(1, 1));
            //mySeries.Points.Add(new OxyPlot.DataPoint(2, 2));
            //mySeries.Points.Add(new OxyPlot.DataPoint(3, 3));

            //XYPlot.Plot1Model = new OxyPlot.PlotModel();

            //XYPlot.Plot1Model.Series.Add(mySeries);
            //XYPlot.Plot1Model.InvalidatePlot(true);
        }
    }
}
