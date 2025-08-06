using System.Collections.Generic;
using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using HEC.FDA.ViewModel.Utilities;

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
            EADLabel = "Mean EAD";
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
            EADLabel = "Mean EAD";
            RateAndPeriodVisible = false;
            List<string> damCats = alternativeResults.GetDamageCategories();
            foreach (string damCat in damCats)
            {
                if (damageMeasureYear == DamageMeasureYear.Base)
                {
                    Rows.Add(new DamageCategoryRowItem(damCat, alternativeResults.SampleMeanBaseYearEAD(damageCategory: damCat)));
                }
                else
                {
                    Rows.Add(new DamageCategoryRowItem(damCat, alternativeResults.SampleMeanFutureYearEAD(damageCategory: damCat)));
                }
            }
        }

        public DamageByDamCatVM(AlternativeResults alternativeResults, double discountRate, int period)
        {
            EADLabel = "Mean EqAD";

            DiscountRate = discountRate;
            PeriodOfAnalysis = period;
            RateAndPeriodVisible = true;

            List<string> damCats = alternativeResults.GetDamageCategories();
            foreach (string damCat in damCats)
            {
                Rows.Add(new DamageCategoryRowItem(damCat, alternativeResults.SampleMeanEqad(damageCategory: damCat)));
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
                EADLabel = "EqAD Reduced";
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
                        Rows.Add(new DamageCategoryRowItem(damCat, alternativeCompReportResults.SampleMeanBaseYearEADReduced(altID, damageCategory: damCat)));
                        break;
                    case DamageMeasureYear.Future:
                        Rows.Add(new DamageCategoryRowItem(damCat, alternativeCompReportResults.SampleMeanFutureYearEADReduced(altID, damageCategory: damCat)));
                        break;
                    case DamageMeasureYear.Eqad:
                        Rows.Add(new DamageCategoryRowItem(damCat, alternativeCompReportResults.SampleMeanEqadReduced(altID, damageCategory: damCat)));
                        break;
                }
            }
        }
    }
}
