using Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    public interface IComputableTransformFunction : IFdaFunction<double>
    {
        ImpactAreaFunctionEnum Type { get; }
        IFunction TransformFunction { get; }
    }
}
