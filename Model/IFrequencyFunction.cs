using Model.Inputs.Functions;
using Model.Inputs.Functions.ImpactAreaFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public interface IFrequencyFunction : IFdaFunction
    {
        List<ImpactAreaFunctionEnum> ComposeableTypes { get; }
        //IFrequencyFunction Compose(ITransformFunction transformFunction, double probability1, double probability2);

    }
}
