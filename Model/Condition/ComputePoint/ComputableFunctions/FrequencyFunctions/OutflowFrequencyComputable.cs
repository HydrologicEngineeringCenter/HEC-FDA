using Functions;
using Model.Condition.ComputePoint.ImpactAreaFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Condition.ComputePoint.ComputableFunctions
{
    internal class OutflowFrequencyComputable : ImpactAreaFunctionBase<double>, IComputableFrequencyFunction
    {
        public override string XLabel => "Flow";

        public override string YLabel => "Frequency";
        private IFunction _Function;

        internal OutflowFrequencyComputable(IFunction function) : base(function, ImpactAreaFunctionEnum.OutflowFrequency)
        {
            _Function = function;
        }

        public IComputableFrequencyFunction Compose(IComputableTransformFunction function)
        {
            throw new NotImplementedException();
        }

        public double Integrate()
        {
            return _Function.RiemannSum();
        }
    }
}
