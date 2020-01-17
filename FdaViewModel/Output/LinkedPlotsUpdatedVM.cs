using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot;

namespace FdaViewModel.Output
{
    //[Author(q0heccdm, 3 / 7 / 2017 3:34:19 PM)]
    public class LinkedPlotsUpdatedVM : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 3/7/2017 3:34:19 PM
        #endregion
        #region Fields
        private string _Name;
        private double[] _Xs;
        private double[] _Ys;
        private OxyPlot.PlotModel _MyModel;
        private TrackerHitResult _MyTrackerHitResult;
        private OxyPlot.Wpf.PlotView _MyPlotView;

        #endregion
        #region Properties
        public OxyPlot.Wpf.PlotView MyPlotView
        {
            get { return _MyPlotView; }
            set { _MyPlotView = value; }
        }
        public TrackerHitResult MyTrackerHitResult
        {
            get { return _MyTrackerHitResult; }
            set { _MyTrackerHitResult = value; NotifyPropertyChanged(); }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public double[] Xs
        {
            get { return _Xs; }
            set { _Xs = value; }
        }
        public double[] Ys
        {
            get { return _Ys; }
            set { _Ys = value; }
        }
        public OxyPlot.PlotModel MyModel
        {
            get { return _MyModel; }
            set { _MyModel = value; }
        }
        #endregion
        #region Constructors
        public LinkedPlotsUpdatedVM() : base()
        {

        }
        public LinkedPlotsUpdatedVM(string blah)
        {
            Name = blah;
            Xs = new double[] { 1, 2, 3, 4, 5, 6 };
            Ys = new double[] { 7, 8, 9, 10, 11, 12 };
            LineSeries mySeries = new LineSeries();
            for(int i=0;i<Xs.Length;i++)
            {
                mySeries.Points.Add(new OxyPlot.DataPoint(Xs[i], Ys[i]));

            }
            //MySeries = mySeries;
            MyPlotView = new OxyPlot.Wpf.PlotView();
            MyModel = new OxyPlot.PlotModel();

            setUpPlot1();


            MyModel.Series.Add(mySeries);
            MyPlotView.Model = MyModel;
            
            
        }
        #endregion
        #region Voids
        private void setUpPlot1()
        {
            //_OxyPlotModel = new OxyPlot.PlotModel();
            MyModel.MouseMove += MyModel_MouseMove;
            //_OxyPlotModel.MouseMove += _OxyPlotModel_MouseMove;
            //_OxyPlotModel.MouseUp += _OxyPlotModel_MouseUp;
            //_OxyPlotModel.MouseEnter += _OxyPlotModel_MouseEnter;

            MyModel.Title = "Flow Frequency Relationship";

            //LinearAxis XAxis = new LinearAxis();
            LinearAxis XAxis = new LinearAxis();

            XAxis.Position = AxisPosition.Bottom;
            XAxis.Title = "Annual Chance Exceedance";
            //XAxis.Minimum = 0;
            //XAxis.Maximum = 1;
            //XAxis.StartPosition = 1;
            //XAxis.EndPosition = .001;
            // XAxis.AbsoluteMaximum = 1;

            // XAxis.AbsoluteMinimum = .001;
            //XAxis.Minimum = _ACEMin;
            //XAxis.Maximum = _ACEMax;
            //XAxis.Maximum = .5;
            //XAxis.AxisTitleDistance = 20;

            //XAxis.MajorGridlineStyle = LineStyle.Solid;
            //XAxis.MinorGridlineStyle = LineStyle.Dash;
            MyModel.Axes.Add(XAxis);

            LogarithmicAxis YAxis = new LogarithmicAxis();
            YAxis.Position = AxisPosition.Left;

            YAxis.Title = "Flow (cfs)";
            //YAxis.MajorGridlineStyle = LineStyle.Solid;
            //YAxis.MinorGridlineStyle = LineStyle.Dash;
            //YAxis.Minimum = _FlowMin;
            //YAxis.Maximum = _FlowMax;
            MyModel.Axes.Add(YAxis);

            MyModel.LegendBackground = OxyColors.White;
            MyModel.LegendBorder = OxyColors.DarkGray;
            MyModel.LegendPosition = LegendPosition.TopRight;

            MyModel.PlotMargins = new OxyThickness(60, 10, 10, 40);





            MyModel.InvalidatePlot(true);
        }

        private void MyModel_MouseMove(object sender, OxyMouseEventArgs e)
        {
            //_PlotEntered = 1;
            //if (_MouseDown == false && _HideTracker == false)
            {
                Series mySeries = MyModel.GetSeriesFromPoint(e.Position);
                if (mySeries != null)
                {
                    TrackerHitResult result = mySeries.GetNearestPoint(e.Position, true);
                    if (result != null && !result.DataPoint.Equals(null))
                    {
                        MyTrackerHitResult = result;
                        DataPoint dp = result.DataPoint;
                        
                        result.Text = "ACE: " + Math.Round(dp.X, 3).ToString() + Environment.NewLine + "Flow: " + Math.Round(dp.Y, 3).ToString();
                       //MyPlotView.ShowTracker(result);

                       // _ACE = dp.X;
                        //_Flow = dp.Y;
                        // at this point i have the (x,y) value on the line series that was clicked.
                        //ShowTrackerInPlot2(dp.Y); // this will also call plot 3 and plot 4
                    }
                }
            }
        }

        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

    }
}
