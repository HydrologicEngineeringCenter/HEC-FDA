
namespace ViewModel.Alternatives.Results.ResultObject
{
    public class AAEQResult
    {
        public DamageWithUncertaintyVM DamageWithUncertaintyVM { get; set; }
        public DamageByImpactAreaVM DamageByImpactAreaVM { get; set; }
        public DamageByDamCatVM DamageByDamCatVM { get; set; }

        public AAEQResult(DamageWithUncertaintyVM damWithUncert, DamageByImpactAreaVM damByImpactArea,
            DamageByDamCatVM damByDamCat)
        {
            DamageWithUncertaintyVM = damWithUncert;
            DamageByImpactAreaVM = damByImpactArea;
            DamageByDamCatVM = damByDamCat;
        }

    }
}
