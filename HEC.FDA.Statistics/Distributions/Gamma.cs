using System;

namespace Statistics.Distributions
{
    class Gamma 
    {
        #region fields
        private double _Shape;
        private double _Scale;
        private const double DX = 1E-10;
        #endregion

        #region Constructors
        public Gamma(double shape, double scale)
        {
            _Shape = shape;
            _Scale = scale;
        }
        #endregion


        #region Methods
        public double CDF(double x)
        {
            if (x <= 0)
            {
                return 0;
            } else if(x >= double.MaxValue)
            {
                return 1;
            }
            else
            {
                return SpecialFunctions.regIncompleteGamma(_Shape, x / _Scale);
            }
        }

        public double InverseCDF(double p)
        {
            if (p <= 0.0)
            {
                p = 0.0000000000001;
            }
            else if (p >= 1.0)
            {
                p = .99999999999999;
            }

            double xMin = 0.0;
            double xMax = 1.0;
            for (int j = 0; j < 100; j++)
            {
                double pMax = CDF(xMax);
                if (pMax > p)
                {
                    return invCDFNewtonBiSearch(p, xMin, xMax, 1E-12, 100);
                }
                xMax *= 2.0;
            }
            return double.NaN;

            
        }

        public double PDF(double x)
        {
            if (x <= 0.0) return 0.0;
            return Math.Exp(-x / _Scale + (_Shape - 1.0) * Math.Log(x) -
                    _Shape * Math.Log(_Scale) - SpecialFunctions.logGamma(_Shape));
        }

        #endregion

        #region Supporting Methods
        private double invCDFNewtonBiSearch(double p, double xMin, double xMax, double tolX, int maxIter)
        {
            return newtonBiSearch(p, xMin, xMax, tolX, maxIter);
        }

        private double f(double x)
            {
                return CDF(x);
            }
            
    
        private double dfdx(double x)
            {
                return PDF(x);
            }
    
    //********** GENERAL COMBINATION NEWTON / BISECTION SEARCH FORMULA **********
    /**
     * @param f = a real function, a method with a double parameter that returns a double
     * @param dfdx = derivative of f, a method with a double parameter that returns a double
     * @param y = the right hand side of the equation
     * @param xGuess = initial guess for the iterative solver
     * @return a value corresponding to the solution x of f(x)=y
     *         found using the Newton method and bisection searchin the interval [xMin,xMax]
     *         It is required that f(xMax)-y and f(xMin)-y have opposite signs.
     */
    private double newtonBiSearch(double y, double xMin, double xMax, double tolX, int maxIter)
        { 
            int j;
            double dfrts, dx, dxold, frts, fh, fl;
            double temp, xh, xl, rts, rtsOld;
            fl = f(xMin) - y;
            fh = f(xMax) - y;
            if (fl == 0.0)
            {
                return xMin;
            }
            if (fh == 0.0)
            {
                return xMax;
            }
            if (fl < 0.0)
            { //Orient the search so that f(xl) < 0.
                xl = xMin;
                xh = xMax;
            }
            else
            {
                xh = xMin;
                xl = xMax;
            }
            rtsOld = xl;
            rts = 0.5 * (xMin + xMax); //Initialize the guess for root,
            dxold = Math.Abs(xMax - xMin); //the stepsize before last,
            dx = dxold; //and the last step.
            frts = f(rts) - y;
            dfrts = dfdx(rts);
            for (j = 1; j <= maxIter; j++)
            { //Loop over allowed iterations.
                if ((((rts - xh) * dfrts - frts) * ((rts - xl) * dfrts - frts) > 0.0) //Bisect if Newton out of range,
                || (Math.Abs(2.0 * frts) > Math.Abs(dxold * dfrts)))
                {      //or not decreasing fast enough.
                    dxold = dx;
                    dx = 0.5 * (xh - xl);
                    rts = xl + dx;
                    if (xl == rts || rts == rtsOld)
                    {
                        return rts; //Change in root is negligible.
                    }
                    //System.out.println("Bisect: j = "+j+" rts = "+rts);
                }
                else
                { //Newton step acceptable. Take it.
                    dxold = dx;
                    dx = frts / dfrts;
                    temp = rts;
                    rts -= dx;
                    if (temp == rts)
                    {
                        return rts;
                    }
                    //System.out.println("Newton: j = "+j+" rts = "+rts);
                }

                rtsOld = rts;

                if (Math.Abs(dx) < tolX)
                {
                    return rts; //Convergence criterion.
                }
                frts = f(rts) - y;
                dfrts = dfdx(rts);
                //The one new function evaluation per iteration.
                if (frts < 0.0)
                { //Maintain the bracket on the root.
                    xl = rts;
                }
                else
                {
                    xh = rts;
                }
            }
            return double.NaN;
    }
    #endregion
}
}