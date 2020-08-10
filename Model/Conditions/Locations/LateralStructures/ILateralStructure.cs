using System;
using System.Collections.Generic;
using System.Text;
using Functions;

namespace Model
{
    public interface ILateralStructure: IParameter
    {
        IParameterOrdinate TopElevation { get; }
        ITransformFunction FailureFunction { get; }

        //ILateralStructureRealization Compute(double failureFxProbability, IFrequencyFunction extFreqFx, double extFailElevProbability);
    }
}
