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
        [DisplayAsColumn("Mean EAD")]
        public double Mean { get; set; }
        [DisplayAsColumn("25th Percentile EAD")]
        public double Point25 { get; set; }
        [DisplayAsColumn("50th Percentile EAD")]
        public double Point5 { get; set; }
        [DisplayAsColumn("75th Percentile EAD")]
        public double Point75 { get; set; }


        private ScenarioDamageRowItem(string name, string analysisYear, string impactArea, double mean, double point75, double point5, double point25)
        {
            Name = name;
            AnalysisYear = analysisYear;
            ImpactArea = impactArea;
            Mean = mean;
            Point75 = point75;
            Point5 = point5;
            Point25 = point25;
        }

        public static List<ScenarioDamageRowItem> CreateScenarioDamageRowItems(IASElement scenario)
        {
            List<ScenarioDamageRowItem> rowItems = [];

            string name = scenario.Name;
            string analysisYear = scenario.AnalysisYear;
            ScenarioResults results = scenario.Results;
            List<int> impactAreaIds = results.GetImpactAreaIDs();
            Dictionary<int, string> impactAreaIdToName = IASElement.GetImpactAreaNamesFromIDs();
           
            foreach (int impactAreaID in impactAreaIds)
            {
                    double Mean = results.SampleMeanExpectedAnnualConsequences(impactAreaID);
                    double point75 = results.ConsequencesExceededWithProbabilityQ(.75, impactAreaID);
                    double point5 = results.ConsequencesExceededWithProbabilityQ(.50, impactAreaID);
                    double point25 = results.ConsequencesExceededWithProbabilityQ(.25, impactAreaID);
                    rowItems.Add(new(name, analysisYear, impactAreaIdToName[impactAreaID], Mean, point75, point5, point25));
            }
            return rowItems;
        }

    }
}
