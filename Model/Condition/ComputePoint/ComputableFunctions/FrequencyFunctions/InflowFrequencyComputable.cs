using Functions;
using Model.Condition.ComputePoint.ImpactAreaFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Condition.ComputePoint.ComputableFunctions
{
    internal class InflowFrequencyComputable : ImpactAreaFunctionBase<double>, IComputableFrequencyFunction
    {
        public override string XLabel => throw new NotImplementedException();

        public override string YLabel => throw new NotImplementedException();

        private IFunction _Function;

        internal InflowFrequencyComputable(IFunction function) : base(function, ImpactAreaFunctionEnum.InflowFrequency)
        {
            _Function = function;
        }

        public IComputableFrequencyFunction Compose(IComputableTransformFunction transform)
        {
            if (transform.Type - 1 == this.Type)
            {
                IFunction composedFunction = _Function.Compose(transform.TransformFunction);

                return ImpactAreaFunctionFactory.CreateNew(composedFunction, transform.Type + 1);
            }
            else
            {
                throw new ArgumentException(ReportCompositionError());
            }
        }

        private string ReportCompositionError()
        {
            return "Composition could not be initialized because no transform function was provided or the two functions do not share a common set of ordinates.";
        }

        public double Integrate()
        {
            return _Function.RiemannSum();
        }
    }
}
