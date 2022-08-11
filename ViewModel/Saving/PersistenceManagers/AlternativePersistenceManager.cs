using HEC.FDA.ViewModel.Alternatives;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class AlternativePersistenceManager : SavingBase<AlternativeElement>
    {
        public override string TableName => "alternatives";

        public AlternativePersistenceManager(Study.FDACache studyCache):base(studyCache)
        {
        }
 
    }
}
