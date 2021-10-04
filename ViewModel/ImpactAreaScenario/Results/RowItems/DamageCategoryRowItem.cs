using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ImpactAreaScenario.Results.RowItems
{
    public class DamageCategoryRowItem
    {
        public string DamageCategory { get; set; }
        public double EAD { get; set; }

        public DamageCategoryRowItem(string damCat, double ead)
        {
            EAD = ead;
            DamageCategory = damCat;
        }

    }
}
