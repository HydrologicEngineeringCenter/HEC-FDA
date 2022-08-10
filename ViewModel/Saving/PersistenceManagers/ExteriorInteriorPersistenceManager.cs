using HEC.FDA.ViewModel.StageTransforms;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class ExteriorInteriorPersistenceManager:SavingBase<ExteriorInteriorElement>
    {
        public override string TableName { get { return "exterior_interior_curves"; } }

        public ExteriorInteriorPersistenceManager(Study.FDACache studyCache):base(studyCache)
        {
        }

    }
}
