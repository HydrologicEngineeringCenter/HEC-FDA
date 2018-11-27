using FdaModel.Functions;
using FdaViewModel.StageTransforms;
using FdaViewModel.Utilities;
using Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private const string _TableConstant = "Conditions - ";


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
        private FdaModel.ComputationPoint.PerformanceThresholdTypes _ThresholdType;//dollars or stage. need enum.
        private double _ThresholdValue;
        //private ParentElement _ConditionsOwnerElement;
        private List<BaseFdaElement> _ConditionsTreeNodes;
        private bool _IsExpanded;
        #endregion
        #region Properties

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

       
        public GeoTech.FailureFunctionElement FailureFunctionElement
        {
            get { return _FailureFunctionElement; }
            set { _FailureFunctionElement = value; NotifyPropertyChanged(); }
        }
        public bool UseFailureFunction
        {
            get { return _UseFailureFunction; }
            set { _UseFailureFunction = value; NotifyPropertyChanged(); }
        }
        public GeoTech.LeveeFeatureElement LeveeElement
        {
            get { return _LeveeElement; }
            set { _LeveeElement = value; NotifyPropertyChanged(); }
        }
        public bool UseLevee
        {
            get { return _UseLevee; }
            set { _UseLevee = value; NotifyPropertyChanged(); }
        }
        public StageTransforms.ExteriorInteriorElement ExteriorInteriorElement
        {
            get { return _ExteriorInteriorElement; }
            set { _ExteriorInteriorElement = value; NotifyPropertyChanged(); }
        }
        public bool UseExteriorInteriorStage
        {
            get { return _UseExteriorInteriorStage; }
            set { _UseExteriorInteriorStage = value; NotifyPropertyChanged(); }
        }
        public FlowTransforms.InflowOutflowElement InflowOutflowElement
        {
            get { return _InflowOutflowElement; }
            set { _InflowOutflowElement = value; NotifyPropertyChanged(); }
        }
        public bool UseInflowOutflow
        {
            get { return _UseInflowOutflow; }
            set { _UseInflowOutflow = value; NotifyPropertyChanged(); }
        }
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
        public ImpactArea.ImpactAreaElement ImpactAreaElement
        {
            get { return _ImpactAreaSet; }
            set { _ImpactAreaSet = value; NotifyPropertyChanged(); }
        }
        public ImpactArea.ImpactAreaRowItem ImpactArea
        {
            get { return _ImpactArea; }
            set { _ImpactArea = value; NotifyPropertyChanged(); }
        }
        public bool UseAnalyiticalFlowFrequency
        {
            get { return _UsesAnalyiticalFlowFrequency; }
            set { _UsesAnalyiticalFlowFrequency = value; NotifyPropertyChanged(); }
        }
        public FrequencyRelationships.AnalyticalFrequencyElement AnalyticalFlowFrequency
        {
            get { return _AnalyticalFlowFrequency; }
            set { _AnalyticalFlowFrequency = value; NotifyPropertyChanged(); }
        }
        public bool UseRatingCurve
        {
            get { return _UseRatingCurve; }
            set { _UseRatingCurve = value; NotifyPropertyChanged(); }
        }
        public StageTransforms.RatingCurveElement RatingCurveElement
        {
            get { return _RatingCurve; }
            set { _RatingCurve = value; NotifyPropertyChanged(); }
        }
        public bool UseAggregatedStageDamage
        {
            get { return _UseAggregatedStageDamage; }
            set { _UseAggregatedStageDamage = value; NotifyPropertyChanged(); }
        }
        public AggregatedStageDamage.AggregatedStageDamageElement StageDamageElement
        {
            get { return _StageDamage; }
            set { _StageDamage = value; NotifyPropertyChanged(); }
        }
        public bool UseThreshold
        {
            get { return _UseThreshold; }
            set { _UseThreshold = value; NotifyPropertyChanged(); }
        }
        public FdaModel.ComputationPoint.PerformanceThresholdTypes ThresholdType {
            get { return _ThresholdType; }
            set { _ThresholdType = value; NotifyPropertyChanged(); }
        }
        public double ThresholdValue
        {
            get { return _ThresholdValue; }
            set { _ThresholdValue = value; NotifyPropertyChanged(); }
        }

        
        public string FlowFreqName { get; set; }
        public string InfOutName { get; set; }
        public string RatingName { get; set; }
        public string ExtIntName { get; set; }
        public string LeveeName { get; set; }
        public string FailureFuncName { get; set; }
        public string StageDamageName { get; set; }


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
            ImpactAreaElement = elem.ImpactAreaElement;
            ImpactArea = elem.ImpactArea;

            UseAnalyiticalFlowFrequency = elem.UseAnalyiticalFlowFrequency;
            AnalyticalFlowFrequency = elem.AnalyticalFlowFrequency;

            UseInflowOutflow = elem.UseInflowOutflow;
            InflowOutflowElement = elem.InflowOutflowElement;

            UseRatingCurve = elem.UseRatingCurve;
            RatingCurveElement = elem.RatingCurveElement;

            UseExteriorInteriorStage = elem.UseExteriorInteriorStage;
            ExteriorInteriorElement = elem.ExteriorInteriorElement;

            UseLevee = elem.UseLevee;
            LeveeElement = elem.LeveeElement;

            UseFailureFunction = elem.UseFailureFunction;
            FailureFunctionElement = elem.FailureFunctionElement;

            UseAggregatedStageDamage = elem.UseAggregatedStageDamage;
            StageDamageElement = elem.StageDamageElement;

            UseThreshold = elem.UseThreshold;
            ThresholdType = elem.ThresholdType;
            ThresholdValue = elem.ThresholdValue;

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
        public ConditionsElement(string name, string description, int analysisYear, ImpactArea.ImpactAreaElement impactAreaElement, ImpactArea.ImpactAreaRowItem indexLocation,
            bool usesAnalyiticalFlowFrequency, FrequencyRelationships.AnalyticalFrequencyElement aFlowFreq, bool usesInflowOutflow, FlowTransforms.InflowOutflowElement inflowOutflowElement,
            bool useRating, StageTransforms.RatingCurveElement rc, bool useIntExtStage, StageTransforms.ExteriorInteriorElement extInt, bool useLevee, GeoTech.LeveeFeatureElement leveeElement,
            bool useFailureFunction, GeoTech.FailureFunctionElement failureFunctionElement, bool useAggStageDamage, AggregatedStageDamage.AggregatedStageDamageElement stageDamage,
            bool useThreshold, FdaModel.ComputationPoint.PerformanceThresholdTypes thresholdType, double thresholdValue ) : base()
        {
            Name = name;
           // _ConditionsOwnerElement = owner;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/Condition.png");

            Description = description;
            AnalysisYear = analysisYear;
            ImpactAreaElement = impactAreaElement;
            ImpactArea = indexLocation;

            UseAnalyiticalFlowFrequency = usesAnalyiticalFlowFrequency;
            AnalyticalFlowFrequency = aFlowFreq;

            UseInflowOutflow = usesInflowOutflow;
            InflowOutflowElement = inflowOutflowElement;

            UseRatingCurve = useRating;
            RatingCurveElement = rc;

            UseExteriorInteriorStage = useIntExtStage;
            ExteriorInteriorElement = extInt;

            UseLevee = useLevee;
            LeveeElement = leveeElement;

            UseFailureFunction = useFailureFunction;
            FailureFunctionElement = failureFunctionElement;

            UseAggregatedStageDamage = useAggStageDamage;
            StageDamageElement = stageDamage;

            UseThreshold = useThreshold;
            ThresholdType = thresholdType;
            ThresholdValue = thresholdValue;

            Utilities.NamedAction edit = new Utilities.NamedAction();
            edit.Header = "Edit Condition";
            edit.Action = EditCondition;

            Utilities.NamedAction compute = new Utilities.NamedAction();
            compute.Header = "Compute Condition";
            compute.Action = ComputeCondition;

            Utilities.NamedAction removeCondition = new Utilities.NamedAction();
            removeCondition.Header = "Remove";
            removeCondition.Action = RemoveElement;

            Utilities.NamedAction renameElement = new Utilities.NamedAction(this);
            renameElement.Header = "Rename";
            renameElement.Action = Rename;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(edit);
            localActions.Add(compute);
            localActions.Add(removeCondition);
            localActions.Add(renameElement);

            Actions = localActions;
            _ConditionsTreeNodes = new List<BaseFdaElement>() { AnalyticalFlowFrequency,inflowOutflowElement};

        }
       
        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetConditionsManager().Remove(this);
        }

        #region BuildControlsFromElement
        private Plots.IndividualLinkedPlotControlVM BuildLP3ControlFromElement()
        {
            List<FrequencyRelationships.AnalyticalFrequencyElement> listOfLp3 = StudyCache.GetChildElementsOfType<FrequencyRelationships.AnalyticalFrequencyElement>();// StudyCache.FlowFrequencyElements;// GetElementsOfType<FrequencyRelationships.AnalyticalFrequencyElement>();
                FdaModel.Functions.FrequencyFunctions.LogPearsonIII lp3 = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(AnalyticalFlowFrequency.Distribution, FdaModel.Functions.FunctionTypes.InflowFrequency);
                Statistics.CurveIncreasing curve = lp3.GetOrdinatesFunction().Function;
                Plots.IndividualLinkedPlotVM plotVM = new Plots.IndividualLinkedPlotVM(lp3, curve, AnalyticalFlowFrequency.Name);
                Plots.ConditionsIndividualPlotWrapperVM plotWrapper = new Plots.ConditionsIndividualPlotWrapperVM(true, true, "LP3", "Probability", "Inflow");
                plotWrapper.PlotVM = plotVM;
                AddFlowFrequencyToConditionVM importer = new AddFlowFrequencyToConditionVM(listOfLp3, AnalyticalFlowFrequency);
                importer.RequestNavigation += Navigate;
                return new Plots.IndividualLinkedPlotControlVM(plotWrapper, new Plots.IndividualLinkedPlotCoverButtonVM("Flow Frequency Curve"), importer);
        }

        private Plots.IndividualLinkedPlotControlVM BuildInflowOutflowControlFromElement()
        {
            List<FlowTransforms.InflowOutflowElement> listOfInfOut = StudyCache.GetChildElementsOfType<FlowTransforms.InflowOutflowElement>();
            UncertainCurveDataCollection curve = InflowOutflowElement.Curve;
            FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction infOut =
                new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((UncertainCurveIncreasing)curve, FunctionTypes.InflowOutflow);

            Plots.IndividualLinkedPlotVM plotVM = new Plots.IndividualLinkedPlotVM(infOut, infOut.GetOrdinatesFunction().Function, InflowOutflowElement.Name);

            Plots.ConditionsIndividualPlotWrapperVM plotWrapper = new Plots.ConditionsIndividualPlotWrapperVM(true, false, "Inflow Outflow", "Inflow", "OutFlow");
            plotWrapper.PlotVM = plotVM;
            AddInflowOutflowToConditionVM importer = new AddInflowOutflowToConditionVM(listOfInfOut, InflowOutflowElement);
            importer.RequestNavigation += Navigate;
            return new Plots.IndividualLinkedPlotControlVM(plotWrapper, new Plots.IndividualLinkedPlotCoverButtonVM("Inflow Outflow"), importer,
                new Plots.DoubleLineModulatorCoverButtonVM(),
                new Plots.DoubleLineModulatorWrapperVM(plotVM));
        }

        private Plots.IndividualLinkedPlotControlVM BuildRatingControlFromElement()
        {
            List<StageTransforms.RatingCurveElement> listOfRatingCurves = StudyCache.GetChildElementsOfType<StageTransforms.RatingCurveElement>();
            UncertainCurveDataCollection curve = RatingCurveElement.Curve;
            FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction ratFunc =
                new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((UncertainCurveIncreasing)curve, FunctionTypes.Rating);

            Plots.IndividualLinkedPlotVM plotVM = new Plots.IndividualLinkedPlotVM(ratFunc, ratFunc.GetOrdinatesFunction().Function, RatingCurveElement.Name);

            Plots.ConditionsIndividualPlotWrapperVM plotWrapper = new Plots.ConditionsIndividualPlotWrapperVM(true, false, "Rating", "Exterior Stage", "OutFlow");
            plotWrapper.PlotVM = plotVM;
            AddRatingCurveToConditionVM importer = new AddRatingCurveToConditionVM(listOfRatingCurves, RatingCurveElement);
            //importer.RequestNavigation += Navigate;
           // Navigate(importer, false, false, "mytest");
            return new Plots.IndividualLinkedPlotControlVM(plotWrapper, new Plots.IndividualLinkedPlotCoverButtonVM("Rating Curve"), importer);
        }

        private Plots.IndividualLinkedPlotControlVM BuildExtIntControlFromElement()
        {
            List<StageTransforms.ExteriorInteriorElement> listOfExtIntElements = StudyCache.GetChildElementsOfType<StageTransforms.ExteriorInteriorElement>();
            UncertainCurveDataCollection curve = ExteriorInteriorElement.Curve;
            FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction extIntCurve =
                new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((UncertainCurveIncreasing)curve, FunctionTypes.ExteriorInteriorStage);

            Plots.IndividualLinkedPlotVM plotVM = new Plots.IndividualLinkedPlotVM(extIntCurve, extIntCurve.GetOrdinatesFunction().Function, ExteriorInteriorElement.Name);

            Plots.ConditionsIndividualPlotWrapperVM plotWrapper = new Plots.ConditionsIndividualPlotWrapperVM(true, false, "Exterior Interior Stage", "Exterior Stage", "Interior Stage");
            plotWrapper.PlotVM = plotVM;
            AddExteriorInteriorStageToConditionVM importer = new AddExteriorInteriorStageToConditionVM(listOfExtIntElements, ExteriorInteriorElement);
            importer.RequestNavigation += Navigate;
            return new Plots.IndividualLinkedPlotControlVM(plotWrapper, new Plots.IndividualLinkedPlotCoverButtonVM("Ext Int Stage Curve"),importer,
                new Plots.DoubleLineModulatorHorizontalCoverButtonVM(),
                new Plots.HorizontalDoubleLineModulatorWrapperVM(plotVM));
        }
        private Plots.IndividualLinkedPlotControlVM BuildStageDamageControlFromElement()
        {
            List<AggregatedStageDamage.AggregatedStageDamageElement> listOfStageDamage = StudyCache.GetChildElementsOfType<AggregatedStageDamage.AggregatedStageDamageElement>();
            UncertainCurveDataCollection curve = StageDamageElement.Curve;
            FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction stageDamageCurve =
                new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((UncertainCurveIncreasing)curve, FunctionTypes.InteriorStageDamage);

            Plots.IndividualLinkedPlotVM plotVM = new Plots.IndividualLinkedPlotVM(stageDamageCurve, stageDamageCurve.GetOrdinatesFunction().Function, StageDamageElement.Name);

            Plots.ConditionsIndividualPlotWrapperVM plotWrapper = new Plots.ConditionsIndividualPlotWrapperVM(true, false, "Stage Damage", "Interior Stage", "Damage");
            plotWrapper.PlotVM = plotVM;
            AddStageDamageToConditionVM importer = new AddStageDamageToConditionVM(listOfStageDamage, StageDamageElement);
            return new Plots.IndividualLinkedPlotControlVM(plotWrapper, new Plots.IndividualLinkedPlotCoverButtonVM("Int Stage Damage Curve"), importer);
        }
        #endregion
        public void EditCondition(object arg1, EventArgs arg2)
        {
            List<ImpactArea.ImpactAreaElement> impactAreas = StudyCache.GetChildElementsOfType<ImpactArea.ImpactAreaElement>();
            List<FrequencyRelationships.AnalyticalFrequencyElement> freqeles = StudyCache.GetChildElementsOfType<FrequencyRelationships.AnalyticalFrequencyElement>();
            List<FlowTransforms.InflowOutflowElement> inflowOutflowList = StudyCache.GetChildElementsOfType<FlowTransforms.InflowOutflowElement>();
            List<StageTransforms.RatingCurveElement> ratingeles = StudyCache.GetChildElementsOfType<StageTransforms.RatingCurveElement>();

            List<StageTransforms.ExteriorInteriorElement> extIntList = StudyCache.GetChildElementsOfType<StageTransforms.ExteriorInteriorElement>();
            List<GeoTech.LeveeFeatureElement> leveeList = StudyCache.GetChildElementsOfType<GeoTech.LeveeFeatureElement>();
            List<GeoTech.FailureFunctionElement> failureFunctionList = StudyCache.GetChildElementsOfType<GeoTech.FailureFunctionElement>();


            List<AggregatedStageDamage.AggregatedStageDamageElement> damageles = StudyCache.GetChildElementsOfType<AggregatedStageDamage.AggregatedStageDamageElement>();
            //if (impactAreas.Count == 0)
            //{
            //    ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("No Impact Area Sets have been defined.", FdaModel.Utilities.Messager.ErrorMessageEnum.Report | FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel));
            //    return;
            //}

            //////////////////////////////

            Plots.IndividualLinkedPlotControlVM lp3Control;
            Plots.IndividualLinkedPlotControlVM infOutControl;
            Plots.IndividualLinkedPlotControlVM ratingControl;
            Plots.IndividualLinkedPlotControlVM extIntStageControl;
            Plots.IndividualLinkedPlotControlVM stageDamageControl;
            Plots.IndividualLinkedPlotControlVM damageFrequencyControl;

            if (UseAnalyiticalFlowFrequency)
            {
                lp3Control = BuildLP3ControlFromElement();
                lp3Control.RequestNavigation += Navigate;
            }
            else
            {
                lp3Control = ConditionsOwnerElement.BuildDefaultLP3Control(StudyCache.GetParentElementOfType<Conditions.ConditionsOwnerElement>());
            }

            if (UseInflowOutflow)
            {
                infOutControl = BuildInflowOutflowControlFromElement();
            }
            else
            {
                infOutControl = ConditionsOwnerElement.BuildDefaultInflowOutflowControl(StudyCache.GetParentElementOfType<Conditions.ConditionsOwnerElement>());
            }

            if (UseRatingCurve)
            {
                ratingControl = BuildRatingControlFromElement();
                ratingControl.RequestNavigation += Navigate;
            }
            else
            {
                ratingControl = ConditionsOwnerElement.BuildDefaultRatingControl(StudyCache.GetParentElementOfType<Conditions.ConditionsOwnerElement>());
            }

            if (UseExteriorInteriorStage)
            {
                extIntStageControl = BuildExtIntControlFromElement();
            }
            else
            {
                extIntStageControl = ConditionsOwnerElement.BuildDefaultExtIntStageControl(StudyCache.GetParentElementOfType<Conditions.ConditionsOwnerElement>());
            }

            if (_UseAggregatedStageDamage)
            {
                stageDamageControl = BuildStageDamageControlFromElement();
            }
            else
            {
                stageDamageControl = ConditionsOwnerElement.BuildDefaultStageDamageControl(StudyCache.GetParentElementOfType<Conditions.ConditionsOwnerElement>());
            }

            damageFrequencyControl = ConditionsOwnerElement.BuildDefaultDamageFrequencyControl(StudyCache.GetParentElementOfType<Conditions.ConditionsOwnerElement>());



            //    Plots.IndividualLinkedPlotControlVM inflowOutflowControl = new Plots.IndividualLinkedPlotControlVM(new Plots.ConditionsIndividualPlotWrapperVM(true, false, "Inflow Outflow", "Inflow", "OutFlow"), new Plots.IndividualLinkedPlotCoverButtonVM("Inflow Outflow"), new AddInflowOutflowToConditionVM(listOfInfOut, this), new Plots.DoubleLineModulatorCoverButtonVM(), new Plots.DoubleLineModulatorWrapperVM());
            //Plots.IndividualLinkedPlotControlVM ratingControl = new Plots.IndividualLinkedPlotControlVM(new Plots.ConditionsIndividualPlotWrapperVM(true, false, "Rating", "Exterior Stage", "OutFlow"), new Plots.IndividualLinkedPlotCoverButtonVM("Rating Curve"), new AddRatingCurveToConditionVM(ratingeles, this));
            //Plots.IndividualLinkedPlotControlVM extIntStageControl = new Plots.IndividualLinkedPlotControlVM(new Plots.ConditionsIndividualPlotWrapperVM(true, false, "Exterior Interior Stage", "Exterior Stage", "Interior Stage"), new Plots.IndividualLinkedPlotCoverButtonVM("Ext Int Stage Curve"), new AddExteriorInteriorStageToConditionVM(listOfExtIntElements, this), new Plots.DoubleLineModulatorHorizontalCoverButtonVM(), new Plots.HorizontalDoubleLineModulatorWrapperVM());
            //Plots.IndividualLinkedPlotControlVM StageDamageControl = new Plots.IndividualLinkedPlotControlVM(new Plots.ConditionsIndividualPlotWrapperVM(true, false, "Stage Damage", "Interior Stage", "Damage"), new Plots.IndividualLinkedPlotCoverButtonVM("Int Stage Damage Curve"), new AddStageDamageToConditionVM(listOfStageDamage, this));
            //Plots.IndividualLinkedPlotControlVM DamageFrequencyControl = new Plots.IndividualLinkedPlotControlVM(new Plots.ConditionsIndividualPlotWrapperVM(true, true, "Damage Frequency", "Probability", "Damage", false), new Plots.IndividualLinkedPlotCoverButtonVM("Preview Compute"), null);


            // ConditionsPlotEditorVM vm = new ConditionsPlotEditorVM(impactAreas, lp3Control, infOutControl, ratingControl, extIntStageControl, stageDamageControl, damageFrequencyControl,
            //   UseAnalyiticalFlowFrequency, UseInflowOutflow, UseRatingCurve,UseExteriorInteriorStage,UseAggregatedStageDamage);


            


            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                 .WithSiblingRules(this)
               .WithParentGuid(this.GUID)
               .WithCanOpenMultipleTimes(false);

            ConditionsPlotEditorVM vm = new ConditionsPlotEditorVM(impactAreas, lp3Control, infOutControl, ratingControl, extIntStageControl, stageDamageControl, damageFrequencyControl, this, actionManager);
            ///////////////////////////////////////


            //ConditionsEditorVM vm = new ConditionsEditorVM(Name,Description, AnalysisYear, impactAreas,ImpactAreaSet,ImpactArea, UseAnalyiticalFlowFrequency, freqeles, AnalyticalFlowFrequency, UseInflowOutflow, inflowOutflowList,InflowOutflowElement,UseRatingCurve, ratingeles,RatingCurve, UseExteriorInteriorStage, extIntList,ExteriorInteriorElement,UseLevee,leveeList,LeveeElement,UseFailureFunction,failureFunctionList,FailureFunctionElement,UseAggregatedStageDamage, damageles, StageDamage,UseThreshold,ThresholdType,ThresholdValue, (ConditionsOwnerElement)_Owner);
            Navigate( vm, false,false,"Edit " + vm.Name);
            //if (!vm.WasCanceled)
            //{
            //    if (!vm.HasError)
            //    {
            //        string oldName = Name;
            //        //its just easier to create a new one and then copy the values from that one to this one
            //        ConditionsElement newElem = ConditionFactory.BuildConditionsElement(vm, _ConditionsOwnerElement);
            //        ConditionFactory.CopyConditionsElement(newElem, this);

            //        ((ConditionsOwnerElement)_Owner).UpdateTableRowIfModified((ParentElement)_Owner, oldName, this);
                   
            //    }
            //}
        }

        private void ComputeCondition(object arg1, EventArgs arg2)
        {
            //convert to model types, run model compute, show results in window.
            //ImpactArea.Name
            //AnalyticalFlowFrequency.Distribution;
            //RatingCurve.RatingCurve //isvalid //distribution //getx gety
            //StageDamage.Curve same as above.
            //convert to model objects
            //compute model objects
            //take result and pass to results viewmodel
            List<FdaModel.Functions.BaseFunction> listOfBaseFunctions = new List<FdaModel.Functions.BaseFunction>();
            FdaModel.Functions.FrequencyFunctions.LogPearsonIII LP3 = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(AnalyticalFlowFrequency.Distribution, FdaModel.Functions.FunctionTypes.InflowFrequency);

            listOfBaseFunctions.Add(LP3);

            //FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction inflowOutflow = new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((Statistics.UncertainCurveIncreasing)InflowOutflowElement.InflowOutflowCurve, FdaModel.Functions.FunctionTypes.InflowOutflow);

           // listOfBaseFunctions.Add(inflowOutflow);

            FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction rating = new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((Statistics.UncertainCurveIncreasing)RatingCurveElement.Curve, FdaModel.Functions.FunctionTypes.Rating);

            listOfBaseFunctions.Add(rating);

            //FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction extIntStage = new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((Statistics.UncertainCurveIncreasing)ExteriorInteriorElement.ExteriorInteriorCurve, FdaModel.Functions.FunctionTypes.ExteriorInteriorStage);

            //listOfBaseFunctions.Add(extIntStage);

            FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction stageDamage = new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((Statistics.UncertainCurveIncreasing)StageDamageElement.Curve, FdaModel.Functions.FunctionTypes.InteriorStageDamage);

            listOfBaseFunctions.Add(stageDamage);

            FdaModel.ComputationPoint.PerformanceThreshold threshold;

                //ThresholdValue = 120;
            //when i do the real compute I will need to handle every threshold type
            if (ThresholdType == FdaModel.ComputationPoint.PerformanceThresholdTypes.InteriorStage)
            {
                threshold = new FdaModel.ComputationPoint.PerformanceThreshold(FdaModel.ComputationPoint.PerformanceThresholdTypes.InteriorStage, ThresholdValue);

            }
            else // ThresholdType == "Dollars"
            {
                threshold = new FdaModel.ComputationPoint.PerformanceThreshold(FdaModel.ComputationPoint.PerformanceThresholdTypes.Damage, ThresholdValue);

            }


            FdaModel.ComputationPoint.Condition condition = new FdaModel.ComputationPoint.Condition(AnalysisYear, ImpactAreaElement.Name, listOfBaseFunctions, threshold, null); //this constructor will call Validate

            //FdaModel.ComputationPoint.Outputs.Realization realization = new FdaModel.ComputationPoint.Outputs.Realization(condition, false, false);
            //Random randomNumGenerator = new Random(0);
            //realization.Compute(randomNumGenerator);

            FdaModel.ComputationPoint.Outputs.Result result = new FdaModel.ComputationPoint.Outputs.Result(condition, 10);

            List<string> selectedElementNames = new List<string>();
            selectedElementNames.Add(AnalyticalFlowFrequency.Name);
            selectedElementNames.Add(RatingCurveElement.Name);
            selectedElementNames.Add(ExteriorInteriorElement.Name);
            selectedElementNames.Add(StageDamageElement.Name);
            selectedElementNames.Add("Computed Stage Frequency");
            // write out results for testing purposes.
            Plots.LinkedPlotsVM vem = new Plots.LinkedPlotsVM(result, ThresholdType, ThresholdValue, selectedElementNames);
            Navigate( vem, false, false, "Computed Condition");
            //Output.LinkedPlotsVM vm = new Output.LinkedPlotsVM(result);
            //Navigate(vm);
        }
        #endregion
        #region Voids
        private void LoadTheTreeNodes()
        {
            _ConditionsTreeNodes = new List<BaseFdaElement>();
            if(ImpactAreaElement != null)
            {
                _ConditionsTreeNodes.Add(ImpactAreaElement);
            }
            if (AnalyticalFlowFrequency != null)
            {
                _ConditionsTreeNodes.Add(AnalyticalFlowFrequency);
            }
            if (InflowOutflowElement != null)
            {
                _ConditionsTreeNodes.Add(InflowOutflowElement);
            }
            if (RatingCurveElement != null)
            {
                _ConditionsTreeNodes.Add(RatingCurveElement);
            }
            if (ExteriorInteriorElement != null)
            {
                _ConditionsTreeNodes.Add(ExteriorInteriorElement);
            }
            if (StageDamageElement != null)
            {
                _ConditionsTreeNodes.Add(StageDamageElement);
            }
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




        #endregion
        #region Functions
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            ConditionsElement elem = (ConditionsElement)elementToClone;
            return new ConditionsElement(elem);
        }
        #endregion



    }
}
