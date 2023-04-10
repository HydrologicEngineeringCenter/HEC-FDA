using HEC.FDA.Model.compute;
using HEC.FDA.Model.extensions;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    public class AnalyticalFrequencyEditorVM :CurveEditorVM
    {
        #region Notes
        #endregion
        #region Fields     
        private readonly ObservableCollection<FlowDoubleWrapper> _AnalyticalFlows = new ObservableCollection<FlowDoubleWrapper>();
        private const string MEAN = "Mean: ";
        private const string SKEW = "Skew: ";
        private const string ST_DEV = "St. Dev.: ";
        private const string RECORD_LENGTH = "Record Length: ";

        private double _Mean;
        private double _StDev;
        private double _Skew;
        private bool _IsAnalytical = true;
        private bool _IsStandard = true;
        private string _FitToFlowMean = MEAN + "N/A";
        private string _FitToFlowStDev = ST_DEV + "N/A";
        private string _FitToFlowSkew = SKEW + "N/A";
        private string _FitToFlowRecordLength = RECORD_LENGTH + "N/A";
        private int _POR;
        private ViewResolvingPlotModel _plotModel;
        private int _StandardPOR;
        #endregion
        #region Properties
        public ViewResolvingPlotModel PlotModel
        {
            get { return _plotModel; }
            set
            {
                _plotModel = value;
                NotifyPropertyChanged();
            }
        }
        public string FitToFlowMean
        {
            get { return _FitToFlowMean; }
            set { _FitToFlowMean = value; NotifyPropertyChanged(); }
        }
        public string FitToFlowStDev
        {
            get { return _FitToFlowStDev; }
            set { _FitToFlowStDev = value; NotifyPropertyChanged(); }
        }
        public string FitToFlowSkew
        {
            get { return _FitToFlowSkew; }
            set { _FitToFlowSkew = value; NotifyPropertyChanged(); }
        }
        public string FitToFlowRecordLength
        {
            get { return _FitToFlowRecordLength; }
            set { _FitToFlowRecordLength = value; NotifyPropertyChanged(); }
        }
        public bool IsStandard
        {
            get { return _IsStandard; }
            set 
            { 
                _IsStandard = value; 
                if(_IsStandard)
                {
                    _POR = _StandardPOR;
                }
                else
                {
                    _StandardPOR = _POR;
                }
                NotifyPropertyChanged(nameof(PeriodOfRecord));
                NotifyPropertyChanged(); 
            }
        }
        public bool IsAnalytical
        {
            get { return _IsAnalytical; }
            set  { _IsAnalytical = value; NotifyPropertyChanged();}
        }
        public bool IsGraphical
        {
            get { return !_IsAnalytical; }
            set { _IsAnalytical = !value; NotifyPropertyChanged(); }
        }
        public double Mean
        {
            get { return _Mean; }
            set { _Mean = value; UpdateChartLineData(); NotifyPropertyChanged(); }
        }
        public double StandardDeviation
        {
            get { return _StDev; }
            set { _StDev = value; UpdateChartLineData(); NotifyPropertyChanged(); }
        }
        public double Skew
        {
            get { return _Skew; }
            set { _Skew = value; UpdateChartLineData(); NotifyPropertyChanged(); }
        }
        public int PeriodOfRecord
        {
            get { return _POR; }
            set 
            { 
                _POR = value; 
                UpdateChartLineData(); 
                NotifyPropertyChanged(); 
            }
        }
        public ObservableCollection<FlowDoubleWrapper> AnalyticalFlows
        {
            get  {return _AnalyticalFlows; }
        }
        public TableWithPlotVM GraphicalTableWithPlotVM { get; set; } 

        #endregion
        #region Constructors
        //This supports a fresh editor
        public AnalyticalFrequencyEditorVM(CurveComponentVM defaultCurve,  EditorActionManager actionManager) : base(defaultCurve, actionManager)
        {
            _Mean = DefaultData.LP3Mean;
            _StDev = DefaultData.LP3StDev;
            _Skew = DefaultData.LP3Skew;
            _POR = DefaultData.PeriodOfRecord;
            GraphicalTableWithPlotVM = new TableWithPlotVM(new GraphicalVM(StringConstants.GRAPHICAL_FREQUENCY,StringConstants.EXCEEDANCE_PROBABILITY,StringConstants.DISCHARGE), true);
            AddLegendToPlot();
            LoadDefaultFlows();
            InitializePlotModel();
            NotifyPropertyChanged(nameof(IsAnalytical));
        }
        //This supports loading from a saved state. 
        public AnalyticalFrequencyEditorVM(AnalyticalFrequencyElement elem, EditorActionManager actionManager) :base(elem, actionManager)
        {
            IsAnalytical = elem.IsAnalytical;
            IsStandard = elem.IsStandard;
            LoadFlows(elem);
            InitializePlotModel();
            _Mean = elem.Mean;
            _StDev = elem.StDev;
            _Skew = elem.Skew;
            _POR = elem.POR;

            elem.MyGraphicalVM = new GraphicalVM(elem.MyGraphicalVM.ToXML());

            GraphicalTableWithPlotVM = new TableWithPlotVM(elem.MyGraphicalVM, true);
            AddLegendToPlot();
        }

        private void AddLegendToPlot()
        {
            Legend legend = new Legend();
            legend.LegendPosition = LegendPosition.TopLeft;
            GraphicalTableWithPlotVM.PlotModel.Legends.Clear();
            GraphicalTableWithPlotVM.PlotModel.Legends.Add(legend);
        }
        #endregion
        #region Voids  

        private void LoadFlows(AnalyticalFrequencyElement elem)
        {
            if (elem.AnalyticalFlows.Count == 0)
            {
                LoadDefaultFlows();
            }
            else
            {
                foreach (double flow in elem.AnalyticalFlows)
                {
                    FlowDoubleWrapper fdw = new FlowDoubleWrapper(flow);
                    AnalyticalFlows.Add(fdw);
                }
            }
        }

        public void InitializePlotModel()
        {
            _plotModel = new ViewResolvingPlotModel();
            _plotModel.Title = StringConstants.ANALYTICAL_FREQUENCY;
            Legend legend = new Legend();
            legend.LegendPosition = LegendPosition.BottomRight;
            _plotModel.Legends.Add(legend);

            LinearAxis x = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Title = StringConstants.EXCEEDANCE_PROBABILITY,
                LabelFormatter = _formatter,
                MinorTickSize= 0
            };
            _plotModel.Axes.Add(x);

            LogarithmicAxis y = new LogarithmicAxis()
            {
                Position = AxisPosition.Left,
                Title = StringConstants.DISCHARGE
            };
            _plotModel.Axes.Add(y);
        }

        private static string _formatter(double d)
        {
            Normal standardNormal = new Normal(0,1);
            double value = standardNormal.CDF(d);
            return Math.Round(value, 3).ToString();
        }

        private void LoadDefaultFlows()
        {
            for(int i = 1;i<11;i++)
            {
                FlowDoubleWrapper fdw = new FlowDoubleWrapper(i*1000);
                AnalyticalFlows.Add(fdw);
            }
        }

        public void UpdateChartLineData()
        {
            _plotModel.Series.Clear();
            UncertainPairedData function = GetCoordinatesFunction();
            if (function != null)
            {
                AddLineSeriesToPlot(function);
                AddLineSeriesToPlot(function, 0.025, true);
                AddLineSeriesToPlot(function, 0.975, true);
            }
            else
            {
                _plotModel.Series.Clear();
            }
            _plotModel.InvalidatePlot(true);
        }

        private void AddLineSeriesToPlot(UncertainPairedData function, double probability = 0.5, bool isConfidenceLimit = false)
        {
            LineSeries lineSeries = new LineSeries()
            {
                TrackerFormatString = "X: {Probability:0.####}, Y: {4:F2} "
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

            lineSeries.ItemsSource= points;
            lineSeries.DataFieldX = nameof(NormalDataPoint.ZScore);
            lineSeries.DataFieldY = nameof(NormalDataPoint.Value);
            _plotModel.Series.Add(lineSeries);
        }



        #endregion

        private LogPearson3 CreateLP3()
        {
            LogPearson3 lp3 = new LogPearson3();
            if (IsAnalytical)
            {
                if (IsStandard)
                {
                    lp3 = new LogPearson3(Mean, StandardDeviation, Skew, PeriodOfRecord);
                }
                else
                {
                    //this is fit to flow
                    List<double> flows = new List<double>();
                    foreach (FlowDoubleWrapper d in AnalyticalFlows)
                    {
                        flows.Add(d.Flow);
                    }

                    lp3 = (LogPearson3)lp3.Fit(flows.ToArray());
                    _POR = flows.Count;
                    NotifyPropertyChanged(nameof(PeriodOfRecord));
                }               
            }
            return lp3;
        }

        public FdaValidationResult IsLP3Valid(LogPearson3 lp3)
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (lp3 == null)
            {
                vr.AddErrorMessage("Invalid log pearson 3");
            }
            else
            {
                lp3.Validate();
                if (lp3.HasErrors)
                {
                    List<string> errors = lp3.GetErrors().Cast<string>().ToList();
                    vr.AddErrorMessages(errors);
                }
            }
            return vr;
        }

        public UncertainPairedData GetCoordinatesFunction()
        {
            UncertainPairedData upd = null;
            LogPearson3 lp3 = CreateLP3();
            FdaValidationResult result = IsLP3Valid(lp3);
            if (result.IsValid)
            {
                upd = lp3.BootstrapToUncertainPairedData(new RandomProvider(1234), LogPearson3._RequiredExceedanceProbabilitiesForBootstrapping);
                if (!IsStandard)
                {
                    //if we are on "fit to flows" then update the labels.
                    FitToFlowMean = MEAN + Math.Round(lp3.Mean, 2);
                    FitToFlowSkew = SKEW + Math.Round(lp3.Skewness, 2);
                    FitToFlowStDev = ST_DEV + Math.Round(lp3.StandardDeviation, 2);
                    FitToFlowRecordLength = RECORD_LENGTH + lp3.SampleSize;
                }
            }
            else
            {
                //clear the labels
                FitToFlowMean = MEAN + "N/A";
                FitToFlowStDev = ST_DEV + "N/A";
                FitToFlowSkew = SKEW + "N/A";
                FitToFlowRecordLength = RECORD_LENGTH + "N/A";
                MessageBox.Show(result.ErrorMessage, "Unable to Create LP3", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return upd;
        }

        private FdaValidationResult ValidateEditor()
        {
            FdaValidationResult result = new FdaValidationResult();
            //if it is "fit to flow" ie not standard then we require at least 3 points
            if (!IsStandard)
            {
                if(AnalyticalFlows.Count() < 3)
                {
                    result.AddErrorMessage("Fit to Flows option requires at least 3 flows.");
                }
            }

            return result;
        }

        public override void Save()
        {
            if(IsAnalytical)
            {
                TableWithPlot.CurveComponentVM.Name = Name;
            }
            else
            {
                GraphicalTableWithPlotVM.CurveComponentVM.Name = Name;
            }
            FdaValidationResult result = ValidateEditor();
            if (result.IsValid)
            {
                string editDate = DateTime.Now.ToString("G");
                
                List<double> analyticalFlows = new List<double>();
                foreach (FlowDoubleWrapper d in AnalyticalFlows)
                {
                    analyticalFlows.Add(d.Flow);
                }
                int id = GetElementID<AnalyticalFrequencyElement>();
                AnalyticalFrequencyElement elem = new AnalyticalFrequencyElement(Name, editDate, Description, PeriodOfRecord, IsAnalytical, IsStandard, Mean, StandardDeviation, Skew,
                     analyticalFlows, GraphicalTableWithPlotVM.CurveComponentVM as GraphicalVM, TableWithPlot.CurveComponentVM, id);

                base.Save(elem);
            }
            else
            {
                MessageBox.Show(result.ErrorMessage, "Unable to Create LP3", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AddRows(int startRow, int numRows)
        {
            for(int i = 0;i<numRows;i++)
            {
                AnalyticalFlows.Insert(startRow, CreateDefaultRow());
            }
            UpdateChartLineData();
        }

        private FlowDoubleWrapper CreateDefaultRow()
        {
            FlowDoubleWrapper defaultRow = new FlowDoubleWrapper(1000);
            return defaultRow;
        }

        /// <summary>
        /// Adds a row to the end of the table. This happens when Enter is pressed while in
        /// the last row.
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="numRows"></param>
        public void AddRow()
        {          
            AnalyticalFlows.Add(CreateDefaultRow());
            UpdateChartLineData();
        }

        public void DeleteRows(List<int> indexes)
        {
            for (int i = 0; i < indexes.Count; i++)
            {
                AnalyticalFlows.RemoveAt(indexes[i] - i);
            }
            //if all rows are gone then add a new default row
            if (AnalyticalFlows.Count == 0)
            {
                AnalyticalFlows.Add(CreateDefaultRow());
            }
            UpdateChartLineData();
        }

    }
}
