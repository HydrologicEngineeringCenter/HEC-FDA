using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results;
public class UncertaintyControlConfigs
{
    /// <summary>
    /// Defines the properties which the "Consequence with Uncertainty" controls must have, unique to each consequence type
    /// </summary>
    public interface IUncertaintyControlConfig
    {
        string PlotTitle { get; }
        string YAxisTitle { get; }
        string MeanFormat { get; }
        string YAxisFormat { get; }
        string TrackerFormat { get; }
        string QuartileHeaderText { get; }
    }

    public class DamageWithUncertaintyControlConfig : IUncertaintyControlConfig
    {
        public string PlotTitle => StringConstants.EAD_DISTRIBUTION;
        public string YAxisTitle => StringConstants.EXPECTED_ANNUAL_DAMAGE;
        public string MeanFormat => "C2";
        public string YAxisFormat => "C0";
        public string TrackerFormat => "X: {Probability:0.####}, Y: {Value:C0}";
        public string QuartileHeaderText => "Quartile of EAD Distribution";
    }
}
