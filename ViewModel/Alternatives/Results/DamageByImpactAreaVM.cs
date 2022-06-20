using HEC.FDA.ViewModel.ImpactArea;
using metrics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.Alternatives.Results
{
    public class DamageByImpactAreaVM :BaseViewModel, IAlternativeResult
    {
        public List<ImpactAreaRowItem> Rows { get; } = new List<ImpactAreaRowItem>();
        public double DiscountRate { get; set; }
        public int PeriodOfAnalysis { get; set; }
        public bool RateAndPeriodVisible { get; }
        public DamageByImpactAreaVM(ScenarioResults results)
        {
            RateAndPeriodVisible = false;
            //todo: i think richard is writing a method to return the impact areas list
            List<int> impactAreaIDs = new List<int>();

            foreach(IContainImpactAreaScenarioResults result in results.ResultsList)
            {
                impactAreaIDs.Add(result.ImpactAreaID);
            }

            foreach(int id in impactAreaIDs)
            {
                double mean = results.MeanExpectedAnnualConsequences(impactAreaID: id);
                string impactAreaName = GetImpactAreaFromID(id);
                Rows.Add(new ImpactAreaRowItem(impactAreaName, mean));
            }

        }

        public DamageByImpactAreaVM(AlternativeResults results)
        {
            RateAndPeriodVisible = false;

            List<int> impactAreaIDs = new List<int>();

            foreach (ConsequenceDistributionResult result in results.ConsequenceResults.ConsequenceResultList)
            {
                impactAreaIDs.Add(result.RegionID);
            }

            foreach (int id in impactAreaIDs)
            {
                double mean = results.MeanConsequence(impactAreaID: id);
                string impactAreaName = GetImpactAreaFromID(id);
                Rows.Add(new ImpactAreaRowItem(impactAreaName, mean));
            }

        }

        public DamageByImpactAreaVM(double discountRate,int period, AlternativeComparisonReportResults results, int altID)
        {
            DiscountRate = discountRate;
            PeriodOfAnalysis = period;
            RateAndPeriodVisible = false;

            List<int> impactAreaIDs = new List<int>();

            foreach (AlternativeResults result in results.ConsequencesReducedResultsList)
            {
                //todo: richard will write method to get the impact area ids.
                //impactAreaIDs.Add(result.);
            }

            foreach (int id in impactAreaIDs)
            {
                double mean = results.MeanConsequencesReduced(altID, impactAreaID: id);
                string impactAreaName = GetImpactAreaFromID(id);
                Rows.Add(new ImpactAreaRowItem(impactAreaName, mean));
            }

        }

       
        public DamageByImpactAreaVM(double discountRate, int period, AlternativeResults results)
        {
            DiscountRate = discountRate;
            PeriodOfAnalysis = period;
            RateAndPeriodVisible = true;

            List<int> impactAreaIDs = new List<int>();
            //todo: waiting to get richards new get impact areas method.
            foreach (ConsequenceDistributionResult result in results.ConsequenceResults.ConsequenceResultList)
            {
                impactAreaIDs.Add(result.RegionID);
            }

            foreach (int id in impactAreaIDs)
            {
                double mean = results.MeanConsequence(impactAreaID: id);
                string impactAreaName = GetImpactAreaFromID(id);
                Rows.Add(new ImpactAreaRowItem(impactAreaName, mean));
            }

        }

        private string GetImpactAreaFromID(int id)
        {
            string impactName = null;
            List<ImpactAreaElement> impactAreaElems = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            if (impactAreaElems.Count > 0)
            {
                //there only ever be one or zero
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
