using System;

namespace Statistics.Distributions
{
    public class ShiftedGamma
    {
        private readonly Gamma _Gamma;

        public double Shift { get; }

        public ShiftedGamma(double alpha, double beta, double shift)
        {
            // The Gamma distribution is defined by 2 parameters: 
            //      (1) alpha is the shape parameter 
            //      (2) beta is the scale parameter
            _Gamma = new Gamma(alpha, beta); //Gamma(alpha, 1/beta);
            Shift = shift;
        }

        public double CDF(double x)
        {
            double val = x - Shift;
            if(Math.Abs(val) < .001)
            {
                val = 0;
            }
            return _Gamma.CDF(val);
        }
        public double PDF(double x)
        {
            return _Gamma.PDF(x - Shift);
        }

        public double InverseCDF(double p)
        {
            return _Gamma.InverseCDF(p) + Shift;
        }
    }
}
