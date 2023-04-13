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
    public class ScenarioDamageRowItem
    {

        public ScenarioDamageRowItem(IASElement scenario)
        {
            string name = scenario.Name;
            int analysisYear = scenario.AnalysisYear;
            ScenarioResults results = scenario.Results;
            double mean = results.MeanExpectedAnnualConsequences();
            double quarter1 = results.ConsequencesExceededWithProbabilityQ(.75);
            double quarter2 = results.ConsequencesExceededWithProbabilityQ(.5);
            double quarter3 = results.ConsequencesExceededWithProbabilityQ(.25);
            List<string> damCats = results.GetDamageCategories();
            foreach (string damCat in damCats)
            {
                Empirical damCatValue = results.GetConsequencesDistribution(damageCategory: damCat);
            }

                
        }

    }
}
