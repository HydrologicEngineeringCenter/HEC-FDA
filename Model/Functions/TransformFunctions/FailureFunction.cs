using System;
using System.Collections.Generic;
using Functions;
using Utilities;

namespace Model.Functions
{
    internal sealed class FailureFunction : FdaFunctionBase, ITransformFunction
    {
        public override string Label { get; }
        public override IParameterRange XSeries { get; }
        public override IParameterRange YSeries { get; }
        public override IParameterEnum ParameterType => IParameterEnum.LateralStructureFailure;
        
        public override IMessageLevels State { get; }
        public override IEnumerable<IMessage> Messages { get; }

        internal FailureFunction(IFunction fx, UnitsEnum xUnits = UnitsEnum.Foot, string xLabel = "", string yLabel = "", string label = ""): base(fx)
        {
            Label = label == "" ? ParameterType.Print() : label;
            XSeries = IParameterFactory.Factory(fx, IParameterEnum.ExteriorElevation, true, true, xUnits, xLabel);
            YSeries = IParameterFactory.Factory(fx, IParameterEnum.FailureProbability, IsConstant, false, UnitsEnum.Probability, yLabel);
            State = Validate(new Validation.Functions.FdaFunctionBaseValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }

        public double F(double p)
        {
            if (!p.IsOnRange(0.0, 1.0)) throw new ArgumentOutOfRangeException($"The specified probability of failure: {p} is invalid because it is not on the valid range of probabilities: [0, 1].");
            if (!(_Function.DistributionType == IOrdinateEnum.Constant)) throw new ArgumentException($"The specified failure function has {_Function.DistributionType} distributed elevation (i.e. stage) values, therefore the failure probability {p} cannot be mapped to a single elevation. To sample the failure stage, draw a sample failure function (from this function) and try again.");
            return _Function.F(IOrdinateFactory.Factory(p)).Value();
        }
        public override IOrdinate F(IOrdinate p)
        {
            if (!p.Range.Min.IsOnRange(0.0, 1.0) || 
                !p.Range.Max.IsOnRange(0.0, 1.0)) throw new ArgumentOutOfRangeException($"The specified probability of failure: {p.Print(true)} is invalid because it is not on the valid range of probabilities: [0, 1].");
            return base.F(p);
        }
    }
}
