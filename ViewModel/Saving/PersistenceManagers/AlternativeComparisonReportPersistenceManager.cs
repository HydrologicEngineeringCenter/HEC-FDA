using HEC.FDA.ViewModel.AlternativeComparisonReport;
using HEC.FDA.ViewModel.Study;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class AlternativeComparisonReportPersistenceManager : SavingBase<AlternativeComparisonReportElement>
    {
        public override string TableName => "alternative_comparison_reports";

        public AlternativeComparisonReportPersistenceManager(FDACache studyCache):base(studyCache)
        {
        }       

    }
}
