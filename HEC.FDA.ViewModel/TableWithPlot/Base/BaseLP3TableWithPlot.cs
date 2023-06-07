using HEC.FDA.Model.compute;
using HEC.FDA.Model.extensions;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.TableWithPlot.Data;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.ViewModel.Implementations;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using Statistics.Distributions;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.TableWithPlot.Base
{
    public abstract class BaseLP3TableWithPlot: ValidatingBaseViewModel
    {
        #region Fields
        public static readonly double[] _ExceedenceProbs = new double[16] { 0.999, 0.99, 0.95, 0.9, 0.8, 0.7, 0.5, 0.2, 0.1, 0.04, 0.02, 0.01, 0.005, 0.004, 0.002, 0.001 };
        #endregion

        #region Properties
        private PlotModel _plotModel;
        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set
            {
                _plotModel = value;
                NotifyPropertyChanged();
            }
        }
        private LogPearson3 _lP3Distriution;
        public LogPearson3 LP3Distribution
        {
            get { return _lP3Distriution; }
            set 
            { 
                _lP3Distriution = value;
                UpdatePlot();
            }
        }
        public HistogramDataProvider ConfidenceLimitsDataTable{ get; } =  new HistogramDataProvider();
        #endregion

        #region OxyPlot
        protected void InitializePlotModel()
        {
            PlotModel = new PlotModel();
            _plotModel.Title = StringConstants.ANALYTICAL_FREQUENCY;
            Legend legend = new()
            {
                LegendPosition = LegendPosition.BottomRight
            };
            _plotModel.Legends.Add(legend);
            AddAxes();
            PlotModel.InvalidatePlot(true);
        }
        protected void UpdatePlot()
        {
            PlotModel.Series.Clear();
            LP3Distribution.Validate();
            if (!LP3Distribution.HasErrors)
            {
                RandomProvider rp = new (1234);
                UncertainPairedData LP3asUPD = LP3Distribution.BootstrapToUncertainPairedData(rp, _ExceedenceProbs);
                AddLineSeriesToPlot(LP3asUPD);
                AddLineSeriesToPlot(LP3asUPD, 0.95, true);
                AddLineSeriesToPlot(LP3asUPD, 0.05, true);
                ConfidenceLimitsDataTable.Data.Clear();
                ConfidenceLimitsDataTable.UpdateFromUncertainPairedData(LP3asUPD);
            }
            PlotModel.InvalidatePlot(true);
            NotifyPropertyChanged(nameof(ConfidenceLimitsDataTable));
            NotifyPropertyChanged(nameof(PlotModel));
        }
        private void AddLineSeriesToPlot(UncertainPairedData function, double probability = 0.5, bool isConfidenceLimit = false)
        {
            LineSeries lineSeries = new()
            {
                TrackerFormatString = "X: {Probability:0.####}, Y: {4:F2} ",
                CanTrackerInterpolatePoints = false
            };

            NormalDataPoint[] points = new NormalDataPoint[function.Xvals.Length];

            for (int i = 0; i < function.Xvals.Length; i++)
            {

                double zScore = Normal.StandardNormalInverseCDF(function.Xvals[i]);
                double flowValue = function.Yvals[i].InverseCDF(probability);
                points[i] = new NormalDataPoint(function.Xvals[i], zScore, flowValue);
            }
            if (isConfidenceLimit) { lineSeries.Color = OxyColors.Blue; lineSeries.LineStyle = LineStyle.Dash; }
            else { lineSeries.Color = OxyColors.Black; }

            lineSeries.ItemsSource = points;
            lineSeries.DataFieldX = nameof(NormalDataPoint.ZScore);
            lineSeries.DataFieldY = nameof(NormalDataPoint.Value);
            _plotModel.Series.Add(lineSeries);
        }
        private void AddAxes()
        {
            LinearAxis x = new()
            {
                Position = AxisPosition.Bottom,
                Title = StringConstants.EXCEEDANCE_PROBABILITY,
                LabelFormatter = ProbabilityFormatter,
                Maximum = 3.719, //probability of .9999
                Minimum = -3.719, //probability of .0001
                StartPosition = 1,
                EndPosition = 0
            };
            LogarithmicAxis y = new()
            {
                Position = AxisPosition.Left,
                Title = StringConstants.DISCHARGE,
                Unit = "cfs",

            };
            PlotModel.Axes.Add(x);
            PlotModel.Axes.Add(y);
        }
        private static string ProbabilityFormatter(double d)
        {
            Normal standardNormal = new(0, 1);
            double value = standardNormal.CDF(d);
            string stringval = value.ToString("0.0000");
            return stringval;
        }
        #endregion

        #region Load and Save
        public XElement ToXML()
        {
            XElement ele = new(this.GetType().Name);
            ele.Add(LP3Distribution.ToXML());
            ele = ToXMLInternal(ele);
            return ele;
        }
        public void FromXML(XElement ele)
        {
            foreach (XElement child in ele.Elements())
            {
                if(child.Name == typeof(LogPearson3).Name)
                {
                    LP3Distribution = LogPearson3.FromXML(child) as LogPearson3; 
                    break;
                }
            }
            FromXMLInternal(ele);
        }
        public virtual void FromXMLInternal(XElement ele) {  }
        public virtual XElement ToXMLInternal(XElement ele) { return ele; }

        #endregion
    }
}
