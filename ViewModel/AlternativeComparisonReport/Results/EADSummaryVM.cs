using System.Collections.Generic;
using ViewModel.Alternatives.Results;

namespace ViewModel.AlternativeComparisonReport.Results
{
    public class EADSummaryVM : IAlternativeResult
    {
        public List<EADSummaryRowItem> Rows { get; } = new List<EADSummaryRowItem>();

        public EADSummaryVM() 
        {
            loadDummyData();
        }

        private void loadDummyData()
        {
            for (int i = 0; i < 3; i++)
            {
                Rows.Add(new EADSummaryRowItem());
            }
        }
    }
}
