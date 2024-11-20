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
        public double DiscountRate { get; set; }
        public int PeriodOfAnalysis { get; set; }
        public string ImpactArea { get; set; }
        public double Mean { get; set; }
        public double Q1 { get; set; }
        public double Q2 { get; set; }
        public double Q3 { get; set; }

        public Dictionary<string, double> DamCatMap = new Dictionary<string, double>();
        

        private AlternativeDamCatRowItem(string name, string impactArea, double discountRate, int periodOfAnalysis, Dictionary<string, double> damCatToMean)
        {
            Name = name;
            DiscountRate = discountRate;
            PeriodOfAnalysis = periodOfAnalysis;
            DamCatMap = damCatToMean;
            ImpactArea = impactArea;
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
                foreach (string damCat in damCats)
                {
                    Empirical damCatValue = altElem.Results.GetAAEQDamageDistribution(impactAreaID, damCat);
                    damCatToMean.Add(damCat, Math.Round(damCatValue.Mean, 2));
                }
                AlternativeDamCatRowItem row = new(Name, impactAreaIdToName[impactAreaID], DiscountRate, PeriodOfAnalysis, damCatToMean);
                rowItems.Add(row);
            }
            return rowItems;
        }
    }
}
