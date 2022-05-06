
using System.Collections.Generic;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;

namespace metrics
{
    public class ScenarioResults : HEC.MVVMFramework.Base.Implementations.Validation, IReportMessage
    {
        #region Fields 
        List<Results> _resultsList;
        #endregion

        #region Properties 
        public List<Results> ResultsList
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
            _resultsList = new List<Results>();
        }
        #endregion

        #region Methods
        public void AddResults(Results resultsToAdd)
        {
            Results results = GetResults(resultsToAdd.ImpactAreaID);
            if (results.IsNull)
            {
                _resultsList.Add(resultsToAdd);
            }
        }
        public Results GetResults(int impactAreaID)
        {
            foreach(Results results in _resultsList)
            {
                if (results.ImpactAreaID.Equals(impactAreaID))
                {
                    return results;
                }
            }
            Results dummyResults = new Results();
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
