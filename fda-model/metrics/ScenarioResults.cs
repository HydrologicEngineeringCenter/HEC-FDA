
using System.Collections.Generic;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using Statistics.Histograms;

namespace metrics
{
    public class ScenarioResults : HEC.MVVMFramework.Base.Implementations.Validation, IReportMessage
    {
        #region Fields 
        List<IContainImpactAreaScenarioResults> _resultsList;
        #endregion

        #region Properties 
        public List<IContainImpactAreaScenarioResults> ResultsList
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
            _resultsList = new List<IContainImpactAreaScenarioResults>();
        }
        #endregion

        #region Methods
        public double MeanAEP(int impactAreaID, int thresholdID)
        {
            return GetResults(impactAreaID).MeanAEP(thresholdID);
        }
        public double MedianAEP(int impactAreaID, int thresholdID)
        {
            return GetResults(impactAreaID).MedianAEP(thresholdID);
        }
        public double AssuranceOfAEP(int impactAreaID, int thresholdID, double exceedanceProbability)
        {
            return GetResults(impactAreaID).AssuranceOfAEP(thresholdID, exceedanceProbability);
        }
        public double LongTermExceedanceProbability(int impactAreaID, int thresholdID, int years)
        {
            return GetResults(impactAreaID).LongTermExceedanceProbability(thresholdID, years);
        }
        public double AssuranceOfEvent(int impactAreaID, int thresholdID, double standardNonExceedanceProbability)
        {
            return GetResults(impactAreaID).AssuranceOfEvent(thresholdID, standardNonExceedanceProbability);
        }
        public double MeanExpectedAnnualConsequences(int impactAreaID = -999, string damageCategory = null, string assetCategory= null)
        {//TODO: This could probably be more efficient 
            double consequenceValue = 0;
            foreach (ImpactAreaScenarioResults impactAreaScenarioResults in ResultsList)
            {
                foreach (ConsequenceResult consequenceResult in impactAreaScenarioResults.ConsequenceResults.ConsequenceResultList)
                {
                    if (damageCategory == null && assetCategory == null && impactAreaID == -999)
                    {
                        consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                    }
                    if (damageCategory != null && assetCategory == null && impactAreaID == -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory))
                        {
                            consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                        }
                    }
                    if (damageCategory == null && assetCategory != null && impactAreaID == -999)
                    {
                        if (assetCategory.Equals(consequenceResult.AssetCategory))
                        {
                            consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                        }
                    }
                    if (damageCategory == null && assetCategory == null && impactAreaID != -999)
                    {
                        if (impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                        }
                    }
                    if (damageCategory != null && assetCategory != null && impactAreaID == -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory))
                        {
                            consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                        }
                    }
                    if (damageCategory != null && assetCategory == null && impactAreaID != -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                        }
                    }
                    if (damageCategory == null && assetCategory != null && impactAreaID != -999)
                    {
                        if (assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                        }
                    }
                    if (damageCategory != null && assetCategory != null && impactAreaID != -999)
                    {
                        if(damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            return consequenceResult.MeanExpectedAnnualConsequences();
                        }
                        
                    }
                }
            }
            return consequenceValue;
        }
        //TODO: Do we need to aggregate over impact area? YES
        public double ConsequencesExceededWithProbabilityQ(double exceedanceProbability, int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            double consequenceValue = 0;
            foreach (ImpactAreaScenarioResults impactAreaScenarioResults in ResultsList)
            {
                foreach (ConsequenceResult consequenceResult in impactAreaScenarioResults.ConsequenceResults.ConsequenceResultList)
                {
                    if (damageCategory == null && assetCategory == null && impactAreaID == -999)
                    {
                        consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                    }
                    if (damageCategory != null && assetCategory == null && impactAreaID == -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory))
                        {
                            consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                    if (damageCategory == null && assetCategory != null && impactAreaID == -999)
                    {
                        if (assetCategory.Equals(consequenceResult.AssetCategory))
                        {
                            consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                    if (damageCategory == null && assetCategory == null && impactAreaID != -999)
                    {
                        if (impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                    if (damageCategory != null && assetCategory != null && impactAreaID == -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory))
                        {
                            consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                    if (damageCategory != null && assetCategory == null && impactAreaID != -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                    if (damageCategory == null && assetCategory != null && impactAreaID != -999)
                    {
                        if (assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                    if (damageCategory != null && assetCategory != null && impactAreaID != -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            return consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                }
            }
            return consequenceValue;
        } 
        //TODO: Aggregate over impact areas 
        public ThreadsafeInlineHistogram GetConsequencesHistogram(int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            List<ThreadsafeInlineHistogram> histograms = new List<ThreadsafeInlineHistogram>();

            foreach (ImpactAreaScenarioResults impactAreaScenarioResults in ResultsList)
            {
                foreach (ConsequenceResult consequenceResult in impactAreaScenarioResults.ConsequenceResults.ConsequenceResultList)
                {
                    if (damageCategory == null && assetCategory == null && impactAreaID == -999)
                    {
                        histograms.Add(consequenceResult.ConsequenceHistogram);
                    }
                    if (damageCategory != null && assetCategory == null && impactAreaID == -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory))
                        {
                            histograms.Add(consequenceResult.ConsequenceHistogram);
                        }
                    }
                    if (damageCategory == null && assetCategory != null && impactAreaID == -999)
                    {
                        if (assetCategory.Equals(consequenceResult.AssetCategory))
                        {
                            histograms.Add(consequenceResult.ConsequenceHistogram);
                        }
                    }
                    if (damageCategory == null && assetCategory == null && impactAreaID != -999)
                    {
                        if (impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            histograms.Add(consequenceResult.ConsequenceHistogram);
                        }
                    }
                    if (damageCategory != null && assetCategory != null && impactAreaID == -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory))
                        {
                            histograms.Add(consequenceResult.ConsequenceHistogram);
                        }
                    }
                    if (damageCategory != null && assetCategory == null && impactAreaID != -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            histograms.Add(consequenceResult.ConsequenceHistogram);
                        }
                    }
                    if (damageCategory == null && assetCategory != null && impactAreaID != -999)
                    {
                        if (assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            histograms.Add(consequenceResult.ConsequenceHistogram);
                        }
                    }
                    if (damageCategory != null && assetCategory != null && impactAreaID != -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            return consequenceResult.ConsequenceHistogram;
                        }
                    }
                }
            }
            ThreadsafeInlineHistogram aggregatedHistogram = ThreadsafeInlineHistogram.AddHistograms(histograms);
            return aggregatedHistogram;
        }
        public void AddResults(IContainImpactAreaScenarioResults resultsToAdd)
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
            string message = $"The IMPACT AREA SCENARIO RESULTS could not be found. an arbitrary object is being returned";
            HEC.MVVMFramework.Model.Messaging.ErrorMessage errorMessage = new HEC.MVVMFramework.Model.Messaging.ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
            ReportMessage(this, new MessageEventArgs(errorMessage));
            return dummyResults;
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        public XElement WriteToXML()
        {
            XElement mainElement = new XElement("ScenarioResults");
            foreach (ImpactAreaScenarioResults impactAreaScenarioResults in _resultsList)
            {
                XElement impactAreaScenarioResultsElement = impactAreaScenarioResults.WriteToXml();
                mainElement.Add(impactAreaScenarioResults);
            }
            return mainElement;
        }

        public static ScenarioResults ReadFromXML(XElement xElement)
        {
            ScenarioResults scenarioResults = new ScenarioResults();
            foreach (XElement element in xElement.Elements())
            {
                IContainImpactAreaScenarioResults impactAreaScenarioResults = ImpactAreaScenarioResults.ReadFromXML(element);
                scenarioResults.AddResults(impactAreaScenarioResults);
            }
            return scenarioResults;
        }
        #endregion

    }
}
