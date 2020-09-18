using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Utilities;

namespace Statistics.Distributions
{
    internal sealed class Gamma
    {
        public double Alpha { get; }
        public double Beta { get; }

        internal Gamma(double alpha, double beta)
        {
            if (beta == 0d) throw new ArgumentOutOfRangeException(nameof(beta), "The Gamma distribution Beta parameter cannot be equal to 0.");
            Alpha = alpha;
            Beta = beta;
        }

        internal double CDF(double x)
        {

        }
        public double IncompleteGamma(double t, double x)
        {
            return RegularizedGammaP(t, x, 0.00000000000001, int.MaxValue);
        }
        public double RegularizedGammaP(double a, double x, double epsilon, int maxIterations)
        {
            if (!a.IsFinite() || a <= 0) throw new ArgumentOutOfRangeException(nameof(a), $"The incomplete gamma alpha parameter: {a} is invalid. It must be a positive finite value.");
            if (!x.IsFinite() || x < 0) throw new ArgumentOutOfRangeException(nameof(x), $"The incomplete gamma paramter x: {x} is invalid. It must be a non-negative finite value.");
            if (x == 0) return 0;
            else if (x >= a + 1) return 
        }
    }
}
