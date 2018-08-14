using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace FdaTester.ModelTester.UnitTests.Compute_Testing
{
    //[Author(q0heccdm, 3 / 17 / 2017 2:21:49 PM)]
    public class XYOxyPlot2DVM
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 3/17/2017 2:21:49 PM
        #endregion
        #region Fields
        private List<Tuple<double, double>> _Points;
        private string _PlotTitle;
        private string _XAxisTitle = "this is the vm X axis";
        private string _YAxisTitle = "This is the vm Y axis";

        private OxyPlot.PlotModel _PlotModel;
        #endregion
        #region Properties
        public List<Tuple<double,double>> Points
        {
            get { return _Points; }
            set { _Points = value; }
        }

        public string XAxisTitle
        {
            get { return _XAxisTitle; }
            set { _XAxisTitle = value; }
        }
        public string YAxisTitle
        {
            get { return _YAxisTitle; }
            set { _YAxisTitle = value; }
        }
        public string PlotTitle
        {
            get { return _PlotTitle; }
            set { _PlotTitle = value; }
        }
        public OxyPlot.PlotModel PlotModel
        {
            get { return _PlotModel; }
            set { _PlotModel = value; }
        }
        #endregion
        #region Constructors
        public XYOxyPlot2DVM()
        {

        }
        public XYOxyPlot2DVM(List<Tuple<double,double>> points, string plotTitle, string xAxisTitle, string yAxisTitle)
        {
            PlotTitle = plotTitle;
            XAxisTitle = xAxisTitle;
            YAxisTitle = yAxisTitle;

            LineSeries series1 = new LineSeries();

            foreach (Tuple<double, double> t in points)
            {
                series1.Points.Add(new DataPoint(t.Item1, t.Item2));
            }

            PlotModel = new OxyPlot.PlotModel();
            PlotModel.Series.Add(series1);
            
        }

        public XYOxyPlot2DVM(double[] xValues, double[] yValues)
        {
            if(xValues.Length != yValues.Length)
            {
                throw new Exception("There is a different number of x values than y values.");
            }
            _Points = new List<Tuple<double, double>>();
            for(int i = 0; i<xValues.Length;i++)
            {
                _Points.Add(new Tuple<double, double>(xValues[i], yValues[i]));
            }
               
        }
        #endregion
        #region Voids
        public void ShowWindowWithPlots()
        {
            WindowWithPlots wwp = new WindowWithPlots();
            wwp.Show();
        }
        #endregion
        #region Functions
        #endregion
    }
}
