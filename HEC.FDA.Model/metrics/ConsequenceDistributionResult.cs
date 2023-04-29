using System;
using Statistics.Histograms;
using Statistics;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using System.Collections.Generic;
using System.Linq;
using Statistics.Distributions;
using System.Collections.Concurrent;

namespace HEC.FDA.Model.metrics
{ //TODO: I THINK SOME OR ALL OF THIS CLASS SHOULD BE INTERNAL 
    public class ConsequenceDistributionResult : IReportMessage, IProgressReport
    {
        #region Fields
        private ConcurrentBag<double> _tempResults = new ConcurrentBag<double>();
        private IHistogram _consequenceHistogram;
        private string _damageCategory;
        private string _assetCategory;
        private int _regionID = utilities.IntegerConstants.DEFAULT_MISSING_VALUE;
        private ConvergenceCriteria _convergenceCriteria;
        private bool _isNull;
        public event MessageReportedEventHandler MessageReport;
        public event ProgressReportedEventHandler ProgressReport;
        #endregion

        #region Properties
        public IHistogram ConsequenceHistogram
        {
            get
            {
                return _consequenceHistogram;
            }
        }
        public string DamageCategory
        {
            get
            {
                return _damageCategory;
            }
        }
        public string AssetCategory
        {
            get
            {
                return _assetCategory;
            }
        }
        public int RegionID
        {
            get
            {
                return _regionID;
            }
        }
        public bool IsNull
        {
            get
            {
                return _isNull;
            }
        }
        public ConvergenceCriteria ConvergenceCriteria
        {
            get
            {
                return _convergenceCriteria;
            }
        }
        #endregion 

        #region Constructors
        /// <summary>
        /// This constructor builds a ThreadsafeInlineHistogram. Only use for parallel computes. 
        /// </summary>
        public ConsequenceDistributionResult()
        {
            _damageCategory = "unassigned";
            _assetCategory = "unassigned";
            _regionID = 0;
            _convergenceCriteria = new ConvergenceCriteria();
            _consequenceHistogram = new ThreadsafeInlineHistogram();
            _isNull = true;
            MessageHub.Register(this);

        }
        /// <summary>
        /// This constructor builds a ThreadsafeInlineHistogram. Only use for parallel computes. 
        /// This constructor is used only for simulation compute and does not track impact area ID
        /// </summary>
        public ConsequenceDistributionResult(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, int impactAreaID)
        {
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _convergenceCriteria = convergenceCriteria;
            _consequenceHistogram = new ThreadsafeInlineHistogram(_convergenceCriteria);
            _isNull = false;
            _regionID = impactAreaID;
            MessageHub.Register(this);

        }
        public ConsequenceDistributionResult(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, List<double> consequences, int impactAreaID)
        {
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _convergenceCriteria = convergenceCriteria;
            _consequenceHistogram = new Histogram(consequences, convergenceCriteria);
            _regionID = impactAreaID;

        }
        /// <summary>
        /// This constructor can accept wither a Histogram or a ThreadsageInlineHistogram
        /// as such can be used for both compute types
        /// </summary>
        /// <param name="damageCategory"></param>
        /// <param name="assetCategory"></param>
        /// <param name="histogram"></param>
        /// <param name="impactAreaID"></param>
        public ConsequenceDistributionResult(string damageCategory, string assetCategory, IHistogram histogram, int impactAreaID)
        {
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _consequenceHistogram = histogram;
            _convergenceCriteria = _consequenceHistogram.ConvergenceCriteria;
            _regionID = impactAreaID;
            _isNull = false;
            MessageHub.Register(this);
        }
        #endregion

        #region Methods
        //For the EAD compute, we need to put data in concurrent bag into a histogram 
        public void PutDataIntoHistogram()
        {
            List<double> listToSort = new List<double>();
            listToSort = _tempResults.ToList();
            listToSort.Sort();
            int j = 0;
            foreach(double item in listToSort) { 
                _consequenceHistogram.AddObservationToHistogram(item, j);
                j++;
            }
            _tempResults.Clear();
        }

        //If computing stage damage, we'll plop the realization directly into the histogram
        internal void AddConsequenceRealization(double damageRealization)
        {
            if (_consequenceHistogram is ThreadsafeInlineHistogram)
            {
                _tempResults.Add(damageRealization);
            } else
            {
                _consequenceHistogram.AddObservationToHistogram(damageRealization, _consequenceHistogram.SampleSize + 1); ;
            }
        }

        internal double MeanExpectedAnnualConsequences()
        {
            return _consequenceHistogram.Mean;
        }

        internal double ConsequenceExceededWithProbabilityQ(double exceedanceProbability)
        {
            double nonExceedanceProbability = 1 - exceedanceProbability;
            double quartile = _consequenceHistogram.InverseCDF(nonExceedanceProbability);
            return quartile;
        }

        public bool Equals(ConsequenceDistributionResult damageResult)
        {
            bool histogramsMatch = _consequenceHistogram.Equals(damageResult.ConsequenceHistogram);
            if (!histogramsMatch)
            {
                return false;
            }
            return true;
        }
        public static SingleEmpiricalDistributionOfConsequences ConvertToSingleEmpiricalDistributionOfConsequences(ConsequenceDistributionResult consequenceDistributionResult)
        {
            Empirical empirical = Histogram.ConvertToEmpiricalDistribution(consequenceDistributionResult.ConsequenceHistogram);
            return new SingleEmpiricalDistributionOfConsequences(consequenceDistributionResult.DamageCategory, consequenceDistributionResult.AssetCategory, empirical, consequenceDistributionResult.RegionID);
        }
        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("ConsequenceResult");
            masterElement.SetAttributeValue("Type", _consequenceHistogram.TypeOfIHistogram);
            XElement histogramElement = _consequenceHistogram.ToXML();
            histogramElement.Name = "DamageHistogram";
            masterElement.Add(histogramElement);
            masterElement.SetAttributeValue("DamageCategory", _damageCategory);
            masterElement.SetAttributeValue("AssetCategory", _assetCategory);
            masterElement.SetAttributeValue("ImpactAreaID", _regionID);
            return masterElement;
        }

        public static ConsequenceDistributionResult ReadFromXML(XElement xElement)
        {
            string type = xElement.Attribute("Type").Value;
            IHistogram damageHistogram;
            if (type.Equals("Histogram"))
            {
                damageHistogram = Histogram.ReadFromXML(xElement.Element("DamageHistogram"));
            }
            else
            {
                damageHistogram = ThreadsafeInlineHistogram.ReadFromXML(xElement.Element("DamageHistogram"));

            }
            string damageCategory = xElement.Attribute("DamageCategory").Value;
            string assetCategory = xElement.Attribute("AssetCategory").Value;
            int id = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);
            return new ConsequenceDistributionResult(damageCategory, assetCategory, damageHistogram, id);
        }

        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }

        public void ReportProgress(object sender, ProgressReportEventArgs e)
        {
            ProgressReport?.Invoke(sender, e);
        }
        #endregion
    }
}
