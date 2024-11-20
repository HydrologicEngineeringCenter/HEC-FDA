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
        [DisplayAsColumn("Mean")]
        public double Mean { get; set; }
        [DisplayAsColumn("Q1")]
        public double Q1 { get; set; }
        [DisplayAsColumn("Q2")]
        public double Q2 { get; set; }
        [DisplayAsColumn("Q3")]
        public double Q3 { get; set; }

        private AlternativeDamageRowItem(string name, string impactArea, int baseYear, int futureYear, double discountRate, int periodOfAnalysis, double mean, double q1, double q2, double q3)
        {
            Name = name;
            ImpactArea = impactArea;
            BaseYear = baseYear;
            FutureYear = futureYear;
            DiscountRate = discountRate;
            PeriodOfAnalysis = periodOfAnalysis;
            Mean = mean;
            Q1 = q1;
            Q2 = q2;
            Q3 = q3;
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
            List<string> impactAreaNames = BaseYearScenario.SpecificIASElements.Select(x => x.ImpactAreaName).ToList();
            Dictionary<int, string> impactAreaIdToName = [];
            for (int i = 0; i < impactAreaIds.Count; i++)
            {
                impactAreaIdToName.Add(impactAreaIds[i], impactAreaNames[i]);
            }

            int rowsPerScenario = impactAreaIds.Count;
            List<AlternativeDamageRowItem> rowItems = new(rowsPerScenario);

            foreach(int impactAreaID in impactAreaIds)
            {
                double mean = altElem.Results.MeanAAEQDamage(impactAreaID);
                double q1 = altElem.Results.AAEQDamageExceededWithProbabilityQ(.75,impactAreaID);
                double q2 = altElem.Results.AAEQDamageExceededWithProbabilityQ( .5, impactAreaID);
                double q3 = altElem.Results.AAEQDamageExceededWithProbabilityQ( .25, impactAreaID);
                AlternativeDamageRowItem row = new(Name, impactAreaIdToName[impactAreaID], baseYear,futureYear, DiscountRate, PeriodOfAnalysis, mean, q1, q2, q3);
                rowItems.Add(row);
            }
            return rowItems;
        }

    }
}
