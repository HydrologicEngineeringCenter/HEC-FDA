using FdaViewModel.AggregatedStageDamage;
using FdaViewModel.FlowTransforms;
using FdaViewModel.FrequencyRelationships;
using FdaViewModel.GeoTech;
using FdaViewModel.ImpactArea;
using FdaViewModel.Output;
using FdaViewModel.StageTransforms;
using FdaViewModel.Utilities;
using Model;
using Model.Conditions.Locations;
using Model.Conditions.Locations.Years;
using Model.Conditions.Locations.Years.Results;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FdaViewModel.Conditions
{
    public class ConditionsElement : Utilities.ChildElement
    {
        #region Notes
        #endregion
        #region Fields
        public event EventHandler EditConditionsTreeElement;
        public event EventHandler RemoveConditionsTreeElement;
        public event EventHandler RenameConditionsTreeElement;
        public event EventHandler UpdateExpansionValueInTreeElement;

        //private const string _TableConstant = "Conditions - ";

        private string _Description;
        private int _AnalysisYear;
        private ImpactArea.ImpactAreaElement _ImpactAreaSet;
        private ImpactArea.ImpactAreaRowItem _ImpactArea;
        private bool _UsesAnalyiticalFlowFrequency;
        private FrequencyRelationships.AnalyticalFrequencyElement _AnalyticalFlowFrequency;

        private bool _UseInflowOutflow;
        private FlowTransforms.InflowOutflowElement _InflowOutflowElement;

        private bool _UseRatingCurve;
        private StageTransforms.RatingCurveElement _RatingCurve;

        private bool _UseExteriorInteriorStage;
        private StageTransforms.ExteriorInteriorElement _ExteriorInteriorElement;

        private bool _UseLevee;
        private GeoTech.LeveeFeatureElement _LeveeElement;

        private bool _UseFailureFunction;
        private GeoTech.FailureFunctionElement _FailureFunctionElement;

        private bool _UseAggregatedStageDamage;
        private AggregatedStageDamage.AggregatedStageDamageElement _StageDamage;
        private bool _UseThreshold;
        private Model.IMetricEnum _MetricType;//dollars or stage. need enum.
        private double _ThresholdValue;
        //private ParentElement _ConditionsOwnerElement;
        private List<BaseFdaElement> _ConditionsTreeNodes;
        private bool _IsExpanded;
        private int _impactAreaID;
        private int _flowFreqID;
        private int _inflowOutflowID;
        private int _ratingID;
        private int _leveeFailureID;
        private int _extIntStageID;
        private int _stageDamageID;
        #endregion
        #region Properties
        public ConditionsPlotEditorVM ConditionsEditor { get; set; }
        public int ImpactAreaID { get; set; }
        public int FlowFreqID { get; set; }
        public int InflowOutflowID { get; set; }
        public int RatingID { get; set; }
        public int LeveeFailureID { get; set; }
        public int ExtIntStageID { get; set; }
        public int StageDamageID { get; set; }


        //public ICondition Condition { get; set; }

        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set { _IsExpanded = value; UpdateIsElementExpanded(this, new EventArgs()); }
        }
        public bool IsBold
        {
            get { return false; }
        }
        public int FontSize//this is somehow determining the size for the study tree, but not the cond tree
        {
            get { return 12; }
        }
        public List<BaseFdaElement> ConditionsTreeNodes
        {
            get { return _ConditionsTreeNodes; }
            set { _ConditionsTreeNodes = value; NotifyPropertyChanged(); }
        }

       
        //public GeoTech.FailureFunctionElement FailureFunctionElement
        //{
        //    get { return _FailureFunctionElement; }
        //    set { _FailureFunctionElement = value; NotifyPropertyChanged(); }
        //}
        //public bool UseFailureFunction
        //{
        //    get { return _UseFailureFunction; }
        //    set { _UseFailureFunction = value; NotifyPropertyChanged(); }
        //}
        //public GeoTech.LeveeFeatureElement LeveeElement
        //{
        //    get { return _LeveeElement; }
        //    set { _LeveeElement = value; NotifyPropertyChanged(); }
        //}
        //public bool UseLevee
        //{
        //    get { return _UseLevee; }
        //    set { _UseLevee = value; NotifyPropertyChanged(); }
        //}
        //public ExteriorInteriorElement ExteriorInteriorElement
        //{
        //    get { return _ExteriorInteriorElement; }
        //    set { _ExteriorInteriorElement = value; NotifyPropertyChanged(); }
        //}
        //public bool UseExteriorInteriorStage
        //{
        //    get { return _UseExteriorInteriorStage; }
        //    set { _UseExteriorInteriorStage = value; NotifyPropertyChanged(); }
        //}
        //public FlowTransforms.InflowOutflowElement InflowOutflowElement
        //{
        //    get { return _InflowOutflowElement; }
        //    set { _InflowOutflowElement = value; NotifyPropertyChanged(); }
        //}
        //public bool UseInflowOutflow
        //{
        //    get { return _UseInflowOutflow; }
        //    set { _UseInflowOutflow = value; NotifyPropertyChanged(); }
        //}
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }
        public int AnalysisYear
        {
            get { return _AnalysisYear; }
            set { _AnalysisYear = value; NotifyPropertyChanged(); }
        }
        //public ImpactArea.ImpactAreaElement ImpactAreaElement
        //{
        //    get { return _ImpactAreaSet; }
        //    set { _ImpactAreaSet = value; NotifyPropertyChanged(); }
        //}
        //public ImpactArea.ImpactAreaRowItem ImpactArea
        //{
        //    get { return _ImpactArea; }
        //    set { _ImpactArea = value; NotifyPropertyChanged(); }
        //}
        //public bool UseAnalyiticalFlowFrequency
        //{
        //    get { return _UsesAnalyiticalFlowFrequency; }
        //    set { _UsesAnalyiticalFlowFrequency = value; NotifyPropertyChanged(); }
        //}
        //public FrequencyRelationships.AnalyticalFrequencyElement AnalyticalFlowFrequency
        //{
        //    get { return _AnalyticalFlowFrequency; }
        //    set { _AnalyticalFlowFrequency = value; NotifyPropertyChanged(); }
        //}
        //public bool UseRatingCurve
        //{
        //    get { return _UseRatingCurve; }
        //    set { _UseRatingCurve = value; NotifyPropertyChanged(); }
        //}
        //public StageTransforms.RatingCurveElement RatingCurveElement
        //{
        //    get { return _RatingCurve; }
        //    set { _RatingCurve = value; NotifyPropertyChanged(); }
        //}
        //public bool UseAggregatedStageDamage
        //{
        //    get { return _UseAggregatedStageDamage; }
        //    set { _UseAggregatedStageDamage = value; NotifyPropertyChanged(); }
        //}
        //public AggregatedStageDamage.AggregatedStageDamageElement StageDamageElement
        //{
        //    get { return _StageDamage; }
        //    set { _StageDamage = value; NotifyPropertyChanged(); }
        //}
        //public bool UseThreshold
        //{
        //    get { return _UseThreshold; }
        //    set { _UseThreshold = value; NotifyPropertyChanged(); }
        //}
        public Model.IMetricEnum ThresholdType {
            get { return _MetricType; }
            set { _MetricType = value; NotifyPropertyChanged(); }
        }
        public double ThresholdValue
        {
            get { return _ThresholdValue; }
            set { _ThresholdValue = value; NotifyPropertyChanged(); }
        }

        
        //public string FlowFreqName { get; set; }
        //public string InfOutName { get; set; }
        //public string RatingName { get; set; }
        //public string ExtIntName { get; set; }
        //public string LeveeName { get; set; }
        //public string FailureFuncName { get; set; }
        //public string StageDamageName { get; set; }


        #endregion
        #region Constructors

        /// <summary>
        /// This should only be used in conjunction with the Conditions tree tab.
        /// I use this copy constructor to create a new conditions element that is identical to
        /// the conditions element in the study tree. This cond elem will be placed in the cond tree tab.
        /// It will call events for rename, remove, edit that get handled by its owner which is the 
        /// ConditionsTreeOwnerElement. These elements should never be saved. The identical version
        /// in the study tree gets saved.
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="owner"></param>
        public ConditionsElement(ConditionsElement elem) : base()
        {
            IsExpanded = elem.IsExpanded;
            Name = elem.Name;
            //_ConditionsOwnerElement = owner;
            CustomTreeViewHeader = elem.CustomTreeViewHeader;

            Description = elem.Description;
            AnalysisYear = elem.AnalysisYear;
            ImpactAreaID = elem.ImpactAreaID;
            FlowFreqID = elem.FlowFreqID;
            InflowOutflowID = elem.InflowOutflowID;
            RatingID = elem.RatingID;
            ExtIntStageID = elem.ExtIntStageID;
            LeveeFailureID = elem.LeveeFailureID;
            StageDamageID = elem.StageDamageID;

            ThresholdType = elem.ThresholdType;
            ThresholdValue = elem.ThresholdValue;
            

            //ImpactAreaElement = elem.ImpactAreaElement;
            //ImpactArea = elem.ImpactArea;

            //UseAnalyiticalFlowFrequency = elem.UseAnalyiticalFlowFrequency;
            //AnalyticalFlowFrequency = elem.AnalyticalFlowFrequency;

            //UseInflowOutflow = elem.UseInflowOutflow;
            //InflowOutflowElement = elem.InflowOutflowElement;

            //UseRatingCurve = elem.UseRatingCurve;
            //RatingCurveElement = elem.RatingCurveElement;

            //UseExteriorInteriorStage = elem.UseExteriorInteriorStage;
            //ExteriorInteriorElement = elem.ExteriorInteriorElement;

            //UseLevee = elem.UseLevee;
            //LeveeElement = elem.LeveeElement;

            //UseFailureFunction = elem.UseFailureFunction;
            //FailureFunctionElement = elem.FailureFunctionElement;

            //UseAggregatedStageDamage = elem.UseAggregatedStageDamage;
            //StageDamageElement = elem.StageDamageElement;

            //UseThreshold = elem.UseThreshold;

            NamedAction edit = new NamedAction();
            edit.Header = "Edit Condition";
            edit.Action = EditConditionsTreeElem;

            NamedAction compute = new NamedAction();
            compute.Header = "Compute Condition";
            compute.Action = ComputeCondition;

            NamedAction removeCondition = new NamedAction();
            removeCondition.Header = "Remove";
            removeCondition.Action = RemoveConditionsTreeElem;

            NamedAction renameElement = new NamedAction();
            renameElement.Header = "Rename";
            renameElement.Action = RenameConditionsTreeElem;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(edit);
            localActions.Add(compute);
            localActions.Add(removeCondition);
            localActions.Add(renameElement);

            Actions = localActions;

            LoadTheTreeNodes();

        }

        public ConditionsElement(string name, string description, int analysisYear, int impactAreaID,
                     int flowFreqID, int inflowOutflowID, int ratingID, int extIntID, int leveeFailureID, int stageDamageID,
                   IMetricEnum thresholdType, double thresholdValue) : base()
        {
            Name = name;
            // _ConditionsOwnerElement = owner;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/Condition.png");

            Description = description;
            AnalysisYear = analysisYear;
            ImpactAreaID = impactAreaID;
            //ImpactArea = indexLocation;

            FlowFreqID = flowFreqID;

            InflowOutflowID = inflowOutflowID;

            RatingID = ratingID;

            ExtIntStageID = extIntID;

            LeveeFailureID = leveeFailureID;

            StageDamageID = stageDamageID;

            ThresholdType = thresholdType;
            ThresholdValue = thresholdValue;

            NamedAction edit = new Utilities.NamedAction();
            edit.Header = "Edit Condition";
            edit.Action = EditCondition;

            NamedAction compute = new Utilities.NamedAction();
            compute.Header = "Compute Condition";
            compute.Action = ComputeCondition;

            NamedAction removeCondition = new Utilities.NamedAction();
            removeCondition.Header = "Remove";
            removeCondition.Action = RemoveElement;

            NamedAction renameElement = new Utilities.NamedAction(this);
            renameElement.Header = "Rename";
            renameElement.Action = Rename;

            List<NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(edit);
            localActions.Add(compute);
            localActions.Add(removeCondition);
            localActions.Add(renameElement);

            Actions = localActions;
           // _ConditionsTreeNodes = new List<BaseFdaElement>() { AnalyticalFlowFrequency, inflowOutflowElement };
        }

        /// <summary>
        /// Always use the ConditionsFactory to create a conditions Element
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="analysisYear"></param>
        /// <param name="impactAreaElement"></param>
        /// <param name="indexLocation"></param>
        /// 
        /// <param name="usesAnalyiticalFlowFrequency"></param>
        /// <param name="aFlowFreq"></param>
        /// 
        /// <param name="usesInflowOutflow"></param>
        /// <param name="inflowOutflowElement"></param>
        /// 
        /// <param name="useRating"></param>
        /// <param name="rc"></param>
        /// 
        /// <param name="useIntExtStage"></param>
        /// <param name="extInt"></param>
        /// 
        /// <param name="useLevee"></param>
        /// <param name="leveeElement"></param>
        /// 
        /// <param name="useFailureFunction"></param>
        /// <param name="failureFunctionElement"></param>
        /// 
        /// <param name="useAggStageDamage"></param>
        /// <param name="stageDamage"></param>
        /// 
        /// <param name="useThreshold"></param>
        /// <param name="thresholdType"></param>
        /// <param name="thresholdValue"></param>
        /// 
        /// <param name="owner"></param>
        //public ConditionsElement(string name, string description, int analysisYear, ImpactArea.ImpactAreaElement impactAreaElement, ImpactArea.ImpactAreaRowItem indexLocation,
        //    bool usesAnalyiticalFlowFrequency, FrequencyRelationships.AnalyticalFrequencyElement aFlowFreq, bool usesInflowOutflow, FlowTransforms.InflowOutflowElement inflowOutflowElement,
        //    bool useRating, StageTransforms.RatingCurveElement rc, bool useIntExtStage, StageTransforms.ExteriorInteriorElement extInt, bool useLevee, GeoTech.LeveeFeatureElement leveeElement,
        //    bool useFailureFunction, GeoTech.FailureFunctionElement failureFunctionElement, bool useAggStageDamage, AggregatedStageDamage.AggregatedStageDamageElement stageDamage,
        //    bool useThreshold, Model.IMetricEnum thresholdType, double thresholdValue ) : base()
        //{
        //    Name = name;
        //   // _ConditionsOwnerElement = owner;
        //    CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/Condition.png");

        //    Description = description;
        //    AnalysisYear = analysisYear;
        //    ImpactAreaElement = impactAreaElement;
        //    ImpactArea = indexLocation;

        //    UseAnalyiticalFlowFrequency = usesAnalyiticalFlowFrequency;
        //    AnalyticalFlowFrequency = aFlowFreq;

        //    UseInflowOutflow = usesInflowOutflow;
        //    InflowOutflowElement = inflowOutflowElement;

        //    UseRatingCurve = useRating;
        //    RatingCurveElement = rc;

        //    UseExteriorInteriorStage = useIntExtStage;
        //    ExteriorInteriorElement = extInt;

        //    UseLevee = useLevee;
        //    LeveeElement = leveeElement;

        //    UseFailureFunction = useFailureFunction;
        //    FailureFunctionElement = failureFunctionElement;

        //    UseAggregatedStageDamage = useAggStageDamage;
        //    StageDamageElement = stageDamage;

        //    UseThreshold = useThreshold;
        //    ThresholdType = thresholdType;
        //    ThresholdValue = thresholdValue;

        //    Utilities.NamedAction edit = new Utilities.NamedAction();
        //    edit.Header = "Edit Condition";
        //    edit.Action = EditCondition;

        //    Utilities.NamedAction compute = new Utilities.NamedAction();
        //    compute.Header = "Compute Condition";
        //    compute.Action = ComputeCondition;

        //    Utilities.NamedAction removeCondition = new Utilities.NamedAction();
        //    removeCondition.Header = "Remove";
        //    removeCondition.Action = RemoveElement;

        //    Utilities.NamedAction renameElement = new Utilities.NamedAction(this);
        //    renameElement.Header = "Rename";
        //    renameElement.Action = Rename;

        //    List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
        //    localActions.Add(edit);
        //    localActions.Add(compute);
        //    localActions.Add(removeCondition);
        //    localActions.Add(renameElement);

        //    Actions = localActions;
        //    _ConditionsTreeNodes = new List<BaseFdaElement>() { AnalyticalFlowFrequency,inflowOutflowElement};

        //}
       
        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetConditionsManager().Remove(this);
        }

        #region BuildControlsFromElement
        //todo: Refactor: commented out
        private Plots.IndividualLinkedPlotControlVM BuildLP3ControlFromElement(int lp3ID)
        {
            AnalyticalFrequencyElement selectedElem = null;
            List<AnalyticalFrequencyElement> listOfLp3 = StudyCache.GetChildElementsOfType<AnalyticalFrequencyElement>();// StudyCache.FlowFrequencyElements;// GetElementsOfType<FrequencyRelationships.AnalyticalFrequencyElement>();
            foreach(AnalyticalFrequencyElement elem in listOfLp3)
            {
                if(elem.GetElementID() == lp3ID)
                {
                    selectedElem = elem;
                }
            }
            //if we are here and we didn't find the element then something has gone terribly wrong
            if(selectedElem == null)
            {
                throw new Exception("Could not find the LP3 element with an ID of " + lp3ID);
            }
            
            
            Plots.IndividualLinkedPlotVM plotVM = new Plots.IndividualLinkedPlotVM(selectedElem.Curve, false,true,true,selectedElem.Name);
            Plots.ConditionsIndividualPlotWrapperVM plotWrapper = new Plots.ConditionsIndividualPlotWrapperVM(false, true, true, false, false, false);
            plotWrapper.PlotVM = plotVM;
            AddFlowFrequencyToConditionVM importer = new AddFlowFrequencyToConditionVM(listOfLp3, selectedElem);
            //importer.RequestNavigation += Navigate;
            return new Plots.IndividualLinkedPlotControlVM(IFdaFunctionEnum.InflowFrequency, plotWrapper, new Plots.IndividualLinkedPlotCoverButtonVM("Flow Frequency Curve"), importer);

        }

        private Plots.IndividualLinkedPlotControlVM BuildInflowOutflowControlFromElement(int inOutID)
        {
            InflowOutflowElement selectedElem = null;
            List<InflowOutflowElement> listOfInfOut = StudyCache.GetChildElementsOfType<InflowOutflowElement>();

            foreach (InflowOutflowElement elem in listOfInfOut)
            {
                if (elem.GetElementID() == inOutID)
                {
                    selectedElem = elem;
                }
            }
            //if we are here and we didn't find the element then something has gone terribly wrong
            if (selectedElem == null)
            {
                throw new Exception("Could not find the inflow-outflow element with an ID of " + inOutID);
            }


            Plots.IndividualLinkedPlotVM plotVM = new Plots.IndividualLinkedPlotVM(selectedElem.Curve,true,true,false, selectedElem.Name);

            Plots.ConditionsIndividualPlotWrapperVM plotWrapper = new Plots.ConditionsIndividualPlotWrapperVM(true,true,false,false,true,true);
            plotWrapper.PlotVM = plotVM;
            AddInflowOutflowToConditionVM importer = new AddInflowOutflowToConditionVM(listOfInfOut, selectedElem);
            //importer.RequestNavigation += Navigate;
            return new Plots.IndividualLinkedPlotControlVM(IFdaFunctionEnum.InflowOutflow, plotWrapper, new Plots.IndividualLinkedPlotCoverButtonVM("Inflow Outflow"), importer,
                new Plots.DoubleLineModulatorCoverButtonVM(),
                new Plots.DoubleLineModulatorWrapperVM(plotVM));
        }

        private Plots.IndividualLinkedPlotControlVM BuildRatingControlFromElement(int ratingID)
        {
            RatingCurveElement selectedElem = null;
            List<RatingCurveElement> listOfRatingCurves = StudyCache.GetChildElementsOfType<RatingCurveElement>();

            foreach (RatingCurveElement elem in listOfRatingCurves)
            {
                if (elem.GetElementID() == ratingID)
                {
                    selectedElem = elem;
                }
            }
            //if we are here and we didn't find the element then something has gone terribly wrong
            if (selectedElem == null)
            {
                throw new Exception("Could not find the rating element with an ID of " + ratingID);
            }

            Plots.IndividualLinkedPlotVM plotVM = new Plots.IndividualLinkedPlotVM(selectedElem.Curve,false,true,false, selectedElem.Name);

            Plots.ConditionsIndividualPlotWrapperVM plotWrapper = new Plots.ConditionsIndividualPlotWrapperVM(false,true,false,false,false,true);
            plotWrapper.PlotVM = plotVM;
            AddRatingCurveToConditionVM importer = new AddRatingCurveToConditionVM(listOfRatingCurves, selectedElem);
            //importer.RequestNavigation += Navigate;
            // Navigate(importer, false, false, "mytest");
            return new Plots.IndividualLinkedPlotControlVM(IFdaFunctionEnum.Rating, plotWrapper, new Plots.IndividualLinkedPlotCoverButtonVM("Rating Curve"), importer);
        }

        private Plots.IndividualLinkedPlotControlVM BuildLeveeFailureFromElement(int leveeFailureID)
        {
            LeveeFeatureElement selectedElem = null;
            List<LeveeFeatureElement> listOfLeveeFeatureElements = StudyCache.GetChildElementsOfType<LeveeFeatureElement>();

            foreach (LeveeFeatureElement elem in listOfLeveeFeatureElements)
            {
                if (elem.GetElementID() == leveeFailureID)
                {
                    selectedElem = elem;
                }
            }
            //if we are here and we didn't find the element then something has gone terribly wrong
            if (selectedElem == null)
            {
                throw new Exception("Could not find the levee feature element with an ID of " + leveeFailureID);
            }

            Plots.IndividualLinkedPlotVM plotVM = new Plots.IndividualLinkedPlotVM(selectedElem.Curve,false,false,false, selectedElem.Name);

            Plots.ConditionsIndividualPlotWrapperVM plotWrapper = new Plots.ConditionsIndividualPlotWrapperVM(false,false,false,true,true,true);
            plotWrapper.PlotVM = plotVM;
            AddFailureFunctionToConditionVM importer = new AddFailureFunctionToConditionVM(listOfLeveeFeatureElements, selectedElem);
            //importer.RequestNavigation += Navigate;
            return new Plots.IndividualLinkedPlotControlVM(IFdaFunctionEnum.LateralStructureFailure, plotWrapper, new Plots.IndividualLinkedPlotCoverButtonVM("Lateral Structure"), importer,
                new Plots.DoubleLineModulatorHorizontalCoverButtonVM("Lateral Structure"),
                new Plots.ConditionsHorizontalFailureFunctionVM(plotVM));
        }

        private Plots.IndividualLinkedPlotControlVM BuildExtIntControlFromElement(int extintID)
        {
            ExteriorInteriorElement selectedElem = null;
            List<ExteriorInteriorElement> listOfExtIntElements = StudyCache.GetChildElementsOfType<ExteriorInteriorElement>();

            foreach (ExteriorInteriorElement elem in listOfExtIntElements)
            {
                if (elem.GetElementID() == extintID)
                {
                    selectedElem = elem;
                }
            }
            //if we are here and we didn't find the element then something has gone terribly wrong
            if (selectedElem == null)
            {
                throw new Exception("Could not find the exterior-interior stage element with an ID of " + extintID);
            }

            Plots.IndividualLinkedPlotVM plotVM = new Plots.IndividualLinkedPlotVM(selectedElem.Curve,false,false,false, selectedElem.Name);

            Plots.ConditionsIndividualPlotWrapperVM plotWrapper = new Plots.ConditionsIndividualPlotWrapperVM(false,false,false,false,true,true);
            plotWrapper.PlotVM = plotVM;
            AddExteriorInteriorStageToConditionVM importer = new AddExteriorInteriorStageToConditionVM(listOfExtIntElements, selectedElem);
            //importer.RequestNavigation += Navigate;
            return new Plots.IndividualLinkedPlotControlVM(IFdaFunctionEnum.ExteriorInteriorStage, plotWrapper, new Plots.IndividualLinkedPlotCoverButtonVM("Ext Int Stage Curve"), importer,
                new Plots.DoubleLineModulatorHorizontalCoverButtonVM("Ext Int Stage"),
                new Plots.HorizontalDoubleLineModulatorWrapperVM(plotVM));
        }
        private Plots.IndividualLinkedPlotControlVM BuildStageDamageControlFromElement(int stageDamageID)
        {
            AggregatedStageDamageElement selectedElem = null;
            List<AggregatedStageDamageElement> listOfStageDamage = StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();

            foreach (AggregatedStageDamageElement elem in listOfStageDamage)
            {
                if (elem.GetElementID() == stageDamageID)
                {
                    selectedElem = elem;
                }
            }
            //if we are here and we didn't find the element then something has gone terribly wrong
            if (selectedElem == null)
            {
                throw new Exception("Could not find the stage damage element with an ID of " + stageDamageID);
            }

            Plots.IndividualLinkedPlotVM plotVM = new Plots.IndividualLinkedPlotVM(selectedElem.Curve,false,true,false, selectedElem.Name);

            Plots.ConditionsIndividualPlotWrapperVM plotWrapper = new Plots.ConditionsIndividualPlotWrapperVM(false,true,false,false,true,true);
            plotWrapper.PlotVM = plotVM;
            AddStageDamageToConditionVM importer = new AddStageDamageToConditionVM(listOfStageDamage, selectedElem);
            //importer.RequestNavigation += Navigate;
            return new Plots.IndividualLinkedPlotControlVM(IFdaFunctionEnum.InteriorStageDamage, plotWrapper, new Plots.IndividualLinkedPlotCoverButtonVM("Int Stage Damage Curve"), importer);
        }
        #endregion
        public void EditCondition(object arg1, EventArgs arg2)
        {
            //todo: Refactor: CO the body of this method.
            List<ImpactArea.ImpactAreaElement> impactAreas = StudyCache.GetChildElementsOfType<ImpactArea.ImpactAreaElement>();
            List<FrequencyRelationships.AnalyticalFrequencyElement> freqeles = StudyCache.GetChildElementsOfType<FrequencyRelationships.AnalyticalFrequencyElement>();
            List<FlowTransforms.InflowOutflowElement> inflowOutflowList = StudyCache.GetChildElementsOfType<FlowTransforms.InflowOutflowElement>();
            List<StageTransforms.RatingCurveElement> ratingeles = StudyCache.GetChildElementsOfType<StageTransforms.RatingCurveElement>();

            List<StageTransforms.ExteriorInteriorElement> extIntList = StudyCache.GetChildElementsOfType<StageTransforms.ExteriorInteriorElement>();
            List<GeoTech.LeveeFeatureElement> leveeList = StudyCache.GetChildElementsOfType<GeoTech.LeveeFeatureElement>();
            List<GeoTech.FailureFunctionElement> failureFunctionList = StudyCache.GetChildElementsOfType<GeoTech.FailureFunctionElement>();


            List<AggregatedStageDamage.AggregatedStageDamageElement> damageles = StudyCache.GetChildElementsOfType<AggregatedStageDamage.AggregatedStageDamageElement>();


            //////////////////////////////

            Plots.IndividualLinkedPlotControlVM lp3Control;
            Plots.IndividualLinkedPlotControlVM infOutControl;
            Plots.IndividualLinkedPlotControlVM ratingControl;
            Plots.IndividualLinkedPlotControlVM leveeFailureControl;
            Plots.IndividualLinkedPlotControlVM extIntStageControl;
            Plots.IndividualLinkedPlotControlVM stageDamageControl;
            Plots.IndividualLinkedPlotControlVM damageFrequencyControl;

            if (FlowFreqID != -1)
            {
                lp3Control = BuildLP3ControlFromElement(FlowFreqID);
            }
            else
            {
                lp3Control = ConditionsOwnerElement.BuildDefaultLP3Control(StudyCache.GetParentElementOfType<Conditions.ConditionsOwnerElement>());
            }

            if (InflowOutflowID != -1)
            {
                infOutControl = BuildInflowOutflowControlFromElement(InflowOutflowID);
            }
            else
            {
                infOutControl = ConditionsOwnerElement.BuildDefaultInflowOutflowControl(StudyCache.GetParentElementOfType<Conditions.ConditionsOwnerElement>());
            }

            if (RatingID != -1)
            {
                ratingControl = BuildRatingControlFromElement(RatingID);
            }
            else
            {
                ratingControl = ConditionsOwnerElement.BuildDefaultRatingControl(StudyCache.GetParentElementOfType<Conditions.ConditionsOwnerElement>());
            }

            if(LeveeFailureID != -1)
            {
                leveeFailureControl = BuildLeveeFailureFromElement(LeveeFailureID);
            }
            else
            {
                leveeFailureControl = ConditionsOwnerElement.BuildDefaultLateralFeaturesControl(StudyCache.GetParentElementOfType<ConditionsOwnerElement>());
            }

            if (ExtIntStageID != -1)
            {
                extIntStageControl = BuildExtIntControlFromElement(ExtIntStageID);
            }
            else
            {
                extIntStageControl = ConditionsOwnerElement.BuildDefaultExtIntStageControl(StudyCache.GetParentElementOfType<Conditions.ConditionsOwnerElement>());
            }

            if (StageDamageID != -1)
            {
                stageDamageControl = BuildStageDamageControlFromElement(StageDamageID);
            }
            else
            {
                stageDamageControl = ConditionsOwnerElement.BuildDefaultStageDamageControl(StudyCache.GetParentElementOfType<Conditions.ConditionsOwnerElement>());
            }

            damageFrequencyControl = ConditionsOwnerElement.BuildDefaultDamageFrequencyControl(StudyCache.GetParentElementOfType<Conditions.ConditionsOwnerElement>());

            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                 .WithSiblingRules(this);

            ConditionsEditor = new ConditionsPlotEditorVM(impactAreas, lp3Control, infOutControl, ratingControl, leveeFailureControl, extIntStageControl,
                stageDamageControl, damageFrequencyControl, this, actionManager);

            ConditionsEditor.RequestNavigation += Navigate;
            string header = "Create Condition";
            DynamicTabVM tab = new DynamicTabVM(header, ConditionsEditor, "CreateCondition");
            Navigate(tab, false, false);
        }

        
        private ConditionLocationYearNoLateralStructure CreateConditionNoLateralStructure()
        {
            //if (!Validate())
            {
                //todo: show errors in popup?
                //return null;
            }
            bool hasInflowOutflow = InflowOutflowID != -1;
            bool hasLeveeFailure = LeveeFailureID != -1;
            bool hasExtInt = ExtIntStageID != -1;
            try
            {
                //required params
                ImpactAreaElement impArea = (ImpactAreaElement)StudyCache.GetChildElementOfType(typeof(ImpactAreaElement), ImpactAreaID);
                AnalyticalFrequencyElement flowFreqElement = (AnalyticalFrequencyElement)StudyCache.GetChildElementOfType(typeof(AnalyticalFrequencyElement), FlowFreqID);
                RatingCurveElement ratingElement = (RatingCurveElement)StudyCache.GetChildElementOfType(typeof(RatingCurveElement), RatingID);
                AggregatedStageDamageElement stageDamageElement = (AggregatedStageDamageElement)StudyCache.GetChildElementOfType(typeof(AggregatedStageDamageElement), StageDamageID);

                //possible other functions
                InflowOutflowElement inflowOutflowElement = null;
                LeveeFeatureElement leveeFailureElement = null;
                ExteriorInteriorElement extIntElement = null;
                if (hasInflowOutflow)
                {
                    inflowOutflowElement = (InflowOutflowElement)StudyCache.GetChildElementOfType(typeof(InflowOutflowElement), InflowOutflowID);
                }
                if (hasLeveeFailure)
                {
                    leveeFailureElement = (LeveeFeatureElement)StudyCache.GetChildElementOfType(typeof(LeveeFeatureElement), LeveeFailureID);
                }
                if (hasExtInt)
                {
                    extIntElement = (ExteriorInteriorElement)StudyCache.GetChildElementOfType(typeof(ExteriorInteriorElement), ExtIntStageID);
                }

                List<ITransformFunction> transforms = new List<ITransformFunction>();
                if (hasInflowOutflow)
                {
                    transforms.Add((ITransformFunction)inflowOutflowElement.Curve);
                }
                transforms.Add((ITransformFunction)ratingElement.Curve);
                if (hasLeveeFailure)
                {
                    transforms.Add((ITransformFunction)leveeFailureElement.Curve);
                }
                if (hasExtInt)
                {
                    transforms.Add((ITransformFunction)extIntElement.Curve);
                }
                transforms.Add((ITransformFunction)stageDamageElement.Curve);

                ILocation location = new Location(impArea.Name, impArea.Description);
                List<IMetric> metrics = new List<IMetric>();
                metrics.Add(IMetricFactory.Factory()); //this is the ead metric
                metrics.Add( IMetricFactory.Factory(ThresholdType, ThresholdValue));

                IFrequencyFunction inflowFreqFunc = (IFrequencyFunction)flowFreqElement.Curve;

                return new ConditionLocationYearNoLateralStructure(location, AnalysisYear, inflowFreqFunc, transforms, metrics);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error in trying to retrieve one of the sub elements to create the condition.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private void DisplayResults(IConditionLocationYearResult result)
        {
            LinkedPlotsVM vm = new LinkedPlotsVM(result);
            vm.RequestNavigation += Navigate;
            string header = "Results";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "resultViewer");
            Navigate(tab, false, false);
        }
        private void ComputeCondition(object arg1, EventArgs arg2)
        {
            //IConditionLocationYearResult dummyResult = null;
            //DisplayResults(dummyResult);
            //return;

            EnterSeedVM enterSeedVM = new EnterSeedVM();
            string header = "Enter Seed Value";
            DynamicTabVM tab = new DynamicTabVM(header, enterSeedVM, "EnterSeed");
            Navigate(tab, true, true);

            int seedValue = enterSeedVM.Seed;

            bool hasLeveeFailure = LeveeFailureID != -1;
            if (hasLeveeFailure)
            {

            }
            else
            {
                ConditionLocationYearNoLateralStructure condition = CreateConditionNoLateralStructure();

                if (condition == null)
                {
                    return;
                }
                IConditionLocationYearRealization conditionRealization = condition.ComputePreview();
                //get the damage frequency and plot it
                IReadOnlyDictionary<IParameterEnum, ISampledParameter<IFdaFunction>> realizationFunctions = conditionRealization.Functions;

                IConvergenceCriteria convergenceCriteria = IConvergenceCriteriaFactory.Factory();
                Dictionary<IMetric, IConvergenceCriteria> metricsDictionary = new Dictionary<IMetric, IConvergenceCriteria>();
                foreach (IMetric metric in condition.Metrics)
                {
                    metricsDictionary.Add(metric, IConvergenceCriteriaFactory.Factory());
                }

                IReadOnlyDictionary<IMetric, IConvergenceCriteria> metrics = new ReadOnlyDictionary<IMetric, IConvergenceCriteria>(metricsDictionary);

                int seed = 5;
                //todo pop up and have user enter seed. Put the random clock time as int into it as default.
                //This constructor will automatically run the compute
                IConditionLocationYearResult result = new ConditionLocationYearResult(condition, metrics, 99);
                DisplayResults(result);
            }
            //store the seed with the other results.

            //get the result:

















            ////todo: Refactor: I commented out this method
            ////convert to model types, run model compute, show results in window.
            ////ImpactArea.Name
            ////AnalyticalFlowFrequency.Distribution;
            ////RatingCurve.RatingCurve //isvalid //distribution //getx gety
            ////StageDamage.Curve same as above.
            ////convert to model objects
            ////compute model objects
            ////take result and pass to results viewmodel
            //List<FdaModel.Functions.BaseFunction> listOfBaseFunctions = new List<FdaModel.Functions.BaseFunction>();
            //FdaModel.Functions.FrequencyFunctions.LogPearsonIII LP3 = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(AnalyticalFlowFrequency.Distribution, FdaModel.Functions.FunctionTypes.InflowFrequency);

            //listOfBaseFunctions.Add(LP3);

            ////FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction inflowOutflow = new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((Statistics.UncertainCurveIncreasing)InflowOutflowElement.InflowOutflowCurve, FdaModel.Functions.FunctionTypes.InflowOutflow);

            //// listOfBaseFunctions.Add(inflowOutflow);

            //FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction rating = new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((Statistics.UncertainCurveIncreasing)RatingCurveElement.Curve, FdaModel.Functions.FunctionTypes.Rating);

            //listOfBaseFunctions.Add(rating);

            ////FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction extIntStage = new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((Statistics.UncertainCurveIncreasing)ExteriorInteriorElement.ExteriorInteriorCurve, FdaModel.Functions.FunctionTypes.ExteriorInteriorStage);

            ////listOfBaseFunctions.Add(extIntStage);

            //FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction stageDamage = new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((Statistics.UncertainCurveIncreasing)StageDamageElement.Curve, FdaModel.Functions.FunctionTypes.InteriorStageDamage);

            //listOfBaseFunctions.Add(stageDamage);

            //FdaModel.ComputationPoint.PerformanceThreshold threshold;

            ////ThresholdValue = 120;
            ////when i do the real compute I will need to handle every threshold type
            //if (ThresholdType == FdaModel.ComputationPoint.PerformanceThresholdTypes.InteriorStage)
            //{
            //    threshold = new FdaModel.ComputationPoint.PerformanceThreshold(FdaModel.ComputationPoint.PerformanceThresholdTypes.InteriorStage, ThresholdValue);

            //}
            //else // ThresholdType == "Dollars"
            //{
            //    threshold = new FdaModel.ComputationPoint.PerformanceThreshold(FdaModel.ComputationPoint.PerformanceThresholdTypes.Damage, ThresholdValue);

            //}


            //FdaModel.ComputationPoint.Condition condition = new FdaModel.ComputationPoint.Condition(AnalysisYear, ImpactAreaElement.Name, listOfBaseFunctions, threshold, null); //this constructor will call Validate

            ////FdaModel.ComputationPoint.Outputs.Realization realization = new FdaModel.ComputationPoint.Outputs.Realization(condition, false, false);
            ////Random randomNumGenerator = new Random(0);
            ////realization.Compute(randomNumGenerator);

            //FdaModel.ComputationPoint.Outputs.Result result = new FdaModel.ComputationPoint.Outputs.Result(condition, 10);

            //List<string> selectedElementNames = new List<string>();
            //selectedElementNames.Add(AnalyticalFlowFrequency.Name);
            //selectedElementNames.Add(RatingCurveElement.Name);
            //if (ExteriorInteriorElement != null)
            //{
            //    selectedElementNames.Add(ExteriorInteriorElement.Name);
            //}
            //selectedElementNames.Add(StageDamageElement.Name);
            //selectedElementNames.Add("Computed Stage Frequency");
            //// write out results for testing purposes.
            //if (result.Realizations.Count != 0)
            //{
            //    Plots.LinkedPlotsVM vem = new Plots.LinkedPlotsVM(result, ThresholdType, ThresholdValue, selectedElementNames);
            //    string title = "Condition " + Name;
            //    DynamicTabVM tab = new DynamicTabVM(title, vem, "ComputedCondition" + Name);
            //    Navigate(tab, false, false);
            //}
            //else
            //{
            //    MessageBox.Show("The compute produced no realizations");
            //}
            ////Output.LinkedPlotsVM vm = new Output.LinkedPlotsVM(result);
            ////Navigate(vm);
        }
        #endregion
        #region Voids
        /// <summary>
        /// Loads the treenodes under the condition in the conditions tree tab.
        /// </summary>
        private void LoadTheTreeNodes()
        {
            bool hasInflowOutflow = InflowOutflowID != -1;
            bool hasLeveeFailure = LeveeFailureID != -1;
            bool hasExtInt = ExtIntStageID != -1;

            _ConditionsTreeNodes = new List<BaseFdaElement>();
            //required elems
            ImpactAreaElement impArea = (ImpactAreaElement)StudyCache.GetChildElementOfType(typeof(ImpactAreaElement), ImpactAreaID);
            AnalyticalFrequencyElement flowFreqElement = (AnalyticalFrequencyElement)StudyCache.GetChildElementOfType(typeof(AnalyticalFrequencyElement), FlowFreqID);
            RatingCurveElement ratingElement = (RatingCurveElement)StudyCache.GetChildElementOfType(typeof(RatingCurveElement), RatingID);
            AggregatedStageDamageElement stageDamageElement = (AggregatedStageDamageElement)StudyCache.GetChildElementOfType(typeof(AggregatedStageDamageElement), StageDamageID);


            //possible other functions
            InflowOutflowElement inflowOutflowElement = null;
            LeveeFeatureElement leveeFailureElement = null;
            ExteriorInteriorElement extIntElement = null;
            if (hasInflowOutflow)
            {
                inflowOutflowElement = (InflowOutflowElement)StudyCache.GetChildElementOfType(typeof(InflowOutflowElement), InflowOutflowID);
            }
            if (hasLeveeFailure)
            {
                leveeFailureElement = (LeveeFeatureElement)StudyCache.GetChildElementOfType(typeof(LeveeFeatureElement), LeveeFailureID);
            }
            if (hasExtInt)
            {
                extIntElement = (ExteriorInteriorElement)StudyCache.GetChildElementOfType(typeof(ExteriorInteriorElement), ExtIntStageID);
            }

            _ConditionsTreeNodes.Add(impArea);
            _ConditionsTreeNodes.Add(flowFreqElement);
            if(hasInflowOutflow)
            {
                _ConditionsTreeNodes.Add(inflowOutflowElement);
            }
            _ConditionsTreeNodes.Add(ratingElement);
            if(hasLeveeFailure)
            {
                _ConditionsTreeNodes.Add(leveeFailureElement);
            }
            if(hasExtInt)
            {
                _ConditionsTreeNodes.Add(extIntElement);
            }
            _ConditionsTreeNodes.Add(stageDamageElement);
            //if (ImpactAreaElement != null)
            //{
            //    _ConditionsTreeNodes.Add(ImpactAreaElement);
            //}
            //if (AnalyticalFlowFrequency != null)
            //{
            //    _ConditionsTreeNodes.Add(AnalyticalFlowFrequency);
            //}
            //if (InflowOutflowElement != null)
            //{
            //    _ConditionsTreeNodes.Add(InflowOutflowElement);
            //}
            //if (RatingCurveElement != null)
            //{
            //    _ConditionsTreeNodes.Add(RatingCurveElement);
            //}
            //if (ExteriorInteriorElement != null)
            //{
            //    _ConditionsTreeNodes.Add(ExteriorInteriorElement);
            //}
            //if (StageDamageElement != null)
            //{
            //    _ConditionsTreeNodes.Add(StageDamageElement);
            //}
        }
        private void UpdateIsElementExpanded(object sender, EventArgs e)
        {
            if(UpdateExpansionValueInTreeElement != null)
            {
                UpdateExpansionValueInTreeElement.Invoke(this, e);
            }
        }
        private void RenameConditionsTreeElem(object sender, EventArgs e)
        {
            if (RenameConditionsTreeElement != null)
            {
                RenameConditionsTreeElement.Invoke(this, e);
            }
        }
        private void RemoveConditionsTreeElem(object sender, EventArgs e)
        {
            if (RemoveConditionsTreeElement != null)
            {
                RemoveConditionsTreeElement.Invoke(this, e);
            }
        }

        private void EditConditionsTreeElem(object sender, EventArgs e)
        {
            if (EditConditionsTreeElement != null)
            {
                EditConditionsTreeElement.Invoke(this, e);
            }
        }
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }



        //public override object[] RowData()
        //{
        //    FlowFreqName = (AnalyticalFlowFrequency == null)? "" :AnalyticalFlowFrequency.Name;
        //    InfOutName = (InflowOutflowElement == null)? "" : InflowOutflowElement.Name;
        //    RatingName = (RatingCurveElement == null)? "" : RatingCurveElement.Name;
        //    ExtIntName = (ExteriorInteriorElement == null) ? "" : ExteriorInteriorElement.Name;
        //    LeveeName = (LeveeElement == null) ? "" : LeveeElement.Name;
        //    FailureFuncName = (FailureFunctionElement == null) ? "" : FailureFunctionElement.Name;
        //    StageDamageName = (StageDamageElement == null) ? "" : StageDamageElement.Name;

        //    return new object[] { Name, Description, AnalysisYear, ImpactAreaElement.Name,
        //        UseAnalyiticalFlowFrequency, FlowFreqName,
        //        UseInflowOutflow, InfOutName,
        //        UseRatingCurve,RatingName,
        //        UseExteriorInteriorStage,ExtIntName,
        //        UseLevee,LeveeName,
        //        UseFailureFunction,FailureFuncName,
        //        UseAggregatedStageDamage, StageDamageName,
        //        UseThreshold, ThresholdType,ThresholdValue};
        //}


        //public void UpdateElementInEditor_ChildRemoved(ConditionsElement newElement)
        //{
        //    //todo: i think i need to set this to null when it closes? not sure.
        //    if(ConditionsEditor != null)
        //    {
        //        ConditionsEditor.UpdateEditorWhileEditing_ChildRemoved(this, newElement);
        //    }
        //}
        public void UpdateElementInEditor_ChildModified(int elemID, ChildElement newElement)
        {
            //todo: i think i need to set this to null when it closes? not sure.
            if (ConditionsEditor != null)
            {
                ConditionsEditor.UpdateEditorWhileEditing_ChildModified(elemID, newElement);
            }
        }

        #endregion
        #region Functions
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            ConditionsElement elem = (ConditionsElement)elementToClone;
            return new ConditionsElement(elem);
        }


        //public ICondition CreateCondition()
       // {
           // return ConditionFactory.Factory(Name, AnalysisYear, GetEntryFrequencyFunction(), GetTransformFunctions(),
               // GetMetrics());

        //}

        //private IFrequencyFunction GetEntryFrequencyFunction()
        //{
        //    if(_UsesAnalyiticalFlowFrequency)
        //    {
        //        return (IFrequencyFunction)AnalyticalFlowFrequency.Curve;
        //    }
        //    //todo: shouldn't there be an outflow freq option here? and the other frequency options?
        //    throw new NotImplementedException();
        //}
        private List<ITransformFunction> GetTransformFunctions()
        {
            throw new NotImplementedException();
        }

        private List<IMetric> GetMetrics()
        {
            throw new NotImplementedException();

        }
        #endregion



    }
}
