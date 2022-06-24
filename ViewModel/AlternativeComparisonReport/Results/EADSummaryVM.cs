using System.Collections.Generic;
using HEC.FDA.ViewModel.Alternatives.Results;
using HEC.FDA.ViewModel.Alternatives.Results.ResultObject;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results
{
    public class EADSummaryVM : IAlternativeResult
    {
        public List<EADSummaryRowItem> Rows { get; } = new List<EADSummaryRowItem>();
        public double DiscountRate { get; set; }
        public int PeriodOfAnalysis { get; set; }
        public EADSummaryVM(List<EADSummaryRowItem> rows , AlternativeResult altResult, double discountRate, int period) 
        {
            DiscountRate = discountRate;
            PeriodOfAnalysis = period;
            Rows = rows;
        }

    }
}
