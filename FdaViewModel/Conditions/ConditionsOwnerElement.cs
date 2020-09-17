using FdaViewModel.AggregatedStageDamage;
using FdaViewModel.FlowTransforms;
using FdaViewModel.FrequencyRelationships;
using FdaViewModel.GeoTech;
using FdaViewModel.StageTransforms;
using FdaViewModel.Utilities;
using Functions;
using Model;
using System;
using System.Collections.Generic;

namespace FdaViewModel.Conditions
{
    public class ConditionsOwnerElement : Utilities.ParentElement
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
        public ConditionsOwnerElement( ) : base()
        {
            Name = "Conditions";
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addCondition = new NamedAction();
            addCondition.Header = "Create New Condition";
            addCondition.Action = AddNewCondition;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addCondition);

            Actions = localActions;
            StudyCache.ConditionsElementAdded += AddConditionsElement;
            StudyCache.ConditionsElementRemoved += RemoveConditionsElement;
            StudyCache.ConditionsElementUpdated += UpdateConditionsElement;

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

                List<ConditionsElement> conditionsElements = StudyCache.GetChildElementsOfType<ConditionsElement>();
                foreach (ConditionsElement condElem in conditionsElements)
                {
                    condElem.UpdateElementInEditor_ChildModified(elemID, (ChildElement)newElement);
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
                Saving.PersistenceFactory.GetConditionsManager().UpdateConditionsChildElementRemoved(childElem, removedElementID, -1);
            }
        }
        private void UpdateEditorWhileEditing(ChildElement elem, int removedElementID)
        {
            List<ConditionsElement> conditionsElements = StudyCache.GetChildElementsOfType<ConditionsElement>();
            foreach(ConditionsElement condElem in conditionsElements)
            {
                if(condElem.ConditionsEditor != null)
                {
                    condElem.ConditionsEditor.UpdateEditorWhileEditing_ChildRemoved(removedElementID, elem);
                }
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
            //todo: just for testing, delete this dummy lp3
            //List<double> xs = new List<double>() { .1,.3,.5,.7,.9 };
            //List<double> ys = new List<double>() { 1, 10000, 30000, 50000, 70000 };
            //ICoordinatesFunction coordFunc = ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
            //IFdaFunction func = IFdaFunctionFactory.Factory( IParameterEnum.InflowFrequency, (IFunction)coordFunc, "Inflow Freq", UnitsEnum.Probability);
            //List<double> analyticalFlows = new List<double>() { 1, 2, 3 };

            //AnalyticalFrequencyElement dummyElem = new AnalyticalFrequencyElement("dummy elem","", "",2000,true,true, 2000,300,300,false,analyticalFlows,analyticalFlows, func);
            //listOfLp3.Add(dummyElem);

            AddFlowFrequencyToConditionVM lp3Importer = new AddFlowFrequencyToConditionVM(listOfLp3);
            lp3Importer.RequestNavigation += ownerElement.Navigate;

            bool isXAxisLog = false;
            bool isYAxisLog = true;
            bool isProbabilityXAxis = true;
            bool isProbabilityYAxis = false;
            bool isXAxisOnBottom = false;
            bool isYAxisOnLeft = false;

            return new Plots.IndividualLinkedPlotControlVM( IFdaFunctionEnum.InflowFrequency,
                new Plots.ConditionsIndividualPlotWrapperVM(isXAxisLog, isYAxisLog, isProbabilityXAxis, isProbabilityYAxis, isXAxisOnBottom, isYAxisOnLeft),
                new Plots.IndividualLinkedPlotCoverButtonVM("Flow Frequency Curve"),
                lp3Importer);
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultInflowOutflowControl(ParentElement ownerElement)
        {
            List<InflowOutflowElement> listOfInfOut = StudyCache.GetChildElementsOfType<InflowOutflowElement>();
            AddInflowOutflowToConditionVM inOutImporter = new AddInflowOutflowToConditionVM(listOfInfOut);
            inOutImporter.RequestNavigation += ownerElement.Navigate;

            bool isXAxisLog = true;
            bool isYAxisLog = true;
            bool isProbabilityXAxis = false;
            bool isProbabilityYAxis = false;
            bool isXAxisOnBottom = false;
            bool isYAxisOnLeft = false;

            return new Plots.IndividualLinkedPlotControlVM(IFdaFunctionEnum.InflowOutflow,
                new Plots.ConditionsIndividualPlotWrapperVM(isXAxisLog, isYAxisLog, isProbabilityXAxis, isProbabilityYAxis, isXAxisOnBottom, isYAxisOnLeft),
                new Plots.IndividualLinkedPlotCoverButtonVM("Inflow Outflow"),
                inOutImporter, 
                new Plots.DoubleLineModulatorCoverButtonVM(),
                new Plots.DoubleLineModulatorWrapperVM());
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultRatingControl(ParentElement ownerElement)
        {
            List<RatingCurveElement> listOfRatingCurves = StudyCache.GetChildElementsOfType<RatingCurveElement>();
            AddRatingCurveToConditionVM ratImporter = new AddRatingCurveToConditionVM(listOfRatingCurves);

            bool isXAxisLog = false;
            bool isYAxisLog = true;
            bool isProbabilityXAxis = false;
            bool isProbabilityYAxis = false;
            bool isXAxisOnBottom = false;
            bool isYAxisOnLeft = true;

            return new Plots.IndividualLinkedPlotControlVM(IFdaFunctionEnum.Rating,
                new Plots.ConditionsIndividualPlotWrapperVM(isXAxisLog, isYAxisLog, isProbabilityXAxis, isProbabilityYAxis, isXAxisOnBottom, isYAxisOnLeft),
                new Plots.IndividualLinkedPlotCoverButtonVM("Rating Curve"),
                ratImporter);
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultExtIntStageControl(ParentElement ownerElement)
        {
            List<StageTransforms.ExteriorInteriorElement> listOfExtIntElements = StudyCache.GetChildElementsOfType<ExteriorInteriorElement>();
            AddExteriorInteriorStageToConditionVM extIntImporter = new AddExteriorInteriorStageToConditionVM(listOfExtIntElements);
            extIntImporter.RequestNavigation += ownerElement.Navigate;

            bool isXAxisLog = false;
            bool isYAxisLog = false;
            bool isProbabilityXAxis = false;
            bool isProbabilityYAxis = false;
            bool isXAxisOnBottom = true;
            bool isYAxisOnLeft = true;

            return new Plots.IndividualLinkedPlotControlVM(IFdaFunctionEnum.ExteriorInteriorStage,
                new Plots.ConditionsIndividualPlotWrapperVM(isXAxisLog, isYAxisLog, isProbabilityXAxis, isProbabilityYAxis, isXAxisOnBottom, isYAxisOnLeft),
                new Plots.IndividualLinkedPlotCoverButtonVM("Ext Int Stage Curve"),
                extIntImporter,
                new Plots.DoubleLineModulatorHorizontalCoverButtonVM("Ext Int Stage Curve"),
                new Plots.HorizontalDoubleLineModulatorWrapperVM());
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultLateralFeaturesControl(ParentElement ownerElement)
        {
            List<LeveeFeatureElement> listOfLeveeFeatureElements = StudyCache.GetChildElementsOfType<LeveeFeatureElement>();
            AddFailureFunctionToConditionVM failureImporter = new AddFailureFunctionToConditionVM(listOfLeveeFeatureElements);
            failureImporter.RequestNavigation += ownerElement.Navigate;

            bool isXAxisLog = false;
            bool isYAxisLog = false;
            bool isProbabilityXAxis = false;
            bool isProbabilityYAxis = false;
            bool isXAxisOnBottom = false;
            bool isYAxisOnLeft = true;

            Plots.IndividualLinkedPlotCoverButtonVM coverButton = new Plots.IndividualLinkedPlotCoverButtonVM("Lateral Structure");

            return new Plots.IndividualLinkedPlotControlVM(IFdaFunctionEnum.LateralStructureFailure,
                new Plots.ConditionsIndividualPlotWrapperVM(isXAxisLog, isYAxisLog, isProbabilityXAxis, isProbabilityYAxis, isXAxisOnBottom, isYAxisOnLeft),
                coverButton,
                failureImporter,
                new Plots.DoubleLineModulatorHorizontalCoverButtonVM("Lateral Structure"),
                new Plots.ConditionsHorizontalFailureFunctionVM());
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultStageDamageControl(ParentElement ownerElement)
        {
            List<AggregatedStageDamage.AggregatedStageDamageElement> listOfStageDamage = StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();
            AddStageDamageToConditionVM stageDamageImporter = new AddStageDamageToConditionVM(listOfStageDamage);
            stageDamageImporter.RequestNavigation += ownerElement.Navigate;

            bool isXAxisLog = false;
            bool isYAxisLog = true;
            bool isProbabilityXAxis = false;
            bool isProbabilityYAxis = false;
            bool isXAxisOnBottom = true;
            bool isYAxisOnLeft = true;

            return new Plots.IndividualLinkedPlotControlVM(IFdaFunctionEnum.InteriorStageDamage,
                new Plots.ConditionsIndividualPlotWrapperVM(isXAxisLog, isYAxisLog, isProbabilityXAxis, isProbabilityYAxis, isXAxisOnBottom, isYAxisOnLeft),
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
                new Plots.ConditionsIndividualPlotWrapperVM(isXAxisLog, isYAxisLog, isProbabilityXAxis, isProbabilityYAxis, isXAxisOnBottom, isYAxisOnLeft),
                new Plots.IndividualLinkedPlotCoverButtonVM("Preview Compute"),
                null);
        }

        #endregion

        public void AddNewCondition(object arg1, EventArgs arg2)
        {
            List<ImpactArea.ImpactAreaElement> impactAreas = StudyCache.GetChildElementsOfType<ImpactArea.ImpactAreaElement>();
            Plots.IndividualLinkedPlotControlVM lp3Control = BuildDefaultLP3Control(this);
            Plots.IndividualLinkedPlotControlVM inflowOutflowControl = BuildDefaultInflowOutflowControl(this);
            Plots.IndividualLinkedPlotControlVM ratingControl = BuildDefaultRatingControl(this);
            ratingControl.RequestNavigation += Navigate;

            Plots.IndividualLinkedPlotControlVM extIntStageControl = BuildDefaultExtIntStageControl(this);
            Plots.IndividualLinkedPlotControlVM failureControl = BuildDefaultLateralFeaturesControl(this);
            Plots.IndividualLinkedPlotControlVM StageDamageControl = BuildDefaultStageDamageControl(this);
            Plots.IndividualLinkedPlotControlVM DamageFrequencyControl = BuildDefaultDamageFrequencyControl(this);

            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                 .WithSiblingRules(this);
            // .WithParentGuid(this.GUID)
            // .WithCanOpenMultipleTimes(true);
            List<double> xs = new List<double>() { 0 };
            List<double> ys = new List<double>() { 0 };
            ICoordinatesFunction coordFunc = ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
            IFdaFunction dummyDefault = IFdaFunctionFactory.Factory(IParameterEnum.Rating, (IFunction)coordFunc);
            ConditionsPlotEditorVM vm = new ConditionsPlotEditorVM(impactAreas, lp3Control, inflowOutflowControl, ratingControl, extIntStageControl, 
                failureControl, StageDamageControl, DamageFrequencyControl, actionManager,dummyDefault);
            //StudyCache.AddSiblingRules(vm, this);
            //vm.AddSiblingRules(this);
            vm.RequestNavigation += Navigate;
            string header = "Create Condition";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "CreateCondition");
            Navigate(tab, false, false);

            //if (!vm2.WasCanceled)
            //{
            //    if (!vm2.HasError)
            //    {
            //        ConditionsElement ce = ConditionFactory.BuildConditionsElement(vm2, this);
            //        AddElement(ce);
            //    }
            //}
        }
        #endregion
        #region Functions
        //public override string TableName
        //{
        //    get
        //    {
        //        return "Conditions";
        //    }
        //}
        //public override string[] TableColumnNames()
        //{
        //    return new string[] { "Name", "Description", "AnalysisYear", "ImpactArea",
        //        "UseFlowFreq","FlowFreq",
        //        "UseInOutFlow","InOutFlow",
        //        "UseRating","Rating",
        //        "UseExtIntStage","ExtIntStage",
        //        "UseLevee","Levee",
        //        "UseFailureFunc","FailureFunc",
        //        "UseStageDamage","StageDamage",
        //        "UseThreshold","ThresholdType","ThresholdValue"};
        //}

        //public override Type[] TableColumnTypes()
        //{
        //    return new Type[] { typeof(string), typeof(string), typeof(int), typeof(string),

        //        typeof(bool), typeof(string),
        //        typeof(bool), typeof(string),
        //        typeof(bool), typeof(string),
        //        typeof(bool), typeof(string),
        //        typeof(bool), typeof(string),
        //        typeof(bool), typeof(string),
        //        typeof(bool), typeof(string),
        //        typeof(bool), typeof(string), typeof(double)};
        //}

        //public override void AddElementFromRowData(object[] rowData)
        //{
        //    //this is when it loads

        //    if (rowData.Length >= 18)
        //    {
        //        //get the impact area
        //        string selectedImpAreaName = (string)rowData[3];
        //        ImpactArea.ImpactAreaElement selectedImpArea = null;
        //        List<ImpactArea.ImpactAreaElement> impactAreas = GetElementsOfType<ImpactArea.ImpactAreaElement>();
        //        foreach(ImpactArea.ImpactAreaElement impArea in impactAreas)
        //        {
        //            if(impArea.Name.Equals(selectedImpAreaName))
        //            {
        //                selectedImpArea = impArea;
        //            }
        //        }
        //        if (selectedImpArea == null)
        //        {
        //            //what do we do?
        //        }

        //        //threshold stuff
        //        bool useThreshold = (bool)rowData[18];
        //        PerformanceThresholdTypes thresholdType = PerformanceThresholdTypes.InteriorStage;
        //        Enum.TryParse((string)rowData[19], out thresholdType);
        //        double thresholdValue = (double)rowData[20];

        //        //get the impAreaRowItem. What is this? do we need it?
        //        ImpactArea.ImpactAreaRowItem indexLocation = new ImpactArea.ImpactAreaRowItem();
        //        int analysisYear = Convert.ToInt32(rowData[2]);
        //        ConditionBuilder builder = new ConditionBuilder((string)rowData[0], (string)rowData[1], analysisYear, selectedImpArea, indexLocation,
        //             thresholdType,thresholdValue, this);

        //        bool useFlowFreq = (bool)rowData[4];
        //        if(useFlowFreq)
        //        {
        //            string flowFreqName = (string)rowData[5];
        //            AnalyticalFrequencyElement flowFreqElem = GetSelectedElementOfType<AnalyticalFrequencyElement>(flowFreqName);
        //            builder.WithAnalyticalFreqElem(flowFreqElem);
        //        }

        //        bool useInflowOutflow = (bool)rowData[6];
        //        if (useInflowOutflow)
        //        {
        //            string infOutName = (string)rowData[7];
        //            InflowOutflowElement inOutElem = GetSelectedElementOfType<InflowOutflowElement>(infOutName);
        //            builder.WithInflowOutflowElem(inOutElem);
        //        }

        //        bool useRating = (bool)rowData[8];
        //        if (useRating)
        //        {
        //            string ratingName = (string)rowData[9];
        //            RatingCurveElement ratingElem = GetSelectedElementOfType<RatingCurveElement>(ratingName);
        //            builder.WithRatingCurveElem(ratingElem);
        //        }

        //        bool useIntExt = (bool)rowData[10];
        //        if (useIntExt)
        //        {
        //            string extIntName = (string)rowData[11];
        //            ExteriorInteriorElement extIntElem = GetSelectedElementOfType<ExteriorInteriorElement>(extIntName);
        //            builder.WithExtIntStageElem(extIntElem);
        //        }

        //        bool useLevee = (bool)rowData[12];
        //        if (useLevee)
        //        {
        //            string leveeName = (string)rowData[13];
        //            LeveeFeatureElement leveeElem = GetSelectedElementOfType<LeveeFeatureElement>(leveeName);
        //            builder.WithLevee(leveeElem);
        //        }

        //        bool useFailure = (bool)rowData[14];
        //        if (useFailure)
        //        {
        //            string failureName = (string)rowData[15];
        //            FailureFunctionElement failureElem = GetSelectedElementOfType<FailureFunctionElement>(failureName);
        //            builder.WithFailureFunctionElem(failureElem);
        //        }

        //        bool useStageDam = (bool)rowData[16];
        //        if (useStageDam)
        //        {
        //            string stageDamName = (string)rowData[17];
        //            AggregatedStageDamageElement stageDamElem = GetSelectedElementOfType<AggregatedStageDamageElement>(stageDamName);
        //            builder.WithAggStageDamageElem(stageDamElem);
        //        }

        //        AddElement(builder.build(), false);
        //    }

        //}

        //private T GetSelectedElementOfType<T>(string name) where T:ChildElement
        //{
        //    List<T> elems = GetElementsOfType<T>();
        //    foreach(T elem in elems)
        //    {
        //        if(elem.Name.Equals(name))
        //        {
        //            return elem;
        //        }
        //    }
        //    return null;
        //}
        #endregion
    }
}
