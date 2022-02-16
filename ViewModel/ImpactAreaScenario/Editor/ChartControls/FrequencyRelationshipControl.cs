using HEC.Plotting.Core.DataModel;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor.ChartControls
{
    public class FrequencyRelationshipControl : ChartControlBase
    {


        public FrequencyRelationshipControl()
            : base("FrequencyRelationship", "Frequency", "Flow", "Flow-Frequency", useProbabilityX: true, xAxisAlignment: AxisAlignment.Top, yAxisAlignment: AxisAlignment.Right)
        {
            
        }

        

    }
}
