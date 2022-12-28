using HEC.FDA.Model.metrics;
using System.Collections.Generic;

namespace HEC.FDA.Model.structures
{
    public class DeterministicInventory
    {
        #region Fields 
        private List<int> _ImpactAreaIDs;
        private List<string> _DamageCategories;
        private double _priceIndex;
        #endregion
        #region Properties
        public IList<DeterministicStructure> Inventory;
        #endregion
        #region Constructors
        public DeterministicInventory(IList<DeterministicStructure> inventorySample, List<int> impactAreaIDs, List<string> damageCategories, double priceIndex)
        {
            Inventory = inventorySample;
            _ImpactAreaIDs = impactAreaIDs;
            _DamageCategories = damageCategories;
            _priceIndex = priceIndex;
        }
        #endregion
        #region Methods
        public ConsequenceResults ComputeDamages(float[] wses, int analysisYear)
        {
            //assume each structure has a corresponding index to the depth
            ConsequenceResults consequenceResults = new ConsequenceResults();
            for (int i = 0; i < Inventory.Count; i++)
            {
                float wse = wses[i];
                if (wse != -9999)
                {
                    ConsequenceResult consequenceResult = Inventory[i].ComputeDamage(wse, _priceIndex, analysisYear);
                    consequenceResults.AddExistingConsequenceResultObject(consequenceResult);
                }
            }
            return consequenceResults;
        }
        #endregion
    }
}