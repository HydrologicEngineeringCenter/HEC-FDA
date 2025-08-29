
namespace HEC.FDA.ViewModel.Alternatives.Results.ResultObject
{
    public class EqadResult
    {
        public DamageWithUncertaintyVM DamageWithUncertaintyVM { get;  }
        public DamageByImpactAreaVM DamageByImpactAreaVM { get;  }
        public DamageByDamCatVM DamageByDamCatVM { get;  }

        public EqadResult(DamageWithUncertaintyVM damWithUncert, DamageByImpactAreaVM damByImpactArea,
            DamageByDamCatVM damByDamCat)
        {
            DamageWithUncertaintyVM = damWithUncert;
            DamageByImpactAreaVM = damByImpactArea;
            DamageByDamCatVM = damByDamCat;
        }

    }
}
