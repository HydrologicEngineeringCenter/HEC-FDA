using HEC.FDA.ViewModel.Study;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results
{
    public class SummaryVM : SpecificAltCompReportResultsVM
    {

        public AAEQSummaryVM AAEQSummaryVM { get; }

        public EADSummaryVM BaseEADSummaryVM { get; }

        public EADSummaryVM FutureEADSummaryVM { get; }

        public string BaseYearLabel { get; }
        public string FutureYearLabel { get; }

        public SummaryVM(List<EADSummaryRowItem> baseYearSummary, List<EADSummaryRowItem> futureYearSummary, List<AAEQSummaryRowItem> aaeqSummary, List<int> years) :base()
        {
            StudyPropertiesElement studyPropElem = StudyCache.GetStudyPropertiesElement();
            AAEQSummaryVM = new AAEQSummaryVM(aaeqSummary, studyPropElem.DiscountRate, studyPropElem.PeriodOfAnalysis);
            BaseEADSummaryVM = new EADSummaryVM(baseYearSummary, studyPropElem.DiscountRate, studyPropElem.PeriodOfAnalysis);
            FutureEADSummaryVM = new EADSummaryVM(futureYearSummary, studyPropElem.DiscountRate, studyPropElem.PeriodOfAnalysis);

            BaseYearLabel = "Base Year: " + years[0] + ":";
            FutureYearLabel = "Future Year: " + years[1] + ":";

        }

    }
}
