using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HEC.FDA.Model.metrics
{
    public class AggregatedConsequencesByQuantile
    {

        #region Properties
        public Empirical ConsequenceDistribution { get; }
        public string DamageCategory { get; }
        public string AssetCategory { get; }
        public int RegionID { get; } = -999;
        public ConsequenceType ConsequenceType { get; }
        public RiskType RiskType { get; }
        public bool IsNull { get; }
        #endregion 

        #region Constructors
        public AggregatedConsequencesByQuantile()
        {
            DamageCategory = "unassigned";
            AssetCategory = "unassigned";
            ConsequenceType = ConsequenceType.Damage;
            RiskType = RiskType.Fail;
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
        public AggregatedConsequencesByQuantile(string damageCategory, string assetCategory, List<double> consequences, int impactAreaID, RiskType riskType = RiskType.Fail)
        {
            DamageCategory = damageCategory;
            AssetCategory = assetCategory;
            ConsequenceDistribution = Empirical.FitToSample(consequences);
            RegionID = impactAreaID;
            RiskType = riskType;

        }
        /// <summary>
        /// This constructor can accept a previously created Empirical Distribution
        /// as such can be used for both compute types
        /// </summary>
        /// <param name="damageCategory"></param>
        /// <param name="assetCategory"></param>
        /// <param name="impactAreaID"></param>
        public AggregatedConsequencesByQuantile(string damageCategory, string assetCategory, Empirical empirical, int impactAreaID, ConsequenceType consequenceType = ConsequenceType.Damage, RiskType riskType = RiskType.Fail)
        {
            DamageCategory = damageCategory;
            AssetCategory = assetCategory;
            ConsequenceType = consequenceType;
            RiskType = riskType;
            ConsequenceDistribution = empirical;
            RegionID = impactAreaID;
            IsNull = false;
        }
        #endregion

        #region Methods

        internal double ConsequenceSampleMean()
        {
            return ConsequenceDistribution.SampleMean;
        }

        internal double ConsequenceExceededWithProbabilityQ(double exceedanceProbability)
        {
            double nonExceedanceProbability = 1 - exceedanceProbability;
            double quartile = ConsequenceDistribution.InverseCDF(nonExceedanceProbability);
            return quartile;
        }
        #endregion
    }
}
