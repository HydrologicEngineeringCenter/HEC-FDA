using System;
using Utilities;

namespace Model.Samples
{
    internal sealed class SampledFunction : ISampledParameter<IFdaFunction>
    {
        public bool IsSample { get; }
        public double Probability { get; }
        public IFdaFunction Parameter { get; }

        internal SampledFunction(ISample sample, IFdaFunction fx)
        {
            if (fx.IsNull()) throw new ArgumentNullException(nameof(IFdaFunction));
            IsSample = sample.IsSample;
            Probability = sample.Probability;
            Parameter = fx;
        }
    }
}
