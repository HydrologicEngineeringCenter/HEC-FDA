
namespace HEC.FDA.ViewModel.Alternatives.Results.ResultObject
{
    public class AAEQResult
    {
        public DamageWithUncertaintyVM DamageWithUncertaintyVM { get;  }
        public DamageByImpactAreaVM DamageByImpactAreaVM { get;  }
        public DamageByDamCatVM DamageByDamCatVM { get;  }

        public AAEQResult(DamageWithUncertaintyVM damWithUncert, DamageByImpactAreaVM damByImpactArea,
            DamageByDamCatVM damByDamCat)
        {
            DamageWithUncertaintyVM = damWithUncert;
            DamageByImpactAreaVM = damByImpactArea;
            DamageByDamCatVM = damByDamCat;
        }

    }
}
