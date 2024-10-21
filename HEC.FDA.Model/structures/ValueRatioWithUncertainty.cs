using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;
using Statistics;
using Statistics.Distributions;
using System;

namespace HEC.FDA.Model.structures
{
    public class ValueRatioWithUncertainty : Validation
    {
        #region Fields
        private readonly double _StandardDeviationOrMin;
        private readonly double _CentralTendency;
        private readonly double _Max;
        private readonly IDistributionEnum _DistributionType;


        private const string DistributionErrorMessage = "Only Deterministic, Normal, Triangular, and Uniform distributions can be used for value ratio uncertainty";
        private const string ValueRatioParameterErrorMessage = "Value ratio parameter values must be non-negative";
        private const string MaxMinErrorMessage = "The max must be larger than the minimum";
        private const string MaxCentralTendencyErrorMessage = "The max must be larger than the central tendency";
        private const string MinCentralTendencyErrorMessage = "The min must be less than the central tendency";
        #endregion

        #region Constructor 
        public ValueRatioWithUncertainty()
        {
            _StandardDeviationOrMin = 0;
            _CentralTendency = 0;
            _Max = double.MaxValue;
            _DistributionType = IDistributionEnum.Deterministic;
            AddRules();
        }

        public ValueRatioWithUncertainty(IDistributionEnum distributionEnum, double standardDeviationOrMin, double centralTendency, double max = double.MaxValue)
        {
            _DistributionType = distributionEnum;
            _StandardDeviationOrMin = standardDeviationOrMin;
            _CentralTendency = centralTendency;
            _Max = max;
            AddRules();
        }

        public ValueRatioWithUncertainty(double deterministicValueRatio)
        {
            _CentralTendency = deterministicValueRatio;
            _StandardDeviationOrMin = 0;
            _Max = 0;
            _DistributionType = IDistributionEnum.Deterministic;
            AddRules();
        }
        #endregion

        #region Methods
        private void AddRules()
        {
            AddSinglePropertyRule(nameof(_StandardDeviationOrMin), new Rule(() => _StandardDeviationOrMin >= 0 && _Max >= 0 && _CentralTendency >= 0, ValueRatioParameterErrorMessage, ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(_Max), new Rule(() => _Max >= _StandardDeviationOrMin, MaxMinErrorMessage, ErrorLevel.Fatal));
            Func<bool> validateMaxLargerThanCenter = MaxGreaterThanCentral;
            AddSinglePropertyRule(nameof(_Max), new Rule(validateMaxLargerThanCenter, MaxCentralTendencyErrorMessage, ErrorLevel.Fatal));
            Func<bool> validateMinLessThanCentral = MinLessThanCentral;
            AddSinglePropertyRule(nameof(_StandardDeviationOrMin), new Rule(validateMinLessThanCentral, MinCentralTendencyErrorMessage, ErrorLevel.Fatal));
        }

        private bool MaxGreaterThanCentral()
        {
            if (_DistributionType.Equals(IDistributionEnum.Deterministic))
            {
                return true; //because this doesn't matter for deterministic
            }
            return _Max >= _CentralTendency;
        }
        private bool MinLessThanCentral()
        {
            if (_DistributionType.Equals(IDistributionEnum.Triangular))
            {
                return _StandardDeviationOrMin <= _CentralTendency;
            }
            else
            {
                return true; //because this doesn't matter for deterministic or normal
            }
        }

            public double Sample(double probability, bool computeIsDeterministic)
        {
            double sampledValueRatio;
            if (computeIsDeterministic)
            {
                switch (_DistributionType)
                {
                    case IDistributionEnum.LogNormal:
                        LogNormal logNormal = new(_CentralTendency, _StandardDeviationOrMin);
                        Deterministic deterministicLogNormal = UncertainToDeterministicDistributionConverter.ConvertDistributionToDeterministic(logNormal);
                        sampledValueRatio = deterministicLogNormal.InverseCDF(probability);
                        break;
                    case IDistributionEnum.Uniform:
                        Uniform uniform = new(_StandardDeviationOrMin, _Max);
                        Deterministic deterministicUniform = UncertainToDeterministicDistributionConverter.ConvertDistributionToDeterministic(uniform);
                        sampledValueRatio = deterministicUniform.InverseCDF(probability);
                        break;
                    default:
                        sampledValueRatio = _CentralTendency;
                        break;
                }

            }
            else
            {
                switch (_DistributionType)
                {
                    case IDistributionEnum.Normal:
                        Normal normal = new(_CentralTendency, _StandardDeviationOrMin);
                        sampledValueRatio = normal.InverseCDF(probability);
                        break;
                    case IDistributionEnum.LogNormal:
                        LogNormal logNormal = new(_CentralTendency, _StandardDeviationOrMin);
                        sampledValueRatio = logNormal.InverseCDF(probability);
                        break;
                    case IDistributionEnum.Triangular:
                        Triangular triangular = new(_StandardDeviationOrMin, _CentralTendency, _Max);
                        sampledValueRatio = triangular.InverseCDF(probability);
                        break;
                    case IDistributionEnum.Uniform:
                        Uniform uniform = new(_StandardDeviationOrMin, _Max);
                        sampledValueRatio = uniform.InverseCDF(probability);
                        break;
                    default:
                        sampledValueRatio = _CentralTendency;
                        break;
                }
            }

            //do not allow for negative value ratios
            if (sampledValueRatio < 0)
            {
                sampledValueRatio = 0;
            }

            return sampledValueRatio;
        }
        #endregion
    }
}
