using HEC.FDA.ViewModel.IndexPoints;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class IndexPointsPersistenceManager : SavingBase<IndexPointsElement>
    {

        public override string TableName
        {
            get { return "index_points"; }
        }

        public IndexPointsPersistenceManager(Study.FDACache studyCache):base(studyCache)
        {
        }

    }
}
