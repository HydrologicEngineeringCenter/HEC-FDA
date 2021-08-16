using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model.Samples
{
    internal sealed class SampledOrdinate : ISampledParameter<IParameterOrdinate>
    {
        public bool IsSample { get; }
        public double Probability { get; }
        public IParameterOrdinate Parameter { get; }

        public SampledOrdinate(IParameterOrdinate ordinate, ISample sampleParameter)
        {
            if (ordinate.IsNull()) throw new ArgumentNullException(nameof(ordinate));
            if (sampleParameter.IsNull()) throw new ArgumentNullException(nameof(sampleParameter));
            Parameter = ordinate;
            IsSample = sampleParameter.IsSample;
            Probability = sampleParameter.Probability;
        }
    }
}
