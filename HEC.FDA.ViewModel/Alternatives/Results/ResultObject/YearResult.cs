
namespace HEC.FDA.ViewModel.Alternatives.Results.ResultObject
{
    public class YearResult
    {
        public DamageWithUncertaintyVM DamageWithUncertaintyVM { get; set; }
        public DamageWithUncertaintyVM LifeLossWithUncertaintyVM { get; set; }
        public DamageByImpactAreaVM DamageByImpactAreaVM { get; set; }
        public DamageByImpactAreaVM LifeLossByImpactAreaVM { get; set; }
        public DamageByDamCatVM DamageByDamCatVM { get; set; }
        public int Year { get; set; }
        public YearResult(
            int year,
            DamageWithUncertaintyVM damWithUncert, DamageByImpactAreaVM damByImpactArea, DamageByDamCatVM damByDamCat,
            DamageWithUncertaintyVM lifeLossWithUncert, DamageByImpactAreaVM lifeLossByImpactArea)
        {
            Year = year;
            DamageWithUncertaintyVM = damWithUncert;
            DamageByImpactAreaVM = damByImpactArea;
            DamageByDamCatVM = damByDamCat;
            LifeLossWithUncertaintyVM = lifeLossWithUncert;
            LifeLossByImpactAreaVM = lifeLossByImpactArea;
        }

    }
}
