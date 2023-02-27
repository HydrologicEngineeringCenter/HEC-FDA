using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HEC.FDA.Model.metrics
{
    public class ManyEmpiricalDistributionsOfConsequences : Validation, IReportMessage
    {
        #region Fields 
        private int _alternativeID;
        private ConvergenceCriteria _ConvergenceCriteria;
        private List<SingleEmpiricalDistributionOfConsequences> _ConsequenceResultList;
        private bool _isNull;

        #endregion

        #region Properties 
        public List<SingleEmpiricalDistributionOfConsequences> ConsequenceResultList
        {
            get
            { 
                return _ConsequenceResultList; 
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
        internal int AlternativeID
        {
            get
            {
                return _alternativeID;
            }
        }



        #endregion

        #region Constructors
        public ManyEmpiricalDistributionsOfConsequences()
        {
            _ConsequenceResultList = new List<SingleEmpiricalDistributionOfConsequences>();
            SingleEmpiricalDistributionOfConsequences dummyConsequenceDistributionResult = new SingleEmpiricalDistributionOfConsequences();
            _ConsequenceResultList.Add(dummyConsequenceDistributionResult);
            _isNull = true;
            MessageHub.Register(this);
        }
        internal ManyEmpiricalDistributionsOfConsequences(bool isNull)
        {
            _ConsequenceResultList = new List<SingleEmpiricalDistributionOfConsequences>();
            _isNull = isNull;
            MessageHub.Register(this);
        }
        internal ManyEmpiricalDistributionsOfConsequences(int alternativeID)
        {
            _ConsequenceResultList = new List<SingleEmpiricalDistributionOfConsequences>();
            _alternativeID = alternativeID;
            _isNull = false;
            MessageHub.Register(this);
        }
        //public for testing
        internal ManyEmpiricalDistributionsOfConsequences(List<SingleEmpiricalDistributionOfConsequences> damageResults)
        {
            _ConsequenceResultList = damageResults;
            _isNull = false;
            MessageHub.Register(this);
        }



        #endregion

        #region Methods 
        //This constructor is used in the simulation parallel compute and creates a threadsafe inline histogram inside consequence distribution result 
        internal void AddNewConsequenceResultObject(string damageCategory, string assetCategory, List<double> consequences, int impactAreaID)
        {
            SingleEmpiricalDistributionOfConsequences damageResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
            if (damageResult.IsNull)
            {
                SingleEmpiricalDistributionOfConsequences newDamageResult = new SingleEmpiricalDistributionOfConsequences(damageCategory, assetCategory, consequences, impactAreaID);
                _ConsequenceResultList.Add(newDamageResult);
            }
        }
        //public for testing purposes
        public void AddExistingConsequenceResultObject(SingleEmpiricalDistributionOfConsequences consequenceResultToAdd)
        {
            SingleEmpiricalDistributionOfConsequences consequenceResult = GetConsequenceResult(consequenceResultToAdd.DamageCategory, consequenceResultToAdd.AssetCategory, consequenceResultToAdd.RegionID);
            if (consequenceResult.IsNull)
            {
                _ConsequenceResultList.Add(consequenceResultToAdd);
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
            foreach (SingleEmpiricalDistributionOfConsequences consequenceResult in _ConsequenceResultList)
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
            foreach (SingleEmpiricalDistributionOfConsequences consequenceResult in _ConsequenceResultList)
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
        /// Impact area ID is used for alternative and alternative comparison reports 
        /// Impact area ID is -999 otherwise 
        /// </summary>
        /// <param name="damageCategory"></param>
        /// <param name="assetCategory"></param>
        /// <param name="impactAreaID"></param>
        /// <returns></returns>
        public SingleEmpiricalDistributionOfConsequences GetConsequenceResult(string damageCategory, string assetCategory, int impactAreaID = -999)
        {
            for (int i = 0; i < _ConsequenceResultList.Count; i++)
            {
                if (_ConsequenceResultList[i].RegionID.Equals(impactAreaID))
                {
                    if (_ConsequenceResultList[i].DamageCategory.Equals(damageCategory))
                    {
                        if (_ConsequenceResultList[i].AssetCategory.Equals(assetCategory))
                        {
                            return (_ConsequenceResultList[i]);
                        }
                    }
                }
            }
            string message = "The requested damage category - asset category - impact area combination could not be found. An arbitrary object is being returned";
            ErrorMessage errorMessage = new ErrorMessage(message, MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
            ReportMessage(this, new MessageEventArgs(errorMessage));
            SingleEmpiricalDistributionOfConsequences singleEmpiricalDistributionOfConsequences = new SingleEmpiricalDistributionOfConsequences();
            return singleEmpiricalDistributionOfConsequences;
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
        public Empirical GetAggregateEmpiricalDistribution(string damageCategory = null, string assetCategory = null, int impactAreaID = -999)
        {
            List<Empirical> empiricalDistsToStack = new List<Empirical>();
            foreach (SingleEmpiricalDistributionOfConsequences consequenceResult in _ConsequenceResultList)
            {
                if (damageCategory == null && assetCategory == null && impactAreaID == -999)
                {
                    empiricalDistsToStack.Add(consequenceResult.ConsequenceDistribution);
                }
                if (damageCategory != null && assetCategory == null && impactAreaID == -999)
                {
                    if (damageCategory.Equals(consequenceResult.DamageCategory))
                    {
                        empiricalDistsToStack.Add(consequenceResult.ConsequenceDistribution);
                    }
                }
                if (damageCategory == null && assetCategory != null && impactAreaID == -999)
                {
                    if (assetCategory.Equals(consequenceResult.AssetCategory))
                    {
                        empiricalDistsToStack.Add(consequenceResult.ConsequenceDistribution);
                    }
                }
                if (damageCategory == null && assetCategory == null && impactAreaID != -999)
                {
                    if (impactAreaID.Equals(consequenceResult.RegionID))
                    {
                        empiricalDistsToStack.Add(consequenceResult.ConsequenceDistribution);
                    }
                }
                if (damageCategory != null && assetCategory != null && impactAreaID == -999)
                {
                    if (damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory))
                    {
                        empiricalDistsToStack.Add(consequenceResult.ConsequenceDistribution);
                    }
                }
                if (damageCategory != null && assetCategory == null && impactAreaID != -999)
                {
                    if (damageCategory.Equals(consequenceResult.DamageCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                    {
                        empiricalDistsToStack.Add(consequenceResult.ConsequenceDistribution);
                    }
                }
                if (damageCategory == null && assetCategory != null && impactAreaID != -999)
                {
                    if (assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                    {
                        empiricalDistsToStack.Add(consequenceResult.ConsequenceDistribution);
                    }
                }
                if (damageCategory != null && assetCategory != null && impactAreaID != -999)
                {
                    SingleEmpiricalDistributionOfConsequences consequence = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
                    return consequence.ConsequenceDistribution;
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





        private List<string> GetAssetCategories()
        {
            List<string> assetCategories = new List<string>();
            foreach (SingleEmpiricalDistributionOfConsequences damageResult in _ConsequenceResultList)
            {
                if (!assetCategories.Contains(damageResult.AssetCategory))
                {
                    assetCategories.Add(damageResult.AssetCategory);
                }
            }
            return assetCategories;
        }

        private List<string> GetDamageCategories()
        {
            List<string> damageCategories = new List<string>();
            foreach (SingleEmpiricalDistributionOfConsequences damageResult in _ConsequenceResultList)
            {
                if (!damageCategories.Contains(damageResult.DamageCategory))
                {
                    damageCategories.Add(damageResult.DamageCategory);
                }
            }
            return damageCategories;
        }

        public XElement WriteToXML()
        {
            XElement masterElem = new XElement("EAD_Results");
            foreach (SingleEmpiricalDistributionOfConsequences damageResult in _ConsequenceResultList)
            {
                XElement damageResultElement = damageResult.WriteToXML();
                damageResultElement.Name = $"{damageResult.DamageCategory}-{damageResult.AssetCategory}";
                masterElem.Add(damageResultElement);
            }
            return masterElem;
        }

        public static ManyEmpiricalDistributionsOfConsequences ReadFromXML(XElement xElement)
        {
            List<SingleEmpiricalDistributionOfConsequences> damageResults = new List<SingleEmpiricalDistributionOfConsequences>();
            foreach (XElement histogramElement in xElement.Elements())
            {
                damageResults.Add(SingleEmpiricalDistributionOfConsequences.ReadFromXML(histogramElement));
            }
            return new ManyEmpiricalDistributionsOfConsequences(damageResults);
        }
        #endregion

    }
}
