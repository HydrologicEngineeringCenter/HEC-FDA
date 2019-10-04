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
using System.Windows;
using FdaViewModel.ImpactArea;

namespace FdaViewModel.Conditions
{
    //[Author(q0heccdm, 12 / 1 / 2017 11:23:15 AM)]
    public class ConditionsPlotEditorVM : Editors.BaseEditorVM
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/1/2017 11:23:15 AM
        #endregion
        #region Fields
        //public delegate void MyEventHandler(object sender, bool stuff);

        //public event MyEventHandler OpenImporterInWindow;



        private ImpactArea.ImpactAreaOwnerElement _ImpactAreaOwner;

        //private bool _IsPlot0Visible = false;
        //private bool _IsPlot1Visible = false;
        //private bool _IsPlot3Visible = false;
        //private bool _IsPlot5Visible = false;
        //private bool _IsPlot7Visible = false;
        //private bool _IsPlot8Visible = false;


        //private Plots.IndividualLinkedPlotVM _Plot0VM;
        //private Plots.IndividualLinkedPlotVM _Plot1VM;
        //private Plots.IndividualLinkedPlotVM _Plot3VM;
        //private Plots.IndividualLinkedPlotVM _Plot5VM;
        //private Plots.IndividualLinkedPlotVM _Plot7VM;
        //private Plots.IndividualLinkedPlotVM _Plot8VM;

        private Plots.IndividualLinkedPlotControlVM _Plot0ControlVM;
        private Plots.IndividualLinkedPlotControlVM _Plot1ControlVM;
        private Plots.IndividualLinkedPlotControlVM _Plot3ControlVM;
        private Plots.IndividualLinkedPlotControlVM _Plot5ControlVM;
        private Plots.IndividualLinkedPlotControlVM _Plot7ControlVM;
        private Plots.IndividualLinkedPlotControlVM _Plot8ControlVM;


        private ObservableCollection<Plots.IndividualLinkedPlotControlVM> _AddedPlots = new ObservableCollection<Plots.IndividualLinkedPlotControlVM>();
        private ObservableCollection<Plots.IndividualLinkedPlotVM> _AvailablePlots = new ObservableCollection<Plots.IndividualLinkedPlotVM>();

        private string _Name;
        private int _Year;
        private ImpactArea.ImpactAreaElement _SelectedImpactArea;

        private ObservableCollection<PerformanceThresholdTypes> _ThresholdTypes = new ObservableCollection<PerformanceThresholdTypes>();
        private PerformanceThresholdTypes _SelectedThresholdType;
        private bool _ThresholdLinesAllowedToShow = true;

        #endregion
        #region Properties
        //public List<Plots.IndividualLinkedPlotControlVM> ListOfLinkedPlots { get; set; }

        public double ThresholdValue { get;
            set; }
        public ObservableCollection<PerformanceThresholdTypes> ThresholdTypes
        {
            get { return _ThresholdTypes; }
        }
        public PerformanceThresholdTypes SelectedThresholdType
        {
            get { return _SelectedThresholdType; }
            set { _SelectedThresholdType = value; PlotThresholdLine(ThresholdValue); }
        }
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<Plots.IndividualLinkedPlotControlVM> AddedPlots
        {
            get { return _AddedPlots; }
            set { _AddedPlots = value; NotifyPropertyChanged(); }
        }


        public ImpactArea.ImpactAreaRowItem IndexLocation { get; set; }

        #region ArePlotsVisible
        //public bool IsPlot0Visible
        //{
        //    get { return _IsPlot0Visible; }
        //    set { _IsPlot0Visible = value; NotifyPropertyChanged(); }
        //}
        //public bool IsPlot1Visible
        //{
        //    get { return _IsPlot1Visible; }
        //    set { _IsPlot1Visible = value; NotifyPropertyChanged(); }
        //}
        //public bool IsPlot3Visible
        //{
        //    get { return _IsPlot3Visible; }
        //    set { _IsPlot3Visible = value; NotifyPropertyChanged(); }
        //}
        //public bool IsPlot5Visible
        //{
        //    get { return _IsPlot5Visible; }
        //    set { _IsPlot5Visible = value; NotifyPropertyChanged(); }
        //}
        //public bool IsPlot7Visible
        //{
        //    get { return _IsPlot7Visible; }
        //    set { _IsPlot7Visible = value; NotifyPropertyChanged(); }
        //}
        //public bool IsPlot8Visible
        //{
        //    get { return _IsPlot8Visible; }
        //    set { _IsPlot8Visible = value; NotifyPropertyChanged(); }
        //}

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

        public Plots.IndividualLinkedPlotControlVM Plot8ControlVM
        {
            get { return _Plot8ControlVM; }
            set { _Plot8ControlVM = value; NotifyPropertyChanged(); }
        }

        //public Plots.IndividualLinkedPlotVM Plot0VM
        //{
        //    get { return _Plot0VM; }
        //    set { _Plot0VM = value; NotifyPropertyChanged(); }
        //}
        //public Plots.IndividualLinkedPlotVM Plot1VM
        //{
        //    get { return _Plot1VM; }
        //    set { _Plot1VM = value; NotifyPropertyChanged(); }
        //}
        //public Plots.IndividualLinkedPlotVM Plot3VM
        //{
        //    get { return _Plot3VM; }
        //    set { _Plot3VM = value; NotifyPropertyChanged(); }
        //}
        //public Plots.IndividualLinkedPlotVM Plot5VM
        //{
        //    get { return _Plot5VM; }
        //    set { _Plot5VM = value; NotifyPropertyChanged(); }
        //}
        //public Plots.IndividualLinkedPlotVM Plot7VM
        //{
        //    get { return _Plot7VM; }
        //    set { _Plot7VM = value; NotifyPropertyChanged(); }
        //}
        //public Plots.IndividualLinkedPlotVM Plot8VM
        //{
        //    get { return _Plot8VM; }
        //    set { _Plot8VM = value; NotifyPropertyChanged(); }
        //}
        #endregion

            public ObservableCollection<Plots.IndividualLinkedPlotVM> AvailablePlots
        {
            get { return _AvailablePlots; }
            set { _AvailablePlots = value;NotifyPropertyChanged(); }
        }

        public string Description { get;
            set; }
           
        public int Year
        {
            get { return _Year; }
            set { _Year = value; NotifyPropertyChanged(); }
        }

        //public AddFlowFrequencyToConditionVM FlowFrequencyVM { get; set; }
        //public AddInflowOutflowToConditionVM InflowOutflowVM { get; set; }
        //public AddRatingCurveToConditionVM RatingCurveVM { get; set; }
        //public AddExteriorInteriorStageToConditionVM ExteriorInteriorVM { get; set; }
        //public AddStageDamageToConditionVM StageDamageVM { get; set; }
        private List<ImpactArea.ImpactAreaElement> _ImpactAreas;
        public List<ImpactArea.ImpactAreaElement> ImpactAreas
        {
            get { return _ImpactAreas; }
            set { _ImpactAreas = value;NotifyPropertyChanged(); }
            
        }
        public ImpactArea.ImpactAreaElement SelectedImpactArea
        {
            get { return _SelectedImpactArea; }
            set { _SelectedImpactArea = value; NotifyPropertyChanged(); }
        }
        public List<Inventory.InventoryElement> StructureInventories { get; set; }

        #endregion
        #region Constructors
        //public ConditionsPlotEditorVM(List<ImpactArea.ImpactAreaElement> impAreas,List<Inventory.InventoryElement> structInventories, AddFlowFrequencyToConditionVM lp3vm, AddInflowOutflowToConditionVM inOutVM, AddRatingCurveToConditionVM ratingCurveVM, AddStageDamageToConditionVM stageDamageVM, AddExteriorInteriorStageToConditionVM extIntStageVM, ImpactArea.ImpactAreaOwnerElement impactAreaOwnerElement)
        //{
        //    _ImpactAreaOwner = impactAreaOwnerElement;
        //    ImpactAreas = impAreas;
        //    StructureInventories = structInventories;
        //    FlowFrequencyVM = lp3vm;
        //    InflowOutflowVM = inOutVM;
        //    RatingCurveVM = ratingCurveVM;
        //    ExteriorInteriorVM = extIntStageVM;
        //    StageDamageVM = stageDamageVM;
        //    ListOfLinkedPlots = new List<Plots.IndividualLinkedPlotControlVM>();

        //}


            /// <summary>
            /// This constructor is used when opening the editor up for editing a previously defined condition
            /// </summary>
            /// <param name="impAreas"></param>
            /// <param name="indLinkedPlotControl0VM"></param>
            /// <param name="control1VM"></param>
            /// <param name="control3VM"></param>
            /// <param name="control5VM"></param>
            /// <param name="control7VM"></param>
            /// <param name="control8VM"></param>
            /// <param name="name"></param>
            /// <param name="description"></param>
            /// <param name="year"></param>
            /// <param name="selectedImpArea"></param>
        public ConditionsPlotEditorVM(List<ImpactArea.ImpactAreaElement> impAreas, Plots.IndividualLinkedPlotControlVM indLinkedPlotControl0VM, Plots.IndividualLinkedPlotControlVM control1VM,
            Plots.IndividualLinkedPlotControlVM control3VM, Plots.IndividualLinkedPlotControlVM control5VM, Plots.IndividualLinkedPlotControlVM control7VM, Plots.IndividualLinkedPlotControlVM control8VM,
           ConditionsElement element, Editors.EditorActionManager actionManager) : base(element,actionManager)
        {
            ImpactAreas = impAreas;

            Plot0ControlVM = indLinkedPlotControl0VM;
            //start with only plot0 button enabled
            Plot0ControlVM.ImportButtonVM.IsEnabled = true;

            Plot1ControlVM = control1VM;
            Plot3ControlVM = control3VM;
            Plot5ControlVM = control5VM;
            Plot7ControlVM = control7VM;
            Plot8ControlVM = control8VM;

            AttachEventsToControls();
            LoadThresholdTypes();
            Name = element.Name;
            Description = element.Description;
            Year = element.AnalysisYear;
            SelectedImpactArea = element.ImpactAreaElement;
            SelectedThresholdType = element.ThresholdType;
            ThresholdValue = element.ThresholdValue;

            if (Plot0ControlVM.CurveImporterVM != null && Plot0ControlVM.CurveImporterVM.SelectedElement != null)//then we are opening an existing node
            {
                Plot0ControlVM.AddCurveToPlot(this, new EventArgs());
            }
            if (Plot1ControlVM.CurveImporterVM != null && Plot1ControlVM.CurveImporterVM.SelectedElement != null)//then we are opening an existing node
            {
                Plot1ControlVM.AddCurveToPlot(this, new EventArgs());
                Plot1ControlVM.CurrentVM = (BaseViewModel)Plot1ControlVM.ModulatorPlotWrapperVM;
            }
            if (Plot3ControlVM.CurveImporterVM != null && Plot3ControlVM.CurveImporterVM.SelectedElement != null)//then we are opening an existing node
            {
                Plot3ControlVM.AddCurveToPlot(this, new EventArgs());
            }
            if (Plot5ControlVM.CurveImporterVM != null && Plot5ControlVM.CurveImporterVM.SelectedElement != null)//then we are opening an existing node
            {
                Plot5ControlVM.AddCurveToPlot(this, new EventArgs());
                Plot5ControlVM.CurrentVM = (BaseViewModel)Plot5ControlVM.ModulatorPlotWrapperVM;
            }
            if (Plot7ControlVM.CurveImporterVM != null && Plot7ControlVM.CurveImporterVM.SelectedElement != null)//then we are opening an existing node
            {
                Plot7ControlVM.AddCurveToPlot(this, new EventArgs());
            }


            AddEvents();


        }
        /// <summary>
        /// This constructor is used when creating a new condition
        /// </summary>
        /// <param name="impAreas"></param>
        /// <param name="defaultControl0VM"></param>
        /// <param name="defaultControl1VM"></param>
        /// <param name="defaultControl3VM"></param>
        /// <param name="defaultControl5VM"></param>
        /// <param name="DefaultControl7VM"></param>
        /// <param name="DefaultControl8VM"></param>
        public ConditionsPlotEditorVM(List<ImpactArea.ImpactAreaElement> impAreas, Plots.IndividualLinkedPlotControlVM defaultControl0VM, Plots.IndividualLinkedPlotControlVM defaultControl1VM, 
            Plots.IndividualLinkedPlotControlVM defaultControl3VM, Plots.IndividualLinkedPlotControlVM defaultControl5VM, Plots.IndividualLinkedPlotControlVM DefaultControl7VM, Plots.IndividualLinkedPlotControlVM DefaultControl8VM, Editors.EditorActionManager actionManager) :base(actionManager)
        {
            //ownerValidationRules(this);
            
            ImpactAreas = impAreas;

            Plot0ControlVM = defaultControl0VM;
            //start with only plot0 button enabled
            Plot0ControlVM.ImportButtonVM.IsEnabled = true;

            Plot1ControlVM = defaultControl1VM;       
            Plot3ControlVM = defaultControl3VM;
            Plot5ControlVM = defaultControl5VM;
            Plot7ControlVM = DefaultControl7VM;
            Plot8ControlVM = DefaultControl8VM;

            AttachEventsToControls();
            LoadThresholdTypes();
            AddEvents();
        }

        private void AddEvents()
        {
            StudyCache.ImpactAreaAdded += ImpactAreaAdded;
        }

        private void ImpactAreaAdded(object sender, Saving.ElementAddedEventArgs e)
        {
            List<ImpactArea.ImpactAreaElement> tempList = new List<ImpactArea.ImpactAreaElement>();
            foreach(ImpactAreaElement elem in ImpactAreas)
            {
                tempList.Add(elem);
            }
            tempList.Add((ImpactArea.ImpactAreaElement)e.Element);
            ImpactAreas = tempList;//this is to hit the notify prop changed
        }



        #endregion
        #region Voids

        public void ToggleThresholdLines()
        {
            if (_ThresholdLinesAllowedToShow)
            {
                _ThresholdLinesAllowedToShow = false;
                Plot7ControlVM.IndividualPlotWrapperVM.Threshold = null;//this is basically a flag that the callback uses to turn them off
                Plot8ControlVM.IndividualPlotWrapperVM.Threshold = new PerformanceThreshold(new LateralStructure(0));//this is just do change it from null to a value so that i can turn it back to null
                Plot8ControlVM.IndividualPlotWrapperVM.Threshold = null;
            }
            else
            {
                //turn them on
                _ThresholdLinesAllowedToShow = true ;
                PerformanceThreshold pt = new PerformanceThreshold(SelectedThresholdType, ThresholdValue);
                Plot7ControlVM.IndividualPlotWrapperVM.Threshold = pt;//this is basically a flag that the callback uses to turn them off
                //Plot8ControlVM.IndividualPlotWrapperVM.Threshold = pt;
            }
        }
        public void PlotThresholdLine(double thresholdValue)
        {
            if(_ThresholdLinesAllowedToShow == false) { return; }
            //I can't use the Threshold property that exists in this class
            //because it hasn't changed yet, it is an ordering issue, so i just pass it in.
            PerformanceThreshold pt = new PerformanceThreshold(SelectedThresholdType, thresholdValue);
            if(SelectedThresholdType == PerformanceThresholdTypes.InteriorStage)
            {
                Plot7ControlVM.IndividualPlotWrapperVM.Threshold = pt;//this will trigger the callback in the view side

            }
            else if (SelectedThresholdType == PerformanceThresholdTypes.Damage)
            {
                Plot7ControlVM.IndividualPlotWrapperVM.Threshold = pt;
            }
            //if(Plot8ControlVM.CurrentVM.GetType() == typeof(Plots.ConditionsIndividualPlotWrapperVM))
            //{
            //    Plot8ControlVM.IndividualPlotWrapperVM.Threshold = pt;
            //}
        }

        /// <summary>
        /// This updates the available plots collection whos entire purpose is to work with the plot specific point tool
        /// </summary>
        private void UpdateAvailablePlots()
        {
            AvailablePlots.Clear();
            foreach(Plots.IndividualLinkedPlotControlVM plot in AddedPlots)
            {
                AvailablePlots.Add(plot.IndividualPlotWrapperVM.PlotVM);
            }
        }

        private void LoadThresholdTypes()
        {
            foreach (PerformanceThresholdTypes ptt in Enum.GetValues(typeof(PerformanceThresholdTypes)))
            {
                _ThresholdTypes.Add(ptt);
            }
        }
        private void AttachEventsToControls()
        {
            Plot0ControlVM.PlotIsShowing += Plot0IsShowing;
            Plot0ControlVM.PlotIsNotShowing += Plot0IsNotShowing;

            Plot0ControlVM.SelectedCurveUpdated += UpdateSelectedCurves;

            Plot1ControlVM.SelectedCurveUpdated += UpdateSelectedCurves;

            Plot3ControlVM.PlotIsShowing += Plot3IsShowing;
            Plot3ControlVM.PlotIsNotShowing += Plot3IsNotShowing;
            Plot3ControlVM.SelectedCurveUpdated += UpdateSelectedCurves;

            Plot5ControlVM.SelectedCurveUpdated += UpdateSelectedCurves;

            Plot7ControlVM.PlotIsShowing += Plot7IsShowing;
            Plot7ControlVM.PlotIsNotShowing += Plot7IsNotShowing;
            Plot7ControlVM.SelectedCurveUpdated += UpdateSelectedCurves;

            Plot8ControlVM.PreviewCompute += RunPreviewCompute;

            //_Plot0ControlVM.RequestNavigation += Navigate;
            //_Plot1ControlVM.RequestNavigation += Navigate;
            //_Plot3ControlVM.RequestNavigation += Navigate;
            //_Plot5ControlVM.RequestNavigation += Navigate;
            //_Plot7ControlVM.RequestNavigation += Navigate;
            //_Plot8ControlVM.RequestNavigation += Navigate;

            AttachUpdatePreviewPlotEvents();
        }

        private void AttachUpdatePreviewPlotEvents()
        {
            Plot0ControlVM.PlotIsNotShowing += UpdatePreviewComputePlot;
            Plot0ControlVM.PlotIsShowing += UpdatePreviewComputePlot;
            Plot0ControlVM.SelectedCurveUpdated += UpdatePreviewComputePlot;

            Plot1ControlVM.PlotIsNotShowing += UpdatePreviewComputePlot;
            Plot1ControlVM.PlotIsShowing += UpdatePreviewComputePlot;
            Plot1ControlVM.SelectedCurveUpdated += UpdatePreviewComputePlot;

            Plot3ControlVM.PlotIsNotShowing += UpdatePreviewComputePlot;
            Plot3ControlVM.PlotIsShowing += UpdatePreviewComputePlot;
            Plot3ControlVM.SelectedCurveUpdated += UpdatePreviewComputePlot;

            Plot5ControlVM.PlotIsNotShowing += UpdatePreviewComputePlot;
            Plot5ControlVM.PlotIsShowing += UpdatePreviewComputePlot;
            Plot5ControlVM.SelectedCurveUpdated += UpdatePreviewComputePlot;

            Plot7ControlVM.PlotIsNotShowing += UpdatePreviewComputePlot;
            Plot7ControlVM.PlotIsShowing += UpdatePreviewComputePlot;
            Plot7ControlVM.SelectedCurveUpdated += UpdatePreviewComputePlot;
        }
        private void Plot7IsShowing(object sender, EventArgs e)
        {
            Plot8ControlVM.ImportButtonVM.IsEnabled = true;
        }
        private void Plot7IsNotShowing(object sender, EventArgs e)
        {
            Plot8ControlVM.ImportButtonVM.IsEnabled = false;

        }
        private void Plot3IsShowing(object sender, EventArgs e)
        {
            Plot7ControlVM.ImportButtonVM.IsEnabled = true;
            Plot5ControlVM.ImportButtonVM.IsEnabled = true;
            if (Plot5ControlVM.ModulatorCoverButtonVM != null)
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

            //add the selected curve to the list of added curves
            //_AddedPlots.Add(Plot0ControlVM.IndividualPlotWrapperVM.PlotVM);
        }
        private void Plot0IsNotShowing(object sender, EventArgs e)
        {
            //figure out what to do here. It will depend on if plots already exist there or not.
            Plot3ControlVM.ImportButtonVM.IsEnabled = false;
            //_AddedPlots.Remove(Plot0ControlVM.IndividualPlotWrapperVM.PlotVM);

        }

        /// <summary>
        /// Any change to the other curves will make the preview plot go back to the
        /// import button. If the conditions doesn't have what it needs to run
        /// a compute, it will disable the compute button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdatePreviewComputePlot(object sender, EventArgs e)
        {
            Plot8ControlVM.SetCurrentViewToCoverButton();
            //if plot 0,3,7 are showing then enable, else, disable the button
            bool hasPlot0 = false;
            bool hasPlot3 = false;
            bool hasPlot7 = false;
            if(Plot0ControlVM.CurrentVM.GetType() == typeof(Plots.ConditionsIndividualPlotWrapperVM))
            {
                hasPlot0 = true;
            }
            if (Plot3ControlVM.CurrentVM.GetType() == typeof(Plots.ConditionsIndividualPlotWrapperVM))
            {
                hasPlot3 = true;
            }
            if (Plot7ControlVM.CurrentVM.GetType() == typeof(Plots.ConditionsIndividualPlotWrapperVM))
            {
                hasPlot7 = true;
            }

            if (hasPlot0 && hasPlot3 && hasPlot7)
            {
                Plot8ControlVM.ImportButtonVM.IsEnabled = true;
            }
            else
            {
                Plot8ControlVM.ImportButtonVM.IsEnabled = false;
            }
            //create a method to see if the compute would be valid?
            //then... if(condition.isValid){enable the button} else disable the button
        }

        private void UpdateSelectedCurves(object sender, EventArgs e)
        {
            _AddedPlots.Clear();

            if(Plot0ControlVM.IndividualPlotWrapperVM.PlotVM != null && Plot0ControlVM.IndividualPlotWrapperVM.PlotVM.Curve != null)
            {
                _AddedPlots.Add(Plot0ControlVM);
            }
            if (Plot1ControlVM.IndividualPlotWrapperVM.PlotVM != null && Plot1ControlVM.IndividualPlotWrapperVM.PlotVM.Curve != null)
            {
                _AddedPlots.Add(Plot1ControlVM);
            }
            if (Plot3ControlVM.IndividualPlotWrapperVM.PlotVM != null && Plot3ControlVM.IndividualPlotWrapperVM.PlotVM.Curve != null)
            {
                _AddedPlots.Add(Plot3ControlVM);
            }
            if (Plot5ControlVM.IndividualPlotWrapperVM.PlotVM != null && Plot5ControlVM.IndividualPlotWrapperVM.PlotVM.Curve != null)
            {
                _AddedPlots.Add(Plot5ControlVM);
            }
            if (Plot7ControlVM.IndividualPlotWrapperVM.PlotVM != null && Plot7ControlVM.IndividualPlotWrapperVM.PlotVM.Curve != null)
            {
                _AddedPlots.Add(Plot7ControlVM);
            }
            if (Plot8ControlVM.IndividualPlotWrapperVM.PlotVM != null && Plot8ControlVM.IndividualPlotWrapperVM.PlotVM.Curve != null)
            {
                _AddedPlots.Add(Plot8ControlVM);
            }
            UpdateAvailablePlots();

        }

        public void LaunchNewImpactArea(object sender, EventArgs e)
        {

            ImpactArea.ImpactAreaOwnerElement parent =  StudyCache.GetParentElementOfType<ImpactArea.ImpactAreaOwnerElement>();
            parent.AddNew(sender, e);
            //if (_ImpactAreaOwner != null)
            //{
            //    List<ImpactArea.ImpactAreaOwnerElement> eles = StudyCache;// _ImpactAreaOwner.GetElementsOfType<ImpactArea.ImpactAreaOwnerElement>();
            //    if (eles.Count > 0)
            //    {
            //        //after finding the parent, then it launches the add impact area dialog
            //        eles.FirstOrDefault().AddNew(sender, e);
            //        //need to determine what the most recent element is and see if we already have it.
            //        if (eles.FirstOrDefault().Elements.Count > 0)
            //        {
            //            if (eles.FirstOrDefault().Elements.Count > ImpactAreas.Count)
            //            {
            //                List<ImpactArea.ImpactAreaElement> theNewList = new List<ImpactArea.ImpactAreaElement>();
            //                for (int i = 0; i < eles.FirstOrDefault().Elements.Count; i++)
            //                {
            //                    theNewList.Add((ImpactArea.ImpactAreaElement)eles.FirstOrDefault().Elements[i]);
            //                }
            //                ImpactAreas = theNewList;
            //                //AnalyiticalRelationships.Add((FrequencyRelationships.AnalyticalFrequencyElement)eles.FirstOrDefault().Elements.Last());
            //                SelectedImpactArea = ImpactAreas.Last();
            //            }
            //        }
            //    }

            //}

        }
   

        public void RunPreviewCompute(Object sender, EventArgs e)
        {

            //get the threshold values
            //PerformanceThreshold threshold = new PerformanceThreshold(PerformanceThresholdTypes.InteriorStage, 8);
            PerformanceThreshold threshold = new PerformanceThreshold(SelectedThresholdType, ThresholdValue);

            //get the selected impact area

            //get the selected struct inv

            //get all the selected curves

            List<FdaModel.Functions.BaseFunction> myListOfBaseFunctions = new List<FdaModel.Functions.BaseFunction>();

            if (Plot0ControlVM.IndividualPlotWrapperVM.PlotVM != null && Plot0ControlVM.IndividualPlotWrapperVM.PlotVM.Curve != null)
            {
                FdaModel.Functions.FrequencyFunctions.LogPearsonIII zero = (FdaModel.Functions.FrequencyFunctions.LogPearsonIII)Plot0ControlVM.IndividualPlotWrapperVM.PlotVM.BaseFunction;//new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(FlowFrequencyVM.SelectedFlowFrequencyElement.Distribution, FdaModel.Functions.FunctionTypes.InflowFrequency);
                myListOfBaseFunctions.Add(zero);
            }
            if (Plot1ControlVM.IndividualPlotWrapperVM.PlotVM != null && Plot1ControlVM.IndividualPlotWrapperVM.PlotVM.Curve != null)
            {
                FdaModel.Functions.BaseFunction one = Plot1ControlVM.IndividualPlotWrapperVM.PlotVM.BaseFunction;// new OrdinatesFunction(Plot1VM.Curve, FdaModel.Functions.FunctionTypes.InflowOutflow);
                myListOfBaseFunctions.Add(one);
            }
            if (Plot3ControlVM.IndividualPlotWrapperVM.PlotVM != null && Plot3ControlVM.IndividualPlotWrapperVM.PlotVM.Curve != null)
            {
                //i have to flip the x and y values back before computing
                ReadOnlyCollection<double> xs = Plot3ControlVM.IndividualPlotWrapperVM.PlotVM.BaseFunction.GetOrdinatesFunction().Function.YValues;// Plot3VM.Curve.YValues;
                ReadOnlyCollection<double> ys = Plot3ControlVM.IndividualPlotWrapperVM.PlotVM.BaseFunction.GetOrdinatesFunction().Function.XValues; //Plot3VM.Curve.XValues;
                //FdaModel.Functions.BaseFunction temp = new OrdinatesFunction();
                FdaModel.Functions.BaseFunction three = new OrdinatesFunction(xs, ys, FdaModel.Functions.FunctionTypes.Rating);
                myListOfBaseFunctions.Add(three);
            }
            if (Plot5ControlVM.IndividualPlotWrapperVM.PlotVM != null && Plot5ControlVM.IndividualPlotWrapperVM.PlotVM.Curve != null)
            {
                FdaModel.Functions.BaseFunction five = Plot5ControlVM.IndividualPlotWrapperVM.PlotVM.BaseFunction; //new OrdinatesFunction(Plot5VM.Curve, FdaModel.Functions.FunctionTypes.ExteriorInteriorStage);
                myListOfBaseFunctions.Add(five);
            }
            if (Plot7ControlVM.IndividualPlotWrapperVM.PlotVM != null && Plot7ControlVM.IndividualPlotWrapperVM.PlotVM.Curve != null)
            {
                FdaModel.Functions.BaseFunction seven = Plot7ControlVM.IndividualPlotWrapperVM.PlotVM.BaseFunction; //new OrdinatesFunction(Plot7VM.Curve, FdaModel.Functions.FunctionTypes.InteriorStageDamage);
                myListOfBaseFunctions.Add(seven);
            }


            //create lateral structure
            LateralStructure myLateralStruct = new LateralStructure(10);

            //create the condition
            FdaModel.ComputationPoint.Condition simpleTest = new FdaModel.ComputationPoint.Condition(2008, Name, myListOfBaseFunctions, threshold, null); //bool call Validate

            FdaModel.ComputationPoint.Outputs.Result result = new FdaModel.ComputationPoint.Outputs.Result(simpleTest,1);
            //create random number gen
            //Random randomNumberGenerator = new Random(0);

            //create the realization
            //FdaModel.ComputationPoint.Outputs.Realization simpleTestRealization = new FdaModel.ComputationPoint.Outputs.Realization(simpleTest, false, false); //bool oldCompute, bool performance only

            //compute
            //simpleTestRealization.Compute(randomNumberGenerator);

            //if it was successful, plot number 8. if not then message why not
            if(result.Realizations.Count == 0)
            {
                MessageBox.Show("A damage frequency curve could not be created", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            foreach (FdaModel.Functions.BaseFunction bf in result.Realizations.First().Functions)
            {
                if (bf.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
                {
                   Plot8ControlVM.IndividualPlotWrapperVM.PlotVM = new Plots.IndividualLinkedPlotVM(bf, "Damage Frequency", "Frequency", "Damage ($)");
                    //Plot8ControlVM.AddCurveToPlot(this, new EventArgs());
                    TrimZeroesFromCurve(Plot8ControlVM.IndividualPlotWrapperVM.PlotVM.Curve);
                    Plot8ControlVM.CurrentVM = (BaseViewModel)Plot8ControlVM.IndividualPlotWrapperVM;
                    //IsPlot8Visible = true;


                }
            }
            //if (IsPlot8Visible == false)
            //{
            //    StringBuilder messages = new StringBuilder();
            //    foreach (FdaModel.Utilities.Messager.ErrorMessage em in simpleTestRealization.Messages.Messages)
            //    {
            //        messages.AppendLine(em.Message);
            //    }
            //    Utilities.CustomMessageBoxVM custmb = new Utilities.CustomMessageBoxVM(Utilities.CustomMessageBoxVM.ButtonsEnum.OK, messages.ToString());
            //    Navigate(custmb);
            //}


            //the compute isn't working right now so i am going to just throw a random 8 at it.
            //double[] x5 = new double[] { .2f, .3f, .4f, .5f, .6f, .7f, .8f, .9f };
            //double[] y5 = new double[] { 2, 200, 300, 600, 1100, 2000, 3000, 4000 };
            //OrdinatesFunction eight = new OrdinatesFunction(x5, y5, FdaModel.Functions.FunctionTypes.DamageFrequency);

            //Plot8ControlVM.IndividualPlotWrapperVM.PlotVM = new Plots.IndividualLinkedPlotVM(eight,eight.GetOrdinatesFunction().Function,"Cody test");
            //Plot8ControlVM.CurrentVM = (FdaViewModel.BaseViewModel)Plot8ControlVM.IndividualPlotWrapperVM;
        }

        private void TrimZeroesFromCurve(Statistics.CurveIncreasing curveIncreasing)
        {
            int index = -1;
            for(int i = 0;i<curveIncreasing.XValues.Count;i++)
            //foreach(double y in curveIncreasing.YValues)
            {
                if(curveIncreasing.YValues[i] > 0)
                {
                    index = i;
                    break;
                }
            }
            if(index>-1)
            {
                curveIncreasing.RemoveRange(0, index);
                
            }
        }

        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => { if (Name == null) { return false; } else { return !Name.Equals(""); } }, "Name cannot be blank");
            AddRule(nameof(Year), () => { if (Year < 1900 || Year > 3000) { return false; } else { return true; } }, "Invalid Year");
            AddRule(nameof(SelectedImpactArea), () => { if (SelectedImpactArea == null) { return false; } else { return true; } }, "No impact area selected");
        }

        public override void Save()
        {
            if (Description == null) { Description = ""; }
            ConditionsElement elementToSave = ConditionFactory.BuildConditionsElement(this);
            Saving.PersistenceManagers.ConditionsPersistenceManager manager = Saving.PersistenceFactory.GetConditionsManager();
            if (IsImporter && HasSaved == false)
            {
                manager.SaveNew(elementToSave);
                HasSaved = true;
                OriginalElement = elementToSave;
            }
            else
            {
                manager.SaveExisting((ConditionsElement)OriginalElement, elementToSave, 0);
            }
        }




        #endregion
        #region Functions
        #endregion
    }
}
