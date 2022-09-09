using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using structures;
using compute;
using Statistics;
using paireddata;
using fda_model.hydraulics;

namespace stageDamage
{
    public class ScenarioStageDamage
    {
        #region Fields 
        //TODO: This needs to be the set of water surface elevation events 
        private HydraulicDataset _hydraulics;
        private Inventory _Inventory;
        private List<OccupancyType> _OccupancyType;
        private List<ImpactAreaStageDamage> _ImpactAreaStageDamage;
        #endregion

        #region Constructor 
        public ScenarioStageDamage(HydraulicDataset hydro, Inventory inventory, List<OccupancyType> occupancyTypes, List<ImpactAreaStageDamage> impactAreaStageDamages)
        {
            _hydraulics = hydro;
            _Inventory = inventory;
            _OccupancyType = occupancyTypes;
            _ImpactAreaStageDamage = impactAreaStageDamages;
        }
        #endregion

        #region Methods 
        public List<UncertainPairedData> Compute(RandomProvider randomProvider, ConvergenceCriteria convergenceCriteria)
        {
            List<UncertainPairedData> scenarioStageDamageResults = new List<UncertainPairedData>();
            foreach(ImpactAreaStageDamage impactAreaStageDamage in _ImpactAreaStageDamage)
            {
                List<UncertainPairedData> impactAreaStageDamageResults = impactAreaStageDamage.Compute(randomProvider, convergenceCriteria, _Inventory, _OccupancyType, _hydraulics);
                foreach(UncertainPairedData uncertainPairedData in impactAreaStageDamageResults)
                {
                    scenarioStageDamageResults.Add(uncertainPairedData);
                }
            }
            return scenarioStageDamageResults;
        }

        #endregion
    }
}
