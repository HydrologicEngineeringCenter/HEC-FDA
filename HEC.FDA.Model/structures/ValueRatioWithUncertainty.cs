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
        private double[] _RandomNumbers;


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
        internal void GenerateRandomNumbers(int seed, long size)
        {
            Random random = new Random(seed);
            double[] randos = new double[size];
            for (int i = 0; i < size; i++)
            {
                randos[i] = random.NextDouble();
            }
            _RandomNumbers = randos;
        }
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

        public double Sample(double probability)
        {
            double sampledValueRatio;
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

            //do not allow for negative value ratios
            if (sampledValueRatio < 0)
            {
                sampledValueRatio = 0;
            }

            return sampledValueRatio;
        }
        /// <summary>
        /// All sampling methods include a computeIsDeterministic argument that bypasses the iteration number for the retrieval of the deterministic representation of the variable 
        /// </summary>
        /// <param name="iteration"></param> If this method is called during a full compute with uncertainty, random numbers need to have been previously generated, and the correct random number will be pulled for the given iteration number
        /// <param name="computeIsDeterministic"></param> If the method is instead called during a deterministic compute, the iteration number will be bypassed and the deterministic representation will be returned
        /// <returns></returns> the sampled value ratio is returned as a double.
        public double Sample(long iteration, bool computeIsDeterministic)
        {
            double sampledValueRatio;
            if (computeIsDeterministic)
            {
                switch (_DistributionType)
                {
                    case IDistributionEnum.LogNormal:
                        LogNormal logNormal = new(_CentralTendency, _StandardDeviationOrMin);
                        Deterministic deterministicLogNormal = UncertainToDeterministicDistributionConverter.ConvertDistributionToDeterministic(logNormal);
                        sampledValueRatio = deterministicLogNormal.InverseCDF(0.5);
                        break;
                    case IDistributionEnum.Uniform:
                        Uniform uniform = new(_StandardDeviationOrMin, _Max);
                        Deterministic deterministicUniform = UncertainToDeterministicDistributionConverter.ConvertDistributionToDeterministic(uniform);
                        sampledValueRatio = deterministicUniform.InverseCDF(.5);
                        break;
                    default:
                        sampledValueRatio = _CentralTendency;
                        break;
                }

            }
            else
            {
                if (_RandomNumbers.Length == 0)
                {
                    throw new Exception("Random numbers by iteration have not yet been generated for this compute but the software attempted to sample value uncertainty by iteration.");
                }
                if (iteration < 0 || iteration >= _RandomNumbers.Length)
                {
                    throw new Exception("The iteration at which value uncertainty was attempted to be sampled is beyond the quantity of random numbers sampled. There is a significant conflict between the stage-damage convergence criteria and the quantity of iterations being computed.");
                }
                switch (_DistributionType)
                {
                    case IDistributionEnum.Normal:
                        Normal normal = new(_CentralTendency, _StandardDeviationOrMin);
                        sampledValueRatio = normal.InverseCDF(_RandomNumbers[iteration]);
                        break;
                    case IDistributionEnum.LogNormal:
                        LogNormal logNormal = new(_CentralTendency, _StandardDeviationOrMin);
                        sampledValueRatio = logNormal.InverseCDF(_RandomNumbers[iteration]);
                        break;
                    case IDistributionEnum.Triangular:
                        Triangular triangular = new(_StandardDeviationOrMin, _CentralTendency, _Max);
                        sampledValueRatio = triangular.InverseCDF(_RandomNumbers[iteration]);
                        break;
                    case IDistributionEnum.Uniform:
                        Uniform uniform = new(_StandardDeviationOrMin, _Max);
                        sampledValueRatio = uniform.InverseCDF(_RandomNumbers[iteration]);
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
