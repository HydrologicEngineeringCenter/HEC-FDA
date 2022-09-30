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
using interfaces;

namespace stageDamage
{
    public class ScenarioStageDamage
    {
        #region Fields 
        private List<ImpactAreaStageDamage> _ImpactAreaStageDamage;
        #endregion

        #region Constructor 
        public ScenarioStageDamage(List<ImpactAreaStageDamage> impactAreaStageDamages)
        {
            _ImpactAreaStageDamage = impactAreaStageDamages;
        }
        #endregion

        #region Methods 
        public List<UncertainPairedData> Compute(IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria)
        {
            List<UncertainPairedData> scenarioStageDamageResults = new List<UncertainPairedData>();
            foreach(ImpactAreaStageDamage impactAreaStageDamage in _ImpactAreaStageDamage)
            {
                List<UncertainPairedData> impactAreaStageDamageResults = impactAreaStageDamage.Compute(randomProvider);
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
