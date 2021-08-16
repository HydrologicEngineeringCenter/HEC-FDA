using System;
using Utilities;

namespace Model.Samples
{
    internal sealed class Sample : ISample
    {
        public bool IsSample { get; }
        public double Probability { get; }

        internal Sample()
        {
            IsSample = false;
            Probability = 0.50d; // default value.
        }
        internal Sample(double nonexceedanceProbability)
        {
            if (!nonexceedanceProbability.IsOnRange(0d, 1d)) throw new ArgumentOutOfRangeException();
            IsSample = true;
            Probability = nonexceedanceProbability;
        }
    }
}
