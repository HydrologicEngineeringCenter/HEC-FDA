using System;
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
        public bool RiskTypeVisible { get; }

        public List<DamageCategoryRowItem> Rows { get; } = new List<DamageCategoryRowItem>();

        private static string FormatRiskType(RiskType riskType)
        {
            return riskType switch
            {
                RiskType.Fail => "Fail",
                RiskType.Non_Fail => "Non-Fail",
                RiskType.Total => "Total",
                _ => riskType.ToString()
            };
        }

        /// <summary>
        /// Gets the list of risk types to display based on what's in the results.
        /// If both Fail and Non_Fail exist, includes Total as well.
        /// </summary>
        private static List<RiskType> GetDisplayRiskTypes(List<RiskType> availableRiskTypes)
        {
            bool hasFail = availableRiskTypes.Contains(RiskType.Fail);
            bool hasNonFail = availableRiskTypes.Contains(RiskType.Non_Fail);

            if (hasFail && hasNonFail)
            {
                // Both exist, show all three: Fail, Non-Fail, Total
                return new List<RiskType> { RiskType.Fail, RiskType.Non_Fail, RiskType.Total };
            }
            else if (hasFail)
            {
                return new List<RiskType> { RiskType.Fail };
            }
            else if (hasNonFail)
            {
                return new List<RiskType> { RiskType.Non_Fail };
            }
            else
            {
                throw new Exception("Tried to display result without Risk Type");
            }
        }

        public DamageByDamCatVM(ImpactAreaScenarioResults iasResult, List<string> damCats, double discountRate, int period)
        {
            EADLabel = "Mean EAD";
            DiscountRate = discountRate;
            PeriodOfAnalysis = period;
            RateAndPeriodVisible = false;

            // Get available risk types from the results
            List<RiskType> availableRiskTypes = iasResult.ConsequenceResults.ConsequenceResultList
                .Select(r => r.RiskType)
                .Distinct()
                .ToList();
            List<RiskType> displayRiskTypes = GetDisplayRiskTypes(availableRiskTypes);
            RiskTypeVisible = displayRiskTypes.Count > 1;

            foreach (RiskType riskType in displayRiskTypes)
            {
                string riskTypeDisplay = RiskTypeVisible ? FormatRiskType(riskType) : null;
                foreach (string damCat in damCats)
                {
                    double meanValue = iasResult.MeanExpectedAnnualConsequences(damageCategory: damCat, riskType: riskType);
                    Rows.Add(new DamageCategoryRowItem(damCat, meanValue, riskTypeDisplay));
                }
            }
        }

        public DamageByDamCatVM(AlternativeResults alternativeResults, DamageMeasureYear damageMeasureYear)
        {
            EADLabel = "Mean EAD";
            RateAndPeriodVisible = false;

            List<RiskType> availableRiskTypes = alternativeResults.GetRiskTypes();
            List<RiskType> displayRiskTypes = GetDisplayRiskTypes(availableRiskTypes);
            RiskTypeVisible = displayRiskTypes.Count > 1;

            List<string> damCats = alternativeResults.GetDamageCategories();
            foreach (RiskType riskType in displayRiskTypes)
            {
                string riskTypeDisplay = RiskTypeVisible ? FormatRiskType(riskType) : null;
                foreach (string damCat in damCats)
                {
                    if (damageMeasureYear == DamageMeasureYear.Base)
                    {
                        Rows.Add(new DamageCategoryRowItem(damCat, alternativeResults.SampleMeanBaseYearEAD(damageCategory: damCat, riskType: riskType), riskTypeDisplay));
                    }
                    else
                    {
                        Rows.Add(new DamageCategoryRowItem(damCat, alternativeResults.SampleMeanFutureYearEAD(damageCategory: damCat, riskType: riskType), riskTypeDisplay));
                    }
                }
            }
        }

        public DamageByDamCatVM(AlternativeResults alternativeResults, double discountRate, int period)
        {
            EADLabel = "Mean EqAD";

            DiscountRate = discountRate;
            PeriodOfAnalysis = period;
            RateAndPeriodVisible = true;

            List<RiskType> availableRiskTypes = alternativeResults.GetRiskTypes();
            List<RiskType> displayRiskTypes = GetDisplayRiskTypes(availableRiskTypes);
            RiskTypeVisible = displayRiskTypes.Count > 1;

            List<string> damCats = alternativeResults.GetDamageCategories();
            foreach (RiskType riskType in displayRiskTypes)
            {
                string riskTypeDisplay = RiskTypeVisible ? FormatRiskType(riskType) : null;
                foreach (string damCat in damCats)
                {
                    Rows.Add(new DamageCategoryRowItem(damCat, alternativeResults.SampleMeanEqad(damageCategory: damCat, riskType: riskType), riskTypeDisplay));
                }
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

            List<RiskType> availableRiskTypes = alternativeCompReportResults.GetRiskTypes();
            List<RiskType> displayRiskTypes = GetDisplayRiskTypes(availableRiskTypes);
            RiskTypeVisible = displayRiskTypes.Count > 1;

            List<string> damCats = alternativeCompReportResults.GetDamageCategories();
            foreach (RiskType riskType in displayRiskTypes)
            {
                string riskTypeDisplay = RiskTypeVisible ? FormatRiskType(riskType) : null;
                foreach (string damCat in damCats)
                {
                    switch (dmy)
                    {
                        case DamageMeasureYear.Base:
                            Rows.Add(new DamageCategoryRowItem(damCat, alternativeCompReportResults.SampleMeanBaseYearEADReduced(altID, damageCategory: damCat, riskType: riskType), riskTypeDisplay));
                            break;
                        case DamageMeasureYear.Future:
                            Rows.Add(new DamageCategoryRowItem(damCat, alternativeCompReportResults.SampleMeanFutureYearEADReduced(altID, damageCategory: damCat, riskType: riskType), riskTypeDisplay));
                            break;
                        case DamageMeasureYear.Eqad:
                            Rows.Add(new DamageCategoryRowItem(damCat, alternativeCompReportResults.SampleMeanEqadReduced(altID, damageCategory: damCat, riskType: riskType), riskTypeDisplay));
                            break;
                    }
                }
            }
        }
    }
}
