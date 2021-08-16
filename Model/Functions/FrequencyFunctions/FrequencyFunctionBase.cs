using System;
using System.Collections.Generic;
using System.Text;
using Functions;

namespace Model.Functions.FrequencyFunctions
{
    internal abstract class FrequencyFunctionBase : FdaFunctionBase, IFrequencyFunction
    {
        private readonly IFunction _fx;
        internal FrequencyFunctionBase(IFunction fx) : base(fx)
        {
            _fx = fx;
        }

        public abstract List<IParameterEnum> ComposeableTypes { get; }

        public double Integrate()
        {
            return _fx.TrapizoidalRiemannSum();
        }
    }
}
