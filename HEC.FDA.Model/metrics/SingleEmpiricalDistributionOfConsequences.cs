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

namespace HEC.FDA.Model.metrics
{
    public class SingleEmpiricalDistributionOfConsequences: IReportMessage, IProgressReport
    {

        #region Fields
        private Empirical _consequenceDistribution;
        private string _damageCategory;
        private string _assetCategory;
        private int _regionID = -999;
        private bool _isNull;

        public event MessageReportedEventHandler MessageReport;
        public event ProgressReportedEventHandler ProgressReport;
        #endregion

        #region Properties
        public Empirical ConsequenceDistribution
        {
            get
            {
                return _consequenceDistribution;
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
        #endregion 

        #region Constructors
        /// <summary>
        /// This constructor builds a ThreadsafeInlineHistogram. Only use for parallel computes. 
        /// </summary>
        public SingleEmpiricalDistributionOfConsequences()
        {
            _damageCategory = "unassigned";
            _assetCategory = "unassigned";
            _regionID = 0;
            _consequenceDistribution = new Empirical();
            _isNull = true;
            MessageHub.Register(this);

        }
        /// <summary>
        /// This constructor creates a new empirical distribution based on a list of data 
        /// </summary>
        /// <param name="damageCategory"></param>
        /// <param name="assetCategory"></param>
        /// <param name="convergenceCriteria"></param>
        /// <param name="consequences"></param>
        /// <param name="impactAreaID"></param>
        public SingleEmpiricalDistributionOfConsequences(string damageCategory, string assetCategory, List<double> consequences, int impactAreaID)
        {
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _consequenceDistribution = Empirical.FitToSample(consequences);
            _regionID = impactAreaID;

        }
        /// <summary>
        /// This constructor can accept a previously created Empirical Distribution
        /// as such can be used for both compute types
        /// </summary>
        /// <param name="damageCategory"></param>
        /// <param name="assetCategory"></param>
        /// <param name="impactAreaID"></param>
        public SingleEmpiricalDistributionOfConsequences(string damageCategory, string assetCategory, Empirical empirical, int impactAreaID)
        {
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _consequenceDistribution = empirical;
            _regionID = impactAreaID;
            _isNull = false;
            MessageHub.Register(this);
        }
        #endregion

        #region Methods
        
        internal double MeanExpectedAnnualConsequences()
        {
            return _consequenceDistribution.Mean;
        }

        internal double ConsequenceExceededWithProbabilityQ(double exceedanceProbability)
        {
            double nonExceedanceProbability = 1 - exceedanceProbability;
            double quartile = _consequenceDistribution.InverseCDF(nonExceedanceProbability);
            return quartile;
        }

        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("ConsequenceResult");
            XElement histogramElement = _consequenceDistribution.ToXML();
            histogramElement.Name = "DamageDistribution";
            masterElement.Add(histogramElement);
            masterElement.SetAttributeValue("DamageCategory", _damageCategory);
            masterElement.SetAttributeValue("AssetCategory", _assetCategory);
            masterElement.SetAttributeValue("ImpactAreaID", _regionID);
            return masterElement;
        }

        public static SingleEmpiricalDistributionOfConsequences ReadFromXML(XElement xElement)
        {
            Empirical empirical = Empirical.ReadFromXML(xElement.Element("DamageDistribution"));
            string damageCategory = xElement.Attribute("DamageCategory").Value;
            string assetCategory = xElement.Attribute("AssetCategory").Value;
            int id = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);
            return new SingleEmpiricalDistributionOfConsequences(damageCategory, assetCategory, empirical, id);
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
