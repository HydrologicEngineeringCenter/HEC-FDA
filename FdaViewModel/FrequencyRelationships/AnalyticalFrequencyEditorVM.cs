using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaLogging;
using FdaViewModel.Editors;
using FdaViewModel.Utilities;
using FdaViewModel.Utilities.Transactions;
using Functions;
using FunctionsView.ViewModel;
using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using Model;
using Statistics;
using Utilities;

namespace FdaViewModel.FrequencyRelationships
{
    public class AnalyticalFrequencyEditorVM :CurveEditorVM
    {
        #region Notes
        #endregion
        #region Fields
        
        private IFdaFunction _Curve;
       // private System.Collections.ObjectModel.ObservableCollection<object> _Items;
        private System.Collections.ObjectModel.ObservableCollection<double> _Probabilities = new System.Collections.ObjectModel.ObservableCollection<double>();
        // private double _TestKnowledge = .9;
        // private double _TestNatural = .01;
        // private string _SavingText;
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


        //public Action<Utilities.ISaveUndoRedo> SaveAction { get; set; }
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

        //public int SelectedIndexInUndoList
        //{
        //    set { CurrentElement.ChangeIndex += value + 1; Undo(); }
        //}


        //public string Description
        //{
        //    get { return _Description; }
        //    set { _Description = value; NotifyPropertyChanged(); }
        //}
        public CoordinatesFunctionEditorVM EditorVM
        {
            get;
            set;
        }
        //public IFdaFunction Curve
        //{
        //    get { return _Curve; }
        //    set { _Curve = value; NotifyPropertyChanged(); UpdateItems(); }
        //}
        //public ObservableCollection<object> Items
        //{
        //    get { return _Items; }
        //    set { _Items = value; NotifyPropertyChanged(); }
        //}
        public ObservableCollection<double> Probabilities
        {
            get { return _Probabilities; }
            set { _Probabilities = value; NotifyPropertyChanged(); UpdateItems(); }
        }
        //public string SavingText
        //{
        //    get { return _SavingText; }
        //    set { _SavingText = value; NotifyPropertyChanged(); }
        //}

        //todo: Refactor: commented out the props below
        //public double Mean 
        //{ 
        //    get 
        //    { 
        //        //todo: Refactor: How to deal with LP3. I think i will need to make it public and just cast the IFdaFunction to an LP3
        //        return _Curve.GetMean; 
        //    } 
        //    set 
        //    { 
        //        _Curve = new Statistics.LogPearsonIII(value, _Curve.GetStDev, _Curve.GetG, _Curve.GetSampleSize); NotifyPropertyChanged(); 
        //    } 
        //}
        //public double StandardDeviation 
        //{ 
        //    get 
        //    { 
        //        return _Curve.GetStDev; 
        //    } 
        //    set 
        //    { 
        //        _Curve = new Statistics.LogPearsonIII(_Curve.GetMean, value, _Curve.GetG, _Curve.GetSampleSize); NotifyPropertyChanged(); 
        //    } 
        //}
        //public double Skew 
        //{ 
        //    get 
        //    { 
        //        return _Curve.GetG; 
        //    } 
        //    set 
        //    { 
        //        _Curve = new Statistics.LogPearsonIII(_Curve.GetMean, _Curve.GetStDev, value, _Curve.GetSampleSize); NotifyPropertyChanged(); 
        //    } 
        //}
        //public int SampleSize 
        //{ 
        //    get 
        //    { 
        //        return _Curve.GetSampleSize; 
        //    } 
        //    set 
        //    { 
        //        _Curve = new Statistics.LogPearsonIII(_Curve.GetMean, _Curve.GetStDev, _Curve.GetG, value); NotifyPropertyChanged(); 
        //    } 
        //}
        //public double TestKnowledge 
        //{ 
        //    get 
        //    { 
        //        return _TestKnowledge; 
        //    } 
        //    set 
        //    { 
        //        _TestKnowledge = value; NotifyPropertyChanged(); NotifyPropertyChanged(nameof(Result)); 
        //    } 
        //}
        //public double TestNatural { get { return _TestNatural; } set { _TestNatural = value; NotifyPropertyChanged(); NotifyPropertyChanged(nameof(Result)); } }
        //public double Result 
        //{ 
        //    get 
        //    {
        //        return -1;//todo: Refactor: CO _Curve.GetG; 
        //    } 
        //}

        //public ObservableCollection<TransactionRowItem> TransactionRows
        //{
        //    get;
        //    set;
        //}
        //public ObservableCollection<FdaLogging.LogItem> MessageRows
        //{
        //    get;
        //    set;
        //}
        //public int MessageCount
        //{
        //    get { return MessageRows.Count; }
        //}
        //public bool TransactionsMessagesVisible
        //{
        //    get;
        //    set;
        //}

        //public ChildElement CurrentElement { get; set; }


        #endregion
        #region Constructors
        public AnalyticalFrequencyEditorVM(IFdaFunction defaultCurve, string xLabel,string yLabel,string chartTitle, Editors.EditorActionManager actionManager) : base(defaultCurve, xLabel, yLabel, chartTitle, actionManager)
        {
            //EditorVM = new FunctionsView.ViewModel.CoordinatesFunctionEditorVM(defaultCurve.Function);

            _Curve = null;//todo: Refactor: CO new Statistics.LogPearsonIII(4, .4, .5, 50);
            Probabilities = new System.Collections.ObjectModel.ObservableCollection<double>() { .99, .95, .9, .8, .7, .6, .5, .4, .3, .2, .1, .05, .01 };
            //ActionManager = actionManager;
            //TransactionRows = new ObservableCollection<TransactionRowItem>();
            //MessageRows = new ObservableCollection<FdaLogging.LogItem>();
            UpdateChartLineData();
            LoadDefaultFlows();
        }
        public AnalyticalFrequencyEditorVM(AnalyticalFrequencyElement elem,string xLabel,string yLabel,string chartTitle, Editors.EditorActionManager actionManager) :base(elem, xLabel, yLabel, chartTitle, actionManager)// string name, Statistics.LogPearsonIII lpiii, string description, Utilities.OwnerElement owner) : base()
        {
            //OriginalName = elem.Name;
            CurrentElement = elem;
            //CurrentElement.ChangeIndex = 0;
            Probabilities = new ObservableCollection<double>() { .99, .95, .9, .8, .7, .6, .5, .4, .3, .2, .1, .05, .01 };
            //AssignValuesFromElementToEditor(elem);
            //ActionManager = actionManager;
            //TransactionHelper.LoadTransactionsAndMessages(this, elem);
            //SavingText = elem.Name + " last saved: " + elem.LastEditDate;
            UpdateChartLineData();
            if(elem.AnalyticalFlows.Count == 0)
            {
                LoadDefaultFlows();
            }
            //DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(CurrentElement.ChangeTableName());
            //UpdateUndoRedoVisibility(changeTableView, CurrentElement.ChangeIndex);

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

                CoordinatesFunctionEditorChartHelper chartHelper = new CoordinatesFunctionEditorChartHelper(func);
                List<SciLineData> lineData = chartHelper.CreateLineData(false, true, true);
                StandardChartViewModel.LineData.Set(lineData);
            }
            catch(Exception e)
            {
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


        //public void Undo()
        //{
        //    ChildElement prevElement = ActionManager.SaveUndoRedoHelper.UndoElement(CurrentElement);
        //    if (prevElement != null)
        //    {
        //        AssignValuesFromElementToEditor(prevElement);
        //        SavingText = prevElement.Name + " last saved: " + prevElement.LastEditDate;
        //        TransactionRows.Insert(0, new TransactionRowItem(DateTime.Now.ToString("G"), "Previously saved values", "me"));
        //    }
        //}

        //public void Redo()
        //{
        //    ChildElement nextElement = ActionManager.SaveUndoRedoHelper.RedoElement(CurrentElement);
        //    if (nextElement != null)
        //    {
        //        AssignValuesFromElementToEditor(nextElement);
        //        SavingText = nextElement.Name + " last saved: " + nextElement.LastEditDate;

        //    }
        //}

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

        //public override void SaveWhileEditing()
        //{
        //    if (!HasChanges)
        //    {
        //        //todo: it looks like this never gets hit. It always has changes.
        //        String time = DateTime.Now.ToString();
        //        LogItem li = LogItemFactory.FactoryTemp(LoggingLevel.Info, "No new changes to save." + time);
        //        MessageRows.Insert(0, li);
        //        SaveStatusLevel = LoggingLevel.Debug;
        //        return;
        //    }

        //    try
        //    {

        //        //try to construct the new coordinates function
        //        ICoordinatesFunction coordFunc = GetCoordinatesFunction();
        //        UpdateLineData(coordFunc);
        //        //EditorVM.Function = coordFunc;
        //        //todo: what is this, i can't just assume its a rating curve? This line needs to be here to save the curve out properly.
        //        //I think i just needed some enum to be there so i chose rating.
        //        //Curve = IFdaFunctionFactory.Factory(IParameterEnum.Rating, (IFunction)coordFunc);


        //    }
        //    catch (Exception ex)
        //    {
        //        //we were unsuccessful in creating the coordinates function                
        //        TempErrors.Add(LogItemFactory.FactoryTemp(LoggingLevel.Fatal, ex.Message));
        //        UpdateMessages(true);
        //        return;
        //    }


        //    InTheProcessOfSaving = true;
        //    ChildElement elementToSave = ActionManager.SaveUndoRedoHelper.CreateElementFromEditorAction(this);
        //    if (CurrentElement == null)
        //    {
        //        CurrentElement = elementToSave;
        //    }

        //    LastEditDate = DateTime.Now.ToString("G");
        //    elementToSave.LastEditDate = LastEditDate;
        //    CurrentElement.LastEditDate = LastEditDate;
        //    elementToSave.Curve = Curve;
        //    ActionManager.SaveUndoRedoHelper.Save(CurrentElement.Name, CurrentElement, elementToSave);
        //    //saving puts all the right values in the db but does not update the owned element in the tree. (in memory values)
        //    // i need to update those properties here
        //    AssignValuesFromEditorToCurrentElement();

        //    //update the rules to exclude the new name from the banned list
        //    //OwnerValidationRules.Invoke(this, _CurrentElement.Name);  
        //    SavingText = CreateLastSavedText(elementToSave);

        //    ReloadMessages(true);
        //    HasChanges = false;
        //}

        //public void SaveWhileEditing()
        //{
        //    SavingText = " Saving...";
        //    ChildElement elementToSave = ActionManager.SaveUndoRedoHelper.CreateElementFromEditorAction(this);
        //    if (CurrentElement == null)
        //    {
        //        CurrentElement = elementToSave;
        //    }
        //    LastEditDate = DateTime.Now.ToString("G");
        //    elementToSave.LastEditDate = LastEditDate;
        //    CurrentElement.LastEditDate = LastEditDate;
        //    ActionManager.SaveUndoRedoHelper.Save(CurrentElement.Name, CurrentElement, elementToSave);
        //    //saving puts all the right values in the db but does not update the owned element in the tree. (in memory values)
        //    // i need to update those properties here
        //    AssignValuesFromEditorToCurrentElement();

        //    //update the rules to exclude the new name from the banned list
        //    //OwnerValidationRules.Invoke(this, _CurrentElement.Name);  
        //    SavingText = " Saved at " + DateTime.Now.ToShortTimeString();
        //}

        //public override void Save()
        //{
        //    SaveWhileEditing();
        //}

        //public void UpdateTheUndoRedoRowItems()
        //{
        //    //int currentIndex = CurrentElement.ChangeIndex;
        //    //RedoRows.Clear();
        //    //for (int i = currentIndex + 1; i < UndoRedoRows.Count; i++)
        //    //{
        //    //    RedoRows.Add(UndoRedoRows[i]);
        //    //}

        //}

        //public void AssignValuesFromElementToEditor(ChildElement element)
        //{
        //    //todo: Refactor: CO
        //    //AnalyticalFrequencyElement elem = (AnalyticalFrequencyElement)element;
        //    //Name = elem.Name;
        //    //LastEditDate = elem.LastEditDate;
        //    //Description = elem.Description;
        //    //_Curve = elem.Distribution;
        //    //Mean = elem.Distribution.GetMean;
        //    //StandardDeviation = elem.Distribution.GetStDev;
        //    //Skew = elem.Distribution.GetG;
        //    //SampleSize = elem.Distribution.GetSampleSize;

        //}
        //public void AssignValuesFromEditorToCurrentElement()
        //{
        //    CurrentElement.LastEditDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
        //    CurrentElement.Name = Name;
        //    ((AnalyticalFrequencyElement)CurrentElement).Description = Description;
        //    ((AnalyticalFrequencyElement)CurrentElement).Curve = _Curve;
        //}

        //public void FilterRowsByLevel(FdaLogging.LoggingLevel level)
        //{

        //    ObservableCollection<FdaLogging.LogItem> tempList = new ObservableCollection<FdaLogging.LogItem>();
        //    foreach (FdaLogging.LogItem mri in MessageRows)
        //    {
        //        if (mri.LogLevel.Equals(level.ToString()))
        //        {
        //            tempList.Add(mri);
        //        }
        //    }

        //    MessageRows = tempList;

        //}
        //public void DisplayAllMessages()
        //{
        //    //MessageRows = NLogDataBaseHelper.GetMessageRowsForType(GetType());
        //}
        //public UncertainCurveDataCollection GetTheElementsCurve()
        //{
        //    FdaModel.Functions.FrequencyFunctions.LogPearsonIII lp3 = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(Distribution, FdaModel.Functions.FunctionTypes.InflowFrequency);
        //    Statistics.CurveIncreasing curve = lp3.GetOrdinatesFunction().Function;
        //    //return (UncertainCurveDataCollection)curve;
        //    throw new NotImplementedException();

        //}

        //public UncertainCurveDataCollection GetTheEditorsCurve()
        //{
        //    //return Distribution;
        //    throw new NotImplementedException();

        //}
        //public void UpdateNameWithNewValue(string name)
        //{
        //    Name = name;
        //}


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
