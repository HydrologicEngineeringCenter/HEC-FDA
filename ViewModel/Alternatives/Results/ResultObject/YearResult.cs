using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Alternatives.Results.ResultObject
{
    public class YearResult
    {

        //private AAEQDamageWithUncertaintyVM _AAEQDamageWithUncertaintyVM;
        //private AAEQDamageByImpactAreaVM _AAEQDamageByImpactAreaVM;
        //private AAEQDamageByDamCatVM _AAEQDamageByDamCatVM;
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
