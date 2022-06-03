using System;
using System.Collections.Generic;
using Statistics;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Model.Messaging;
using Statistics.Histograms;

namespace metrics
{ //TODO: I THINK SOME OR ALL OF THIS CLASS SHOULD BE INTERNAL 

    public class ConsequenceResults : HEC.MVVMFramework.Base.Implementations.Validation, IReportMessage
    {
        #region Fields
        private List<ConsequenceResult> _consequenceResultList;
        //impact area to be string?
        private int _regionID;//impact area ID or census block ID
        private bool _isNull;
        #endregion

        #region Properties 
        public List<ConsequenceResult> ConsequenceResultList
        {
            get
            {
                return _consequenceResultList;
            }
        }
        public int RegionID
        {
            get
            {
                return _regionID;
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
        #endregion
        #region Constructors
        public ConsequenceResults()
        {
            _consequenceResultList = new List<ConsequenceResult>();
            _regionID = 0;
            _isNull = true;
        }
        public ConsequenceResults(int impactAreaID){
            _consequenceResultList = new List<ConsequenceResult>();
            _regionID = impactAreaID;
            _isNull = false;
        }
        private ConsequenceResults(List<ConsequenceResult> damageResults, int impactAreaID)
        {
            _consequenceResultList = damageResults;
            _regionID = impactAreaID;
            _isNull = false;

        }
        #endregion

        #region Methods 
        internal void AddConsequenceResultObject(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, int impactAreaID)
        {
            ConsequenceResult damageResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
            if (damageResult.IsNull)
            {
                ConsequenceResult newDamageResult = new ConsequenceResult(damageCategory, assetCategory, convergenceCriteria, impactAreaID);
                _consequenceResultList.Add(newDamageResult);
            }
        }
        internal void AddConsequenceResult(ConsequenceResult consequenceResultToAdd)
        {
            ConsequenceResult consequenceResult = GetConsequenceResult(consequenceResultToAdd.DamageCategory, consequenceResultToAdd.AssetCategory, consequenceResultToAdd.RegionID);
            if (consequenceResult.IsNull)
            {
                _consequenceResultList.Add(consequenceResultToAdd);
            }
        }
        internal void AddConsequenceRealization(double dammageEstimate, string damageCategory, string assetCategory, int impactAreaID, Int64 iteration)
        {
            ConsequenceResult damageResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
            damageResult.AddConsequenceRealization(dammageEstimate, iteration);

        }
        /// <summary>
        /// This method returns the mean of the consequences measure of the consequence result object for the given damage category, asset category, impact area combination 
        /// Damage measures could be EAD or other measures of consequences 
        /// Note that when working with impact area scenario results, there is only 1 impact area 
        /// </summary>
        /// <param name="damageCategory"></param> either residential, commercial, etc...
        /// <param name="assetCategory"></param> either structure, content, etc...
        /// <param name="impactAreaID"></param>
        /// <returns></returns>
        public double MeanDamage(string damageCategory, string assetCategory, int impactAreaID)
        {
            ConsequenceResult damageResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
            return damageResult.MeanExpectedAnnualConsequences();
        }
        /// <summary>
        /// This method returns the mean of the consequences measure for the given category and impact area 
        /// If categoryIsDamageCategory is true, then category should be a damage category and the mean is summed over asset categories 
        /// If categoryIsDamageCategory is false, then category should be an asset category and the mean is summed over damage categories 
        /// Damage measures could be EAD or other measures of consequences 
        /// Note that when working with impact area scenario results, there is only 1 impact area 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="impactAreaID"></param>
        /// <param name="categoryIsDamageCategory"></param>
        /// <returns></returns>
        public double MeanDamage(string category, int impactAreaID, bool categoryIsDamageCategory = true)
        {
            double meanConsequence = 0;
            foreach (ConsequenceResult consequenceResult in _consequenceResultList)
            {
                if (categoryIsDamageCategory)
                {
                    if ((consequenceResult.DamageCategory.Equals(category)) && (consequenceResult.RegionID.Equals(impactAreaID)))
                    {
                        meanConsequence += consequenceResult.MeanExpectedAnnualConsequences();
                    }
                }
                else
                {
                    if ((consequenceResult.AssetCategory.Equals(category)) && (consequenceResult.RegionID.Equals(impactAreaID))) 
                    {
                        meanConsequence += consequenceResult.MeanExpectedAnnualConsequences();
                    }
                }

            }
            return meanConsequence;
        }
        /// <summary>
        /// This method returns the mean of the consequences measure for the given category 
        /// If categoryIsDamageCategory is true, then category should be a damage category and the mean is summed over asset categories and impact areas
        /// If categoryIsDamageCategory is false, then category should be an asset category and the mean is summed over damage categories and impact areas 
        /// Damage measures could be EAD or other measures of consequences 
        /// Note that when working with impact area scenario results, there is only 1 impact area 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="categoryIsDamageCategory"></param> 
        /// <returns></returns>
        public double MeanDamage(string category, bool categoryIsDamageCategory = true)
        {
            double meanConsequence = 0;
            foreach (ConsequenceResult consequenceResult in _consequenceResultList)
            {
                if (categoryIsDamageCategory)
                {
                    if ((consequenceResult.DamageCategory.Equals(category)))
                    {
                        meanConsequence += consequenceResult.MeanExpectedAnnualConsequences();
                    }
                }
                else
                {
                    if ((consequenceResult.AssetCategory.Equals(category)))
                    {
                        meanConsequence += consequenceResult.MeanExpectedAnnualConsequences();
                    }
                }

            }
            return meanConsequence;
        }
        /// <summary>
        /// This method returns the mean of the consequences measure for the given impact area 
        /// The mean is summed over damage categories and asset categories 
        /// Damage measures could be EAD or other measures of consequences 
        /// Note that when working with impact area scenario results, there is only 1 impact area 
        /// </summary>
        /// <param name="impactAreaID"></param>
        /// <returns></returns>
        public double MeanDamage(int impactAreaID)
        {
            double meanConsequence = 0;
            foreach (ConsequenceResult consequenceResult in _consequenceResultList)
            {
                    if ((consequenceResult.RegionID.Equals(impactAreaID)))
                    {
                        meanConsequence += consequenceResult.MeanExpectedAnnualConsequences();
                    }
            }
            return meanConsequence;
        }
        /// <summary>
        /// This method returns the mean of the consequences measure 
        /// The mean is summed over damage categories, asset categories, and impact areas  
        /// Damage measures could be EAD or other measures of consequences 
        /// Note that when working with impact area scenario results, there is only 1 impact area         
        /// </summary>
        /// <returns></returns>
        public double MeanDamage()
        {
            double meanConsequence = 0;
            foreach (ConsequenceResult consequenceResult in _consequenceResultList)
            {
                    meanConsequence += consequenceResult.MeanExpectedAnnualConsequences();
            }
            return meanConsequence;
        }
        /// <summary>
        /// This method calls the inverse CDF of thedamage histogram up to the non-exceedance probabilty. The method accepts exceedance probability as an argument. 
        /// </summary>
        /// <param name="damageCategory"></param> either residential, commercial, etc....
        /// <param name="exceedanceProbability"></param>
        /// <param name="assetCategory"></param> either structure, content, etc...
        /// <param name="impactAreaID"></param>
        /// <returns></returns>
        public double ConsequenceExceededWithProbabilityQ(string damageCategory, double exceedanceProbability, string assetCategory, int impactAreaID)
        {
            ConsequenceResult damageResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
            double quantileRequested = damageResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
            return quantileRequested;

        }
        /// <summary>
        /// This method calls the inverse CDF of the damage histograms for the given category and impact area up to the non-exceedance probability. 
        /// The method accepts exceedance probability as an arugment, not non-exceedance probability 
        /// If categoryIsDamageCategory is true, then the inverse CDF is summed over asset categories
        /// Else, the inverse CDF is summed over damage categories. 
        /// Note that when working with impact area scenario results, there is only 1 impact area 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="exceedanceProbability"></param>
        /// <param name="impactAreaID"></param>
        /// <param name="categoryIsDamageCategory"></param>
        /// <returns></returns>
        public double ConsequenceExceededWithProbabilityQ(string category, double exceedanceProbability, int impactAreaID, bool categoryIsDamageCategory = true)
        {
            double consequenceValue = 0;
            foreach (ConsequenceResult consequenceResult in _consequenceResultList)
            {
                if (categoryIsDamageCategory)
                {
                    if ((consequenceResult.DamageCategory.Equals(category)) && (consequenceResult.RegionID.Equals(impactAreaID)))
                    {
                        consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                    }
                }
                else
                {
                    if ((consequenceResult.AssetCategory.Equals(category)) && (consequenceResult.RegionID.Equals(impactAreaID)))
                    {
                        consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                    }
                }

            }
            return consequenceValue;
        }
        /// <summary>
        /// This method calls the inverse CDF of the damage histograms for the given category up to the non-exceedance probability. 
        /// The method accepts exceedance probability as an arugment, not non-exceedance probability 
        /// If categoryIsDamageCategory is true, then the inverse CDF is summed over asset categories and impact areas
        /// Else, the inverse CDF is summed over damage categories and impact areas.   
        /// Note that when working with impact area scenario results, there is only 1 impact area
        /// </summary>
        /// <param name="category"></param>
        /// <param name="exceedanceProbability"></param>
        /// <param name="categoryIsDamageCategory"></param>
        /// <returns></returns>
        public double ConsequenceExceededWithProbabilityQ(string category, double exceedanceProbability, bool categoryIsDamageCategory = true)
        {
            double consequenceValue = 0;
            foreach (ConsequenceResult consequenceResult in _consequenceResultList)
            {
                if (categoryIsDamageCategory)
                {
                    if ((consequenceResult.DamageCategory.Equals(category)))
                    {
                        consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                    }
                }
                else
                {
                    if ((consequenceResult.AssetCategory.Equals(category)))
                    {
                        consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                    }
                }

            }
            return consequenceValue;
        }
        /// <summary>
        /// This method calls the inverse CDF of the damage histograms for the given impact area up to the non-exceedance probability. 
        /// The method accepts exceedance probability as an arugment, not non-exceedance probability 
        /// The consequence value is summed over damage categories and asset categories  
        /// Note that when working with impact area scenario results, there is only 1 impact area        
        /// </summary>
        /// <param name="exceedanceProbability"></param>
        /// <param name="impactAreaID"></param>
        /// <returns></returns>
        public double ConsequenceExceeededWithProbabilityQ(double exceedanceProbability, int impactAreaID)
        {
            double consequenceValue = 0;
            foreach (ConsequenceResult consequenceResult in _consequenceResultList)
            {
                    if ((consequenceResult.RegionID.Equals(impactAreaID)))
                    {
                        consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                    }
            }
            return consequenceValue;
        }
        /// <summary>
        /// This method calls the inverse CDF of the damage histograms up to the non-exceedance probability. 
        /// The method accepts exceedance probability as an arugment, not non-exceedance probability 
        /// The consequence value is summed over damage categories, asset categories, and impact areas  
        /// Note that when working with impact area scenario results, there is only 1 impact area         
        /// </summary>
        /// <param name="exceedanceProbability"></param>
        /// <returns></returns>
        public double ConsequenceExceededWithProbabilityQ(double exceedanceProbability)
        {
            double consequenceValue = 0;
            foreach (ConsequenceResult consequenceResult in _consequenceResultList)
            {
                    consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
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
        public ConsequenceResult GetConsequenceResult(string damageCategory, string assetCategory, int impactAreaID)
        {
            foreach (ConsequenceResult damageResult in _consequenceResultList)
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
            ConsequenceResult dummyResult = new ConsequenceResult();
            return dummyResult;
        }
        public bool Equals(ConsequenceResults inputDamageResults)
        {
           foreach (ConsequenceResult damageResult in _consequenceResultList)
           {
               ConsequenceResult inputDamageResult = inputDamageResults.GetConsequenceResult(damageResult.DamageCategory, damageResult.AssetCategory, damageResult.RegionID);
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
        [Obsolete("An overloaded method of MeanDamage has been created to return MeanDamage in a systematic way. This method is now deprecated")]
        /// <summary>
        /// This method returns the sum of mean EAD over damage categories for a given asset category. 
        /// For example, for asset category of structure, this would return the mean EAD for all damage categories that have an asset category of structure. 
        /// </summary>
        /// <param name="assetCategory"></param>
        /// <returns></returns>
        public double MeanExpectedAnnualConsequencesAllDamageCategories(string assetCategory)
        {
            double meanEAD = 0;
            foreach (ConsequenceResult consequenceResult in ConsequenceResultList)
            {
                if (consequenceResult.AssetCategory.Equals(assetCategory))
                {
                    meanEAD += consequenceResult.MeanExpectedAnnualConsequences();
                }
            }
            return meanEAD;
        }
        [Obsolete("This method is deprecated. An overloaded method of MeanDamage has been created to return MeanDamage in a systematic way.")]
        /// <summary>
        /// This method returns the sum of mean EAD over damage categories for a given asset category. 
        /// For example, for asset category of structure, this would return the mean EAD for all damage categories that have an asset category of structure.         /// </summary>
        /// <param name="damageCategory"></param>
        /// <returns></returns>
        public double MeanExpectedAnnualConsequencesAllAssetCategories(string damageCategory)
        {
            double meanEAD = 0;
            foreach (ConsequenceResult consequenceResult in ConsequenceResultList)
            {
                if(consequenceResult.DamageCategory.Equals(damageCategory))
                {
                    meanEAD += consequenceResult.MeanExpectedAnnualConsequences();
                }
            }
            return meanEAD;
        }
        /// <summary>
        /// This method gets the histogram (distribution) of consequences for the given damage category, asset category, and impact area 
        /// </summary>
        /// <param name="damageCategory"></param>
        /// <param name="assetCategory"></param>
        /// <param name="impactAreaID"></param>
        /// <returns></returns>
        public ThreadsafeInlineHistogram GetConsequenceResultsHistogram(string damageCategory, string assetCategory, int impactAreaID)
        {
            return GetConsequenceResult(damageCategory, assetCategory, impactAreaID).ConsequenceHistogram;
        }
        /// <summary>
        /// This method returns the histogram (distribution) of consequences for the give category and impact area ID
        /// If categoryIsDamageCategory is true, then category is damage category, and histograms are aggregated over asset categories
        /// If categoryIsDamageCategory is false, then category is asset category, and histograms are aggregated over damage categories
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="impactAreaID"></param>
        /// <param name="categoryIsDamageCategory"></param>
        /// <returns></returns>
        public ThreadsafeInlineHistogram GetConsequenceResultsHistogram(string category, int impactAreaID, bool categoryIsDamageCategory)
        {
            List<ThreadsafeInlineHistogram> histograms = new List<ThreadsafeInlineHistogram>();
            foreach (ConsequenceResult consequenceResult in _consequenceResultList)
            {
                if (categoryIsDamageCategory)
                {
                    if((consequenceResult.DamageCategory.Equals(category)) && consequenceResult.RegionID.Equals(impactAreaID))
                    {
                        histograms.Add(consequenceResult.ConsequenceHistogram);
                    }
                } 
                else
                {
                    if ((consequenceResult.AssetCategory.Equals(category)) && consequenceResult.RegionID.Equals(impactAreaID))
                    {
                        histograms.Add(consequenceResult.ConsequenceHistogram);
                    }
                }
            }
            ThreadsafeInlineHistogram threadsafeInlineHistogram = ThreadsafeInlineHistogram.AddHistograms(histograms);

            return threadsafeInlineHistogram;
        }
        /// <summary>
        /// This method returns the histogram (distribution) of consequences for the give category
        /// If categoryIsDamageCategory is true, then category is damage category, and histograms are aggregated over asset categories and impact areas
        /// If categoryIsDamageCategory is false, then category is asset category, and histograms are aggregated over damage categories and impact areas   
        /// Note that when working with impact area scenario results, there is only 1 impact area    
        /// </summary>
        /// <param name="category"></param>
        /// <param name="categoryIsDamageCategory"></param>
        /// <returns></returns>
        public ThreadsafeInlineHistogram GetConsequenceResultsHistogram(string category, bool categoryIsDamageCategory)
        {
            List<ThreadsafeInlineHistogram> histograms = new List<ThreadsafeInlineHistogram>();

            foreach (ConsequenceResult consequenceResult in _consequenceResultList)
            {
                if (categoryIsDamageCategory)
                {
                    if ((consequenceResult.DamageCategory.Equals(category)))
                    {
                        histograms.Add(consequenceResult.ConsequenceHistogram);
                     }
                }
                else
                {
                    if ((consequenceResult.AssetCategory.Equals(category)))
                    {
                        histograms.Add(consequenceResult.ConsequenceHistogram);
                    }
                }
            }
            ThreadsafeInlineHistogram threadsafeInlineHistogram = ThreadsafeInlineHistogram.AddHistograms(histograms);
            return threadsafeInlineHistogram;
        }
        /// <summary>
        /// This method returns the histogram (distribution) of consequences for the given impact area
        /// The histograms are aggregated over all damage and asset categories 
        /// </summary>
        /// <param name="impactAreaID"></param>
        /// <returns></returns>
        public ThreadsafeInlineHistogram GetConsequenceResultsHistogram(int impactAreaID)
        {
            List<ThreadsafeInlineHistogram> histograms = new List<ThreadsafeInlineHistogram>();
            foreach (ConsequenceResult consequenceResult in _consequenceResultList)
            {
                    if (consequenceResult.RegionID.Equals(impactAreaID))
                    {
                    histograms.Add(consequenceResult.ConsequenceHistogram);
                    }
            }
            ThreadsafeInlineHistogram threadsafeInlineHistogram = ThreadsafeInlineHistogram.AddHistograms(histograms);
            return threadsafeInlineHistogram;
        }
        /// <summary>
        /// This method returns the histogram (distribution) of consequences
        /// The returned histogram has aggregated the histograms for all damage and asset categories and impact areas
        /// </summary>
        /// <returns></returns>
        public ThreadsafeInlineHistogram GetConsequenceResultsHistogram()
        {
            List<ThreadsafeInlineHistogram> histograms = new List<ThreadsafeInlineHistogram>();
            foreach (ConsequenceResult consequenceResult in _consequenceResultList)
            {
                histograms.Add(consequenceResult.ConsequenceHistogram);
            }
            ThreadsafeInlineHistogram threadsafeInlineHistogram = ThreadsafeInlineHistogram.AddHistograms(histograms);
            return threadsafeInlineHistogram;
        }

        public XElement WriteToXML()
        {
            XElement masterElem = new XElement("EAD_Results");
            foreach (ConsequenceResult damageResult in _consequenceResultList)
            {
                XElement damageResultElement = damageResult.WriteToXML();
                damageResultElement.Name = $"{damageResult.DamageCategory}-{damageResult.AssetCategory}";
                masterElem.Add(damageResultElement);
            }
            masterElem.SetAttributeValue("ImpactAreaID", _regionID);
            return masterElem;
        }

        public static ConsequenceResults ReadFromXML(XElement xElement)
        {
            List<ConsequenceResult> damageResults = new List<ConsequenceResult>();
            foreach (XElement histogramElement in xElement.Elements())
            {
                damageResults.Add(ConsequenceResult.ReadFromXML(histogramElement));
            }
            int impactAreaID = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);
            return new ConsequenceResults(damageResults,impactAreaID);
        }

        #endregion
    }
}