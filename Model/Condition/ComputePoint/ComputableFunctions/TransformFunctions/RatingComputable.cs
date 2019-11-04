using Functions;
using Model.Condition.ComputePoint.ImpactAreaFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Condition.ComputePoint
{
    internal class RatingComputable : ImpactAreaFunctionBase<double>, IComputableTransformFunction
    {
        public override string XLabel => "Stage";
        public override string YLabel => "Flow";
        public IFunction TransformFunction { get; }
        internal RatingComputable(IFunction function) : base(function, ImpactAreaFunctionEnum.Rating)
        {
            TransformFunction = function;
        }



    }
}
