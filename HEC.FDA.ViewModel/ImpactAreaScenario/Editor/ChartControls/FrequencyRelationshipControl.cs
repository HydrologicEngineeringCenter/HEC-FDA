using HEC.FDA.ViewModel.Utilities;
using HEC.Plotting.Core.DataModel;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor.ChartControls
{
    public class FrequencyRelationshipControl : ChartControlBase
    {


        public FrequencyRelationshipControl()
            : base(StringConstants.FREQUENCY_RELATIONSHIP, StringConstants.EXCEEDANCE_PROBABILITY, StringConstants.DISCHARGE, StringConstants.FREQUENCY_RELATIONSHIP, useProbabilityX: true, xAxisAlignment: AxisAlignment.Top, yAxisAlignment: AxisAlignment.Right)
        {
            
        }

        public void UpdateYAxisLabel()
        {
            if (Function != null)
            {
                UpdateYAxisLabel(Function.MetaData.YLabel);
            }
        }

    }
}
