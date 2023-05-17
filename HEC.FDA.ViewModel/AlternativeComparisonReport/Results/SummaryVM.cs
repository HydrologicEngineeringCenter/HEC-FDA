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
        public string AAEQLabel { get; }

        public SummaryVM(List<EADSummaryRowItem> baseYearSummary, List<EADSummaryRowItem> futureYearSummary, List<AAEQSummaryRowItem> aaeqSummary, List<int> years) :base()
        {
            StudyPropertiesElement studyPropElem = StudyCache.GetStudyPropertiesElement();
            AAEQSummaryVM = new AAEQSummaryVM(aaeqSummary, studyPropElem.DiscountRate, studyPropElem.PeriodOfAnalysis);
            BaseEADSummaryVM = new EADSummaryVM(baseYearSummary, studyPropElem.DiscountRate, studyPropElem.PeriodOfAnalysis);
            FutureEADSummaryVM = new EADSummaryVM(futureYearSummary, studyPropElem.DiscountRate, studyPropElem.PeriodOfAnalysis);

            BaseYearLabel = "Base Year EAD" + years[0] + ":";
            FutureYearLabel = "Future Year EAD " + years[1] + ":";
            AAEQLabel = "Period of Analysis AAEQ Damage:";

        }

    }
}
