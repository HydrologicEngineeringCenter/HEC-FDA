using System;
using System.Collections.Generic;
using System.Text;

using Ordinates;
using Functions;

namespace DbMemory
{
    public class FunctionXY
    {
        private int _NumOrds;
        private double[] _Xa;
        private double[] _Ya;
        private IFunction<double> _FunctionPair;
        public FunctionXY()
        {
            _NumOrds = 0;
            _Xa = null;
            _Ya = null;
        }
        public FunctionXY(int n, double[] x, double[] y)
        {
            _NumOrds = n;
            _Xa = new double[n];
            _Ya = new double[n];
            for (int i = 0; i < n; i++)
            {
                _Xa[i] = x[i];
                _Ya[i] = y[i];
            }
        }
        public IFunction<double> CompressFunc(int numOrds, double[]x, double[] y, double[] xout, double[] yout)
        {
            _FunctionPair = null;
            if (numOrds > 0)
            {
                xout = new double[numOrds];
                yout = new double[numOrds];
                _Xa = new double[numOrds];
                _Ya = new double[numOrds];
                for(int i = 0; i < numOrds; i++)
                {
                    xout[i] = _Xa[i] = x[i];
                    yout[i] = _Ya[i] = y[i];
                }
                _FunctionPair = FunctionFactory.CreateFunction(xout, yout);
            }
            else
            {
                _NumOrds = 0;
                _FunctionPair = null;
                xout = null;
                yout = null;
            }
            return _FunctionPair;
        }
        public IFunction<double> GetCompressedFunction(double[] xout, double[] yout)
        {
            int numOrds = _NumOrds;
            //xout = new double[numOrds];
            //yout = new double[numOrds];
            for (int i = 0; i < numOrds; i++)
            {
                xout[i] = _Xa[i];
                yout[i] = _Ya[i];
            }
            _FunctionPair = FunctionFactory.CreateFunction(xout, yout);
            return _FunctionPair;
        }
    }
}
