using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FdaLogging;
using ViewModel.Editors;
using Functions;
using FunctionsView.ViewModel;
using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using Model;
using Statistics;
using Utilities;

namespace ViewModel.FrequencyRelationships
{
    public class AnalyticalFrequencyEditorVM :CurveEditorVM
    {
        #region Notes
        #endregion
        #region Fields
        
        private IFdaFunction _Curve;
        private ObservableCollection<double> _Probabilities = new ObservableCollection<double>();
        private ObservableCollection<FlowDoubleWrapper> _AnalyticalFlows = new ObservableCollection<FlowDoubleWrapper>();

        private ObservableCollection<FlowDoubleWrapper> _GraphicalFlows = new ObservableCollection<FlowDoubleWrapper>();

        private double _Mean = 2;
        private double _StDev = 2;
        private double _Skew = 2;

        private bool _IsAnalytical = true;
        private bool _IsStandard = true;
        private string _FitToFlowMean = "Mean: N/A";
        private string _FitToFlowStDev = "St. Dev.: N/A";
        private string _FitToFlowSkew = "Skew: N/A";
        private int _POR = 200;
        #endregion
        #region Properties

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
            set { _Mean = value; UpdateChartLineData(); }
        }
        public double StandardDeviation
        {
            get { return _StDev; }
            set { _StDev = value; UpdateChartLineData(); }
        }
        public double Skew
        {
            get { return _Skew; }
            set { _Skew = value; UpdateChartLineData(); }
        }
        public int PeriodOfRecord
        {
            get { return _POR; }
            set { _POR = value; UpdateChartLineData(); }
        }
        public bool IsLogFlow { get; set; }

        public SciChart2DChartViewModel StandardChartViewModel { get; } = new SciChart2DChartViewModel("chart title");
        public SciChart2DChartViewModel FitToFLowChartViewModel { get; } = new SciChart2DChartViewModel("chart title");

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

     
        public CoordinatesFunctionEditorVM EditorVM
        {
            get;
            set;
        }
 
        public ObservableCollection<double> Probabilities
        {
            get { return _Probabilities; }
            set { _Probabilities = value; NotifyPropertyChanged(); UpdateItems(); }
        }
       

        #endregion
        #region Constructors
        public AnalyticalFrequencyEditorVM(IFdaFunction defaultCurve, string xLabel,string yLabel,string chartTitle, Editors.EditorActionManager actionManager) : base(defaultCurve, xLabel, yLabel, chartTitle, actionManager)
        {
            _Curve = null;//todo: Refactor: CO new Statistics.LogPearsonIII(4, .4, .5, 50);
            Probabilities = new System.Collections.ObjectModel.ObservableCollection<double>() { .99, .95, .9, .8, .7, .6, .5, .4, .3, .2, .1, .05, .01 };   
            UpdateChartLineData();
            LoadDefaultFlows();
        }
        public AnalyticalFrequencyEditorVM(AnalyticalFrequencyElement elem,string xLabel,string yLabel,string chartTitle, Editors.EditorActionManager actionManager) :base(elem, xLabel, yLabel, chartTitle, actionManager)// string name, Statistics.LogPearsonIII lpiii, string description, Utilities.OwnerElement owner) : base()
        {
            CurrentElement = elem;
            Probabilities = new ObservableCollection<double>() { .99, .95, .9, .8, .7, .6, .5, .4, .3, .2, .1, .05, .01 };
            UpdateChartLineData();
            if(elem.AnalyticalFlows.Count == 0)
            {
                LoadDefaultFlows();
            }
        }
        #endregion
        #region Voids
        
        private void LoadDefaultFlows()
        {
            for(int i = 0;i<10;i++)
            {
                FlowDoubleWrapper fdw = new FlowDoubleWrapper(i);
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
            try
            {
                ICoordinatesFunction function = GetCoordinatesFunction();
                IFdaFunction fdaFunction = IFdaFunctionFactory.Factory(IParameterEnum.InflowFrequency, function);
                ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(fdaFunction.Coordinates, fdaFunction.Interpolator);

                CoordinatesFunctionEditorChartHelper chartHelper = new CoordinatesFunctionEditorChartHelper(func, "Frequency", "Flow");

                List<SciLineData> lineData = chartHelper.CreateLineData(false, true, true);
                StandardChartViewModel.LineData.Set(lineData);
            }
            catch(Exception e)
            {
                //todo: delete me
                int test = 0;
            }
        }


        private ICoordinatesFunction CreateFunctionFromTable()
        {
            //list xs will be the probabilties
            List<double> xs = new List<double>();
            foreach(double d in  Probabilities)
            {
                xs.Add(d);
            }

            List<double> ys = new List<double>();
            foreach(FlowDoubleWrapper d in AnalyticalFlows)
            {
                ys.Add(d.Flow);
            }

            //list of ys will be the flow data with the mean, st dev, skew on it.
            return ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
        }

        private void UpdateItems()
        {
            //todo: Refactor: CO
            //System.Collections.ObjectModel.ObservableCollection<object> tmp = new System.Collections.ObjectModel.ObservableCollection<object>();
            //if (_Curve == null) return;
            //if (_Probabilities.Count <= 0) return;
            //List<double> probs = new List<double>();
            //foreach (double d in _Probabilities)
            //{
            //    probs.Add(d);
            //}
            //System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
            //s.Start();
            ////System.Diagnostics.Debug.Print(DateTime.Now.Millisecond.ToString());
            //try
            //{
            //    List<Statistics.Histogram> histos = _Curve.CreateConfidenceInterval(probs, .05, .95, .01, 10000);
            //    s.Stop();
            //    System.Diagnostics.Debug.Print(s.ElapsedMilliseconds.ToString());
            //    for (int i = 0; i < probs.Count; i++)
            //    {
            //        tmp.Add(new AnalyticalFrequencyRowItem(tmp, _Probabilities[i], histos[i]));
            //    }
            //    Items = tmp;
            //}
            //catch (Exception ex)
            //{
            //    s.Stop();
            //    //ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("A value of mean standard deviation or skew was supplied that caused the confidence interval method to crash", FdaModel.Utilities.Messager.ErrorMessageEnum.Report | FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel));
            //}
        }
        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {
            //probs must be increasing?
            //skew limits?
            //variance limits?
            //todo: Refactor: CO
            //AddRule(nameof(Mean), () => Mean > 1, "Mean must be greater than 1");
            //AddRule(nameof(Mean), () => Mean < 9, "Mean must be less than 9");
            //AddRule(nameof(StandardDeviation), () => StandardDeviation > 0, "Standard Deviation must be greater than 0");
            //AddRule(nameof(StandardDeviation), () => StandardDeviation < .5, "Standard Deviation must be less than .5");
            //AddRule(nameof(Skew), () => Skew > -1.5, "Skew must be greater than -1.5");
            //AddRule(nameof(Skew), () => Skew < 1.5, "Skew must be less than 1.5");

            //AddRule(nameof(SampleSize), () => SampleSize > 5, "Sample size cannot be less than 5");
            //AddRule(nameof(SampleSize), () => SampleSize < 300, "Sample size cannot be more than 300");

            AddRule(nameof(Name), () => { if (Name == null) { return false; } else { return !Name.Equals(""); } } , "Name cannot be blank");

        }


        public override ICoordinatesFunction GetCoordinatesFunction()
        {
            List<double> xs = new List<double>();
            List<double> ys = new List<double>();
            try
            {
                if (IsAnalytical)
                {
                    if (IsStandard)
                    {
                        //todo use mean, st dev, and skew to create the curve
                        
                        //return ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
                        IDistribution dist = IDistributionFactory.FactoryLogPearsonIII(Mean, StandardDeviation, Skew, PeriodOfRecord);
                        if(dist.State < IMessageLevels.Error)
                        {
                            return IFunctionFactory.Factory(dist);
                        }

                        //return ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
                    }
                    else
                    {
                        //this is fit to flow
                        List<double> flows = new List<double>();
                        foreach(FlowDoubleWrapper d in AnalyticalFlows)
                        {
                            flows.Add(d.Flow);
                        }

                        IDistribution dist = IDistributionFactory.FactoryFitLogPearsonIII(flows, IsLogFlow, PeriodOfRecord);
                        if (dist.State < IMessageLevels.Error)
                        {
                            double mean = dist.Mean;
                            double stDev = dist.StandardDeviation;
                            double skew = dist.Skewness;
                            FitToFlowMean = "Mean: " + mean.ToString(".##");
                            FitToFlowStDev = "St. Dev.: " + stDev.ToString(".##");
                            FitToFlowSkew = "Skew: " + skew.ToString(".##");

                            return IFunctionFactory.Factory(dist);
                        }
                        //return ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
                    }
                }
            }
            catch (Exception e)
            {
                FitToFlowMean = "Mean: N/A";
                FitToFlowStDev = "Mean: N/A";
                FitToFlowSkew = "Mean: N/A";

                xs = new List<double>() { 0 };
                ys = new List<double>() { 0 };
                return ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
            }
            FitToFlowMean = "Mean: N/A";
            FitToFlowStDev = "Mean: N/A";
            FitToFlowSkew = "Mean: N/A";

            xs = new List<double>() { 0 };
            ys = new List<double>() { 0 };
            return ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
            
        }

        public bool CanCreateValidFunction()
        {
            List<double> xs = new List<double>();
            List<double> ys = new List<double>();
            try
            {
                if (IsAnalytical)
                {
                    if (IsStandard)
                    {
                        //todo use mean, st dev, and skew to create the curve

                        //return ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
                        IDistribution dist = IDistributionFactory.FactoryLogPearsonIII(Mean, StandardDeviation, Skew, PeriodOfRecord);
                        if (dist.State < IMessageLevels.Error)
                        {
                            IFunction func = IFunctionFactory.Factory(dist);
                            IFdaFunction fdaFunction = IFdaFunctionFactory.Factory(IParameterEnum.InflowFrequency, func);
                            //todo: currently the flows are way to high to be real which is causing an error message
                            //i am commenting this out so that i can move on
                            //if(func.State < IMessageLevels.Error && fdaFunction.State < IMessageLevels.Error)
                            {
                                return true;
                            }
                        }

                        //return ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
                    }
                    else
                    {
                        List<double> flows = new List<double>();
                        foreach (FlowDoubleWrapper d in AnalyticalFlows)
                        {
                            flows.Add(d.Flow);
                        }

                        IDistribution dist = IDistributionFactory.FactoryFitLogPearsonIII(flows, IsLogFlow, PeriodOfRecord);
                        if (dist.State < IMessageLevels.Error)
                        {
                            IFunction func = IFunctionFactory.Factory(dist);
                            IFdaFunction fdaFunction = IFdaFunctionFactory.Factory(IParameterEnum.InflowFrequency, func);
                            if (func.State < IMessageLevels.Error && fdaFunction.State < IMessageLevels.Error)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return false;

        }

        public IFdaFunction CreateFdaFunction()
        {
            if(CanCreateValidFunction())
            {
                ICoordinatesFunction func =  GetCoordinatesFunction();
                return IFdaFunctionFactory.Factory(IParameterEnum.InflowFrequency, (IFunction) func);
            }
            return null;
        }


        public override void SaveWhileEditing()
        {
            UpdateChartLineData();
            if (CanCreateValidFunction())
            {
                base.SaveWhileEditing();
            }
            else
            {
                //update error saving message
                TempErrors.Add(LogItemFactory.FactoryTemp(LoggingLevel.Fatal, "Could not construct a valid function"));
                UpdateMessages(true);
            }
        }

        public void AddRows(int startRow, int numRows)
        {
            for(int i = 0;i<numRows;i++)
            {
                FlowDoubleWrapper emptyFlow = new FlowDoubleWrapper(0);
                AnalyticalFlows.Insert(startRow, emptyFlow);
            }
        }

        /// <summary>
        /// Adds a row to the end of the table. This happens when Enter is pressed while in
        /// the last row.
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="numRows"></param>
        public void AddRow()
        {
           
            FlowDoubleWrapper emptyFlow = new FlowDoubleWrapper(0);
            AnalyticalFlows.Add(emptyFlow);
            
        }

        public void DeleteRows(List<int> indexes)
        {
            for (int i = 0; i < indexes.Count; i++)
            {
                AnalyticalFlows.RemoveAt(indexes[i] - i);
            }
            //if all rows are gone then tell the parent so that it can delete this table
            if (AnalyticalFlows.Count == 0)
            {
                AnalyticalFlows.Add(new FlowDoubleWrapper(0));
                //NoMoreRows?.Invoke(this, new EventArgs());
            }
            //TableWasModified?.Invoke(this, new EventArgs());
        }

    }
}
