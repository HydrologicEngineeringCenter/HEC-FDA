using System.Collections.Generic;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.interfaces;
using HEC.FDA.Statistics.Convergence;

namespace HEC.FDA.Model.stageDamage
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
            foreach (ImpactAreaStageDamage impactAreaStageDamage in _ImpactAreaStageDamage)
            {
                List<UncertainPairedData> impactAreaStageDamageResults = impactAreaStageDamage.Compute(randomProvider);
                foreach (UncertainPairedData uncertainPairedData in impactAreaStageDamageResults)
                {
                    scenarioStageDamageResults.Add(uncertainPairedData);
                }
            }
            return scenarioStageDamageResults;
        }

        #endregion
    }
}
