using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Conditions
{
    class ConditionFactory
    {

        public static ConditionsElement BuildConditionsElement(ConditionsPlotEditorVM vm, ConditionsOwnerElement owner)
        {
            ConditionBuilder builder = new ConditionBuilder(vm.Name, vm.Description, vm.Year, vm.SelectedImpactArea, vm.IndexLocation, owner);
            
            foreach(Plots.IndividualLinkedPlotControlVM control in vm.AddedPlots)
            {
                switch(control.IndividualPlotWrapperVM.PlotVM.BaseFunction.FunctionType)
                {
                    
                    case FdaModel.Functions.FunctionTypes.InflowFrequency:
                    case FdaModel.Functions.FunctionTypes.OutflowFrequency:
                        builder.WithAnalyticalFreqElem((FrequencyRelationships.AnalyticalFrequencyElement)control.CurveImporterVM.SelectedElement);
                        break;
                    case FdaModel.Functions.FunctionTypes.InflowOutflow:
                        builder.WithInflowOutflowElem((FlowTransforms.InflowOutflowElement)control.CurveImporterVM.SelectedElement);
                        break;
                    case FdaModel.Functions.FunctionTypes.Rating:
                        builder.WithRatingCurveElem((StageTransforms.RatingCurveElement)control.CurveImporterVM.SelectedElement);
                        break;
                    case FdaModel.Functions.FunctionTypes.ExteriorInteriorStage:
                        builder.WithExtIntStageElem((StageTransforms.ExteriorInteriorElement)control.CurveImporterVM.SelectedElement);
                        break;
                    case FdaModel.Functions.FunctionTypes.InteriorStageDamage:
                        builder.WithAggStageDamageElem((AggregatedStageDamage.AggregatedStageDamageElement)control.CurveImporterVM.SelectedElement);
                        break;
                        
                }
            }
            //builder.WithAnalyticalFreqElem(vm.a);

            return builder.build();
        }

    }
}
