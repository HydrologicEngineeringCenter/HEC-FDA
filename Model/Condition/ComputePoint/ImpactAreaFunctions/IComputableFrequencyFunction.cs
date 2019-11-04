using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    public interface IComputableFrequencyFunction:IFdaFunction<double>
    {
        ImpactAreaFunctionEnum Type { get; }

        IComputableFrequencyFunction Compose(IComputableTransformFunction function);
        double Integrate();
    }
}
