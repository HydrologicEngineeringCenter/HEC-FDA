using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Functions;
using Functions.CoordinatesFunctions;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    internal class ExteriorInteriorStageComputable : ImpactAreaFunctionBase<double>, IComputableTransformFunction
    {
        public override string XLabel => "Exterior Stage";

        public override string YLabel => "Interior Stage";

        public IFunction TransformFunction { get; }

        internal ExteriorInteriorStageComputable(IFunction function):base(function, ImpactAreaFunctionEnum.ExteriorInteriorStage)
        {
            TransformFunction = function;
        }

    }
}
