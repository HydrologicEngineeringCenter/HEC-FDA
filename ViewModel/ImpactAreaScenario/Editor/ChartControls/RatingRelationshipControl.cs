using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor.ChartControls
{
    public class RatingRelationshipControl : ChartControlBase
    {

        public RatingRelationshipControl()
            : base(StringConstants.STAGE_DISCHARGE, StringConstants.STAGE, StringConstants.DISCHARGE, StringConstants.STAGE_DISCHARGE, flipXY: true, xAxisAlignment: HEC.Plotting.Core.DataModel.AxisAlignment.Top)
        {

        }

    }
}
