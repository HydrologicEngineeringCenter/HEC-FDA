using Functions;
using Model.Condition.ComputePoint.ImpactAreaFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Condition.ComputePoint.ComputableFunctions
{
    internal class InflowOutflowComputable : ImpactAreaFunctionBase<double>, IComputableTransformFunction
    {
        public override string XLabel => throw new NotImplementedException();

        public override string YLabel => throw new NotImplementedException();
        public IFunction TransformFunction { get; }

        public InflowOutflowComputable(IFunction function) : base(function, ImpactAreaFunctionEnum.InflowOutflow)
        {
            TransformFunction = function;
        }

    }
}
