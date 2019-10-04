using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                SummaryStatistics stats = new SummaryStatistics(sample);
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
                        return Histograms.Histogram.Fit(sample);
                    case IDistributions.LogPearsonIII:
                        return Distributions.LogPearsonIII.Fit(sample);
                    default:
                        throw new NotImplementedException($"An unexpected error occured. The requested return type: {returnType} is unsupported");
                }
            }          
        }
        public static IHistogram Fit(IEnumerable<double> sample, int nBins)
        {
            return Histograms.Histogram.Fit(sample, nBins);
        }
        public static IDistribution Fit(IEnumerable<double> sample, double minimum, double maximum, IDistributions returnType)
        {
            if ((int)returnType < 10)
            {
                return Fit(sample, returnType);
            }
            else
            {
                IDistribution distribution = Fit(sample, (int)returnType / 10);
                return new Distributions.TruncatedDistribution(distribution, minimum, maximum);
            }
        }
    }
}
