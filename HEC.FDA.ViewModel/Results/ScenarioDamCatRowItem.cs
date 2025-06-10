using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
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
        [DisplayAsColumn("Scenario Name")]
        public string Name { get; set; }
        [DisplayAsColumn("Analysis Year")]
        public string AnalysisYear { get; set; }
        [DisplayAsColumn("Impact Area")]
        public string ImpactAreaName { get; set; }
        [DisplayAsColumn("Damage Category")]
        public string DamCat { get; set; }
        [DisplayAsColumn("Asset Category")]
        public string AssetCat { get; set; }
        [DisplayAsColumn("Mean EAD")]
        public double MeanDamage { get; set; }

        private ScenarioDamCatRowItem(string name, string analysisYear, string impactAreaName, string damcat, string assetCat, double mean)
        {
            Name = name;
            AnalysisYear = analysisYear;
            ImpactAreaName = impactAreaName;
            DamCat = damcat;
            AssetCat = assetCat;
            MeanDamage = mean;
        }

        public static List<ScenarioDamCatRowItem> CreateScenarioDamCatRowItems(IASElement scenario)
        {
            string name = scenario.Name;
            string analysisYear = scenario.AnalysisYear;

            ScenarioResults results = scenario.Results;
            List<int> impactAreaIds = results.GetImpactAreaIDs();
            Dictionary<int, string> impactAreaIdToName = IASElement.GetImpactAreaNamesFromIDs();
            List<string> damCats = results.GetDamageCategories();
            List<string> assetCats = results.GetAssetCategories();

            List<ScenarioDamCatRowItem> rowItems = [];
            foreach (int impactAreaID in impactAreaIds)
            {
                foreach (string damCat in damCats)
                {
                    foreach (string assetCat in assetCats)
                    {
                        double mean = results.SampleMeanExpectedAnnualConsequences(impactAreaID, damCat, assetCat);
                        ScenarioDamCatRowItem row = new(name, analysisYear, impactAreaIdToName[impactAreaID], damCat, assetCat, mean);
                        rowItems.Add(row);
                    }
                }
            }
            return rowItems;
        }
    }
}
