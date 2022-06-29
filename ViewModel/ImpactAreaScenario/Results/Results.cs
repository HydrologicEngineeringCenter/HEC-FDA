using HEC.FDA.ViewModel.Alternatives.Results;
using HEC.FDA.ViewModel.ImpactArea;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class Results
    {
        public DamageWithUncertaintyVM DamageWithUncertaintyVM { get; }
        public DamageByDamCatVM DamageByDamageCategoryVM { get; }
        public PerformanceVMBase PerformanceAEPVM { get; }
        public PerformanceVMBase PerformanceAssuranceOfThresholdVM { get; }
        public PerformanceVMBase PerformanceLongTermRiskVM { get; }
        public ImpactArea.ImpactAreaRowItem ImpactArea { get; }

        public Results(DamageWithUncertaintyVM damageWithUncertainty, DamageByDamCatVM damageByDamageCategory,
            PerformanceVMBase performanceAEP, PerformanceVMBase performanceAssuranceOfThreshold, PerformanceVMBase performanceLongTermRisk)
        {
            DamageWithUncertaintyVM = damageWithUncertainty;
            DamageByDamageCategoryVM = damageByDamageCategory;
            PerformanceAEPVM = performanceAEP;
            PerformanceAssuranceOfThresholdVM = performanceAssuranceOfThreshold;
            PerformanceLongTermRiskVM = performanceLongTermRisk;
        }



    }
}
