using Functions;
using Model.Condition.ComputePoint.ImpactAreaFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Condition.ComputePoint.ComputableFunctions.FrequencyFunctions
{
    internal class InteriorStageFrequencyComputable : ImpactAreaFunctionBase<double>, IComputableFrequencyFunction
    {
        public override string XLabel => throw new NotImplementedException();

        public override string YLabel => throw new NotImplementedException();

        private IFunction _Function;

        internal InteriorStageFrequencyComputable(IFunction function) : base(function, ImpactAreaFunctionEnum.DamageFrequency)
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
