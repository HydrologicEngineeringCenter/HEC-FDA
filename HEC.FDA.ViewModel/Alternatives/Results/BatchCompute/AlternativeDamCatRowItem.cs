using HEC.FDA.Model.scenarios;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Results;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Alternatives.Results.BatchCompute
{
    public class AlternativeDamCatRowItem : BaseViewModel
    {

        [DisplayAsColumn("Scenario Name")]
        public string Name { get; set; }
        [DisplayAsColumn("Impact Area")]
        public string ImpactAreaName { get; set; }
        [DisplayAsColumn("Damage Category")]
        public string DamCat { get; set; }
        [DisplayAsColumn("Asset Category")]
        public string AssetCat { get; set; }
        [DisplayAsColumn("Mean EqAD")]
        public double MeanDamage { get; set; }

        private AlternativeDamCatRowItem(string name, string impactArea, string assetCategory, string damCat, double mean)
        {
            Name = name;
            ImpactAreaName = impactArea;
            AssetCat = assetCategory;
            DamCat = damCat;
            MeanDamage = mean;
        }

        public static List<AlternativeDamCatRowItem> CreateAlternativeDamCatRowItems(AlternativeElement altElem) 
        {
            List<AlternativeDamCatRowItem> rowItems = [];

            string Name = altElem.Name;
            IASElement BaseYearScenario = altElem.BaseScenario.GetElement();
            List<int> impactAreaIds = BaseYearScenario.Results.GetImpactAreaIDs();
            Dictionary<int, string> impactAreaIdToName = IASElement.GetImpactAreaNamesFromIDs();


            List<string> damCats = altElem.Results.GetDamageCategories();
            List<string> assetCats = altElem.Results.GetAssetCategories();
            foreach (int impactAreaID in impactAreaIds)
            {
                foreach (string damCat in damCats)
                {
                    foreach(string assetCat in assetCats)
                    {
                        Empirical dist = altElem.Results.GetAAEQDamageDistribution(impactAreaID, damCat, assetCat);
                        double mean = Math.Round(dist.Mean, 2);
                        AlternativeDamCatRowItem row = new(Name, impactAreaIdToName[impactAreaID], assetCat, damCat, mean);
                        rowItems.Add(row);
                    }
                }
            }
            return rowItems;
        }
    }
}
