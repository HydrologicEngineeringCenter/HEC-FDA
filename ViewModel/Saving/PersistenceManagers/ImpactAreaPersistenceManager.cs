using HEC.FDA.ViewModel.ImpactArea;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class ImpactAreaPersistenceManager : SavingBase<ImpactAreaElement>
    {

        public override string TableName
        {
            get { return "impact_area_set"; }
        }

        public ImpactAreaPersistenceManager(Study.FDACache studyCache):base(studyCache)
        {
        }

    }
}
