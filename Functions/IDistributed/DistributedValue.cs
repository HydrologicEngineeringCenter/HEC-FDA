using Statistics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Utilities;

namespace Functions
{
    internal class DistributedValue : IDistributedValue, Utilities.IValidate<IDistributedValue>
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
                    case IDistributionEnum.Beta4Parameters:
                        {
                            retval = DistributionType.Beta4Parameters;
                            break;
                        }
                    case IDistributionEnum.Histogram:
                        {
                            retval = DistributionType.Histogram;
                            break;
                        }
                    case IDistributionEnum.LogPearsonIII:
                        {
                            retval = DistributionType.LogPearsonIII;
                            break;
                        }
                    case IDistributionEnum.Normal:
                        {
                            retval = DistributionType.Normal;
                            break;
                        }
                    case IDistributionEnum.NotSupported:
                        {
                            retval = DistributionType.NotSupported;
                            break;
                        }
                    case IDistributionEnum.Triangular:
                        {
                            retval = DistributionType.Triangular;
                            break;
                        }
                    case IDistributionEnum.TruncatedBeta4Parameter:
                        {
                            retval = DistributionType.TruncatedBeta4Parameter;
                            break;
                        }
                    case IDistributionEnum.TruncatedHistogram:
                        {
                            retval = DistributionType.TruncatedHistogram;
                            break;
                        }
                    case IDistributionEnum.TruncatedNormal:
                        {
                            retval = DistributionType.TruncatedNormal;
                            break;
                        }
                    case IDistributionEnum.TruncatedTriangular:
                        {
                            retval = DistributionType.TruncatedTriangular;
                            break;
                        }
                    case IDistributionEnum.TruncatedUniform:
                        {
                            retval = DistributionType.TruncatedUniform;
                            break;
                        }
                    case IDistributionEnum.Uniform:
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
        public Utilities.IRange<double> Range => _distribution.Range;
        //public double Minimum => _distribution.Range.MIn;
        //public double Maximum => _distribution.Maximum;
        public int SampleSize => _distribution.SampleSize;
        public bool IsValid => throw new NotImplementedException(); //_distribution.

        public double Mode => _distribution.Mode;

        public IEnumerable<IMessage> Messages => throw new NotImplementedException();

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
            return _distribution.Sample(new Random());
        }

        public double[] Sample(Random numberGenerator = null)
        {
            return _distribution.Sample(1,numberGenerator);
        }

        public double[] Sample(int sampleSize, Random numberGenerator = null)
        {
            return _distribution.Sample(sampleSize, numberGenerator);
        }

        public IDistributedValue SampleDistribution(Random numberGenerator = null)
        {
            return new DistributedValue( _distribution.SampleDistribution(numberGenerator));
        }

        public bool Validate(IValidator<IDistributedValue> validator, out IEnumerable<IMessage> errors)
        {
            throw new NotImplementedException();
        }

        public XElement WriteToXML()
        {
            return _distribution.WriteToXML();
            
        }
    }
}
