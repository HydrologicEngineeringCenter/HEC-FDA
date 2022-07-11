using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Utilities;
using metrics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.Alternatives.Results
{
    public class DamageByImpactAreaVM :BaseViewModel, IAlternativeResult
    {
        private string _EADLabel;
        public List<ImpactAreaRowItem> Rows { get; } = new List<ImpactAreaRowItem>();
        public double DiscountRate { get; set; }
        public int PeriodOfAnalysis { get; set; }
        public bool RateAndPeriodVisible { get; }
        public string EADLabel
        {
            get { return _EADLabel; }
            set { _EADLabel = value; NotifyPropertyChanged(); }
        }

        public DamageByImpactAreaVM(AlternativeResults results, DamageMeasureYear damageMeasureYear, double discountRate = double.NaN, int period = -1)
        {
            if (double.IsNaN(discountRate))
            {
                RateAndPeriodVisible = false;
                EADLabel = "Expected Annual Damage";
            }
            else
            { 
                DiscountRate = discountRate;
                PeriodOfAnalysis = period;
                RateAndPeriodVisible = true;
                EADLabel = "AAEQ Damage";
            }

            List<int> impactAreaIDs = results.GetImpactAreaIDs();

            foreach (int id in impactAreaIDs)
            {
                string impactAreaName = GetImpactAreaFromID(id);

                switch (damageMeasureYear)
                {
                    case DamageMeasureYear.Base:
                        double baseMean = results.MeanBaseYearEAD(impactAreaID: id);
                        Rows.Add(new ImpactAreaRowItem(impactAreaName, baseMean));
                        break;
                    case DamageMeasureYear.Future:
                        double futureMean = results.MeanFutureYearEAD(impactAreaID: id);
                        Rows.Add(new ImpactAreaRowItem(impactAreaName, futureMean)); 
                        break;
                    case DamageMeasureYear.AAEQ:
                        double mean = results.MeanAAEQDamage(impactAreaID: id);
                        Rows.Add(new ImpactAreaRowItem(impactAreaName, mean)); 
                        break;
                }
            }
        }

        public DamageByImpactAreaVM( AlternativeComparisonReportResults results, int altID, DamageMeasureYear damageMeasureYear, double discountRate = double.NaN, int period = -1)
        {
            if (double.IsNaN(discountRate))
            {
                EADLabel = "EAD Reduced";
                RateAndPeriodVisible = false;
            }
            else
            {
                EADLabel = "AAEQ Damage Reduced";
                DiscountRate = discountRate;
                PeriodOfAnalysis = period;
                RateAndPeriodVisible = true;
            }

            List<int> impactAreaIDs = results.GetImpactAreaIDs();

            foreach (int id in impactAreaIDs)
            {
                string impactAreaName = GetImpactAreaFromID(id);

                switch (damageMeasureYear)
                {
                    case DamageMeasureYear.Base:
                        double baseMean = results.MeanBaseYearEADReduced(altID, impactAreaID: id);
                        Rows.Add(new ImpactAreaRowItem(impactAreaName, baseMean));
                        break;
                    case DamageMeasureYear.Future:
                        double futureMean = results.MeanFutureYearEADReduced(altID, impactAreaID: id);
                        Rows.Add(new ImpactAreaRowItem(impactAreaName, futureMean));
                        break;
                    case DamageMeasureYear.AAEQ:
                        double mean = results.MeanAAEQDamageReduced(altID, impactAreaID: id);
                        Rows.Add(new ImpactAreaRowItem(impactAreaName, mean));
                        break;
                }

            }
        }
  

        private string GetImpactAreaFromID(int id)
        {
            string impactName = null;
            List<ImpactAreaElement> impactAreaElems = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            if (impactAreaElems.Count > 0)
            {
                //there will only ever be one or zero
                ObservableCollection<ImpactArea.ImpactAreaRowItem> impactAreaRows = impactAreaElems[0].ImpactAreaRows;
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
