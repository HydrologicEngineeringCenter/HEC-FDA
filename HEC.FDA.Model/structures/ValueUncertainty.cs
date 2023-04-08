using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;
using Statistics;
using Statistics.Distributions;
using HEC.MVVMFramework.Model.Messaging;
using System;

namespace HEC.FDA.Model.structures
{
    public class ValueUncertainty : ValidationErrorLogger
    {
        #region Fields
        private double _percentOfInventoryValueStandardDeviationOrMin;
        private double _percentOfInventoryValueMax;
        private IDistributionEnum _distributionType;
        #endregion

        #region Properties 
        public IDistributionEnum DistributionType { get { return _distributionType; } }
        #endregion 
        #region Constructor 
        public ValueUncertainty()
        {
            _percentOfInventoryValueMax = 100;
            _percentOfInventoryValueStandardDeviationOrMin = 0;
            _distributionType = IDistributionEnum.Deterministic;
            AddRules();
        }
        public ValueUncertainty(IDistributionEnum distributionType, double percentOfInventoryValueStandardDeviationOrMin, double percentOfInventoryMax = 100)
        {
            _distributionType = distributionType;
            _percentOfInventoryValueStandardDeviationOrMin = percentOfInventoryValueStandardDeviationOrMin;
            _percentOfInventoryValueMax = percentOfInventoryMax;
            AddRules();
        }
        #endregion

        #region Methods
        private void AddRules()
        {
            AddSinglePropertyRule(nameof(_distributionType), new Rule(() => _distributionType.Equals(IDistributionEnum.Normal) || _distributionType.Equals(IDistributionEnum.Uniform) || _distributionType.Equals(IDistributionEnum.Deterministic) || _distributionType.Equals(IDistributionEnum.Triangular), "Only Deterministic, Normal, Triangular, and Uniform distributions can be used for value uncertainty", ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(_percentOfInventoryValueStandardDeviationOrMin), new Rule(() => _percentOfInventoryValueStandardDeviationOrMin >= 0, "The percent of inventory value must be greaeter than or equal to zero.", ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(_percentOfInventoryValueMax), new Rule(() => _percentOfInventoryValueMax >= 100, "The max percent of the inventory value must be greater than or equal to 100", ErrorLevel.Fatal));
        }

        /// <summary>
        /// The use of this method will depend on the type of distribution. 
        /// If Normal, Triangular, or Uniform, the value returned is the percent of inventory value to add or subtract from the inventoried value
        /// If log normal, then the return value will need to be multiplied by the inventoried value
        /// </summary>
        /// <param name="probability"></param>
        /// <param name="computeIsDeterministic"></param>
        /// <returns></returns>        
        public double Sample(double probability, bool computeIsDeterministic)
        {
            double centerOfDistribution = 0;
            double sampledValueOffset;
            if (computeIsDeterministic)
            {
                if(_distributionType == IDistributionEnum.LogNormal)
                {
                    sampledValueOffset = 1;
                } else
                {
                    sampledValueOffset = centerOfDistribution;

                }
            } 
            else
            {
            switch (_distributionType)
            {
                case IDistributionEnum.Normal:
                    Normal normal = new Normal(centerOfDistribution, (_percentOfInventoryValueStandardDeviationOrMin/100));
                    sampledValueOffset = normal.InverseCDF(probability);
                    break;

                case IDistributionEnum.LogNormal:
                    sampledValueOffset = Math.Exp(Normal.StandardNormalInverseCDF(probability)* (_percentOfInventoryValueStandardDeviationOrMin/100));
                    break;
                case IDistributionEnum.Triangular:
                    Triangular triangular = new Triangular(_percentOfInventoryValueStandardDeviationOrMin/100, 1, _percentOfInventoryValueMax/100);
                    sampledValueOffset = triangular.InverseCDF(probability);
                    break;
                case IDistributionEnum.Uniform:
                    Uniform uniform = new Uniform(_percentOfInventoryValueStandardDeviationOrMin/100, _percentOfInventoryValueMax/100);
                    sampledValueOffset = uniform.InverseCDF(probability);
                    break;
                default:
                    sampledValueOffset = centerOfDistribution;
                    break;
            }
            }
            return sampledValueOffset;
        }
        #endregion

    }
}
