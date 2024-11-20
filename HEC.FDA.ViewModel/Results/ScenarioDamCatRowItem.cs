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
        public string ImpactAreaName { get; set; }
        public Dictionary<string, double> DamCatMap = new();

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

        private ScenarioDamCatRowItem(string name, string analysisYear, string impactAreaName, Dictionary<string, double> DamCatToMeanDamage)
        {
            Name = name;
            AnalysisYear = analysisYear;
            ImpactAreaName = impactAreaName;
            DamCatMap = DamCatToMeanDamage;
        }

        public static List<ScenarioDamCatRowItem> CreateScenarioDamCatRowItems(IASElement scenario)
        {
            //shared props
            string name = scenario.Name;
            string analysisYear = scenario.AnalysisYear;

            ScenarioResults results = scenario.Results;
            List<int> impactAreaIds = results.GetImpactAreaIDs();
            List<string> impactAreaNames = scenario.SpecificIASElements.Select(x => x.ImpactAreaName).ToList();
            Dictionary<int, string> impactAreaIdToName = [];
            for (int i = 0; i < impactAreaIds.Count; i++)
            {
                impactAreaIdToName.Add(impactAreaIds[i], impactAreaNames[i]);
            }
            List<string> damCats = results.GetDamageCategories();
            int rowsPerScenario = damCats.Count * impactAreaIds.Count;
            List<ScenarioDamCatRowItem> rowItems = new(rowsPerScenario);
            foreach (int impactAreaID in impactAreaIds)
            {
                Dictionary<string, double> damCatToMean = [];
                foreach (string damcat in damCats)
                {
                        double mean = results.MeanExpectedAnnualConsequences(impactAreaID, damcat);
                    damCatToMean.Add(damcat, mean);
                }
                ScenarioDamCatRowItem row = new(name, analysisYear, impactAreaIdToName[impactAreaID], damCatToMean);
                rowItems.Add(row);
            }
            return rowItems;
        }
    }
}
