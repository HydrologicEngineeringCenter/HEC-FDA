using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;
using Statistics;
using Statistics.Distributions;
using HEC.MVVMFramework.Model.Messaging;

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

        public double Sample(double inventoryValue, double probability, bool computeIsDeterministic)
        {
            double sampledValue;
            if (computeIsDeterministic)
            {
                sampledValue = inventoryValue;

            } 
            else
            {
            switch (_distributionType)
            {
                case IDistributionEnum.Normal:
                    double standardDeviation = (_percentOfInventoryValueStandardDeviationOrMin/100) * inventoryValue;
                    Normal normal = new Normal(inventoryValue, standardDeviation);
                    sampledValue = normal.InverseCDF(probability);
                    break;

                case IDistributionEnum.LogNormal:
                    double logStandardDeviation = (_percentOfInventoryValueStandardDeviationOrMin/100) * inventoryValue;
                    LogNormal logNormal = new LogNormal(inventoryValue, logStandardDeviation);
                    sampledValue = logNormal.InverseCDF(probability);
                    break;
                case IDistributionEnum.Triangular:
                    double min = (_percentOfInventoryValueStandardDeviationOrMin/100) * inventoryValue;
                    double max = (_percentOfInventoryValueMax/100) * inventoryValue;
                    Triangular triangular = new Triangular(min, inventoryValue, max);
                    sampledValue = triangular.InverseCDF(probability);
                    break;
                case IDistributionEnum.Uniform:
                    double minUniform = (_percentOfInventoryValueStandardDeviationOrMin/100) * inventoryValue;
                    double maxUniform = (_percentOfInventoryValueMax/100) * inventoryValue;
                    Uniform uniform = new Uniform(minUniform, maxUniform);
                    sampledValue = uniform.InverseCDF(probability);
                    break;
                default:
                    sampledValue = inventoryValue;
                    break;
            }
            }
            //do not allow negative inventory values 
            if (sampledValue < 0)
            {
                sampledValue = 0;
            }
            return sampledValue;
        }
        #endregion

    }
}
