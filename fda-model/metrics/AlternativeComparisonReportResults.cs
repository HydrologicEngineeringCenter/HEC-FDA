using System.Collections.Generic;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;

namespace metrics
{
    public class AlternativeComparisonReportResults : HEC.MVVMFramework.Base.Implementations.Validation, IReportMessage
    {
        #region Fields
        private List<AlternativeResults> _resultsList;
        #endregion

        #region Properties 
        public List<AlternativeResults> ConsequencesReducedResultsList
        {
            get
            {
                return _resultsList;
            }
        }
        public event MessageReportedEventHandler MessageReport;

        #endregion

        #region Constructor
        public AlternativeComparisonReportResults()
        {
            _resultsList = new List<AlternativeResults>();
        }
        #endregion

        #region Methods 
        /// <summary>
        /// This method gets the mean consequences reduced between the with- and without-project conditions for a given with-project condition, 
        /// impact area, damage category, and asset category combination. 
        /// </summary>
        /// <param name="alternativeID"></param>
        /// <param name="impactAreaID"></param>
        /// <param name="damageCategory"></param> either residential, commercial, etc...
        /// <param name="assetCategory"></param> either structure, content, etc...
        /// <returns></returns>
        public double MeanConsequencesReduced(int alternativeID, int impactAreaID, string damageCategory, string assetCategory)
        {
            return GetAlternativeResults(alternativeID).MeanConsequence(impactAreaID, damageCategory, assetCategory);
        }
        /// <summary>
        /// This method calls the inverse CDF of damage reduced histogram up to the non-exceedance probabilty. The method accepts exceedance probability as an argument. 
        /// </summary>
        /// <param name="exceedanceProbability"></param>
        /// <param name="alternativeID"></param>
        /// <param name="impactAreaID"></param>
        /// <param name="damageCategory"></param> either residential, commercial, etc...
        /// <param name="assetCategory"></param> either structure, content, etc...
        /// <returns></returns>
        public double ConsequencesReducedExceededWithProbabilityQ(double exceedanceProbability, int alternativeID, int impactAreaID, string damageCategory, string assetCategory)
        {
            return GetAlternativeResults(alternativeID).ConsequencesExceededWithProbabilityQ(exceedanceProbability, impactAreaID, damageCategory, assetCategory);
        }
        public void AddAlternativeResults(AlternativeResults alternativeResultsToAdd)
        {
            AlternativeResults alternativeResults = GetAlternativeResults(alternativeResultsToAdd.AlternativeID);
            if (alternativeResults.IsNull)
            {
                _resultsList.Add(alternativeResultsToAdd);
            }
        }
        public Statistics.Histograms.ThreadsafeInlineHistogram GetAlternativeResultsHistogram(int alternativeID, int impactAreaID, string damageCategory, string assetCategory)
        {
            AlternativeResults alternativeResults = GetAlternativeResults(alternativeID);
            return alternativeResults.GetConsequencesHistogram(damageCategory, assetCategory, impactAreaID);
        }
        public AlternativeResults GetAlternativeResults(int alternativeID)
        {
            foreach (AlternativeResults alternativeResults in _resultsList)
            {
                if (alternativeResults.AlternativeID.Equals(alternativeID))
                {
                    return alternativeResults;
                }
            }
            AlternativeResults dummyAlternativeResult = new AlternativeResults();
            ReportMessage(this, new MessageEventArgs(new Message("The requested alternative could not be found. An arbitrary object is being returned.")));
            return dummyAlternativeResult;

        }

        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        #endregion
    }
}
