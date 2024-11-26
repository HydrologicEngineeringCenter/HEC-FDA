using HEC.FDA.ViewModel.Study;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results
{
    public class SummaryVM : SpecificAltCompReportResultsVM
    {

        public List<AAEQSummaryRowItem> AAEQSummaryRows { get; }
        public List<AggregatedAAEQSummaryRowItem> AggAAEQSummaryRows { get; }

        public List<EADSummaryRowItem> BaseEADSummaryRows { get; }
        public List<AggregatedEADSummaryRowItem> AggBaseEADSummaryRows { get; }

        public List<EADSummaryRowItem> FutureEADSummaryRows { get; }
        public List<AggregatedEADSummaryRowItem>  AggFutureEADSummaryRows { get; }

        public string BaseYearLabel { get; }
        public string FutureYearLabel { get; }
        public string AAEQLabel { get; }
        public string AggBaseYearLabel { get; }
        public string AggFutureYearLabel { get; }
        public string AggAAEQLabel { get; }

        public SummaryVM(List<EADSummaryRowItem> baseYearSummary, List<EADSummaryRowItem> futureYearSummary, List<AAEQSummaryRowItem> aaeqSummary, 
            List<AggregatedEADSummaryRowItem> aggBaseYearSummary, List<AggregatedEADSummaryRowItem> aggFutureYearSummary, List<AggregatedAAEQSummaryRowItem> aggAAEQSummary, List<int> years) :base()
        {
            AAEQSummaryRows = aaeqSummary;
            AggAAEQSummaryRows = aggAAEQSummary;
            BaseEADSummaryRows = baseYearSummary;
            AggBaseEADSummaryRows = aggBaseYearSummary;
            FutureEADSummaryRows = futureYearSummary;
            AggFutureEADSummaryRows = aggFutureYearSummary;

            BaseYearLabel = "Base Year Expected Annual Damage Reduced by Damage and Asset Category " + years[0];
            FutureYearLabel = "Future Year Expected Annual Damage Reduced by Damage and Asset Category " + years[1];
            AAEQLabel = "Equivalent Annual Damage Distribution (EqAD) Reduced by Damage and Asset Category";

            AggBaseYearLabel = "Base Year Expected Annual Damage Reduced Distribution " + years[0];
            AggFutureYearLabel = "Future Year Expected Annual Damage Reduced Distribution " + years[1];
            AggAAEQLabel = "Equivalent Annual Damage (EqAD) Reduced Distribution";
        }

    }
}
