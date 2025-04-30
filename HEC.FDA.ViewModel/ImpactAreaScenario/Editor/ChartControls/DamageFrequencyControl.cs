using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor.ChartControls;

public class DamageFrequencyControl : ChartControlBase
{

    public DamageFrequencyControl()
        : base(StringConstants.DAMAGE_FREQUENCY, StringConstants.EXCEEDANCE_PROBABILITY, StringConstants.DAMAGE, StringConstants.DAMAGE_FREQUENCY, useProbabilityX: true, yAxisAlignment: HEC.Plotting.Core.DataModel.AxisAlignment.Right, inverseXAxisProbabilities: true)
    {

    }
}
