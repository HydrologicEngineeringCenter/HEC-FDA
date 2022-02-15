using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HEC.FDA.ViewModel.ImpactArea;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class Results
    {
        public DamageWithUncertaintyVM DamageWithUncertaintyVM { get; }
        public DamageByDamageCategoryVM DamageByDamageCategoryVM { get; }
        public PerformanceVMBase PerformanceAEPVM { get; }
        public PerformanceVMBase PerformanceAssuranceOfThresholdVM { get; }
        public PerformanceVMBase PerformanceLongTermRiskVM { get; }
        public ImpactAreaRowItem ImpactArea { get; }

        public Results(DamageWithUncertaintyVM damageWithUncertainty, DamageByDamageCategoryVM damageByDamageCategory,
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
