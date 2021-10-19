using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utilities;

namespace Statistics.Distributions
{
    internal class Deterministic : IDistribution
    {
        #region IDistribution Properties
        public IDistributionEnum Type => IDistributionEnum.Deterministic;

        public double Mean { get; }

        public double Median { get; }

        public double Mode { get; }

        public double Variance { get; }

        public double StandardDeviation { get; }

        public double Skewness { get; }

        public IRange<double> Range { get; }

        public int SampleSize { get; }

        public IMessageLevels State => throw new NotImplementedException();

        public IEnumerable<IMessage> Messages => throw new NotImplementedException();
        #endregion
        public double Value { get; }
        #region constructor
        public Deterministic(double x)
        {
            Value = x;
            Mean = x;
            Median = x;
            Mode = x;
            Variance = 0;
            StandardDeviation = 0;
            Skewness = 0;
            SampleSize = 1;
            Range = IRangeFactory.Factory(x, x);
        }
        #endregion

        #region functions
        public double CDF(double x)
        {
            if (x>=Value)
            {
                return 1;
            } else
            {
                return 0;
            }
        }

        public bool Equals(IDistribution distribution)
        {
            throw new NotImplementedException();
        }

        public double InverseCDF(double p)
        {
            return Value;
        }

        public double PDF(double x)
        {
            if (x==Value)
            {
                return 1;
            } else
            {
                return 0;
            }
        }

        public string Print(bool round = false)
        {
            return $"The Value is {Value}";
        }

        public string Requirements(bool printNotes)
        {
            return "A value is required";
        }

        public String WriteToXML()
        {
                return $"{Value}";
            
        }

        XElement ISerializeToXML<IDistribution>.WriteToXML()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
