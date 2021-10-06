using ViewModel.AggregatedStageDamage;
using ViewModel.FlowTransforms;
using ViewModel.FrequencyRelationships;
using ViewModel.GeoTech;
using ViewModel.ImpactArea;
using ViewModel.StageTransforms;
using ViewModel.Utilities;
using Model;
using Model.Conditions.Locations.Years.Results;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ViewModel.ImpactAreaScenario.Results;

namespace ViewModel.ImpactAreaScenario
{
    //todo: is fontsize and bold properties being used?
    //todo: i think i need to determine if this is an inflow freq or an outflow freq.
    //right now i am just assuming this is an inflow freq.
    //todo: do i want a "has computed" property so that we can distinguish if a condition has been computed or not? Like maybe different
    //symbology in the tree? Maybe notify the user if recomputing that it will clobber the old results.
    //todo: I will need to make some minor changes to store multiple thresholds.

    /// <summary>
    /// The Conditions element is the in-memory object that holds all the information about a condition.
    /// </summary>
    public class IASElement : ChildElement
    {
        #region Notes
        #endregion
        #region Fields
        public event EventHandler EditConditionsTreeElement;
        public event EventHandler RemoveConditionsTreeElement;
        public event EventHandler RenameConditionsTreeElement;
        public event EventHandler UpdateExpansionValueInTreeElement;

        private string _Description;
        private int _AnalysisYear;
        
        private IMetricEnum _MetricType;
        private double _ThresholdValue;
        private List<BaseFdaElement> _ConditionsTreeNodes;
        private bool _IsExpanded;

        private NamedAction _ViewResults = new NamedAction();
        #endregion
        #region Properties

        /// <summary>
        /// These are the results after doing a compute. If a compute has not been
        /// done, then this will be null.
        /// </summary>
        public IConditionLocationYearResult ComputeResults { get; set; }

        /// <summary>
        /// This is the condition's editor. If it isn't null, then it is open. If it is
        /// open and something in FDA changes that the editor uses, then the editor can be updated.
        /// </summary>
        public IASPlotEditorVM ConditionsEditor { get; set; }

        /// <summary>
        /// The impact area ID for the selected impact area. It will be -1 if no selection was made.
        /// </summary>
        public int ImpactAreaID { get; set; }
        /// <summary>
        /// The flow freq ID for the selected flow freq. It will be -1 if no selection was made.
        /// </summary>
        public int FlowFreqID { get; set; }
        /// <summary>
        /// The inflow outflow ID for the selected inflow outflow. It will be -1 if no selection was made.
        /// </summary>
        public int InflowOutflowID { get; set; }
        /// <summary>
        /// The rating ID for the selected rating. It will be -1 if no selection was made.
        /// </summary>
        public int RatingID { get; set; }
        /// <summary>
        /// The levee failure ID for the selected levee failure. It will be -1 if no selection was made.
        /// </summary>
        public int LeveeFailureID { get; set; }
        /// <summary>
        /// The exterior interior stage ID for the selected ext int stage. It will be -1 if no selection was made.
        /// </summary>
        public int ExtIntStageID { get; set; }
        /// <summary>
        /// The stage damage ID for the selected stage damage. It will be -1 if no selection was made.
        /// </summary>
        public int StageDamageID { get; set; }


        //public bool HasComputed { get; set; }
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

        /// <summary>
        /// This is the list of sub elements that will display in the Conditions tree under this condition.
        /// </summary>
        public List<BaseFdaElement> ConditionsTreeNodes
        {
            get { return _ConditionsTreeNodes; }
            set { _ConditionsTreeNodes = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// The description of the condition
        /// </summary>
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }
        /// <summary>
        /// The analysis year of the condition
        /// </summary>
        public int AnalysisYear
        {
            get { return _AnalysisYear; }
            set { _AnalysisYear = value; NotifyPropertyChanged(); }
        }
        
        /// <summary>
        /// The selected threshold type.
        /// </summary>
        public IMetricEnum ThresholdType {
            get { return _MetricType; }
            set { _MetricType = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// The value for the selected threshold.
        /// </summary>
        public double ThresholdValue
        {
            get { return _ThresholdValue; }
            set { _ThresholdValue = value; NotifyPropertyChanged(); }
        }

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
        /// <param name="elem">The condition element</param>
        public IASElement(IASElement elem) : base()
        {
            IsExpanded = elem.IsExpanded;
            Name = elem.Name;
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
            ComputeResults = elem.ComputeResults;           

            NamedAction edit = new NamedAction();
            edit.Header = "Edit Impact Area Scenario...";
            edit.Action = EditConditionsTreeElem;

            NamedAction additionalThresholds = new NamedAction();
            additionalThresholds.Header = "Additional Thresholds...";
            additionalThresholds.Action = AdditionalThresholds;

            NamedAction compute = new NamedAction();
            compute.Header = "Compute Impact Area Scenario";
            compute.Action = ComputeCondition;

            _ViewResults.Header = "View Results...";
            _ViewResults.Action = ViewResults;

            NamedAction removeCondition = new NamedAction();
            removeCondition.Header = "Remove";
            removeCondition.Action = RemoveConditionsTreeElem;

            NamedAction renameElement = new NamedAction();
            renameElement.Header = "Rename";
            renameElement.Action = RenameConditionsTreeElem;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(edit);
            localActions.Add(compute);
            localActions.Add(_ViewResults);
            localActions.Add(additionalThresholds);
            localActions.Add(removeCondition);
            localActions.Add(renameElement);

            Actions = localActions;

            LoadTheTreeNodes();

        }

        private void AdditionalThresholds(object arg1, EventArgs arg2)
        {
            AdditionalThresholdsVM vm = new AdditionalThresholdsVM();
            string header = "Annual Exceedance Probabilities Thresholds";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "additionalThresholds");
            Navigate(tab, false, false);
        }

        /// <summary>
        /// The constructor for a conditions element.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="analysisYear"></param>
        /// <param name="impactAreaID"></param>
        /// <param name="flowFreqID"></param>
        /// <param name="inflowOutflowID"></param>
        /// <param name="ratingID"></param>
        /// <param name="extIntID"></param>
        /// <param name="leveeFailureID"></param>
        /// <param name="stageDamageID"></param>
        /// <param name="thresholdType"></param>
        /// <param name="thresholdValue"></param>
        public IASElement(string name, string description, int analysisYear, int impactAreaID,
                     int flowFreqID, int inflowOutflowID, int ratingID, int extIntID, int leveeFailureID, int stageDamageID,
                   IMetricEnum thresholdType, double thresholdValue) : base()
        {
            Name = name;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/Condition.png");

            Description = description;
            AnalysisYear = analysisYear;
            ImpactAreaID = impactAreaID;

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

            _ViewResults.Header = "View Results";
            _ViewResults.Action = ViewResults;

            NamedAction removeCondition = new Utilities.NamedAction();
            removeCondition.Header = "Remove";
            removeCondition.Action = RemoveElement;

            NamedAction renameElement = new Utilities.NamedAction(this);
            renameElement.Header = "Rename";
            renameElement.Action = Rename;

            List<NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(edit);
            localActions.Add(compute);
            localActions.Add(_ViewResults);
            localActions.Add(removeCondition);
            localActions.Add(renameElement);

            Actions = localActions;
        }
       
        /// <summary>
        /// Deletes a conditions element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetIASManager().Remove(this);
        }

        #region BuildControlsFromElement
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
            Plots.IASIndividualPlotWrapperVM plotWrapper = new Plots.IASIndividualPlotWrapperVM(false, true, true, false, false, false);
            plotWrapper.PlotVM = plotVM;
            AddFlowFrequencyToIASVM importer = new AddFlowFrequencyToIASVM(listOfLp3, selectedElem);
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

            Plots.IASIndividualPlotWrapperVM plotWrapper = new Plots.IASIndividualPlotWrapperVM(true,true,false,false,true,true);
            plotWrapper.PlotVM = plotVM;
            AddInflowOutflowToIASVM importer = new AddInflowOutflowToIASVM(listOfInfOut, selectedElem);
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

            Plots.IASIndividualPlotWrapperVM plotWrapper = new Plots.IASIndividualPlotWrapperVM(false,true,false,false,false,true);
            plotWrapper.PlotVM = plotVM;
            AddRatingCurveToIASVM importer = new AddRatingCurveToIASVM(listOfRatingCurves, selectedElem);
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

            Plots.IASIndividualPlotWrapperVM plotWrapper = new Plots.IASIndividualPlotWrapperVM(false,false,false,true,true,true);
            plotWrapper.PlotVM = plotVM;
            AddFailureFunctionToIASVM importer = new AddFailureFunctionToIASVM(listOfLeveeFeatureElements, selectedElem);
            return new Plots.IndividualLinkedPlotControlVM(IFdaFunctionEnum.LateralStructureFailure, plotWrapper, new Plots.IndividualLinkedPlotCoverButtonVM("Lateral Structure"), importer,
                new Plots.DoubleLineModulatorHorizontalCoverButtonVM("Lateral Structure"),
                new Plots.IASHorizontalFailureFunctionVM(plotVM));
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

            Plots.IASIndividualPlotWrapperVM plotWrapper = new Plots.IASIndividualPlotWrapperVM(false,false,false,false,true,true);
            plotWrapper.PlotVM = plotVM;
            AddExteriorInteriorStageToIASVM importer = new AddExteriorInteriorStageToIASVM(listOfExtIntElements, selectedElem);
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

            Plots.IASIndividualPlotWrapperVM plotWrapper = new Plots.IASIndividualPlotWrapperVM(false,true,false,false,true,true);
            plotWrapper.PlotVM = plotVM;
            AddStageDamageToIASVM importer = new AddStageDamageToIASVM(listOfStageDamage, selectedElem);
            return new Plots.IndividualLinkedPlotControlVM(IFdaFunctionEnum.InteriorStageDamage, plotWrapper, new Plots.IndividualLinkedPlotCoverButtonVM("Int Stage Damage Curve"), importer);
        }
        #endregion

        /// <summary>
        /// Opens the conditions editor.
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
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
                lp3Control = IASOwnerElement.BuildDefaultLP3Control(StudyCache.GetParentElementOfType<ImpactAreaScenario.IASOwnerElement>());
            }

            if (InflowOutflowID != -1)
            {
                infOutControl = BuildInflowOutflowControlFromElement(InflowOutflowID);
            }
            else
            {
                infOutControl = IASOwnerElement.BuildDefaultInflowOutflowControl(StudyCache.GetParentElementOfType<ImpactAreaScenario.IASOwnerElement>());
            }

            if (RatingID != -1)
            {
                ratingControl = BuildRatingControlFromElement(RatingID);
            }
            else
            {
                ratingControl = IASOwnerElement.BuildDefaultRatingControl(StudyCache.GetParentElementOfType<ImpactAreaScenario.IASOwnerElement>());
            }

            if(LeveeFailureID != -1)
            {
                leveeFailureControl = BuildLeveeFailureFromElement(LeveeFailureID);
            }
            else
            {
                leveeFailureControl = IASOwnerElement.BuildDefaultLateralFeaturesControl(StudyCache.GetParentElementOfType<IASOwnerElement>());
            }

            if (ExtIntStageID != -1)
            {
                extIntStageControl = BuildExtIntControlFromElement(ExtIntStageID);
            }
            else
            {
                extIntStageControl = IASOwnerElement.BuildDefaultExtIntStageControl(StudyCache.GetParentElementOfType<ImpactAreaScenario.IASOwnerElement>());
            }

            if (StageDamageID != -1)
            {
                stageDamageControl = BuildStageDamageControlFromElement(StageDamageID);
            }
            else
            {
                stageDamageControl = IASOwnerElement.BuildDefaultStageDamageControl(StudyCache.GetParentElementOfType<ImpactAreaScenario.IASOwnerElement>());
            }

            damageFrequencyControl = IASOwnerElement.BuildDefaultDamageFrequencyControl(StudyCache.GetParentElementOfType<ImpactAreaScenario.IASOwnerElement>());

            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                 .WithSiblingRules(this);

            ConditionsEditor = new IASPlotEditorVM(impactAreas, lp3Control, infOutControl, ratingControl, leveeFailureControl, extIntStageControl,
                stageDamageControl, damageFrequencyControl, this, actionManager);

            ConditionsEditor.RequestNavigation += Navigate;
            string header = "Create Condition";
            DynamicTabVM tab = new DynamicTabVM(header, ConditionsEditor, "CreateCondition");
            Navigate(tab, false, false);
        }


        private void DisplayResults(IConditionLocationYearResult result)
        {
            IASResultsVM resultViewer = new IASResultsVM(Name);          
            string header = "Results";
            DynamicTabVM tab = new DynamicTabVM(header, resultViewer, "resultViewer");
            Navigate(tab, false, false);
        }
        private void ViewResults(object arg1, EventArgs arg2)
        {
            DisplayResults(ComputeResults);
        }

        /// <summary>
        /// This will not grab the levee failure, only inflow-outflow and ext-int stage.
        /// </summary>
        /// <returns></returns>
        private List<ITransformFunction> GetTransformFunctions()
        {
            List<ITransformFunction> transforms = new List<ITransformFunction>();
            bool hasInflowOutflow = InflowOutflowID != -1;
            bool hasLeveeFailure = LeveeFailureID != -1;
            bool hasExtInt = ExtIntStageID != -1;
            try
            {
                //required params
                RatingCurveElement ratingElement = (RatingCurveElement)StudyCache.GetChildElementOfType(typeof(RatingCurveElement), RatingID);
                AggregatedStageDamageElement stageDamageElement = (AggregatedStageDamageElement)StudyCache.GetChildElementOfType(typeof(AggregatedStageDamageElement), StageDamageID);

                //possible other functions
                InflowOutflowElement inflowOutflowElement = null;
                ExteriorInteriorElement extIntElement = null;
                if (hasInflowOutflow)
                {
                    inflowOutflowElement = (InflowOutflowElement)StudyCache.GetChildElementOfType(typeof(InflowOutflowElement), InflowOutflowID);
                }
                if (hasExtInt)
                {
                    extIntElement = (ExteriorInteriorElement)StudyCache.GetChildElementOfType(typeof(ExteriorInteriorElement), ExtIntStageID);
                }

                if (hasInflowOutflow)
                {
                    transforms.Add((ITransformFunction)inflowOutflowElement.Curve);
                }
                transforms.Add((ITransformFunction)ratingElement.Curve);

                if (hasExtInt)
                {
                    transforms.Add((ITransformFunction)extIntElement.Curve);
                }
                transforms.Add((ITransformFunction)stageDamageElement.Curve);
            }
            catch(Exception e)
            {
                //todo: do something else here?
                return transforms;
            }
            return transforms;
        }

        private IFrequencyFunction GetFrequencyFunction()
        {
            //todo: i think i need to determine if this is an inflow freq or an outflow freq.
            //right now i am just assuming this is an inflow freq.
            AnalyticalFrequencyElement flowFreqElement = (AnalyticalFrequencyElement)StudyCache.GetChildElementOfType(typeof(AnalyticalFrequencyElement), FlowFreqID);
            return (IFrequencyFunction)flowFreqElement.Curve;
        }

        private void ComputeCondition(object arg1, EventArgs arg2)
        {

            EnterSeedVM enterSeedVM = new EnterSeedVM();
            string header = "Enter Seed Value";
            DynamicTabVM tab = new DynamicTabVM(header, enterSeedVM, "EnterSeed");
            Navigate(tab, true, true);

            int seedValue = enterSeedVM.Seed;

            IConditionLocationYearSummary condition = null;

            IFrequencyFunction frequencyFunction = GetFrequencyFunction();
            List<ITransformFunction> transformFunctions = GetTransformFunctions();

            bool hasLeveeFailure = LeveeFailureID != -1;
            if (hasLeveeFailure)
            {
                LeveeFeatureElement leveeFailureElement = (LeveeFeatureElement)StudyCache.GetChildElementOfType(typeof(LeveeFeatureElement), LeveeFailureID);
                ILateralStructure latStruct = ILateralStructureFactory.Factory(leveeFailureElement.Elevation, (ITransformFunction)leveeFailureElement.Curve); ;
                condition = Saving.PersistenceFactory.GetIASManager().CreateIConditionLocationYearSummary(ImpactAreaID,
                    AnalysisYear, frequencyFunction, transformFunctions, leveeFailureElement, ThresholdType, ThresholdValue);
            }
            else 
            { 
                condition = Saving.PersistenceFactory.GetIASManager().CreateIConditionLocationYearSummary(ImpactAreaID,
                    AnalysisYear, frequencyFunction, transformFunctions, ThresholdType, ThresholdValue);
            }

            if (condition == null)
            {
                return;
            }

            IConvergenceCriteria convergenceCriteria = IConvergenceCriteriaFactory.Factory();
            Dictionary<IMetric, IConvergenceCriteria> metricsDictionary = new Dictionary<IMetric, IConvergenceCriteria>();
            foreach (IMetric metric in condition.Metrics)
            {
                metricsDictionary.Add(metric, IConvergenceCriteriaFactory.Factory());
            }

            IReadOnlyDictionary<IMetric, IConvergenceCriteria> metrics = new ReadOnlyDictionary<IMetric, IConvergenceCriteria>(metricsDictionary);

            IConditionLocationYearResult result = new ConditionLocationYearResult(condition, metrics, seedValue);
            result.Compute();
            ComputeResults = result;
            Saving.PersistenceFactory.GetIASManager().SaveConditionResults(result, this.GetElementID(), frequencyFunction, transformFunctions);

            DisplayResults(result);

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

        /// <summary>
        /// This will update an element that has been modified while the editor is open.
        /// For example a rating curve that was modified.
        /// </summary>
        /// <param name="elemID">The ID of the element that was modified (ie: rating curve)</param>
        /// <param name="newElement">The new element (ie: rating element)</param>
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
            IASElement elem = (IASElement)elementToClone;
            return new IASElement(elem);
        }

        #endregion

    }
}
