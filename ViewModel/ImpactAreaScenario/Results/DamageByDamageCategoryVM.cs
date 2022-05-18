using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using metrics;
using Statistics.Histograms;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class DamageByDamageCategoryVM : BaseViewModel
    {
        public List<DamageCategoryRowItem> Rows { get; set; }

        public DamageByDamageCategoryVM(metrics.ImpactAreaScenarioResults iasResult)
        {
            ConsequenceResults eadResults = iasResult.ConsequenceResults;
            LoadDamCatTable(eadResults);
        }

        private void LoadDamCatTable(ConsequenceResults eadResults)
        {
            List<string> xVals = new List<string>();
            List<double> yVals = new List<double>();

            //todo: richard is updating this 
            //foreach(KeyValuePair<string, ThreadsafeInlineHistogram> entry in eadResults.MeanDamage("test", "test", 1)
            //{
            //    xVals.Add(entry.Key);
            //    yVals.Add(entry.Value.Mean);
            //}

            List<DamageCategoryRowItem> rows = new List<DamageCategoryRowItem>();
            for (int i = 0; i < xVals.Count; i++)
            {
                rows.Add(new DamageCategoryRowItem(xVals[i], yVals[i]));
            }

            Rows = rows;
        }

    }
}
