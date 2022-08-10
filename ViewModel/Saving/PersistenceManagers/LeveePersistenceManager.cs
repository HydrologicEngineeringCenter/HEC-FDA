using HEC.FDA.ViewModel.GeoTech;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class LeveePersistenceManager : SavingBase<LeveeFeatureElement>
    {
        public override string TableName
        {
            get { return "levee_features"; }
        }

        public LeveePersistenceManager(Study.FDACache studyCache):base(studyCache)
        {
        }

    }
}
