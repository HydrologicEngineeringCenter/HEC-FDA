using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using HEC.FDA.ViewModel.Utilities;
using metrics;

namespace HEC.FDA.ViewModel.Alternatives.Results
{
    public class DamageByDamCatVM : BaseViewModel, IAlternativeResult
    {
        public double DiscountRate { get; set; }
        public int PeriodOfAnalysis { get; set; }
        public bool RateAndPeriodVisible { get; }
        public string EADLabel { get; }

        public List<DamageCategoryRowItem> Rows { get; } = new List<DamageCategoryRowItem>();


        public DamageByDamCatVM(ImpactAreaScenarioResults iasResult, List<string> damCats, double discountRate, int period)
        {
            EADLabel = "Expected Annual Damage";
            DiscountRate = discountRate;
            PeriodOfAnalysis = period;
            RateAndPeriodVisible = false;

            foreach (string damCat in damCats)
            {
                double meanValue = iasResult.MeanExpectedAnnualConsequences(damageCategory: damCat);
                Rows.Add(new DamageCategoryRowItem(damCat, meanValue));
            }
        }

        public DamageByDamCatVM(AlternativeResults alternativeResults, DamageMeasureYear damageMeasureYear)
        {
            EADLabel = "Expected Annual Damage";
            RateAndPeriodVisible = false;
            List<string> damCats = alternativeResults.GetDamageCategories();
            foreach (string damCat in damCats)
            {
                if (damageMeasureYear == DamageMeasureYear.Base)
                {
                    Rows.Add(new DamageCategoryRowItem(damCat, alternativeResults.MeanBaseYearEAD(damageCategory: damCat)));
                }
                else
                {
                    Rows.Add(new DamageCategoryRowItem(damCat, alternativeResults.MeanFutureYearEAD(damageCategory: damCat)));
                }
            }
        }

        public DamageByDamCatVM(AlternativeResults alternativeResults, double discountRate, int period)
        {
            EADLabel = "AAEQ Damage";

            DiscountRate = discountRate;
            PeriodOfAnalysis = period;
            RateAndPeriodVisible = true;

            List<string> damCats = alternativeResults.GetDamageCategories();
            foreach (string damCat in damCats)
            {
                Rows.Add(new DamageCategoryRowItem(damCat, alternativeResults.MeanAAEQDamage(damageCategory: damCat)));
            }
        }

        public DamageByDamCatVM(AlternativeComparisonReportResults alternativeCompReportResults, DamageMeasureYear dmy,  int altID, double discountRate = double.NaN, int period = -1)
        {
            if (double.IsNaN(discountRate))
            {
                RateAndPeriodVisible = false;
                EADLabel = "EAD Reduced";
            }
            else
            {
                EADLabel = "AAEQ Damage Reduced";
                DiscountRate = discountRate;
                PeriodOfAnalysis = period;
                RateAndPeriodVisible = true;
            }

            List<string> damCats = alternativeCompReportResults.GetDamageCategories();
            foreach (string damCat in damCats)
            {
                switch(dmy)
                {
                    
                    case DamageMeasureYear.Base:
                        Rows.Add(new DamageCategoryRowItem(damCat, alternativeCompReportResults.MeanBaseYearEADReduced(altID, damageCategory: damCat)));
                        break;
                    case DamageMeasureYear.Future:
                        Rows.Add(new DamageCategoryRowItem(damCat, alternativeCompReportResults.MeanFutureYearEADReduced(altID, damageCategory: damCat)));
                        break;
                    case DamageMeasureYear.AAEQ:
                        Rows.Add(new DamageCategoryRowItem(damCat, alternativeCompReportResults.MeanAAEQDamageReduced(altID, damageCategory: damCat)));
                        break;
                }
            }
        }
    }
}
