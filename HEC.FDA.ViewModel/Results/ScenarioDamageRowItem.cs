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
        public string Name { get; set; }
        public int AnalysisYear { get; set; }
        public double Mean { get; set; }
        public double Q1 { get; set; }
        public double Q2 { get; set; }
        public double Q3 { get; set; }


        public ScenarioDamageRowItem(IASElement scenario)
        {
            Name = scenario.Name;
            AnalysisYear = scenario.AnalysisYear;
            ScenarioResults results = scenario.Results;
            Mean = results.MeanExpectedAnnualConsequences();
            Q1 = results.ConsequencesExceededWithProbabilityQ(.75);
            Q2 = results.ConsequencesExceededWithProbabilityQ(.5);
            Q3 = results.ConsequencesExceededWithProbabilityQ(.25);
            List<string> damCats = results.GetDamageCategories();
            foreach (string damCat in damCats)
            {
                Empirical damCatValue = results.GetConsequencesDistribution(damageCategory: damCat);
            }

                
        }

    }
}
