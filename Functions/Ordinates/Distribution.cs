using Functions.Ordinates;
using Statistics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Utilities;
using Utilities.Serialization;

namespace Functions.Ordinates
{
    internal class Distribution : IDistributedOrdinate, Utilities.IValidate<IDistributedOrdinate>, IOrdinate
    {
        #region Fields
        internal Statistics.IDistribution _Distribution;
        #endregion
        public Statistics.IDistribution Dist
        {
            get { return _Distribution; }
        }
        #region Properties
        public IOrdinateEnum Type
        {
            get
            {
                IOrdinateEnum retval = IOrdinateEnum.NotSupported;
                switch (_Distribution.Type)
                {
                    case IDistributionEnum.Beta4Parameters:
                        {
                            retval = IOrdinateEnum.Beta4Parameters;
                            break;
                        }
                    case IDistributionEnum.Histogram:
                        {
                            retval = IOrdinateEnum.Histogram;
                            break;
                        }
                    case IDistributionEnum.LogPearsonIII:
                        {
                            retval = IOrdinateEnum.LogPearsonIII;
                            break;
                        }
                    case IDistributionEnum.Normal:
                        {
                            retval = IOrdinateEnum.Normal;
                            break;
                        }
                    case IDistributionEnum.NotSupported:
                        {
                            retval = IOrdinateEnum.NotSupported;
                            break;
                        }
                    case IDistributionEnum.Triangular:
                        {
                            retval = IOrdinateEnum.Triangular;
                            break;
                        }
                    case IDistributionEnum.TruncatedBeta4Parameter:
                        {
                            retval = IOrdinateEnum.TruncatedBeta4Parameter;
                            break;
                        }
                    case IDistributionEnum.TruncatedHistogram:
                        {
                            retval = IOrdinateEnum.TruncatedHistogram;
                            break;
                        }
                    case IDistributionEnum.TruncatedNormal:
                        {
                            retval = IOrdinateEnum.TruncatedNormal;
                            break;
                        }
                    case IDistributionEnum.TruncatedTriangular:
                        {
                            retval = IOrdinateEnum.TruncatedTriangular;
                            break;
                        }
                    case IDistributionEnum.TruncatedUniform:
                        {
                            retval = IOrdinateEnum.TruncatedUniform;
                            break;
                        }
                    case IDistributionEnum.Uniform:
                        {
                            retval = IOrdinateEnum.Uniform;
                            break;
                        }
                }
                return retval;
            }
        }
        public double Mean => _Distribution.Mean;
        public double Median => _Distribution.Median;
        public double Mode => _Distribution.Mode;
        public double Variance => _Distribution.Variance;
        public double StandardDeviation => _Distribution.StandardDeviation;
        public double Skewness => _Distribution.Skewness;
        public Utilities.IRange<double> Range => _Distribution.Range;
        public int SampleSize => _Distribution.SampleSize;

        public IMessageLevels State => _Distribution.State; //_distribution.
        public IEnumerable<IMessage> Messages => _Distribution.Messages;
        #endregion

        #region Constructor
        internal Distribution(Statistics.IDistribution distribution)
        {
            if (distribution.IsNull()) throw new Utilities.InvalidConstructorArgumentsException($"The {nameof(Distribution)} cannot be constructed because it is null");
            _Distribution = distribution;
        }
        #endregion

        #region Functions
        
        public IMessageLevels Validate(IValidator<IDistributedOrdinate> validator, out IEnumerable<IMessage> errors)
        {
            throw new NotImplementedException();
        }
        #region IOrdinate Functions
        /* IOrdinate contains the following functions:
         *      (1) double Value(double)
         *      (2) bool Equals(IOrdinate)
         *      (3) string Print(bool)
         * Functions (1) - (2) are implemented in this region. (3) is implemented as part of IDistrubuted Value, since it is also part of the IDistributedValue interface.
         */
        public double Value(double p) => _Distribution.InverseCDF(p);
        public bool Equals(IOrdinate ordinate) => Type == ordinate.Type ? Equals((IDistributedOrdinate)ordinate) : false;
        #endregion

        #region IDistributedValue Functions
        public double CDF(double x)
        {
            return _Distribution.CDF(x);
        }
        public bool Equals(IDistributedOrdinate distribution)
        {
            return _Distribution.Equals(distribution);
        }
        public double InverseCDF(double p)
        {
            return _Distribution.InverseCDF(p);
        }
        public double PDF(double x)
        {
            return _Distribution.PDF(x);
        }
        public string Print(bool round)
        {
            return _Distribution.Print(round);
        }
        //public double Sample()
        //{
        //    return _distribution.Sample(new Random());
        //}

        //public double[] Sample(Random numberGenerator = null)
        //{
        //    return _distribution.Sample(1,numberGenerator);
        //}

        //public double[] Sample(int sampleSize, Random numberGenerator = null)
        //{
        //    return _distribution.Sample(sampleSize, numberGenerator);
        //}

        //public IDistributedValue SampleDistribution(Random numberGenerator = null)
        //{
        //    return new Distribution( _distribution.SampleDistribution(numberGenerator));
        //}

        public XElement WriteToXML()
        {
            XElement ordinateElem = new XElement(SerializationConstants.ORDINATE);
            ordinateElem.SetAttributeValue(SerializationConstants.TYPE, GetSerializationConstantForDistributionType(Type));

            XElement distElement = _Distribution.WriteToXML();

            ordinateElem.Add(distElement);
            return ordinateElem;
            
        }
        private string GetSerializationConstantForDistributionType(IOrdinateEnum type)
        {
            switch(type)
            {
                case IOrdinateEnum.Normal:
                    {
                        return SerializationConstants.NORMAL;
                    }
                case IOrdinateEnum.Triangular:
                    {
                        return SerializationConstants.TRIANGULAR;
                    }
                case IOrdinateEnum.Uniform:
                    {
                        return SerializationConstants.UNIFORM;
                    }
                    //todo: cody finish this out.
                    //case IOrdinateEnum.Beta4Parameters:
                    //    {
                    //        return SerializationConstants.
                    //    }
            }
            throw new NotImplementedException("Finish this code.");
        }
        #endregion
        #endregion
    }
}
