using FdaLogging;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using OxyPlot;
using paireddata;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    public class AnalyticalFrequencyEditorVM :CurveEditorVM
    {
        #region Notes
        #endregion
        #region Fields     
        private ObservableCollection<FlowDoubleWrapper> _AnalyticalFlows = new ObservableCollection<FlowDoubleWrapper>();
        private ObservableCollection<FlowDoubleWrapper> _GraphicalFlows = new ObservableCollection<FlowDoubleWrapper>();
        private const string MEAN = "Mean: ";
        private const string SKEW = "Skew: ";
        private const string ST_DEV = "St. Dev.: ";

        private double _Mean = 2;
        private double _StDev = 2;
        private double _Skew = 2;
        private bool _IsAnalytical = true;
        private bool _IsStandard = true;
        private string _FitToFlowMean = MEAN + "N/A";
        private string _FitToFlowStDev = ST_DEV + "N/A";
        private string _FitToFlowSkew = SKEW + "N/A";
        private int _POR = 200;
        private PlotModel _plotModel;

        #endregion
        #region Properties
        public PlotModel PlotModel
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
        public bool IsAnalytical
        {
            get { return _IsStandard; }
            set { _IsStandard = value; NotifyPropertyChanged(); }
        }
        public bool IsStandard
        {
            get { return _IsAnalytical; }
            set { _IsAnalytical = value; NotifyPropertyChanged(); }
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
            set { _POR = value; UpdateChartLineData(); NotifyPropertyChanged(); }
        }

        public ObservableCollection<FlowDoubleWrapper> AnalyticalFlows
        {
            get  {return _AnalyticalFlows; }
            set { _AnalyticalFlows = value; NotifyPropertyChanged();}
        }
        public ObservableCollection<FlowDoubleWrapper> GraphicalFlows
        {
            get { return _GraphicalFlows; }
            set { _GraphicalFlows = value; NotifyPropertyChanged(); }
        }      

        #endregion
        #region Constructors
        public AnalyticalFrequencyEditorVM(ComputeComponentVM defaultCurve,  EditorActionManager actionManager) : base(defaultCurve, actionManager)
        {
            LoadDefaultFlows();
            InitializePlotModel();
        }
        public AnalyticalFrequencyEditorVM(AnalyticalFrequencyElement elem, EditorActionManager actionManager) :base(elem, actionManager)// string name, Statistics.LogPearsonIII lpiii, string description, Utilities.OwnerElement owner) : base()
        {
            IsAnalytical = elem.IsAnalytical;
            IsStandard = elem.IsStandard;
            LoadFlows(elem);
            InitializePlotModel();
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
                    fdw.FlowChanged += FlowValue_FlowChanged;
                    AnalyticalFlows.Add(fdw);
                }
            }
        }

        private void InitializePlotModel()
        {
            _plotModel = new PlotModel();
            _plotModel.Title = "Flow-Frequency";
            _plotModel.LegendPosition = LegendPosition.BottomRight;

            OxyPlot.Axes.LinearAxis x = new OxyPlot.Axes.LinearAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                StartPosition = 1,
                EndPosition = 0,
                Title = "Frequency"
            };
            _plotModel.Axes.Add(x);

            OxyPlot.Axes.LinearAxis y = new OxyPlot.Axes.LinearAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = "Flow"
            };
            _plotModel.Axes.Add(y);

            UpdateChartLineData();
        }

        private void LoadDefaultFlows()
        {
            for(int i = 1;i<11;i++)
            {
                FlowDoubleWrapper fdw = new FlowDoubleWrapper(i*1000);
                fdw.FlowChanged += FlowValue_FlowChanged;
                AnalyticalFlows.Add(fdw);
            }
        }

        private void FlowValue_FlowChanged(object sender, EventArgs e)
        {
            UpdateChartLineData();
        }

        public void UpdateChartLineData()
        {
            _plotModel.Series.Clear();
            OxyPlot.Series.LineSeries lineSeries = new OxyPlot.Series.LineSeries();
            UncertainPairedData function = GetCoordinatesFunction();
            if (function != null)
            {
                for (int i = 0; i < function.Xvals.Length; i++)
                {
                    //todo: should we do uncertainty bounds around the y?
                    double xVal = function.Xvals[i];
                    double yVal = function.Yvals[i].InverseCDF(.5);
                    lineSeries.Points.Add(new DataPoint(xVal, yVal));
                }
                _plotModel.Series.Add(lineSeries);
            }
            else
            {
                _plotModel.Series.Clear();
            }
            _plotModel.InvalidatePlot(true);
        }

        #endregion

        public LogPearson3 CreateLP3()
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
                }               
            }
            return lp3;
        }

        public FdaValidationResult IsLP3Valid(LogPearson3 lp3)
        {
            FdaValidationResult vr = new FdaValidationResult();
            lp3.Validate();
            if (lp3.HasErrors)
            {
                List<string> errors = lp3.GetErrors().Cast<string>().ToList();
                vr.AddErrorMessages(errors);
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

                double[] probs = new double[] { .001, .01, .05, .25, .5, .75, .95, .99, .999 };
                List<double> yVals = new List<double>();
                foreach (double prob in probs)
                {
                    yVals.Add(lp3.InverseCDF(prob));
                }
                upd = UncertainPairedDataFactory.CreateDeterminateData(new List<double>(probs), yVals, "Frequency", "Flow", "Flow-Frequency");
                if (!IsStandard)
                {
                    //if we are on "fit to flows" then update the labels.
                    FitToFlowMean = MEAN + Math.Round(lp3.Mean, 2);
                    FitToFlowSkew = SKEW + Math.Round(lp3.Skewness, 2);
                    FitToFlowStDev = ST_DEV + Math.Round(lp3.StandardDeviation, 2);
                }
            }
            else
            {
                //clear the labels
                FitToFlowMean = MEAN + "N/A";
                FitToFlowStDev = ST_DEV + "N/A";
                FitToFlowSkew = SKEW + "N/A";

                LogItem logItem = LogItemFactory.FactoryTemp(LoggingLevel.Fatal, result.ErrorMessage);
                MessageRows.Add(logItem);
            }
            return upd;
        }

        public override void Save()
        {
            LogPearson3 lp3 = CreateLP3();
            FdaValidationResult result = IsLP3Valid(lp3);
            if (result.IsValid)
            {
                string editDate = DateTime.Now.ToString("G");
                double mean = Mean;
                double stDev = StandardDeviation;
                double skew = Skew;
                int por = PeriodOfRecord;
                bool isAnalytical = IsAnalytical;
                bool isStandard = IsStandard;
                List<double> analyticalFlows = new List<double>();
                foreach (FlowDoubleWrapper d in AnalyticalFlows)
                {
                    analyticalFlows.Add(d.Flow);
                }
                List<double> graphicalFlows = new List<double>();
                foreach (FlowDoubleWrapper d in GraphicalFlows)
                {
                    graphicalFlows.Add(d.Flow);
                }
                int id = GetElementID(Saving.PersistenceFactory.GetFlowFrequencyManager());

                AnalyticalFrequencyElement elem = new AnalyticalFrequencyElement(Name, editDate, Description, por, isAnalytical, isStandard, mean, stDev, skew,
                     analyticalFlows, graphicalFlows, TableWithPlot.ComputeComponentVM, id);
                base.Save(elem);
            }
            else
            {
                LogItem logItem = LogItemFactory.FactoryTemp(LoggingLevel.Fatal, "Could not save with an invalid function.");
                MessageRows.Add(logItem);
            }
        }

        public void AddRows(int startRow, int numRows)
        {
            for(int i = 0;i<numRows;i++)
            {
                FlowDoubleWrapper emptyFlow = new FlowDoubleWrapper(0);
                emptyFlow.FlowChanged += FlowValue_FlowChanged;
                AnalyticalFlows.Insert(startRow, emptyFlow);
            }
            UpdateChartLineData();
        }

        /// <summary>
        /// Adds a row to the end of the table. This happens when Enter is pressed while in
        /// the last row.
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="numRows"></param>
        public void AddRow()
        {          
            FlowDoubleWrapper emptyFlow = new FlowDoubleWrapper(1000);
            emptyFlow.FlowChanged += FlowValue_FlowChanged;
            AnalyticalFlows.Add(emptyFlow);
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
                FlowDoubleWrapper emptyFlow = new FlowDoubleWrapper(0);
                emptyFlow.FlowChanged += FlowValue_FlowChanged;
                AnalyticalFlows.Add(emptyFlow);
            }
            UpdateChartLineData();
        }
    }
}
