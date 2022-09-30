using System;

namespace Statistics.Distributions
{
    public class SpecialFunctions
    {
        public const double EulerConst = 0.5772156649015329;
        private const double GAMMA = 0.5772156649015329;
        private const double GAMMA_MINX = 1.0E-12;
        private const double DIGAMMA_MINNEGX = -1250;
        private const double C_LIMIT = 49;
        private const double S_LIMIT = 1.0E-5;
        //********** GAMMA **********
        public static double logGamma(double t)
        {
            //if (double.IsNaN(t) || t <= 0.0)
            //{
            //	return double.NaN;
            //}
            if (t < 0.5)
            {
                // Reflection formula
                return Math.Log(Math.PI) - Math.Log(Math.Sin(Math.PI * t)) - SpecialFunctions.logGamma(1 - t);
            }
            else
            {
                // Coefficients used by the GNU Scientific Library
                var sum = 0.9999999999998099 + 676.5203681218851 / (t) - 1259.1392167224028 / (t + 1.0) + 771.3234287776531 / (t + 2.0) - 176.6150291621406 / (t + 3.0) + 12.507343278686905 / (t + 4.0) - 0.13857109526572012 / (t + 5.0) + 9.984369578019572E-6 / (t + 6.0) + 1.5056327351493116E-7 / (t + 7.0);
                var baseNumber = t + 7 - 0.5;
                return ((0.5 * Math.Log(2.0 * Math.PI) + Math.Log(sum)) - baseNumber) + (t - 0.5) * Math.Log(baseNumber);
            }
        }
        public static double logFactorial(int n)
        {
            return SpecialFunctions.logGamma(n + 1.0);
        }
        public static double gamma(double t)
        {
            return Math.Exp(SpecialFunctions.logGamma(t));
        }
        public static double factorial(int n)
        {
            return Math.Round(SpecialFunctions.gamma(n + 1.0));
        }
        public static double incompleteGamma(double t, double x)
        {
            return SpecialFunctions.gamma(t) * SpecialFunctions.regIncompleteGamma(t, x);
        }
        public static double incompleteGamma(double t, double xl, double xu)
        {
            return SpecialFunctions.incompleteGamma(t, xu) - SpecialFunctions.incompleteGamma(t, xl);
        }
        public static double regIncompleteGamma(double t, double x)
        {
            return SpecialFunctions.regularizedGammaP(t, x, 1.0E-14, int.MaxValue);
        }
        public static double logIncompleteGamma(double t, double x)
        {
            return SpecialFunctions.logGamma(t) + Math.Log(SpecialFunctions.regIncompleteGamma(t, x));
        }
        /**
            * <p>Computes the digamma function of x.</p>
            *
            * <p>This is an independently written implementation of the algorithm described in
            * Jose Bernardo, Algorithm AS 103: Psi (Digamma) Function, Applied Statistics, 1976.</p>
            *
            * <p>Some of the constants have been changed to increase accuracy at the moderate expense
            * of run-time.  The result should be accurate to within 10^-8 absolute tolerance for
            * x >= 10^-5 and within 10^-8 relative tolerance for x > 0.</p>
            *
            * <p>Performance for large negative values of x will be quite expensive (proportional to
            * |x|).  Accuracy for negative values of x should be about 10^-8 absolute for results
            * less than 10^5 and 10^-8 relative for results larger than that.</p>
            *
            * @param x Argument.
            * @return digamma(x) to within 10-8 relative or absolute error whichever is smaller.
            * @see <a href="http://en.wikipedia.org/wiki/Digamma_function">Digamma</a>
            * @see <a href="http://www.uv.es/~bernardo/1976AppStatist.pdf">Bernardo's original article </a>
            * @since 2.0
            */
        public static double digamma(double x)
        {
            //if (double.IsNaN(x) || double.IsInfinity(x))
            //{
            //	return x;
            //}
            if ((Math.Sign(x) == -1.0 && Math.Ceiling(x) == x) || x == 0)
            {
                return double.PositiveInfinity;
            }
            double value = 0;
            while (true)
            {
                if (x >= 0 && x < SpecialFunctions.GAMMA_MINX)
                {
                    x = SpecialFunctions.GAMMA_MINX;
                }
                if (x < SpecialFunctions.DIGAMMA_MINNEGX)
                {
                    x = SpecialFunctions.DIGAMMA_MINNEGX + SpecialFunctions.GAMMA_MINX;
                    continue;
                }
                if (x > 0 && x <= SpecialFunctions.S_LIMIT)
                {
                    return value + -SpecialFunctions.GAMMA - 1 / x;
                }
                if (x >= SpecialFunctions.C_LIMIT)
                {
                    var inv = 1 / (x * x);
                    return value + Math.Log(x) - 0.5 / x - inv * ((1.0 / 12) + inv * (1.0 / 120 - inv / 252));
                }
                value -= 1 / x;
                x = x + 1;
            }
        }
        /**
            * Returns the regularized gamma function P(a, x).
            * The implementation of this method is based on:
            *   <a href="http://mathworld.wolfram.com/RegularizedGammaFunction.html">
            *   Regularized Gamma Function</a>, equation (1)
            *   <a href="http://mathworld.wolfram.com/IncompleteGammaFunction.html">
            *   Incomplete Gamma Function</a>, equation (4).
            *   <a href="http://mathworld.wolfram.com/ConfluentHypergeometricFunctionoftheFirstKind.html">
            *   Confluent Hypergeometric Function of the First Kind</a>, equation (1).
            * @param a the a parameter.
            * @param x the value.
            * @param epsilon When the absolute value of the nth item in the
            * series is less than epsilon the approximation ceases to calculate
            * further elements in the series.
            * @param maxIterations Maximum number of "iterations" to complete.
            * @return the regularized gamma function P(a, x)
            */
        private static double regularizedGammaP(double a, double x, double epsilon, int maxIterations)
        {
            var ret = 0.0;
            //if (double.IsNaN(a) || double.IsNaN(x))
            //{
            //    ret = double.NaN;
            //}

            if ((a <= 0.0) || (x < 0.0))
            {
                ret = double.NaN;
            }
            else if (x == 0.0)
            {
                ret = 0.0;
            }
            else if (x >= a + 1)
            {
                // use regularizedGammaQ because it should converge faster in this
                // case.
                ret = 1.0 - SpecialFunctions.regularizedGammaQ(a, x, epsilon, maxIterations);
            }
            else
            {
                // calculate series
                var n = 0.0;
                // current element index
                var an = 1.0 / a;
                // n-th element in the series
                var sum = an;
                // partial sum
                while (Math.Abs(an / sum) > epsilon && n < maxIterations && sum < double.PositiveInfinity)
                {
                    // compute next element in the series
                    n += 1.0;
                    an *= x / (a + n);
                    // update partial sum
                    sum += an;
                }
                if (n >= maxIterations)
                { }
                else if (sum>double.MaxValue)
                {
                    ret = 1.0;
                }
                else
                {
                    ret = Math.Exp(-x + (a * Math.Log(x)) - SpecialFunctions.logGamma(a)) * sum;
                }
            }
            return ret;
        }
        /**
            * Returns the regularized gamma function Q(a, x) = 1 - P(a, x).
            * The implementation of this method is based on:
            * <ul>
            *  <li>
            *   <a href="http://mathworld.wolfram.com/RegularizedGammaFunction.html">
            *   Regularized Gamma Function</a>, equation (1).
            *   <a href="http://functions.wolfram.com/GammaBetaErf/GammaRegularized/10/0003/">
            *   Regularized incomplete gamma function: Continued fraction representations
            * @param a the a parameter.
            * @param x the value.
            * @param epsilon When the absolute value of the nth item in the
            * series is less than epsilon the approximation ceases to calculate
            * further elements in the series.
            * @param maxIterations Maximum number of "iterations" to complete.
            * @return the regularized gamma function P(a, x)
            */
        private static double regularizedGammaQ(double a, double x, double epsilon, int maxIterations)
        {
            double ret;
            //if (double.IsNaN(a) || double.IsNaN(x))
            //{
            //    ret = double.NaN;
            //}
            if ((a <= 0.0) || (x < 0.0))
            {
                ret = double.NaN;
            }
            else if (x == 0.0)
            {
                ret = 1.0;
            }
            else if (x < a + 1.0)
            {
                // use regularizedGammaP because it should converge faster in this
                // case.
                ret = 1.0 - SpecialFunctions.regularizedGammaP(a, x, epsilon, maxIterations);
            }
            else
            {
                // create continued fraction
                ret = 1.0 / SpecialFunctions.evaluateCFGammaQ(a, x, epsilon, maxIterations);
                ret = Math.Exp(-x + (a * Math.Log(x)) - SpecialFunctions.logGamma(a)) * ret;
            }
            return ret;
        }
        /**
            * Evaluates the continued fraction at the value x.
            * The implementation of this method is based on the modified Lentz algorithm as described
            * on page 18 ff. in:
            *   I. J. Thompson,  A. R. Barnett. "Coulomb and Bessel Functions of Complex Arguments and Order."
            *   <a target="_blank" href="http://www.fresco.org.uk/papers/Thompson-JCP64p490.pdf">
            *   http://www.fresco.org.uk/papers/Thompson-JCP64p490.pdf</a>
            * <b>Note:</b> the implementation uses the terms a<sub>i</sub> and b<sub>i</sub> as defined in
            * <a href="http://mathworld.wolfram.com/ContinuedFraction.html">Continued Fraction @ MathWorld</a>.
            * @param x the evaluation point.
            * @param epsilon maximum error allowed.
            * @param maxIterations maximum number of convergents
            * @return the value of the continued fraction evaluated at x.
            */
        private static double evaluateCFGammaQ(double a, double x, double epsilon, int maxIterations)
        {
            var small = 1.0E-50;
            var hPrev = ((2.0 * 0.0) + 1.0) - a + x;
            //getA(0, x);
            // use the value of small as epsilon criteria for zero checks
            if (Math.Abs(hPrev - 0.0) < small)
            {
                hPrev = small;
            }
            var n = 1;
            var dPrev = 0.0;
            var cPrev = hPrev;
            var hN = hPrev;
            while (n < maxIterations)
            {

                var aa = ((2.0 * n) + 1.0) - a + x;
                //getA(n, x);

                var bb = n * (a - n);
                //getB(n, x);
                var dN = aa + bb * dPrev;
                if (Math.Abs(dN - 0.0) < small)
                {
                    dN = small;
                }
                var cN = aa + bb / cPrev;
                if (Math.Abs(cN - 0.0) < small)
                {
                    cN = small;
                }
                dN = 1 / dN;

                var deltaN = cN * dN;
                hN = hPrev * deltaN;
                //if (double.IsInfinity(hN))
                //{ }
                //if (double.IsNaN(hN))
                //{ }
                if (Math.Abs(deltaN - 1.0) < epsilon)
                {
                    break;
                }
                dPrev = dN;
                cPrev = cN;
                hPrev = hN;
                n++;
            }
            if (n >= maxIterations)
            { }
            return hN;
        }
        //********** BETA **********
        public static double logBeta(double s, double t)
        {
            if (double.IsNaN(s) || double.IsNaN(t) || s <= 0.0 || t <= 0.0)
            {
                return double.NaN;
            }
            return SpecialFunctions.logGamma(s) + (SpecialFunctions.logGamma(t) - SpecialFunctions.logGamma(s + t));
        }
        public static double beta(double s, double t)
        {
            return Math.Exp(SpecialFunctions.logBeta(s, t));
        }
        public static double incompleteBeta(double s, double t, double x)
        {
            return SpecialFunctions.beta(s, t) * SpecialFunctions.regIncompleteBeta(s, t, x);
        }
        public static double regIncompleteBeta(double s, double t, double x)
        {
            return SpecialFunctions.regularizedBeta(s, t, x, 1.0E-14, int.MaxValue);
        }
        /**
            * Returns the regularized beta function I(x, a, b).
            * The implementation of this method is based on:
            * <a href="http://mathworld.wolfram.com/RegularizedBetaFunction.html">
            * Regularized Beta Function</a>.</li>
            * <a href="http://functions.wolfram.com/06.21.10.0001.01">
            * Regularized Beta Function</a>.</li>
            * @param x the value.
            * @param a Parameter {@code a}.
            * @param b Parameter {@code b}.
            * @param epsilon When the absolute value of the nth item in the
            * series is less than epsilon the approximation ceases to calculate
            * further elements in the series.
            * @param maxIterations Maximum number of "iterations" to complete.
            * @return the regularized beta function I(x, a, b)
            */
        public static double regularizedBeta(double a, double b, double x, double epsilon, int maxIterations)
        {
            double ret;
            if (double.IsNaN(x) || double.IsNaN(a) || double.IsNaN(b) || x < 0 || x > 1 || a <= 0.0 || b <= 0.0)
            {
                ret = double.NaN;
            }
            else if (x > (a + 1) / (2 + b + a) && 1 - x <= (b + 1) / (2 + b + a))
            {
                ret = 1 - SpecialFunctions.regularizedBeta(b, a, 1 - x, epsilon, maxIterations);
            }
            else
            {
                ret = Math.Exp((a * Math.Log(x)) + (b * Math.Log(-x + 1)) - Math.Log(a) - SpecialFunctions.logBeta(a, b)) * 1.0 / SpecialFunctions.evaluateCFBeta(a, b, x, epsilon, maxIterations);
            }
            return ret;
        }
        /**
            * trigamma adapted from https://github.com/tminka/lightspeed/blob/master/trigamma.m
            * 
            *used in MLE fit of gamma distribution which also references Thomas Minka's work
            */
        private
        const double B10 = 5.0 / 66.0;
        private
        const double B2 = 1.0 / 6.0;
        private
        const double B4 = -1.0 / 30.0;
        private
        const double B6 = 1.0 / 42.0;
        private
        const double B8 = -1.0 / 30.0;
        private static double c = Math.Pow(Math.PI, 2.0 / 6.0);
        private
        const double c1 = -2.4041138063191885;
        private
        const double SMALL_TRIGAMMA = 1.0E-4;
        private
        const double LARGE_TRIGAMMA = 8.0;
        public static double trigamma(double x)
        {
            if (double.IsNaN(x) || double.IsInfinity(x) || 0.0.CompareTo(x) == 0 || (x < 0.0 && Math.Floor(x).CompareTo(x) == 0))
            {
                return double.NaN;
            }
            double y = 0;
            if (x < 0.0)
            {
                var val = (Math.PI * (1.0 / Math.Sin(-Math.PI * x)));
                y = -SpecialFunctions.trigamma(-x + 1) + val * val;
            }
            if (x > 0.0 && x <= SpecialFunctions.SMALL_TRIGAMMA)
            {
                y = 1 / (x * x) + SpecialFunctions.c + SpecialFunctions.c1 * x;
            }
            while (x > SpecialFunctions.SMALL_TRIGAMMA && x < SpecialFunctions.LARGE_TRIGAMMA)
            {
                y += 1.0 / (x * x);
                x++;
            }
            if (x >= SpecialFunctions.LARGE_TRIGAMMA)
            {
                var z = 1.0 / (x * x);
                y += 0.5 * z + (1.0 + z * (SpecialFunctions.B2 + z * (SpecialFunctions.B4 + z * (SpecialFunctions.B6 + z * (SpecialFunctions.B8 + z * SpecialFunctions.B10))))) / x;
            }
            return y;
        }
        /**
            * Evaluates the continued fraction at the value x.
            * The implementation of this method is based on the modified Lentz algorithm as described
            * on page 18 ff. in:
            *   I. J. Thompson,  A. R. Barnett. "Coulomb and Bessel Functions of Complex Arguments and Order."
            *   <a target="_blank" href="http://www.fresco.org.uk/papers/Thompson-JCP64p490.pdf">
            *   http://www.fresco.org.uk/papers/Thompson-JCP64p490.pdf</a>
            * <b>Note:</b> the implementation uses the terms a<sub>i</sub> and b<sub>i</sub> as defined in
            * <a href="http://mathworld.wolfram.com/ContinuedFraction.html">Continued Fraction @ MathWorld</a>.
            * @param x the evaluation point.
            * @param epsilon maximum error allowed.
            * @param maxIterations maximum number of convergents
            * @return the value of the continued fraction evaluated at x.
            */
        private static double evaluateCFBeta(double a, double b, double x, double epsilon, int maxIterations)
        {

            var small = 1.0E-50;
            var hPrev = 1.0;
            //getA(0, x);
            // use the value of small as epsilon criteria for zero checks
            if (Math.Abs(hPrev - 0.0) < small)
            {
                hPrev = small;
            }
            var n = 1;
            var dPrev = 0.0;
            var cPrev = hPrev;
            var hN = hPrev;
            while (n < maxIterations)
            {

                var aa = 1.0;
                //getA(n, x);
                double bb;
                //getB(n, x);
                double m;
                if (n % 2 == 0)
                {
                    // even
                    m = n / 2.0;
                    bb = (m * (b - m) * x) / ((a + (2 * m) - 1) * (a + (2 * m)));
                }
                else
                {
                    m = (n - 1.0) / 2.0;
                    bb = -((a + m) * (a + b + m) * x) / ((a + (2 * m)) * (a + (2 * m) + 1.0));
                }
                var dN = aa + bb * dPrev;
                if (Math.Abs(dN - 0.0) < small)
                {
                    dN = small;
                }
                var cN = aa + bb / cPrev;
                if (Math.Abs(cN - 0.0) < small)
                {
                    cN = small;
                }
                dN = 1 / dN;

                var deltaN = cN * dN;
                hN = hPrev * deltaN;
                if (double.IsInfinity(hN))
                { }
                if (double.IsNaN(hN))
                { }
                if (Math.Abs(deltaN - 1.0) < epsilon)
                {
                    break;
                }
                dPrev = dN;
                cPrev = cN;
                hPrev = hN;
                n++;
            }
            if (n >= maxIterations)
            { }
            return hN;
        }
        //********** GENERALIZATION FORMULA **********
        /**
            * @param x must be in the support of t: 
            *       [loctn+scale/shape,+Infinity) for shape<0
            *       (-Infinity,+Infinity) for shape=0
            *       (-Infinity,loctn+scale/shape] for shape>0
            * @return t(x)
            */
        public static double t(double loctn, double scale, double shape, double x)
        {
            if (shape == 0.0)
            {
                return Math.Exp(-(x - loctn) / scale);
            }
            else
            {
                return Math.Pow(1.0 - shape * (x - loctn) / scale, +1.0 / shape);
            }
        }
        /**
            * @param y must be positive
            * @return tInv(y)
            */
        public static double tInv(double loctn, double scale, double shape, double y)
        {
            if (shape == 0.0)
            {
                return loctn - scale * Math.Log(y);
            }
            else
            {
                return loctn - scale * (Math.Pow(y, shape) - 1.0) / shape;
            }
        }
        /**
            * binomial coefficient to determine the number of combinations of size k in n
            * @return
            */
        public static int binomialCoefficient(int n, int k)
        {
            if (n < k)
            {
                throw new Exception("N has to be greater than k!");
            }
            return (int)(((int)SpecialFunctions.factorial(n)) / (((int)SpecialFunctions.factorial(k)) * ((int)SpecialFunctions.factorial(n - k))));
        }
        public static double singleParGammaPDF(double alpha, double x)
        {
            return (Math.Pow(x, (alpha - 1)) * Math.Exp(-x)) / SpecialFunctions.gamma(alpha);
        }
        /**
            * gammaDerivative - Derived from (probfun.f) ddgam.
            *
            * Program to compute derivative of incomplete gamma function.
            * 
            * @param alpha
            * @param x
            */
        public static double gammaDerivative(double alpha, double x)
        {
            if (alpha < 0 || x < 0 || Math.Abs(x - alpha) / Math.Sqrt(alpha + 1.0) > 7.0)
            {
                return 0;
            }
            var log_X = Math.Log(x);
            var tol = 1.0E-11;
            double t;
            double r;
            double sum;
            double del;
            if (x < 5)
            {
                t = Math.Pow(x, alpha);
                r = (alpha * log_X - 1.0) / Math.Pow(alpha, 2);
                sum = t * r;
                for (var i = 1; i <= 1000; i++)
                {
                    var ai = alpha + i;
                    t = -t * x / i;
                    r = (ai * log_X - 1.0) / Math.Pow(ai, 2);
                    del = r * t;
                    sum += del;
                    if (i > 1 && Math.Abs(del) < (1.0 + Math.Abs(sum)) * tol)
                    {
                        return sum / Math.Exp(SpecialFunctions.logGamma(alpha)) - SpecialFunctions.digamma(alpha) * SpecialFunctions.regIncompleteGamma(alpha, x);
                    }
                }
                // TODO: shouldn't come here. return error
                return 0;
            }
            else if (x > alpha + 30.0)
            {
                t = Math.Exp(-x + (alpha - 1.0) * log_X - SpecialFunctions.logGamma(alpha));
                r = log_X - SpecialFunctions.digamma(alpha);
                sum = r * t;
                for (var i = 1; i < (int)(alpha - 1.0); i++)
                {
                    var ami = alpha - i;
                    t = t * ami / x;
                    r = log_X - SpecialFunctions.digamma(ami);
                    del = r * t;
                    sum += del;
                    if (i > 1 && Math.Abs(del) < (1.0 + Math.Abs(sum)) * tol)
                    {
                        return -sum;
                    }
                }
                // TODO: shouldn't come here. return error
                return 0;
            }
            else
            {
                t = Math.Exp(-x + alpha * log_X - SpecialFunctions.logGamma(alpha + 1.0));
                r = log_X - SpecialFunctions.digamma(alpha + 1.0);
                sum = r * t;
                for (var i = 1; i < 10000; i++)
                {
                    t = t * x / (alpha + i);
                    r = log_X - SpecialFunctions.digamma(alpha + i + 1.0);
                    del = r * t;
                    sum += del;
                    if (i > 1 && Math.Abs(del) < (1.0 + Math.Abs(sum)) * tol)
                    {
                        return sum;
                    }
                }
                // TODO: shouldn't come here. return error
                return 0;
            }
        }
        //********** MAIN **********
        public static void Main(String[] args)
        {
            //for(int i=1;i<20;i++){
            //	double x = 10+0.2*i;
            //	System.out.println("gam("+x+"): "+Math.exp(logGamma(x)));
            //}
            var s = 1.0E12;
            for (var i = 0; i <= 20; i++)
            {
                var val = s + (1000000 * i) / 20.0;
                Console.WriteLine("rig(" + val.ToString() + "): " + SpecialFunctions.regIncompleteGamma(s, val).ToString());
            }
        }
    }
}