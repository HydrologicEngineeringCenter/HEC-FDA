using paireddata;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class ImpactAreaFrequencyFunctionConfigurationRowItem
    {
        public string ImpactAreaName { get; }
        public UncertainPairedData FrequencyFunction { get; }
        public UncertainPairedData StageDischargeFunction { get; }
        public bool IsStageDischargeFunctionRequired { get { return StageDischargeFunction != null; } }

        public ImpactAreaFrequencyFunctionConfigurationRowItem(string impactAreaName, UncertainPairedData frequencyFunction, UncertainPairedData stageDischarge)
        {
            ImpactAreaName = impactAreaName;
            FrequencyFunction = frequencyFunction;
            StageDischargeFunction = stageDischarge;
        }

    }
}
