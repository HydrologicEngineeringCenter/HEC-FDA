using HEC.FDA.ViewModel.AggregatedStageDamage;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class StageDamagePersistenceManager : SavingBase<AggregatedStageDamageElement>
    {

        public override string TableName { get { return "stage_damage_relationships"; } }

        public StageDamagePersistenceManager(Study.FDACache studyCache):base(studyCache)
        {
        }

    }
}
