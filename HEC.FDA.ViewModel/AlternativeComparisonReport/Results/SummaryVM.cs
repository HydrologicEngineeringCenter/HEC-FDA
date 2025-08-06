using HEC.FDA.ViewModel.Study;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results
{
    public class SummaryVM : SpecificAltCompReportResultsVM
    {

        public List<EqadSummaryRowItem> EqadSummaryRows { get; }
        public List<AggregatedEqadSummaryRowItem> AggEqadSummaryRows { get; }

        public List<EADSummaryRowItem> BaseEADSummaryRows { get; }
        public List<AggregatedEADSummaryRowItem> AggBaseEADSummaryRows { get; }

        public List<EADSummaryRowItem> FutureEADSummaryRows { get; }
        public List<AggregatedEADSummaryRowItem>  AggFutureEADSummaryRows { get; }

        public string BaseYearLabel { get; }
        public string FutureYearLabel { get; }
        public string EqadLabel { get; }
        public string AggBaseYearLabel { get; }
        public string AggFutureYearLabel { get; }
        public string AggEqadLabel { get; }

        public SummaryVM(List<EADSummaryRowItem> baseYearSummary, List<EADSummaryRowItem> futureYearSummary, List<EqadSummaryRowItem> eqadSummary, 
            List<AggregatedEADSummaryRowItem> aggBaseYearSummary, List<AggregatedEADSummaryRowItem> aggFutureYearSummary, List<AggregatedEqadSummaryRowItem> aggEqadSummary, List<int> years) :base()
        {
            EqadSummaryRows = eqadSummary;
            AggEqadSummaryRows = aggEqadSummary;
            BaseEADSummaryRows = baseYearSummary;
            AggBaseEADSummaryRows = aggBaseYearSummary;
            FutureEADSummaryRows = futureYearSummary;
            AggFutureEADSummaryRows = aggFutureYearSummary;

            BaseYearLabel = "Base Year Expected Annual Damage Reduced by Damage and Asset Category " + years[0];
            FutureYearLabel = "Future Year Expected Annual Damage Reduced by Damage and Asset Category " + years[1];
            EqadLabel = "Equivalent Annual Damage Distribution (EqAD) Reduced by Damage and Asset Category";

            AggBaseYearLabel = "Base Year Expected Annual Damage Reduced Distribution " + years[0];
            AggFutureYearLabel = "Future Year Expected Annual Damage Reduced Distribution " + years[1];
            AggEqadLabel = "Equivalent Annual Damage (EqAD) Reduced Distribution";
        }

    }
}
