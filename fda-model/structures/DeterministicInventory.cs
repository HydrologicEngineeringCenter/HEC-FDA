using metrics;
using System.Collections.Generic;

namespace structures
{
    public class DeterministicInventory
    {
        private List<int> _ImpactAreaIDs;
        private List<string> _DamageCategories;
        public IList<DeterministicStructure> Inventory;

        public DeterministicInventory(IList<DeterministicStructure> inventorySample, List<int> impactAreaIDs, List<string> damageCategories)
        {
            Inventory = inventorySample;
            _ImpactAreaIDs = impactAreaIDs;
            _DamageCategories = damageCategories;
        }
        public ConsequenceResults ComputeDamages(float[] wses)
        {
            //assume each structure has a corresponding index to the depth
            ConsequenceResults consequenceResults = new ConsequenceResults();
            for (int i = 0; i < Inventory.Count; i++)
            {
                float wse = wses[i];
                if (wse != -9999)
                {
                    consequenceResults.AddExistingConsequenceResultObject(Inventory[i].ComputeDamage(wse));
                }
            }
            return consequenceResults;
        }
    }
}