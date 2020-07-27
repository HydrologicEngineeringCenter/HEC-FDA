using System;
using System.Collections.Generic;
using System.Text;

using Functions;
using Utilities;

namespace Model.Conditions.Locations.LateralStructures
{
    internal class LateralStructure: ILateralStructure
    {
        public string Label { get; }
        public UnitsEnum Units { get; }
        public IParameterEnum ParameterType => IParameterEnum.LateralStructure;
        public IRange<double> Range => TopElevation.Range;
        public IElevation TopElevation { get; }
        public IFdaFunction FailureFunction { get; }
        /// <summary>
        /// <see langword="true"/> if failure always occurs at the top of levee elevation, <see langword="false"/> otherwise.
        /// </summary>
        public bool IsConstant { get; }

        internal LateralStructure(IElevation elevation, IFdaFunction fx, UnitsEnum units = UnitsEnum.Foot, string label = "")
        {
            //todo: must have constant elevation.
            //todo: IFdaFunction must be failure function.
            Units = units;
            Label = label == "" ? ParameterType.Print() : label;
            TopElevation = elevation;
            FailureFunction = fx;
        }

        public ILateralStructureRealization Compute(double failureFxProbability, IFrequencyFunction extFreqFx, double extFailElevProbability)
        {
            //todo: units check
            IElevation failureElev = IElevationFactory.Factory(extFreqFx.F(IOrdinateFactory.Factory(extFailElevProbability)), extFreqFx.YSeries.Units, IParameterEnum.ExteriorElevation, label: "");
            IFdaFunction sampledFx = IFdaFunctionFactory.Factory(FailureFunction.Sample(failureFxProbability), IParameterEnum.LateralStructureFailure, FailureFunction.YSeries.Label, xUnits: FailureFunction.XSeries.Units, FailureFunction.XSeries.Label, FailureFunction.Label);
            return new LateralStructureRealization(ILateralStructureFactory.Factory(TopElevation, sampledFx, Units, Label), failureElev);
        }

        public string Print(bool round = false, bool abbreviate = false) => throw new NotImplementedException();
        public string PrintValue(bool round = false, bool abbreviate = false) => throw new NotImplementedException();
    }
}
