using HEC.FDA.ViewModel.Utilities;
using HEC.Plotting.Core.DataModel;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor.ChartControls;

public class DamageFrequencyControl : ChartControlBase
{

    public DamageFrequencyControl()
        : base(StringConstants.DAMAGE_FREQUENCY,
               StringConstants.EXCEEDANCE_PROBABILITY,
               StringConstants.DAMAGE,
               StringConstants.DAMAGE_FREQUENCY,
               useProbabilityX: true,
               yAxisAlignment: AxisAlignment.Right)
    {

    }
}
