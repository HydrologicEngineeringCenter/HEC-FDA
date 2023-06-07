using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Results
{
    public class ScenarioDamCatRowItem
    {
        public string Name { get; set; }
        public string AnalysisYear { get; set; }
        public Dictionary<string, double> DamCatMap = new Dictionary<string, double>();

        public ScenarioDamCatRowItem(IASElement scenario)
        {
            Name = scenario.Name;
            AnalysisYear = scenario.AnalysisYear;
            ScenarioResults results = scenario.Results;

            List<string> damCats = results.GetDamageCategories();
            foreach (string damCat in damCats)
            {
                Empirical damCatValue = results.GetConsequencesDistribution(damageCategory: damCat);
                DamCatMap.Add(damCat, Math.Round(damCatValue.Mean,2));
            }
        }

    }
}
