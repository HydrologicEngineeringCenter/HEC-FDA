using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Utilities;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Alternatives.Results
{
    public class DamageByImpactAreaVM : BaseViewModel, IAlternativeResult
    {
        public List<IConsequenceByImpactAreaRowItem> Rows { get; } = [];
        public double DiscountRate { get; set; }
        public int PeriodOfAnalysis { get; set; }
        public bool RateAndPeriodVisible { get; }

        public DamageByImpactAreaVM(AlternativeResults results, DamageMeasureYear damageMeasureYear, double discountRate = double.NaN, int period = -1, ConsequenceType consequenceType = ConsequenceType.Damage)
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

            List<int> impactAreaIDs = results.GetImpactAreaIDs();
            foreach (int id in impactAreaIDs)
            {
                string impactAreaName = GetImpactAreaFromID(id);
                AddRowForImpactArea(results, damageMeasureYear, consequenceType, id, impactAreaName, RiskType.Total);
            }
        }

        private void AddRowForImpactArea(AlternativeResults results, DamageMeasureYear damageMeasureYear, ConsequenceType consequenceType, int id, string impactAreaName, RiskType riskType)
        {
            string riskTypeDisplay = riskType.ToString();
            switch (damageMeasureYear)
            {
                case DamageMeasureYear.Base:
                    double baseMean = results.SampleMeanBaseYearEAD(impactAreaID: id, consequenceType: consequenceType, riskType: riskType);
                    if (consequenceType == ConsequenceType.Damage)
                        Rows.Add(new EADByImpactAreaRowItem(impactAreaName, baseMean, riskTypeDisplay));
                    else if (consequenceType == ConsequenceType.LifeLoss)
                        Rows.Add(new LifeLossByImpactAreaRowItem(impactAreaName, baseMean, riskTypeDisplay));
                    break;
                case DamageMeasureYear.Future:
                    double futureMean = results.SampleMeanFutureYearEAD(impactAreaID: id, consequenceType: consequenceType, riskType: riskType);
                    if (consequenceType == ConsequenceType.Damage)
                        Rows.Add(new EADByImpactAreaRowItem(impactAreaName, futureMean, riskTypeDisplay));
                    else if (consequenceType == ConsequenceType.LifeLoss)
                        Rows.Add(new LifeLossByImpactAreaRowItem(impactAreaName, futureMean, riskTypeDisplay));
                    break;
                case DamageMeasureYear.Eqad:
                    double mean = results.SampleMeanEqad(impactAreaID: id, consequenceType: consequenceType, riskType: riskType);
                    Rows.Add(new EqADByImpactAreaRowItem(impactAreaName, mean, riskTypeDisplay));
                    break;
            }
        }

        public DamageByImpactAreaVM(AlternativeComparisonReportResults results, int altID, DamageMeasureYear damageMeasureYear, double discountRate = double.NaN, int period = -1, ConsequenceType consequenceType = ConsequenceType.Damage)
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

            List<int> impactAreaIDs = results.GetImpactAreaIDs();
            foreach (int id in impactAreaIDs)
            {
                string impactAreaName = GetImpactAreaFromID(id);
                AddRowForImpactAreaReduced(results, altID, damageMeasureYear, consequenceType, id, impactAreaName, RiskType.Total);
            }
        }

        private void AddRowForImpactAreaReduced(AlternativeComparisonReportResults results, int altID, DamageMeasureYear damageMeasureYear, ConsequenceType consequenceType, int id, string impactAreaName, RiskType riskType)
        {
            string riskTypeDisplay = riskType.ToString();
            switch (damageMeasureYear)
            {
                case DamageMeasureYear.Base:
                    double baseMean = results.SampleMeanBaseYearEADReduced(altID, impactAreaID: id, consequenceType: consequenceType, riskType: riskType);
                    if (consequenceType == ConsequenceType.Damage)
                        Rows.Add(new EADByImpactAreaRowItem(impactAreaName, baseMean, riskTypeDisplay));
                    else if (consequenceType == ConsequenceType.LifeLoss)
                        Rows.Add(new LifeLossByImpactAreaRowItem(impactAreaName, baseMean, riskTypeDisplay));
                    break;
                case DamageMeasureYear.Future:
                    double futureMean = results.SampleMeanFutureYearEADReduced(altID, impactAreaID: id, consequenceType: consequenceType, riskType: riskType);
                    if (consequenceType == ConsequenceType.Damage)
                        Rows.Add(new EADByImpactAreaRowItem(impactAreaName, futureMean, riskTypeDisplay));
                    else if (consequenceType == ConsequenceType.LifeLoss)
                        Rows.Add(new LifeLossByImpactAreaRowItem(impactAreaName, futureMean, riskTypeDisplay));
                    break;
                case DamageMeasureYear.Eqad:
                    double mean = results.SampleMeanEqadReduced(altID, impactAreaID: id, consequenceType: consequenceType, riskType: riskType);
                    Rows.Add(new EqADByImpactAreaRowItem(impactAreaName, mean, riskTypeDisplay));
                    break;
            }
        }


        private static string GetImpactAreaFromID(int id)
        {
            string impactName = null;
            List<ImpactAreaElement> impactAreaElems = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            if (impactAreaElems.Count > 0)
            {
                //there will only ever be one or zero
                List<ImpactArea.ImpactAreaRowItem> impactAreaRows = impactAreaElems[0].ImpactAreaRows;
                foreach (ImpactArea.ImpactAreaRowItem row in impactAreaRows)
                {
                    if (row.ID == id)
                    {
                        impactName = row.Name;
                        break;
                    }
                }
            }
            return impactName;
        }

    }
}
