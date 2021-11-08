using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Alternatives.Results.ResultObject
{
    public class AAEQResult
    {
        private DamageWithUncertaintyVM _DamageWithUncertaintyVM;
        private DamageByImpactAreaVM _DamageByImpactAreaVM;
        private DamageByDamCatVM _DamageByDamCatVM;

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
