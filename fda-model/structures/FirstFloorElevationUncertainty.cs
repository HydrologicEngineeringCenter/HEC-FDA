using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Statistics;
using Statistics.Distributions;

namespace structures
{
    public class FirstFloorElevationUncertainty
{
        #region Fields
        private double _feetAboveInventoryValue;
        private double _standardDeviationFromOrFeetBelowInventoryValue;
        private IDistributionEnum _distributionType;
        #endregion

        #region Contructor 
        public FirstFloorElevationUncertainty(IDistributionEnum distributionEnum, double standardDeviationOrMinimum, double maximum = 0)
        {
            _distributionType = distributionEnum;
            _standardDeviationFromOrFeetBelowInventoryValue = standardDeviationOrMinimum;
            _feetAboveInventoryValue = maximum;
        }

        public double Sample(double inventoriedFirstFloorElevation, double randomProbability)
        {
            double sampledFirstFloorElevation;
            switch(_distributionType)
            {
                case IDistributionEnum.Normal:
                    Normal normal = new Normal(inventoriedFirstFloorElevation, _standardDeviationFromOrFeetBelowInventoryValue);
                    sampledFirstFloorElevation = normal.InverseCDF(randomProbability);
                    break;
                case IDistributionEnum.LogNormal:
                    LogNormal logNormal = new LogNormal(inventoriedFirstFloorElevation, _standardDeviationFromOrFeetBelowInventoryValue);
                    sampledFirstFloorElevation = logNormal.InverseCDF(randomProbability);
                    break ;
                case IDistributionEnum.Triangular:
                    Triangular triangular = new Triangular(inventoriedFirstFloorElevation - _standardDeviationFromOrFeetBelowInventoryValue, inventoriedFirstFloorElevation, inventoriedFirstFloorElevation + _feetAboveInventoryValue);
                    sampledFirstFloorElevation = triangular.InverseCDF(randomProbability);
                    break;
                case IDistributionEnum.Uniform: 
                    Uniform uniform = new Uniform(inventoriedFirstFloorElevation - _standardDeviationFromOrFeetBelowInventoryValue, inventoriedFirstFloorElevation + _feetAboveInventoryValue);
                    sampledFirstFloorElevation = uniform.InverseCDF(randomProbability);
                    break;
                default:
                    sampledFirstFloorElevation = inventoriedFirstFloorElevation;
                    break;
            }
            return sampledFirstFloorElevation;
        }
        #endregion
    }
}
