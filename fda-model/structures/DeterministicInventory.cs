using System.Collections.Generic;

namespace structures
{
    public class DeterministicInventory
    {
        public IList<DeterministicStructure> Inventory;

        public DeterministicInventory(IList<DeterministicStructure> inventorySample)
        {
            Inventory = inventorySample;
        }
        public List<StructureDamageResult> ComputeDamages(float[] depths)
        {
            //assume each structure has a corresponding index to the depth
            List<StructureDamageResult> results = new List<StructureDamageResult>();
            StructureDamageResult nodamage = new StructureDamageResult(0, 0, 0, 0);
            for (int i = 0; i < Inventory.Count; i++)
            {
                float depth = depths[i];
                if (depth != -9999)
                {
                    results.Add(Inventory[i].ComputeDamage(depth));
                }
                else
                {
                    results.Add(nodamage);
                }

            }
            return results;
        }
    }
}