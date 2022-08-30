using paireddata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stageDamage
{
    public class ScenarioStageDamageResults
{
        #region Fields
        private List<ImpactAreaStageDamageFunction> _scenarioStageDamageResults;
        #endregion

        #region Constructor 
        public ScenarioStageDamageResults(List<ImpactAreaStageDamageFunction> impactAreaStageDamageResults)
        {
            _scenarioStageDamageResults = impactAreaStageDamageResults;
        }
        #endregion
    }
}
