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
        /// <summary>
        /// This method returns the mean of the average annual equivalent damage for a given impact area, damage category, asset category combination 
        /// </summary>
        /// <param name="impactAreaID"></param>
        /// <param name="damageCategory"></param> either residential, commercial, etc...
        /// <param name="assetCategory"></param> either structure, content, etc...
        /// <returns></returns>
        public double MeanConsequence(int impactAreaID, string damageCategory, string assetCategory)
        {
            return GetConsequenceResults(impactAreaID).MeanDamage(damageCategory, assetCategory, impactAreaID);
        }
        /// <summary>
        /// This method calls the inverse CDF of average annual equivalent damage histogram up to the non-exceedance probabilty. The method accepts exceedance probability as an argument. 
        /// </summary>
        /// <param name="exceedanceProbability"></param>
        /// <param name="impactAreaID"></param>
        /// <param name="damageCategory"></param> either residential, commerical, etc...
        /// <param name="assetCategory"></param> either structure, content, etc...
        /// <returns></returns>
        public double ConsequencesExceededWithProbabilityQ(double exceedanceProbability, int impactAreaID, string damageCategory, string assetCategory)
        {
            return GetConsequenceResults(impactAreaID).ConsequenceExceededWithProbabilityQ(damageCategory, exceedanceProbability, assetCategory, impactAreaID);
        }
        internal void AddConsequenceResults(int impactAreaID)
        {
            ConsequenceResults consequenceResults = GetConsequenceResults(impactAreaID);
            if (consequenceResults.IsNull)
            {
                ConsequenceResults newConsequenceResults = new ConsequenceResults(impactAreaID);
                _consequenceResultsList.Add(newConsequenceResults);
            }
        }
        internal void AddConsequenceResults(ConsequenceResults consequenceResultsToAdd)
        {
            ConsequenceResults consequenceResults = GetConsequenceResults(consequenceResultsToAdd.RegionID);
            if (consequenceResults.IsNull)
            {
                _consequenceResultsList.Add(consequenceResultsToAdd);
            }
        }
        public Statistics.Histograms.ThreadsafeInlineHistogram GetConsequencesHistogram(string damageCategory, string assetCategory, int impactAreaID)
        {
            ConsequenceResults consequenceResults = GetConsequenceResults(impactAreaID);
            return consequenceResults.GetConsequenceResultsHistogram(damageCategory, assetCategory, impactAreaID);
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
            string message = $"The requested damage cetegory - asset category - impact area combination could not be found. an arbitrary object is being returned";
            HEC.MVVMFramework.Model.Messaging.ErrorMessage errorMessage = new HEC.MVVMFramework.Model.Messaging.ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
            ReportMessage(this, new MessageEventArgs(errorMessage));
            return dummyConsequenceResults;
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        #endregion

    }
}
