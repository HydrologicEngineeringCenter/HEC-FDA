using FdaViewModel.Utilities;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Conditions
{
    class ConditionsElementFactory
    {
        //todo: Refactor: CO
        //public static ConditionsElement BuildConditionsElement(ConditionsPlotEditorVM vm)
        //{
        //    if (vm.Description == null) { vm.Description = ""; }
        //    ConditionBuilder builder = new ConditionBuilder(vm.Name, vm.Description, vm.Year, vm.SelectedImpactArea, vm.IndexLocation,
        //        vm.SelectedThresholdType, vm.ThresholdValue);

        //    foreach (Plots.IndividualLinkedPlotControlVM control in vm.AddedPlots)
        //    {
        //        switch (control.IndividualPlotWrapperVM.PlotVM.BaseFunction.FunctionType)
        //        {

        //            case FdaModel.Functions.FunctionTypes.InflowFrequency:
        //            case FdaModel.Functions.FunctionTypes.OutflowFrequency:
        //                builder.WithAnalyticalFreqElem((FrequencyRelationships.AnalyticalFrequencyElement)control.CurveImporterVM.SelectedElement);
        //                break;
        //            case FdaModel.Functions.FunctionTypes.InflowOutflow:
        //                builder.WithInflowOutflowElem((FlowTransforms.InflowOutflowElement)control.CurveImporterVM.SelectedElement);
        //                break;
        //            case FdaModel.Functions.FunctionTypes.Rating:
        //                builder.WithRatingCurveElem((StageTransforms.RatingCurveElement)control.CurveImporterVM.SelectedElement);
        //                break;
        //            case FdaModel.Functions.FunctionTypes.ExteriorInteriorStage:
        //                builder.WithExtIntStageElem((StageTransforms.ExteriorInteriorElement)control.CurveImporterVM.SelectedElement);
        //                break;
        //            case FdaModel.Functions.FunctionTypes.InteriorStageDamage:
        //                builder.WithAggStageDamageElem((AggregatedStageDamage.AggregatedStageDamageElement)control.CurveImporterVM.SelectedElement);
        //                break;

        //        }
        //    }
        //    return builder.build();
        //}

        //public static ConditionsElement Factory(ICondition condition)
        //{
        //    ConditionBuilder builder = new ConditionBuilder(condition)
        //}

        //public static void CopyConditionsElement(ConditionsElement fromElem, ConditionsElement toElem)
        //{

        //    toElem.Name = fromElem.Name;
        //    toElem.Description = fromElem.Description;
        //    toElem.AnalysisYear = fromElem.AnalysisYear;
        //    toElem.ImpactArea = fromElem.ImpactArea;
        //    toElem.ImpactAreaElement = fromElem.ImpactAreaElement;

        //    toElem.UseAnalyiticalFlowFrequency = fromElem.UseAnalyiticalFlowFrequency;
        //    toElem.AnalyticalFlowFrequency = fromElem.AnalyticalFlowFrequency;

        //    toElem.UseInflowOutflow = fromElem.UseInflowOutflow;
        //    toElem.InflowOutflowElement = fromElem.InflowOutflowElement;

        //    toElem.UseRatingCurve = fromElem.UseRatingCurve;
        //    toElem.RatingCurveElement = fromElem.RatingCurveElement;

        //    toElem.UseExteriorInteriorStage = fromElem.UseExteriorInteriorStage;
        //    toElem.ExteriorInteriorElement = fromElem.ExteriorInteriorElement;

        //    toElem.UseLevee = fromElem.UseLevee;
        //    toElem.LeveeElement = fromElem.LeveeElement;

        //    toElem.UseFailureFunction = fromElem.UseFailureFunction;
        //    toElem.FailureFunctionElement = fromElem.FailureFunctionElement;

        //    toElem.UseAggregatedStageDamage = fromElem.UseAggregatedStageDamage;
        //    toElem.StageDamageElement = fromElem.StageDamageElement;

        //    toElem.UseThreshold = fromElem.UseThreshold;
        //    toElem.ThresholdType = fromElem.ThresholdType;
        //    toElem.ThresholdValue = fromElem.ThresholdValue;

        //}
    }
}
