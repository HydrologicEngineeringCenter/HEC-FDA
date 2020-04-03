using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Utilities;
using Utilities.Serialization;

namespace Statistics.Distributions
{
    internal class Beta4Parameters: IDistribution, IValidate<Beta4Parameters> // IOrdinate<IDistribution>
    {
        private readonly MathNet.Numerics.Distributions.BetaScaled _Distribution;
        
        #region Properties
        public double Mean { get; }
        public double Median { get; }
        public double Mode { get; }
        public double Variance { get; }
        public double StandardDeviation { get; }
        public double Skewness { get; }
        public IDistributionEnum Type => IDistributionEnum.Beta4Parameters;
        public IRange<double> Range { get; }
        public int SampleSize { get; }
        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }
        #endregion

        #region Constructor
        public Beta4Parameters(double alpha, double beta, double location, double scale, int sampleSize = int.MaxValue)
        {
            if (Validation.Beta4ParameterValidator.IsConstructable(alpha, beta, location, scale, sampleSize, out string error)) throw new InvalidConstructorArgumentsException(error);
            else
            {
                _Distribution = new MathNet.Numerics.Distributions.BetaScaled(alpha, beta, location, scale);
                Mean = _Distribution.Mean;
                Median = _Distribution.InverseCumulativeDistribution(0.50); //_Distribution.Median;
                Mode = _Distribution.Mode;
                Variance = _Distribution.Variance;
                StandardDeviation = _Distribution.StdDev;
                Skewness = _Distribution.Skewness;
                Range = IRangeFactory.Factory(_Distribution.Minimum, _Distribution.Maximum);
                SampleSize = sampleSize;
                State = Validate(new Validation.Beta4ParameterValidator(), out IEnumerable<IMessage> msgs);
                Messages = msgs;
            }   
        }
        #endregion

        #region Functions
        public IMessageLevels Validate(IValidator<Beta4Parameters> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }
        
        #region IDistribution Functions
        public double PDF(double x) => _Distribution.Density(x);
        public double CDF(double x) => _Distribution.CumulativeDistribution(x);
        public double InverseCDF(double p) => _Distribution.InverseCumulativeDistribution(p);

        //public double Sample(Random r = null) => InverseCDF(r == null ? new Random().NextDouble() : r.NextDouble());
        ////public double[] Sample(Random numberGenerator = null) => Sample(SampleSize, numberGenerator);
        //public double[] Sample(int sampleSize, Random numberGenerator = null)
        //{
        //    if (numberGenerator == null) numberGenerator = new Random();
        //    double[] sample = new double[SampleSize];
        //    for (int i = 0; i < sample.Length; i++) sample[i] = InverseCDF(numberGenerator.NextDouble());
        //    return sample;
        //}
        //public IDistribution SampleDistribution(Random numberGenerator = null) => Fit(Sample(SampleSize, numberGenerator));
        public bool Equals(IDistribution distribution) => string.Compare(Print(), distribution.Print()) == 0 ? true : false;
        public string Print(bool round = false) => round ? Print(_Distribution.A, _Distribution.B, Range.Min, (Range.Max - Range.Min), SampleSize): $"ScaledBeta(alpha: {_Distribution.A}, beta: {_Distribution.B}, range: [{_Distribution.Location}, {(_Distribution.Location + _Distribution.Scale).Print()}], sample size: {SampleSize})";
        public string Requirements(bool printNotes) => RequiredParameterization(printNotes);
        #endregion

        internal static string Print(double alpha, double beta, double location, double scale, int n) => $"ScaledBeta(alpha: {alpha.Print()}, beta: {beta.Print()}, range: [{location.Print()}, {(location + scale).Print()}], sample size: {n.Print()})";
        //The Unicode character \u2265 should produce a greater than or equal to character.
        internal static string RequiredParameterization(bool printNotes = true)
        {
            string msg = $"The Scaled Beta distribution requires the following parameterization: {Parameterization()}.";
            if (printNotes) msg += " " + RequirementNotes();
            return msg;
        }
        internal static string Parameterization() => $"ScaledBeta(alpha: [0, {double.MaxValue.Print()}], beta: [0, {double.MaxValue.Print()}], {Validation.Resources.DoubleRangeRequirements()}, sample size: > 0)";
        internal static string RequirementNotes() => $"The range parameter is sometimes expressed in terms of location and scale, where location equals the range minimum and scale equals the range maximum minus the range minimum.";

        public static Beta4Parameters Fit(IEnumerable<double> sample)
        {
            // These are the 4 parameters we attempt to fit:
            double alpha, beta, min, max;
            var data = sample.IsNullOrEmpty() ? throw new ArgumentNullException(nameof(sample)) : IDataFactory.Factory(sample);
            var stats = data.Elements.IsNullOrEmpty() || data.Elements.Count() < 5 ? throw new ArgumentException($"The {nameof(sample)} data is invalid because it contains an insufficient number of finite numeric elements (at least 5 are required only {data.Elements.Count()} were provided).") :  ISampleStatisticsFactory.Factory(data);
            if (!(stats.State < IMessageLevels.Error)) throw new ArgumentException($"The 4 Parameter Data Distribution cannot be created because the provided sample data is invalid and contains the following errors: {stats.Messages.PrintTabbedListOfMessages()}");
            else
            {
                /* This attempts to follow the Beta Distribution 4 unknown parameters (alpha, beta, min, max)
                 * method of moments estimation routine described on Wikipedia:
                 *      https://en.wikipedia.org/wiki/Beta_distribution
                 * A fundamental concept is that the 4 parameter Beta distribution kurtosis is bound by the square of skewness.
                 * The relationship between these two properties determine the shape of the distribution which can appear uniform, normal, skewed, etc.
                 */
                double excessKurtosis = stats.Kurtosis; // - 3;
                double squareSkewness = stats.Skewness * stats.Skewness;
                if (squareSkewness - 2 < excessKurtosis && excessKurtosis < 1.5 * squareSkewness)
                {
                    double v;
                    if (stats.Skewness == 0)
                    {
                        /* 0 skewness simplifies the equation in the else block and results in:
                         *      alpha = beta = v / 2     
                         */
                        alpha = (1.5 * excessKurtosis + 3) / -1 * excessKurtosis;
                        beta = alpha;
                        v = beta * 2;
                    }
                    else
                    {
                        // v = alpha + beta
                        v = 3 * (excessKurtosis - squareSkewness + 2) / (1.5 * squareSkewness - excessKurtosis);
                        double denominator = Math.Sqrt(1 + (16 * (v + 1)) / ((v + 2) * (v + 2) * squareSkewness));
                        double shapePlus = v * 0.5 * (1 + 1 / denominator), shapeMinus = v * 0.5 * (1 - 1 / denominator);
                        if (stats.Skewness > 0)
                        {
                            alpha = Math.Min(shapePlus, shapeMinus);
                            beta = Math.Max(shapePlus, shapeMinus);
                        }
                        else
                        {
                            // In this case: skewness < 0
                            alpha = Math.Max(shapePlus, shapeMinus);
                            beta = Math.Min(shapePlus, shapeMinus);
                        }
                    }
                    // another option for range = standard deviation * sqrt(6 + (5 * v) + ((v + 2) * (v + 3)) / 6 * excess kurtosis
                    double range = stats.StandardDeviation * 0.5 * Math.Sqrt((v + 2) * (v + 2) * squareSkewness + 16 * (v + 1));
                    min = stats.Mean - (alpha / v) * range;
                    max = range + min;
                }
                else
                {
                    /* Distribution parameters in this block have surpassed a boundary conditions:
                     *      (1) square skewness - 2 >= excess kurtosis is and invalid result.
                     *              - The 'Impossible Boundary Line' is formed where: 
                     *              excess kurtosis = square skewness - 2 
                     *              (e.g. excess kurtosis + 2 - square skewness = 0)
                     *      (2) excess kurtosis >= 1.5 * square skewness is an invalid result.
                     *             - The 'Gamma Line' is formed where:
                     *             excess kurtosis = 1.5 * square skewness
                     *             (e.g. excess kurtosis - 1.5 * square skewness = 0)
                     *             this is where the denominator approaches infinity (and therefore has been described as dangerously near to chaos)
                     */
                    throw new ArgumentException("The provided data cannot be approximated by a 4 parameter beta distribution with 4 unknown parameters.");
                }
            }
            return new Beta4Parameters(alpha, beta, min, (max - min));
        }

        //public static Beta4Parameters Fit(IEnumerable<double> data)
        //{
        //    if (data.IsNullOrEmpty()) throw new ArgumentException("The provided data is invalid because it is null or an empty.");
        //    double skew = data.Skewness(), kurtosis = data.Kurtosis();
        //    /*
        //     * All Beta distributions are bound the following two lines which form a plane: 
        //     *      
        //     *      skewness^2
        //     *      kurtosis (or excess kurtosis)
        //     * 
        //     * Any parameterization of distribution alpha and beta parameters outside of this plane is impossible. 
        //     */
        //    Tuple<double, double, bool> bounds = IsBound(skew, kurtosis);
        //    if (bounds.Item3 == false) throw new ArgumentOutOfRangeException("The provided data can not be appoximated by a beta distribution");
        //    else
        //    {
        //        double bound1 = bounds.Item1, bound2 = bounds.Item2;
        //        /* 
        //         * The 4Parameter Beta has the following for parameters (alpha, beta, left (aka a) and right (aka c):
        //         * (note: alpha, beta are exponential shape parameters ... left (aka a) and right (aka c) represent min and max bound)
        //         * 
        //         *      - Sample Size can be estimated as follows -
        //         *      sampleSize = alpha + beta = 3(excessKurtosis - skewness^2  + 2) / (3/2)skewness^2 - excessKurtosis
        //         *      
        //         *      - To simplify later equations this quantity is presented seperately here -
        //         *      intermediateParameterQuantity = (1 + ( 16 * (sampleSize + 1) / (sampleSize + 2)^2 * skewness^2 )^1/2
        //         * 
        //         *       alpha = (1/2) * sampleSize * ( 1 + 1 / innerQuantity )
        //         *       beta = (1/2) * sampleSize * ( 1 - 1 / innerQuantity )
        //         */
        //        double sampleSize = 3 * bound1 / bound2;
        //        double intermediateParameterQuantity = Math.Sqrt(1 + (16 * (sampleSize + 1) / (sampleSize + 2) * (sampleSize + 2) * skew * skew));
        //        double alpha = 0.5 * sampleSize * (1 + 1 / intermediateParameterQuantity), beta = 0.5 * sampleSize * (1 - 1 / intermediateParameterQuantity);
        //        if (!(alpha > 0 && beta > 0)) throw new ArgumentException($"The data can not be approximated by a beta distribution because beta distributions are defined by 2 positive exponential shape parameters, referred to as alpha and beta. In this case: Beta(alpha: {alpha}, beta: {beta}) the specified or computed parameters are invalid.");
        //        else
        //        {
        //            MathNet.Numerics.Distributions.Beta fit = new MathNet.Numerics.Distributions.Beta(alpha, beta);
        //            /*
        //             * The 4Parameter Beta is bound by on its left and right side by:
        //             * (note: these left and right bounds are often referred to as a, c respectively)
        //             * 
        //             *      - To simplify later equations these quantities are presented seperately here - 
        //             *      intermediateRangeQuantity = ((sampleSize + 2)^2 * skew^2 + 16 * (sampleSize + 1))^2
        //             *                          range = standardDeviation / 2 * intermediateRangeQuantity
        //             *       
        //             *      left = mean - alpha / sampleSize * range
        //             *      right = left + range 
        //             */
        //            double standardDeviation = data.StandardDeviation(), mean = data.Mean();
        //            double intermediateRangeQuantity = Math.Sqrt((sampleSize + 2) * (sampleSize + 2) * skew * skew + 16 * (sampleSize + 1));
        //            double range = standardDeviation / 2 * intermediateRangeQuantity;
        //            double left = mean - alpha / sampleSize * range;
        //            double right = range + alpha;
        //            /*      
        //             * The 4Parameter Beta distribution properties:
        //             *      - measures of central tendency (mean, median and mode) are scaled by the range (c - a) and shifted by a (i.e. the minimum)
        //             *      - measures of dispersion are scaled by the range (c - a) but are already centered on the mean (and therefore need not be shifted)
        //             *      - skewness and kurtosis are non-dimensional and therefore require no transformation (i.e. they are normalized by (proportional to) standard deviation 
        //             */
        //            double location = fit.Mean * (right - left) + left, variance = fit.Variance * Math.Pow(right - left, 2);
        //            // I may be over thinking the equation above. location may be a and a - c may be the scale. 
        //            return new Beta4Parameters(alpha, beta, location, Math.Pow(variance, 1 / 2), data.Count());
        //        }
        //    }
        //}
        //private static Tuple<double, double, bool> IsBound(double skew, double kurtosis)
        //{
        //    /* 
        //     * All Beta Distribution are bound by skew and kurtosis: 
        //     *      skewness^2 + 1 < kurtosis < (3/2)skewness^2 + 3
        //     * or...
        //     *      0 < kurtosis - skewness^2 - 1 
        //     * and...
        //     *      0 < (3/2)skewness^2 + 3 - kurtosis
        //     * so... 
        //     *      bound1: kurtosis - skewness^2 - 1
        //     *      bound2: (3/2)skewness^2 + 3 - kurtosis
        //     */
        //    double bound1 = kurtosis - skew * skew - 1, bound2 = 1.5 * skew * skew + 3 - kurtosis;
        //    return (bound1 < 0.00001 || bound2 < 0.00001) ? new Tuple<double, double, bool>(bound1, bound2, false) : new Tuple<double, double, bool>(bound1, bound2, true);
        //}

        public string WriteToXML()
        {
            throw new NotImplementedException();
        }
        XElement ISerializeToXML<IDistribution>.WriteToXML()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
