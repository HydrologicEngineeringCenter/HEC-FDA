using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using metrics;

namespace HEC.FDA.ViewModel.Alternatives.Results
{
    public class DamageByDamCatVM : IAlternativeResult
    {
        public List<DamageCategoryRowItem> Rows { get; } = new List<DamageCategoryRowItem>();

        public DamageByDamCatVM(ScenarioResults scenarioResults)
        {
            List<string> damCats = scenarioResults.GetDamageCategories();
            foreach(string damCat in damCats)
            {
                Rows.Add(new DamageCategoryRowItem(damCat, scenarioResults.MeanExpectedAnnualConsequences(damageCategory: damCat)));
            }
        }

        public DamageByDamCatVM(AlternativeResults alternativeResults)
        {
            List<string> damCats =  alternativeResults.GetDamageCategories();
            foreach (string damCat in damCats)
            {
                Rows.Add(new DamageCategoryRowItem(damCat, alternativeResults.MeanConsequence(damageCategory: damCat)));
            }
        }

        public DamageByDamCatVM(AlternativeComparisonReportResults alternativeResults, int altID)
        {
            List<string> damCats = alternativeResults.GetDamageCategories();
            foreach (string damCat in damCats)
            {
                Rows.Add(new DamageCategoryRowItem(damCat, alternativeResults.MeanConsequencesReduced(altID, damageCategory: damCat)));
            }
        }
    }
}
