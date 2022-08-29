using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Statistics;
using Statistics.Distributions;

namespace structures
{
    public class FoundationHeightUncertainty
{
        #region Fields
        private double _feetBelowInventoryValue;
        private double _feetAboveInventoryValue;
        private double _standardDeviationFromInventoryValue;
        private IDistributionEnum _distributionType;
        #endregion

        #region Contructor 
        public FoundationHeightUncertainty(IDistributionEnum distributionEnum, double standardDeviationOrMinimum, double maximum = 0)
        {
            _distributionType = distributionEnum;
            if(_distributionType == IDistributionEnum.Normal || _distributionType == IDistributionEnum.LogNormal)
            {
                _standardDeviationFromInventoryValue = standardDeviationOrMinimum;
            }
            if(_distributionType == IDistributionEnum.Triangular || _distributionType == IDistributionEnum.Uniform)
            {
                _feetBelowInventoryValue = standardDeviationOrMinimum;
                _feetAboveInventoryValue = maximum;
            }
        }

        public double SampleFoundationHeight(double inventoriedFoundationHeight, double randomProbability)
        {
            double sampledFoundationHeight;
            switch(_distributionType)
            {
                case IDistributionEnum.Normal:
                    Normal normal = new Normal(inventoriedFoundationHeight, _standardDeviationFromInventoryValue);
                    sampledFoundationHeight = normal.InverseCDF(randomProbability);
                    break;
                case IDistributionEnum.LogNormal:
                    LogNormal logNormal = new LogNormal(inventoriedFoundationHeight, _standardDeviationFromInventoryValue);
                    sampledFoundationHeight = logNormal.InverseCDF(randomProbability);
                    break ;
                case IDistributionEnum.Triangular:
                    Triangular triangular = new Triangular(inventoriedFoundationHeight - _feetBelowInventoryValue, inventoriedFoundationHeight, inventoriedFoundationHeight + _feetAboveInventoryValue);
                    sampledFoundationHeight = triangular.InverseCDF(randomProbability);
                    break;
                case IDistributionEnum.Uniform: 
                    Uniform uniform = new Uniform(inventoriedFoundationHeight - _feetBelowInventoryValue, inventoriedFoundationHeight + _feetAboveInventoryValue);
                    sampledFoundationHeight = uniform.InverseCDF(randomProbability);
                    break;
                default:
                    sampledFoundationHeight = inventoriedFoundationHeight;
                    break;
            }
            //do not allow for negative foundation heights 
            if (sampledFoundationHeight < 0)
            {
                sampledFoundationHeight = 0;
            }
            return sampledFoundationHeight;
        }
        #endregion
    }
}
