using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MathNet.Numerics.Statistics;

using Utilities;

namespace Statistics
{
    public static class IDistributionFactory
    {
        //TODO: Validate
        //TODO: Call other constructors with inputs and IDistributions Enum (may require switch case on enum values) 
        
       

        public static IDistribution Fit(IEnumerable<double> sample, IDistributions returnType)
        {
            if ((int)returnType >= 10)
            {
                SummaryStatistics stats = new SummaryStatistics(IDataFactory.Factory(sample));
                return Fit(sample, stats.Minimum, stats.Maximum, returnType);
            }
            else
            {
                switch (returnType)
                {
                    case IDistributions.Normal:
                        return Distributions.Normal.Fit(sample);
                    case IDistributions.Uniform:
                        return Distributions.Uniform.Fit(sample);
                    case IDistributions.Beta4Parameters:
                        return Distributions.Beta4Parameters.Fit(sample);
                    case IDistributions.Triangular:
                        return Distributions.Triangular.Fit(sample);
                    case IDistributions.Histogram:
                        return (IDistribution)IHistogramFactory.Factory(IDataFactory.Factory(sample), nBins: 100);
                    case IDistributions.LogPearsonIII:
                        return Distributions.LogPearsonIII.Fit(sample);
                    default:
                        throw new NotImplementedException($"An unexpected error occured. The requested return type: {returnType} is unsupported");
                }
            }          
        }
        public static IHistogram Fit(IEnumerable<double> sample, int nBins)
        {
            return IHistogramFactory.Factory(IDataFactory.Factory(sample), nBins);
        }
        public static IDistribution Fit(IEnumerable<double> sample, double minimum, double maximum, IDistributions returnType)
        {
            if ((int)returnType < 10)
            {
                return Fit(sample, returnType);
            }
            else
            {
                IDistribution distribution = (IDistribution)Fit(sample, (int)returnType / 10);
                return new Distributions.TruncatedDistribution(distribution, minimum, maximum);
            }
        }

        public static IDistribution FactoryNormal(double mean, double stDev, int sample = 2147483647)
        {
            return new Distributions.Normal(mean, stDev, sample);
        }

        public static IDistribution FactoryTriangular(double min, double mostLikely, double max, int sample = 2147483647)
        {
            return new Distributions.Triangular(min, mostLikely, max, sample);
        }

        public static IDistribution FactoryUniform(double min, double max, int sample = 2147483647)
        {
            return new Distributions.Uniform(min, max, sample);
        }

        public static IDistribution FactoryTruncatedNormal(double mean, double stDev, double min, double max, int sample = 2147483647)
        {
            IDistribution normal = new Distributions.Normal(mean, stDev, sample);
            return new Distributions.TruncatedDistribution(normal, min, max);
        }

        public static IDistribution FactoryBeta(double alpha, double beta, double min, double max)
        {
            return new Distributions.Beta4Parameters(alpha, beta, min, max); //todo this isn't correct. Shouldn't be min and max
        }
    }
}
