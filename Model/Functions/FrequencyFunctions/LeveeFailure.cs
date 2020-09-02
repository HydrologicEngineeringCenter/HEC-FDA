using Functions;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model.Functions
{
    internal sealed class LeveeFailure : FdaFunctionBase, IFrequencyFunction
    {
        #region Properties
        public override string Label { get; }
        public override IParameterRange XSeries { get; }
        public override IParameterRange YSeries { get; }
        //public override UnitsEnum Units { get; }
        public override IParameterEnum ParameterType => IParameterEnum.LateralStructureFailure;
        public List<IParameterEnum> ComposeableTypes => new List<IParameterEnum>() {  };

        public override IMessageLevels State => throw new NotImplementedException();

        public override IEnumerable<IMessage> Messages => throw new NotImplementedException();
        #endregion

        #region Constructor
        internal LeveeFailure(IFunction fx, string label = "", string xLabel = "", string yLabel = "", UnitsEnum yUnits = UnitsEnum.Foot) : base(fx)
        {
            Label = label == "" ? ParameterType.Print() : label;
            XSeries = IParameterFactory.Factory(fx, IParameterEnum.FailureProbability, true, true, UnitsEnum.Probability, xLabel);
            YSeries = IParameterFactory.Factory(fx, IParameterEnum.ExteriorElevation, IsConstant, false, yUnits, yLabel);
           // Units = YSeries.Units;
        }
        #endregion

        #region Function
        public double Integrate() => _Function.TrapizoidalRiemannSum();
        #endregion

    }
}
