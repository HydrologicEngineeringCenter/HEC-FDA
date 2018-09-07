using FdaViewModel.AggregatedStageDamage;
using FdaViewModel.FlowTransforms;
using FdaViewModel.FrequencyRelationships;
using FdaViewModel.GeoTech;
using FdaViewModel.StageTransforms;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Conditions
{
    public class ConditionsOwnerElement : Utilities.OwnerElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        public override string GetTableConstant()
        {
            return TableName;
        }
        #endregion
        #region Constructors
        public ConditionsOwnerElement(BaseFdaElement owner) : base(owner)
        {
            Name = "Conditions";
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);

            Utilities.NamedAction addCondition = new Utilities.NamedAction();
            addCondition.Header = "Create New Condition";
            addCondition.Action = AddNewCondition;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(addCondition);

            Actions = localActions;
        }
        #endregion
        #region Voids
        public override void AddBaseElements()
        {
            //throw new NotImplementedException();
        }
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        #region BuildDefaultPlotControls
        public static Plots.IndividualLinkedPlotControlVM BuildDefaultLP3Control(ConditionsOwnerElement ownerElement)
        {
            List<FrequencyRelationships.AnalyticalFrequencyElement> listOfLp3 = ownerElement.GetElementsOfType<FrequencyRelationships.AnalyticalFrequencyElement>();
            AddFlowFrequencyToConditionVM lp3Importer = new AddFlowFrequencyToConditionVM(listOfLp3, ownerElement);
            return new Plots.IndividualLinkedPlotControlVM(
                new Plots.ConditionsIndividualPlotWrapperVM(true, true, "LP3", "Probability", "Inflow"),
                new Plots.IndividualLinkedPlotCoverButtonVM("Flow Frequency Curve"),
                lp3Importer);
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultInflowOutflowControl(ConditionsOwnerElement ownerElement)
        {
            List<FlowTransforms.InflowOutflowElement> listOfInfOut = ownerElement.GetElementsOfType<FlowTransforms.InflowOutflowElement>();
            AddInflowOutflowToConditionVM inOutImporter = new AddInflowOutflowToConditionVM(listOfInfOut, ownerElement);
            return new Plots.IndividualLinkedPlotControlVM(
                new Plots.ConditionsIndividualPlotWrapperVM(true, false, "Inflow Outflow", "Inflow", "OutFlow"),
                new Plots.IndividualLinkedPlotCoverButtonVM("Inflow Outflow"),
                inOutImporter,
                new Plots.DoubleLineModulatorCoverButtonVM(),
                new Plots.DoubleLineModulatorWrapperVM());
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultRatingControl(ConditionsOwnerElement ownerElement)
        {
            List<StageTransforms.RatingCurveElement> listOfRatingCurves = ownerElement.GetElementsOfType<StageTransforms.RatingCurveElement>();
            AddRatingCurveToConditionVM ratImporter = new AddRatingCurveToConditionVM(listOfRatingCurves, ownerElement);
            return new Plots.IndividualLinkedPlotControlVM(
                new Plots.ConditionsIndividualPlotWrapperVM(true, false, "Rating", "Exterior Stage", "OutFlow"),
                new Plots.IndividualLinkedPlotCoverButtonVM("Rating Curve"),
                ratImporter);
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultExtIntStageControl(ConditionsOwnerElement ownerElement)
        {
            List<StageTransforms.ExteriorInteriorElement> listOfExtIntElements = ownerElement.GetElementsOfType<StageTransforms.ExteriorInteriorElement>();
            AddExteriorInteriorStageToConditionVM extIntImporter = new AddExteriorInteriorStageToConditionVM(listOfExtIntElements, ownerElement);
            return new Plots.IndividualLinkedPlotControlVM(
                new Plots.ConditionsIndividualPlotWrapperVM(true, false, "Exterior Interior Stage", "Exterior Stage", "Interior Stage"),
                new Plots.IndividualLinkedPlotCoverButtonVM("Ext Int Stage Curve"),
                extIntImporter,
                new Plots.DoubleLineModulatorHorizontalCoverButtonVM(),
                new Plots.HorizontalDoubleLineModulatorWrapperVM());
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultStageDamageControl(ConditionsOwnerElement ownerElement)
        {
            List<AggregatedStageDamage.AggregatedStageDamageElement> listOfStageDamage = ownerElement.GetElementsOfType<AggregatedStageDamage.AggregatedStageDamageElement>();
            AddStageDamageToConditionVM stageDamageImporter = new AddStageDamageToConditionVM(listOfStageDamage, ownerElement);
            return new Plots.IndividualLinkedPlotControlVM(
                new Plots.ConditionsIndividualPlotWrapperVM(true, false, "Stage Damage", "Interior Stage", "Damage"),
                new Plots.IndividualLinkedPlotCoverButtonVM("Int Stage Damage Curve"),
                stageDamageImporter);
        }

        public static Plots.IndividualLinkedPlotControlVM BuildDefaultDamageFrequencyControl(ConditionsOwnerElement ownerElement)
        {
            return new Plots.IndividualLinkedPlotControlVM(
                new Plots.ConditionsIndividualPlotWrapperVM(true, true, "Damage Frequency", "Probability", "Damage", false),
                new Plots.IndividualLinkedPlotCoverButtonVM("Preview Compute"),
                null);
        }

        #endregion

        private void AddNewCondition(object arg1, EventArgs arg2)
        {
            
            List<ImpactArea.ImpactAreaElement> impactAreas = GetElementsOfType<ImpactArea.ImpactAreaElement>();
            //List<FrequencyRelationships.AnalyticalFrequencyElement> freqeles = GetElementsOfType<FrequencyRelationships.AnalyticalFrequencyElement>();
            ////get a list of the inflow outflow curves
            //List<FlowTransforms.InflowOutflowElement> inflowOutflowList = GetElementsOfType<FlowTransforms.InflowOutflowElement>();

            //List<StageTransforms.RatingCurveElement> ratingeles = GetElementsOfType<StageTransforms.RatingCurveElement>();

            ////get list of int ext stage curves
            //List<StageTransforms.ExteriorInteriorElement> intExtList = GetElementsOfType<StageTransforms.ExteriorInteriorElement>();
            ////get list of lateral structures
            //List<GeoTech.LeveeFeatureElement> leveeList = GetElementsOfType<GeoTech.LeveeFeatureElement>();

            ////get list of failure functions
            //List<GeoTech.FailureFunctionElement> failureFunctionList = GetElementsOfType<GeoTech.FailureFunctionElement>();

            ////get list of stage damage curves
            //List<AggregatedStageDamage.AggregatedStageDamageElement> damageles = GetElementsOfType<AggregatedStageDamage.AggregatedStageDamageElement>();
            if (impactAreas.Count == 0)
            {
                ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("No Impact Area Sets have been defined.", FdaModel.Utilities.Messager.ErrorMessageEnum.Report | FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel));
                return;
            }

            //List<ImpactArea.ImpactAreaElement> impAreas = GetElementsOfType<ImpactArea.ImpactAreaElement>();
            //List<Inventory.InventoryElement> structInventories = GetElementsOfType<Inventory.InventoryElement>();

            //List<ImpactArea.ImpactAreaOwnerElement> impactOwners = GetElementsOfType<ImpactArea.ImpactAreaOwnerElement>();


            //ConditionsPlotEditorVM vm2 = new ConditionsPlotEditorVM(impAreas,structInventories, lp3VM,inOutVM, rat,stageDamage,extInt,impactOwners.FirstOrDefault());
            Plots.IndividualLinkedPlotControlVM lp3Control = BuildDefaultLP3Control(this);

            Plots.IndividualLinkedPlotControlVM inflowOutflowControl = BuildDefaultInflowOutflowControl(this);

            Plots.IndividualLinkedPlotControlVM ratingControl = BuildDefaultRatingControl(this);

            Plots.IndividualLinkedPlotControlVM extIntStageControl = BuildDefaultExtIntStageControl(this);

            Plots.IndividualLinkedPlotControlVM StageDamageControl = BuildDefaultStageDamageControl(this);

            Plots.IndividualLinkedPlotControlVM DamageFrequencyControl = BuildDefaultDamageFrequencyControl(this);

            //test.AddFlowFreqVM = new AddFlowFrequencyToConditionVM(listOfLp3, this);
            //test.CoverButtonVM = new Plots.IndividualLinkedPlotCoverButtonVM(test);
            //test.MyIndividualLinkedPlotVM = new Plots.IndividualLinkedPlotVM();
            //test.CurrentVM = test.CoverButtonVM;
            ConditionsPlotEditorVM vm2 = new ConditionsPlotEditorVM(impactAreas, lp3Control, inflowOutflowControl, ratingControl, extIntStageControl, StageDamageControl, DamageFrequencyControl);

            //need to attach handlers in order to open the importers into their own windows.
            //vm2.RequestNavigation += Navigate;

            Navigate(vm2);

            if (!vm2.WasCancled)
            {
                if (!vm2.HasError)
                {
                    ConditionsElement ce = ConditionFactory.BuildConditionsElement(vm2, this);
                    //ConditionsElement ce = new ConditionsElement(vm2.Name,vm2.Description,vm2.Year,vm2.SelectedImpactArea,null,vm2.IsPlot0Visible,vm2.Plot0VM.)
                    
                        //ConditionsElement ce = new ConditionsElement(vm.Name,vm.Description,vm.AnalysisYear,vm.ImpactAreaSet,vm.ImpactArea,vm.UseAnalyticalFlowFrequency,vm.AnalyticalFlowFrequency, vm.UsesInflowOutflow,vm.InflowOutflowElement, vm.UseRatingCurve,vm.RatingCurve, vm.UsesExteriorInterior,vm.ExteriorInteriorElement,vm.UsesLevee,vm.LeveeElement,vm.UsesFailureFunction,vm.FailureFunctionElement, vm.UseAggregatedStageDamage,vm.StageDamage,vm.UseThreshold, vm.ThresholdType,vm.ThresholdValue,this);
                    AddElement(ce);
                }
            }

            //vm2.RequestNavigation -= Navigate;

        }
        #endregion
        #region Functions
        public override string TableName
        {
            get
            {
                return "Conditions";
            }
        }
        public override string[] TableColumnNames()
        {
            return new string[] { "Name", "Description", "AnalysisYear", "ImpactArea",
                "UseFlowFreq","FlowFreq",
                "UseInOutFlow","InOutFlow",
                "UseRating","Rating",
                "UseExtIntStage","ExtIntStage",
                "UseLevee","Levee",
                "UseFailureFunc","FailureFunc",
                "UseStageDamage","StageDamage"};
        }

        public override Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string), typeof(string), typeof(int), typeof(string),
                typeof(bool), typeof(string),
                typeof(bool), typeof(string),
                typeof(bool), typeof(string),
                typeof(bool), typeof(string),
                typeof(bool), typeof(string),
                typeof(bool), typeof(string),
                typeof(bool), typeof(string)};
        }

        public override void AddElement(object[] rowData)
        {
            //this is when it loads

            if (rowData.Length >= 18)
            {
                //get the impact area
                string selectedImpAreaName = (string)rowData[3];
                ImpactArea.ImpactAreaElement selectedImpArea = null;
                List<ImpactArea.ImpactAreaElement> impactAreas = GetElementsOfType<ImpactArea.ImpactAreaElement>();
                foreach(ImpactArea.ImpactAreaElement impArea in impactAreas)
                {
                    if(impArea.Name.Equals(selectedImpAreaName))
                    {
                        selectedImpArea = impArea;
                    }
                }
                if (selectedImpArea == null)
                {
                    //what do we do?
                }


                //get the impAreaRowItem. What is this? do we need it?
                ImpactArea.ImpactAreaRowItem indexLocation = new ImpactArea.ImpactAreaRowItem();
                int analysisYear = Convert.ToInt32(rowData[2]);
                ConditionBuilder builder = new ConditionBuilder((string)rowData[0], (string)rowData[1], analysisYear, selectedImpArea, indexLocation, this);

                bool useFlowFreq = (bool)rowData[4];
                if(useFlowFreq)
                {
                    string flowFreqName = (string)rowData[5];
                    AnalyticalFrequencyElement flowFreqElem = GetSelectedElementOfType<AnalyticalFrequencyElement>(flowFreqName);
                    builder.WithAnalyticalFreqElem(flowFreqElem);
                }

                bool useInflowOutflow = (bool)rowData[6];
                if (useInflowOutflow)
                {
                    string infOutName = (string)rowData[7];
                    InflowOutflowElement inOutElem = GetSelectedElementOfType<InflowOutflowElement>(infOutName);
                    builder.WithInflowOutflowElem(inOutElem);
                }

                bool useRating = (bool)rowData[8];
                if (useRating)
                {
                    string ratingName = (string)rowData[9];
                    RatingCurveElement ratingElem = GetSelectedElementOfType<RatingCurveElement>(ratingName);
                    builder.WithRatingCurveElem(ratingElem);
                }

                bool useIntExt = (bool)rowData[10];
                if (useIntExt)
                {
                    string extIntName = (string)rowData[11];
                    ExteriorInteriorElement extIntElem = GetSelectedElementOfType<ExteriorInteriorElement>(extIntName);
                    builder.WithExtIntStageElem(extIntElem);
                }

                bool useLevee = (bool)rowData[12];
                if (useLevee)
                {
                    string leveeName = (string)rowData[13];
                    LeveeFeatureElement leveeElem = GetSelectedElementOfType<LeveeFeatureElement>(leveeName);
                    builder.WithLevee(leveeElem);
                }

                bool useFailure = (bool)rowData[14];
                if (useFailure)
                {
                    string failureName = (string)rowData[15];
                    FailureFunctionElement failureElem = GetSelectedElementOfType<FailureFunctionElement>(failureName);
                    builder.WithFailureFunctionElem(failureElem);
                }

                bool useStageDam = (bool)rowData[16];
                if (useStageDam)
                {
                    string stageDamName = (string)rowData[17];
                    AggregatedStageDamageElement stageDamElem = GetSelectedElementOfType<AggregatedStageDamageElement>(stageDamName);
                    builder.WithAggStageDamageElem(stageDamElem);
                }

                AddElement(builder.build(), false);
            }

        }

        private T GetSelectedElementOfType<T>(string name) where T:OwnedElement
        {
            List<T> elems = GetElementsOfType<T>();
            foreach(T elem in elems)
            {
                if(elem.Name.Equals(name))
                {
                    return elem;
                }
            }
            return null;
        }
        #endregion
    }
}
