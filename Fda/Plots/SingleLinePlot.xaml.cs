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
    /// Interaction logic for SingleLinePlot.xaml
    /// </summary>
    public partial class SingleLinePlot : UserControl
    {
        public SingleLinePlot()
        {
            InitializeComponent();
            //This turns off the scrolling
            OxyPlot1.ActualController.UnbindAll();

            OxyPlot1.Model = new PlotModel();
            OxyPlot1.Model.Title = "Inflow";
            LinearAxis xAxis = new LinearAxis();
            xAxis.Position = AxisPosition.Bottom;
            //xAxis.Title = "Exterior Stage";
            xAxis.TickStyle = TickStyle.None;
            xAxis.MajorGridlineStyle = LineStyle.None;
            



            LinearAxis yAxis = new LinearAxis();
            yAxis.Position = AxisPosition.Left;
            yAxis.Title = "Flow";
            OxyPlot1.Model.Axes.Add(xAxis);
            OxyPlot1.Model.Axes.Add(yAxis);

            OxyPlot1.Model.TitlePadding = 0;

            //OxyPlot1.Model.PlotMargins = new OxyThickness(60, 10, 10, 40);
            OxyPlot1.Model.PlotMargins = new OxyThickness(40, 10, 10, 10);



            LineSeries series1 = new LineSeries();

            //LineSeries series2 = new LineSeries();


            //for (int i = 0; i < Curve.Count; i++)
            {
                series1.Points.Add(new DataPoint(1, 0));
                series1.Points.Add(new DataPoint(1, 10));

                //series2.Points.Add(new DataPoint(2, Curve.get_X(i)));


            }
            OxyPlot1.Model.Series.Add(series1);
            OxyPlot1.InvalidatePlot(true);
            //OxyPlot1.Model.MouseMove += Model_MouseMove;
        }
    }
}
