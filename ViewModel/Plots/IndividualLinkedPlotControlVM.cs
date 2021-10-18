using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.ImpactAreaScenario;
using ViewModel.Utilities;
using Functions;
using HEC.Plotting.Core;
using Model;

namespace ViewModel.Plots
{
    //[Author(q0heccdm, 12 / 19 / 2017 1:26:57 PM)]
    public class IndividualLinkedPlotControlVM : BaseViewModel, iIASControl
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/19/2017 1:26:57 PM
        #endregion
        #region Fields
        public event EventHandler PopPlot1IntoModulator;
        public event EventHandler PopPlot5IntoModulator;
        public event EventHandler PopPlotFailureIntoModulator;

        public event EventHandler SelectedCurveUpdated;
        public event EventHandler PlotIsShowing;
        public event EventHandler PlotIsNotShowing;
        public event EventHandler PopPlotOut;
        public event EventHandler PreviewCompute;

        public event EventHandler UpdateDLMLines;
        public event EventHandler UpdateHorizontalDLMLines;
        public event EventHandler UpdateHorizontalFailureFunction;

        private BaseViewModel _CurrentVM;
        private bool _UpdatePlots;
        private bool _IsModulator = false;

        //private bool _IsYAxisLog;
        //private bool _IsXAxisLog;

        //private bool _IsProbabilityXAxis;

        private double _MinX;
        private double _MaxX;
        private double _MinY;
        private double _MaxY;

        private double _CurrentX;
        private double _CurrentY;

        //private bool _XAxisOnBottom = true;
        //private bool _YAxisOnLeft = true;

        #endregion
        #region Properties

        //public double SharedXAxisMin { get; set; }
        //public double SharedXAxisMax { get; set; }
        //public double SharedYAxisMin { get; set; }
        //public double SharedYAxisMax { get; set; }

        public double MinX
        {
            get { return _MinX; }
            set { _MinX = value; NotifyPropertyChanged(); }
        }

        public double MaxX
        {
            get { return _MaxX; }
            set { _MaxX = value; NotifyPropertyChanged(); }
        }
        public double MinY
        {
            get { return _MinY; }
            set { _MinY = value; NotifyPropertyChanged(); }
        }
        public double MaxY
        {
            get { return _MaxY; }
            set { _MaxY = value; NotifyPropertyChanged(); }
        }
        public double CurrentX
        {
            get { return _CurrentX; }
            set { _CurrentX = value; NotifyPropertyChanged(); }
        }
        public double CurrentY
        {
            get { return _CurrentY; }
            set { _CurrentY = value; NotifyPropertyChanged(); }
        }
        public bool IsModulator
        {
            get { return _IsModulator; }
            set { _IsModulator = value; }
        }
        public bool IsPlotShowing { get; set; }

        /// <summary>
        /// this is not ideal but i needed a way to tell the view side to update the plots when a new curve is selected
        /// from the importer. The curve change callback works if the importer is in the editor but it does not trigger
        /// the callback if it is popped into a tab or its own window. It does NOT matter whether this is true or false
        /// it just needs to change values to trigger the callback in the view side.
        /// </summary>
        public bool UpdatePlots
        {
            get { return _UpdatePlots; }
            set { _UpdatePlots = value;NotifyPropertyChanged(); }
        }
        public IndividualLinkedPlotVM PreviousPlot { get; set; }
        public BaseViewModel PreviousVM
        {
            get;
            set;
        }
        public BaseViewModel CurrentVM
        {
            get { return _CurrentVM; }
            set { PreviousVM = _CurrentVM; _CurrentVM = value; NotifyPropertyChanged(); }
        }

        public IIndividualLinkedPlotWrapper IndividualPlotWrapperVM { get; set; }
        //public IndividualLinkedPlotVM MyIndividualLinkedPlotVM { get; set; }
        public ICoverButton ImportButtonVM { get; set; }
        public iIASImporter CurveImporterVM { get; set; }

        public ICoverButton ModulatorCoverButtonVM { get; set; }
        public IIndividualLinkedPlotWrapper ModulatorPlotWrapperVM { get; set; }

        public CrosshairData CrosshairData { get; set; } 
        public FdaCrosshairChartModifier ChartModifier { get; set; }
        public IFdaFunctionEnum PlotType { get; set; }
        #endregion
        #region Constructors
        public IndividualLinkedPlotControlVM():base()
        {
           
        }
        public IndividualLinkedPlotControlVM(IFdaFunctionEnum type, IIndividualLinkedPlotWrapper indLinkedPlotWrapperVM, ICoverButton coverButton, iIASImporter importerVM, 
             ICoverButton modulatorCoverButton = null, IIndividualLinkedPlotWrapper modulatorPlotWrapper = null)
        {
            PlotType = type;
            //_XAxisOnBottom = isXAxisOnBottom;
            //_YAxisOnLeft = isYAxisOnLeft;
            //_IsYAxisLog = isYAxisLog;
            //_IsXAxisLog = isXAxisLog;

            //_IsProbabilityXAxis = isProbabilityXAxis;

            IndividualPlotWrapperVM = indLinkedPlotWrapperVM;
            IndividualPlotWrapperVM.ShowImportButton += new EventHandler(ShowTheImportButton);
            IndividualPlotWrapperVM.ShowTheImporter += new EventHandler(ImportButtonClicked);
            //IndividualPlotWrapperVM.TrackerIsOutsideRange += new EventHandler(TrackerIsOutsideCurveRange);
            //IndividualPlotWrapperVM.CurveUpdated += SelectedCurveUpdated;

            ImportButtonVM = coverButton;
            ImportButtonVM.Clicked += new EventHandler(ImportButtonClicked);

            

            if(modulatorCoverButton != null)
            {
                _IsModulator = true;
                ModulatorCoverButtonVM = modulatorCoverButton;
                ModulatorPlotWrapperVM = modulatorPlotWrapper;
                CurrentVM = (BaseViewModel)ModulatorCoverButtonVM;
            }
            else
            {
                CurrentVM = (BaseViewModel)ImportButtonVM;
            }

            CurveImporterVM = importerVM;
            if (importerVM != null && _IsModulator == false)
            {
                CurveImporterVM.OKClickedEvent += new EventHandler(AddCurveToPlot);
                CurveImporterVM.CancelClickedEvent += new EventHandler(ImporterWasCanceled);
                CurveImporterVM.PopImporterOut += new EventHandler(PopTheImporterOut);
            }
            else
            {
                if (PlotType == IFdaFunctionEnum.InflowOutflow)
                {
                    CurveImporterVM.OKClickedEvent += PopPlot1DirectlyIntoModulator;
                    CurveImporterVM.CancelClickedEvent += new EventHandler(ImporterWasCanceled);
                    CurveImporterVM.PopImporterOut += new EventHandler(PopTheImporterOut);
                }
                if(PlotType == IFdaFunctionEnum.ExteriorInteriorStage)
                {
                    CurveImporterVM.OKClickedEvent += PopPlot5DirectlyIntoModulator;
                    CurveImporterVM.CancelClickedEvent += new EventHandler(ImporterWasCanceled);
                    CurveImporterVM.PopImporterOut += new EventHandler(PopTheImporterOut);
                }
                if(PlotType == IFdaFunctionEnum.LateralStructureFailure)
                {
                    CurveImporterVM.OKClickedEvent += PopPlotFailureDirectlyIntoModulator;
                    CurveImporterVM.CancelClickedEvent += new EventHandler(ImporterWasCanceled);
                    CurveImporterVM.PopImporterOut += new EventHandler(PopTheImporterOut);
                }
            }

        }
        #endregion
        #region Voids

        //private void ModulatorCoverButtonClicked(object sender, EventArgs e)
        //{
        //    //pop plot 1 out
        //    //set the current vm to be the importer? this might already be getting fired?
        //}

        private void PopTheImporterOut(object sender, EventArgs e)
        {
            CurveImporterVM.IsPoppedOut = true;
            //IndividualPlotWrapperVM.DisplayImportButton = false;

            ShowPreviousVM(sender, e);
            //CurveImporterVM.CancelClickedEvent += ImporterWasCanceled
            string header = "Importer";
            DynamicTabVM tab = new DynamicTabVM(header, (BaseViewModel)CurveImporterVM, "Plot1Importer");
            Navigate(tab,false,false);
            //if (((BaseViewModel)CurveImporterVM).WasCanceled == true)
            //{
            //    CurveImporterVM.IsPoppedOut = false;
            //}
        }

        private void ImporterWasCanceled(object sender, EventArgs e)
        {
            //if there is a previous curve then set it
            if (PreviousPlot != null)
            {
                IndividualPlotWrapperVM.PlotVM = PreviousPlot;
            }
            //update the linkages

            if(CurrentVM == CurveImporterVM)
            {
                ShowPreviousVM(sender, e);
            }

            CurveImporterVM.IsPoppedOut = false;
            //IndividualPlotWrapperVM.DisplayImportButton = true;

            //if (CurveImporterVM.IsPoppedOut == true)
            //{

            //    CurveImporterVM.IsPoppedOut = false;
            //}
            //else
            //{
            //    ShowPreviousVM(sender, e);
            //}

        }

        /// <summary>
        /// This gets called when the user is on the import curve form but clicks the cancel button. I will take them back to the VM they were previously on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ShowPreviousVM(object sender, EventArgs e)
        {
            CurrentVM = PreviousVM;
        }

        //public void TrackerIsOutsideCurveRange(object sender, EventArgs e)
        //{
        //    int test = 0;
        //    //CurrentVM = (BaseViewModel)ImportButtonVM;
        //    //PlotIsNotShowingAnymore(sender, e);
        //}
        /// <summary>
        /// this gets called when the user clicks the delete button on the plot wrapper
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ShowTheImportButton(object sender, EventArgs e)
        {
            CurrentVM = (BaseViewModel)ImportButtonVM;
            PlotIsNotShowingAnymore(sender, e);
        }
        /// <summary>
        /// This gets called when the user clicks the import curve button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ImportButtonClicked(object sender,EventArgs e)
        {
            //special logic for the plot8 preview. I want to skip the importer and run the preview compute logic
            if(CurveImporterVM == null)
            {
                PreviewTheCompute(this, new EventArgs());
                return;
            }

            //this gets a little crazy but if the user clicks the modulator cover button the importer will pop up.
            //if they then click cancel i want the view to show the importer button, not the modulator importer button.
           bool previousViewShouldBeTheCoverButton = false;
           if(CurrentVM == ModulatorCoverButtonVM)
            {
                previousViewShouldBeTheCoverButton = true;
            }

            
            //update the linkages, in the curve changed call back, if it gets set to null, then take it away from the selected curve prop
            //this feels hacky but the curve in the oxyplot is going to get set to null automatically. I am not totally sure why. This will
            //cause a crash if the user moves the mouse over one of the other plots becuase the curve will be null on this plot.
            //i am therefore preemptively setting the curve to null and updating the linkages.
            //8/30/18 this doesn't seem to be the case anymore so i commented it out
            //if (IndividualPlotWrapperVM.PlotVM != null)
            //{
            //   // IndividualPlotWrapperVM.PlotVM.Curve = null;
            //}
            
            CurrentVM = (BaseViewModel)CurveImporterVM;

            if (previousViewShouldBeTheCoverButton == true)
            {
                PreviousVM = (BaseViewModel)ImportButtonVM;
            }
            
        }

       private void UpdateThePlots()
        {
            //i just need to switch it
            if(UpdatePlots)
            {
                UpdatePlots = false;
            }
            else
            {
                UpdatePlots = true;
            }
        }

        private void SetChartModifier(IFdaFunction function)
        {
            CrosshairData = new CrosshairData(function);
            IParameterEnum funcType = function.ParameterType;
            switch(funcType)
            {
                case IParameterEnum.InflowFrequency:
                    {
                        ChartModifier = new FdaCrosshairChartModifier(false, false, CrosshairData);
                        break;
                    }
                case IParameterEnum.InflowOutflow:
                    {
                        ChartModifier = new FdaCrosshairChartModifier(false, false, CrosshairData);
                        CrosshairData.UpdateModulator += UpdateDoubleLineModulator;
                        break;
                    }
                case IParameterEnum.Rating:
                    {
                        ChartModifier = new FdaCrosshairChartModifier(false, true, CrosshairData);
                        break;
                    }
                case IParameterEnum.LateralStructureFailure:
                    {
                        ChartModifier = new FdaCrosshairChartModifier(false, true, CrosshairData);
                        CrosshairData.UpdateHorizontalFailureFunction += UpdateHorizontalLateralStructure;
                        break;
                    }
                case IParameterEnum.ExteriorInteriorStage:
                    {
                        ChartModifier = new FdaCrosshairChartModifier(true, true, CrosshairData);
                        CrosshairData.UpdateHorizontalModulator += UpdateHorizontalDoubleLineModulator;
                        break;
                    }
                case IParameterEnum.InteriorStageDamage:
                    {
                        ChartModifier = new FdaCrosshairChartModifier(true, true, CrosshairData);
                        break;
                    }
                case IParameterEnum.DamageFrequency:
                    {
                        ChartModifier = new FdaCrosshairChartModifier(true, false, CrosshairData);
                        break;
                    }               
                default:
                    {
                        int i = 0;
                        break;
                    }
            }
        }

        public void PopPlot5DirectlyIntoModulator(object sender, EventArgs e)
        {
            IsPlotShowing = true;
            IFdaFunction function = CurveImporterVM.SelectedCurve;
            string elementName = CurveImporterVM.SelectedElementName;
            SetChartModifier(function);
            PreviousPlot = IndividualPlotWrapperVM.PlotVM;// new IndividualLinkedPlotVM(function, elementName, null,_IsXAxisLog, _IsYAxisLog, _IsProbabilityXAxis, _XAxisOnBottom, _YAxisOnLeft);

            var modifier = new FdaCrosshairChartModifier(ChartModifier);
            ChartModifier = modifier;

            IndividualPlotWrapperVM.AddCurveToPlot(function, elementName,CurveImporterVM.SelectedElement.GetElementID(), modifier);
            CurveImporterVM.IsPoppedOut = false;
            PopPlot5IntoHorizontalModulator();
            PopPlot5IntoModulator?.Invoke(this, new EventArgs());

            SelectedCurveUpdated?.Invoke(this, new EventArgs());
            UpdateThePlots();
        }

        public void PopPlot1DirectlyIntoModulator(object sender, EventArgs e)
        {
            IsPlotShowing = true;
            IFdaFunction function = CurveImporterVM.SelectedCurve;
            string elementName = CurveImporterVM.SelectedElementName;
            SetChartModifier(function);
            PreviousPlot = IndividualPlotWrapperVM.PlotVM;// new IndividualLinkedPlotVM(function, elementName, null,_IsXAxisLog, _IsYAxisLog, _IsProbabilityXAxis, _XAxisOnBottom, _YAxisOnLeft);

            var modifier = new FdaCrosshairChartModifier(ChartModifier);
            ChartModifier = modifier;

            IndividualPlotWrapperVM.AddCurveToPlot(function, elementName, CurveImporterVM.SelectedElement.GetElementID(), modifier);
            CurveImporterVM.IsPoppedOut = false;
            PopPlotIntoModulator();
            PopPlot1IntoModulator?.Invoke(this, new EventArgs());

            SelectedCurveUpdated?.Invoke(this, new EventArgs());
            UpdateThePlots();
        }

        public void PopPlotIntoModulator()
        {
            CurrentVM = (BaseViewModel)ModulatorPlotWrapperVM;
            CrosshairData.IsPlot1Modulator = true;
            SetMinMaxModulatorValues();
            UpdateDLMLines?.Invoke(this, new EventArgs());
            PlotIsShowing?.Invoke(this, new EventArgs());

        }

        public void PopPlot5IntoHorizontalModulator()
        {
            CurrentVM = (BaseViewModel)ModulatorPlotWrapperVM;
            CrosshairData.IsPlot5Modulator = true;
            SetMinMaxModulatorValues();
            UpdateHorizontalDLMLines?.Invoke(this, new EventArgs());
            PlotIsShowing?.Invoke(this, new EventArgs());

        }

        public void PopPlotFailureDirectlyIntoModulator(object sender, EventArgs e)
        {
            IsPlotShowing = true;
            IFdaFunction function = CurveImporterVM.SelectedCurve;
            string elementName = CurveImporterVM.SelectedElementName;
            SetChartModifier(function);
            PreviousPlot = IndividualPlotWrapperVM.PlotVM;// new IndividualLinkedPlotVM(function, elementName, null,_IsXAxisLog, _IsYAxisLog, _IsProbabilityXAxis, _XAxisOnBottom, _YAxisOnLeft);

            var modifier = new FdaCrosshairChartModifier(ChartModifier);
            ChartModifier = modifier;

            IndividualPlotWrapperVM.AddCurveToPlot(function, elementName, CurveImporterVM.SelectedElement.GetElementID(), modifier);
            CurveImporterVM.IsPoppedOut = false;
            PopFailureFunctionIntoHorizontalModulator();
            PopPlotFailureIntoModulator?.Invoke(this, new EventArgs());

            SelectedCurveUpdated?.Invoke(this, new EventArgs());
            UpdateThePlots();
        }

        public void PopFailureFunctionIntoHorizontalModulator()
        {
            CurrentVM = (BaseViewModel)ModulatorPlotWrapperVM;
            CrosshairData.IsPlotFailureFunction = true;
            SetMinMaxModulatorValues();
            UpdateHorizontalFailureFunction?.Invoke(this, new EventArgs());
            PlotIsShowing?.Invoke(this, new EventArgs());

        }

        public void SetMinMaxModulatorValues()
        {
            IFdaFunction func = IndividualPlotWrapperVM.PlotVM.BaseFunction;
            MinX = func.Coordinates[0].X.Value();
            MinY = func.Coordinates[0].Y.Value();

            MaxX = func.Coordinates[func.Coordinates.Count - 1].X.Value();
            MaxY = func.Coordinates[func.Coordinates.Count - 1].Y.Value();


        }

        public void SetCurrentToModulatorCoverButton()
        {
            CurrentVM = (BaseViewModel)ModulatorCoverButtonVM;
        }

        public void AddCurveToPlot(IFdaFunction function, string elementName, int selectedElementID)
        {
            SetChartModifier(function);

            //store the previouse curve
            PreviousPlot = IndividualPlotWrapperVM.PlotVM;// new IndividualLinkedPlotVM(function, elementName, null,_IsXAxisLog, _IsYAxisLog, _IsProbabilityXAxis, _XAxisOnBottom, _YAxisOnLeft);

            //if(_IsModulator)
            //{
            //    ModulatorPlotWrapperVM.PlotVM = new IndividualLinkedPlotVM(function, elementName, ChartModifier, _IsYAxisLog, _IsProbabilityXAxis);
            //    CurrentVM = (BaseViewModel)ModulatorPlotWrapperVM;
            //}
            //else
            {
                //IndividualPlotWrapperVM.PlotVM = new IndividualLinkedPlotVM(function, elementName, ChartModifier,_IsXAxisLog, _IsYAxisLog, _IsProbabilityXAxis, _XAxisOnBottom,
                 //   _YAxisOnLeft);

                 var modifier = new FdaCrosshairChartModifier(ChartModifier);
                 ChartModifier = modifier;

                IndividualPlotWrapperVM.AddCurveToPlot(function, elementName, selectedElementID, modifier);
                CurrentVM = (BaseViewModel)IndividualPlotWrapperVM;

            }




            //if (ModulatorPlotWrapperVM != null)
            //{
            //    ModulatorPlotWrapperVM.PlotVM = new IndividualLinkedPlotVM(function, elementName, ChartModifier);
            //    CurrentVM = (BaseViewModel)ModulatorPlotWrapperVM;

            //}

            //tell the conditions plot editor vm that this plot is showing
            IsPlotShowing = true;
            PlotIsShowing?.Invoke(this, new EventArgs());

            //raise the updateplots event - is this still necesarry i update the plots in the ind linked plots loaded event
            SelectedCurveUpdated?.Invoke(this, new EventArgs());
            UpdateThePlots();
        }

        /// <summary>
        /// This gets called when the user clicks the OK button on the importer form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddCurveToPlot(object sender,EventArgs e)
        {

            CurveImporterVM.IsPoppedOut = false;
            //IndividualPlotWrapperVM.DisplayImportButton = true;

            IndividualPlotWrapperVM.SubTitle = CurveImporterVM.SelectedElement.Name;

            AddCurveToPlot(CurveImporterVM.SelectedCurve, CurveImporterVM.SelectedElementName,CurveImporterVM.SelectedElement.GetElementID());

            //SetChartModifier(CurveImporterVM.SelectedCurve);

            ////switch to the plot vm
            ////if (IndividualPlotWrapperVM.PlotVM == null)
            ////{
            //    IndividualPlotWrapperVM.PlotVM = new IndividualLinkedPlotVM(CurveImporterVM.SelectedCurve, CurveImporterVM.SelectedElementName, ChartModifier);
            ////}
            ////else
            ////{
            ////    //clobbering the chart view model here in the hopes that the old chart modifier gets destroyed forever.
            ////    IndividualPlotWrapperVM.PlotVM.CoordinatesChartViewModel = new ConditionChartViewModel(CurveImporterVM.SelectedElementName);
            ////    IndividualPlotWrapperVM.PlotVM.CoordinatesChartViewModel.ModifierGroup.ChildModifiers.Add(ChartModifier);
            ////    IndividualPlotWrapperVM.PlotVM.BaseFunction = CurveImporterVM.SelectedCurve;
            ////    IndividualPlotWrapperVM.PlotVM.SelectedElementName = CurveImporterVM.SelectedElementName;
            ////    IndividualPlotWrapperVM.PlotVM.Curve = CurveImporterVM.SelectedCurve;
            ////}

            //CurrentVM = (BaseViewModel)IndividualPlotWrapperVM;

            ////store the previouse curve
            //PreviousPlot = new IndividualLinkedPlotVM(CurveImporterVM.SelectedCurve, CurveImporterVM.SelectedElementName, null);

            //if (ModulatorPlotWrapperVM != null)
            //{
            //    ModulatorPlotWrapperVM.PlotVM = new IndividualLinkedPlotVM(CurveImporterVM.SelectedCurve, CurveImporterVM.SelectedElementName, null);
            //    //CurrentVM = (BaseViewModel)ModulatorPlotWrapperVM;
            //}

            ////tell the conditions plot editor vm that this plot is showing
            //IsPlotShowing = true;
            //if (PlotIsShowing != null)
            //{
            //    PlotIsShowing(sender, e);
            //}


            ////raise the updateplots event - is this still necesarry i update the plots in the ind linked plots loaded event
            //if (SelectedCurveUpdated != null)
            //{
            //    SelectedCurveUpdated(sender, e);
            //}
            //UpdateThePlots();
        }

        public void PreviewTheCompute(object sender, EventArgs e)
        {
            if (PreviewCompute != null)
            {
                PreviewCompute(sender, e);
            }
        }

        public void PlotIsNotShowingAnymore(object sender, EventArgs e)
        {
            //This is when you close the plot.
            //it might be the case that we actually want to clobber the links?
            //maybe clobber the chartModifier? 7/1/20
            IsPlotShowing = false;

            PlotIsNotShowing?.Invoke(sender, e);
            SelectedCurveUpdated?.Invoke(sender, e);

        }

        public void PopThePlotOut(object sender, EventArgs e)
        {
            if(PopPlotOut != null)
            {
                PopPlotOut(sender, e);
            }
        }

        public void SetCurrentViewToCoverButton()
        {
            //This is when you close the plot.
            //it might be the case that we actually want to clobber the links?
            //maybe clobber the chartModifier? 7/1/20
            CurrentVM = (BaseViewModel)ImportButtonVM;
        }

        public override void AddValidationRules()
        {
           // throw new NotImplementedException();
        }

       
        //public void LinkToNothing()
        //{
        //    //todo: need to do a switch on the "type" of this in order to set the "false" "false" correctly.
        //    ConditionChartViewModel currentChartVM = IndividualPlotWrapperVM.PlotVM.CoordinatesChartViewModel;

        //    ImpactAreaFunctionEnum currentType = IndividualPlotWrapperVM.PlotVM.BaseFunction.Type;
        //    currentChartVM.ModifierGroup.ChildModifiers.Add(new FdaCrosshairChartModifier(false, false, CrosshairData));
        //}

        private IParameterEnum GetFunctionType(IndividualLinkedPlotControlVM control)
        {
            IParameterEnum controlType;
            //if (control.IsModulator)
            //{
            //    controlType = control.ModulatorPlotWrapperVM.PlotVM.BaseFunction.Type;
            //}
            //else
            {
                controlType = control.IndividualPlotWrapperVM.PlotVM.BaseFunction.ParameterType;
            }
            return controlType;
        }

        public IASChartViewModel GetChartViewModel()
        {
            IASChartViewModel vm = null;
            vm = IndividualPlotWrapperVM.PlotVM.CoordinatesChartViewModel;
            return vm;
        }

        /// <summary>
        /// This method exists so that the view side can resize shared plots along the shared axis.
        /// </summary>
        /// <param name="otherControl"></param>
        /// <param name="isYAxis"></param>
        private void SetSharedAxisValues(IndividualLinkedPlotControlVM otherControl, bool isYAxis)
        {
            //both this control and the other control should have a curve in them
            //before we get here, but lets check here anyway.

            ICoordinate thisFirstCoord = IndividualPlotWrapperVM.PlotVM.BaseFunction.Coordinates[0];
            ICoordinate thisLastCoord = IndividualPlotWrapperVM.PlotVM.BaseFunction.Coordinates[IndividualPlotWrapperVM.PlotVM.BaseFunction.Coordinates.Count-1];

            ICoordinate otherFirstCoord = otherControl.IndividualPlotWrapperVM.PlotVM.BaseFunction.Coordinates[0];
            ICoordinate otherLastCoord = otherControl.IndividualPlotWrapperVM.PlotVM.BaseFunction.Coordinates[otherControl.IndividualPlotWrapperVM.PlotVM.BaseFunction.Coordinates.Count - 1];

            if (isYAxis)
            {
                double min = getMinSharedAxisValue(thisFirstCoord, otherFirstCoord, true);
                double max = getMaxSharedAxisValue(thisLastCoord, otherLastCoord, true);
                GetChartViewModel().SharedYAxisMin = min;
                GetChartViewModel().SharedYAxisMax = max;
                otherControl.GetChartViewModel().SharedYAxisMin = min;
                otherControl.GetChartViewModel().SharedYAxisMax = max;
            }
            else
            {
                double min = getMinSharedAxisValue(thisFirstCoord, otherFirstCoord, false);
                double max = getMaxSharedAxisValue(thisLastCoord, otherLastCoord, false);
                GetChartViewModel().SharedXAxisMin = min;
                GetChartViewModel().SharedXAxisMax = max;
                otherControl.GetChartViewModel().SharedXAxisMin = min;
                otherControl.GetChartViewModel().SharedXAxisMax = max;
            }
        }

        private double getMinSharedAxisValue(ICoordinate thisCoord, ICoordinate otherCoord, bool isYAxis)
        {
            double min = double.NaN;
            if(isYAxis)
            {
                min = thisCoord.Y.Value();
                if(otherCoord.Y.Value() < min)
                {
                    min = otherCoord.Y.Value();
                }
            }
            else
            {
                min = thisCoord.X.Value();
                if (otherCoord.X.Value() < min)
                {
                    min = otherCoord.X.Value();
                }
            }

            return min;
        }

        private double getMaxSharedAxisValue(ICoordinate thisCoord, ICoordinate otherCoord, bool isYAxis)
        {
            double max = double.NaN;
            if (isYAxis)
            {
                max = thisCoord.Y.Value();
                if (otherCoord.Y.Value() > max)
                {
                    max = otherCoord.Y.Value();
                }
            }
            else
            {
                max = thisCoord.X.Value();
                if (otherCoord.X.Value() > max)
                {
                    max = otherCoord.X.Value();
                }
            }

            return max;
        }

        /// <summary>
        /// This code should mirror the bindings that are happening in IndividualLinkedPlotControl.xaml.cs - BindToNextPlot()
        /// </summary>
        /// <param name="nextControl"></param>
        public void LinkToNextControl(IndividualLinkedPlotControlVM nextControl)
        {
            IParameterEnum thisType = GetFunctionType(this);
            IParameterEnum nextType = GetFunctionType(nextControl);

            IASChartViewModel currentChartVM = GetChartViewModel();
            IASChartViewModel nextChartVM = nextControl.GetChartViewModel();

            bool nextControlIsModulator = nextControl.IsModulator;

            CrosshairData currentCrosshairData = CrosshairData;
            CrosshairData nextCrosshairData = nextControl.CrosshairData;

            switch (thisType)
            {
                case IParameterEnum.InflowFrequency:
                    {
                        SetSharedAxisValues(nextControl, true);
                        //if i am inflow frequency, then i can only link to inflow outflow or to rating
                        if (nextType == IParameterEnum.InflowOutflow)
                        {
                            currentCrosshairData.Next = new SharedAxisCrosshairData(nextCrosshairData,Axis.X, Axis.Y);
                            nextCrosshairData.Previous = new SharedAxisCrosshairData(currentCrosshairData, Axis.Y, Axis.X);
                        }
                        else if (nextType == IParameterEnum.Rating)
                        {
                            currentCrosshairData.Next = new SharedAxisCrosshairData(nextCrosshairData, Axis.Y, Axis.Y);
                            nextCrosshairData.Previous = new SharedAxisCrosshairData(currentCrosshairData, Axis.Y, Axis.Y);
                            
                        }
                        break;
                    }
                case IParameterEnum.InflowOutflow:
                    {
                        if (nextType == IParameterEnum.Rating)
                        {
                            SetSharedAxisValues(nextControl, true);
                            currentCrosshairData.Next = new SharedAxisCrosshairData(nextCrosshairData, Axis.Y, Axis.Y);
                            nextCrosshairData.Previous = new SharedAxisCrosshairData(currentCrosshairData, Axis.Y, Axis.Y);
                        }
                        break;
                    }
                case IParameterEnum.Rating:
                    {
                        SetSharedAxisValues(nextControl, false);
                        if (nextType == IParameterEnum.ExteriorInteriorStage)
                        {
                            currentCrosshairData.Next = new SharedAxisCrosshairData(nextCrosshairData, Axis.X, Axis.X);
                            nextCrosshairData.Previous = new SharedAxisCrosshairData(currentCrosshairData, Axis.X, Axis.X);
                            //SetSharedAxisValues(nextControl, false);

                        }
                        else if (nextType == IParameterEnum.InteriorStageDamage)
                        {
                            currentCrosshairData.Next = new SharedAxisCrosshairData(nextCrosshairData, Axis.X, Axis.X);
                            //currentChartVM.ModifierGroup.ChildModifiers.Clear();
                            //currentChartVM.ModifierGroup.ChildModifiers.Add(new FdaCrosshairChartModifier(false, false, currentCrosshairData));

                            nextCrosshairData.Previous = new SharedAxisCrosshairData(currentCrosshairData, Axis.X, Axis.X);
                            //nextChartVM.ModifierGroup.ChildModifiers.Add(new FdaCrosshairChartModifier(true, true, nextCrosshairData));

                        }
                        else if (nextType == IParameterEnum.LateralStructureFailure)
                        {
                            currentCrosshairData.Next = new SharedAxisCrosshairData(nextCrosshairData, Axis.X, Axis.X);
                            nextCrosshairData.Previous = new SharedAxisCrosshairData(currentCrosshairData, Axis.X, Axis.X);
                        }
                        break;
                    }
                case IParameterEnum.LateralStructureFailure:
                    {
                        SetSharedAxisValues(nextControl, false);
                        if (nextType == IParameterEnum.ExteriorInteriorStage)
                        {
                            //todo: this might need to change once we get the mix and match axis binding worked out.
                            currentCrosshairData.Next = new SharedAxisCrosshairData(nextCrosshairData, Axis.X, Axis.X);
                            nextCrosshairData.Previous = new SharedAxisCrosshairData(currentCrosshairData, Axis.X, Axis.X);
                        }
                        if (nextType == IParameterEnum.InteriorStageDamage)
                        {
                            //SetSharedAxisValues(nextControl, false);
                            //todo: this might need to change once we get the mix and match axis binding worked out.
                            currentCrosshairData.Next = new SharedAxisCrosshairData(nextCrosshairData, Axis.X, Axis.X);
                            nextCrosshairData.Previous = new SharedAxisCrosshairData(currentCrosshairData, Axis.X, Axis.X);
                        }
                        break;
                    }
                case IParameterEnum.ExteriorInteriorStage:
                    {
                        if (nextType == IParameterEnum.InteriorStageDamage)
                        {
                            SetSharedAxisValues(nextControl, false);
                            //todo: this might need to change once we get the mix and match axis binding worked out.
                            currentCrosshairData.Next = new SharedAxisCrosshairData(nextCrosshairData, Axis.X, Axis.Y);
                            nextCrosshairData.Previous = new SharedAxisCrosshairData(currentCrosshairData, Axis.Y, Axis.X);
                        }
                        break;
                    }
                case IParameterEnum.InteriorStageDamage:
                    {
                        if (nextType == IParameterEnum.DamageFrequency)
                        {
                            SetSharedAxisValues(nextControl, true);
                            currentCrosshairData.Next = new SharedAxisCrosshairData(nextCrosshairData, Axis.Y, Axis.Y);
                            //currentChartVM.ModifierGroup.ChildModifiers.Clear();
                            //currentChartVM.ModifierGroup.ChildModifiers.Add(new FdaCrosshairChartModifier(false, false, currentCrosshairData));

                            nextCrosshairData.Previous = new SharedAxisCrosshairData(currentCrosshairData, Axis.Y, Axis.Y);
                            //nextChartVM.ModifierGroup.ChildModifiers.Add(new FdaCrosshairChartModifier(true, true, nextCrosshairData));

                        }
                        break;
                    }
            }

        }

        public void UpdateHorizontalLateralStructure(object sender, ModulatorEventArgs args)
        {
            //x is exterior stage
            //y is failure probability
            CurrentX = args.X;
            CurrentY = args.Y;
            //try
            //{
            //    CurrentY = IndividualPlotWrapperVM.PlotVM.BaseFunction.F(IOrdinateFactory.Factory(args.X)).Value();
            //}
            //catch (Exception e)
            //{
            //    //we are out of range. Just return?
            //    return;
            //}

            //this will tell the view side to draw the modulator lines
            //based on the current x and y values.
            UpdateHorizontalFailureFunction?.Invoke(this, new EventArgs());
        }

        public void UpdateHorizontalDoubleLineModulator(object sender, ModulatorEventArgs args)
        {
            CurrentX = args.X;
            try
            {
                CurrentY = IndividualPlotWrapperVM.PlotVM.BaseFunction.F(IOrdinateFactory.Factory(args.X)).Value();
            }
            catch (Exception e)
            {
                //we are out of range. Just return?
                return;
            }

            //this will tell the view side to draw the modulator lines
            //based on the current x and y values.
            UpdateHorizontalDLMLines?.Invoke(this, new EventArgs());
        }

        public void UpdateDoubleLineModulator(object sender, ModulatorEventArgs args)
        {
            //the y value we want to keep. The x value we need to calculate.
            CurrentX = args.Y;
            try
            {
                CurrentY = IndividualPlotWrapperVM.PlotVM.BaseFunction.F(IOrdinateFactory.Factory(args.Y)).Value();
            }
            catch(Exception e)
            {
                //we are out of range. Just return?
                return;
            }

            //this will tell the view side to draw the modulator lines
            //based on the current x and y values.
            UpdateDLMLines?.Invoke(this, new EventArgs());
        }
        #endregion
        #region Functions
        #endregion
    }
}
