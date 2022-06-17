using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using metrics;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class DamageByDamageCategoryVM : BaseViewModel
    {
        public List<DamageCategoryRowItem> Rows { get; set; }

        public DamageByDamageCategoryVM(ImpactAreaScenarioResults iasResult, List<string> damCats)
        {       
            ConsequenceResults eadResults = iasResult.ConsequenceResults;
            LoadDamCatTable(eadResults, damCats);
        }

        private void LoadDamCatTable(ConsequenceResults eadResults, List<string> damCats)
        {
            List<DamageCategoryRowItem> rows = new List<DamageCategoryRowItem>();

            foreach(string damCat in damCats)
            {
                double yVal = eadResults.MeanDamage(damageCategory: damCat);
                rows.Add(new DamageCategoryRowItem(damCat, yVal));
            }

            Rows = rows;
        }

    }
}
