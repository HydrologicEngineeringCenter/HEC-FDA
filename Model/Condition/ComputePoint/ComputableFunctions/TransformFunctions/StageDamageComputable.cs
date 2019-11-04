using Functions;
using Model.Condition.ComputePoint.ImpactAreaFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Condition.ComputePoint.ComputableFunctions.TransformFunctions
{
    internal class StageDamageComputable : ImpactAreaFunctionBase<double>, IComputableTransformFunction
    {
        public override string XLabel => "Stage";
        public override string YLabel => "Damage";
        public IFunction TransformFunction { get; }
        internal StageDamageComputable(IFunction function) : base(function, ImpactAreaFunctionEnum.InteriorStageDamage)
        {
            TransformFunction = function;
        }



    }
}
