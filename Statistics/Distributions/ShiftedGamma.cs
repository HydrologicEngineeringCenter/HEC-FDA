using System;

namespace Statistics.Distributions
{
    internal class ShiftedGamma
    {
        private readonly Gamma _Gamma;
        
        internal double Shift { get; }
        
        internal ShiftedGamma(double alpha, double beta, double shift)
        {
            // The Gamma distribution is defined by 2 parameters: 
            //      (1) alpha is the shape parameter 
            //      (2) beta is the scale parameter
            _Gamma = new Gamma(alpha, beta); //Gamma(alpha, 1/beta);
            Shift = shift;
        }

        internal double CDF(double x)
        {
            double val = x - Shift;
            if(Math.Abs(val) < .001)
            {
                val = 0;
            }
            return _Gamma.CDF(val);
        }
        internal double PDF(double x)
        {
            return _Gamma.PDF(x - Shift);
        }

        internal double InverseCDF(double p)
        {
            return _Gamma.InverseCDF(p) + Shift;
        }
    }
}
