using System.Collections.Generic;
using HEC.FDA.ViewModel.Alternatives.Results;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results
{
    public class AAEQSummaryVM : IAlternativeResult
    {
        public List<AAEQSummaryRowItem> Rows { get; } = new List<AAEQSummaryRowItem>();
        public double DiscountRate { get; set; }
        public int PeriodOfAnalysis { get; set; }
        public AAEQSummaryVM(List<AAEQSummaryRowItem> rows, double discountRate, int period) 
        {
            DiscountRate = discountRate;
            PeriodOfAnalysis = period;
            Rows = rows;
        }

        //private void loadDummyData()
        //{
        //    for (int i = 0; i < 5; i++)
        //    {
        //        Rows.Add(new AAEQSummaryRowItem());
        //    }
        //}
    }
}
