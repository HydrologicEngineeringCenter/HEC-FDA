using System.Collections.Generic;
using System.Linq;
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

        public List<DamageCategoryRowItem> Rows { get; } = [];

        public DamageByDamCatVM(ImpactAreaScenarioResults iasResult, List<string> damCats, double discountRate, int period)
        {
            EADLabel = "Mean EAD";
            DiscountRate = discountRate;
            PeriodOfAnalysis = period;
            RateAndPeriodVisible = false;

            // Get available risk types from the results
            List<RiskType> riskTypes = iasResult.ConsequenceResults.ConsequenceResultList
                .Select(r => r.RiskType)
                .Distinct()
                .ToList();

            // When only Fail exists (no Non_Fail), Fail = Total, so just show Total
            bool hasNonFail = riskTypes.Contains(RiskType.Non_Fail);
            if (hasNonFail)
            {
                foreach (RiskType riskType in riskTypes)
                {
                    foreach (string damCat in damCats)
                    {
                        double meanValue = iasResult.MeanExpectedAnnualConsequences(damageCategory: damCat, riskType: riskType);
                        Rows.Add(new DamageCategoryRowItem(damCat, meanValue, riskType.ToString()));
                    }
                }
            }
            foreach (string damCat in damCats)
            {
                double meanValue = iasResult.MeanExpectedAnnualConsequences(damageCategory: damCat);
                Rows.Add(new DamageCategoryRowItem(damCat, meanValue, RiskType.Total.ToString()));
            }
        }

        public DamageByDamCatVM(AlternativeResults alternativeResults, DamageMeasureYear damageMeasureYear)
        {
            EADLabel = "Mean EAD";
            RateAndPeriodVisible = false;

            List<string> damCats = alternativeResults.GetDamageCategories();
            foreach (string damCat in damCats)
            {
                double meanValue = damageMeasureYear == DamageMeasureYear.Base
                    ? alternativeResults.SampleMeanBaseYearEAD(damageCategory: damCat)
                    : alternativeResults.SampleMeanFutureYearEAD(damageCategory: damCat);
                Rows.Add(new DamageCategoryRowItem(damCat, meanValue, RiskType.Total.ToString()));
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
                Rows.Add(new DamageCategoryRowItem(damCat, alternativeResults.SampleMeanEqad(damageCategory: damCat), RiskType.Total.ToString()));
            }
        }

        public DamageByDamCatVM(AlternativeComparisonReportResults alternativeCompReportResults, DamageMeasureYear dmy, int altID, double discountRate = double.NaN, int period = -1)
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
                double meanValue = dmy switch
                {
                    DamageMeasureYear.Base => alternativeCompReportResults.SampleMeanBaseYearEADReduced(altID, damageCategory: damCat),
                    DamageMeasureYear.Future => alternativeCompReportResults.SampleMeanFutureYearEADReduced(altID, damageCategory: damCat),
                    DamageMeasureYear.Eqad => alternativeCompReportResults.SampleMeanEqadReduced(altID, damageCategory: damCat),
                    _ => 0
                };
                Rows.Add(new DamageCategoryRowItem(damCat, meanValue, RiskType.Total.ToString()));
            }
        }
    }
}
