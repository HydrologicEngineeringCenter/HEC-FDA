using HEC.FDA.Model.compute;
using HEC.FDA.Model.extensions;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.ViewModel.Implementations;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Statistics.Distributions;

namespace HEC.FDA.ViewModel.TableWithPlot.Base
{
    public class BaseLP3Plotter: ValidatingBaseViewModel
    {
        private PlotModel _plotModel;
        private LogPearson3 _lP3Distriution;
        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set
            {
                _plotModel = value;
                NotifyPropertyChanged();
            }
        }
        public LogPearson3 LP3Distribution
        {
            get { return _lP3Distriution; }
            set 
            { 
                _lP3Distriution = value;
                UpdatePlot();
            }
        }

        #region OxyPlot
        protected void InitializePlotModel()
        {
            PlotModel = new PlotModel();
            AddAxes();
        }
        protected void UpdatePlot()
        {
            PlotModel.Series.Clear();
            UncertainPairedData LP3asUPD = LP3Distribution.BootstrapToUncertainPairedData(new RandomProvider(1234), LogPearson3._RequiredExceedanceProbabilitiesForBootstrapping);
            AddLineSeriesToPlot(LP3asUPD);
            AddLineSeriesToPlot(LP3asUPD, 0.975, true);
            AddLineSeriesToPlot(LP3asUPD, 0.025, true);
            PlotModel.InvalidatePlot(true);
            NotifyPropertyChanged(nameof(PlotModel));
        }
        private void AddLineSeriesToPlot(UncertainPairedData function, double probability = 0.5, bool isConfidenceLimit = false)
        {
            LineSeries lineSeries = new LineSeries()
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
                LabelFormatter = _probabilityFormatter,
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
        private static string _probabilityFormatter(double d)
        {
            Normal standardNormal = new Normal(0, 1);
            double value = standardNormal.CDF(d);
            string stringval = value.ToString("0.0000");
            return stringval;
        }
        #endregion
    }
}
