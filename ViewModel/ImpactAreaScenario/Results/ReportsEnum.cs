using System.ComponentModel;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public enum ReportsEnum
    {

        [Description("Damage with Uncertainty")]
        Damage_With_Uncertainty,

        [Description("Damage by Damage Category")]
        Damage_By_Damage_Category,

        [Description("Annual Exceedance Probability")]
        Annual_Exceedance_Probability,

        [Description("Long-term Risk")]
        Long_Term_Risk,

        [Description("Assurance of Threshold")]
        Assurance_Of_Threshold,
    }
  
}
