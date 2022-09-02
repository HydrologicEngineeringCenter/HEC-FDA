using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using structures;
using compute;
using Statistics;
using paireddata;

namespace stageDamage
{
    public class ScenarioStageDamage
    {
        #region Fields 
        //TODO: This needs to be the set of water surface elevation events 
        private List<double> _Stages;
        private Inventory _Inventory;
        private List<OccupancyType> _OccupancyType;
        private List<ImpactAreaStageDamage> _ImpactAreaStageDamage;
        #endregion

        #region Constructor 
        public ScenarioStageDamage(List<double> stages, Inventory inventory, List<OccupancyType> occupancyTypes, List<ImpactAreaStageDamage> impactAreaStageDamages)
        {
            _Stages = stages;
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
                List<UncertainPairedData> impactAreaStageDamageResults = impactAreaStageDamage.Compute(randomProvider, convergenceCriteria, _Stages, _Inventory, _OccupancyType);
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
