using Statistics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    internal class DistributedValue : IDistributedValue
    {
        private Statistics.IDistribution _distribution;
        internal DistributedValue(Statistics.IDistribution distribution)
        {
            _distribution = distribution;
        }

        public DistributionType Type
        {
            get
            {
                DistributionType retval = DistributionType.NotSupported;
                switch (_distribution.Type)
                {
                    case IDistributions.Beta4Parameters:
                        {
                            retval = DistributionType.Beta4Parameters;
                            break;
                        }
                    case IDistributions.Histogram:
                        {
                            retval = DistributionType.Histogram;
                            break;
                        }
                    case IDistributions.LogPearsonIII:
                        {
                            retval = DistributionType.LogPearsonIII;
                            break;
                        }
                    case IDistributions.Normal:
                        {
                            retval = DistributionType.Normal;
                            break;
                        }
                    case IDistributions.NotSupported:
                        {
                            retval = DistributionType.NotSupported;
                            break;
                        }
                    case IDistributions.Triangular:
                        {
                            retval = DistributionType.NotSupported;
                            break;
                        }
                    case IDistributions.TruncatedBeta4Parameter:
                        {
                            retval = DistributionType.TruncatedBeta4Parameter;
                            break;
                        }
                    case IDistributions.TruncatedHistogram:
                        {
                            retval = DistributionType.TruncatedHistogram;
                            break;
                        }
                    case IDistributions.TruncatedNormal:
                        {
                            retval = DistributionType.TruncatedNormal;
                            break;
                        }
                    case IDistributions.TruncatedTriangular:
                        {
                            retval = DistributionType.TruncatedTriangular;
                            break;
                        }
                    case IDistributions.TruncatedUniform:
                        {
                            retval = DistributionType.TruncatedUniform;
                            break;
                        }
                    case IDistributions.Uniform:
                        {
                            retval = DistributionType.Uniform;
                            break;
                        }
                }
                return retval;
            }
        }

        public double Mean => _distribution.Mean;

        public double Median => _distribution.Median;

        public double Variance => _distribution.Variance;

        public double StandardDeviation => _distribution.StandardDeviation;

        public double Skewness => _distribution.Skewness;

        public double Minimum => _distribution.Minimum;

        public double Maximum => _distribution.Maximum;

        public int SampleSize => _distribution.SampleSize;

        public double CDF(double x)
        {
            return _distribution.CDF(x);
        }

        public bool Equals(IDistributedValue distribution)
        {
            return _distribution.Equals(distribution);
        }

        public double InverseCDF(double p)
        {
            return _distribution.InverseCDF(p);
        }

        public double PDF(double x)
        {
            return _distribution.PDF(x);
        }

        public string Print()
        {
            return _distribution.Print();
        }

        public double Sample()
        {
            return _distribution.Sample();
        }

        public double[] Sample(Random numberGenerator = null)
        {
            return _distribution.Sample(numberGenerator);
        }

        public double[] Sample(int sampleSize, Random numberGenerator = null)
        {
            return _distribution.Sample(sampleSize, numberGenerator);
        }

        public IDistributedValue SampleDistribution(Random numberGenerator = null)
        {
            return new DistributedValue( _distribution.SampleDistribution(numberGenerator));
        }
    }
}
