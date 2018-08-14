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
        private void AddNewCondition(object arg1, EventArgs arg2)
        {
            
            List<ImpactArea.ImpactAreaElement> impactAreas = GetElementsOfType<ImpactArea.ImpactAreaElement>();
            List<FrequencyRelationships.AnalyticalFrequencyElement> freqeles = GetElementsOfType<FrequencyRelationships.AnalyticalFrequencyElement>();
            //get a list of the inflow outflow curves
            List<FlowTransforms.InflowOutflowElement> inflowOutflowList = GetElementsOfType<FlowTransforms.InflowOutflowElement>();

            List<StageTransforms.RatingCurveElement> ratingeles = GetElementsOfType<StageTransforms.RatingCurveElement>();

            //get list of int ext stage curves
            List<StageTransforms.ExteriorInteriorElement> intExtList = GetElementsOfType<StageTransforms.ExteriorInteriorElement>();
            //get list of lateral structures
            List<GeoTech.LeveeFeatureElement> leveeList = GetElementsOfType<GeoTech.LeveeFeatureElement>();

            //get list of failure functions
            List<GeoTech.FailureFunctionElement> failureFunctionList = GetElementsOfType<GeoTech.FailureFunctionElement>();

            //get list of stage damage curves
            List<AggregatedStageDamage.AggregatedStageDamageElement> damageles = GetElementsOfType<AggregatedStageDamage.AggregatedStageDamageElement>();
            if (impactAreas.Count == 0)
            {
                ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("No Impact Area Sets have been defined.", FdaModel.Utilities.Messager.ErrorMessageEnum.Report | FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel));
                return;
            }

            //List<Inventory.InventoryElement> structInventories = GetElementsOfType<Inventory.InventoryElement>();

            //if (freqeles.Count == 0) ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("No Analyitical Frequency Curves have been defined.", FdaModel.Utilities.Messager.ErrorMessageEnum.Report | FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel));
            //if (ratingeles.Count == 0) ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("No Rating Curves have been defined.", FdaModel.Utilities.Messager.ErrorMessageEnum.Report | FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel));
            //if (damageles.Count == 0) ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("No Aggregated Stage Damage Curves have been defined.", FdaModel.Utilities.Messager.ErrorMessageEnum.Report | FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel));

            //ConditionsEditorVM vm = new ConditionsEditorVM(impactAreas, freqeles, inflowOutflowList, ratingeles, intExtList,leveeList,failureFunctionList, damageles, structInventories, this);

            //get all the lp3 curves
            List<FrequencyRelationships.AnalyticalFrequencyElement> listOfLp3 = GetElementsOfType<FrequencyRelationships.AnalyticalFrequencyElement>();
            AddFlowFrequencyToConditionVM lp3VM = new AddFlowFrequencyToConditionVM(listOfLp3,this);

            List<FlowTransforms.InflowOutflowElement> listOfInfOut = GetElementsOfType<FlowTransforms.InflowOutflowElement>();
            AddInflowOutflowToConditionVM inOutVM = new AddInflowOutflowToConditionVM(listOfInfOut, this);

            List<StageTransforms.RatingCurveElement> listOfRatingCurves = GetElementsOfType<StageTransforms.RatingCurveElement>();
            AddRatingCurveToConditionVM rat = new AddRatingCurveToConditionVM(listOfRatingCurves,this);

            List<AggregatedStageDamage.AggregatedStageDamageElement> listOfStageDamage = GetElementsOfType<AggregatedStageDamage.AggregatedStageDamageElement>();
            AddStageDamageToConditionVM stageDamage = new AddStageDamageToConditionVM(listOfStageDamage,this);

            List<StageTransforms.ExteriorInteriorElement> listOfExtIntElements = GetElementsOfType<StageTransforms.ExteriorInteriorElement>();
            AddExteriorInteriorStageToConditionVM extInt = new AddExteriorInteriorStageToConditionVM(listOfExtIntElements,this);

            List<ImpactArea.ImpactAreaElement> impAreas = GetElementsOfType<ImpactArea.ImpactAreaElement>();
            List<Inventory.InventoryElement> structInventories = GetElementsOfType<Inventory.InventoryElement>();

            List<ImpactArea.ImpactAreaOwnerElement> impactOwners = GetElementsOfType<ImpactArea.ImpactAreaOwnerElement>();


            //ConditionsPlotEditorVM vm2 = new ConditionsPlotEditorVM(impAreas,structInventories, lp3VM,inOutVM, rat,stageDamage,extInt,impactOwners.FirstOrDefault());
            Plots.IndividualLinkedPlotControlVM lp3Control = new Plots.IndividualLinkedPlotControlVM(new Plots.ConditionsIndividualPlotWrapperVM(true,true,"LP3","Probability","Inflow"),new Plots.IndividualLinkedPlotCoverButtonVM(),new AddFlowFrequencyToConditionVM(listOfLp3,this));
            Plots.IndividualLinkedPlotControlVM inflowOutflowControl = new Plots.IndividualLinkedPlotControlVM(new Plots.ConditionsIndividualPlotWrapperVM(true, false, "Inflow Outflow", "Inflow", "OutFlow"), new Plots.IndividualLinkedPlotCoverButtonVM(), new AddInflowOutflowToConditionVM(listOfInfOut, this), new Plots.DoubleLineModulatorCoverButtonVM(), new Plots.DoubleLineModulatorWrapperVM());
            //Plots.IndividualLinkedPlotControlVM DLMControl = new Plots.IndividualLinkedPlotControlVM(new Plots.DoubleLineModulatorWrapperVM(), new Plots.DoubleLineModulatorCoverButtonVM(), new AddInflowOutflowToConditionVM(listOfInfOut, this));
            Plots.IndividualLinkedPlotControlVM ratingControl = new Plots.IndividualLinkedPlotControlVM(new Plots.ConditionsIndividualPlotWrapperVM(true,false,"Rating","Exterior Stage","OutFlow"), new Plots.IndividualLinkedPlotCoverButtonVM(), new AddRatingCurveToConditionVM(ratingeles, this));
            Plots.IndividualLinkedPlotControlVM extIntStageControl = new Plots.IndividualLinkedPlotControlVM(new Plots.ConditionsIndividualPlotWrapperVM(true, false, "Exterior Interior Stage", "Exterior Stage", "Interior Stage"), new Plots.IndividualLinkedPlotCoverButtonVM(), new AddExteriorInteriorStageToConditionVM(listOfExtIntElements, this), new Plots.DoubleLineModulatorHorizontalCoverButtonVM(), new Plots.HorizontalDoubleLineModulatorWrapperVM());

            Plots.IndividualLinkedPlotControlVM StageDamageControl = new Plots.IndividualLinkedPlotControlVM(new Plots.ConditionsIndividualPlotWrapperVM(true, false, "Stage Damage", "Interior Stage", "Damage"), new Plots.IndividualLinkedPlotCoverButtonVM(), new AddStageDamageToConditionVM(listOfStageDamage, this));

            //test.AddFlowFreqVM = new AddFlowFrequencyToConditionVM(listOfLp3, this);
            //test.CoverButtonVM = new Plots.IndividualLinkedPlotCoverButtonVM(test);
            //test.MyIndividualLinkedPlotVM = new Plots.IndividualLinkedPlotVM();
            //test.CurrentVM = test.CoverButtonVM;
            ConditionsPlotEditorVM vm2 = new ConditionsPlotEditorVM(lp3Control, inflowOutflowControl, ratingControl, extIntStageControl, StageDamageControl);

            //need to attach handlers in order to open the importers into their own windows.
            //vm2.RequestNavigation += Navigate;

            Navigate(vm2);

            if (!vm2.WasCancled)
            {
                if (!vm2.HasError)
                {
                    //ConditionsElement ce = new ConditionsElement(vm2.Name,vm2.Description,vm2.Year,vm2.SelectedImpactArea,null,vm2.IsPlot0Visible,vm2.Plot0VM.)
                    
                        //ConditionsElement ce = new ConditionsElement(vm.Name,vm.Description,vm.AnalysisYear,vm.ImpactAreaSet,vm.ImpactArea,vm.UseAnalyticalFlowFrequency,vm.AnalyticalFlowFrequency, vm.UsesInflowOutflow,vm.InflowOutflowElement, vm.UseRatingCurve,vm.RatingCurve, vm.UsesExteriorInterior,vm.ExteriorInteriorElement,vm.UsesLevee,vm.LeveeElement,vm.UsesFailureFunction,vm.FailureFunctionElement, vm.UseAggregatedStageDamage,vm.StageDamage,vm.UseThreshold, vm.ThresholdType,vm.ThresholdValue,this);
                    //AddElement(ce);
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
            return new string[] { "Name" };
        }

        public override Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string) };
        }

        public override void AddElement(object[] rowData)
        {
           // throw new NotImplementedException();
        }
        #endregion
    }
}
