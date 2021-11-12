
namespace ViewModel.Alternatives.Results.ResultObject
{
    public class YearResult
    {
        public DamageWithUncertaintyVM DamageWithUncertaintyVM { get; set; }
        public DamageByImpactAreaVM DamageByImpactAreaVM { get; set; }
        public DamageByDamCatVM DamageByDamCatVM { get; set; }
        public int Year { get; set; }
        public YearResult(int year, DamageWithUncertaintyVM damWithUncert, DamageByImpactAreaVM damByImpactArea,
            DamageByDamCatVM damByDamCat)
        {
            Year = year;
            DamageWithUncertaintyVM = damWithUncert;
            DamageByImpactAreaVM = damByImpactArea;
            DamageByDamCatVM = damByDamCat;
        }

    }
}
