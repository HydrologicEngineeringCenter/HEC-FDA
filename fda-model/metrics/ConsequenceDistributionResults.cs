using System;
using System.Collections.Generic;
using Statistics;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Model.Messaging;
using Statistics.Histograms;
using paireddata;

namespace metrics
{ 
    public class ConsequenceDistributionResults : HEC.MVVMFramework.Base.Implementations.Validation, IReportMessage
    {
        #region Fields
        private static string STRUCTURE_ASSET_CATEGORY = "Structure";
        private static string CONTENT_ASSET_CATEGORY = "Content";
        private static string OTHER_ASSET_CATEGORY = "Other";
        private static string VEHICLE_ASSET_CATEGRY = "Vehicle";
        private int _alternativeID;
        private ConvergenceCriteria _ConvergenceCriteria;
        private List<ConsequenceDistributionResult> _consequenceResultList;
        //impact area to be string?
        private bool _isNull;
        
        #endregion

        #region Properties 
        public List<ConsequenceDistributionResult> ConsequenceResultList
        {
            get
            {
                return _consequenceResultList;
            }
        }
        //this needs to be an error report
        public event MessageReportedEventHandler MessageReport;
        public bool IsNull
        {
            get
            {
                return _isNull;
            }
        }
        internal int AlternativeID
        {
            get
            {
                return _alternativeID;
            }
        }
        #endregion
        #region Constructors
        public ConsequenceDistributionResults()
        {
            _consequenceResultList = new List<ConsequenceDistributionResult>();
            ConsequenceDistributionResult dummyConsequenceDistributionResult = new ConsequenceDistributionResult();
            _consequenceResultList.Add(dummyConsequenceDistributionResult);
            _isNull = true;
            MessageHub.Register(this);
        }
        internal ConsequenceDistributionResults(bool isNull)
        {
            _consequenceResultList = new List<ConsequenceDistributionResult>();
            _isNull = isNull;
            MessageHub.Register(this);
        }
        internal ConsequenceDistributionResults(int alternativeID)
        {
            _consequenceResultList = new List<ConsequenceDistributionResult>();
            _alternativeID = alternativeID;
            _isNull = false;
            MessageHub.Register(this);
        }
        //public for testing
        public ConsequenceDistributionResults(ConvergenceCriteria convergenceCriteria)
        {
            _consequenceResultList = new List<ConsequenceDistributionResult>();
            _isNull = false;
            _ConvergenceCriteria = convergenceCriteria;
        }
        private ConsequenceDistributionResults(List<ConsequenceDistributionResult> damageResults)
        {
            _consequenceResultList = damageResults;
            _isNull = false;
            MessageHub.Register(this);

        }
        #endregion

        #region Methods 
        internal void AddNewConsequenceResultObject(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, int impactAreaID, bool histogramIsZeroValued = false)
        {
            ConsequenceDistributionResult damageResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
            if (damageResult.IsNull)
            {
                ConsequenceDistributionResult newDamageResult = new ConsequenceDistributionResult(damageCategory, assetCategory, convergenceCriteria, impactAreaID);
                newDamageResult.ConsequenceHistogram.HistogramIsZeroValued = histogramIsZeroValued;
                _consequenceResultList.Add(newDamageResult);
            }
        }
        //public for testing purposes
        public void AddExistingConsequenceResultObject(ConsequenceDistributionResult consequenceResultToAdd)
        {
            ConsequenceDistributionResult consequenceResult = GetConsequenceResult(consequenceResultToAdd.DamageCategory, consequenceResultToAdd.AssetCategory, consequenceResultToAdd.RegionID);
            if (consequenceResult.IsNull)
            {
                _consequenceResultList.Add(consequenceResultToAdd);
            }
        }
        internal void AddConsequenceRealization(double dammageEstimate, string damageCategory, string assetCategory, int impactAreaID, Int64 iteration)
        {
            ConsequenceDistributionResult damageResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
            damageResult.AddConsequenceRealization(dammageEstimate, iteration);

        }
        internal void AddConsequenceRealization(ConsequenceResults consequenceResults, int impactAreaID, Int64 iteration)
        {
            foreach (ConsequenceResult consequenceResult in consequenceResults.ConsequenceResultList)
            {
                AddConsequenceRealizationByAssetCategory(consequenceResult.StructureDamage, consequenceResult.DamageCategory, STRUCTURE_ASSET_CATEGORY, consequenceResult.RegionID, iteration);
                AddConsequenceRealizationByAssetCategory(consequenceResult.ContentDamage, consequenceResult.DamageCategory, CONTENT_ASSET_CATEGORY, consequenceResult.RegionID, iteration);
                AddConsequenceRealizationByAssetCategory(consequenceResult.OtherDamage, consequenceResult.DamageCategory, OTHER_ASSET_CATEGORY, consequenceResult.RegionID, iteration);
                AddConsequenceRealizationByAssetCategory(consequenceResult.VehicleDamage, consequenceResult.DamageCategory, VEHICLE_ASSET_CATEGRY, consequenceResult.RegionID, iteration);
            }
        }
        private void AddConsequenceRealizationByAssetCategory(double consequenceRealization, string damageCategory, string assetCategory, int impactAreaID, Int64 iteration)
        {
            ConsequenceDistributionResult structureConsequenceDistributionResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
            if (structureConsequenceDistributionResult.IsNull)
            {
                ConsequenceDistributionResult newStructureDamageResult = new ConsequenceDistributionResult(damageCategory, assetCategory, _ConvergenceCriteria, impactAreaID);
                newStructureDamageResult.AddConsequenceRealization(consequenceRealization, iteration);
                _consequenceResultList.Add(newStructureDamageResult);
            }
            else
            {
                structureConsequenceDistributionResult.AddConsequenceRealization(consequenceRealization, iteration);
            }
        }
        /// <summary>
        /// This method returns the mean of the consequences measure of the consequence result object for the given damage category, asset category, impact area combination 
        /// Damage measures could be EAD or other measures of consequences 
        /// Note that when working with impact area scenario results, there is only 1 impact area 
        /// The level of aggregation of the mean is determined by the arguments used in the method
        /// For example, if you wanted mean EAD for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// double meanEAD = MeanDamage(damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="damageCategory"></param> either residential, commercial, etc...the default is null
        /// <param name="assetCategory"></param> either structure, content, etc...the default is null
        /// <param name="impactAreaID"></param> the default is the null value -999
        /// <returns></returns>The mean of consequences
        public double MeanDamage(string damageCategory = null, string assetCategory = null, int impactAreaID = -999)
        {
            double consequenceValue = 0;
            foreach (ConsequenceDistributionResult consequenceResult in _consequenceResultList)
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
                    return GetConsequenceResult(damageCategory, assetCategory, impactAreaID).MeanExpectedAnnualConsequences();
                }
            }
            return consequenceValue;
        }
        /// <summary>
        /// This method calls the inverse CDF of the damage histogram up to the non-exceedance probabilty. The method accepts exceedance probability as an argument. 
        /// The level of aggregation of  consequences is determined by the arguments used in the method
        /// For example, if you wanted the EAD exceeded with probability .98 for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// double consequenceValue = ConsequenceExceededWithProbabilityQ(.98, damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="damageCategory"></param> either residential, commercial, etc....the default is null
        /// <param name="exceedanceProbability"></param>
        /// <param name="assetCategory"></param> either structure, content, etc...the default is null
        /// <param name="impactAreaID"></param>the default is the null value -999
        /// <returns></returns>the level of consequences exceeded by the specified probability 
        public double ConsequenceExceededWithProbabilityQ(double exceedanceProbability, string damageCategory = null, string assetCategory = null, int impactAreaID = -999)
        {
            double consequenceValue = 0;
            foreach (ConsequenceDistributionResult consequenceResult in _consequenceResultList)
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
                    return GetConsequenceResult(damageCategory, assetCategory, impactAreaID).ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                }
            }
            return consequenceValue;
        }
        /// <summary>
        /// This method returns a consequence result for the given damage category, asset category, and impact area 
        /// </summary>
        /// <param name="damageCategory"></param>
        /// <param name="assetCategory"></param>
        /// <param name="impactAreaID"></param>
        /// <returns></returns>
        public ConsequenceDistributionResult GetConsequenceResult(string damageCategory, string assetCategory, int impactAreaID)
        {
            foreach (ConsequenceDistributionResult damageResult in _consequenceResultList)
            {
                //The impact area should always be equal because a consequence result reflects 1 impact area and a consequence resultS reflects 1 impact area   
                if (damageResult.RegionID.Equals(impactAreaID))
                {
                    if (damageResult.DamageCategory.Equals(damageCategory))
                    {
                        if (damageResult.AssetCategory.Equals(assetCategory))
                        {
                            return damageResult;
                        } 
                    }
                }
            }
        string message = "The requested damage category - asset category - impact area combination could not be found. An arbitrary object is being returned";
        ErrorMessage errorMessage = new ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
        ReportMessage(this, new MessageEventArgs(errorMessage));
            ConsequenceDistributionResult dummyResult = new ConsequenceDistributionResult();
            return dummyResult;
        }

        internal void ForceDeQueue()
        {
            foreach (ConsequenceDistributionResult consequenceResult in ConsequenceResultList)
            {
                consequenceResult.ConsequenceHistogram.ForceDeQueue();
            }
        }

        public bool Equals(ConsequenceDistributionResults inputDamageResults)
        {
           foreach (ConsequenceDistributionResult damageResult in _consequenceResultList)
           {
               ConsequenceDistributionResult inputDamageResult = inputDamageResults.GetConsequenceResult(damageResult.DamageCategory, damageResult.AssetCategory, damageResult.RegionID);
               bool resultsMatch = damageResult.Equals(inputDamageResult);
               if (!resultsMatch)
               {
                   return false;
               }
            }
            return true;
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        /// <summary>
        /// This method gets the histogram (distribution) of consequences for the given damage category(ies), asset category(ies), and impact area(s)
        /// The level of aggregation of the distribution of consequences is determined by the arguments used in the method
        /// For example, if you wanted a histogram for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// ThreadsafeInlineHistogram histogram = GetConsequenceResultsHistogram(damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="damageCategory"></param> The default is null 
        /// <param name="assetCategory"></param> The default is null 
        /// <param name="impactAreaID"></param> The default is a null value (-999)
        /// <returns></returns> Aggregated consequences histogram 
        public IHistogram GetConsequenceResultsHistogram(string damageCategory = null, string assetCategory = null, int impactAreaID = -999)
        {
            List<IHistogram> histograms = new List<IHistogram>();
            foreach (ConsequenceDistributionResult consequenceResult in _consequenceResultList)
            {
                if(damageCategory == null && assetCategory == null && impactAreaID == -999)
                {
                    histograms.Add(consequenceResult.ConsequenceHistogram);
                }
                if(damageCategory != null && assetCategory == null && impactAreaID == -999)
                {
                    if(damageCategory.Equals(consequenceResult.DamageCategory))
                    {
                        histograms.Add(consequenceResult.ConsequenceHistogram);
                    }
                }
                if(damageCategory == null && assetCategory != null && impactAreaID == -999)
                {
                    if(assetCategory.Equals(consequenceResult.AssetCategory))
                    {
                        histograms.Add(consequenceResult.ConsequenceHistogram);
                    }
                }
                if(damageCategory == null && assetCategory == null && impactAreaID != -999)
                {
                    if(impactAreaID.Equals(consequenceResult.RegionID))
                    {
                        histograms.Add(consequenceResult.ConsequenceHistogram);
                    }
                }
                if(damageCategory != null && assetCategory != null && impactAreaID == -999)
                {
                    if(damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory))
                    {
                        histograms.Add(consequenceResult.ConsequenceHistogram);
                    }
                }
                if(damageCategory != null && assetCategory == null && impactAreaID != -999)
                {
                    if(damageCategory.Equals(consequenceResult.DamageCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                    {
                        histograms.Add(consequenceResult.ConsequenceHistogram);
                    }
                }
                if(damageCategory == null && assetCategory != null && impactAreaID != -999)
                {
                    if(assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                    {
                        histograms.Add(consequenceResult.ConsequenceHistogram);
                    }
                }
                if (damageCategory != null && assetCategory != null && impactAreaID != -999)
                {
                    return GetConsequenceResult(damageCategory, assetCategory, impactAreaID).ConsequenceHistogram;
                }
            }
            IHistogram aggregateHistogram;
            if (histograms.Count == 0)
            {
                string message = "The requested damage category - asset category - impact area combination could not be found. An arbitrary object is being returned";
                ErrorMessage errorMessage = new ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
                ReportMessage(this, new MessageEventArgs(errorMessage));
                aggregateHistogram = new Histogram();
            }
            else
            {
                aggregateHistogram = Histogram.AddHistograms(histograms);
            }
            return aggregateHistogram;

        }

        public bool ResultsAreConverged(double upperConfidenceLimit, double lowerConfidenceLimit)
        {
            bool allHistogramsAreConverged = true;
            foreach (ConsequenceDistributionResult consequenceDistributionResult in _consequenceResultList)
            {
                bool histogramIsConverged = consequenceDistributionResult.ConsequenceHistogram.IsHistogramConverged(upperConfidenceLimit, lowerConfidenceLimit);
                if (!histogramIsConverged)
                {
                    allHistogramsAreConverged = false;
                    break;
                }
            }
            return allHistogramsAreConverged;
        }
        public static List<UncertainPairedData> ToUncertainPairedData(List<double> xValues, List<ConsequenceDistributionResults> yValues)
        {
            List<UncertainPairedData> uncertainPairedDataList = new List<UncertainPairedData>();
            List<int> impactAreas = yValues[yValues.Count - 1].GetImpactAreas();
            List<string> damageCategories = yValues[yValues.Count - 1].GetDamageCategories();
            List<string> assetCategories = yValues[yValues.Count - 1].GetAssetCategories(); ;
            foreach (int impactAreaID in impactAreas)
            {
                foreach (string damageCategory in damageCategories)
                {
                    foreach (string assetCategory in assetCategories)
                    {
                        CurveMetaData curveMetaData = new CurveMetaData("X Values", "Consequences", "Consequences Uncertain Paired Data", damageCategory,CurveTypesEnum.MonotonicallyIncreasing, impactAreaID, assetCategory);
                        List<IHistogram> histograms = new List<IHistogram>();
                        foreach (ConsequenceDistributionResults consequenceDistributions in yValues)
                        {
                            histograms.Add(consequenceDistributions.GetConsequenceResultsHistogram(damageCategory, assetCategory, impactAreaID));
                        }
                        UncertainPairedData uncertainPairedData = new UncertainPairedData(xValues.ToArray(), histograms.ToArray(), curveMetaData);
                        uncertainPairedDataList.Add(uncertainPairedData);
                    }
                }
            }
            return uncertainPairedDataList;
        }

        private List<string> GetAssetCategories()
        {
            List<string> assetCategories = new List<string>();
            foreach (ConsequenceDistributionResult consequenceDistributionResult in _consequenceResultList)
            {
                if (!assetCategories.Contains(consequenceDistributionResult.AssetCategory))
                {
                    assetCategories.Add(consequenceDistributionResult.AssetCategory);
                }
            }
            return assetCategories;
        }

        private List<int> GetImpactAreas()
        {
            List<int> impactAreas = new List<int>();
            foreach (ConsequenceDistributionResult consequenceDistributionResult in _consequenceResultList)
            {
                if(!impactAreas.Contains(consequenceDistributionResult.RegionID))
                {
                    impactAreas.Add(consequenceDistributionResult.RegionID);
                }
            }
            return impactAreas;
        }

        private List<string> GetDamageCategories()
        {
            List<string> damageCategories = new List<string>();
            foreach (ConsequenceDistributionResult consequenceDistributionResult in _consequenceResultList)
            {
                if (!damageCategories.Contains(consequenceDistributionResult.DamageCategory))
                {
                    damageCategories.Add(consequenceDistributionResult.DamageCategory);
                }
            }
            return damageCategories;
        }

        public XElement WriteToXML()
        {
            XElement masterElem = new XElement("EAD_Results");
            foreach (ConsequenceDistributionResult damageResult in _consequenceResultList)
            {
                XElement damageResultElement = damageResult.WriteToXML();
                damageResultElement.Name = $"{damageResult.DamageCategory}-{damageResult.AssetCategory}";
                masterElem.Add(damageResultElement);
            }
            return masterElem;
        }

        public static ConsequenceDistributionResults ReadFromXML(XElement xElement)
        {
            List<ConsequenceDistributionResult> damageResults = new List<ConsequenceDistributionResult>();
            foreach (XElement histogramElement in xElement.Elements())
            {
                damageResults.Add(ConsequenceDistributionResult.ReadFromXML(histogramElement));
            }
            return new ConsequenceDistributionResults(damageResults);
        }

        #endregion
    }
}