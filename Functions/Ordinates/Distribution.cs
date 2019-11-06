using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Ordinates
{
    public class Distribution : IOrdinate
    {
        public Statistics.IDistribution GetDistribution { get; }
        internal Distribution(Statistics.IDistribution distribution)
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
    }
}
