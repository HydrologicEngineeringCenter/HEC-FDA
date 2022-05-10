using System.Collections.Generic;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;

namespace metrics
{
    public class AlternativeResults : HEC.MVVMFramework.Base.Implementations.Validation, IReportMessage
    {
        #region Fields
        private int _alternativeID;
        private List<ConsequenceResults> _damageResultsList;
        private bool _isNull;
        #endregion

        #region Properties
        public int AlternativeID
        {
            get { return _alternativeID; }
        }
        public List<ConsequenceResults> DamageResultsList
        {
            get
            {
                return _damageResultsList;
            }
        }
        public event MessageReportedEventHandler MessageReport;
        public bool IsNull
        {
            get
            {
                return _isNull;
            }
        }

        #endregion

        #region Constructor
        public AlternativeResults()
        {
            _isNull = true;
            _alternativeID = 0;
            _damageResultsList = new List<ConsequenceResults>();
        }
        public AlternativeResults(int id)
        {
            _alternativeID = id;
            _damageResultsList = new List<ConsequenceResults>();
            _isNull = false;
        }
        #endregion
        #region Methods
        public void AddDamageResults(int impactAreaID)
        {
            ConsequenceResults damageResults = GetDamageResults(impactAreaID);
            if (damageResults.IsNull)
            {
                ConsequenceResults newDamageResults = new ConsequenceResults(impactAreaID);
                _damageResultsList.Add(newDamageResults);
            }
        }
        public void AddDamageResults(ConsequenceResults damageResultsToAdd)
        {
            ConsequenceResults damageResults = GetDamageResults(damageResultsToAdd.RegionID);
            if (damageResults.IsNull)
            {
                _damageResultsList.Add(damageResultsToAdd);
            }
        }
        public ConsequenceResults GetDamageResults(int impactAreaID)
        {
            foreach (ConsequenceResults damageResults in _damageResultsList)
            {
                if(damageResults.RegionID.Equals(impactAreaID))
                {
                    return damageResults;
                }
            }
            ConsequenceResults dummyDamageResults = new ConsequenceResults(impactAreaID);
            ReportMessage(this, new MessageEventArgs(new Message("The requested damage category - asset category - impact area combination could not be found. An arbitrary object is being returned.")));
            return dummyDamageResults;
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        #endregion

    }
}
