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
    public class AggregatedConsequencesByQuantile
    {

        #region Properties
        public Empirical ConsequenceDistribution { get; }
        public string DamageCategory { get; }
        public string AssetCategory { get; }
        public int RegionID { get; } = -999;
        public bool IsNull { get; }
        #endregion 

        #region Constructors
        public AggregatedConsequencesByQuantile()
        {
            DamageCategory = "unassigned";
            AssetCategory = "unassigned";
            RegionID = 0;
            ConsequenceDistribution = new Empirical();
            IsNull = true;

        }
        /// <summary>
        /// This constructor creates a new empirical distribution based on a list of data 
        /// </summary>
        /// <param name="damageCategory"></param>
        /// <param name="assetCategory"></param>
        /// <param name="convergenceCriteria"></param>
        /// <param name="consequences"></param>
        /// <param name="impactAreaID"></param>
        public AggregatedConsequencesByQuantile(string damageCategory, string assetCategory, List<double> consequences, int impactAreaID)
        {
            DamageCategory = damageCategory;
            AssetCategory = assetCategory;
            ConsequenceDistribution = Empirical.FitToSample(consequences);
            RegionID = impactAreaID;

        }
        /// <summary>
        /// This constructor can accept a previously created Empirical Distribution
        /// as such can be used for both compute types
        /// </summary>
        /// <param name="damageCategory"></param>
        /// <param name="assetCategory"></param>
        /// <param name="impactAreaID"></param>
        public AggregatedConsequencesByQuantile(string damageCategory, string assetCategory, Empirical empirical, int impactAreaID)
        {
            DamageCategory = damageCategory;
            AssetCategory = assetCategory;
            ConsequenceDistribution = empirical;
            RegionID = impactAreaID;
            IsNull = false;
        }
        #endregion

        #region Methods
        
        internal double MeanExpectedAnnualConsequences()
        {
            return ConsequenceDistribution.Mean;
        }

        internal double ConsequenceExceededWithProbabilityQ(double exceedanceProbability)
        {
            double nonExceedanceProbability = 1 - exceedanceProbability;
            double quartile = ConsequenceDistribution.InverseCDF(nonExceedanceProbability);
            return quartile;
        }

        public XElement WriteToXML()
        {
            XElement masterElement = new("ConsequenceResult");
            XElement histogramElement = ConsequenceDistribution.ToXML();
            histogramElement.Name = "DamageDistribution";
            masterElement.Add(histogramElement);
            masterElement.SetAttributeValue("DamageCategory", DamageCategory);
            masterElement.SetAttributeValue("AssetCategory", AssetCategory);
            masterElement.SetAttributeValue("ImpactAreaID", RegionID);
            return masterElement;
        }

        public static AggregatedConsequencesByQuantile ReadFromXML(XElement xElement)
        {
            Empirical empirical = Empirical.ReadFromXML(xElement.Element("DamageDistribution"));
            string damageCategory = xElement.Attribute("DamageCategory").Value;
            string assetCategory = xElement.Attribute("AssetCategory").Value;
            int id = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);
            return new AggregatedConsequencesByQuantile(damageCategory, assetCategory, empirical, id);
        }
        #endregion
    }
}
