using Functions;
using Model.Condition.ComputePoint.ImpactAreaFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Condition.ComputePoint
{
    internal class ExteriorStageFrequencyComputable : ImpactAreaFunctionBase<double>, IComputableFrequencyFunction
    {
        public override string XLabel => throw new NotImplementedException();

        public override string YLabel => throw new NotImplementedException();

        internal ExteriorStageFrequencyComputable(IFunction function):base(function, ImpactAreaFunctionEnum.ExteriorStageFrequency)
        {

        }

        public IComputableFrequencyFunction Compose(IComputableTransformFunction function)
        {
            throw new NotImplementedException();
        }

        public double Integrate()
        {
            throw new NotImplementedException();
        }
    }
}
