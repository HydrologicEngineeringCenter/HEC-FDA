using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.Plotting.SciChart2D.ViewModel;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    public class AnalyticalFrequencyEditorVM :CurveEditorVM
    {
        #region Notes
        #endregion
        #region Fields     
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

        #endregion
        #region Constructors
        public AnalyticalFrequencyEditorVM(ComputeComponentVM defaultCurve,  EditorActionManager actionManager) : base(defaultCurve, actionManager)
        {
            UpdateChartLineData();
            LoadDefaultFlows();
        }
        public AnalyticalFrequencyEditorVM(AnalyticalFrequencyElement elem,string xLabel,string yLabel,string chartTitle, EditorActionManager actionManager) :base(elem, actionManager)// string name, Statistics.LogPearsonIII lpiii, string description, Utilities.OwnerElement owner) : base()
        {
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
                //UncertainPairedData function = GetCoordinatesFunction();

                //todo: leaving this here until we get the new table and plot 

                //List<SciLineData> lineData = chartHelper.CreateLineData(false, true, true);
                //StandardChartViewModel.LineData.Set(lineData);
            }
            catch (Exception e)
            {
                //do nothing?
            }
        }

        #endregion

        //public override UncertainPairedData GetCoordinatesFunction()
        //{
        //    double[] probs = new double[] { .001, .01, .05, .25, .5, .75, .95, .99, .999 };
        //    List<double> yVals = new List<double>();

        //    try
        //    {
        //        if (IsAnalytical)
        //        {
        //            LogPearson3 lp3 = new LogPearson3();
        //            if (IsStandard)
        //            {
        //                lp3 = new LogPearson3(Mean, StandardDeviation, Skew, PeriodOfRecord);
        //            }
        //            else
        //            {
        //                //this is fit to flow
        //                List<double> flows = new List<double>();
        //                foreach(FlowDoubleWrapper d in AnalyticalFlows)
        //                {
        //                    flows.Add(d.Flow);
        //                }

        //                lp3 = (LogPearson3)lp3.Fit(flows.ToArray());
        //            }
        //            lp3.Validate();
        //            if (lp3.HasErrors)
        //            {
        //                System.Collections.IEnumerable enumerable = lp3.GetErrors();
        //                ErrorLevel errorLevel = lp3.ErrorLevel;
        //                //todo: do what?
        //            }
        //            else
        //            {
        //                double mean = lp3.Mean;
        //                double stDev = lp3.StandardDeviation;
        //                double skew = lp3.Skewness;
        //                FitToFlowMean = "Mean: " + mean.ToString(".##");
        //                FitToFlowStDev = "St. Dev.: " + stDev.ToString(".##");
        //                FitToFlowSkew = "Skew: " + skew.ToString(".##");
        //                foreach (double prob in probs)
        //                {
        //                    yVals.Add(lp3.InverseCDF(prob));
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        FitToFlowMean = "Mean: N/A";
        //        FitToFlowStDev = "Mean: N/A";
        //        FitToFlowSkew = "Mean: N/A";
        //        //todo: do what?
        //        //return Utilities.DefaultPairedData.CreateDefaultDeterminateUncertainPairedData(xs, ys, "", "", "");
        //    }
        //    FitToFlowMean = "Mean: N/A";
        //    FitToFlowStDev = "Mean: N/A";
        //    FitToFlowSkew = "Mean: N/A";
        //    return Utilities.UncertainPairedDataFactory.CreateDeterminateData(new List<double>(probs), yVals, "Frequency", "Flow", "Flow-Frequency");
        //}

        //todo: is this necessary?
        public bool CanCreateValidFunction()
        {
            List<double> xs = new List<double>();
            List<double> ys = new List<double>();
            LogPearson3 lp3 = new LogPearson3();

            try
            {
                if (IsAnalytical)
                {
                    if (IsStandard)
                    {
                        lp3 = new LogPearson3(Mean, StandardDeviation, Skew, PeriodOfRecord);
                    }
                    else
                    {
                        List<double> flows = new List<double>();
                        foreach (FlowDoubleWrapper d in AnalyticalFlows)
                        {
                            flows.Add(d.Flow);
                        }

                        lp3 = (LogPearson3)lp3.Fit(flows.ToArray());                     
                    }

                    lp3.Validate();
                    if (!lp3.HasErrors)
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return false;

        }

        public override void Save()
        {
            UpdateChartLineData();
            if (CanCreateValidFunction())
            {
                string editDate = DateTime.Now.ToString("G");
                double mean = Mean;
                double stDev = StandardDeviation;
                double skew = Skew;
                int por = PeriodOfRecord;
                bool isAnalytical = IsAnalytical;
                bool isStandard = IsStandard;
                bool isLogFlow = IsLogFlow;
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
                    isLogFlow, analyticalFlows, graphicalFlows, TableWithPlot.ComputeComponentVM, id);
                base.Save(elem);
            }
            else
            {
                //todo: i commented this out 2/21/22
                //update error saving message
                //TempErrors.Add(LogItemFactory.FactoryTemp(LoggingLevel.Fatal, "Could not construct a valid function"));
                //UpdateMessages(true);
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
            }
        }
    }
}
