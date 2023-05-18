using System.Collections.Generic;
using Statistics;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Model.Messaging;
using Statistics.Histograms;
using HEC.FDA.Model.paireddata;
using System;
using System.Linq;
using Statistics.Distributions;

namespace HEC.FDA.Model.metrics
{
    public class ConsequenceDistributionResults : ValidationErrorLogger
    {
        #region Fields
        private int _alternativeID;
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

            //create an array to collect data the side of the convergence criteria iteration count 
        }
        internal ConsequenceDistributionResults(bool isNull)
        {
            _consequenceResultList = new List<ConsequenceDistributionResult>();
            _isNull = isNull;
        }
        internal ConsequenceDistributionResults(int alternativeID)
        {
            _consequenceResultList = new List<ConsequenceDistributionResult>();
            _alternativeID = alternativeID;
            _isNull = false;
        }
        //public for testing
        public ConsequenceDistributionResults(ConvergenceCriteria convergenceCriteria)
        {
            _consequenceResultList = new List<ConsequenceDistributionResult>();
            _isNull = false;
            _ConvergenceCriteria = convergenceCriteria;
        }
        internal ConsequenceDistributionResults(List<ConsequenceDistributionResult> damageResults)
        {
            _consequenceResultList = damageResults;
            _isNull = false;
        }



        #endregion

        #region Methods 
        //This constructor is used in the simulation parallel compute and creates a threadsafe inline histogram inside consequence distribution result 
        internal void AddNewConsequenceResultObject(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, double binWidth, int impactAreaID, bool histogramIsZeroValued = false)
        {
            ConsequenceDistributionResult damageResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
            if (damageResult.IsNull)
            {
                ConsequenceDistributionResult newDamageResult = new ConsequenceDistributionResult(damageCategory, assetCategory, convergenceCriteria, binWidth, impactAreaID);
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
        internal void AddConsequenceRealization(double damageEstimate, string damageCategory, string assetCategory, int impactAreaID, long iteration, bool parallelCompute)
        {
            ConsequenceDistributionResult damageResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
            damageResult.AddConsequenceRealization(damageEstimate, iteration, parallelCompute);

        }
        internal void AddConsequenceRealization(ConsequenceResult consequenceResult, string damageCategory, int impactAreaID, int iteration)
        {
            GetConsequenceResult(damageCategory, utilities.StringConstants.STRUCTURE_ASSET_CATEGORY, impactAreaID).AddConsequenceRealization(consequenceResult.StructureDamage, iteration, true);
            GetConsequenceResult(damageCategory, utilities.StringConstants.CONTENT_ASSET_CATEGORY, impactAreaID).AddConsequenceRealization(consequenceResult.ContentDamage, iteration, true);
            GetConsequenceResult(damageCategory, utilities.StringConstants.VEHICLE_ASSET_CATEGORY, impactAreaID).AddConsequenceRealization(consequenceResult.VehicleDamage, iteration, true);
            GetConsequenceResult(damageCategory, utilities.StringConstants.OTHER_ASSET_CATEGORY, impactAreaID).AddConsequenceRealization(consequenceResult.OtherDamage, iteration, true);
        }
        public void PutDataIntoHistograms()
        {
            foreach(ConsequenceDistributionResult consequenceDistributionResult in _consequenceResultList)
            {
                consequenceDistributionResult.PutDataIntoHistogram();
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
        /// <param name="impactAreaID"></param> the default is the null value utilities.IntegerConstants.DEFAULT_MISSING_VALUE
        /// <returns></returns>The mean of consequences
        public double MeanDamage(string damageCategory = null, string assetCategory = null, int impactAreaID = utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
        {
            double consequenceValue = 0;
            foreach (ConsequenceDistributionResult consequenceResult in _consequenceResultList)
            {
                if (damageCategory == null && assetCategory == null && impactAreaID == utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                }
                if (damageCategory != null && assetCategory == null && impactAreaID == utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    if (damageCategory.Equals(consequenceResult.DamageCategory))
                    {
                        consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                    }
                }
                if (damageCategory == null && assetCategory != null && impactAreaID == utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    if (assetCategory.Equals(consequenceResult.AssetCategory))
                    {
                        consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                    }
                }
                if (damageCategory == null && assetCategory == null && impactAreaID != utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    if (impactAreaID.Equals(consequenceResult.RegionID))
                    {
                        consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                    }
                }
                if (damageCategory != null && assetCategory != null && impactAreaID == utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    if (damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory))
                    {
                        consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                    }
                }
                if (damageCategory != null && assetCategory == null && impactAreaID != utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    if (damageCategory.Equals(consequenceResult.DamageCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                    {
                        consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                    }
                }
                if (damageCategory == null && assetCategory != null && impactAreaID != utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    if (assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                    {
                        consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                    }
                }
                if (damageCategory != null && assetCategory != null && impactAreaID != utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
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
        /// <param name="impactAreaID"></param>the default is the null value utilities.IntegerConstants.DEFAULT_MISSING_VALUE
        /// <returns></returns>the level of consequences exceeded by the specified probability 
        public double ConsequenceExceededWithProbabilityQ(double exceedanceProbability, string damageCategory = null, string assetCategory = null, int impactAreaID = utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
        {
            double consequenceValue = 0;
            foreach (ConsequenceDistributionResult consequenceResult in _consequenceResultList)
            {
                if (damageCategory == null && assetCategory == null && impactAreaID == utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                }
                if (damageCategory != null && assetCategory == null && impactAreaID == utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    if (damageCategory.Equals(consequenceResult.DamageCategory))
                    {
                        consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                    }
                }
                if (damageCategory == null && assetCategory != null && impactAreaID == utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    if (assetCategory.Equals(consequenceResult.AssetCategory))
                    {
                        consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                    }
                }
                if (damageCategory == null && assetCategory == null && impactAreaID != utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    if (impactAreaID.Equals(consequenceResult.RegionID))
                    {
                        consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                    }
                }
                if (damageCategory != null && assetCategory != null && impactAreaID == utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    if (damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory))
                    {
                        consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                    }
                }
                if (damageCategory != null && assetCategory == null && impactAreaID != utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    if (damageCategory.Equals(consequenceResult.DamageCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                    {
                        consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                    }
                }
                if (damageCategory == null && assetCategory != null && impactAreaID != utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    if (assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                    {
                        consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                    }
                }
                if (damageCategory != null && assetCategory != null && impactAreaID != utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    return GetConsequenceResult(damageCategory, assetCategory, impactAreaID).ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                }
            }
            return consequenceValue;
        }
        /// <summary>
        /// This method returns a consequence result for the given damage category, asset category, and impact area 
        /// Impact area ID is used for alternative and alternative comparison reports 
        /// Impact area ID is utilities.IntegerConstants.DEFAULT_MISSING_VALUE otherwise 
        /// </summary>
        /// <param name="damageCategory"></param>
        /// <param name="assetCategory"></param>
        /// <param name="impactAreaID"></param>
        /// <returns></returns>
        public ConsequenceDistributionResult GetConsequenceResult(string damageCategory, string assetCategory, int impactAreaID = utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
        {
            //foreach (ConsequenceDistributionResult damageResult in _consequenceResultList)
            for (int i = 0; i < _consequenceResultList.Count; i++)
            {
                if (_consequenceResultList[i].RegionID.Equals(impactAreaID))
                {
                    if (_consequenceResultList[i].DamageCategory.Equals(damageCategory))
                    {
                        if (_consequenceResultList[i].AssetCategory.Equals(assetCategory))
                        {
                            return (_consequenceResultList[i]);
                        }
                    }
                }
            }
            string message = "The requested damage category - asset category - impact area combination could not be found. An arbitrary object is being returned";
            ErrorMessage errorMessage = new ErrorMessage(message, MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
            ReportMessage(this, new MessageEventArgs(errorMessage));
            ConsequenceDistributionResult dummyResult = new ConsequenceDistributionResult();
            return dummyResult;
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
        /// <summary>
        /// This method gets the histogram (distribution) of consequences for the given damage category(ies), asset category(ies), and impact area(s)
        /// The level of aggregation of the distribution of consequences is determined by the arguments used in the method
        /// For example, if you wanted a histogram for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// ThreadsafeInlineHistogram histogram = GetConsequenceResultsHistogram(damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="damageCategory"></param> The default is null 
        /// <param name="assetCategory"></param> The default is null 
        /// <param name="impactAreaID"></param> The default is a null value (utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
        /// <returns></returns> Aggregated consequences histogram 
        public Empirical GetAggregateEmpiricalDistribution(string damageCategory = null, string assetCategory = null, int impactAreaID = utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
        {
            List<Empirical> empiricalDistsToStack = new List<Empirical>();
            foreach (ConsequenceDistributionResult consequenceResult in _consequenceResultList)
            {
                if (damageCategory == null && assetCategory == null && impactAreaID == utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    empiricalDistsToStack.Add(Histogram.ConvertToEmpiricalDistribution(consequenceResult.ConsequenceHistogram));
                }
                if (damageCategory != null && assetCategory == null && impactAreaID == utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    if (damageCategory.Equals(consequenceResult.DamageCategory))
                    {
                        empiricalDistsToStack.Add(Histogram.ConvertToEmpiricalDistribution(consequenceResult.ConsequenceHistogram));
                    }
                }
                if (damageCategory == null && assetCategory != null && impactAreaID == utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    if (assetCategory.Equals(consequenceResult.AssetCategory))
                    {
                        empiricalDistsToStack.Add(Histogram.ConvertToEmpiricalDistribution(consequenceResult.ConsequenceHistogram));
                    }
                }
                if (damageCategory == null && assetCategory == null && impactAreaID != utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    if (impactAreaID.Equals(consequenceResult.RegionID))
                    {
                        empiricalDistsToStack.Add(Histogram.ConvertToEmpiricalDistribution(consequenceResult.ConsequenceHistogram));
                    }
                }
                if (damageCategory != null && assetCategory != null && impactAreaID == utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    if (damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory))
                    {
                        empiricalDistsToStack.Add(Histogram.ConvertToEmpiricalDistribution(consequenceResult.ConsequenceHistogram));
                    }
                }
                if (damageCategory != null && assetCategory == null && impactAreaID != utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    if (damageCategory.Equals(consequenceResult.DamageCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                    {
                        empiricalDistsToStack.Add(Histogram.ConvertToEmpiricalDistribution(consequenceResult.ConsequenceHistogram));
                    }
                }
                if (damageCategory == null && assetCategory != null && impactAreaID != utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    if (assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                    {
                        empiricalDistsToStack.Add(Histogram.ConvertToEmpiricalDistribution(consequenceResult.ConsequenceHistogram));
                    }
                }
                if (damageCategory != null && assetCategory != null && impactAreaID != utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
                {
                    ConsequenceDistributionResult consequence = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
                    return Histogram.ConvertToEmpiricalDistribution(consequence.ConsequenceHistogram);
                }
            }
            if (empiricalDistsToStack.Count == 0)
            {
                string message = "The requested damage category - asset category - impact area combination could not be found. An arbitrary object is being returned";
                ErrorMessage errorMessage = new ErrorMessage(message, MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
                ReportMessage(this, new MessageEventArgs(errorMessage));
                return new Empirical();
            }
            else
            {
                return Empirical.StackEmpiricalDistributions(empiricalDistsToStack, Empirical.Sum);
            }

        }

        public IHistogram GetSpecificHistogram(string damageCategory, string assetCategory, int impactAreaID)
        {
            IHistogram returnHistogram = null;
            foreach (ConsequenceDistributionResult consequenceDistributionResult in _consequenceResultList)
            {
                if (consequenceDistributionResult.DamageCategory == damageCategory)
                {
                    if (consequenceDistributionResult.AssetCategory == assetCategory)
                    {
                        if (consequenceDistributionResult.RegionID == impactAreaID)
                        {
                            returnHistogram = consequenceDistributionResult.ConsequenceHistogram;
                        }
                    }
                }
            }
            if (returnHistogram == null)
            {
                string message = "The requested damage category - asset category - impact area combination could not be found. An arbitrary object is being returned";
                ErrorMessage errorMessage = new ErrorMessage(message, MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
                ReportMessage(this, new MessageEventArgs(errorMessage));
                returnHistogram = new Histogram();
            }
            return returnHistogram;
        }

        internal long RemainingIterations(double upperProb, double lowerProb)
        {
            List<long> stageDamageIterationsRemaining = new List<long>();

                foreach (ConsequenceDistributionResult consequenceDistributionResult in ConsequenceResultList)
                {
                    if (consequenceDistributionResult.ConsequenceHistogram.HistogramIsZeroValued)
                    {
                        stageDamageIterationsRemaining.Add(0);
                    }
                    else
                    {
                        stageDamageIterationsRemaining.Add(consequenceDistributionResult.ConsequenceHistogram.EstimateIterationsRemaining(upperProb, lowerProb));
                    }
                }
            return stageDamageIterationsRemaining.Max();
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
        public static List<UncertainPairedData> ToUncertainPairedData(List<double> xValues, List<ConsequenceDistributionResults> yValues, int impactAreaID)
        {
            List<UncertainPairedData> uncertainPairedDataList = new List<UncertainPairedData>();
            List<string> damageCategories = yValues[yValues.Count - 1].GetDamageCategories();
            List<string> assetCategories = yValues[yValues.Count - 1].GetAssetCategories();
   

                foreach (string damageCategory in damageCategories)
                {
                    foreach (string assetCategory in assetCategories)
                    {
                        CurveMetaData curveMetaData = new CurveMetaData("X Values", "Consequences", "Consequences Uncertain Paired Data", damageCategory, CurveTypesEnum.MonotonicallyIncreasing, impactAreaID, assetCategory);
                        List<IHistogram> histograms = new List<IHistogram>();
                        foreach (ConsequenceDistributionResults consequenceDistributions in yValues)
                        {
                            IHistogram histogram = consequenceDistributions.GetSpecificHistogram(damageCategory, assetCategory, impactAreaID);
                            histograms.Add(histogram);
                        }
                        UncertainPairedData uncertainPairedData = new UncertainPairedData(xValues.ToArray(), histograms.ToArray(), curveMetaData);
                        uncertainPairedDataList.Add(uncertainPairedData);
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