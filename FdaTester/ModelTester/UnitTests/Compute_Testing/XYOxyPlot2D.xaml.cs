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
    /// Interaction logic for XYOxyPlot2D.xaml
    /// </summary>
    public partial class XYOxyPlot2D : UserControl
    {
        private PlotModel _Plot1Model;
        private List<Tuple<double, double>> _Points;
        private string _Title;
        private string _xLabel;
        private string _yLabel;

        private LineSeries _MyLineSeries;

        public PlotModel Plot1Model
        {
            get { return _Plot1Model; }
            set { _Plot1Model = value; }
        }

        public LineSeries MyLineSeries
        {
            get { return _MyLineSeries; }
            set { _MyLineSeries = value; }
        }

        public XYOxyPlot2D()
        {
            InitializeComponent();

            OxyPlot1.Model = new PlotModel();
            OxyPlot1.Model.Title = "Failure Function Plot";
            LinearAxis xAxis = new LinearAxis();
            xAxis.Position = AxisPosition.Bottom;
            xAxis.Title = "Exterior Stage";
            LinearAxis yAxis = new LinearAxis();
            yAxis.Position = AxisPosition.Left;
            yAxis.Title = "Probability";
            OxyPlot1.Model.Axes.Add(xAxis);
            OxyPlot1.Model.Axes.Add(yAxis);

            ////empty series
            //MyLineSeries = new LineSeries();
            //OxyPlot1.Model.Series.Add(MyLineSeries);




        }
        public XYOxyPlot2D(PlotModel plotModel)
        {
            InitializeComponent();
            //OxyPlot1.Model = new PlotModel();
            OxyPlot1.Model = plotModel;
           // OxyPlot1.Model.InvalidatePlot(true);
        }

        public XYOxyPlot2D(List<Tuple<double,double>> points, string title, string xAxisLable, string yAxisLabel)
        {
            InitializeComponent();


            _Title = title;
            _xLabel = xAxisLable;
            _yLabel = yAxisLabel;

            _Points = new List<Tuple<double, double>>();
            _Points = points;

            setUpPlot(title, xAxisLable, yAxisLabel);
            //OxyPlot1 = new OxyPlot.Wpf.PlotView();
            //_Plot1Model = new PlotModel();
            _Plot1Model.Title = _Title;

            OxyPlot1.Model = _Plot1Model;


            //Plot1Model.InvalidatePlot(true);

        }

        public void createLineSeries(double[] xValues, double[] yValues)
        {
            LineSeries series1 = new LineSeries();
            for (int i = 0; i < xValues.Length; i++)
            {
                series1.Points.Add(new DataPoint(xValues[i], yValues[i]));

            }
            OxyPlot1.Model.Series.Add(series1);
            OxyPlot1.Model.InvalidatePlot(true);
        }

        private void setUpPlot(string title, string xlabel, string ylabel)
        {
            _Plot1Model = new PlotModel();


            //_Plot1Model.Title = _Title;

            //LinearAxis XAxis = new LinearAxis();
            LinearAxis XAxis = new LinearAxis();

            XAxis.Position = AxisPosition.Bottom;
            XAxis.Title = _xLabel;
            //XAxis.Minimum = 0;
            //XAxis.Maximum = 1;
            //XAxis.StartPosition = 1;
            //XAxis.EndPosition = .001;
            // XAxis.AbsoluteMaximum = 1;

            XAxis.AbsoluteMinimum = .001;
            //XAxis.Minimum = _ACEMin;
            //XAxis.Maximum = _ACEMax;
            //XAxis.Maximum = .5;
            //XAxis.AxisTitleDistance = 20;

            //XAxis.MajorGridlineStyle = LineStyle.Solid;
            //XAxis.MinorGridlineStyle = LineStyle.Dash;
            _Plot1Model.Axes.Add(XAxis);

            LinearAxis YAxis = new LinearAxis();
            YAxis.Position = AxisPosition.Left;

            YAxis.Title = _yLabel;
            //YAxis.MajorGridlineStyle = LineStyle.Solid;
            //YAxis.MinorGridlineStyle = LineStyle.Dash;
            //YAxis.Minimum = _FlowMin;
            //YAxis.Maximum = _FlowMax;
            _Plot1Model.Axes.Add(YAxis);

            _Plot1Model.LegendBackground = OxyColors.White;
            _Plot1Model.LegendBorder = OxyColors.DarkGray;
            _Plot1Model.LegendPosition = LegendPosition.TopRight;

            _Plot1Model.PlotMargins = new OxyThickness(60, 10, 10, 40);


            _Plot1Model.Series.Add(getSeries1Data());

        }

        private Series getSeries1Data()
        {

            LineSeries series1 = new LineSeries();

            foreach(Tuple<double,double> t in _Points)
            {
                series1.Points.Add(new DataPoint(t.Item1, t.Item2));
            }
            return series1;
        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    //_Plot1Model = new PlotModel();


        //    //_Plot1Model.Title = _Title;

        //    //LinearAxis XAxis = new LinearAxis();
        //    LinearAxis XAxis = new LinearAxis();

        //    XAxis.Position = AxisPosition.Bottom;
        //    XAxis.Title = _xLabel;
        //    //XAxis.Minimum = 0;
        //    //XAxis.Maximum = 1;
        //    //XAxis.StartPosition = 1;
        //    //XAxis.EndPosition = .001;
        //    // XAxis.AbsoluteMaximum = 1;

        //    XAxis.AbsoluteMinimum = .001;
        //    //XAxis.Minimum = _ACEMin;
        //    //XAxis.Maximum = _ACEMax;
        //    //XAxis.Maximum = .5;
        //    //XAxis.AxisTitleDistance = 20;

        //    //XAxis.MajorGridlineStyle = LineStyle.Solid;
        //    //XAxis.MinorGridlineStyle = LineStyle.Dash;
        //    _Plot1Model.Axes.Add(XAxis);

        //    LinearAxis YAxis = new LinearAxis();
        //    YAxis.Position = AxisPosition.Left;

        //    YAxis.Title = _yLabel;
        //    //YAxis.MajorGridlineStyle = LineStyle.Solid;
        //    //YAxis.MinorGridlineStyle = LineStyle.Dash;
        //    //YAxis.Minimum = _FlowMin;
        //    //YAxis.Maximum = _FlowMax;
        //    _Plot1Model.Axes.Add(YAxis);

        //    _Plot1Model.LegendBackground = OxyColors.White;
        //    _Plot1Model.LegendBorder = OxyColors.DarkGray;
        //    _Plot1Model.LegendPosition = LegendPosition.TopRight;

        //    _Plot1Model.PlotMargins = new OxyThickness(60, 10, 10, 40);


        //    _Plot1Model.Series.Add(getSeries1Data());
        //}
    }
}
