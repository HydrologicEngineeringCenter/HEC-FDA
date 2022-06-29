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

        public List<DamageCategoryRowItem> Rows { get; } = new List<DamageCategoryRowItem>();


        public DamageByDamCatVM(ImpactAreaScenarioResults iasResult, List<string> damCats, double discountRate, int period)
        {
            DiscountRate = discountRate;
            PeriodOfAnalysis = period;
            RateAndPeriodVisible = true;

            foreach (string damCat in damCats)
            {
                double meanValue = iasResult.MeanExpectedAnnualConsequences(damageCategory: damCat);
                Rows.Add(new DamageCategoryRowItem(damCat, meanValue));
            }
        }

        public DamageByDamCatVM(AlternativeResults alternativeResults, double discountRate = double.NaN, int period = -1)
        {
            if (double.IsNaN(discountRate))
            {
                RateAndPeriodVisible = false;
            }
            else
            {
                DiscountRate = discountRate;
                PeriodOfAnalysis = period;
                RateAndPeriodVisible = true;
            }

            List<string> damCats =  alternativeResults.GetDamageCategories();
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
            }
            else
            {
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
                        Rows.Add(new DamageCategoryRowItem(damCat, alternativeCompReportResults.MeanWithProjectBaseYearEAD(altID, damageCategory: damCat)));
                        break;
                    case DamageMeasureYear.Future:
                        Rows.Add(new DamageCategoryRowItem(damCat, alternativeCompReportResults.MeanWithProjectFutureYearEAD(altID, damageCategory: damCat)));
                        break;
                    case DamageMeasureYear.AAEQ:
                        Rows.Add(new DamageCategoryRowItem(damCat, alternativeCompReportResults.MeanWithProjectAAEQDamage(altID, damageCategory: damCat)));
                        break;
                }
            }
        }
    }
}
