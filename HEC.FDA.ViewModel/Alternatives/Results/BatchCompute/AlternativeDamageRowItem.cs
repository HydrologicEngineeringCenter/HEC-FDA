using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Alternatives.Results.BatchCompute
{
    public class AlternativeDamageRowItem:BaseViewModel
    {
        [DisplayAsColumn("Name")]
        public string Name { get; set; }
        [DisplayAsColumn("Impact Area")]
        public string ImpactArea { get; set; }
        [DisplayAsColumn("Base Year")]
        public int BaseYear { get; set; }
        [DisplayAsColumn("Future Year")]
        public int FutureYear { get; set; }
        [DisplayAsColumn("Discount Rate")]
        public double DiscountRate { get; set; }
        [DisplayAsColumn("Period of Analysis")]
        public int PeriodOfAnalysis { get; set; }
        [DisplayAsColumn("Mean EqAD")]
        public double Mean { get; set; }
        [DisplayAsColumn("25th Percentile EqAD")] //This is intentionally swapped 1-x 
        public double Point75 { get; set; }
        [DisplayAsColumn("50th Percentile EqAD")]
        public double Point5 { get; set; }
        [DisplayAsColumn("75th Percentile EqAD")]//This is intentionally swapped 1-x 
        public double Point25 { get; set; }

        private AlternativeDamageRowItem(string name, string impactArea, int baseYear, int futureYear, double discountRate, int periodOfAnalysis, double mean, double point75, double point5, double point25)
        {
            Name = name;
            ImpactArea = impactArea;
            BaseYear = baseYear;
            FutureYear = futureYear;
            DiscountRate = discountRate;
            PeriodOfAnalysis = periodOfAnalysis;
            Mean = mean;
            Point75 = point75;
            Point5 = point5;
            Point25 = point25;
        }

        public static List<AlternativeDamageRowItem> CreateAlternativeDamageRowItems(AlternativeElement altElem)
        {
            string Name = altElem.Name;
            int baseYear = altElem.Results.AnalysisYears[0];
            int futureYear = altElem.Results.AnalysisYears[1];

            StudyPropertiesElement studyPropElem = StudyCache.GetStudyPropertiesElement();
            double DiscountRate = studyPropElem.DiscountRate;
            int PeriodOfAnalysis = altElem.Results.PeriodOfAnalysis;
            IASElement BaseYearScenario = altElem.BaseScenario.GetElement();
            List<int> impactAreaIds = BaseYearScenario.Results.GetImpactAreaIDs();
            Dictionary<int, string> impactAreaIdToName = IASElement.GetImpactAreaNamesFromIDs();

            int rowsPerScenario = impactAreaIds.Count;
            List<AlternativeDamageRowItem> rowItems = new(rowsPerScenario);

            foreach(int impactAreaID in impactAreaIds)
            {
                double mean = altElem.Results.MeanAAEQDamage(impactAreaID);
                double point75 = altElem.Results.AAEQDamageExceededWithProbabilityQ(.75,impactAreaID);
                double point5 = altElem.Results.AAEQDamageExceededWithProbabilityQ( .5, impactAreaID);
                double point25 = altElem.Results.AAEQDamageExceededWithProbabilityQ( .25, impactAreaID);
                AlternativeDamageRowItem row = new(Name, impactAreaIdToName[impactAreaID], baseYear,futureYear, DiscountRate, PeriodOfAnalysis, mean, point75, point5, point25);
                rowItems.Add(row);
            }
            return rowItems;
        }

    }
}
