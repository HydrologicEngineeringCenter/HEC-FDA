using FdaViewModel.AggregatedStageDamage;
using FdaViewModel.FlowTransforms;
using FdaViewModel.FrequencyRelationships;
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
        }
        #endregion
        #region Voids
        private void UpdateConditionsElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
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
            List<double> xs = new List<double>() { .1,.3,.5,.7,.9 };
            List<double> ys = new List<double>() { 1, 1000, 3000, 5000, 7000 };
            ICoordinatesFunction coordFunc = ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
            IFdaFunction func = IFdaFunctionFactory.Factory( IParameterEnum.InflowFrequency, (IFunction)coordFunc, "Inflow Freq", UnitsEnum.Probability);
            AnalyticalFrequencyElement dummyElem = new AnalyticalFrequencyElement("dummyElem", "now", "desc", func);
            listOfLp3.Add(dummyElem);

            AddFlowFrequencyToConditionVM lp3Importer = new AddFlowFrequencyToConditionVM(listOfLp3);
            lp3Importer.RequestNavigation += ownerElement.Navigate;

            bool isYAxisLog = true;
            bool isProbabilityXAxis = true;

            return new Plots.IndividualLinkedPlotControlVM(
                new Plots.ConditionsIndividualPlotWrapperVM(true, true, "LP3", "Probability", "Inflow"), 
                new Plots.IndividualLinkedPlotCoverButtonVM("Flow Frequency Curve"),
                lp3Importer, isYAxisLog, isProbabilityXAxis, false, false);
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultInflowOutflowControl(ParentElement ownerElement)
        {
            List<InflowOutflowElement> listOfInfOut = StudyCache.GetChildElementsOfType<InflowOutflowElement>();
            AddInflowOutflowToConditionVM inOutImporter = new AddInflowOutflowToConditionVM(listOfInfOut);
            inOutImporter.RequestNavigation += ownerElement.Navigate;

            bool isYAxisLog = false;
            bool isProbabilityXAxis = false;

            return new Plots.IndividualLinkedPlotControlVM(
                new Plots.ConditionsIndividualPlotWrapperVM(true, false, "Inflow Outflow", "Inflow", "OutFlow"),
                new Plots.IndividualLinkedPlotCoverButtonVM("Inflow Outflow"),
                inOutImporter, isYAxisLog, isProbabilityXAxis,true, true, 
                new Plots.DoubleLineModulatorCoverButtonVM(),
                new Plots.DoubleLineModulatorWrapperVM());
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultRatingControl(ParentElement ownerElement)
        {
            List<RatingCurveElement> listOfRatingCurves = StudyCache.GetChildElementsOfType<RatingCurveElement>();
            AddRatingCurveToConditionVM ratImporter = new AddRatingCurveToConditionVM(listOfRatingCurves);

            bool isYAxisLog = true;
            bool isProbabilityXAxis = false;

            return new Plots.IndividualLinkedPlotControlVM(
                new Plots.ConditionsIndividualPlotWrapperVM(true, false, "Rating", "Exterior Stage", "OutFlow"),
                new Plots.IndividualLinkedPlotCoverButtonVM("Rating Curve"),
                ratImporter, isYAxisLog, isProbabilityXAxis, false, true);
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultExtIntStageControl(ParentElement ownerElement)
        {
            List<StageTransforms.ExteriorInteriorElement> listOfExtIntElements = StudyCache.GetChildElementsOfType<ExteriorInteriorElement>();
            AddExteriorInteriorStageToConditionVM extIntImporter = new AddExteriorInteriorStageToConditionVM(listOfExtIntElements);
            extIntImporter.RequestNavigation += ownerElement.Navigate;

            bool isYAxisLog = false;
            bool isProbabilityXAxis = false;

            return new Plots.IndividualLinkedPlotControlVM(
                new Plots.ConditionsIndividualPlotWrapperVM(true, false, "Exterior Interior Stage", "Exterior Stage", "Interior Stage"),
                new Plots.IndividualLinkedPlotCoverButtonVM("Ext Int Stage Curve"),
                extIntImporter, isYAxisLog, isProbabilityXAxis, true, true,
                new Plots.DoubleLineModulatorHorizontalCoverButtonVM(),
                new Plots.HorizontalDoubleLineModulatorWrapperVM());
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultStageDamageControl(ParentElement ownerElement)
        {
            List<AggregatedStageDamage.AggregatedStageDamageElement> listOfStageDamage = StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();
            AddStageDamageToConditionVM stageDamageImporter = new AddStageDamageToConditionVM(listOfStageDamage);
            stageDamageImporter.RequestNavigation += ownerElement.Navigate;

            bool isYAxisLog = true;
            bool isProbabilityXAxis = false;

            return new Plots.IndividualLinkedPlotControlVM(
                new Plots.ConditionsIndividualPlotWrapperVM(true, false, "Stage Damage", "Interior Stage", "Damage"),
                new Plots.IndividualLinkedPlotCoverButtonVM("Int Stage Damage Curve"),
                stageDamageImporter, isYAxisLog, isProbabilityXAxis, true, true);
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultDamageFrequencyControl(ParentElement ownerElement)
        {
            bool isYAxisLog = true;
            bool isProbabilityXAxis = true;

            return new Plots.IndividualLinkedPlotControlVM(
                new Plots.ConditionsIndividualPlotWrapperVM(true, true, "Damage Frequency", "Probability", "Damage", false),
                new Plots.IndividualLinkedPlotCoverButtonVM("Preview Compute"),
                null, isYAxisLog, isProbabilityXAxis, true,false);
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

            Plots.IndividualLinkedPlotControlVM StageDamageControl = BuildDefaultStageDamageControl(this);

            Plots.IndividualLinkedPlotControlVM DamageFrequencyControl = BuildDefaultDamageFrequencyControl(this);

            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                 .WithSiblingRules(this);
            // .WithParentGuid(this.GUID)
            // .WithCanOpenMultipleTimes(true);

            ConditionsPlotEditorVM vm = new ConditionsPlotEditorVM(impactAreas, lp3Control, inflowOutflowControl, ratingControl, extIntStageControl, StageDamageControl, DamageFrequencyControl, actionManager);
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
