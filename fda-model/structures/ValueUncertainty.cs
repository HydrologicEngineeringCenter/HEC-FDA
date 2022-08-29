using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace structures
{
    public class ValueUncertainty
{
        #region Fields
        private double _percentOfInventoryValueStandardDeviationOrMin;
        private double _percentOfInventoryValueMax;
        private IDistributionEnum _distributionType;
        #endregion

        #region Constructor 
        public ValueUncertainty(IDistributionEnum distributionType, double percentOfInventoryValueStandardDeviationOrMin, double percentOfInventoryMax = 0)
        {
            _distributionType = distributionType;
            _percentOfInventoryValueStandardDeviationOrMin = percentOfInventoryValueStandardDeviationOrMin;
            _percentOfInventoryValueMax = percentOfInventoryMax;

        }
        #endregion

        #region Methods
        public double Sample(double inventoryValue, double randomProbability)
        {
            double sampledValue;
            switch(_distributionType)
            {
                case IDistributionEnum.Normal:
                    double standardDeviation = _percentOfInventoryValueStandardDeviationOrMin * inventoryValue;
                    Normal normal = new Normal(inventoryValue, standardDeviation);
                    sampledValue = normal.InverseCDF(randomProbability);
                    break;
                    //I am not sure if this math makes sense 
                    //percent of the log of inventory value?
                case IDistributionEnum.LogNormal:
                    double logStandardDeviation = _percentOfInventoryValueStandardDeviationOrMin * inventoryValue;
                    LogNormal logNormal = new LogNormal(inventoryValue, logStandardDeviation);
                    sampledValue = logNormal.InverseCDF(randomProbability);
                    break;
                case IDistributionEnum.Triangular:
                    double min = (1 - _percentOfInventoryValueStandardDeviationOrMin) * inventoryValue;
                    double max = (1 + _percentOfInventoryValueMax) * inventoryValue;
                    Triangular triangular = new Triangular(min, inventoryValue, max);
                    sampledValue = triangular.InverseCDF(randomProbability);
                    break;
                case IDistributionEnum.Uniform:
                    double minUniform = (1 - _percentOfInventoryValueStandardDeviationOrMin) * inventoryValue;
                    double maxUniform = (1 + _percentOfInventoryValueMax) * inventoryValue;
                    Uniform uniform = new Uniform(minUniform, maxUniform);
                    sampledValue = uniform.InverseCDF(randomProbability);
                    break;
                default:
                    sampledValue = inventoryValue;
                    break;
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
