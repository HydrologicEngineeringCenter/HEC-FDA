using System.Collections.Generic;
using HEC.FDA.ViewModel.Alternatives.Results;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results
{
    public class EqadSummaryVM : IAlternativeResult
    {
        public List<EqadSummaryRowItem> Rows { get; } = new List<EqadSummaryRowItem>();
        public double DiscountRate { get; set; }
        public int PeriodOfAnalysis { get; set; }
        public EqadSummaryVM(List<EqadSummaryRowItem> rows, double discountRate, int period) 
        {
            DiscountRate = discountRate;
            PeriodOfAnalysis = period;
            Rows = rows;
        }

    }
}
