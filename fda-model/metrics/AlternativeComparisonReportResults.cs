using System.Collections.Generic;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;

namespace metrics
{
    public class AlternativeComparisonReportResults : HEC.MVVMFramework.Base.Implementations.Validation, IReportMessage
    {   //TODO: save a year 
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
        ///  The level of aggregation of  consequences is determined by the arguments used in the method
        /// For example, if you wanted the mean EAD for alternative 1, residential, impact area 2, all asset categories, then the method call would be as follows:
        /// double consequenceValue = MeanConsequencesReduced(1, damageCategory: "residential", impactAreaID: 2);        /// </summary>
        /// <param name="alternativeID"></param>
        /// <param name="impactAreaID"></param>
        /// <param name="damageCategory"></param> either residential, commercial, etc...
        /// <param name="assetCategory"></param> either structure, content, etc...
        /// <returns></returns>
        public double MeanConsequencesReduced(int alternativeID, int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            return GetAlternativeResults(alternativeID).MeanConsequence(impactAreaID, damageCategory, assetCategory);
        }
        /// <summary>
        /// This method calls the inverse CDF of damage reduced histogram up to the non-exceedance probabilty. The method accepts exceedance probability as an argument. 
        ///  The level of aggregation of  consequences is determined by the arguments used in the method
        /// For example, if you wanted the EAD exceeded with probability .98 for alternative 1, residential, impact area 2, all asset categories, then the method call would be as follows:
        /// double consequenceValue = ConsequencesReducedExceededWithProbabilityQ(.98, 1, damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="exceedanceProbability"></param>
        /// <param name="alternativeID"></param>
        /// <param name="impactAreaID"></param>
        /// <param name="damageCategory"></param> either residential, commercial, etc...
        /// <param name="assetCategory"></param> either structure, content, etc...
        /// <returns></returns>
        public double ConsequencesReducedExceededWithProbabilityQ(double exceedanceProbability, int alternativeID, int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
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
        /// <summary>
        /// This method gets the histogram (distribution) of consequences for the given damage category(ies), asset category(ies), and impact area(s)
        /// The level of aggregation of the distribution of consequences is determined by the arguments used in the method
        /// For example, if you wanted a histogram for alternative 1, residential, impact area 2, all asset categories, then the method call would be as follows:
        /// ThreadsafeInlineHistogram histogram = GetAlternativeResultsHistogram(1, damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="alternativeID"></param>
        /// <param name="impactAreaID"></param>
        /// <param name="damageCategory"></param>
        /// <param name="assetCategory"></param>
        /// <returns></returns>
        public Statistics.Histograms.ThreadsafeInlineHistogram GetAlternativeResultsHistogram(int alternativeID, int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            AlternativeResults alternativeResults = GetAlternativeResults(alternativeID);
            return alternativeResults.GetConsequencesHistogram(impactAreaID, damageCategory, assetCategory);
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

        public XElement WriteToXML()
        {
            XElement mainElement = new XElement("AlternativeComparisonReportResults");
            foreach (AlternativeResults alternativeResults in _resultsList)
            {
                XElement xElement = alternativeResults.WriteToXML();
                mainElement.Add(xElement);
            }
            return mainElement;
        }
        public static AlternativeComparisonReportResults ReadFromXML(XElement xElement)
        {
            AlternativeComparisonReportResults alternativeComparisonReportResults = new AlternativeComparisonReportResults();
            foreach (XElement element in xElement.Elements())
            {
                AlternativeResults alternativeResults = AlternativeResults.ReadFromXML(element);
                alternativeComparisonReportResults.AddAlternativeResults(alternativeResults);
            }
            return alternativeComparisonReportResults;
        }
        #endregion
    }
}
