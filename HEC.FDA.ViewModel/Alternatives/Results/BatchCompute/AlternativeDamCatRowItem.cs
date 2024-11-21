using HEC.FDA.Model.scenarios;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Results;
using HEC.FDA.ViewModel.Study;
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

        public string Name { get; set; }
        public string ImpactArea { get; set; }
        public string AssetCategory { get; set; }


        public Dictionary<string, double> DamCatMap = new Dictionary<string, double>();
        

        private AlternativeDamCatRowItem(string name, string impactArea, string assetCategory, Dictionary<string, double> damCatToMean)
        {
            Name = name;
            DamCatMap = damCatToMean;
            ImpactArea = impactArea;
            AssetCategory = assetCategory;
        }

        public static List<AlternativeDamCatRowItem> CreateAlternativeDamCatRowItems(AlternativeElement altElem) 
        {

            string Name = altElem.Name;
            IASElement BaseYearScenario = altElem.BaseScenario.GetElement();
            IASElement FutureYearScenario = altElem.FutureScenario.GetElement();

            StudyPropertiesElement studyPropElem = StudyCache.GetStudyPropertiesElement();
            double DiscountRate = studyPropElem.DiscountRate;
            int PeriodOfAnalysis = altElem.Results.PeriodOfAnalysis;
            List<int> impactAreaIds = BaseYearScenario.Results.GetImpactAreaIDs();
            List<string> impactAreaNames = BaseYearScenario.SpecificIASElements.Select(x => x.ImpactAreaName).ToList();
            Dictionary<int, string> impactAreaIdToName = [];
            for (int i = 0; i < impactAreaIds.Count; i++)
            {
                impactAreaIdToName.Add(impactAreaIds[i], impactAreaNames[i]);
            }

            int rowsPerScenario = 2 * impactAreaIds.Count;
            List<AlternativeDamCatRowItem> rowItems = new(rowsPerScenario);

            foreach (int impactAreaID in impactAreaIds)
            {
                Dictionary<string, double> damCatToMean = new();
                List<string> damCats = altElem.Results.GetDamageCategories();
                List<string> assetCats = altElem.Results.GetAssetCategories();
                foreach (string damCat in damCats)
                {
                    foreach(string assetCat in assetCats)
                    {
                        Empirical damCatValue = altElem.Results.GetAAEQDamageDistribution(impactAreaID, damCat, assetCat);
                        damCatToMean.Add(damCat, Math.Round(damCatValue.Mean, 2));
                    }
                }
                //AlternativeDamCatRowItem row = new(Name, impactAreaIdToName[impactAreaID], assetCat, damCatToMean);
                //rowItems.Add(row);
            }
            return rowItems;
        }
    }
}
