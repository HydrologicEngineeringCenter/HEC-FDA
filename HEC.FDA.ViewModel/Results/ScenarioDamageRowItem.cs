using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility.Memory;

namespace HEC.FDA.ViewModel.Results
{
    public class ScenarioDamageRowItem
    {
        [DisplayAsColumn("Name")]
        public string Name { get; set; }
        [DisplayAsColumn("Analysis Year")]
        public string AnalysisYear { get; set; }
        [DisplayAsColumn("Impact Area")]
        public string ImpactArea { get; set; }
        [DisplayAsColumn("Damage Category")]
        public string DamageCategory { get; set; }
        [DisplayAsColumn("Mean")]
        public double Mean { get; set; }
        [DisplayAsColumn("Q1")]
        public double Q1 { get; set; }
        [DisplayAsColumn("Q2")]
        public double Q2 { get; set; }
        [DisplayAsColumn("Q3")]
        public double Q3 { get; set; }

        private ScenarioDamageRowItem(string name, string analysisYear, string impactArea, string damageCategory, double mean, double q1, double q2, double q3)
        {
            Name = name;
            AnalysisYear = analysisYear;
            ImpactArea = impactArea;
            DamageCategory = damageCategory;
            Mean = mean;
            Q1 = q1;
            Q2 = q2;
            Q3 = q3;
        }

        public static List<ScenarioDamageRowItem> CreateScenarioDamageRowItems(IASElement scenario)
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
            List<ScenarioDamageRowItem> rowItems = new(rowsPerScenario);

            foreach (int impactAreaID in impactAreaIds)
            {
                foreach (string damcat in damCats)
                {
                    double Mean = results.MeanExpectedAnnualConsequences(impactAreaID, damcat);
                    double Q1 = results.ConsequencesExceededWithProbabilityQ(.75, impactAreaID, damcat);
                    double Q2 = results.ConsequencesExceededWithProbabilityQ(.50, impactAreaID, damcat);
                    double Q3 = results.ConsequencesExceededWithProbabilityQ(.25, impactAreaID, damcat);
                    rowItems.Add(new(name, analysisYear, impactAreaIdToName[impactAreaID], damcat, Mean, Q1, Q2, Q3));
                }
            }
            return rowItems;
        }

    }
}
