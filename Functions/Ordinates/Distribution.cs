using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Utilities.Serialization;

namespace Functions.Ordinates
{
    public class Distribution : IOrdinate
    {
        public IDistributedValue GetDistribution { get; }
        internal Distribution(IDistributedValue distribution)
        {
            GetDistribution = distribution;
        }
        public Tuple<double, double> Range => new Tuple<double, double>(GetDistribution.Minimum, GetDistribution.Maximum);

        public bool IsDistributed => true;

        public bool Equals(IOrdinate scalar)
        {
            return scalar.Print().Equals(this.Print());
        }

        public string Print()
        {
            return GetDistribution.Print();
        }

        public double Value(double p = 0.5)
        {
            if (p < 0 || p > 1)
            {
                throw new Exception("Probability value must be between 0 and 1");
            }
            else
            {
                return GetDistribution.InverseCDF(p);
            }
        }

        public XElement WriteToXML()
        {     
            XElement distElem = GetDistribution.WriteToXML();

            XElement ordinateElem = new XElement(SerializationConstants.ORDINATE);
            ordinateElem.SetAttributeValue(SerializationConstants.TYPE, SerializationConstants.DISTRIBUTION);

            ordinateElem.Add(distElem);

            return ordinateElem;
        }
    }
}
