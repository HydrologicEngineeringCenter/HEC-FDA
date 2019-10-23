using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Ordinates
{
    internal class Distribution : IOrdinate
    {
        private Statistics.IDistribution _Distribution;
        internal Distribution(Statistics.IDistribution distribution)
        {
            _Distribution = distribution;
        }
        public Tuple<double, double> Range => new Tuple<double, double>(_Distribution.Minimum, _Distribution.Maximum);

        public bool IsDistributed => true;

        public bool Equals(IOrdinate scalar)
        {
            return scalar.Print().Equals(this.Print());
        }

        public string Print()
        {
            return _Distribution.Print();
        }

        public double Value(double p = 0.5)
        {
            if (p < 0 || p > 1)
            {
                throw new Exception("Probability value must be between 0 and 1");
            }
            else
            {
                return _Distribution.InverseCDF(p);
            }
        }
    }
}
