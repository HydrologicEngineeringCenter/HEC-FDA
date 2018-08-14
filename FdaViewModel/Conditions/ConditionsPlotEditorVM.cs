using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using FdaModel.ComputationPoint;
using FdaModel.Functions.OrdinatesFunctions;
using System.Collections.ObjectModel;

namespace FdaViewModel.Conditions
{
    //[Author(q0heccdm, 12 / 1 / 2017 11:23:15 AM)]
    public class ConditionsPlotEditorVM : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/1/2017 11:23:15 AM
        #endregion
        #region Fields
        //public delegate void MyEventHandler(object sender, bool stuff);
        
        //public event MyEventHandler OpenImporterInWindow;

        

        private ImpactArea.ImpactAreaOwnerElement _ImpactAreaOwner;

        private bool _IsPlot0Visible = false;
        private bool _IsPlot1Visible = false;
        private bool _IsPlot3Visible = false;
        private bool _IsPlot5Visible = false;
        private bool _IsPlot7Visible = false;
        private bool _IsPlot8Visible = false;


        private Plots.IndividualLinkedPlotVM _Plot0VM;
        private Plots.IndividualLinkedPlotVM _Plot1VM;
        private Plots.IndividualLinkedPlotVM _Plot3VM;
        private Plots.IndividualLinkedPlotVM _Plot5VM;
        private Plots.IndividualLinkedPlotVM _Plot7VM;
        private Plots.IndividualLinkedPlotVM _Plot8VM;

        private Plots.IndividualLinkedPlotControlVM _Plot0ControlVM;
        //private Plots.IndividualLinkedPlotControlVM _DLMControlVM;
        private Plots.IndividualLinkedPlotControlVM _Plot1ControlVM;
        private Plots.IndividualLinkedPlotControlVM _Plot3ControlVM;
        private Plots.IndividualLinkedPlotControlVM _Plot5ControlVM;
        private Plots.IndividualLinkedPlotControlVM _Plot7ControlVM;



        #endregion
        #region Properties
        public List<Plots.IndividualLinkedPlotControlVM> ListOfLinkedPlots { get; set; }
        #region ArePlotsVisible
        public bool IsPlot0Visible
        {
            get { return _IsPlot0Visible; }
            set { _IsPlot0Visible = value; NotifyPropertyChanged(); }
        }
        public bool IsPlot1Visible
        {
            get { return _IsPlot1Visible; }
            set { _IsPlot1Visible = value; NotifyPropertyChanged(); }
        }
        public bool IsPlot3Visible
        {
            get { return _IsPlot3Visible; }
            set { _IsPlot3Visible = value; NotifyPropertyChanged(); }
        }
        public bool IsPlot5Visible
        {
            get { return _IsPlot5Visible; }
            set { _IsPlot5Visible = value; NotifyPropertyChanged(); }
        }
        public bool IsPlot7Visible
        {
            get { return _IsPlot7Visible; }
            set { _IsPlot7Visible = value; NotifyPropertyChanged(); }
        }
        public bool IsPlot8Visible
        {
            get { return _IsPlot8Visible; }
            set { _IsPlot8Visible = value; NotifyPropertyChanged(); }
        }

        #endregion

        #region IndividualPlotVM's
        public Plots.IndividualLinkedPlotControlVM Plot0ControlVM
        {
            get { return _Plot0ControlVM; }
            set { _Plot0ControlVM = value; NotifyPropertyChanged(); }
        }

        //public Plots.IndividualLinkedPlotControlVM DLMControlVM
        //{
        //    get { return _DLMControlVM; }
        //    set { _DLMControlVM = value; NotifyPropertyChanged(); }
        //}
        public Plots.IndividualLinkedPlotControlVM Plot1ControlVM
        {
            get { return _Plot1ControlVM; }
            set { _Plot1ControlVM = value; NotifyPropertyChanged(); }
        }
        public Plots.IndividualLinkedPlotControlVM Plot3ControlVM
        {
            get { return _Plot3ControlVM; }
            set { _Plot3ControlVM = value; NotifyPropertyChanged(); }
        }
        public Plots.IndividualLinkedPlotControlVM Plot5ControlVM
        {
            get { return _Plot5ControlVM; }
            set { _Plot5ControlVM = value; NotifyPropertyChanged(); }
        }
        public Plots.IndividualLinkedPlotControlVM Plot7ControlVM
        {
            get { return _Plot7ControlVM; }
            set { _Plot7ControlVM = value; NotifyPropertyChanged(); }
        }



        public Plots.IndividualLinkedPlotVM Plot0VM
        {
            get { return _Plot0VM; }
            set { _Plot0VM = value; NotifyPropertyChanged(); }
        }
        public Plots.IndividualLinkedPlotVM Plot1VM
        {
            get { return _Plot1VM; }
            set { _Plot1VM = value; NotifyPropertyChanged(); }
        }
        public Plots.IndividualLinkedPlotVM Plot3VM
        {
            get { return _Plot3VM; }
            set { _Plot3VM = value; NotifyPropertyChanged(); }
        }
        public Plots.IndividualLinkedPlotVM Plot5VM
        {
            get { return _Plot5VM; }
            set { _Plot5VM = value; NotifyPropertyChanged(); }
        }
        public Plots.IndividualLinkedPlotVM Plot7VM
        {
            get { return _Plot7VM; }
            set { _Plot7VM = value; NotifyPropertyChanged(); }
        }
        public Plots.IndividualLinkedPlotVM Plot8VM
        {
            get { return _Plot8VM; }
            set { _Plot8VM = value; NotifyPropertyChanged(); }
        }
        #endregion

        public string Description { get;
            set; }
        public string Name { get;
            set; }
        public int Year { get;
            set; }
        public AddFlowFrequencyToConditionVM FlowFrequencyVM { get; set; }
        public AddInflowOutflowToConditionVM InflowOutflowVM { get; set; }
        public AddRatingCurveToConditionVM RatingCurveVM { get; set; }
        public AddExteriorInteriorStageToConditionVM ExteriorInteriorVM { get; set; }
        public AddStageDamageToConditionVM StageDamageVM { get; set; }

        public List<ImpactArea.ImpactAreaElement> ImpactAreas { get; set; }
        public ImpactArea.ImpactAreaElement SelectedImpactArea { get;
            set; }
        public List<Inventory.InventoryElement> StructureInventories { get; set; }

        #endregion
        #region Constructors
        public ConditionsPlotEditorVM(List<ImpactArea.ImpactAreaElement> impAreas,List<Inventory.InventoryElement> structInventories, AddFlowFrequencyToConditionVM lp3vm, AddInflowOutflowToConditionVM inOutVM, AddRatingCurveToConditionVM ratingCurveVM, AddStageDamageToConditionVM stageDamageVM, AddExteriorInteriorStageToConditionVM extIntStageVM, ImpactArea.ImpactAreaOwnerElement impactAreaOwnerElement)
        {
            _ImpactAreaOwner = impactAreaOwnerElement;
            ImpactAreas = impAreas;
            StructureInventories = structInventories;
            FlowFrequencyVM = lp3vm;
            InflowOutflowVM = inOutVM;
            RatingCurveVM = ratingCurveVM;
            ExteriorInteriorVM = extIntStageVM;
            StageDamageVM = stageDamageVM;
            ListOfLinkedPlots = new List<Plots.IndividualLinkedPlotControlVM>();
            
        }

        public ConditionsPlotEditorVM(Plots.IndividualLinkedPlotControlVM indLinkedPlotControl0VM,  Plots.IndividualLinkedPlotControlVM control1VM, Plots.IndividualLinkedPlotControlVM control3VM, Plots.IndividualLinkedPlotControlVM control5VM, Plots.IndividualLinkedPlotControlVM control7VM)
        {
            Plot0ControlVM = indLinkedPlotControl0VM;
            //start with only plot0 button enabled
            Plot0ControlVM.ImportButtonVM.IsEnabled = true;
            Plot0ControlVM.PlotIsShowing += Plot0IsShowing;
            Plot0ControlVM.PlotIsNotShowing += Plot0IsNotShowing;

           // DLMControlVM = DLMcontrol;

            Plot1ControlVM = control1VM;
            //Plot1ControlVM.PlotIsShowing += Plot1IsShowing;
            //Plot1ControlVM.PlotIsNotShowing += Plot1IsNotShowing;

            Plot3ControlVM = control3VM;
            Plot3ControlVM.PlotIsShowing += Plot3IsShowing;
            Plot3ControlVM.PlotIsNotShowing += Plot3IsNotShowing;

            Plot5ControlVM = control5VM;

            Plot7ControlVM = control7VM;




            _Plot0ControlVM.RequestNavigation += Navigate;
            _Plot1ControlVM.RequestNavigation += Navigate;
            _Plot3ControlVM.RequestNavigation += Navigate;
            _Plot5ControlVM.RequestNavigation += Navigate;
            _Plot7ControlVM.RequestNavigation += Navigate;

        }

      

        #endregion
        #region Voids
       
        private void Plot3IsShowing(object sender, EventArgs e)
        {
            Plot7ControlVM.ImportButtonVM.IsEnabled = true;
            Plot5ControlVM.ImportButtonVM.IsEnabled = true;
            if(Plot5ControlVM.ModulatorCoverButtonVM != null)
            {
                Plot5ControlVM.ModulatorCoverButtonVM.IsEnabled = true;
            }

        }
        private void Plot3IsNotShowing(object sender, EventArgs e)
        {
            Plot7ControlVM.ImportButtonVM.IsEnabled = false;

        }

        //private void Plot1IsShowing(object sender, EventArgs e)
        //{
        //    //Plot7ControlVM.ImportButtonVM.IsEnabled = true;

        //}
        //private void Plot1IsNotShowing(object sender, EventArgs e)
        //{
        //    //Plot7ControlVM.ImportButtonVM.IsEnabled = false;

        //}
        private void Plot0IsShowing(object sender, EventArgs e)
        {
            //turn plot 1 and 3 on
            if (Plot1ControlVM.ModulatorCoverButtonVM != null)
            {
                Plot1ControlVM.ModulatorCoverButtonVM.IsEnabled = true;
            }
           
                Plot1ControlVM.ImportButtonVM.IsEnabled = true;
            
                //DLMControlVM.ImportButtonVM.IsEnabled = true;
            Plot3ControlVM.ImportButtonVM.IsEnabled = true;

        }
        private void Plot0IsNotShowing(object sender, EventArgs e)
        {
            //figure out what to do here. It will depend on if plots already exist there or not.
            Plot3ControlVM.ImportButtonVM.IsEnabled = false;
        }
        public void LaunchNewImpactArea(object sender, EventArgs e)
        {
            if (_ImpactAreaOwner != null)
            {
                List<ImpactArea.ImpactAreaOwnerElement> eles = _ImpactAreaOwner.GetElementsOfType<ImpactArea.ImpactAreaOwnerElement>();
                if (eles.Count > 0)
                {
                    eles.FirstOrDefault().AddNew(sender, e);
                    //need to determine what the most recent element is and see if we already have it.
                    if (eles.FirstOrDefault().Elements.Count > 0)
                    {
                        if (eles.FirstOrDefault().Elements.Count > ImpactAreas.Count)
                        {
                            List<ImpactArea.ImpactAreaElement> theNewList = new List<ImpactArea.ImpactAreaElement>();
                            for (int i = 0; i < eles.FirstOrDefault().Elements.Count; i++)
                            {
                                theNewList.Add((ImpactArea.ImpactAreaElement)eles.FirstOrDefault().Elements[i]);
                            }
                            ImpactAreas = theNewList;
                            //AnalyiticalRelationships.Add((FrequencyRelationships.AnalyticalFrequencyElement)eles.FirstOrDefault().Elements.Last());
                            SelectedImpactArea = ImpactAreas.Last();
                        }
                    }
                }

            }

        }
        public void LaunchAddInflowFrequencyCurve(object o,EventArgs e)
        {
            Navigate(FlowFrequencyVM);
            if (!FlowFrequencyVM.WasCancled)
            {
                if (!FlowFrequencyVM.HasError)
                {
                    if(FlowFrequencyVM.SelectedFlowFrequencyElement != null)
                    {
                        FdaModel.Functions.FrequencyFunctions.LogPearsonIII lp3 = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(FlowFrequencyVM.SelectedFlowFrequencyElement.Distribution,FdaModel.Functions.FunctionTypes.InflowFrequency);

                        Plot0VM = new Plots.IndividualLinkedPlotVM( lp3,lp3.GetOrdinatesFunction().Function, "LP3", "Probability", "Inflow", FlowFrequencyVM.SelectedFlowFrequencyElement.Name);
                        if (Plot0VM.Curve.Count < 1)
                        {
                            Utilities.CustomMessageBoxVM message = new Utilities.CustomMessageBoxVM(Utilities.CustomMessageBoxVM.ButtonsEnum.OK, "The selected curve contains zero points.");
                            Navigate(message);
                        }
                        else
                        {
                            //IsPlot0Visible = true;
                            Plot0VM.IsVisible = true;
                        }
                    }   
                }
            }
        }

       
        public void LaunchAddRatingCurve()
        {
            Navigate(RatingCurveVM);
            if (!RatingCurveVM.WasCancled)
            {
                if (!RatingCurveVM.HasError)
                {
                    if (RatingCurveVM.SelectedRatingElement != null)
                    {
                        FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction rating = new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((Statistics.UncertainCurveIncreasing)RatingCurveVM.SelectedRatingElement.RatingCurve, FdaModel.Functions.FunctionTypes.Rating);

                        List<double> ys = new List<double>();
                        List<double> xs = new List<double>();
                        foreach (double y in (rating.GetOrdinatesFunction().Function.YValues))
                        {
                            ys.Add(y);
                        }
                        foreach (double x in (rating.GetOrdinatesFunction().Function.XValues))
                        {
                            xs.Add(x);
                        }
                        Plot3VM = new Plots.IndividualLinkedPlotVM( rating,new Statistics.CurveIncreasing(ys.ToArray(), xs.ToArray(), true, false), "Rating", "Exterior Stage (ft)", "Outflow (cfs)",RatingCurveVM.SelectedRatingElement.Name);
                        if (Plot3VM.Curve.Count < 1)
                        {
                            Utilities.CustomMessageBoxVM message = new Utilities.CustomMessageBoxVM(Utilities.CustomMessageBoxVM.ButtonsEnum.OK, "The selected curve contains zero points.");
                            Navigate(message);
                        }
                        else
                        {
                            IsPlot3Visible = true;
                        }
                      
                    }
                }
            }
        }

        public void LaunchAddInflowOutflowCurve()
        {
            Navigate(InflowOutflowVM);
            if (!InflowOutflowVM.WasCancled)
            {
                if (!InflowOutflowVM.HasError)
                {
                    if (InflowOutflowVM.SelectedInflowOutflowElement != null)
                    {
                        FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction inflowOutflow = new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((Statistics.UncertainCurveIncreasing)InflowOutflowVM.SelectedInflowOutflowElement.InflowOutflowCurve, FdaModel.Functions.FunctionTypes.InflowOutflow);
                        
                        Plot1VM = new Plots.IndividualLinkedPlotVM(inflowOutflow, inflowOutflow.GetOrdinatesFunction().Function, "InflowOutflow", "Inflow (cfs)", "Outflow (cfs)", InflowOutflowVM.SelectedInflowOutflowElement.Name);
                        if (Plot1VM.Curve.Count < 1)
                        {
                            Utilities.CustomMessageBoxVM message = new Utilities.CustomMessageBoxVM(Utilities.CustomMessageBoxVM.ButtonsEnum.OK, "The selected curve contains zero points.");
                            Navigate(message);
                        }
                        else
                        {
                            IsPlot1Visible = true;
                        }
                        
                        
                    }
                }
            }
        }

        public void LaunchAddStageDamageCurve()
        {
            Navigate(StageDamageVM);
            if (!StageDamageVM.WasCancled)
            {
                if (!StageDamageVM.HasError)
                {
                    if (StageDamageVM.StageDamageElement != null)
                    {
                        FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction stageDamage = new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((Statistics.UncertainCurveIncreasing)StageDamageVM.StageDamageElement.Curve, FdaModel.Functions.FunctionTypes.InteriorStageDamage);

                        Plot7VM = new Plots.IndividualLinkedPlotVM( stageDamage,stageDamage.GetOrdinatesFunction().Function, "StageDamage", "Stage (ft)", "Damage ($)",StageDamageVM.StageDamageElement.Name);

                        if (Plot7VM.Curve.Count < 1)
                        {
                            Utilities.CustomMessageBoxVM message = new Utilities.CustomMessageBoxVM(Utilities.CustomMessageBoxVM.ButtonsEnum.OK, "The selected curve contains zero points.");
                            Navigate(message);
                        }
                        else
                        {
                            IsPlot7Visible = true;
                        }
                       
                    }
                }
            }

        }

        public void LaunchAddExteriorInteriorCurve()
        {
            Navigate(ExteriorInteriorVM);
            if (!ExteriorInteriorVM.WasCancled)
            {
                if (!ExteriorInteriorVM.HasError)
                {
                    if (ExteriorInteriorVM.SelectedExteriorInteriorStageElement != null)
                    {
                        FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction extIntStage = new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((Statistics.UncertainCurveIncreasing)ExteriorInteriorVM.SelectedExteriorInteriorStageElement.ExteriorInteriorCurve, FdaModel.Functions.FunctionTypes.ExteriorInteriorStage);

                        Plot5VM = new Plots.IndividualLinkedPlotVM(extIntStage, extIntStage.GetOrdinatesFunction().Function, "ExteriorInteriorStage", "Exterior Stage (ft)", "Interior Stage (ft)",ExteriorInteriorVM.SelectedExteriorInteriorStageElement.Name);
                        if (Plot5VM.Curve.Count < 1)
                        {
                            Utilities.CustomMessageBoxVM message = new Utilities.CustomMessageBoxVM(Utilities.CustomMessageBoxVM.ButtonsEnum.OK, "The selected curve contains zero points.");
                            Navigate(message);
                        }
                        else
                        {
                            IsPlot5Visible = true;
                        }
                        
                    }
                }
            }

        }

        public void RunPreviewCompute()
        {
            
            //get the threshold values
            PerformanceThreshold threshold = new PerformanceThreshold(PerformanceThresholdTypes.InteriorStage, 8);


            //get the selected impact area

            //get the selected struct inv

            //get all the selected curves

            List<FdaModel.Functions.BaseFunction> myListOfBaseFunctions = new List<FdaModel.Functions.BaseFunction>();

            if(IsPlot0Visible == true)
            {
                FdaModel.Functions.FrequencyFunctions.LogPearsonIII zero = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(FlowFrequencyVM.SelectedFlowFrequencyElement.Distribution, FdaModel.Functions.FunctionTypes.InflowFrequency);
                myListOfBaseFunctions.Add(zero);
            }
            if (IsPlot1Visible == true)
            {
                FdaModel.Functions.BaseFunction one = new OrdinatesFunction(Plot1VM.Curve, FdaModel.Functions.FunctionTypes.InflowOutflow);
                myListOfBaseFunctions.Add(one);
            }
            if(IsPlot3Visible ==true)
            {
                //i have to flip the x and y values back before computing
                ReadOnlyCollection<double> xs = Plot3VM.Curve.YValues;
                ReadOnlyCollection<double> ys = Plot3VM.Curve.XValues;
                //FdaModel.Functions.BaseFunction temp = new OrdinatesFunction();
                FdaModel.Functions.BaseFunction three = new OrdinatesFunction(xs,ys, FdaModel.Functions.FunctionTypes.Rating);
                myListOfBaseFunctions.Add(three);
            }
            if (IsPlot5Visible == true)
            {
                FdaModel.Functions.BaseFunction five = new OrdinatesFunction(Plot5VM.Curve, FdaModel.Functions.FunctionTypes.ExteriorInteriorStage);
                myListOfBaseFunctions.Add(five);
            }
            if (IsPlot7Visible == true)
            {
                FdaModel.Functions.BaseFunction seven = new OrdinatesFunction(Plot7VM.Curve, FdaModel.Functions.FunctionTypes.InteriorStageDamage);
                myListOfBaseFunctions.Add(seven);
            }


            //create lateral structure
            LateralStructure myLateralStruct = new LateralStructure(10);

            //create the condition
            Condition simpleTest = new Condition(2008, Name, myListOfBaseFunctions, threshold, myLateralStruct); //bool call Validate

            //create random number gen
            Random randomNumberGenerator = new Random(0);

            //create the realization
            FdaModel.ComputationPoint.Outputs.Realization simpleTestResult = new FdaModel.ComputationPoint.Outputs.Realization(simpleTest, false, false); //bool oldCompute, bool performance only

            //compute
            simpleTestResult.Compute(randomNumberGenerator);

            //if it was successful, plot number 8. if not then message why not
            
            foreach(FdaModel.Functions.BaseFunction bf in simpleTestResult.Functions)
            {
                if(bf.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
                {
                    Plot8VM = new Plots.IndividualLinkedPlotVM(bf, bf.GetOrdinatesFunction().Function, "Damage Frequency", "Frequency", "Damage ($)");
                    IsPlot8Visible = true;
                    

                }
            }
            if(IsPlot8Visible == false)
            {
                StringBuilder messages = new StringBuilder();
                foreach(FdaModel.Utilities.Messager.ErrorMessage em in simpleTestResult.Messages.Messages)
                {
                    messages.AppendLine(em.Message);
                }
                Utilities.CustomMessageBoxVM custmb = new Utilities.CustomMessageBoxVM(Utilities.CustomMessageBoxVM.ButtonsEnum.OK, messages.ToString());
                Navigate(custmb);
            }

        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }

      
        #endregion
        #region Functions
        #endregion
    }
}
