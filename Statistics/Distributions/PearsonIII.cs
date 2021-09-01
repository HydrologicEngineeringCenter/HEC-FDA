using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Statistics.Distributions
{
    internal class PearsonIII
    {
        /// <summary>
        /// A minimum skewness value based on the PearsonIII class in the William Lehman's Statistics assembly.
        /// </summary>
        private readonly double _NoSkewness = 0.00001;
        /// <summary>
        /// A minimum standard deviation value based on the Pearson III class in William Lehman's Statistics assembly.
        /// </summary>
        private readonly double _StandardDeviationLimit = 0.00001;

        public double Mean { get; }
        public double StandardDeviation { get; }
        public double Skewness { get; }
        public int SampleSize { get; }

        public PearsonIII(double mean, double sd, double skew, int n = int.MaxValue)
        {
            Mean = mean;
            StandardDeviation = sd;
            Skewness = skew;
            SampleSize = n;
        }

        public double CDF(double x)
        {
            if (Math.Abs(Skewness) < _NoSkewness)
            {              
                // a PearsonIII distribution with no skew is normally distributed.
                IDistribution norm = new Normal(Mean, StandardDeviation);
                return norm.CDF(x);
            }
            else
            {
                // a skewed PearsonIII distribution is a shifted gamma distribution
                double shift;
                double alpha = 4d / (Skewness * Skewness); 
                double beta = 0.5 * StandardDeviation * Skewness;
                // positively skewed distribution 
                if (Skewness > 0)
                {
                    shift = Mean - 2d * StandardDeviation / Skewness;
                    if (!alpha.IsOnRange(0, double.PositiveInfinity, false, false) || !beta.IsOnRange(0, double.PositiveInfinity)) throw new InvalidOperationException(PrintExceptionMessage(alpha, beta));
                    ShiftedGamma gamma = new ShiftedGamma(alpha, beta, shift);
                    return gamma.CDF(x);
                }
                // negatively skewed distribution
                else
                {
                    beta = -beta;
                    shift = -Mean + 2d * StandardDeviation / Skewness;
                    if (!alpha.IsOnRange(0, double.PositiveInfinity, false, false) || !beta.IsOnRange(0, double.PositiveInfinity)) throw new InvalidOperationException(PrintExceptionMessage(alpha, beta));
                    ShiftedGamma gamma = new ShiftedGamma(alpha, beta, shift);
                    return 1 - gamma.CDF(-x);
                }               
            }
        }
        public double PDF(double x)
        {
            if (Math.Abs(Skewness) < _NoSkewness)
            {
                // a PearsonIII distribution with no skew is normally distributed.
                IDistribution norm = new Normal(Mean, StandardDeviation);
                return norm.PDF(x);
            }
            else
            {
                // a skewed PearsonIII distribution is a shifted gamma distribution
                double shift, alpha = 4d / (Skewness * Skewness), beta = 0.5 * StandardDeviation * Skewness;
                // positively skewed distribution 
                if (Skewness > 0)
                {
                    shift = Mean - 2d * StandardDeviation / Skewness;
                    if (!alpha.IsOnRange(0, double.PositiveInfinity, false, false) || !beta.IsOnRange(0, double.PositiveInfinity)) throw new InvalidOperationException(PrintExceptionMessage(alpha, beta));
                    ShiftedGamma gamma = new ShiftedGamma(alpha, beta, shift);
                    return gamma.PDF(x);
                }
                // negatively skewed distribution
                else
                {
                    beta = -beta;
                    shift = -Mean + 2d * StandardDeviation / Skewness;
                    if (!alpha.IsOnRange(0, double.PositiveInfinity, false, false) || !beta.IsOnRange(0, double.PositiveInfinity)) throw new InvalidOperationException(PrintExceptionMessage(alpha, beta));
                    ShiftedGamma gamma = new ShiftedGamma(alpha, beta, shift);
                    return -gamma.PDF(x);
                }
            }
        }

        public double InverseCDF(double p)
        {
            if (Math.Abs(Skewness) < _NoSkewness)
            {
                // a PearsonIII distribution with no skew is normally distributed.
                IDistribution norm = new Normal(Mean, StandardDeviation);
                return norm.InverseCDF(p);
            }
            else
            {
                // a skewed PearsonIII distribution is a shifted gamma distribution
                double shift, alpha = 4d / (Skewness * Skewness), beta = 0.5 * StandardDeviation * Skewness;
                // positively skewed distribution 
                if (Skewness > 0)
                {
                    shift = Mean - 2d * StandardDeviation / Skewness;
                    if (!alpha.IsOnRange(0, double.PositiveInfinity, false, false) || !beta.IsOnRange(0, double.PositiveInfinity)) throw new InvalidOperationException(PrintExceptionMessage(alpha, beta));
                    ShiftedGamma gamma = new ShiftedGamma(alpha, beta, shift);
                    return gamma.InverseCDF(p);
                }
                // negatively skewed distribution
                else
                {
                    beta = -beta;
                    shift = -Mean + 2d * StandardDeviation / Skewness;
                    if (!alpha.IsOnRange(0, double.PositiveInfinity, false, false) || !beta.IsOnRange(0, double.PositiveInfinity)) throw new InvalidOperationException(PrintExceptionMessage(alpha, beta));
                    ShiftedGamma gamma = new ShiftedGamma(alpha, beta, shift);
                    return -gamma.InverseCDF(1 - p);
                }
            }

        }
        private string PrintExceptionMessage(double alpha, double beta)
        {
            return $"The CDF operation could not be performed." +
                $" The computed parameterization of the Gamma(alpha: {alpha}, beta: {beta}) distribution is invalid. " +
                $"The CDF of the positively skewed {Print()} distribution is computed as the CDF of a Shifted Gamma distribution, " +
                $"which requires positive alpha (e.g. shape): {alpha}, and beta (e.g. rate): {beta} parameter.";
        }
        public string Print() => $"PearsonIII(mean: {Mean}, sd: {StandardDeviation}, skew: {Skewness})";
    }
}
