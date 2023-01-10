using HEC.FDA.Model.metrics;
using System.Collections.Generic;

namespace HEC.FDA.Model.structures
{
    public class DeterministicInventory
    {
        #region Fields 
        private double _priceIndex;
        #endregion
        #region Properties
        public IList<DeterministicStructure> Inventory;
        #endregion
        #region Constructors
        public DeterministicInventory(IList<DeterministicStructure> inventorySample, double priceIndex)
        {
            Inventory = inventorySample;
            _priceIndex = priceIndex;
        }
        #endregion
        #region Methods
        public ConsequenceResult ComputeDamages(float[] wses, int analysisYear, string damageCategory)
        {
            ConsequenceResult aggregateConsequenceResult = new ConsequenceResult(damageCategory);
            //assume each structure has a corresponding index to the depth
            for (int i = 0; i < Inventory.Count; i++)
            {
                float wse = wses[i];
                if (wse != -9999)
                {
                    ConsequenceResult consequenceResult = Inventory[i].ComputeDamage(wse, _priceIndex, analysisYear);
                    aggregateConsequenceResult.IncrementConsequence(consequenceResult.StructureDamage, consequenceResult.ContentDamage, consequenceResult.VehicleDamage, consequenceResult.OtherDamage);
                }
            }
            return aggregateConsequenceResult;
        }
        #endregion
    }
}