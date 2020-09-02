using Functions;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model.Functions
{
    internal sealed class MetricYear : FdaFunctionBase
    {
        public override string Label { get; }
        public override IParameterRange XSeries { get; }
        public override IParameterRange YSeries { get; }

        public override IParameterEnum ParameterType { get; }

        public override IMessageLevels State { get; }
        public override IEnumerable<IMessage> Messages { get; }

        internal MetricYear(IFunction fx, IParameterEnum type, string label, string xLabel = "", string yLabel = "", UnitsEnum yUnits = UnitsEnum.Probability) : base(fx)
        {
            ParameterType = type;
            Label = label == "" ? ParameterType.Print() : label;
            XSeries = IParameterFactory.Factory(fx, IParameterEnum.Year, isConstant: true, x: true, units: UnitsEnum.Year, xLabel);
            if (type == IParameterEnum.YearEAD) YSeries = IParameterFactory.Factory(fx, IParameterEnum.FloodDamages, IsConstant, x: false, units: yUnits, label: yLabel);
            else YSeries = IParameterFactory.Factory(fx, IParameterEnum.ExceedanceProbability, IsConstant, x: false, units: UnitsEnum.Probability, label: yLabel);
            State = Validate(new Validation.Functions.FdaFunctionBaseValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }
    }
}
