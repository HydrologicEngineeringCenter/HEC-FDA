using System.Collections.Generic;
using ViewModel.Alternatives.Results;

namespace ViewModel.AlternativeComparisonReport.Results
{
    public class AAEQSummaryVM : IAlternativeResult
    {
        public List<AAEQSummaryRowItem> Rows { get; } = new List<AAEQSummaryRowItem>();
        public double DiscountRate { get; set; }
        public int PeriodOfAnalysis { get; set; }
        public AAEQSummaryVM(double discountRate, int period) 
        {
            DiscountRate = discountRate;
            PeriodOfAnalysis = period;
            loadDummyData();
        }

        private void loadDummyData()
        {
            for (int i = 0; i < 5; i++)
            {
                Rows.Add(new AAEQSummaryRowItem());
            }
        }
    }
}
