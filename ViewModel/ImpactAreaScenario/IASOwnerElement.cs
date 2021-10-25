using ViewModel.AggregatedStageDamage;
using ViewModel.FlowTransforms;
using ViewModel.FrequencyRelationships;
using ViewModel.GeoTech;
using ViewModel.StageTransforms;
using ViewModel.Utilities;
using Functions;
using Model;
using System;
using System.Collections.Generic;
using ViewModel.ImpactAreaScenario;

namespace ViewModel.ImpactAreaScenario
{
    public class IASOwnerElement : ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        //public override string GetTableConstant()
        //{
        //    return TableName;
        //}

        #endregion
        #region Constructors
        public IASOwnerElement( ) : base()
        {
            Name = "Impact Area Scenarios";
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addCondition = new NamedAction();
            addCondition.Header = "Create New Impact Area Scenario";
            addCondition.Action = AddNewCondition;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addCondition);

            Actions = localActions;
            StudyCache.IASElementAdded += AddConditionsElement;
            StudyCache.IASElementRemoved += RemoveConditionsElement;
            StudyCache.IASElementUpdated += UpdateConditionsElement;

            //the child elements
            StudyCache.ImpactAreaRemoved += ChildElementRemoved;
            StudyCache.FlowFrequencyRemoved += ChildElementRemoved;
            StudyCache.InflowOutflowRemoved += ChildElementRemoved;
            StudyCache.RatingRemoved += ChildElementRemoved;
            StudyCache.LeveeRemoved += ChildElementRemoved;
            StudyCache.ExteriorInteriorRemoved += ChildElementRemoved;
            StudyCache.StageDamageRemoved += ChildElementRemoved;


            StudyCache.ImpactAreaUpdated += ChildElementUpdated;
            StudyCache.FlowFrequencyUpdated += ChildElementUpdated;
            StudyCache.InflowOutflowUpdated += ChildElementUpdated;
            StudyCache.RatingUpdated += ChildElementUpdated;
            StudyCache.LeveeUpdated += ChildElementUpdated;
            StudyCache.ExteriorInteriorUpdated += ChildElementUpdated;
            StudyCache.StageDamageUpdated += ChildElementUpdated;


        }

        /// <summary>
        /// When an impact area or any of the curves gets modified (saved) then nothing needs to get
        /// updated in the DB or the study cache for the conditions element because it is just storing
        /// the id's of the sub elements which have not changed. If a conditions editor is open, then
        /// we need to update the curve values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ChildElementUpdated(object sender, Saving.ElementUpdatedEventArgs args)
        {
            //these elements will be the sub elements of the condition (ie: rating, inflow-outflow, etc)
            BaseFdaElement newElement = args.NewElement;
            if (newElement is ChildElement)
            {
                int elemID = args.ID;

                List<IASElementSet> conditionsElements = StudyCache.GetChildElementsOfType<IASElementSet>();
                foreach (IASElementSet condElem in conditionsElements)
                {
                    //todo: this got broken when i changed to the IASElementSet
                    //condElem.UpdateElementInEditor_ChildModified(elemID, (ChildElement)newElement);
                }
            }

        }

        /// <summary>
        /// When an impact area or any of the curves gets removed from the study, this event will get fired. 
        /// The persistence manager for the conditions will loop over all the conditions in the study and if
        /// this child element was being used by a condition, the id for that element in the condition will 
        /// be set to -1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ChildElementRemoved(object sender, Saving.ElementAddedEventArgs args)
        {
            int removedElementID = args.ID;
            if (args.Element is ChildElement)
            {
                ChildElement childElem = (ChildElement)args.Element;
                UpdateEditorWhileEditing(childElem, removedElementID);
                Saving.PersistenceFactory.GetIASManager().UpdateConditionsChildElementRemoved(childElem, removedElementID, -1);
            }
        }
        private void UpdateEditorWhileEditing(ChildElement elem, int removedElementID)
        {
            List<IASElementSet> conditionsElements = StudyCache.GetChildElementsOfType<IASElementSet>();
            foreach(IASElementSet condElem in conditionsElements)
            {
                //todo:what to do here.
                //if(condElem.ConditionsEditor != null)
                //{
                //    condElem.ConditionsEditor.UpdateEditorWhileEditing_ChildRemoved(removedElementID, elem);
                //}
            }
        }

        #endregion
        #region Voids

        private void UpdateConditionsElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            //so if the element has an editor that is open (not null)
            //then we need to update it with the new element. I guess
            //we just care about the curves and the impact area.
            //((ConditionsElement)e.OldElement).UpdateElementInEditor_ChildRemoved((ConditionsElement)e.NewElement);
            UpdateElement(e.OldElement, e.NewElement);
        }
        private void AddConditionsElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveConditionsElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        #region BuildDefaultPlotControls
        public static Plots.IndividualLinkedPlotControlVM BuildDefaultLP3Control(ParentElement ownerElement)
        {
            List<AnalyticalFrequencyElement> listOfLp3 = StudyCache.GetChildElementsOfType<AnalyticalFrequencyElement>();

            AddFlowFrequencyToIASVM lp3Importer = new AddFlowFrequencyToIASVM(listOfLp3);
            lp3Importer.RequestNavigation += ownerElement.Navigate;

            bool isXAxisLog = false;
            bool isYAxisLog = true;
            bool isProbabilityXAxis = true;
            bool isProbabilityYAxis = false;
            bool isXAxisOnBottom = false;
            bool isYAxisOnLeft = false;

            return new Plots.IndividualLinkedPlotControlVM( IFdaFunctionEnum.InflowFrequency,
                new Plots.IASIndividualPlotWrapperVM(isXAxisLog, isYAxisLog, isProbabilityXAxis, isProbabilityYAxis, isXAxisOnBottom, isYAxisOnLeft),
                new Plots.IndividualLinkedPlotCoverButtonVM("Flow Frequency Curve"),
                lp3Importer);
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultInflowOutflowControl(ParentElement ownerElement)
        {
            List<InflowOutflowElement> listOfInfOut = StudyCache.GetChildElementsOfType<InflowOutflowElement>();
            AddInflowOutflowToIASVM inOutImporter = new AddInflowOutflowToIASVM(listOfInfOut);
            inOutImporter.RequestNavigation += ownerElement.Navigate;

            bool isXAxisLog = true;
            bool isYAxisLog = true;
            bool isProbabilityXAxis = false;
            bool isProbabilityYAxis = false;
            bool isXAxisOnBottom = false;
            bool isYAxisOnLeft = false;

            return new Plots.IndividualLinkedPlotControlVM(IFdaFunctionEnum.InflowOutflow,
                new Plots.IASIndividualPlotWrapperVM(isXAxisLog, isYAxisLog, isProbabilityXAxis, isProbabilityYAxis, isXAxisOnBottom, isYAxisOnLeft),
                new Plots.IndividualLinkedPlotCoverButtonVM("Inflow Outflow"),
                inOutImporter, 
                new Plots.DoubleLineModulatorCoverButtonVM(),
                new Plots.DoubleLineModulatorWrapperVM());
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultRatingControl(ParentElement ownerElement)
        {
            List<RatingCurveElement> listOfRatingCurves = StudyCache.GetChildElementsOfType<RatingCurveElement>();
            AddRatingCurveToIASVM ratImporter = new AddRatingCurveToIASVM(listOfRatingCurves);

            bool isXAxisLog = false;
            bool isYAxisLog = true;
            bool isProbabilityXAxis = false;
            bool isProbabilityYAxis = false;
            bool isXAxisOnBottom = false;
            bool isYAxisOnLeft = true;

            return new Plots.IndividualLinkedPlotControlVM(IFdaFunctionEnum.Rating,
                new Plots.IASIndividualPlotWrapperVM(isXAxisLog, isYAxisLog, isProbabilityXAxis, isProbabilityYAxis, isXAxisOnBottom, isYAxisOnLeft),
                new Plots.IndividualLinkedPlotCoverButtonVM("Rating Curve"),
                ratImporter);
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultExtIntStageControl(ParentElement ownerElement)
        {
            List<StageTransforms.ExteriorInteriorElement> listOfExtIntElements = StudyCache.GetChildElementsOfType<ExteriorInteriorElement>();
            AddExteriorInteriorStageToIASVM extIntImporter = new AddExteriorInteriorStageToIASVM(listOfExtIntElements);
            extIntImporter.RequestNavigation += ownerElement.Navigate;

            bool isXAxisLog = false;
            bool isYAxisLog = false;
            bool isProbabilityXAxis = false;
            bool isProbabilityYAxis = false;
            bool isXAxisOnBottom = true;
            bool isYAxisOnLeft = true;

            return new Plots.IndividualLinkedPlotControlVM(IFdaFunctionEnum.ExteriorInteriorStage,
                new Plots.IASIndividualPlotWrapperVM(isXAxisLog, isYAxisLog, isProbabilityXAxis, isProbabilityYAxis, isXAxisOnBottom, isYAxisOnLeft),
                new Plots.IndividualLinkedPlotCoverButtonVM("Ext Int Stage Curve"),
                extIntImporter,
                new Plots.DoubleLineModulatorHorizontalCoverButtonVM("Ext Int Stage Curve"),
                new Plots.HorizontalDoubleLineModulatorWrapperVM());
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultLateralFeaturesControl(ParentElement ownerElement)
        {
            List<LeveeFeatureElement> listOfLeveeFeatureElements = StudyCache.GetChildElementsOfType<LeveeFeatureElement>();
            AddFailureFunctionToIASVM failureImporter = new AddFailureFunctionToIASVM(listOfLeveeFeatureElements);
            failureImporter.RequestNavigation += ownerElement.Navigate;

            bool isXAxisLog = false;
            bool isYAxisLog = false;
            bool isProbabilityXAxis = false;
            bool isProbabilityYAxis = false;
            bool isXAxisOnBottom = false;
            bool isYAxisOnLeft = true;

            Plots.IndividualLinkedPlotCoverButtonVM coverButton = new Plots.IndividualLinkedPlotCoverButtonVM("Lateral Structure");

            return new Plots.IndividualLinkedPlotControlVM(IFdaFunctionEnum.LateralStructureFailure,
                new Plots.IASIndividualPlotWrapperVM(isXAxisLog, isYAxisLog, isProbabilityXAxis, isProbabilityYAxis, isXAxisOnBottom, isYAxisOnLeft),
                coverButton,
                failureImporter,
                new Plots.DoubleLineModulatorHorizontalCoverButtonVM("Lateral Structure"),
                new Plots.IASHorizontalFailureFunctionVM());
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultStageDamageControl(ParentElement ownerElement)
        {
            List<AggregatedStageDamage.AggregatedStageDamageElement> listOfStageDamage = StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();
            AddStageDamageToIASVM stageDamageImporter = new AddStageDamageToIASVM(listOfStageDamage);
            stageDamageImporter.RequestNavigation += ownerElement.Navigate;

            bool isXAxisLog = false;
            bool isYAxisLog = true;
            bool isProbabilityXAxis = false;
            bool isProbabilityYAxis = false;
            bool isXAxisOnBottom = true;
            bool isYAxisOnLeft = true;

            return new Plots.IndividualLinkedPlotControlVM(IFdaFunctionEnum.InteriorStageDamage,
                new Plots.IASIndividualPlotWrapperVM(isXAxisLog, isYAxisLog, isProbabilityXAxis, isProbabilityYAxis, isXAxisOnBottom, isYAxisOnLeft),
                new Plots.IndividualLinkedPlotCoverButtonVM("Int Stage Damage Curve"),
                stageDamageImporter);
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultDamageFrequencyControl(ParentElement ownerElement)
        {

            bool isXAxisLog = false;
            bool isYAxisLog = true;
            bool isProbabilityXAxis = true;
            bool isProbabilityYAxis = false;
            bool isXAxisOnBottom = true;
            bool isYAxisOnLeft = false;

            return new Plots.IndividualLinkedPlotControlVM(IFdaFunctionEnum.DamageFrequency,
                new Plots.IASIndividualPlotWrapperVM(isXAxisLog, isYAxisLog, isProbabilityXAxis, isProbabilityYAxis, isXAxisOnBottom, isYAxisOnLeft),
                new Plots.IndividualLinkedPlotCoverButtonVM("Preview Compute"),
                null);
        }

        #endregion

        public void AddNewCondition(object arg1, EventArgs arg2)
        {



            //List<ImpactArea.ImpactAreaElement> impactAreas = StudyCache.GetChildElementsOfType<ImpactArea.ImpactAreaElement>();
            //Plots.IndividualLinkedPlotControlVM lp3Control = BuildDefaultLP3Control(this);
            //Plots.IndividualLinkedPlotControlVM inflowOutflowControl = BuildDefaultInflowOutflowControl(this);
            //Plots.IndividualLinkedPlotControlVM ratingControl = BuildDefaultRatingControl(this);
            //ratingControl.RequestNavigation += Navigate;

            //Plots.IndividualLinkedPlotControlVM extIntStageControl = BuildDefaultExtIntStageControl(this);
            //Plots.IndividualLinkedPlotControlVM failureControl = BuildDefaultLateralFeaturesControl(this);
            //Plots.IndividualLinkedPlotControlVM StageDamageControl = BuildDefaultStageDamageControl(this);
            //Plots.IndividualLinkedPlotControlVM DamageFrequencyControl = BuildDefaultDamageFrequencyControl(this);

            //Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
            //     .WithSiblingRules(this);
            //List<double> xs = new List<double>() { 0 };
            //List<double> ys = new List<double>() { 0 };
            //ICoordinatesFunction coordFunc = ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
            //IFdaFunction dummyDefault = IFdaFunctionFactory.Factory(IParameterEnum.Rating, (IFunction)coordFunc);
            //IASPlotEditorVM vm = new IASPlotEditorVM(impactAreas, lp3Control, inflowOutflowControl, ratingControl, extIntStageControl,
            //    failureControl, StageDamageControl, DamageFrequencyControl, actionManager, dummyDefault);

            //vm.RequestNavigation += Navigate;
            //string header = "Create Condition";
            //DynamicTabVM tab = new DynamicTabVM(header, vm, "CreateCondition");
            //Navigate(tab, false, false);



            ///////////////////////////////////////////
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                 .WithSiblingRules(this);
            Editor.IASEditorVM vm = new Editor.IASEditorVM(actionManager);
            vm.RequestNavigation += Navigate;
            DynamicTabVM tab2 = new DynamicTabVM("Impact Area Scenario Editor", vm, "CreateIAS");
            Navigate(tab2, false, false);

        }
        #endregion
        #region Functions

        #endregion
    }
}
