using System;
using System.Collections.Generic;
using Functions;
using Functions.Ordinates;
using Model.Functions;

using Utilities;

namespace Model.Metrics
{
    internal sealed class Metric: IMetric
    {
        #region Properties
        public string Label { get; }
        public UnitsEnum Units { get; }

        public IParameterEnum ParameterType { get; }
        public IOrdinate Ordinate { get; }  
        public IParameterEnum TargetFunction { get; }
        public bool IsConstant => true;

        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }
        #endregion

        #region Constructors
        internal Metric (IMetricEnum type, double exceedanceTarget = 0, string label = "", UnitsEnum units = UnitsEnum.NotSet, bool abbreviate = true)
        {
            if (type == IMetricEnum.NotSet) throw new ArgumentException("The desired type of metric must be set.");
            if (type != IMetricEnum.ExpectedAnnualDamage && !exceedanceTarget.IsFinite()) throw new ArgumentOutOfRangeException("A computable target value must be provided.");
            ParameterType = SetType(type);
            Units = units == UnitsEnum.NotSet ? ParameterType.UnitsDefault() : units;
            Label = label == "" ? ParameterType.PrintLabel(Units, abbreviate) : label;
            Ordinate = IOrdinateFactory.Factory(exceedanceTarget);
            TargetFunction = SetTargetFunction(type);
            State = Validate(new Validation.Metrics.MetricValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }
        #endregion

        #region Methods
        public IMessageLevels Validate(IValidator<IMetric> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }
        public string Print(bool round = true, bool abbreviate = true)
        {
            if (ParameterType == IParameterEnum.EAD) return $"{ParameterType.Print(abbreviate)}";
            else return $"{ParameterType.Print(abbreviate)}({Ordinate.Print(round)})";
        }

        private IParameterEnum SetTargetFunction(IMetricEnum type)
        {
            switch (type)
            {
                case IMetricEnum.ExteriorStage:
                    return IParameterEnum.ExteriorStageFrequency;
                case IMetricEnum.InteriorStage:
                    return IParameterEnum.InteriorStageFrequency;
                case IMetricEnum.Damages:
                    return IParameterEnum.DamageFrequency;
                case IMetricEnum.ExpectedAnnualDamage:
                    return IParameterEnum.DamageFrequency;
                default:
                    throw new InvalidOperationException($"The specified metric type: {type.ToString()} was not successfully matched with a valid target function.");
            }
        }
        private IParameterEnum SetType(IMetricEnum type)
        {
            switch (type)
            {
                case IMetricEnum.ExteriorStage:
                    return IParameterEnum.ExteriorStageAEP;
                case IMetricEnum.InteriorStage:
                    return IParameterEnum.InteriorStageAEP;
                case IMetricEnum.Damages:
                    return IParameterEnum.DamageAEP;
                case IMetricEnum.ExpectedAnnualDamage:
                    return IParameterEnum.EAD;
                default:
                    throw new InvalidOperationException($"The specified metric type: {type.ToString()} was not successfully matched with a valid target function.");
            }
        }
        public double Compute(IFrequencyFunction frequencyFx, double p = 0.50)
        {
            if (frequencyFx.ParameterType == TargetFunction)
            {
                IFrequencyFunction fx = frequencyFx.IsConstant ? frequencyFx : frequencyFx.Sample(p);
                if (ParameterType == IParameterEnum.EAD) return fx.Integrate();
                else return fx.InverseF(IOrdinateFactory.Factory(Ordinate.Value())).Value();
            }
            else throw new ArgumentException($"The {ParameterType} metric cannot be computed with the provided {frequencyFx.ParameterType} function, a {TargetFunction} function is required.");
        }
        #endregion
    }
}
