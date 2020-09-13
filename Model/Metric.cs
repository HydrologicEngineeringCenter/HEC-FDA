using System;
using System.Collections.Generic;
using Functions;
using Functions.Ordinates;
using Model.Functions;
using Utilities;

namespace Model
{
    public class Metric: IMetric
    {
        #region Properties
        public double Ordinate { get; } = 0;
        public IMetricEnum Type { get; } = IMetricEnum.NotSet;
        public IParameterEnum TargetFunction { get; }

        public UnitsEnum Units => throw new NotImplementedException();

        IOrdinate IParameterOrdinate.Ordinate => throw new NotImplementedException();

        public IParameterEnum ParameterType => throw new NotImplementedException();

        public string Label => throw new NotImplementedException();

        public bool IsConstant => throw new NotImplementedException();

        public IMessageLevels State => throw new NotImplementedException();

        public IEnumerable<IMessage> Messages => throw new NotImplementedException();
        #endregion

        #region Constructors
        public Metric (IMetricEnum type, double exceedanceTarget = 0)
        {
            if (type == IMetricEnum.NotSet) throw new ArgumentException("The desired type of metric must be set.");
            if (type != IMetricEnum.ExpectedAnnualDamage && double.IsNaN(exceedanceTarget) || double.IsInfinity(exceedanceTarget)) throw new ArgumentException("A computable target value must be provided.");
            Type = type;
            Ordinate = exceedanceTarget;
            TargetFunction = GetTargetFunction();
        }
        internal Metric ()
        {
            Type = IMetricEnum.ExpectedAnnualDamage;
            TargetFunction = GetTargetFunction();
        }
        #endregion

        #region Methods
        private IParameterEnum GetTargetFunction()
        {
            switch (Type)
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
                    throw new InvalidOperationException("The application could not set a valid target function when calling Metric.TargetFunction()");
            }
        }
        public double Compute(IFrequencyFunction frequencyFunction, double probability)
        {
            IParameterEnum targetFunction = TargetFunction;
            if (frequencyFunction.ParameterType != targetFunction)
            {
                throw new ArgumentException(string.Format("A {0} metric cannot be computed from the provided {1} function. Provide a {2} function and try again.", Type, frequencyFunction.ParameterType, targetFunction));
            }

            IFunction sampledFreqFunc = Sampler.Sample(((FdaFunctionBase)frequencyFunction)._Function, probability);
            if (Type == IMetricEnum.ExpectedAnnualDamage)
            {
                return sampledFreqFunc.TrapizoidalRiemannSum();
                //return frequencyFunction.Integrate();
            }
            else
            {
                return sampledFreqFunc.InverseF(new Constant(Ordinate)).Value();
                //return frequencyFunction.GetXFromY(ExceedanceTarget);
            }
        }

        public string Print(bool round = false, bool abbreviate = false)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
