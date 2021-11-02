using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.ImpactArea;

namespace ViewModel.ImpactAreaScenario.Results
{
    public class Results
    {
        public  DamageWithUncertaintyVM DamageWithUncertaintyVM { get; set; }
        public DamageByDamageCategoryVM DamageByDamageCategoryVM { get; set; }
        public PerformanceVMBase PerformanceAEPVM { get; set; }
        public PerformanceVMBase PerformanceAssuranceOfThresholdVM { get; set; }
        public PerformanceVMBase PerformanceLongTermRiskVM { get; set; }
        public ImpactAreaRowItem ImpactArea { get; set; }

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
