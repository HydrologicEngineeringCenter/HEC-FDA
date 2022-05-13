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
        private List<ConsequenceResults> _consequenceResultsList;
        private bool _isNull;
        #endregion

        #region Properties
        public int AlternativeID
        {
            get { return _alternativeID; }
        }
        public List<ConsequenceResults> ConsequenceResultsList
        {
            get
            {
                return _consequenceResultsList;
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
            _consequenceResultsList = new List<ConsequenceResults>();
        }
        public AlternativeResults(int id)
        {
            _alternativeID = id;
            _consequenceResultsList = new List<ConsequenceResults>();
            _isNull = false;
        }
        #endregion
        #region Methods
        public double MeanConsequence(int impactAreaID, string damageCategory, string assetCategory)
        {
            return GetConsequenceResults(impactAreaID).MeanDamage(damageCategory, assetCategory, impactAreaID);
        }
        public double ConsequencesExceededWithProbabilityQ(double exceedanceProbability, int impactAreaID, string damageCategory, string assetCategory)
        {
            return GetConsequenceResults(impactAreaID).ConsequenceExceededWithProbabilityQ(damageCategory, exceedanceProbability, assetCategory, impactAreaID);
        }
        public void AddConsequenceResults(int impactAreaID)
        {
            ConsequenceResults consequenceResults = GetConsequenceResults(impactAreaID);
            if (consequenceResults.IsNull)
            {
                ConsequenceResults newConsequenceResults = new ConsequenceResults(impactAreaID);
                _consequenceResultsList.Add(newConsequenceResults);
            }
        }
        public void AddConsequenceResults(ConsequenceResults consequenceResultsToAdd)
        {
            ConsequenceResults consequenceResults = GetConsequenceResults(consequenceResultsToAdd.RegionID);
            if (consequenceResults.IsNull)
            {
                _consequenceResultsList.Add(consequenceResultsToAdd);
            }
        }
        public ConsequenceResults GetConsequenceResults(int regionID)
        {
            foreach (ConsequenceResults consequenceResults in _consequenceResultsList)
            {
                if(consequenceResults.RegionID.Equals(regionID))
                {
                    return consequenceResults;
                }
            }
            ConsequenceResults dummyConsequenceResults = new ConsequenceResults();
            ReportMessage(this, new MessageEventArgs(new Message("The requested damage category - asset category - impact area combination could not be found. An arbitrary object is being returned.")));
            return dummyConsequenceResults;
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        #endregion

    }
}
