using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace Statistics.Distributions
{
    internal class Beta4Parameters: MathNet.Numerics.Distributions.BetaScaled, IDistribution // IOrdinate<IDistribution>
    {
        //TODO: Validation
        //TODO: Cleanup commented out code
        
        #region Properties
        public double StandardDeviation => StdDev;
        public IDistributions Type => IDistributions.Beta4Parameters;
        public int SampleSize { get; }
        //#region IOrdinate Properties
        //public bool IsVariable => true;
        //public Type OrdinateType => typeof(IDistribution);
        //#endregion
        #endregion

        #region Constructor
        public Beta4Parameters(double alpha, double beta, double location, double scale, int sampleSize = int.MaxValue) : base(a: alpha, b: beta, location, scale)
        {
            SampleSize = sampleSize;
        }
        #endregion

        #region Functions
        #region IDistribution Functions
        public double PDF(double x) => Density(x);
        public double CDF(double x) => CumulativeDistribution(x);
        public double InverseCDF(double p) => InverseCumulativeDistribution(p);

        public double Sample(Random r = null) => InverseCDF(r == null ? new Random().NextDouble() : r.NextDouble());
        //public double[] Sample(Random numberGenerator = null) => Sample(SampleSize, numberGenerator);
        public double[] Sample(int sampleSize, Random numberGenerator = null)
        {
            if (numberGenerator == null) numberGenerator = new Random();
            double[] sample = new double[SampleSize];
            for (int i = 0; i < sample.Length; i++) sample[i] = InverseCDF(numberGenerator.NextDouble());
            return sample;
        }
        public IDistribution SampleDistribution(Random numberGenerator = null) => Fit(Sample(SampleSize, numberGenerator));
        public bool Equals(IDistribution distribution) => string.Compare(Print(), distribution.Print()) == 0 ? true : false;
        public string Print() => $"ScaledBeta(alpha: {base.A}, beta: {base.B}, range: [{Minimum}, {Maximum}], sample size: {SampleSize})";
        #endregion
        //#region IOrdinate Functions
        //public double GetValue(double sampleProbability = 0.5) => InverseCDF(sampleProbability);
        //public bool Equals<T>(IOrdinate<T> ordinate) => ordinate.OrdinateType == typeof(IDistribution) ? Equals((IDistribution)ordinate) : false;
        //#endregion        
        public static Beta4Parameters Fit(IEnumerable<double> data)
        {
            if (data.IsNullOrEmpty()) throw new ArgumentException("The provided data is invalid because it is null or an empty.");
            double skew = data.Skewness(), kurtosis = data.Kurtosis();
            /*
             * All Beta distributions are bound the following two lines which form a plane: 
             *      
             *      skewness^2
             *      kurtosis (or excess kurtosis)
             * 
             * Any parameterization of distribution alpha and beta parameters outside of this plane is impossible. 
             */
            Tuple<double, double, bool> bounds = IsBound(skew, kurtosis);
            if (bounds.Item3 == false) throw new ArgumentOutOfRangeException("The provided data can not be appoximated by a beta distribution");
            else
            {
                double bound1 = bounds.Item1, bound2 = bounds.Item2;
                /* 
                 * The 4Parameter Beta has the following for parameters (alpha, beta, left (aka a) and right (aka c):
                 * (note: alpha, beta are exponential shape parameters ... left (aka a) and right (aka c) represent min and max bound)
                 * 
                 *      - Sample Size can be estimated as follows -
                 *      sampleSize = alpha + beta = 3(excessKurtosis - skewness^2  + 2) / (3/2)skewness^2 - excessKurtosis
                 *      
                 *      - To simplify later equations this quantity is presented seperately here -
                 *      intermediateParameterQuantity = (1 + ( 16 * (sampleSize + 1) / (sampleSize + 2)^2 * skewness^2 )^1/2
                 * 
                 *       alpha = (1/2) * sampleSize * ( 1 + 1 / innerQuantity )
                 *       beta = (1/2) * sampleSize * ( 1 - 1 / innerQuantity )
                 */
                double sampleSize = 3 * bound1 / bound2;
                double intermediateParameterQuantity = Math.Sqrt(1 + (16 * (sampleSize + 1) / (sampleSize + 2) * (sampleSize + 2) * skew * skew));
                double alpha = 0.5 * sampleSize * (1 + 1 / intermediateParameterQuantity), beta = 0.5 * sampleSize * (1 - 1 / intermediateParameterQuantity);
                if (!(alpha > 0 && beta > 0)) throw new ArgumentException("The provided data can not be approximated by a beta distribution");
                else
                {
                    MathNet.Numerics.Distributions.Beta fit = new MathNet.Numerics.Distributions.Beta(alpha, beta);
                    /*
                     * The 4Parameter Beta is bound by on its left and right side by:
                     * (note: these left and right bounds are often referred to as a, c respectively)
                     * 
                     *      - To simplify later equations these quantities are presented seperately here - 
                     *      intermediateRangeQuantity = ((sampleSize + 2)^2 * skew^2 + 16 * (sampleSize + 1))^2
                     *                          range = standardDeviation / 2 * intermediateRangeQuantity
                     *       
                     *      left = mean - alpha / sampleSize * range
                     *      right = left + range 
                     */
                    double standardDeviation = data.StandardDeviation(), mean = data.Mean();
                    double intermediateRangeQuantity = Math.Sqrt((sampleSize + 2) * (sampleSize + 2) * skew * skew + 16 * (sampleSize + 1));
                    double range = standardDeviation / 2 * intermediateRangeQuantity;
                    double left = mean - alpha / sampleSize * range;
                    double right = range + alpha;
                    /*      
                     * The 4Parameter Beta distribution properties:
                     *      - measures of central tendency (mean, median and mode) are scaled by the range (c - a) and shifted by a (i.e. the minimum)
                     *      - measures of dispersion are scaled by the range (c - a) but are already centered on the mean (and therefore need not be shifted)
                     *      - skewness and kurtosis are non-dimensional and therefore require no transformation (i.e. they are normalized by (proportional to) standard deviation 
                     */
                    double location = fit.Mean * (right - left) + left, variance = fit.Variance * Math.Pow(right - left, 2);
                    return new Beta4Parameters(alpha, beta, location, Math.Pow(variance, 1 / 2), data.Count());
                }
            }
        }
        private static Tuple<double, double, bool> IsBound(double skew, double kurtosis)
        {
            /* 
             * All Beta Distribution are bound by skew and kurtosis: 
             *      skewness^2 + 1 < kurtosis < (3/2)skewness^2 + 3
             * or...
             *      0 < kurtosis - skewness^2 - 1 
             * and...
             *      0 < (3/2)skewness^2 + 3 - kurtosis
             * so... 
             *      bound1: kurtosis - skewness^2 - 1
             *      bound2: (3/2)skewness^2 + 3 - kurtosis
             */
            double bound1 = kurtosis - skew * skew - 1, bound2 = 1.5 * skew * skew + 3 - kurtosis;
            return (bound1 < 0.00001 || bound2 < 0.00001) ? new Tuple<double, double, bool>(bound1, bound2, false) : new Tuple<double, double, bool>(bound1, bound2, true);
        }

        public IDistribution Read(string xmlString)
        {
            throw new NotImplementedException();
        }

        public string WriteToXML()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
