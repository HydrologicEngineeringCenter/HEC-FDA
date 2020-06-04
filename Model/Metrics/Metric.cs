using System;
using Functions;
using Functions.Ordinates;
using Model.Functions;

using Utilities;

namespace Model.Metrics
{
    internal class Metric: IMetric
    {
        #region Properties
        public MetricEnum Type { get; }
        public double ExceedanceTarget { get; }  
        public IParameterEnum TargetFunction { get; }
        #endregion

        #region Constructors
        public Metric (MetricEnum type, double exceedanceTarget = 0)
        {
            if (type == MetricEnum.NotSet) throw new ArgumentException("The desired type of metric must be set.");
            if (type != MetricEnum.ExpectedAnnualDamage && exceedanceTarget.IsFinite()) throw new ArgumentOutOfRangeException("A computable target value must be provided.");
            Type = type;
            ExceedanceTarget = exceedanceTarget;
            TargetFunction = SetTargetFunction(type);
        }
        #endregion

        #region Methods
        private IParameterEnum SetTargetFunction(MetricEnum type)
        {
            switch (type)
            {
                case MetricEnum.ExteriorStage:
                    return IParameterEnum.ExteriorStageFrequency;
                case MetricEnum.InteriorStage:
                    return IParameterEnum.InteriorStageFrequency;
                case MetricEnum.Damages:
                    return IParameterEnum.DamageFrequency;
                case MetricEnum.ExpectedAnnualDamage:
                    return IParameterEnum.DamageFrequency;
                default:
                    throw new InvalidOperationException($"The specified metric type: {type.ToString()} was not successfully matched with a valid target function.");
            }
        }
        public double Compute(IFrequencyFunction fx, double p)
        {
            if (fx.ParameterType == TargetFunction)
            {
                IFrequencyFunction sampledFx = IFrequencyFunctionFactory.Factory(
                    ((ICoordinatesFunction)fx).Sample(p), fx.ParameterType, 
                    fx.XSeries.Label, fx.YSeries.Units, fx.YSeries.Label);
                if (Type == MetricEnum.ExpectedAnnualDamage) return sampledFx.Integrate();
                else return sampledFx.InverseF(IOrdinateFactory.Factory(ExceedanceTarget)).Value();
            }
            else throw new ArgumentException($"The {Type} metric cannot be computed with the provided {fx.ParameterType} function, a {TargetFunction} function is required.");
        }
        #endregion
    }
}
