
using System.Collections.Generic;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;

namespace metrics
{
    public class ScenarioResults : HEC.MVVMFramework.Base.Implementations.Validation, IReportMessage
    {
        #region Fields 
        List<ImpactAreaScenarioResults> _resultsList;
        #endregion

        #region Properties 
        public List<ImpactAreaScenarioResults> ResultsList
        {
            get
            {
                return _resultsList;
            }
        }
        public event MessageReportedEventHandler MessageReport;

        #endregion

        #region Constructor
        public ScenarioResults()
        {
            _resultsList = new List<ImpactAreaScenarioResults>();
        }
        #endregion

        #region Methods
        public void AddResults(ImpactAreaScenarioResults resultsToAdd)
        {
            ImpactAreaScenarioResults results = GetResults(resultsToAdd.ImpactAreaID);
            if (results.IsNull)
            {
                _resultsList.Add(resultsToAdd);
            }
        }
        public ImpactAreaScenarioResults GetResults(int impactAreaID)
        {
            foreach(ImpactAreaScenarioResults results in _resultsList)
            {
                if (results.ImpactAreaID.Equals(impactAreaID))
                {
                    return results;
                }
            }
            ImpactAreaScenarioResults dummyResults = new ImpactAreaScenarioResults();
            ReportMessage(this, new MessageEventArgs(new Message("The requested impact area Results could not be found. An arbitrary object is being returned.")));
            return dummyResults;
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        #endregion

    }
}
